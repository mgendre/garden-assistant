using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Gardens;

public record UpdateGardenRequest(
    [Required][MaxLength(256)] string Name,
    [MaxLength(2000)] string? Description);
