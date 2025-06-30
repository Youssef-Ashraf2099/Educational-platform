using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearningPrefrenceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningPrefrenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearningPrefrence
        public async Task<IActionResult> Index()
        {
            var learningPrefrences = _context.LearningPrefrences.Include(lp => lp.Learner);
            return View(await learningPrefrences.ToListAsync());
        }

        // GET: LearningPrefrence/Details/5
        public async Task<IActionResult> Details(int? learnerId, string prefrences)
        {
            if (learnerId == null || prefrences == null)
            {
                return NotFound();
            }

            var learningPrefrence = await _context.LearningPrefrences
                .Include(lp => lp.Learner)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.Prefrences == prefrences);
            if (learningPrefrence == null)
            {
                return NotFound();
            }

            return View(learningPrefrence);
        }

        // GET: LearningPrefrence/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearningPrefrence/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,Prefrences")] LearningPrefrence learningPrefrence)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningPrefrence);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learningPrefrence);
        }

        // GET: LearningPrefrence/Edit/5
        public async Task<IActionResult> Edit(int? learnerId, string prefrences)
        {
            if (learnerId == null || prefrences == null)
            {
                return NotFound();
            }

            var learningPrefrence = await _context.LearningPrefrences.FindAsync(learnerId, prefrences);
            if (learningPrefrence == null)
            {
                return NotFound();
            }
            return View(learningPrefrence);
        }

        // POST: LearningPrefrence/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int learnerId, string prefrences, [Bind("LearnerId,Prefrences")] LearningPrefrence learningPrefrence)
        {
            if (learnerId != learningPrefrence.LearnerId || prefrences != learningPrefrence.Prefrences)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningPrefrence);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningPrefrenceExists(learningPrefrence.LearnerId, learningPrefrence.Prefrences))
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
            return View(learningPrefrence);
        }

        // GET: LearningPrefrence/Delete/5
        public async Task<IActionResult> Delete(int? learnerId, string prefrences)
        {
            if (learnerId == null || prefrences == null)
            {
                return NotFound();
            }

            var learningPrefrence = await _context.LearningPrefrences
                .Include(lp => lp.Learner)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.Prefrences == prefrences);
            if (learningPrefrence == null)
            {
                return NotFound();
            }

            return View(learningPrefrence);
        }

        // POST: LearningPrefrence/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int learnerId, string prefrences)
        {
            var learningPrefrence = await _context.LearningPrefrences.FindAsync(learnerId, prefrences);
            _context.LearningPrefrences.Remove(learningPrefrence);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearningPrefrenceExists(int learnerId, string prefrences)
        {
            return _context.LearningPrefrences.Any(e => e.LearnerId == learnerId && e.Prefrences == prefrences);
        }
    }
}
