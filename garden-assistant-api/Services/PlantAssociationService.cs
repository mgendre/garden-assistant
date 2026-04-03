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
    private const double NeutralScore = 0.0;
    private const double HarmfulScore = -1.5;
    private const double UnknownScore = -0.1;

    public async Task<CompanionSearchResultDto> GetCompanionRecommendationsAsync(
        List<Guid> selectedPlantIds, double? minScore = null)
    {
        var candidates = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .Include(p => p.ParentPlant)
            .AsSplitQuery()
            .ToListAsync();

        if (candidates.Count == 0)
        {
            return new CompanionSearchResultDto([], [], [], [], [], [], []);
        }

        var varietyToParent = candidates
            .Where(c => c.ParentPlantId.HasValue)
            .ToDictionary(c => c.Id, c => c.ParentPlantId!.Value);

        var resolvedSelectedIds = selectedPlantIds
            .Select(id => varietyToParent.TryGetValue(id, out var parentId) ? parentId : id)
            .Distinct()
            .ToList();

        var allPlantIds = candidates.Select(c => c.Id).ToList();

        var associations = await dbContext.PlantAssociations
            .Where(pa => allPlantIds.Contains(pa.SourcePlantId)
                      && allPlantIds.Contains(pa.TargetPlantId))
            .ToListAsync();

        var associationLookup = BuildAssociationLookup(associations);

        var selectedRootDepths = candidates
            .Where(c => resolvedSelectedIds.Contains(c.Id))
            .ToDictionary(c => c.Id, c => c.RootDepth);

        var scores = new Dictionary<Guid, double>();
        foreach (var candidate in candidates)
        {
            var resolvedCandidateId = varietyToParent.TryGetValue(candidate.Id, out var candidateParentId)
                ? candidateParentId
                : candidate.Id;

            scores[candidate.Id] = resolvedSelectedIds.Sum(selectedId =>
            {
                var pairScore = ScorePair(resolvedCandidateId, selectedId, associationLookup);
                if (pairScore > 0
                    && selectedRootDepths.TryGetValue(selectedId, out var selectedDepth)
                    && candidate.RootDepth != selectedDepth)
                {
                    var boosted = pairScore * 1.10;
                    logger.LogDebug(
                        "Root depth bonus applied: {CandidateId} ({CandidateDepth}) vs {SelectedId} ({SelectedDepth}): {Before:F4} → {After:F4}",
                        candidate.Id, candidate.RootDepth, selectedId, selectedDepth, pairScore, boosted);
                    return boosted;
                }
                return pairScore;
            });
        }

        var goodCompanions = candidates
        .Select(p => new
        {
            Plant = p,
            Score = Math.Round(scores[p.Id], 2),
            ResolvedId = varietyToParent.TryGetValue(p.Id, out var pId) ? pId : p.Id,
        })
        .Select(c => new
        {
            c.Plant,
            c.Score,
            Mechanisms = CollectBeneficialMechanisms(c.ResolvedId, resolvedSelectedIds, associationLookup),
            ResolvedLinkedIds = CollectLinkedPlantIds(c.ResolvedId, resolvedSelectedIds, associationLookup)
        })
        .Where(c => !minScore.HasValue || c.Score >= minScore.Value)
        .OrderByDescending(c => c.Score)
        .ThenBy(c => c.Plant.Name, StringComparer.OrdinalIgnoreCase)
        .Select(c => new CompanionRecommendationDto(c.Plant.Id, c.Mechanisms, c.ResolvedLinkedIds))
        .ToList();

        var plantsToAvoid = BuildPlantsToAvoid(resolvedSelectedIds, associations);

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

        return new CompanionSearchResultDto(goodCompanions, plantsToAvoid, conflicts, selectedPlantMechanisms, selectedPlantsMechanisms, intrinsicMechanismsByPlant, selectedPlantAssociations);
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

    private static List<CompanionRecommendationDto> BuildPlantsToAvoid(
        List<Guid> selectedPlantIds,
        List<PlantAssociation> associations)
    {
        var harmfulByPlant = new Dictionary<Guid, (HashSet<AssociationMechanism> Mechanisms, HashSet<Guid> LinkedIds)>();

        foreach (var a in associations.Where(a => a.Effect == AssociationEffect.Harmful))
        {
            Guid candidateId;
            Guid linkedId;
            if (selectedPlantIds.Contains(a.SourcePlantId) && !selectedPlantIds.Contains(a.TargetPlantId))
            {
                candidateId = a.TargetPlantId;
                linkedId = a.SourcePlantId;
            }
            else if (selectedPlantIds.Contains(a.TargetPlantId) && !selectedPlantIds.Contains(a.SourcePlantId))
            {
                candidateId = a.SourcePlantId;
                linkedId = a.TargetPlantId;
            }
            else
            {
                continue;
            }

            if (!harmfulByPlant.TryGetValue(candidateId, out var entry))
            {
                entry = ([], []);
                harmfulByPlant[candidateId] = entry;
            }
            entry.Mechanisms.Add(a.Mechanism);
            entry.LinkedIds.Add(linkedId);
        }

        return harmfulByPlant
            .Select(kv => new CompanionRecommendationDto(kv.Key, kv.Value.Mechanisms.ToList(), kv.Value.LinkedIds.ToList()))
            .OrderBy(p => p.PlantId)
            .ToList();
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

    private static (Guid, Guid) NormalizeKey(Guid a, Guid b) =>
        a.CompareTo(b) <= 0 ? (a, b) : (b, a);
}
