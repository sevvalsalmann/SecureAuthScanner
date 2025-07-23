using MinimalApiProject.Models;
using MinimalApiProject.Services;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// ---- SERVİSLER ----
builder.Services.AddScoped<ScanService>();

builder.Services.AddControllersWithViews();  // ← Bunu ekle! (MVC view ve controller için)
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

// ---- APP BUILD ----
var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureAuthScanner API v1");
        c.RoutePrefix = "swagger";   // ← Artık Swagger sadece /swagger'da açılır!
    });
}

// MVC routing
app.UseStaticFiles();   // (CSS/JS kullanacaksan)
app.UseRouting();

app.UseAuthorization(); // (kullanmıyorsan sorun olmaz)
app.UseAuthentication(); // (kullanmıyorsan sorun olmaz)

app.MapControllers();  // ← Klasik MVC Controller'ları ve arayüzü aktif eder!

// (Minimal API endpointleri istersen aşağıda bırakabilirsin.)
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

// ---- DEFAULT ROUTE AYARI ----
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ScanUi}/{action=Index}/{id?}");
// Ana sayfa açıldığında /ScanUi/Index çalışır

app.Run();
