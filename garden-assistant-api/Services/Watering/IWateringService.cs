using GardenAssistant.DTOs.Watering;

namespace GardenAssistant.Services.Watering;

public interface IWateringService
{
    Task<WateringTodayDto> GetWateringTodayAsync(Guid userId, DateOnly today);
    Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source);
}
