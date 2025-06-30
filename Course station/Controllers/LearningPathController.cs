using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearningPathController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningPathController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearningPath
        public async Task<IActionResult> Index()
        {
            var learningPaths = _context.LearningPaths.Include(lp => lp.PersonalProfile);
            return View(await learningPaths.ToListAsync());
        }

        // GET: LearningPath/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningPath = await _context.LearningPaths
                .Include(lp => lp.PersonalProfile)
                .FirstOrDefaultAsync(m => m.PathId == id);
            if (learningPath == null)
            {
                return NotFound();
            }

            return View(learningPath);
        }

        // GET: LearningPath/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearningPath/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PathId,LearnerId,ProfileId,CompletionStatus,CustomContent,AdaptiveRules")] LearningPath learningPath)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningPath);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learningPath);
        }

        // GET: LearningPath/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningPath = await _context.LearningPaths.FindAsync(id);
            if (learningPath == null)
            {
                return NotFound();
            }
            return View(learningPath);
        }

        // POST: LearningPath/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PathId,LearnerId,ProfileId,CompletionStatus,CustomContent,AdaptiveRules")] LearningPath learningPath)
        {
            if (id != learningPath.PathId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningPath);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningPathExists(learningPath.PathId))
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
            return View(learningPath);
        }

        // GET: LearningPath/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningPath = await _context.LearningPaths
                .Include(lp => lp.PersonalProfile)
                .FirstOrDefaultAsync(m => m.PathId == id);
            if (learningPath == null)
            {
                return NotFound();
            }

            return View(learningPath);
        }

        // POST: LearningPath/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningPath = await _context.LearningPaths.FindAsync(id);
            _context.LearningPaths.Remove(learningPath);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearningPathExists(int id)
        {
            return _context.LearningPaths.Any(e => e.PathId == id);
        }
    }
}
