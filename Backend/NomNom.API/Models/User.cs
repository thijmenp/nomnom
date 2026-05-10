namespace NomNom.API.Models;

public class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? City { get; set; }
    public DateTime JoinedAt { get; set; }

    public ICollection<Spot> Spots { get; set; } = [];
    public ICollection<Collection> Collections { get; set; } = [];
}
