using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearningActivityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearningActivity
        public async Task<IActionResult> Index()
        {
            var learningActivities = _context.LearningActivities.Include(la => la.Module);
            return View(await learningActivities.ToListAsync());
        }

        // GET: LearningActivity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningActivity = await _context.LearningActivities
                .Include(la => la.Module)
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (learningActivity == null)
            {
                return NotFound();
            }

            return View(learningActivity);
        }

        // GET: LearningActivity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearningActivity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityId,CourseId,ModuleId,ActivityType,InstructionDetails,MaxScore")] LearningActivity learningActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learningActivity);
        }

        // GET: LearningActivity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningActivity = await _context.LearningActivities.FindAsync(id);
            if (learningActivity == null)
            {
                return NotFound();
            }
            return View(learningActivity);
        }

        // POST: LearningActivity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,CourseId,ModuleId,ActivityType,InstructionDetails,MaxScore")] LearningActivity learningActivity)
        {
            if (id != learningActivity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningActivityExists(learningActivity.ActivityId))
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
            return View(learningActivity);
        }

        // GET: LearningActivity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningActivity = await _context.LearningActivities
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (learningActivity == null)
            {
                return NotFound();
            }

            return View(learningActivity);
        }

// POST: LearningActivity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningActivity = await _context.LearningActivities.FindAsync(id);
            if (learningActivity != null)
            {
                _context.LearningActivities.Remove(learningActivity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

       

        private bool LearningActivityExists(int id)
        {
            return _context.LearningActivities.Any(e => e.ActivityId == id);
        }
    }
}
