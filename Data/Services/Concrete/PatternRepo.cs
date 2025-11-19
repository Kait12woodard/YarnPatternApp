using YarnPatternApp.Data.Services.Abstract;
using YarnPatternApp.Models.ViewModels;
using YarnPatternApp.Models;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;

namespace YarnPatternApp.Data.Services.Concrete
{
    public class PatternRepo : IPatternRepo
    {
        private readonly YarnPatternContext _context;
        public PatternRepo(YarnPatternContext context)
        {
            _context = context;
        }

        public bool AddPattern(NewPattern newPattern)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var designer = GetOrCreateDesigner(newPattern.Designer);
                var craftType = GetOrCreateCraftType(newPattern.CraftType);
                var difficulty = GetDifficulty(newPattern.Difficulty);

                var pattern = new Pattern
                {
                    Name = newPattern.Name,
                    Designer = designer,
                    CraftType = craftType,
                    Difficulty = difficulty,
                    IsFree = newPattern.IsFree,
                    IsFavorite = newPattern.IsFavorite,
                    PatSource = newPattern.PatSource,
                    HaveMade = newPattern.HaveMade,
                    FilePath = newPattern.FilePath,
                    DateAdded = DateTime.Now
                };

                _context.Patterns.Add(pattern);
                var savedRows = _context.SaveChanges();

                if (savedRows == 0)
                    return false;

