using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Models;

namespace GardenAssistant.Services.Watering;

public interface IWateringCalculator
{
    WateringFrequency CalculateFrequency(
        WaterNeeds waterNeeds,
        int halfMonth,
        SoilType? soilType = null,
        bool hasMulch = false);
}
