using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs;

public record CreatePlantingRequest(
    Guid GardenId,
    [Required][MaxLength(256)] string Name,
    [MaxLength(2000)] string? Description,
    DateOnly? PlannedDate
);
