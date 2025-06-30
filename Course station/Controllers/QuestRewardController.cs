using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class QuestRewardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestRewardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: QuestReward
        public async Task<IActionResult> Index()
        {
            var questRewards = _context.QuestRewards
                .Include(qr => qr.Learner)
                .Include(qr => qr.Quest)
                .Include(qr => qr.Reward);
            return View(await questRewards.ToListAsync());
        }

        // GET: QuestReward/Details/5
        public async Task<IActionResult> Details(int? questId, int? rewardId, int? learnerId)
        {
            if (questId == null || rewardId == null || learnerId == null)
            {
                return NotFound();
            }

            var questReward = await _context.QuestRewards
                .Include(qr => qr.Learner)
                .Include(qr => qr.Quest)
                .Include(qr => qr.Reward)
                .FirstOrDefaultAsync(m => m.QuestId == questId && m.RewardId == rewardId && m.LearnerId == learnerId);
            if (questReward == null)
            {
                return NotFound();
            }

            return View(questReward);
        }

        // GET: QuestReward/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuestReward/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestId,RewardId,LearnerId,TimeEarned")] QuestReward questReward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questReward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(questReward);
        }

        // GET: QuestReward/Edit/5
        public async Task<IActionResult> Edit(int? questId, int? rewardId, int? learnerId)
        {
            if (questId == null || rewardId == null || learnerId == null)
            {
                return NotFound();
            }

            var questReward = await _context.QuestRewards.FindAsync(questId, rewardId, learnerId);
            if (questReward == null)
            {
                return NotFound();
            }
            return View(questReward);
        }

        // POST: QuestReward/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int questId, int rewardId, int learnerId, [Bind("QuestId,RewardId,LearnerId,TimeEarned")] QuestReward questReward)
        {
            if (questId != questReward.QuestId || rewardId != questReward.RewardId || learnerId != questReward.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questReward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestRewardExists(questReward.QuestId, questReward.RewardId, questReward.LearnerId))
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
            return View(questReward);
        }

        // GET: QuestReward/Delete/5
        public async Task<IActionResult> Delete(int? questId, int? rewardId, int? learnerId)
        {
            if (questId == null || rewardId == null || learnerId == null)
            {
                return NotFound();
            }

            var questReward = await _context.QuestRewards
                .Include(qr => qr.Learner)
                .Include(qr => qr.Quest)
                .Include(qr => qr.Reward)
                .FirstOrDefaultAsync(m => m.QuestId == questId && m.RewardId == rewardId && m.LearnerId == learnerId);
            if (questReward == null)
            {
                return NotFound();
            }

            return View(questReward);
        }

        // POST: QuestReward/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int questId, int rewardId, int learnerId)
        {
            var questReward = await _context.QuestRewards.FindAsync(questId, rewardId, learnerId);
            _context.QuestRewards.Remove(questReward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestRewardExists(int questId, int rewardId, int learnerId)
        {
            return _context.QuestRewards.Any(e => e.QuestId == questId && e.RewardId == rewardId && e.LearnerId == learnerId);
        }
    }
}
