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
            .Include(p => p.ParentPlant)
                .ThenInclude(pp => pp!.IntrinsicMechanisms)
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

        var varieties = plant.Varieties
            .Select(v => new PlantSummaryDto(v.Id, v.Name, v.ScientificName))
            .ToList();

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
            isVariety,
            isVariety ? plant.ParentPlantId : null,
            isVariety ? parent?.Name : null,
            varieties
        );
    }

    private static T ResolveEnum<T>(T varietyValue, T? parentValue) where T : struct, Enum
    {
        return EqualityComparer<T>.Default.Equals(varietyValue, default) && parentValue.HasValue
            ? parentValue.Value
            : varietyValue;
    }
}
