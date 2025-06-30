using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Message { get; set; }

    public string? Urgency { get; set; }

    public bool? Readstatus { get; set; }

    public virtual ICollection<Learner> Learners { get; set; } = new List<Learner>();
}
