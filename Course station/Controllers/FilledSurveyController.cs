using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class FilledSurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilledSurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FilledSurvey
        public async Task<IActionResult> Index()
        {
            var filledSurveys = _context.FilledSurveys.Include(f => f.Learner).Include(f => f.SurveyQuestion);
            return View(await filledSurveys.ToListAsync());
        }

        // GET: FilledSurvey/Details/5
        public async Task<IActionResult> Details(int? surveyId, string question, int? learnerId)
        {
            if (surveyId == null || question == null || learnerId == null)
            {
                return NotFound();
            }

            var filledSurvey = await _context.FilledSurveys
                .Include(f => f.Learner)
                .Include(f => f.SurveyQuestion)
                .FirstOrDefaultAsync(m => m.SurveyId == surveyId && m.Question == question && m.LearnerId == learnerId);
            if (filledSurvey == null)
            {
                return NotFound();
            }

            return View(filledSurvey);
        }

        // GET: FilledSurvey/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FilledSurvey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyId,Question,LearnerId,Answer")] FilledSurvey filledSurvey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(filledSurvey);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(filledSurvey);
        }

        // GET: FilledSurvey/Edit/5
        public async Task<IActionResult> Edit(int? surveyId, string question, int? learnerId)
        {
            if (surveyId == null || question == null || learnerId == null)
            {
                return NotFound();
            }

            var filledSurvey = await _context.FilledSurveys.FindAsync(surveyId, question, learnerId);
            if (filledSurvey == null)
            {
                return NotFound();
            }
            return View(filledSurvey);
        }

        // POST: FilledSurvey/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int surveyId, string question, int learnerId, [Bind("SurveyId,Question,LearnerId,Answer")] FilledSurvey filledSurvey)
        {
            if (surveyId != filledSurvey.SurveyId || question != filledSurvey.Question || learnerId != filledSurvey.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(filledSurvey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilledSurveyExists(filledSurvey.SurveyId, filledSurvey.Question, filledSurvey.LearnerId))
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
            return View(filledSurvey);
        }

        // GET: FilledSurvey/Delete/5
        public async Task<IActionResult> Delete(int? surveyId, string question, int? learnerId)
        {
            if (surveyId == null || question == null || learnerId == null)
            {
                return NotFound();
            }

            var filledSurvey = await _context.FilledSurveys
                .Include(f => f.Learner)
                .Include(f => f.SurveyQuestion)
                .FirstOrDefaultAsync(m => m.SurveyId == surveyId && m.Question == question && m.LearnerId == learnerId);
            if (filledSurvey == null)
            {
                return NotFound();
            }

            return View(filledSurvey);
        }

        // POST: FilledSurvey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int surveyId, string question, int learnerId)
        {
            var filledSurvey = await _context.FilledSurveys.FindAsync(surveyId, question, learnerId);
            _context.FilledSurveys.Remove(filledSurvey);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilledSurveyExists(int surveyId, string question, int learnerId)
        {
            return _context.FilledSurveys.Any(e => e.SurveyId == surveyId && e.Question == question && e.LearnerId == learnerId);
        }
    }
}
