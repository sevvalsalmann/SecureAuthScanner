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
        c.RoutePrefix = string.Empty;
    });

}

app.MapPost("/scan-local", async (ScanRequest req, ScanService scanner) =>
{
    return await scanner.ScanLocalAsync(req.RepositoryPath);
});

app.MapPost("/scan-azure", async (ScanAzureRequest req, ScanService scanner) =>
{
    return await scanner.ScanAzureAsync(req.Organization, req.Project, req.Repository, req.PersonalAccessToken);
});


app.Run();
