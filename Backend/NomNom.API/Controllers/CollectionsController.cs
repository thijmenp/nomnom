using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NomNom.API.Data;
using NomNom.API.DTOs;
using NomNom.API.Models;

namespace NomNom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController(AppDbContext db) : ControllerBase
{
    // GET /api/collections
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var collections = await db.Collections
            .Include(c => c.CollectionSpots)
                .ThenInclude(cs => cs.Spot)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(collections.Select(c => new CollectionDto(
            c.Id,
            c.Name,
            c.Subtitle,
            c.CollectionSpots.Count,
            c.CollectionSpots.Take(3).Select(cs => new SwatchPair(cs.Spot.SwatchLight, cs.Spot.SwatchDark))
        )));
    }

    // GET /api/collections/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await db.Collections
            .Include(c => c.CollectionSpots)
                .ThenInclude(cs => cs.Spot)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (c is null) return NotFound();

        return Ok(new CollectionDetailDto(
            c.Id,
            c.Name,
            c.Subtitle,
            c.CollectionSpots.Select(cs => SpotToDto(cs.Spot))
        ));
    }

    // POST /api/collections
    [HttpPost]
    public async Task<IActionResult> Create(CreateCollectionRequest req)
    {
        var collection = new Collection
        {
            Name     = req.Name,
            Subtitle = req.Subtitle,
            UserId   = 1, // replaced once auth is in place
        };
        db.Collections.Add(collection);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = collection.Id },
            new CollectionDto(collection.Id, collection.Name, collection.Subtitle, 0, []));
    }

    // PUT /api/collections/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCollectionRequest req)
    {
        var collection = await db.Collections.FindAsync(id);
        if (collection is null) return NotFound();

        collection.Name     = req.Name;
        collection.Subtitle = req.Subtitle;
        await db.SaveChangesAsync();

        return Ok(new CollectionDto(collection.Id, collection.Name, collection.Subtitle, 0, []));
    }

    // DELETE /api/collections/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var collection = await db.Collections.FindAsync(id);
        if (collection is null) return NotFound();
        db.Collections.Remove(collection);
        await db.SaveChangesAsync();
        return NoContent();
    }

    // POST /api/collections/{id}/spots/{spotId}
    [HttpPost("{id}/spots/{spotId}")]
    public async Task<IActionResult> AddSpot(int id, int spotId)
    {
        if (!await db.Collections.AnyAsync(c => c.Id == id)) return NotFound("Collection not found.");
        if (!await db.Spots.AnyAsync(s => s.Id == spotId)) return NotFound("Spot not found.");

        var exists = await db.CollectionSpots
            .AnyAsync(cs => cs.CollectionId == id && cs.SpotId == spotId);
        if (exists) return Conflict("Spot is already in this collection.");

        db.CollectionSpots.Add(new CollectionSpot { CollectionId = id, SpotId = spotId });
        await db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/collections/{id}/spots/{spotId}
    [HttpDelete("{id}/spots/{spotId}")]
    public async Task<IActionResult> RemoveSpot(int id, int spotId)
    {
        var link = await db.CollectionSpots.FindAsync(id, spotId);
        if (link is null) return NotFound();
        db.CollectionSpots.Remove(link);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static SpotDto SpotToDto(Spot s) => new(
        s.Id, s.Name, s.Kind.ToString(), s.Cuisine, s.Neighborhood,
        s.Latitude, s.Longitude, s.VisitedOn, s.Dish, s.Note,
        s.Rating, s.Price, s.Returnable, s.SwatchLight, s.SwatchDark, s.PhotoPath
    );
}
