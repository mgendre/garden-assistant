using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Plants;
using Microsoft.EntityFrameworkCore;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class PlantService(AppDbContext dbContext) : IPlantService
{
    public async Task<List<PlantDto>> GetAllAsync()
    {
        var plants = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .Include(p => p.SoilTypes)
            .Include(p => p.HarvestReadiness)
                .ThenInclude(hr => hr!.Criteria)
            .Include(p => p.Actions)
            .Include(p => p.ParentPlant)
                .ThenInclude(pp => pp!.IntrinsicMechanisms)
            .Include(p => p.ParentPlant)
                .ThenInclude(pp => pp!.SoilTypes)
            .Include(p => p.ParentPlant)
                .ThenInclude(pp => pp!.HarvestReadiness)
                    .ThenInclude(hr => hr!.Criteria)
            .Include(p => p.ParentPlant)
                .ThenInclude(pp => pp!.Actions)
            .Include(p => p.Varieties)
            .AsSplitQuery()
            .OrderBy(p => p.Name)
            .ToListAsync();

        return plants.Select(p => ToDto(p)).ToList();
    }

    internal static PlantDto ToDto(Plant plant)
    {
        var isVariety = plant.ParentPlantId is not null;
        var parent = plant.ParentPlant;

        var family = isVariety ? (plant.Family ?? parent?.Family) : plant.Family;
        var genus = isVariety ? (plant.Genus ?? parent?.Genus) : plant.Genus;
        var scientificName = isVariety ? (plant.ScientificName ?? parent?.ScientificName) : plant.ScientificName;
        var description = isVariety ? (plant.Description ?? parent?.Description) : plant.Description;

        var lifeCycle = isVariety ? ResolveEnum(plant.LifeCycle, parent?.LifeCycle) : plant.LifeCycle;
        var heightAtMaturityCm = isVariety ? (plant.HeightAtMaturityCm ?? parent?.HeightAtMaturityCm) : plant.HeightAtMaturityCm;
        var rootDepth = isVariety ? ResolveEnum(plant.RootDepth, parent?.RootDepth) : plant.RootDepth;
        var sunRequirement = isVariety ? ResolveEnum(plant.SunRequirement, parent?.SunRequirement) : plant.SunRequirement;
        var waterNeeds = isVariety ? ResolveEnum(plant.WaterNeeds, parent?.WaterNeeds) : plant.WaterNeeds;
        var propagationMethod = isVariety ? ResolveEnum(plant.PropagationMethod, parent?.PropagationMethod) : plant.PropagationMethod;
        var maxAltitudeM = isVariety ? (plant.MaxAltitudeM ?? parent?.MaxAltitudeM) : plant.MaxAltitudeM;
        var frostSensitive = isVariety ? (plant.FrostSensitive || (parent?.FrostSensitive ?? false)) : plant.FrostSensitive;

        var mechanisms = isVariety
            ? (parent?.IntrinsicMechanisms.Select(im => im.Mechanism).ToList() ?? [])
            : plant.IntrinsicMechanisms.Select(im => im.Mechanism).ToList();

        var soilTypes = isVariety
            ? (plant.SoilTypes.Count > 0
                ? plant.SoilTypes.Select(st => st.SoilType.ToString()).ToList()
                : parent?.SoilTypes.Select(st => st.SoilType.ToString()).ToList() ?? [])
            : plant.SoilTypes.Select(st => st.SoilType.ToString()).ToList();

        var optimalPhMin = isVariety ? (plant.OptimalPhMin ?? parent?.OptimalPhMin) : plant.OptimalPhMin;
        var optimalPhMax = isVariety ? (plant.OptimalPhMax ?? parent?.OptimalPhMax) : plant.OptimalPhMax;

        var varieties = plant.Varieties
            .Select(v => new PlantSummaryDto(v.Id, v.Name, v.ScientificName))
            .ToList();

        var hr = plant.HarvestReadiness ?? (isVariety ? parent?.HarvestReadiness : null);
        var harvestReadiness = hr is not null
            ? new HarvestReadinessDto(
                hr.Description,
                hr.DaysFromTransplant,
                hr.DaysFromSowing,
                hr.Criteria.Select(c => new HarvestReadinessCriterionDto(c.CriterionType, c.Description)).ToList())
            : null;

        var waterAmountMl = isVariety ? (plant.WaterAmountMl ?? parent?.WaterAmountMl) : plant.WaterAmountMl;

        return new PlantDto(
            plant.Id,
            plant.Name,
            scientificName,
            description,
            family,
            genus,
            lifeCycle,
            heightAtMaturityCm,
            rootDepth,
            sunRequirement,
            waterNeeds,
            propagationMethod,
            maxAltitudeM,
            frostSensitive,
            mechanisms,
            soilTypes,
            optimalPhMin,
            optimalPhMax,
            isVariety,
            isVariety ? plant.ParentPlantId : null,
            isVariety ? parent?.Name : null,
            varieties,
            harvestReadiness,
            MapActions(plant, isVariety, parent),
            waterAmountMl
        );
    }

    private static List<PlantActionDto> MapActions(Plant plant, bool isVariety, Plant? parent)
    {
        var actions = plant.Actions.Count > 0 ? plant.Actions : (isVariety ? parent?.Actions ?? [] : []);
        return actions.Select(a => new PlantActionDto(a.Id, a.ActionType, a.HalfMonthStart, a.HalfMonthEnd, a.Notes)).ToList();
    }

    private static T ResolveEnum<T>(T varietyValue, T? parentValue) where T : struct, Enum
    {
        return EqualityComparer<T>.Default.Equals(varietyValue, default) && parentValue.HasValue
            ? parentValue.Value
            : varietyValue;
    }
}
