using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using NomNom.API.Controllers;
using NomNom.API.Data;
using NomNom.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:80")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Resolve the upload directory once and register it so UploadsController can inject it.
var rawUploadPath = builder.Configuration["Uploads:Path"] ?? "uploads";
var uploadDir = Path.IsPathRooted(rawUploadPath)
    ? rawUploadPath
    : Path.Combine(builder.Environment.ContentRootPath, rawUploadPath);
Directory.CreateDirectory(uploadDir);
builder.Services.AddSingleton(new UploadDirectory(uploadDir));

var app = builder.Build();

// Apply pending migrations and seed the default user.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            DisplayName = "Sam Pieters",
            City        = "Amsterdam",
            JoinedAt    = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
        });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serve uploaded photos as static files at /uploads/*.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadDir),
    RequestPath  = "/uploads",
});

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
