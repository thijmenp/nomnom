using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NomNom.API.Data;
using NomNom.API.DTOs;

namespace NomNom.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext db) : ControllerBase
{
    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();
        return Ok(new UserDto(user.Id, user.DisplayName, user.City, user.JoinedAt));
    }

    // GET /api/users/{id}/stats
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetStats(int id)
    {
        if (!await db.Users.AnyAsync(u => u.Id == id)) return NotFound();

        var spots = await db.Spots.Where(s => s.UserId == id).ToListAsync();

        if (spots.Count == 0)
            return Ok(new UserStatsDto(0, 0, 0, 0, [], []));

        var total         = spots.Count;
        var avgRating     = Math.Round(spots.Average(s => s.Rating), 1);
        var returningPct  = Math.Round((decimal)spots.Count(s => s.Returnable) / total * 100, 1);

        // Extract city from "Neighborhood, City" — falls back to the whole string if no comma.
        var cityCount = spots
            .Select(s => s.Neighborhood.Contains(',')
                ? s.Neighborhood.Split(',').Last().Trim()
                : s.Neighborhood.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var byKind = spots
            .GroupBy(s => s.Kind)
            .Select(g => new KindStatDto(
                g.Key.ToString(),
                g.Count(),
                Math.Round(g.Average(s => s.Rating), 1)
            ));

        var topNeighborhoods = spots
            .GroupBy(
                s => s.Neighborhood.Contains(',')
                    ? s.Neighborhood.Split(',').First().Trim()
                    : s.Neighborhood.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new NeighborhoodStatDto(g.Key, g.Count()));

        return Ok(new UserStatsDto(total, avgRating, returningPct, cityCount, byKind, topNeighborhoods));
    }
}
