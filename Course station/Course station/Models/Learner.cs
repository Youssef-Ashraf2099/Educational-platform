using System;
using System.Collections.Generic;

namespace Course_station.Models;

public partial class Learner
{
    public int LearnerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Gender { get; set; }

    public string? Country { get; set; }

    public string? CulturalBackground { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<EmotionalFeedback> EmotionalFeedbacks { get; set; } = new List<EmotionalFeedback>();

    public virtual ICollection<FilledSurvey> FilledSurveys { get; set; } = new List<FilledSurvey>();

    public virtual ICollection<InteractionLog> InteractionLogs { get; set; } = new List<InteractionLog>();

    public virtual ICollection<LearnerCollaboration> LearnerCollaborations { get; set; } = new List<LearnerCollaboration>();

    public virtual ICollection<LearnerDiscussion> LearnerDiscussions { get; set; } = new List<LearnerDiscussion>();

    public virtual ICollection<LearnerMastery> LearnerMasteries { get; set; } = new List<LearnerMastery>();

    public virtual ICollection<LearningPrefrence> LearningPrefrences { get; set; } = new List<LearningPrefrence>();

    public virtual ICollection<PersonalProfile> PersonalProfiles { get; set; } = new List<PersonalProfile>();

    public virtual ICollection<QuestReward> QuestRewards { get; set; } = new List<QuestReward>();

    public virtual ICollection<Ranking> Rankings { get; set; } = new List<Ranking>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public virtual ICollection<TakenAssessment> TakenAssessments { get; set; } = new List<TakenAssessment>();

    public virtual ICollection<LearningGoal> Goals { get; set; } = new List<LearningGoal>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<LearnerQuest> LearnerQuests { get; set; } = new List<LearnerQuest>();

}
