namespace Course_station.Models
{
    public class PrerequisiteViewModel
    {
        public int LearnerId { get; set; }
        public int CourseId { get; set; }
        public List<CoursePrerequisite> Prerequisites { get; set; }
    }
}


