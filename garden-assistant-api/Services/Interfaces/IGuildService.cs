using GardenAssistant.DTOs;

namespace GardenAssistant.Services.Interfaces;

public interface IGuildService
{
    Task<IEnumerable<GuildSummaryDto>> GetAllAsync();
    Task<GuildDetailDto?> GetByIdAsync(Guid id);
}
