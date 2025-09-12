using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class Difficulty
{
    public int ID { get; set; }

    public string Ranking { get; set; } = null!;

    public virtual ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
}
