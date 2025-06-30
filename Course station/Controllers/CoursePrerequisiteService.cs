using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course_station.Models;

namespace Course_station.Services
{
    public class CoursePrerequisiteService
    {
        private readonly ApplicationDbContext _context;

        public CoursePrerequisiteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckPrerequisitesCompleted(int learnerId, int courseId)
        {
            var prerequisites = await _context.CoursePrerequisites
                .Where(cp => cp.CourseId == courseId)
                .ToListAsync();

            foreach (var prerequisite in prerequisites)
            {
                var prerequisiteCourse = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Title == prerequisite.Prerequisite);

                if (prerequisiteCourse == null)
                {
                    return false;
                }

                var completed = await _context.CourseEnrollments
                    .AnyAsync(ce => ce.LearnerId == learnerId && ce.CourseId == prerequisiteCourse.CourseId && ce.Status == "Completed");

                if (!completed)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
