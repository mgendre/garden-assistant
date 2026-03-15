using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class PlantAssociationService(AppDbContext dbContext)
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

    public async Task<List<CompanionRecommendationDto>> GetCompanionRecommendationsAsync(
        List<Guid> selectedPlantIds)
    {
        var candidates = await dbContext.Plants
            .Where(p => !selectedPlantIds.Contains(p.Id))
            .ToListAsync();

        if (candidates.Count == 0)
            return [];

        var allRelevantPlantIds = selectedPlantIds
            .Concat(candidates.Select(c => c.Id))
            .ToList();

        var associations = await dbContext.PlantAssociations
            .Where(pa => allRelevantPlantIds.Contains(pa.SourcePlantId)
                      && allRelevantPlantIds.Contains(pa.TargetPlantId))
            .ToListAsync();

        var associationLookup = BuildAssociationLookup(associations);

        var baseScores = new Dictionary<Guid, double>();
        foreach (var candidate in candidates)
        {
            var score = 0.0;
            foreach (var selectedId in selectedPlantIds)
                score += ScorePair(candidate.Id, selectedId, associationLookup);
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
                    totalScore += ScorePair(candidate.Id, alreadyPicked.Id, associationLookup);

                if (totalScore > bestScore)
                {
                    bestScore = totalScore;
                    best = candidate;
                }
            }

            selected.Add(best!);
            remaining.Remove(best!);
        }

        return selected.Select(p => new CompanionRecommendationDto(
            p.Id,
            p.Name,
            p.ScientificName,
            Math.Round(baseScores[p.Id] + InterCandidateScore(p.Id, selected, associationLookup), 2)
        )).ToList();
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
        if (association is null) return false;

        dbContext.PlantAssociations.Remove(association);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static double ScoreForEffect(AssociationEffect effect) => effect switch
    {
        AssociationEffect.Beneficial => BeneficialScore,
        AssociationEffect.Harmful => HarmfulScore,
        AssociationEffect.Neutral => NeutralScore,
        _ => NeutralScore
    };

    private static Dictionary<(Guid, Guid), List<AssociationEffect>> BuildAssociationLookup(
        List<PlantAssociation> associations)
    {
        var lookup = new Dictionary<(Guid, Guid), List<AssociationEffect>>();
        foreach (var a in associations)
        {
            var key = NormalizeKey(a.SourcePlantId, a.TargetPlantId);
            if (!lookup.TryGetValue(key, out var effects))
            {
                effects = [];
                lookup[key] = effects;
            }
            effects.Add(a.Effect);
        }
        return lookup;
    }

    private static double ScorePair(Guid plantA, Guid plantB,
        Dictionary<(Guid, Guid), List<AssociationEffect>> lookup)
    {
        var key = NormalizeKey(plantA, plantB);
        if (!lookup.TryGetValue(key, out var effects))
            return UnknownScore;

        return effects.Sum(ScoreForEffect);
    }

    private static double InterCandidateScore(Guid plantId, List<Plant> selected,
        Dictionary<(Guid, Guid), List<AssociationEffect>> lookup)
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
