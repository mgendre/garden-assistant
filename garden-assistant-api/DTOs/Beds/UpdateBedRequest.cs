using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Beds;

public record UpdateBedRequest([StringLength(100)] string? Name);
