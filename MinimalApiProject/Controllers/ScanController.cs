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

        [HttpPost("scan-local")]
        public async Task<IActionResult> ScanLocal([FromBody] ScanRequest request)
        {
            var result = await _scanService.ScanLocalAsync(request.RepositoryPath);
            return Ok(result);
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
