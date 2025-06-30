using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class EmotionalFeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmotionalFeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Trend()
        {
            var emotionalFeedbacks = _context.EmotionalFeedbacks.Include(e => e.Activity).Include(e => e.Learner);
            await emotionalFeedbacks.ToListAsync();
            return RedirectToAction("Index", "EmotionalfeedbackReview");
        }
        public IActionResult EmotionalTrendAnalysis()
        {
            return View();
        }

        public class EmotionalTrendAnalysisResult
        {
            public int? ActivityId { get; set; }
            public string? EmotionalState { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmotionalTrendAnalysis(int CourseID, int ModuleID, DateTime TimePeriod)
        {
            if (ModelState.IsValid)
            {
                var result = await _context.EmotionalFeedbacks
                    .Where(e => e.Activity.CourseId == CourseID && e.Activity.ModuleId == ModuleID && e.Timestamp >= TimePeriod)
                    .Select(e => new EmotionalTrendAnalysisResult
                    {
                        ActivityId = e.ActivityId,
                        EmotionalState = e.EmotionalState
                    })
                    .ToListAsync();

                return View("EmotionalTrendAnalysisResult", result);
            }
            return View();
        }




        // GET: EmotionalFeedback
        public async Task<IActionResult> Index()
        {
            var emotionalFeedbacks = _context.EmotionalFeedbacks.Include(e => e.Activity).Include(e => e.Learner);
            return View(await emotionalFeedbacks.ToListAsync());
        }

        // GET: EmotionalFeedback/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalFeedback = await _context.EmotionalFeedbacks
                .Include(e => e.Activity)
                .Include(e => e.Learner)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (emotionalFeedback == null)
            {
                return NotFound();
            }

            return View(emotionalFeedback);
        }

        // GET: EmotionalFeedback/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FeedbackId,LearnerId,ActivityId,Timestamp,EmotionalState")] EmotionalFeedback emotionalFeedback)
        {
            if (ModelState.IsValid)
            {
                var activityExists = await _context.LearningActivities.AnyAsync(a => a.ActivityId == emotionalFeedback.ActivityId);
                if (!activityExists)
                {
                    ModelState.AddModelError("ActivityId", "Invalid Activity ID.");
                    return View(emotionalFeedback);
                }

                _context.Add(emotionalFeedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emotionalFeedback);
        }




        // GET: EmotionalFeedback/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalFeedback = await _context.EmotionalFeedbacks
                .Include(e => e.Activity)
                .Include(e => e.Learner)
                .FirstOrDefaultAsync(e => e.FeedbackId == id);

            if (emotionalFeedback == null)
            {
                return NotFound();
            }

            return View(emotionalFeedback);
        }



// POST: EmotionalFeedback/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,LearnerId,ActivityId,Timestamp,EmotionalState")] EmotionalFeedback emotionalFeedback)
        {
            if (id != emotionalFeedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emotionalFeedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmotionalFeedbackExists(emotionalFeedback.FeedbackId))
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

            return View(emotionalFeedback);
        }
        


        // GET: EmotionalFeedback/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalFeedback = await _context.EmotionalFeedbacks
                .Include(e => e.Activity)
                .Include(e => e.Learner)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (emotionalFeedback == null)
            {
                return NotFound();
            }

            return View(emotionalFeedback);
        }

        // POST: EmotionalFeedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emotionalFeedback = await _context.EmotionalFeedbacks.FindAsync(id);
            _context.EmotionalFeedbacks.Remove(emotionalFeedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmotionalFeedbackExists(int id)
        {
            return _context.EmotionalFeedbacks.Any(e => e.FeedbackId == id);
        }
    }
}
