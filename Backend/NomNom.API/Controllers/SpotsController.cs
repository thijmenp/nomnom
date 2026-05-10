using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NomNom.API.Data;
using NomNom.API.DTOs;
using NomNom.API.Models;

namespace NomNom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpotsController(AppDbContext db) : ControllerBase
{
    // GET /api/spots?kind=coffee
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? kind)
    {
        var query = db.Spots.AsQueryable();

        if (kind is not null && Enum.TryParse<SpotKind>(kind, ignoreCase: true, out var k))
            query = query.Where(s => s.Kind == k);

        var spots = await query.OrderByDescending(s => s.VisitedOn).ToListAsync();
        return Ok(spots.Select(ToDto));
    }

    // GET /api/spots/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var spot = await db.Spots.FindAsync(id);
        return spot is null ? NotFound() : Ok(ToDto(spot));
    }

    // POST /api/spots
    [HttpPost]
    public async Task<IActionResult> Create(CreateSpotRequest req)
    {
        if (!Enum.TryParse<SpotKind>(req.Kind, ignoreCase: true, out var kind))
            return BadRequest("Invalid kind value. Use: coffee, lunch, or dinner.");

        var spot = new Spot
        {
            Name        = req.Name,
            Kind        = kind,
            Cuisine     = req.Cuisine,
            Neighborhood = req.Neighborhood,
            Latitude    = req.Latitude,
            Longitude   = req.Longitude,
            VisitedOn   = req.VisitedOn,
            Dish        = req.Dish,
            Note        = req.Note,
            Rating      = req.Rating,
            Price       = req.Price,
            Returnable  = req.Returnable,
            SwatchLight = req.SwatchLight,
            SwatchDark  = req.SwatchDark,
            PhotoPath   = req.PhotoPath,
            UserId      = 1, // replaced once auth is in place
        };

        db.Spots.Add(spot);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = spot.Id }, ToDto(spot));
    }

    // PUT /api/spots/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateSpotRequest req)
    {
        var spot = await db.Spots.FindAsync(id);
        if (spot is null) return NotFound();

        if (!Enum.TryParse<SpotKind>(req.Kind, ignoreCase: true, out var kind))
            return BadRequest("Invalid kind value. Use: coffee, lunch, or dinner.");

        spot.Name        = req.Name;
        spot.Kind        = kind;
        spot.Cuisine     = req.Cuisine;
        spot.Neighborhood = req.Neighborhood;
        spot.Latitude    = req.Latitude;
        spot.Longitude   = req.Longitude;
        spot.VisitedOn   = req.VisitedOn;
        spot.Dish        = req.Dish;
        spot.Note        = req.Note;
        spot.Rating      = req.Rating;
        spot.Price       = req.Price;
        spot.Returnable  = req.Returnable;
        spot.SwatchLight = req.SwatchLight;
        spot.SwatchDark  = req.SwatchDark;
        spot.PhotoPath   = req.PhotoPath;

        await db.SaveChangesAsync();
        return Ok(ToDto(spot));
    }

    // DELETE /api/spots/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var spot = await db.Spots.FindAsync(id);
        if (spot is null) return NotFound();
        db.Spots.Remove(spot);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static SpotDto ToDto(Spot s) => new(
        s.Id, s.Name, s.Kind.ToString(), s.Cuisine, s.Neighborhood,
        s.Latitude, s.Longitude, s.VisitedOn, s.Dish, s.Note,
        s.Rating, s.Price, s.Returnable, s.SwatchLight, s.SwatchDark, s.PhotoPath
    );
}
