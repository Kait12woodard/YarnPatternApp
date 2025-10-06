using YarnPatternApp.Data.Services.Abstract;

namespace YarnPatternApp.Data.Services.Concrete
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SavePdfAsync(IFormFile pdfFile, string fileName)
        {
            var patternsFolder = Path.Combine(_environment.ContentRootPath, "Data", "patterns");

            if (!Directory.Exists(patternsFolder))
                Directory.CreateDirectory(patternsFolder);

            var fullPath = Path.Combine(patternsFolder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }

            return $"Data/patterns/{fileName}";
        }
    }
}
