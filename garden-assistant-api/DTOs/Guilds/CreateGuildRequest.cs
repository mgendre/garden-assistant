using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Guilds;

public record CreateGuildRequest(
    [Required][MaxLength(256)] string Name,
    [MaxLength(2000)] string? Description,
    [Required] List<Guid> PlantIds);
