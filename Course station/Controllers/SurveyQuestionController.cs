using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class SurveyQuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyQuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurveyQuestion
        public async Task<IActionResult> Index()
        {
            var surveyQuestions = _context.SurveyQuestions
                .Include(sq => sq.Survey);
            return View(await surveyQuestions.ToListAsync());
        }

        // GET: SurveyQuestion/Details/5
        public async Task<IActionResult> Details(int? surveyId, string question)
        {
            if (surveyId == null || question == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestions
                .Include(sq => sq.Survey)
                .Include(sq => sq.FilledSurveys)
                .FirstOrDefaultAsync(m => m.SurveyId == surveyId && m.Question == question);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return View(surveyQuestion);
        }

        // GET: SurveyQuestion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SurveyQuestion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SurveyId,Question")] SurveyQuestion surveyQuestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surveyQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surveyQuestion);
        }

        // GET: SurveyQuestion/Edit/5
        public async Task<IActionResult> Edit(int? surveyId, string question)
        {
            if (surveyId == null || question == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestions.FindAsync(surveyId, question);
            if (surveyQuestion == null)
            {
                return NotFound();
            }
            return View(surveyQuestion);
        }

        // POST: SurveyQuestion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int surveyId, string question, [Bind("SurveyId,Question")] SurveyQuestion surveyQuestion)
        {
            if (surveyId != surveyQuestion.SurveyId || question != surveyQuestion.Question)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surveyQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyQuestionExists(surveyQuestion.SurveyId, surveyQuestion.Question))
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
            return View(surveyQuestion);
        }

        // GET: SurveyQuestion/Delete/5
        public async Task<IActionResult> Delete(int? surveyId, string question)
        {
            if (surveyId == null || question == null)
            {
                return NotFound();
            }

            var surveyQuestion = await _context.SurveyQuestions
                .Include(sq => sq.Survey)
                .FirstOrDefaultAsync(m => m.SurveyId == surveyId && m.Question == question);
            if (surveyQuestion == null)
            {
                return NotFound();
            }

            return View(surveyQuestion);
        }

        // POST: SurveyQuestion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int surveyId, string question)
        {
            var surveyQuestion = await _context.SurveyQuestions.FindAsync(surveyId, question);
            _context.SurveyQuestions.Remove(surveyQuestion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyQuestionExists(int surveyId, string question)
        {
            return _context.SurveyQuestions.Any(e => e.SurveyId == surveyId && e.Question == question);
        }
    }
}
