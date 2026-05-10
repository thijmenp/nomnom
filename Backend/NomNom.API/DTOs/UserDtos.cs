namespace NomNom.API.DTOs;

public record UserDto(
    int Id,
    string DisplayName,
    string? City,
    DateTime JoinedAt
);

public record KindStatDto(string Kind, int Count, decimal AverageRating);

public record NeighborhoodStatDto(string Name, int Visits);

public record UserStatsDto(
    int TotalEntries,
    decimal AverageRating,
    decimal ReturningPercent,
    int CityCount,
    IEnumerable<KindStatDto> ByKind,
    IEnumerable<NeighborhoodStatDto> TopNeighborhoods
);
