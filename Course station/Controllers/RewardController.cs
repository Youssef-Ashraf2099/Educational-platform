using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class RewardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RewardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reward
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

        // GET: Reward/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reward/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RewardId,Value,Description,Type")] Reward reward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reward);
        }

        // GET: Reward/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reward = await _context.Rewards.FindAsync(id);
            if (reward == null)
            {
                return NotFound();
            }
            return View(reward);
        }

        // POST: Reward/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RewardId,Value,Description,Type")] Reward reward)
        {
            if (id != reward.RewardId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RewardExists(reward.RewardId))
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
            return View(reward);
        }

        // GET: Reward/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reward = await _context.Rewards
                .Include(r => r.QuestRewards)
                .FirstOrDefaultAsync(m => m.RewardId == id);
            if (reward == null)
            {
                return NotFound();
            }

            return View(reward);
        }

        // POST: Reward/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reward = await _context.Rewards.FindAsync(id);
            _context.Rewards.Remove(reward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RewardExists(int id)
        {
            return _context.Rewards.Any(e => e.RewardId == id);
        }
    }
}
