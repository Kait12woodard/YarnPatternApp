using System;
using System.Collections.Generic;

namespace YarnPatternApp.Models;

public partial class YarnWeight
{
    public int ID { get; set; }

    public byte Weight { get; set; }

    public virtual ICollection<Pattern> Patterns { get; set; } = new List<Pattern>();
}
