using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class RankingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RankingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ranking
        public async Task<IActionResult> Index()
        {
            var rankings = _context.Rankings
                .Include(r => r.Board)
                .Include(r => r.Course)
                .Include(r => r.Learner);
            return View(await rankings.ToListAsync());
        }

        // GET: Ranking/Details/5
        public async Task<IActionResult> Details(int? boardId, int? learnerId)
        {
            if (boardId == null || learnerId == null)
            {
                return NotFound();
            }

            var ranking = await _context.Rankings
                .Include(r => r.Board)
                .Include(r => r.Course)
                .Include(r => r.Learner)
                .FirstOrDefaultAsync(m => m.BoardId == boardId && m.LearnerId == learnerId);
            if (ranking == null)
            {
                return NotFound();
            }

            return View(ranking);
        }

        // GET: Ranking/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ranking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardId,LearnerId,CourseId,Rank,TotalPoints")] Ranking ranking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ranking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ranking);
        }

        // GET: Ranking/Edit/5
        public async Task<IActionResult> Edit(int? boardId, int? learnerId)
        {
            if (boardId == null || learnerId == null)
            {
                return NotFound();
            }

            var ranking = await _context.Rankings.FindAsync(boardId, learnerId);
            if (ranking == null)
            {
                return NotFound();
            }
            return View(ranking);
        }

        // POST: Ranking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int boardId, int learnerId, [Bind("BoardId,LearnerId,CourseId,Rank,TotalPoints")] Ranking ranking)
        {
            if (boardId != ranking.BoardId || learnerId != ranking.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ranking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RankingExists(ranking.BoardId, ranking.LearnerId))
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
            return View(ranking);
        }

        // GET: Ranking/Delete/5
        public async Task<IActionResult> Delete(int? boardId, int? learnerId)
        {
            if (boardId == null || learnerId == null)
            {
                return NotFound();
            }

            var ranking = await _context.Rankings
                .Include(r => r.Board)
                .Include(r => r.Course)
                .Include(r => r.Learner)
                .FirstOrDefaultAsync(m => m.BoardId == boardId && m.LearnerId == learnerId);
            if (ranking == null)
            {
                return NotFound();
            }

            return View(ranking);
        }

        // POST: Ranking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int boardId, int learnerId)
        {
            var ranking = await _context.Rankings.FindAsync(boardId, learnerId);
            _context.Rankings.Remove(ranking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RankingExists(int boardId, int learnerId)
        {
            return _context.Rankings.Any(e => e.BoardId == boardId && e.LearnerId == learnerId);
        }
    }
}
