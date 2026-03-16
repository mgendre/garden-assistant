namespace GardenAssistant.DTOs;

public record PaginatedResult<T>(
    IEnumerable<T> Items,
    int TotalCount
);
