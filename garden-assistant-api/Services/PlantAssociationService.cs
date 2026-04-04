using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Companions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class PlantAssociationService(AppDbContext dbContext, ILogger<PlantAssociationService> logger) : IPlantAssociationService
{
    private const double BeneficialScore = 1.0;
    private const double NeutralScore = 0.1;
    private const double HarmfulScore = -1.5;
    private const double UnknownScore = 0.0;
    private const double SameFamilyMalus = -0.5;
    private const double WaterIncompatibilityMalus = -0.5;

    public async Task<CompanionSearchResultDto> GetCompanionRecommendationsAsync(
        List<Guid> selectedPlantIds, List<Guid>? centralPlantIds = null, double? minScore = null)
    {
        var candidates = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .Include(p => p.ParentPlant)
            .AsSplitQuery()
            .ToListAsync();

        if (candidates.Count == 0)
        {
            return new CompanionSearchResultDto([], [], [], [], [], []);
        }

        var varietyToParent = candidates
            .Where(c => c.ParentPlantId.HasValue)
            .ToDictionary(c => c.Id, c => c.ParentPlantId!.Value);

        var resolvedSelectedIds = selectedPlantIds
            .Select(id => varietyToParent.TryGetValue(id, out var parentId) ? parentId : id)
            .Distinct()
            .ToList();

        var resolvedCentralIds = (centralPlantIds ?? [])
            .Select(id => varietyToParent.TryGetValue(id, out var parentId) ? parentId : id)
            .Distinct()
            .ToHashSet();

        var allPlantIds = candidates.Select(c => c.Id).ToList();

        var associations = await dbContext.PlantAssociations
            .Where(pa => allPlantIds.Contains(pa.SourcePlantId)
                      && allPlantIds.Contains(pa.TargetPlantId))
            .ToListAsync();

        var associationLookup = BuildAssociationLookup(associations);

        var selectedPlantLookup = candidates
            .Where(c => resolvedSelectedIds.Contains(c.Id))
            .ToDictionary(c => c.Id, c => c);

        var scores = new Dictionary<Guid, double>();
        var scoreFlags = new Dictionary<Guid, (bool HasRootBonus, bool HasSameFamily, bool HasWaterIncompat)>();
        foreach (var candidate in candidates)
        {
            var resolvedCandidateId = varietyToParent.TryGetValue(candidate.Id, out var candidateParentId)
                ? candidateParentId
                : candidate.Id;

            var hasRootBonus = false;

            var pairScoreSum = resolvedSelectedIds.Sum(selectedId =>
            {
                var pairScore = ScorePair(resolvedCandidateId, selectedId, associationLookup);

                if (pairScore > 0 && resolvedCentralIds.Contains(selectedId))
                {
                    pairScore += 0.3;
                }

                if (selectedPlantLookup.TryGetValue(selectedId, out var selectedPlant)
                    && candidate.RootDepth != selectedPlant.RootDepth)
                {
                    pairScore += 0.1;
                    hasRootBonus = true;
                }

                return pairScore;
            });

            var malus = 0.0;
            var hasSameFamily = false;
            var hasWaterIncompat = false;

            foreach (var selectedId in resolvedSelectedIds)
            {
                if (!selectedPlantLookup.TryGetValue(selectedId, out var selectedPlant))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(candidate.Family)
                    && candidate.Family == selectedPlant.Family)
                {
                    malus += SameFamilyMalus;
                    hasSameFamily = true;
                }

                var waterDiff = Math.Abs((int)candidate.WaterNeeds - (int)selectedPlant.WaterNeeds);
                if (waterDiff >= 2)
                {
                    malus += WaterIncompatibilityMalus;
                    hasWaterIncompat = true;
                }
            }

            scores[candidate.Id] = Math.Round(pairScoreSum + malus, 2);
            scoreFlags[candidate.Id] = (hasRootBonus, hasSameFamily, hasWaterIncompat);
        }

        var goodCompanions = candidates
        .Select(p => new
        {
            Plant = p,
            Score = Math.Round(scores[p.Id], 2),
            ResolvedId = varietyToParent.TryGetValue(p.Id, out var pId) ? pId : p.Id,
            Flags = scoreFlags.GetValueOrDefault(p.Id),
        })
        .Select(c => new
        {
            c.Plant,
            c.Score,
            c.Flags,
            Mechanisms = CollectBeneficialMechanisms(c.ResolvedId, resolvedSelectedIds, associationLookup),
            HarmfulMechanisms = CollectHarmfulMechanisms(c.ResolvedId, resolvedSelectedIds, associationLookup),
            ResolvedLinkedIds = CollectLinkedPlantIds(c.ResolvedId, resolvedSelectedIds, associationLookup),
            Rating = CalculateRating(c.Score),
        })
        .Where(c => !minScore.HasValue || c.Score >= minScore.Value)
        .OrderByDescending(c => c.Rating)
        .ThenByDescending(c => c.Score)
        .ThenBy(c => c.Plant.Name, StringComparer.OrdinalIgnoreCase)
        .Select(c => new CompanionRecommendationDto(
            c.Plant.Id, c.Mechanisms, c.HarmfulMechanisms, c.ResolvedLinkedIds,
            c.Rating, c.Score, c.Flags.HasRootBonus, c.Flags.HasSameFamily, c.Flags.HasWaterIncompat))
        .ToList();

        var conflicts = BuildSelectedPlantConflicts(resolvedSelectedIds, associations);

        var intraGuildAssociations = associations
            .Where(a => resolvedSelectedIds.Contains(a.SourcePlantId) && resolvedSelectedIds.Contains(a.TargetPlantId))
            .ToList();

        var selectedPlantMechanisms = intraGuildAssociations
            .Select(a => a.Mechanism)
            .Distinct()
            .ToList();

        var selectedPlantsMechanisms = resolvedSelectedIds
            .Select(plantId => new PlantMechanismsDto(
                plantId,
                intraGuildAssociations
                    .Where(a => a.SourcePlantId == plantId || a.TargetPlantId == plantId)
                    .Select(a => a.Mechanism)
                    .Distinct()
                    .ToList()))
            .Where(p => p.Mechanisms.Count > 0)
            .ToList();

        var candidateById = candidates.ToDictionary(c => c.Id);

        var intrinsicMechanismsByPlant = resolvedSelectedIds
            .Select(plantId =>
            {
                var mechanisms = candidateById.TryGetValue(plantId, out var plant)
                    ? plant.IntrinsicMechanisms.Select(im => im.Mechanism).ToList()
                    : [];
                return new PlantMechanismsDto(plantId, mechanisms);
            })
            .Where(p => p.Mechanisms.Count > 0)
            .ToList();

        var selectedPlantAssociations = intraGuildAssociations
            .Select(a => new GuildAssociationDto(
                a.SourcePlantId,
                a.TargetPlantId,
                a.Mechanism,
                a.Effect,
                a.Notes))
            .OrderBy(a => a.SourcePlantId)
            .ThenBy(a => a.TargetPlantId)
            .ToList();

        return new CompanionSearchResultDto(goodCompanions, conflicts, selectedPlantMechanisms, selectedPlantsMechanisms, intrinsicMechanismsByPlant, selectedPlantAssociations);
    }

    private static List<SelectedPlantConflictDto> BuildSelectedPlantConflicts(
        List<Guid> selectedPlantIds,
        List<PlantAssociation> associations)
    {
        var conflicts = new List<SelectedPlantConflictDto>();
        for (var i = 0; i < selectedPlantIds.Count; i++)
        {
            for (var j = i + 1; j < selectedPlantIds.Count; j++)
            {
                var a = selectedPlantIds[i];
                var b = selectedPlantIds[j];
                var harmful = associations
                    .Where(assoc => assoc.Effect == AssociationEffect.Harmful
                        && ((assoc.SourcePlantId == a && assoc.TargetPlantId == b)
                         || (assoc.SourcePlantId == b && assoc.TargetPlantId == a)))
                    .Select(assoc => assoc.Mechanism)
                    .Distinct()
                    .ToList();

                if (harmful.Count > 0)
                {
                    conflicts.Add(new SelectedPlantConflictDto(a, b, harmful));
                }
            }
        }
        return conflicts;
    }

    private static double ScoreForEffect(AssociationEffect effect) => effect switch
    {
        AssociationEffect.Beneficial => BeneficialScore,
        AssociationEffect.Harmful => HarmfulScore,
        _ => NeutralScore
    };

    private static Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> BuildAssociationLookup(
        List<PlantAssociation> associations)
    {
        var lookup = new Dictionary<(Guid, Guid), List<(AssociationEffect, AssociationMechanism)>>();
        foreach (var a in associations)
        {
            var key = NormalizeKey(a.SourcePlantId, a.TargetPlantId);
            if (!lookup.TryGetValue(key, out var entries))
            {
                entries = [];
                lookup[key] = entries;
            }
            entries.Add((a.Effect, a.Mechanism));
        }
        return lookup;
    }

    private static double ScorePair(Guid plantA, Guid plantB,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        var key = NormalizeKey(plantA, plantB);
        if (!lookup.TryGetValue(key, out var entries))
        {
            return UnknownScore;
        }

        return entries.Sum(e => ScoreForEffect(e.Effect));
    }

    private static List<AssociationMechanism> CollectBeneficialMechanisms(Guid candidateId, List<Guid> selectedPlantIds,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        var mechanisms = new HashSet<AssociationMechanism>();
        foreach (var selectedId in selectedPlantIds)
        {
            var key = NormalizeKey(candidateId, selectedId);
            if (!lookup.TryGetValue(key, out var entries))
            {
                continue;
            }
            foreach (var e in entries.Where(e => e.Effect == AssociationEffect.Beneficial))
            {
                mechanisms.Add(e.Mechanism);
            }
        }
        return mechanisms.ToList();
    }

    private static List<AssociationMechanism> CollectHarmfulMechanisms(Guid candidateId, List<Guid> selectedPlantIds,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        var mechanisms = new HashSet<AssociationMechanism>();
        foreach (var selectedId in selectedPlantIds)
        {
            var key = NormalizeKey(candidateId, selectedId);
            if (!lookup.TryGetValue(key, out var entries))
            {
                continue;
            }
            foreach (var e in entries.Where(e => e.Effect == AssociationEffect.Harmful))
            {
                mechanisms.Add(e.Mechanism);
            }
        }
        return mechanisms.ToList();
    }

    private static List<Guid> CollectLinkedPlantIds(Guid candidateId, List<Guid> selectedPlantIds,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        var linked = new HashSet<Guid>();
        foreach (var selectedId in selectedPlantIds)
        {
            var key = NormalizeKey(candidateId, selectedId);
            if (lookup.TryGetValue(key, out var entries) && entries.Any(e => e.Effect == AssociationEffect.Beneficial))
            {
                linked.Add(selectedId);
            }
        }
        return linked.ToList();
    }

    private static int CalculateRating(double score)
    {
        if (score >= 1.5) { return 5; }
        if (score >= 1.0) { return 4; }
        if (score > 0) { return 3; }
        if (score >= 0) { return 2; }
        return 1;
    }

    private static (Guid, Guid) NormalizeKey(Guid a, Guid b) =>
        a.CompareTo(b) <= 0 ? (a, b) : (b, a);
}
