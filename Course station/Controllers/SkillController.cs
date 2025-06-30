using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class SkillController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SkillController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Skill
        public async Task<IActionResult> Index()
        {
            var skills = _context.Skills.Include(s => s.Learner);
            return View(await skills.ToListAsync());
        }

        // GET: Skill/Details/5
        public async Task<IActionResult> Details(int? learnerId, string skillName)
        {
            if (learnerId == null || skillName == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills
                .Include(s => s.Learner)
                .Include(s => s.SkillProgressions)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.Skill1 == skillName);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // GET: Skill/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Skill/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,Skill1")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skill/Edit/5
        public async Task<IActionResult> Edit(int? learnerId, string skillName)
        {
            if (learnerId == null || skillName == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills.FindAsync(learnerId, skillName);
            if (skill == null)
            {
                return NotFound();
            }
            return View(skill);
        }

        // POST: Skill/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int learnerId, string skillName, [Bind("LearnerId,Skill1")] Skill skill)
        {
            if (learnerId != skill.LearnerId || skillName != skill.Skill1)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.LearnerId, skill.Skill1))
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
            return View(skill);
        }

        // GET: Skill/Delete/5
        public async Task<IActionResult> Delete(int? learnerId, string skillName)
        {
            if (learnerId == null || skillName == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills
                .Include(s => s.Learner)
                .FirstOrDefaultAsync(m => m.LearnerId == learnerId && m.Skill1 == skillName);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // POST: Skill/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int learnerId, string skillName)
        {
            var skill = await _context.Skills.FindAsync(learnerId, skillName);
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(int learnerId, string skillName)
        {
            return _context.Skills.Any(e => e.LearnerId == learnerId && e.Skill1 == skillName);
        }
    }
}
