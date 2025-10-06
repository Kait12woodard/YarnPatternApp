namespace YarnPatternApp.Data.Services.Abstract
{
    public interface IFileStorageService
    {
        Task<string> SavePdfAsync(IFormFile pdfFile, string fileName);
    }
}
