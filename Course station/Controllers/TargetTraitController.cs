using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class TargetTraitController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TargetTraitController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TargetTrait
        public async Task<IActionResult> Index()
        {
            var targetTraits = _context.TargetTraits
                .Include(t => t.Module);
            return View(await targetTraits.ToListAsync());
        }

        // GET: TargetTrait/Details/5
        public async Task<IActionResult> Details(int? moduleId, int? courseId, string trait)
        {
            if (moduleId == null || courseId == null || trait == null)
            {
                return NotFound();
            }

            var targetTrait = await _context.TargetTraits
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId && m.CourseId == courseId && m.Trait == trait);
            if (targetTrait == null)
            {
                return NotFound();
            }

            return View(targetTrait);
        }

        // GET: TargetTrait/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TargetTrait/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,CourseId,Trait")] TargetTrait targetTrait)
        {
            if (ModelState.IsValid)
            {
                _context.Add(targetTrait);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(targetTrait);
        }

        // GET: TargetTrait/Edit/5
        public async Task<IActionResult> Edit(int? moduleId, int? courseId, string trait)
        {
            if (moduleId == null || courseId == null || trait == null)
            {
                return NotFound();
            }

            var targetTrait = await _context.TargetTraits.FindAsync(moduleId, courseId, trait);
            if (targetTrait == null)
            {
                return NotFound();
            }
            return View(targetTrait);
        }

        // POST: TargetTrait/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int moduleId, int courseId, string trait, [Bind("ModuleId,CourseId,Trait")] TargetTrait targetTrait)
        {
            if (moduleId != targetTrait.ModuleId || courseId != targetTrait.CourseId || trait != targetTrait.Trait)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(targetTrait);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TargetTraitExists(targetTrait.ModuleId, targetTrait.CourseId, targetTrait.Trait))
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
            return View(targetTrait);
        }

        // GET: TargetTrait/Delete/5
        public async Task<IActionResult> Delete(int? moduleId, int? courseId, string trait)
        {
            if (moduleId == null || courseId == null || trait == null)
            {
                return NotFound();
            }

            var targetTrait = await _context.TargetTraits
                .Include(t => t.Module)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId && m.CourseId == courseId && m.Trait == trait);
            if (targetTrait == null)
            {
                return NotFound();
            }

            return View(targetTrait);
        }

        // POST: TargetTrait/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int moduleId, int courseId, string trait)
        {
            var targetTrait = await _context.TargetTraits.FindAsync(moduleId, courseId, trait);
            _context.TargetTraits.Remove(targetTrait);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TargetTraitExists(int moduleId, int courseId, string trait)
        {
            return _context.TargetTraits.Any(e => e.ModuleId == moduleId && e.CourseId == courseId && e.Trait == trait);
        }
    }
}
