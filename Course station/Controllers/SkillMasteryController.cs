using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class SkillMasteryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SkillMasteryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SkillMastery
        public async Task<IActionResult> Index()
        {
            var skillMasteries = _context.SkillMasteries
                .Include(sm => sm.Quest);
            return View(await skillMasteries.ToListAsync());
        }

        // GET: SkillMastery/Details/5
        public async Task<IActionResult> Details(int? questId, string skill)
        {
            if (questId == null || skill == null)
            {
                return NotFound();
            }

            var skillMastery = await _context.SkillMasteries
                .Include(sm => sm.Quest)
                .Include(sm => sm.LearnerMasteries)
                .FirstOrDefaultAsync(m => m.QuestId == questId && m.Skill == skill);
            if (skillMastery == null)
            {
                return NotFound();
            }

            return View(skillMastery);
        }

        // GET: SkillMastery/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SkillMastery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestId,Skill")] SkillMastery skillMastery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skillMastery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skillMastery);
        }

        // GET: SkillMastery/Edit/5
        public async Task<IActionResult> Edit(int? questId, string skill)
        {
            if (questId == null || skill == null)
            {
                return NotFound();
            }

            var skillMastery = await _context.SkillMasteries.FindAsync(questId, skill);
            if (skillMastery == null)
            {
                return NotFound();
            }
            return View(skillMastery);
        }

        // POST: SkillMastery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int questId, string skill, [Bind("QuestId,Skill")] SkillMastery skillMastery)
        {
            if (questId != skillMastery.QuestId || skill != skillMastery.Skill)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skillMastery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillMasteryExists(skillMastery.QuestId, skillMastery.Skill))
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
            return View(skillMastery);
        }

        // GET: SkillMastery/Delete/5
        public async Task<IActionResult> Delete(int? questId, string skill)
        {
            if (questId == null || skill == null)
            {
                return NotFound();
            }

            var skillMastery = await _context.SkillMasteries
                .Include(sm => sm.Quest)
                .FirstOrDefaultAsync(m => m.QuestId == questId && m.Skill == skill);
            if (skillMastery == null)
            {
                return NotFound();
            }

            return View(skillMastery);
        }

        // POST: SkillMastery/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int questId, string skill)
        {
            var skillMastery = await _context.SkillMasteries.FindAsync(questId, skill);
            _context.SkillMasteries.Remove(skillMastery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillMasteryExists(int questId, string skill)
        {
            return _context.SkillMasteries.Any(e => e.QuestId == questId && e.Skill == skill);
        }
    }
}
