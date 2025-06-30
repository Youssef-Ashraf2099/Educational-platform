using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class LearningGoal
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly Deadline { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Learner> Learners { get; set; } = new List<Learner>();
}
