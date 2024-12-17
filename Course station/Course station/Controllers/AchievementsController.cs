using Microsoft.AspNetCore.Mvc;
using Course_station.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Course_station.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AchievementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Achievements
        public async Task<IActionResult> Index()
        {
            var model = await _context.Achievements.ToListAsync();
            return View(model);
        }

        // GET: Achievements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == id);

            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

      
        // GET: Achievements/Create
        public IActionResult Create()
        {
         
            return View();
        }

        // POST: Achievements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                // Validate LearnerId and BadgeId
                if (!_context.Learners.Any(l => l.LearnerId == achievement.LearnerId))
                {
                    ModelState.AddModelError("LearnerId", "Invalid Learner.");
                }
                if (!_context.Badges.Any(b => b.BadgeId == achievement.BadgeId))
                {
                    ModelState.AddModelError("BadgeId", "Invalid Badge.");
                }

                if (ModelState.IsValid)
                {
                    // Ensure DateEarned and Type are set
                    if (achievement.DateEarned == default)
                    {
                        achievement.DateEarned = DateOnly.FromDateTime(DateTime.Now);
                    }
                    if (string.IsNullOrEmpty(achievement.Type))
                    {
                        achievement.Type = "DefaultType"; // Set a default type if necessary
                    }

                    _context.Add(achievement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Learners = new SelectList(_context.Learners, "LearnerId", "Email", achievement.LearnerId);
            ViewBag.Badges = new SelectList(_context.Badges, "BadgeId", "Title", achievement.BadgeId);
            return View(achievement);
        }


        // GET: Achievements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await _context.Achievements.FindAsync(id);

            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

        // POST: Achievements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AchievementId,Title,Description")] Achievement achievement)
        {
            if (id != achievement.AchievementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(achievement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Achievements.Any(e => e.AchievementId == id))
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
            return View(achievement);
        }

        // GET: Achievements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.AchievementId == id);

            if (achievement == null)
            {
                return NotFound();
            }

            return View(achievement);
        }

        // POST: Achievements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null)
            {
                return NotFound();
            }

            _context.Achievements.Remove(achievement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
