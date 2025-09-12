using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class ToolSize
{
    public int ID { get; set; }

    public decimal Size { get; set; }

    public virtual ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
}
