namespace NomNom.API.Models;

public class Collection
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CollectionSpot> CollectionSpots { get; set; } = [];
}
