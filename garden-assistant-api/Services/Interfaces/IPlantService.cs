using GardenAssistant.DTOs.Plants;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantService
{
    Task<List<PlantDto>> GetAllAsync();
}
