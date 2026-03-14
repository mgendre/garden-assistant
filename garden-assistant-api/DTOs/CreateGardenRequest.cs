using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs;

// TODO(auth): UserId must be removed from this DTO and sourced from
// HttpContext.User claims once authentication is implemented.
public record CreateGardenRequest(
    [Required][MaxLength(256)] string Name,
    [MaxLength(2000)] string? Description,
    Guid UserId);
