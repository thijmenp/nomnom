namespace NomNom.API.DTOs;

public record SpotDto(
    int Id,
    string Name,
    string Kind,
    string Cuisine,
    string Neighborhood,
    decimal? Latitude,
    decimal? Longitude,
    DateOnly VisitedOn,
    string? Dish,
    string? Note,
    decimal Rating,
    int Price,
    bool Returnable,
    string SwatchLight,
    string SwatchDark,
    string? PhotoPath
);

public record CreateSpotRequest(
    string Name,
    string Kind,
    string Cuisine,
    string Neighborhood,
    decimal? Latitude,
    decimal? Longitude,
    DateOnly VisitedOn,
    string? Dish,
    string? Note,
    decimal Rating,
    int Price,
    bool Returnable,
    string SwatchLight,
    string SwatchDark,
    string? PhotoPath
);

public record UpdateSpotRequest(
    string Name,
    string Kind,
    string Cuisine,
    string Neighborhood,
    decimal? Latitude,
    decimal? Longitude,
    DateOnly VisitedOn,
    string? Dish,
    string? Note,
    decimal Rating,
    int Price,
    bool Returnable,
    string SwatchLight,
    string SwatchDark,
    string? PhotoPath
);
