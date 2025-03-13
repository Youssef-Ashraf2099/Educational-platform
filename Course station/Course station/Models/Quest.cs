using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Quest
{
    public int QuestId { get; set; }

    public int? DifficultyLevel { get; set; }

    public string? Criteria { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public virtual Collaborative? Collaborative { get; set; }

    public virtual ICollection<QuestReward> QuestRewards { get; set; } = new List<QuestReward>();

    public virtual SkillMastery? SkillMastery { get; set; }
    public virtual ICollection<LearnerQuest> LearnerQuests { get; set; } = new List<LearnerQuest>();

}
