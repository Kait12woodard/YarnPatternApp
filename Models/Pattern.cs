using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class Pattern
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public int? DesignerID { get; set; }

    public int CraftTypeID { get; set; }

    public int? DifficultyID { get; set; }

    public bool IsFree { get; set; }

    public bool IsFavorite { get; set; }

    public string? PatSource { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? LastViewed { get; set; }

    public bool HaveMade { get; set; }

    public string FilePath { get; set; } = null!;

    public virtual CraftType CraftType { get; set; } = null!;

    public virtual Designer? Designer { get; set; }

    public virtual Difficulty? Difficulty { get; set; }

    public virtual ICollection<ProjectType> ProjectTypes { get; set; } = new List<ProjectType>();

    public virtual ICollection<PatternTag> Tags { get; set; } = new List<PatternTag>();

    public virtual ICollection<ToolSize> ToolSizes { get; set; } = new List<ToolSize>();

    public virtual ICollection<YarnBrand> YarnBrands { get; set; } = new List<YarnBrand>();

    public virtual ICollection<YarnWeight> YarnWeights { get; set; } = new List<YarnWeight>();
}
