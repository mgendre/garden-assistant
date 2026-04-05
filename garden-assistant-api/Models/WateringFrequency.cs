namespace GardenAssistant.Models;

public record WateringFrequency(int TimesPerWeek, DayOfWeek[] RecommendedDays);
