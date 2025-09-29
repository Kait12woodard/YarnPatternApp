using YarnPatternApp.Models.ViewModels;

namespace YarnPatternApp.Data.Services.Abstract
{
    public interface IPatternRepo
    {
        bool AddPattern(NewPattern newPattern);
    }
}
