using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class CoursePrerequisite
{
    public int CourseId { get; set; }

    public string Prerequisite { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
