using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using YarnPatternApp.Data.Services.Abstract;
using YarnPatternApp.Data.Services.Concrete;
using YarnPatternApp.Models;
using YarnPatternApp.Models.ViewModels;

namespace YarnPatternApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatternRepo _patternRepo;
        private readonly IPdfParsingService _pdfParsingService = new PdfParsingService();

        public HomeController(ILogger<HomeController> logger, IPatternRepo patternRepo, IPdfParsingService pdfParsingService)
        {
            _logger = logger;
            _patternRepo = patternRepo;
            _pdfParsingService = pdfParsingService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManagePatterns()
        {
            return View();
        }

        public IActionResult AddPattern()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddPattern(NewPattern newPattern)
        {
            if (!ModelState.IsValid) return View(newPattern);
            var success = _patternRepo.AddPattern(newPattern);
            if (!success)
            {
                ModelState.AddModelError("", "Save Failed. Please check entries and try again.");
                return View(newPattern);
            }
            return RedirectToAction("ManagePatterns");
        }

        [HttpPost]
        public async Task<IActionResult> ParsePdf(IFormFile pdfFile)
        {
            if (pdfFile == null || !pdfFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Please upload a valid PDF file." });
            }

            try
            {
                var parsedData = _pdfParsingService.ParsePdfToPattern(pdfFile);

                return Json(new
                {
                    success = true,
                    data = parsedData,
                    message = "PDF parsed successfully! Please review and edit the information below."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing PDF: {FileName}", pdfFile.FileName);
                return Json(new { success = false, message = "Error parsing PDF. Please fill out manually." });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
