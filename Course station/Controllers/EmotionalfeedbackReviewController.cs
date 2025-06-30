using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class EmotionalfeedbackReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmotionalfeedbackReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EmotionalfeedbackReview
        public async Task<IActionResult> Index()
        {
            var emotionalfeedbackReviews = _context.EmotionalfeedbackReviews.Include(e => e.Feedback).Include(e => e.Instructor);
            return View(await emotionalfeedbackReviews.ToListAsync());
        }

        // GET: EmotionalfeedbackReview/Details/5
        public async Task<IActionResult> Details(int? feedbackId, int? instructorId)
        {
            if (feedbackId == null || instructorId == null)
            {
                return NotFound();
            }

            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews
                .Include(e => e.Feedback)
                .Include(e => e.Instructor)
                .FirstOrDefaultAsync(m => m.FeedbackId == feedbackId && m.InstructorId == instructorId);
            if (emotionalfeedbackReview == null)
            {
                return NotFound();
            }

            return View(emotionalfeedbackReview);
        }

        // GET: EmotionalfeedbackReview/Create
        public IActionResult Create()
        {
            return View();
        }

// POST: EmotionalfeedbackReview/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerID,Activity_ID,Timestamp,EmotionalState")] EmotionalfeedbackReview emotionalfeedbackReview)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(emotionalfeedbackReview);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Ensure all foreign key constraints are met.");
                }
            }
            return View(emotionalfeedbackReview);
        }

      

        // GET: EmotionalfeedbackReview/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews.FindAsync(id);
            if (emotionalfeedbackReview == null)
            {
                return NotFound();
            }
            return View(emotionalfeedbackReview);
        }

// POST: EmotionalfeedbackReview/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,LearnerID,Activity_ID,Timestamp,EmotionalState")] EmotionalfeedbackReview emotionalfeedbackReview)
        {
            
            if (id != emotionalfeedbackReview.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emotionalfeedbackReview);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.EmotionalfeedbackReviews.Any(e => e.FeedbackId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Ensure all foreign key constraints are met.");
                }
            }
            return View(emotionalfeedbackReview);
        }


        // GET: EmotionalfeedbackReview/Delete/5
        public async Task<IActionResult> Delete(int? feedbackId, int? instructorId)
        {
            if (feedbackId == null || instructorId == null)
            {
                return NotFound();
            }

            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews
                .Include(e => e.Feedback)
                .Include(e => e.Instructor)
                .FirstOrDefaultAsync(m => m.FeedbackId == feedbackId && m.InstructorId == instructorId);
            if (emotionalfeedbackReview == null)
            {
                return NotFound();
            }

            return View(emotionalfeedbackReview);
        }

        // POST: EmotionalfeedbackReview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int feedbackId, int instructorId)
        {
            var emotionalfeedbackReview = await _context.EmotionalfeedbackReviews.FindAsync(feedbackId, instructorId);
            _context.EmotionalfeedbackReviews.Remove(emotionalfeedbackReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmotionalfeedbackReviewExists(int feedbackId, int instructorId)
        {
            return _context.EmotionalfeedbackReviews.Any(e => e.FeedbackId == feedbackId && e.InstructorId == instructorId);
        }
        
        public async Task<IActionResult> EmotionalTrendAnalysis(int courseId, int moduleId, DateTime timePeriod)
        {
            var result = await _context.EmotionalFeedbacks
                .FromSqlInterpolated($"EXEC EmotionalTrendAnalysis @CourseID = {courseId}, @ModuleID = {moduleId}, @TimePeriod = {timePeriod}")
                .ToListAsync();

            if (!result.Any())
            {
                ViewBag.Message = "No emotional feedback data found for the specified course and module.";
                return View("NoData");
            }

            return View(result);
        }

        
        

    }
}
