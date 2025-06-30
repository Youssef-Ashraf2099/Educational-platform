using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class HealthConditionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthConditionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HealthCondition
        public async Task<IActionResult> Index()
        {
            var healthConditions = _context.HealthConditions.Include(h => h.PersonalProfile);
            return View(await healthConditions.ToListAsync());
        }

        // GET: HealthCondition/Details/5
        public async Task<IActionResult> Details(int? learnerId, int? profileId, string condition)
        {
            if (learnerId == null || profileId == null || condition == null)
            {
                return NotFound();
            }

            var healthCondition = await _context.HealthConditions
                .Include(h => h.PersonalProfile)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.ProfileId == profileId && m.Condition == condition);
            if (healthCondition == null)
            {
                return NotFound();
            }

            return View(healthCondition);
        }

        // GET: HealthCondition/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HealthCondition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,ProfileId,Condition")] HealthCondition healthCondition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(healthCondition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(healthCondition);
        }

        // GET: HealthCondition/Edit/5
        public async Task<IActionResult> Edit(int? learnerId, int? profileId, string condition)
        {
            if (learnerId == null || profileId == null || condition == null)
            {
                return NotFound();
            }

            var healthCondition = await _context.HealthConditions.FindAsync(learnerId, profileId, condition);
            if (healthCondition == null)
            {
                return NotFound();
            }
            return View(healthCondition);
        }

        // POST: HealthCondition/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int learnerId, int profileId, string condition, [Bind("LearnerId,ProfileId,Condition")] HealthCondition healthCondition)
        {
            if (learnerId != healthCondition.LearnerId || profileId != healthCondition.ProfileId || condition != healthCondition.Condition)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthCondition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthConditionExists(healthCondition.LearnerId, healthCondition.ProfileId, healthCondition.Condition))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(healthCondition);
        }

        // GET: HealthCondition/Delete/5
        public async Task<IActionResult> Delete(int? learnerId, int? profileId, string condition)
        {
            if (learnerId == null || profileId == null || condition == null)
            {
                return NotFound();
            }

            var healthCondition = await _context.HealthConditions
                .Include(h => h.PersonalProfile)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.ProfileId == profileId && m.Condition == condition);
            if (healthCondition == null)
            {
                return NotFound();
            }

            return View(healthCondition);
        }

        // POST: HealthCondition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int learnerId, int profileId, string condition)
        {
            var healthCondition = await _context.HealthConditions.FindAsync(learnerId, profileId, condition);
            _context.HealthConditions.Remove(healthCondition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthConditionExists(int learnerId, int profileId, string condition)
        {
            return _context.HealthConditions.Any(e => e.LearnerId == learnerId && e.ProfileId == profileId && e.Condition == condition);
        }
    }
}