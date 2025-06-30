using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class LearnerDiscussion
{
    public int ForumId { get; set; }

    public int LearnerId { get; set; }

    public string Post { get; set; } = null!;

    public TimeOnly? Time { get; set; }

    public virtual DiscussionForum Forum { get; set; } = null!;

    public virtual Learner Learner { get; set; } = null!;
}
