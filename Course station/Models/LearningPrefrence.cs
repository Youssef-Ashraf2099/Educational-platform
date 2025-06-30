using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class LearningPrefrence
{
    public int LearnerId { get; set; }

    public string Prefrences { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;
}
