using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public int CourseId { get; set; }

    public string? Title { get; set; }

    public string? DifficultyLevel { get; set; }

    public string? ContentUrl { get; set; }

    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();

    public virtual ICollection<ContentLibrary> ContentLibraries { get; set; } = new List<ContentLibrary>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<DiscussionForum> DiscussionForums { get; set; } = new List<DiscussionForum>();

    public virtual ICollection<LearningActivity> LearningActivities { get; set; } = new List<LearningActivity>();

    public virtual ICollection<ModuleContent> ModuleContents { get; set; } = new List<ModuleContent>();

    public virtual ICollection<TargetTrait> TargetTraits { get; set; } = new List<TargetTrait>();
}
