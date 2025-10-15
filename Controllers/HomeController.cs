using Microsoft.AspNetCore.Mvc;
using PDFtoImage;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
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
        private readonly IWebHostEnvironment _environment;
        private readonly IFileStorageService _fileStorage;
        private readonly IThumbnailGeneratorService _thumbnailService;
        private readonly ILLMParsingService _llmService;

        public HomeController(ILogger<HomeController> logger, IPatternRepo patternRepo, IPdfParsingService pdfParsingService, IWebHostEnvironment environment, IFileStorageService fileStorage, IThumbnailGeneratorService thumbnailService, ILLMParsingService llmService)
        {
            _logger = logger;
            _patternRepo = patternRepo;
            _pdfParsingService = pdfParsingService;
            _environment = environment;
            _fileStorage = fileStorage;
            _thumbnailService = thumbnailService;
            _llmService = llmService;
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
        public async Task<IActionResult> AddPattern(NewPattern newPattern)
        {
            _logger.LogInformation("ThumbnailPage received: {ThumbnailPage}", newPattern.ThumbnailPage);
            if (!ModelState.IsValid) return View(newPattern);

            var pdfFile = Request.Form.Files.FirstOrDefault(file => file.Name == "pdfUpload");

            if (pdfFile != null && pdfFile.Length > 0)
            {
                var fileName = Path.GetFileName(newPattern.FilePath);
                await _fileStorage.SavePdfAsync(pdfFile, fileName);
            }

            if (!string.IsNullOrEmpty(newPattern.FilePath))
            {
                var fileName = Path.GetFileName(newPattern.FilePath);
                await _thumbnailService.GetOrCreateThumbnailAsync(fileName, newPattern.ThumbnailPage);
            }

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
                var fileName = Path.GetFileName(parsedData.FilePath);
                var pdfPath = Path.Combine(_environment.ContentRootPath, "Data", "patterns", fileName);

                using (var stream = new FileStream(pdfPath, FileMode.Create))
                {
                    await pdfFile.CopyToAsync(stream);
                }

                parsedData = await _llmService.ReviewAndEnhancePatternAsync(pdfPath, parsedData);
                var thumbnailPreviews = _thumbnailService.GenerateThumbnailPreviews(pdfPath, maxPages: 5);

                return Json(new
                {
                    success = true,
                    data = parsedData,
                    thumbnailPreviews = thumbnailPreviews,
                    message = "PDF parsed successfully! Please review and edit the information below."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing PDF: {FileName}", pdfFile.FileName);
                return Json(new { success = false, message = "Error parsing PDF. Please fill out manually." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetThumbnail(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            fileName = Path.GetFileName(fileName);

            var thumbnailPath = Path.Combine(_environment.WebRootPath, "images", "thumbnails",
                Path.GetFileNameWithoutExtension(fileName) + ".jpg");

            if (!System.IO.File.Exists(thumbnailPath))
                return NotFound();

            var thumbnailBytes = await System.IO.File.ReadAllBytesAsync(thumbnailPath);
            return File(thumbnailBytes, "image/jpeg");
        }

        [HttpGet]
        public IActionResult GetPatterns()
        {
            var patterns = _patternRepo.GetAllPatterns();

            var result = patterns.Select(p => new
            {
                id = p.ID,
                name = p.Name,
                designer = p.Designer?.Name,
                craftType = p.CraftType.Craft,
                filePath = p.FilePath
            });

            return Json(result);
        }

        [HttpGet]
        public IActionResult ViewPatternPdf(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return NotFound();

            fileName = Path.GetFileName(fileName);
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "patterns", fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "application/pdf");
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
