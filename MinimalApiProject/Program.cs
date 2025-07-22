using MinimalApiProject.Models;
using MinimalApiProject.Services;
using Microsoft.AspNetCore.Cors;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ScanService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SecureAuthScanner API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
    );
});


var app = builder.Build();

app.UseCors();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureAuthScanner API v1");
        c.RoutePrefix = "swagger";
    });

}

app.MapPost("/scan-local", async (ScanRequest request, ScanService scanner) =>
{
    var result = await scanner.ScanLocalAsync(request.RepositoryPath);
    return Results.Ok(result);
});

app.MapPost("/scan-azure", async (ScanAzureRequest request, ScanService scanner) =>
{
    var result = await scanner.ScanAzureAsync(
        request.Organization,
        request.Project,
        request.Repository,
        request.PersonalAccessToken
    );
    return Results.Ok(result);
});

app.Run();
