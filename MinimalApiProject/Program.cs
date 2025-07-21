using MinimalApiProject.Models;
using MinimalApiProject.Services;
using Microsoft.AspNetCore.Cors;


var builder = WebApplication.CreateBuilder(args);

// ScanService'i DI container'a ekle
builder.Services.AddScoped<ScanService>();

// ✅ Swagger'ı ekle
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


// ✅ Swagger middleware'ini ekle
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureAuthScanner API v1");
        c.RoutePrefix = string.Empty; // Böylece localhost:5000 doğrudan Swagger arayüzü olur
    });

}

// Local dizini taramak için
app.MapPost("/scan-local", async (ScanRequest request, ScanService scanner) =>
{
    var result = await scanner.ScanRepositoryAsync(request);
    return Results.Ok(result);
});

// Azure DevOps reposunu taramak için
app.MapPost("/scan-azure", async (ScanAzureRequest request, ScanService scanner) =>
{
    var result = await scanner.ScanAzureRepositoryAsync(request);
    return Results.Ok(result);
});

app.Run();
