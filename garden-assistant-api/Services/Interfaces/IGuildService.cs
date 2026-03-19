using GardenAssistant.DTOs.Guilds;

namespace GardenAssistant.Services.Interfaces;

public interface IGuildService
{
    Task<IEnumerable<GuildDto>> GetAllAsync(Guid userId);
    Task<GuildDto?> GetByIdAsync(Guid id, Guid userId);
    Task<GuildDto> CreateAsync(CreateGuildRequest request, Guid userId);
    Task<GuildDto?> UpdateAsync(Guid id, UpdateGuildRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
