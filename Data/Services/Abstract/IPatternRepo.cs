using YarnPatternApp.Models;
using YarnPatternApp.Models.ViewModels;

namespace YarnPatternApp.Data.Services.Abstract
{
    public interface IPatternRepo
    {
        bool AddPattern(NewPattern newPattern);
        List<Pattern> GetAllPatterns();
        Pattern? GetPatternById(int id);
        NewPattern? GetPatternForEdit(int id);
        bool UpdatePattern(int id, NewPattern updatedPattern);
    }
}
