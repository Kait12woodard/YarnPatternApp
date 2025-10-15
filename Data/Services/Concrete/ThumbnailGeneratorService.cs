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

        public async Task<byte[]> GetOrCreateThumbnailAsync(string pdfFileName, int pageNumber)
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

            _logger.LogInformation("Generating thumbnail from page {Page} of {PdfPath} to {ThumbnailPath}",
                pageNumber, pdfPath, thumbnailPath);

            using (var pdfStream = File.OpenRead(pdfPath))
            {
                var skBitmap = Conversion.ToImage(pdfStream, page: pageNumber);
                using (var data = skBitmap.Encode(SKEncodedImageFormat.Jpeg, 90))
                using (var fileStream = File.OpenWrite(thumbnailPath))
                {
                    data.SaveTo(fileStream);
                }
            }

            return await File.ReadAllBytesAsync(thumbnailPath);
        }

        public List<string> GenerateThumbnailPreviews(string pdfPath, int maxPages = 5)
        {
            var previews = new List<string>();
            try
            {
                if (!System.IO.File.Exists(pdfPath))
                {
                    _logger.LogWarning("PDF not found for preview generation: {Path}", pdfPath);
                    return previews;
                }

                for (int i = 0; i < maxPages; i++)
                {
                    try
                    {
                        using var pdfStream = System.IO.File.OpenRead(pdfPath);
                        var skBitmap = Conversion.ToImage(pdfStream, page: i);
                        using (var data = skBitmap.Encode(SKEncodedImageFormat.Jpeg, 60))
                        {
                            var base64 = Convert.ToBase64String(data.ToArray());
                            previews.Add($"data:image/jpeg;base64,{base64}");
                        }
                        skBitmap.Dispose();  
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex, "Reached end of document at page {Page}", i + 1);
                        break;
                    }
                }

                _logger.LogInformation("Generated {Count} thumbnail previews from {Path}", previews.Count, pdfPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating thumbnail previews for {Path}", pdfPath);
            }

            return previews;
        }
    }
}