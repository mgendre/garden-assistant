using GardenAssistant.DTOs.Watering;

namespace GardenAssistant.Services.Watering;

public interface IGardenWateringService
{
    Task<WateringScheduleDto> GetScheduleAsync(Guid userId, Guid gardenId, int halfMonth);
}
