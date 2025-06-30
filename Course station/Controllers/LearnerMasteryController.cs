using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearnerMasteryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearnerMasteryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearnerMastery
        public async Task<IActionResult> Index()
        {
            var learnerMasteries = _context.LearnerMasteries.Include(lm => lm.Learner).Include(lm => lm.Quest);
            return View(await learnerMasteries.ToListAsync());
        }

        // GET: LearnerMastery/Details/5
        public async Task<IActionResult> Details(int? learnerId, int? questId)
        {
            if (learnerId == null || questId == null)
            {
                return NotFound();
            }

            var learnerMastery = await _context.LearnerMasteries
                .Include(lm => lm.Learner)
                .Include(lm => lm.Quest)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.QuestId == questId);
            if (learnerMastery == null)
            {
                return NotFound();
            }

            return View(learnerMastery);
        }

        // GET: LearnerMastery/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearnerMastery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,QuestId,CompletionStatus")] LearnerMastery learnerMastery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learnerMastery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learnerMastery);
        }

        // GET: LearnerMastery/Edit/5
        public async Task<IActionResult> Edit(int? learnerId, int? questId)
        {
            if (learnerId == null || questId == null)
            {
                return NotFound();
            }

            var learnerMastery = await _context.LearnerMasteries.FindAsync(learnerId, questId);
            if (learnerMastery == null)
            {
                return NotFound();
            }
            return View(learnerMastery);
        }

        // POST: LearnerMastery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int learnerId, int questId, [Bind("LearnerId,QuestId,CompletionStatus")] LearnerMastery learnerMastery)
        {
            if (learnerId != learnerMastery.LearnerId || questId != learnerMastery.QuestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learnerMastery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerMasteryExists(learnerMastery.LearnerId, learnerMastery.QuestId))
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
            return View(learnerMastery);
        }

        // GET: LearnerMastery/Delete/5
        public async Task<IActionResult> Delete(int? learnerId, int? questId)
        {
            if (learnerId == null || questId == null)
            {
                return NotFound();
            }

            var learnerMastery = await _context.LearnerMasteries
                .Include(lm => lm.Learner)
                .Include(lm => lm.Quest)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.QuestId == questId);
            if (learnerMastery == null)
            {
                return NotFound();
            }

            return View(learnerMastery);
        }

        // POST: LearnerMastery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int learnerId, int questId)
        {
            var learnerMastery = await _context.LearnerMasteries.FindAsync(learnerId, questId);
            _context.LearnerMasteries.Remove(learnerMastery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearnerMasteryExists(int learnerId, int questId)
        {
            return _context.LearnerMasteries.Any(e => e.LearnerId == learnerId && e.QuestId == questId);
        }
    }
}
