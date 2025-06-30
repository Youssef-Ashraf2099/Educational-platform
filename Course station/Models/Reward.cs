using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Reward
{
    public int RewardId { get; set; }

    public int? Value { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<QuestReward> QuestRewards { get; set; } = new List<QuestReward>();
}
