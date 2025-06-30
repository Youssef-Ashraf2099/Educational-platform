using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class SkillProgressionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SkillProgressionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SkillProgression
        public async Task<IActionResult> Index()
        {
            var skillProgressions = _context.SkillProgressions.Include(sp => sp.Skill);
            return View(await skillProgressions.ToListAsync());
        }

        // GET: SkillProgression/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillProgression = await _context.SkillProgressions
                .Include(sp => sp.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (skillProgression == null)
            {
                return NotFound();
            }

            return View(skillProgression);
        }

        // GET: SkillProgression/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SkillProgression/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProficiencyLevel,LearnerId,SkillName,Timestamp")] SkillProgression skillProgression)
        {
            if (ModelState.IsValid)
            {
                _context.Add(skillProgression);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skillProgression);
        }

        // GET: SkillProgression/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillProgression = await _context.SkillProgressions.FindAsync(id);
            if (skillProgression == null)
            {
                return NotFound();
            }
            return View(skillProgression);
        }

        // POST: SkillProgression/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProficiencyLevel,LearnerId,SkillName,Timestamp")] SkillProgression skillProgression)
        {
            if (id != skillProgression.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(skillProgression);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillProgressionExists(skillProgression.Id))
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
            return View(skillProgression);
        }

        // GET: SkillProgression/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var skillProgression = await _context.SkillProgressions
                .Include(sp => sp.Skill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (skillProgression == null)
            {
                return NotFound();
            }

            return View(skillProgression);
        }

        // POST: SkillProgression/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skillProgression = await _context.SkillProgressions.FindAsync(id);
            _context.SkillProgressions.Remove(skillProgression);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillProgressionExists(int id)
        {
            return _context.SkillProgressions.Any(e => e.Id == id);
        }
    }
}
