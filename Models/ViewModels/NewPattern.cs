namespace YarnPatternApp.Models.ViewModels
{
    public class NewPattern
    {
        public string Name { get; set; } = null!;
        public string? Designer { get; set; }
        public string CraftType { get; set; } = null!;
        public int? Difficulty { get; set; }
        public bool IsFree { get; set; }
        public bool IsFavorite { get; set; }
        public string? PatSource { get; set; }
        public bool HaveMade { get; set; }
        public string FilePath { get; set; } = null!;
        public List<string>? ProjectTypes { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? ToolSizes { get; set; }
        public List<string>? YarnBrands { get; set; }
        public List<string>? YarnWeights { get; set; }
        public int ThumbnailPage { get; set; } = 0;
    }
}
