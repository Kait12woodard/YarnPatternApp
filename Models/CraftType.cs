using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class CraftType
{
    public int ID { get; set; }

    public string Craft { get; set; } = null!;

    public virtual ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
}
