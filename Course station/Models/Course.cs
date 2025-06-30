using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? DiffLevel { get; set; }

    public int? CreditPoint { get; set; }

    public string? LearningObjective { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<CoursePrerequisite> CoursePrerequisites { get; set; } = new List<CoursePrerequisite>();

    public virtual ICollection<Module> Modules { get; set; } = new List<Module>();

    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
}
