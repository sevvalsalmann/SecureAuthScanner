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
            if (string.IsNullOrEmpty(model.Mode))
            {
                ViewBag.ErrorMessage = "Please select repository mode.";
                return View("Index", model);
            }

            if (model.Mode == "Remote")
            {
                if (string.IsNullOrWhiteSpace(model.Organization)
                    || string.IsNullOrWhiteSpace(model.Project)
                    || string.IsNullOrWhiteSpace(model.Repository))
                {
                    ViewBag.ErrorMessage = "Please fill in all required fields for remote repository.";
                    return View("Index", model);
                }

                var results = await _scanService.ScanAzureAsync(
                    model.Organization,
                    model.Project,
                    model.Repository,
                    model.PersonalAccessToken
                );

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
            else if (model.Mode == "Local")
            {
                // Dosyaları Request.Form.Files ile alıyoruz
                var localFiles = Request.Form.Files.Where(f => f.FileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)).ToList();
                if (localFiles == null || localFiles.Count == 0)
                {
                    ViewBag.ErrorMessage = "Please select at least one .cs file.";
                    return View("Index", model);
                }

                var results = await _scanService.ScanLocalAsync(localFiles);

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
            else
            {
                ViewBag.ErrorMessage = "Invalid repository mode.";
                return View("Index", model);
            }

        }
    }
}
