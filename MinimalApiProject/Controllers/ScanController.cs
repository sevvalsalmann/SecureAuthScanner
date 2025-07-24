using Microsoft.AspNetCore.Mvc;
using MinimalApiProject.Services;
using MinimalApiProject.Models;

namespace MinimalApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanController : ControllerBase
    {
        private readonly ScanService _scanService;

        public ScanController(ScanService scanService)
        {
            _scanService = scanService;
        }

        [HttpPost]
        [Route("/api/scan")]
        public async Task<IActionResult> ScanLocal([FromForm] List<IFormFile> localFiles)
        {
            if (localFiles == null || !localFiles.Any())
                return BadRequest("No files uploaded.");

            var results = await _scanService.ScanLocalAsync(localFiles);
            return Ok(results);
        }


        [HttpPost("scan-azure")]
        public async Task<IActionResult> ScanAzure([FromBody] ScanAzureRequest request)
        {
            var result = await _scanService.ScanAzureAsync(
                request.Organization,
                request.Project,
                request.Repository,
                request.PersonalAccessToken
            );
            return Ok(result);
        }
    }
}
