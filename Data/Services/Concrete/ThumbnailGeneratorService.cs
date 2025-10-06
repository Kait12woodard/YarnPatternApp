using YarnPatternApp.Data.Services.Abstract;
using PDFtoImage;
using SkiaSharp;

namespace YarnPatternApp.Data.Services.Concrete
{
    public class ThumbnailGeneratorService : IThumbnailGeneratorService
    {
        private readonly IWebHostEnvironment _envirorment;
        private readonly ILogger<ThumbnailGeneratorService> _logger;

        public ThumbnailGeneratorService(IWebHostEnvironment envirorment, ILogger<ThumbnailGeneratorService> logger)
        {
            _envirorment = envirorment;
            _logger = logger;
        }

        public async Task<byte[]> GetOrCreateThumbnailAsync(string pdfFileName)
        {
            var thumbnailPath = Path.Combine(_envirorment.WebRootPath, "images", "thumbnails",
                Path.GetFileNameWithoutExtension(pdfFileName) + ".jpg");

            if (File.Exists(thumbnailPath))
            {
                return await File.ReadAllBytesAsync(thumbnailPath);
            }

            var pdfPath = Path.Combine(_envirorment.ContentRootPath, "Data", "patterns", pdfFileName);

            if (!File.Exists(pdfPath))
            {
                throw new FileNotFoundException($"PDF not Found at: {pdfPath}");
            }

            var thumbDir = Path.GetDirectoryName(thumbnailPath);
            if (!Directory.Exists(thumbDir))
            {
                Directory.CreateDirectory(thumbDir);
            }

            _logger.LogInformation("Generating thumbnail form {PdfPath} to {ThumbnailPath}", pdfPath, thumbnailPath);

            using (var pdfStream = File.OpenRead(pdfPath))
            {
                var skBitmap = Conversion.ToImage(pdfStream, page: 1);
                using (var data = skBitmap.Encode(SKEncodedImageFormat.Jpeg, 90))
                using (var fileStream = File.OpenWrite(thumbnailPath))
                {
                    data.SaveTo(fileStream);
                }
            }

            return await File.ReadAllBytesAsync(thumbnailPath);
        }
    }
}