using Microsoft.AspNetCore.Mvc;
using MinimalApiProject.Models;
using MinimalApiProject.Services;

namespace MinimalApiProject.Controllers
{
    public class ScanUiController : Controller
    {
        private readonly ScanService _scanService;

        public ScanUiController(ScanService scanService)
        {
            _scanService = scanService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("ScanUi/Index")]
        public async Task<IActionResult> Scan(ScanFormModel model)

        {
            List<ScanResult> results;

            if (model.Mode == "Remote")
            {
                results = await _scanService.ScanAzureAsync(
                    model.Organization,
                    model.Project, 
                    model.Repository, 
                    model.PersonalAccessToken
                );
            }
            else
            {
                results = await _scanService.ScanLocalAsync(model.LocalPath);
            }

            // Burada ScanResultModel'e map edelim (veya ViewModel varsa onu kullan)
            var resultModel = new ScanResultModel
            {
                Success = true,
                Results = results.Select(r => new ScanResultItem
                {
                    FilePath = r.FilePath,
                    ClassName = r.ClassName,
                    MethodName = r.MethodName,
                    LineNumber = r.LineNumber,
                    IssueType = r.IssueType,
                    Annotation = r.Annotation
                }).ToList()
            };

            return View("Result", resultModel);
        }
    }
}