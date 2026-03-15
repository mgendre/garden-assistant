using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public interface IPlantAssociationService
{
    Task<IEnumerable<PlantAssociationDto>> GetForPlantAsync(Guid plantId);
    Task<CompanionSearchResultDto> GetCompanionRecommendationsAsync(List<Guid> selectedPlantIds);
    Task<PlantAssociationDto> CreateAsync(CreatePlantAssociationRequest request);
    Task<bool> DeleteAsync(Guid id);
}

public class PlantAssociationService(AppDbContext dbContext) : IPlantAssociationService
{
    private const int MaxRecommendations = 10;
    private const double BeneficialScore = 1.0;
    private const double NeutralScore = 0.0;
    private const double HarmfulScore = -1.5;
    private const double UnknownScore = -0.1;

    public async Task<IEnumerable<PlantAssociationDto>> GetForPlantAsync(Guid plantId)
    {
        return await dbContext.PlantAssociations
            .Where(pa => pa.SourcePlantId == plantId || pa.TargetPlantId == plantId)
            .Select(pa => ToDto(pa))
            .ToListAsync();
    }

    public async Task<CompanionSearchResultDto> GetCompanionRecommendationsAsync(
        List<Guid> selectedPlantIds)
    {
        var candidates = await dbContext.Plants
            .Where(p => !selectedPlantIds.Contains(p.Id))
            .ToListAsync();

        if (candidates.Count == 0)
        {
            return new CompanionSearchResultDto([], [], []);
        }

        var allRelevantPlantIds = selectedPlantIds
            .Concat(candidates.Select(c => c.Id))
            .ToList();

        var associations = await dbContext.PlantAssociations
            .Where(pa => allRelevantPlantIds.Contains(pa.SourcePlantId)
                      && allRelevantPlantIds.Contains(pa.TargetPlantId))
            .ToListAsync();

        var associationLookup = BuildAssociationLookup(associations);

        var allCandidates = new List<Plant>(candidates);
        candidates.RemoveAll(c => HasHarmfulAssociation(c.Id, selectedPlantIds, associationLookup));

        var baseScores = new Dictionary<Guid, double>();
        foreach (var candidate in candidates)
        {
            var score = 0.0;
            foreach (var selectedId in selectedPlantIds)
            {
                score += ScorePair(candidate.Id, selectedId, associationLookup);
            }
            baseScores[candidate.Id] = score;
        }

        var selected = new List<Plant>();
        var remaining = new List<Plant>(candidates);

        while (selected.Count < MaxRecommendations && remaining.Count > 0)
        {
            Plant? best = null;
            var bestScore = double.MinValue;

            foreach (var candidate in remaining)
            {
                var totalScore = baseScores[candidate.Id];
                foreach (var alreadyPicked in selected)
                {
                    totalScore += ScorePair(candidate.Id, alreadyPicked.Id, associationLookup);
                }

                if (totalScore > bestScore)
                {
                    bestScore = totalScore;
                    best = candidate;
                }
            }

            selected.Add(best!);
            remaining.Remove(best!);
        }

        var guildLookup = await BuildGuildLookup(selected.Select(p => p.Id).ToList(), selectedPlantIds);

        var goodCompanions = selected.Select(p =>
        {
            var score = Math.Round(baseScores[p.Id] + InterCandidateScore(p.Id, selected, associationLookup), 2);
            var mechanisms = CollectBeneficialMechanisms(p.Id, selectedPlantIds, associationLookup);
            var guilds = guildLookup.GetValueOrDefault(p.Id, []);
            return new CompanionRecommendationDto(p.Id, p.Name, p.ScientificName, score, mechanisms, guilds);
        })
        .OrderByDescending(c => c.Score)
        .ToList();

        var plantsToAvoid = BuildPlantsToAvoid(allCandidates, selectedPlantIds, associations);

        var selectedPlants = await dbContext.Plants
            .Where(p => selectedPlantIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
        var conflicts = BuildSelectedPlantConflicts(selectedPlantIds, selectedPlants, associations);

        return new CompanionSearchResultDto(goodCompanions, plantsToAvoid, conflicts);
    }

    public async Task<PlantAssociationDto> CreateAsync(CreatePlantAssociationRequest request)
    {
        var association = new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = request.SourcePlantId,
            TargetPlantId = request.TargetPlantId,
            Mechanism = request.Mechanism,
            Effect = request.Effect,
            DistanceEffect = request.DistanceEffect,
            ConfidenceLevel = request.ConfidenceLevel,
            Notes = request.Notes
        };

        dbContext.PlantAssociations.Add(association);
        await dbContext.SaveChangesAsync();

        return ToDto(association);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var association = await dbContext.PlantAssociations.FindAsync(id);
        if (association is null)
        {
            return false;
        }

        dbContext.PlantAssociations.Remove(association);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<Dictionary<Guid, List<GuildInfoDto>>> BuildGuildLookup(
        List<Guid> recommendedPlantIds, List<Guid> selectedPlantIds)
    {
        var allPlantIds = recommendedPlantIds.Concat(selectedPlantIds).ToList();

        var guildPlants = await dbContext.GuildPlants
            .Where(gp => allPlantIds.Contains(gp.PlantId))
            .ToListAsync();

        var guildIds = guildPlants.Select(gp => gp.GuildId).Distinct().ToList();
        var guilds = await dbContext.Guilds
            .Where(g => guildIds.Contains(g.Id))
            .ToDictionaryAsync(g => g.Id);

        var selectedGuildIds = guildPlants
            .Where(gp => selectedPlantIds.Contains(gp.PlantId))
            .Select(gp => gp.GuildId)
            .ToHashSet();

        var lookup = new Dictionary<Guid, List<GuildInfoDto>>();
        foreach (var gp in guildPlants)
        {
            if (!recommendedPlantIds.Contains(gp.PlantId))
            {
                continue;
            }
            if (!selectedGuildIds.Contains(gp.GuildId))
            {
                continue;
            }
            if (!guilds.TryGetValue(gp.GuildId, out var guild))
            {
                continue;
            }

            if (!lookup.TryGetValue(gp.PlantId, out var guildList))
            {
                guildList = [];
                lookup[gp.PlantId] = guildList;
            }
            guildList.Add(new GuildInfoDto(guild.Id, guild.Name, guild.Description));
        }

        return lookup;
    }

    private static List<SelectedPlantConflictDto> BuildSelectedPlantConflicts(
        List<Guid> selectedPlantIds,
        Dictionary<Guid, Plant> selectedPlants,
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

                if (harmful.Count > 0 && selectedPlants.TryGetValue(a, out var plantA) && selectedPlants.TryGetValue(b, out var plantB))
                {
                    conflicts.Add(new SelectedPlantConflictDto(a, plantA.Name, b, plantB.Name, harmful));
                }
            }
        }
        return conflicts;
    }

    private static List<PlantToAvoidDto> BuildPlantsToAvoid(
        List<Plant> candidates,
        List<Guid> selectedPlantIds,
        List<PlantAssociation> associations)
    {
        var harmfulByPlant = new Dictionary<Guid, HashSet<AssociationMechanism>>();

        foreach (var a in associations)
        {
            if (a.Effect != AssociationEffect.Harmful) continue;

            Guid candidateId;
            if (selectedPlantIds.Contains(a.SourcePlantId) && !selectedPlantIds.Contains(a.TargetPlantId))
                candidateId = a.TargetPlantId;
            else if (selectedPlantIds.Contains(a.TargetPlantId) && !selectedPlantIds.Contains(a.SourcePlantId))
                candidateId = a.SourcePlantId;
            else
                continue;

            if (!harmfulByPlant.TryGetValue(candidateId, out var mechanisms))
            {
                mechanisms = [];
                harmfulByPlant[candidateId] = mechanisms;
            }
            mechanisms.Add(a.Mechanism);
        }

        var candidateMap = candidates.ToDictionary(c => c.Id);

        return harmfulByPlant
            .Where(kv => candidateMap.ContainsKey(kv.Key))
            .Select(kv =>
            {
                var plant = candidateMap[kv.Key];
                return new PlantToAvoidDto(plant.Id, plant.Name, plant.ScientificName, kv.Value.ToList());
            })
            .OrderByDescending(p => p.Mechanisms.Count)
            .ToList();
    }

    private static double ScoreForEffect(AssociationEffect effect) => effect switch
    {
        AssociationEffect.Beneficial => BeneficialScore,
        AssociationEffect.Harmful => HarmfulScore,
        AssociationEffect.Neutral => NeutralScore,
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
            return UnknownScore;

        return entries.Sum(e => ScoreForEffect(e.Effect));
    }

    private static bool HasHarmfulAssociation(Guid candidateId, List<Guid> selectedPlantIds,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        return selectedPlantIds.Any(selectedId =>
        {
            var key = NormalizeKey(candidateId, selectedId);
            return lookup.TryGetValue(key, out var entries)
                && entries.Any(e => e.Effect == AssociationEffect.Harmful);
        });
    }

    private static List<AssociationMechanism> CollectBeneficialMechanisms(Guid candidateId, List<Guid> selectedPlantIds,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        var mechanisms = new HashSet<AssociationMechanism>();
        foreach (var selectedId in selectedPlantIds)
        {
            var key = NormalizeKey(candidateId, selectedId);
            if (!lookup.TryGetValue(key, out var entries)) continue;
            foreach (var e in entries)
                if (e.Effect == AssociationEffect.Beneficial)
                    mechanisms.Add(e.Mechanism);
        }
        return mechanisms.ToList();
    }

    private static double InterCandidateScore(Guid plantId, List<Plant> selected,
        Dictionary<(Guid, Guid), List<(AssociationEffect Effect, AssociationMechanism Mechanism)>> lookup)
    {
        return selected
            .Where(s => s.Id != plantId)
            .Sum(s => ScorePair(plantId, s.Id, lookup));
    }

    private static (Guid, Guid) NormalizeKey(Guid a, Guid b) =>
        a.CompareTo(b) <= 0 ? (a, b) : (b, a);

    private static PlantAssociationDto ToDto(PlantAssociation pa) => new(
        pa.Id,
        pa.SourcePlantId,
        pa.TargetPlantId,
        pa.Mechanism,
        pa.Effect,
        pa.DistanceEffect,
        pa.ConfidenceLevel,
        pa.Notes
    );
}
