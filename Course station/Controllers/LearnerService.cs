using System.Threading.Tasks;
using Course_station.Models;
using Microsoft.EntityFrameworkCore;

namespace Course_station.Controllers
{
    public class LearnerService
    {
        private readonly ApplicationDbContext _context;

        public LearnerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Learner> CreateLearnerAsync(Learner learner)
        {
            _context.Learners.Add(learner);
            await _context.SaveChangesAsync();
            return learner;
        }
        
        public async Task<Learner> FindByEmailAsync(int id)
        {
            return await _context.Learners.FirstOrDefaultAsync(l => l.LearnerId == id);
        }

        public async Task<bool> ValidateLearnerAsync(int id, string password)
        {
            var learner = await FindByEmailAsync(id);
            if (learner != null && learner.Password == password)
            {
                return true;
            }
            return false;
        }
    }
}
