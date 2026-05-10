namespace NomNom.API.DTOs;

public record SwatchPair(string Light, string Dark);

public record CollectionDto(
    int Id,
    string Name,
    string Subtitle,
    int SpotCount,
    IEnumerable<SwatchPair> Swatches
);

public record CollectionDetailDto(
    int Id,
    string Name,
    string Subtitle,
    IEnumerable<SpotDto> Spots
);

public record CreateCollectionRequest(string Name, string Subtitle);

public record UpdateCollectionRequest(string Name, string Subtitle);
