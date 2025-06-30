using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Leaderboard
        public async Task<IActionResult> Index()
        {
            return View(await _context.Leaderboards.ToListAsync());
        }

        // GET: Leaderboard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaderboard = await _context.Leaderboards
                .Include(l => l.Rankings)
                .ThenInclude(r => r.Learner)
                .Include(l => l.Rankings)
                .ThenInclude(r => r.Course)
                .FirstOrDefaultAsync(m => m.BoardId == id);
            if (leaderboard == null)
            {
                return NotFound();
            }

            return View(leaderboard);
        }

        // GET: Leaderboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Leaderboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardId,Season")] Leaderboard leaderboard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaderboard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaderboard);
        }

        // GET: Leaderboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaderboard = await _context.Leaderboards.FindAsync(id);
            if (leaderboard == null)
            {
                return NotFound();
            }
            return View(leaderboard);
        }

        // POST: Leaderboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoardId,Season")] Leaderboard leaderboard)
        {
            if (id != leaderboard.BoardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaderboard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaderboardExists(leaderboard.BoardId))
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
            return View(leaderboard);
        }

        // GET: Leaderboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaderboard = await _context.Leaderboards
                .Include(l => l.Rankings)
                .FirstOrDefaultAsync(m => m.BoardId == id);
            if (leaderboard == null)
            {
                return NotFound();
            }

            return View(leaderboard);
        }

        // POST: Leaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaderboard = await _context.Leaderboards.FindAsync(id);
            _context.Leaderboards.Remove(leaderboard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaderboardExists(int id)
        {
            return _context.Leaderboards.Any(e => e.BoardId == id);
        }
    }
}
