using MinimalApiProject.Models;
using MinimalApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ScanService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/scan", async (ScanRequest request, ScanService service) =>
{
    var result = await service.ScanRepositoryAsync(request);
    return Results.Json(result);
});

app.Run();
