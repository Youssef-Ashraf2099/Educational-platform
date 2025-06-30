using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearningGoalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningGoalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearningGoal
        public async Task<IActionResult> Index()
        {
            return View(await _context.LearningGoals.ToListAsync());
        }

        // GET: LearningGoal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningGoal = await _context.LearningGoals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (learningGoal == null)
            {
                return NotFound();
            }

            return View(learningGoal);
        }

        // GET: LearningGoal/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearningGoal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,Deadline,Description")] LearningGoal learningGoal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningGoal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learningGoal);
        }

        // GET: LearningGoal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningGoal = await _context.LearningGoals.FindAsync(id);
            if (learningGoal == null)
            {
                return NotFound();
            }
            return View(learningGoal);
        }

        // POST: LearningGoal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Deadline,Description")] LearningGoal learningGoal)
        {
            if (id != learningGoal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningGoal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningGoalExists(learningGoal.Id))
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
            return View(learningGoal);
        }

        // GET: LearningGoal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningGoal = await _context.LearningGoals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (learningGoal == null)
            {
                return NotFound();
            }

            return View(learningGoal);
        }

        // POST: LearningGoal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningGoal = await _context.LearningGoals.FindAsync(id);
            _context.LearningGoals.Remove(learningGoal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearningGoalExists(int id)
        {
            return _context.LearningGoals.Any(e => e.Id == id);
        }
    }
}
