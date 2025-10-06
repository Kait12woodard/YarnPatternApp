using YarnPatternApp.Models.ViewModels;

namespace YarnPatternApp.Data.Services.Abstract
{
    public interface ILLMParsingService
    {
        Task<NewPattern> ReviewAndEnhancePatternAsync(string pdfPath, NewPattern parsedPattern);
    }
}
