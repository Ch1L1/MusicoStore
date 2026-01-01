namespace MusicoStore.Domain.Records;

public record ProductFilterCriteria(
    string? Name,
    string? Description,
    decimal? MaxPrice,
    string? Category,
    string? Manufacturer
);
