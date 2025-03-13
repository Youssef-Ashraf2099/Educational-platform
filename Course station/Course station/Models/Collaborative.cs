using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Course_station.Models;

public partial class Collaborative
{
    public int QuestId { get; set; }

    [ModelBinder(BinderType = typeof(DateOnlyModelBinder))]
    public DateOnly? Deadline { get; set; }

    public int? MaxNumParticipants { get; set; }

    public virtual ICollection<LearnerCollaboration> LearnerCollaborations { get; set; } = new List<LearnerCollaboration>();

    public virtual Quest Quest { get; set; } = null!;
}
