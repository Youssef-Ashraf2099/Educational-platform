using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearnerCollaborationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearnerCollaborationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearnerCollaboration
        public async Task<IActionResult> Index()
        {
            var learnerCollaborations = _context.LearnerCollaborations.Include(l => l.Learner).Include(l => l.Quest);
            return View(await learnerCollaborations.ToListAsync());
        }

        // GET: LearnerCollaboration/Details/5
        public async Task<IActionResult> Details(int? learnerId, int? questId)
        {
            if (learnerId == null || questId == null)
            {
                return NotFound();
            }

            var learnerCollaboration = await _context.LearnerCollaborations
                .Include(l => l.Learner)
                .Include(l => l.Quest)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.QuestId == questId);
            if (learnerCollaboration == null)
            {
                return NotFound();
            }

            return View(learnerCollaboration);
        }

        // GET: LearnerCollaboration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearnerCollaboration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,QuestId,CompletionStatus")] LearnerCollaboration learnerCollaboration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learnerCollaboration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learnerCollaboration);
        }

        // GET: LearnerCollaboration/Edit/5
        public async Task<IActionResult> Edit(int? learnerId, int? questId)
        {
            if (learnerId == null || questId == null)
            {
                return NotFound();
            }

            var learnerCollaboration = await _context.LearnerCollaborations.FindAsync(learnerId, questId);
            if (learnerCollaboration == null)
            {
                return NotFound();
            }
            return View(learnerCollaboration);
        }

        // POST: LearnerCollaboration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int learnerId, int questId, [Bind("LearnerId,QuestId,CompletionStatus")] LearnerCollaboration learnerCollaboration)
        {
            if (learnerId != learnerCollaboration.LearnerId || questId != learnerCollaboration.QuestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learnerCollaboration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerCollaborationExists(learnerCollaboration.LearnerId, learnerCollaboration.QuestId))
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
            return View(learnerCollaboration);
        }

        // GET: LearnerCollaboration/Delete/5
        public async Task<IActionResult> Delete(int? learnerId, int? questId)
        {
            if (learnerId == null || questId == null)
            {
                return NotFound();
            }

            var learnerCollaboration = await _context.LearnerCollaborations
                .Include(l => l.Learner)
                .Include(l => l.Quest)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.QuestId == questId);
            if (learnerCollaboration == null)
            {
                return NotFound();
            }

            return View(learnerCollaboration);
        }

        // POST: LearnerCollaboration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int learnerId, int questId)
        {
            var learnerCollaboration = await _context.LearnerCollaborations.FindAsync(learnerId, questId);
            _context.LearnerCollaborations.Remove(learnerCollaboration);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearnerCollaborationExists(int learnerId, int questId)
        {
            return _context.LearnerCollaborations.Any(e => e.LearnerId == learnerId && e.QuestId == questId);
        }
    }
}
