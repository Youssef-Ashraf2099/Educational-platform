using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class TakenAssessmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TakenAssessmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TakenAssessment
        public async Task<IActionResult> Index()
        {
            var takenAssessments = _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner);
            return View(await takenAssessments.ToListAsync());
        }

        // GET: TakenAssessment/Details/5
        public async Task<IActionResult> Details(int? assessmentId, int? learnerId)
        {
            if (assessmentId == null || learnerId == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .FirstOrDefaultAsync(m => m.AssessmentId == assessmentId && m.LearnerId == learnerId);
            if (takenAssessment == null)
            {
                return NotFound();
            }

            return View(takenAssessment);
        }

        // GET: TakenAssessment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TakenAssessment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssessmentId,LearnerId,ScoredPoint")] TakenAssessment takenAssessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(takenAssessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(takenAssessment);
        }

        // GET: TakenAssessment/Edit/5
        public async Task<IActionResult> Edit(int? assessmentId, int? learnerId)
        {
            if (assessmentId == null || learnerId == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments.FindAsync(assessmentId, learnerId);
            if (takenAssessment == null)
            {
                return NotFound();
            }
            return View(takenAssessment);
        }

        // POST: TakenAssessment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int assessmentId, int learnerId, [Bind("AssessmentId,LearnerId,ScoredPoint")] TakenAssessment takenAssessment)
        {
            if (assessmentId != takenAssessment.AssessmentId || learnerId != takenAssessment.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(takenAssessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TakenAssessmentExists(takenAssessment.AssessmentId, takenAssessment.LearnerId))
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
            return View(takenAssessment);
        }

        // GET: TakenAssessment/Delete/5
        public async Task<IActionResult> Delete(int? assessmentId, int? learnerId)
        {
            if (assessmentId == null || learnerId == null)
            {
                return NotFound();
            }

            var takenAssessment = await _context.TakenAssessments
                .Include(t => t.Assessment)
                .Include(t => t.Learner)
                .FirstOrDefaultAsync(m => m.AssessmentId == assessmentId && m.LearnerId == learnerId);
            if (takenAssessment == null)
            {
                return NotFound();
            }

            return View(takenAssessment);
        }

        // POST: TakenAssessment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int assessmentId, int learnerId)
        {
            var takenAssessment = await _context.TakenAssessments.FindAsync(assessmentId, learnerId);
            _context.TakenAssessments.Remove(takenAssessment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TakenAssessmentExists(int assessmentId, int learnerId)
        {
            return _context.TakenAssessments.Any(e => e.AssessmentId == assessmentId && e.LearnerId == learnerId);
        }
    }
}
