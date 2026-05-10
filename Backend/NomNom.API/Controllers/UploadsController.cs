using Microsoft.AspNetCore.Mvc;

namespace NomNom.API.Controllers;

// Registered as a singleton in Program.cs so the path is resolved once.
public record UploadDirectory(string Path);

[ApiController]
[Route("api/[controller]")]
public class UploadsController(UploadDirectory uploads) : ControllerBase
{
    private static readonly Dictionary<string, string> ExtByMime = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"]  = ".png",
        ["image/webp"] = ".webp",
        ["image/heic"] = ".heic",
        ["image/heif"] = ".heic",
    };

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        if (!ExtByMime.TryGetValue(file.ContentType, out var ext))
            return BadRequest("Only JPEG, PNG, WebP, or HEIC images are accepted.");

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = System.IO.Path.Combine(uploads.Path, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        return Ok(new { path = $"/uploads/{fileName}" });
    }
}
