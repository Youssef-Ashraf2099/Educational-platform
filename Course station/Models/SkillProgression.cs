using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class SkillProgression
{
    public int Id { get; set; }

    public int ProficiencyLevel { get; set; }

    public int? LearnerId { get; set; }

    public string? SkillName { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Skill? Skill { get; set; }
}
