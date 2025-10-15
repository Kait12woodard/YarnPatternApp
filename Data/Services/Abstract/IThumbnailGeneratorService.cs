namespace YarnPatternApp.Data.Services.Abstract
{
    public interface IThumbnailGeneratorService
    {
        Task<byte[]> GetOrCreateThumbnailAsync(string pdfFileName, int pageNumber);
        List<string> GenerateThumbnailPreviews(string pdfPath, int maxPages = 5);
    }
}
