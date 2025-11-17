var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapGet("/api/templates", () =>
{
    var folder = "/var/www/dt.sunduk.one/templates";

    if (!Directory.Exists(folder))
        return Results.Json(Array.Empty<object>());

    var files = Directory.GetFiles(folder)
        .Select(Path.GetFileName)
        .Select(f => new {
            FileName = f,
            Url = $"https://dt.sunduk.one/templates/{f}"
        });

    return Results.Json(files);
});

app.Run();
