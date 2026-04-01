using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Beds;

public record CreateBedRequest([StringLength(100)] string? Name);
