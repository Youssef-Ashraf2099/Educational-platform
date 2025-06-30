using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class HealthCondition
{
    public int LearnerId { get; set; }

    public int ProfileId { get; set; }

    public string Condition { get; set; } = null!;

    public virtual PersonalProfile PersonalProfile { get; set; } = null!;
}
