using Microsoft.AspNetCore.Mvc;
using MinimalApiProject.Models;
using MinimalApiProject.Services;
using ClosedXML.Excel;

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
                var localFiles = Request.Form.Files
                    .Where(f => f.FileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    .ToList();
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

        // Excel Export Endpoint
        [HttpPost]
        [Route("ScanUi/ExportToExcel")]
        public IActionResult ExportToExcel([FromBody] List<ScanResultItem> results)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ScanResults");

                // Header
                worksheet.Cell(1, 1).Value = "File Path";
                worksheet.Cell(1, 2).Value = "Class";
                worksheet.Cell(1, 3).Value = "Method";
                worksheet.Cell(1, 4).Value = "Line";
                worksheet.Cell(1, 5).Value = "Issue Type";
                worksheet.Cell(1, 6).Value = "Annotation";

                // Data
                for (int i = 0; i < results.Count; i++)
                {
                    var r = results[i];
                    worksheet.Cell(i + 2, 1).Value = r.FilePath;
                    worksheet.Cell(i + 2, 2).Value = r.ClassName;
                    worksheet.Cell(i + 2, 3).Value = r.MethodName;
                    worksheet.Cell(i + 2, 4).Value = r.LineNumber;
                    worksheet.Cell(i + 2, 5).Value = r.IssueType;
                    worksheet.Cell(i + 2, 6).Value = r.Annotation;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ScanResults.xlsx"
                    );
                }
            }
        }
    }
}
