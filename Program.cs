using MinimalApiProject.Models;
using MinimalApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/scan", (ScanRequest request) =>
{
    var service = new ScanService();
    var results = service.ScanProject(request.DirectoryPath);
    return Results.Json(results);
});

app.Run();