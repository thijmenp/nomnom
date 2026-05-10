namespace NomNom.API.Models;

public class CollectionSpot
{
    public int CollectionId { get; set; }
    public Collection Collection { get; set; } = null!;

    public int SpotId { get; set; }
    public Spot Spot { get; set; } = null!;
}
