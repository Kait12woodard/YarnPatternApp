using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class PatternTag
{
    public int ID { get; set; }

    public string Tag { get; set; } = null!;

    public virtual ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
}
