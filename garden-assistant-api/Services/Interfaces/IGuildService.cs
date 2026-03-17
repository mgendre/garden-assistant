using GardenAssistant.DTOs.Guilds;

namespace GardenAssistant.Services.Interfaces;

public interface IGuildService
{
    Task<IEnumerable<GuildSummaryDto>> GetAllAsync(Guid userId);
    Task<GuildDetailDto?> GetByIdAsync(Guid id, Guid userId);
    Task<GuildDetailDto> CreateAsync(CreateGuildRequest request, Guid userId);
    Task<GuildDetailDto?> UpdateAsync(Guid id, UpdateGuildRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
