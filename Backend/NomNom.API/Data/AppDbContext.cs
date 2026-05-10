using Microsoft.EntityFrameworkCore;
using NomNom.API.Models;

namespace NomNom.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Spot> Spots => Set<Spot>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<CollectionSpot> CollectionSpots => Set<CollectionSpot>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        // Composite PK for join table
        model.Entity<CollectionSpot>()
            .HasKey(cs => new { cs.CollectionId, cs.SpotId });

        // Store SpotKind as a string so migrations are readable
        model.Entity<Spot>()
            .Property(s => s.Kind)
            .HasConversion<string>();

        // Rating precision: 3 digits, 1 decimal (e.g. 10.0)
        model.Entity<Spot>()
            .Property(s => s.Rating)
            .HasPrecision(3, 1);

        // Lat/lng precision: 9 digits, 6 decimal places
        model.Entity<Spot>()
            .Property(s => s.Latitude)
            .HasPrecision(9, 6);

        model.Entity<Spot>()
            .Property(s => s.Longitude)
            .HasPrecision(9, 6);
    }
}
