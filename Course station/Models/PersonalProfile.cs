using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class PersonalProfile
{
    public int LearnerId { get; set; }

    public int ProfileId { get; set; }

    public string? PreferedContentType { get; set; }

    public string? EmotionalState { get; set; }

    public string? PersonalityType { get; set; }

    public virtual ICollection<HealthCondition> HealthConditions { get; set; } = new List<HealthCondition>();

    public virtual Learner Learner { get; set; } = null!;

    public virtual ICollection<LearningPath> LearningPaths { get; set; } = new List<LearningPath>();
}
