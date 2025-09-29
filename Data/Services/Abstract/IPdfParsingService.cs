using YarnPatternApp.Models.ViewModels;

namespace YarnPatternApp.Data.Services.Abstract
{
    public interface IPdfParsingService
    {
        NewPattern ParsePdfToPattern(IFormFile pdfFile);
    }
}