                AddProjectTypes(pattern.ID, newPattern.ProjectTypes);
                AddYarnWeights(pattern.ID, newPattern.YarnWeights);
                AddToolSizes(pattern.ID, newPattern.ToolSizes);
                AddYarnBrands(pattern.ID, newPattern.YarnBrands);
                AddTags(pattern.ID, newPattern.Tags);

                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback(); 
                return false;
            }
        }

        private Designer? GetOrCreateDesigner(string? designerName)
        {
            if (string.IsNullOrWhiteSpace(designerName)) return null;
            var doesExist = _context.Designers.FirstOrDefault(person => person.Name == designerName);
            if (doesExist != null) return doesExist;
            var newDesigner = new Designer { Name = designerName };
            _context.Designers.Add(newDesigner);
            _context.SaveChanges();
            return newDesigner;
        }

        private CraftType GetOrCreateCraftType(string craftTypeName)
        {
            var doesExist = _context.CraftTypes.FirstOrDefault(craft => craft.Craft == craftTypeName);
            if (doesExist != null) return doesExist;
            var newCraftType = new CraftType { Craft = craftTypeName };
            _context.CraftTypes.Add(newCraftType);
            _context.SaveChanges();
            return newCraftType;
        }

        private Difficulty? GetDifficulty(int? level)
        {
            if (level == null) return null;
            return _context.Difficulties.FirstOrDefault(diff => diff.ID == level);
        }

        private ProjectType GetOrCreateProjectType(string typeName)
        {
            var doesExist = _context.ProjectTypes
                .FirstOrDefault(project => project.Name == typeName);
            if (doesExist != null) return doesExist;
            var newProjectType = new ProjectType { Name = typeName };
            var newType = new ProjectType { Name = typeName };
            _context.ProjectTypes.Add(newType);
            _context.SaveChanges();
            return newType;
        }

        private void AddProjectTypes(int patternId, List<string>? projectTypes)
        {
            if (projectTypes?.Any() != true) return;
            var pattern = _context.Patterns.Include(pat => pat.ProjectTypes)
                .First(pat => pat.ID == patternId);
            foreach (var typeName in projectTypes)
            {
                var projectType = GetOrCreateProjectType(typeName);
                if (!pattern.ProjectTypes.Any(pt => pt.ID == projectType.ID))
                {
                    pattern.ProjectTypes.Add(projectType);
                }
            }
        }

        private YarnWeight GetOrCreateYarnWeight(byte weight)
        {
            var doesExist = _context.YarnWeights
                .FirstOrDefault(yw => yw.Weight == weight);
            if (doesExist != null) return doesExist;
            var newWeight = new YarnWeight { Weight = weight };
            _context.YarnWeights.Add(newWeight);
            _context.SaveChanges();
            return newWeight;
        }

        private ToolSize GetOrCreateToolSize(decimal size)
        {
            var doesExist = _context.ToolSizes
                .FirstOrDefault(tool => tool.Size == size);
            if (doesExist != null) return doesExist;
            var newSize = new ToolSize { Size = size };
            _context.ToolSizes.Add(newSize);
            _context.SaveChanges();
            return newSize;
        }

        private YarnBrand GetOrCreateYarnBrand(string brandName)
        {
            var doesExist = _context.YarnBrands
                .FirstOrDefault(brand => brand.Name == brandName);
            if (doesExist != null) return doesExist;
            var newBrand = new YarnBrand { Name = brandName };
            _context.YarnBrands.Add(newBrand);
            _context.SaveChanges();
            return newBrand;
        }

        private PatternTag GetOrCreateTag(string tag)
        {
            var doesExist = _context.PatternTags
                .FirstOrDefault(t => t.Tag == tag);
            if (doesExist != null) return doesExist;
            var newTag = new PatternTag { Tag = tag };
            _context.PatternTags.Add(newTag);
            _context.SaveChanges();
            return newTag;

        }

        private void AddYarnWeights(int patternId, List<string?> yarnWeights)
        {
            if (yarnWeights?.Any() != true) return;
            var pattern = _context.Patterns.Include(pat => pat.YarnWeights)
                .First(pat => pat.ID == patternId);
            foreach (var weightStr in yarnWeights)
            {
                if (byte.TryParse(weightStr, out byte weight))
                {
                    var yarnWeight = GetOrCreateYarnWeight(weight);
                    if (!pattern.YarnWeights.Any(yw => yw.ID == yarnWeight.ID))
                    {
                        pattern.YarnWeights.Add(yarnWeight);
                    }
                }
            }
        }

        private void AddToolSizes(int patternId, List<string?> toolSizes)
        {
            if (toolSizes?.Any() != true) return;
            var pattern = _context.Patterns.Include(pat => pat.ToolSizes)
                .First(pat => pat.ID == patternId);
            foreach (var sizeStr in toolSizes)
            {
                if (decimal.TryParse(sizeStr, out decimal size))
                {
                    var toolSize = GetOrCreateToolSize(size);
                    if (!pattern.ToolSizes.Any(ts => ts.ID == toolSize.ID))
                    {
                        pattern.ToolSizes.Add(toolSize);
                    }
                }
            }
        }

        private void AddYarnBrands(int patternId, List<string?> yarnBrands)
        {
            if (yarnBrands?.Any() != true) return;
            var pattern = _context.Patterns.Include(pat => pat.YarnBrands)
                .First(pat => pat.ID == patternId);
            foreach (var brandName in yarnBrands)
            {
                if (string.IsNullOrWhiteSpace(brandName)) continue;
                var yarnBrand = GetOrCreateYarnBrand(brandName);
                if (!pattern.YarnBrands.Any(yb => yb.ID == yarnBrand.ID))
                {
                    pattern.YarnBrands.Add(yarnBrand);
                }
            }
        }

        private void AddTags(int patternId, List<string?> tags)
        {
            if (tags?.Any() != true) return;
            var pattern = _context.Patterns.Include(pat => pat.Tags)
                .First(pat => pat.ID == patternId);
            foreach (var tagName in tags)
            {
                var tag = GetOrCreateTag(tagName);
                if (!pattern.Tags.Any(t => t.ID == tag.ID))
                {
                    pattern.Tags.Add(tag);
                }
            }
        }

        public List<Pattern> GetAllPatterns()
        {
            return _context.Patterns
                .Include(p => p.Designer)
                .Include(p => p.CraftType)
                .ToList();
        }

        public Pattern? GetPatternById(int patternId)
        {
            return _context.Patterns
                .Include(p => p.Designer)
                .Include(p => p.CraftType)
                .Include(p => p.Difficulty)
                .Include(p => p.ProjectTypes)
                .Include(p => p.Tags)
                .Include(p => p.ToolSizes)
                .Include(p => p.YarnBrands)
                .Include(p => p.YarnWeights)
                .FirstOrDefault(p => p.ID == patternId);
        }

        public NewPattern? GetPatternForEdit(int patternId)
        {
            var pattern = GetPatternById(patternId);
            if (pattern == null) return null;

            return new NewPattern
            {
                Name = pattern.Name,
                Designer = pattern.Designer?.Name,
                CraftType = pattern.CraftType.Craft,
                Difficulty = pattern.Difficulty?.ID,
                IsFree = pattern.IsFree,
                IsFavorite = pattern.IsFavorite,
                PatSource = pattern.PatSource,
                HaveMade = pattern.HaveMade,
                FilePath = pattern.FilePath,
                ProjectTypes = pattern.ProjectTypes?.Select(pt => pt.Name).ToList(),
                Tags = pattern.Tags?.Select(t => t.Tag).ToList(),
                ToolSizes = pattern.ToolSizes?.Select(ts => ts.Size.ToString()).ToList(),
                YarnBrands = pattern.YarnBrands?.Select(yb => yb.Name).ToList(),
                YarnWeights = pattern.YarnWeights?.Select(yw => yw.Weight.ToString()).ToList()
            };
        }

        public bool UpdatePattern(int patternId, NewPattern updatedPattern)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var pattern = GetPatternById(patternId);
                if (pattern == null) return false;

                pattern.Name = updatedPattern.Name;
                pattern.IsFree = updatedPattern.IsFree;
                pattern.IsFavorite = updatedPattern.IsFavorite;
                pattern.PatSource = updatedPattern.PatSource;
                pattern.HaveMade = updatedPattern.HaveMade;
                pattern.FilePath = updatedPattern.FilePath;

                pattern.Designer = GetOrCreateDesigner(updatedPattern.Designer);
                pattern.CraftType = GetOrCreateCraftType(updatedPattern.CraftType);
                pattern.Difficulty = GetDifficulty(updatedPattern.Difficulty);

                _context.SaveChanges();

                UpdateProjectTypes(pattern.ID, updatedPattern.ProjectTypes);
                UpdateYarnWeights(pattern.ID, updatedPattern.YarnWeights);
                UpdateToolSizes(pattern.ID, updatedPattern.ToolSizes);
                UpdateYarnBrands(pattern.ID, updatedPattern.YarnBrands);
                UpdateTags(pattern.ID, updatedPattern.Tags);

                _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        private void UpdateProjectTypes(int patternId, List<string>? projectTypes)
        {
            var pattern = _context.Patterns.Include(pat => pat.ProjectTypes)
                .First(pat => pat.ID == patternId);

            pattern.ProjectTypes.Clear();

            if (projectTypes?.Any() == true)
            {
                foreach (var typeName in projectTypes)
                {
                    var projectType = GetOrCreateProjectType(typeName);
                    pattern.ProjectTypes.Add(projectType);
                }
            }
        }

        private void UpdateYarnWeights(int patternId, List<string?>? yarnWeights)
        {
            var pattern = _context.Patterns.Include(pat => pat.YarnWeights)
                .First(pat => pat.ID == patternId);

            pattern.YarnWeights.Clear();

            if (yarnWeights?.Any() == true)
            {
                foreach (var weightStr in yarnWeights)
                {
                    if (byte.TryParse(weightStr, out byte weight))
                    {
                        var yarnWeight = GetOrCreateYarnWeight(weight);
                        pattern.YarnWeights.Add(yarnWeight);
                    }
                }
            }
        }

        private void UpdateToolSizes(int patternId, List<string?>? toolSizes)
        {
            var pattern = _context.Patterns.Include(pat => pat.ToolSizes)
                .First(pat => pat.ID == patternId);

            pattern.ToolSizes.Clear();

            if (toolSizes?.Any() == true)
            {
                foreach (var sizeStr in toolSizes)
                {
                    if (decimal.TryParse(sizeStr, out decimal size))
                    {
                        var toolSize = GetOrCreateToolSize(size);
                        pattern.ToolSizes.Add(toolSize);
                    }
                }
            }
        }

        private void UpdateYarnBrands(int patternId, List<string?>? yarnBrands)
        {
            var pattern = _context.Patterns.Include(pat => pat.YarnBrands)
                .First(pat => pat.ID == patternId);

            pattern.YarnBrands.Clear();

            if (yarnBrands?.Any() == true)
            {
                foreach (var brandName in yarnBrands)
                {
                    if (!string.IsNullOrWhiteSpace(brandName))
                    {
                        var yarnBrand = GetOrCreateYarnBrand(brandName);
                        pattern.YarnBrands.Add(yarnBrand);
                    }
                }
            }
        }

        private void UpdateTags(int patternId, List<string?>? tags)
        {
            var pattern = _context.Patterns.Include(pat => pat.Tags)
                .First(pat => pat.ID == patternId);

            pattern.Tags.Clear();

            if (tags?.Any() == true)
            {
                foreach (var tagName in tags)
                {
                    if (!string.IsNullOrWhiteSpace(tagName))
                    {
                        var tag = GetOrCreateTag(tagName);
                        pattern.Tags.Add(tag);
                    }
                }
            }
        }
    }
}
