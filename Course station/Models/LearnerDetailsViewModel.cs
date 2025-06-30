namespace Course_station.Models
{

    public class LearnerDetailsViewModel
    {
        public Learner Learner { get; set; }
        public List<Course> EnrolledCourses { get; set; }
        public List<TakenAssessment> TakenAssessments { get; set; }
        public PersonalProfile PersonalProfile { get; set; }
        public List<LearningPath> LearningPaths { get; set; }
        public List<HealthCondition> HealthConditions { get; set; }
        public List<Ranking> Rankings { get; set; }
        public List<Module> Modules { get; set; }
        public List<CoursePrerequisite> CoursePrerequisites { get; set; }
        public List<CourseEnrollment> CourseEnrollments { get; set; }
    }
}