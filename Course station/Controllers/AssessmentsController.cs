using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class AssessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssessmentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var assessments = await _context.Assessments
                .Include(a => a.Module)
                .Include(a => a.TakenAssessments)
                .ToListAsync();

            foreach (var assessment in assessments)
            {
                var analytics = assessment.TakenAssessments.Any()
                    ? new
                    {
                        AverageScore = assessment.TakenAssessments.Average(ta => ta.ScoredPoint ?? 0),
                        AttemptCount = assessment.TakenAssessments.Count(),
                        PassCount = assessment.TakenAssessments.Count(ta => ta.ScoredPoint >= assessment.PassingMarks)
                    }
                    : null;

                ViewData[$"Analytics_{assessment.AssessmentId}"] = analytics;
            }

            return View(assessments);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = await _context.Assessments
                .Include(a => a.TakenAssessments)
                .ThenInclude(ta => ta.Learner)
                .FirstOrDefaultAsync(a => a.AssessmentId == id);

            if (assessment == null)
            {
                return NotFound();
            }

            var analytics = assessment.TakenAssessments.Any()
                ? new
                {
                    AverageScore = assessment.TakenAssessments.Average(ta => ta.ScoredPoint ?? 0),
                    HighestScore = assessment.TakenAssessments.Max(ta => ta.ScoredPoint ?? 0),
                    LowestScore = assessment.TakenAssessments.Min(ta => ta.ScoredPoint ?? 0)
                }
                : null;

            ViewBag.AverageScore = analytics?.AverageScore;
            ViewBag.HighestScore = analytics?.HighestScore;
            ViewBag.LowestScore = analytics?.LowestScore;

            return View(assessment);
        }

        public IActionResult Take(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assessment = _context.Assessments
                .Include(a => a.TakenAssessments)
                .FirstOrDefault(a => a.AssessmentId == id);

            if (assessment == null)
            {
                return NotFound();
            }

            var takenAssessment = new TakenAssessment
            {
                AssessmentId = assessment.AssessmentId,
                Assessment = assessment
            };

            return View(takenAssessment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Take(int assessmentId, int ScoredPoint)
        {
            // Fetch the assessment details
            var assessment = await _context.Assessments
                .Include(a => a.TakenAssessments)
                .FirstOrDefaultAsync(a => a.AssessmentId == assessmentId);

            if (assessment == null)
            {
                TempData["Error"] = "Assessment not found.";
                return RedirectToAction("Index");
            }

            // Validate the score
            if (ScoredPoint > assessment.TotalMarks)
            {
                TempData["Error"] = "Score cannot exceed total marks.";
                return RedirectToAction("Take", new { id = assessmentId });
            }

            // Get LearnerId from session
            var learnerId = HttpContext.Session.GetInt32("LearnerId");
            if (learnerId == null || learnerId == 0)
            {
                TempData["Error"] = "You must be logged in to submit an assessment.";
                return RedirectToAction("Login", "Account");
            }

            // Check if the learner already has an entry for this assessment
            var existingAttempt = await _context.TakenAssessments
                .FirstOrDefaultAsync(ta => ta.AssessmentId == assessmentId && ta.LearnerId == learnerId);

            if (existingAttempt != null)
            {
                TempData["Error"] = "You have already submitted a score for this assessment.";
                return RedirectToAction("Details", "Learners", new { id = learnerId });
            }

            // Create new TakenAssessment
            var takenAssessment = new TakenAssessment
            {
                LearnerId = learnerId.Value,
                AssessmentId = assessmentId,
                ScoredPoint = ScoredPoint
            };

            // Add the new attempt and save to the database
            _context.TakenAssessments.Add(takenAssessment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your score has been successfully submitted.";
            return RedirectToAction("ViewScore", new { id = assessmentId, learnerId = learnerId });
        }





        // GET: Assessments/ViewScore
        public async Task<IActionResult> ViewScore(int id, int learnerId)
        {
            var takenAssessment = await _context.TakenAssessments
                .Include(ta => ta.Assessment)
                .Include(ta => ta.Learner)
                .FirstOrDefaultAsync(ta => ta.AssessmentId == id && ta.LearnerId == learnerId);

            if (takenAssessment == null)
            {
                return NotFound();
            }

            // Calculate analytics
            var analytics = await _context.TakenAssessments
                .Where(ta => ta.AssessmentId == id)
                .GroupBy(ta => ta.AssessmentId)
                .Select(g => new
                {
                    AverageScore = g.Average(ta => ta.ScoredPoint ?? 0),
                    HighestScore = g.Max(ta => ta.ScoredPoint ?? 0),
                    LowestScore = g.Min(ta => ta.ScoredPoint ?? 0),
                    TotalAttempts = g.Count()
                })
                .FirstOrDefaultAsync();

            ViewBag.Analytics = analytics;

            return View(takenAssessment);
        }
    }
}