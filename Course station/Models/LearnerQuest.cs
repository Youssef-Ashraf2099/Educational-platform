using System;
using System.Collections.Generic;

namespace Course_station.Models
{
    public partial class LearnerQuest
    {
        public int LearnerId { get; set; }
        public int QuestId { get; set; }
        public DateTime AssignedDate { get; set; }
        public string? Status { get; set; }

        public virtual Learner Learner { get; set; } = null!;
        public virtual Quest Quest { get; set; } = null!;
    }
}
