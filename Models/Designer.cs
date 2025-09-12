using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class Designer
{
    public int ID { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
}
