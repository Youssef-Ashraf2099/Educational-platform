using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Ranking
{
    public int BoardId { get; set; }

    public int LearnerId { get; set; }

    public int? CourseId { get; set; }

    public int Rank { get; set; }

    public int TotalPoints { get; set; }

    public virtual Leaderboard Board { get; set; } = null!;

    public virtual Course? Course { get; set; }

    public virtual Learner Learner { get; set; } = null!;
}
