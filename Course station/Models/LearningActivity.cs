using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class LearningActivity
{
    public int ActivityId { get; set; }

    public int? CourseId { get; set; }

    public int? ModuleId { get; set; }

    public string? ActivityType { get; set; }

    public string? InstructionDetails { get; set; }

    public int? MaxScore { get; set; }

    public virtual ICollection<EmotionalFeedback> EmotionalFeedbacks { get; set; } = new List<EmotionalFeedback>();

    public virtual ICollection<InteractionLog> InteractionLogs { get; set; } = new List<InteractionLog>();

    public virtual Module? Module { get; set; }
}
