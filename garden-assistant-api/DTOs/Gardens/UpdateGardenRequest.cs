using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Gardens;

public record UpdateGardenRequest(
    [Required] [StringLength(100, MinimumLength = 1)] string Name,
    [StringLength(2000)] string? Description);
