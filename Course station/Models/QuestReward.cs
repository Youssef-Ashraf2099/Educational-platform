using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class QuestReward
{
    public int QuestId { get; set; }

    public int RewardId { get; set; }

    public int LearnerId { get; set; }

    public DateTime? TimeEarned { get; set; }

    public virtual Learner Learner { get; set; } = null!;

    public virtual Quest Quest { get; set; } = null!;

    public virtual Reward Reward { get; set; } = null!;
}
