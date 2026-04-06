using System.ComponentModel.DataAnnotations;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Beds;

public record CreateBedRequest([StringLength(100)] string? Name, SoilType? SoilType, bool HasMulch = false);
