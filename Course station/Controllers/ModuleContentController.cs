using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class ModuleContentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleContentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ModuleContent
        public async Task<IActionResult> Index()
        {
            var moduleContents = _context.ModuleContents.Include(mc => mc.Module);
            return View(await moduleContents.ToListAsync());
        }

        // GET: ModuleContent/Details/5
        public async Task<IActionResult> Details(int? moduleId, int? courseId, string contetntType)
        {
            if (moduleId == null || courseId == null || contetntType == null)
            {
                return NotFound();
            }

            var moduleContent = await _context.ModuleContents
                .Include(mc => mc.Module)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId && m.CourseId == courseId && m.ContetntType == contetntType);
            if (moduleContent == null)
            {
                return NotFound();
            }

            return View(moduleContent);
        }

        // GET: ModuleContent/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ModuleContent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,CourseId,ContetntType")] ModuleContent moduleContent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(moduleContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(moduleContent);
        }

        // GET: ModuleContent/Edit/5
        public async Task<IActionResult> Edit(int? moduleId, int? courseId, string contetntType)
        {
            if (moduleId == null || courseId == null || contetntType == null)
            {
                return NotFound();
            }

            var moduleContent = await _context.ModuleContents.FindAsync(moduleId, courseId, contetntType);
            if (moduleContent == null)
            {
                return NotFound();
            }
            return View(moduleContent);
        }

        // POST: ModuleContent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int moduleId, int courseId, string contetntType, [Bind("ModuleId,CourseId,ContetntType")] ModuleContent moduleContent)
        {
            if (moduleId != moduleContent.ModuleId || courseId != moduleContent.CourseId || contetntType != moduleContent.ContetntType)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(moduleContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleContentExists(moduleContent.ModuleId, moduleContent.CourseId, moduleContent.ContetntType))
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
            return View(moduleContent);
        }

        // GET: ModuleContent/Delete/5
        public async Task<IActionResult> Delete(int? moduleId, int? courseId, string contetntType)
        {
            if (moduleId == null || courseId == null || contetntType == null)
            {
                return NotFound();
            }

            var moduleContent = await _context.ModuleContents
                .Include(mc => mc.Module)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId && m.CourseId == courseId && m.ContetntType == contetntType);
            if (moduleContent == null)
            {
                return NotFound();
            }

            return View(moduleContent);
        }

        // POST: ModuleContent/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int moduleId, int courseId, string contetntType)
        {
            var moduleContent = await _context.ModuleContents.FindAsync(moduleId, courseId, contetntType);
            _context.ModuleContents.Remove(moduleContent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleContentExists(int moduleId, int courseId, string contetntType)
        {
            return _context.ModuleContents.Any(e => e.ModuleId == moduleId && e.CourseId == courseId && e.ContetntType == contetntType);
        }
    }
}
