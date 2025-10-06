namespace YarnPatternApp.Data.Services.Abstract
{
    public interface IThumbnailGeneratorService
    {
        Task<byte[]> GetOrCreateThumbnailAsync(string pdfFileName);
    }
}
