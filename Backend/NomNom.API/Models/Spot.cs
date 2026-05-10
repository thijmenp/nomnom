namespace NomNom.API.Models;

public class Spot
{
    public int Id { get; set; }

    // What and where
    public string Name { get; set; } = string.Empty;
    public SpotKind Kind { get; set; }
    public string Cuisine { get; set; } = string.Empty;
    public string Neighborhood { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    // The visit
    public DateOnly VisitedOn { get; set; }
    public string? Dish { get; set; }
    public string? Note { get; set; }

    // Scores
    public decimal Rating { get; set; }  // 0.0 – 10.0
    public int Price { get; set; }       // 1 (€) – 4 (€€€€)
    public bool Returnable { get; set; }

    // Visual identity (Polaroid gradient)
    public string SwatchLight { get; set; } = string.Empty;
    public string SwatchDark { get; set; } = string.Empty;

    // Media
    public string? PhotoPath { get; set; }

    // Relations
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<CollectionSpot> CollectionSpots { get; set; } = [];
}
