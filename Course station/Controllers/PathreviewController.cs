using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class PathreviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PathreviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pathreview
        public async Task<IActionResult> Index()
        {
            var pathreviews = _context.Pathreviews.Include(p => p.Instructor).Include(p => p.Path);
            return View(await pathreviews.ToListAsync());
        }

        // GET: Pathreview/Details/5
        public async Task<IActionResult> Details(int? instructorId, int? pathId)
        {
            if (instructorId == null || pathId == null)
            {
                return NotFound();
            }

            var pathreview = await _context.Pathreviews
                .Include(p => p.Instructor)
                .Include(p => p.Path)
                .FirstOrDefaultAsync(m => m.InstructorId == instructorId && m.PathId == pathId);
            if (pathreview == null)
            {
                return NotFound();
            }

            return View(pathreview);
        }

        // GET: Pathreview/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pathreview/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorId,PathId,Feedback")] Pathreview pathreview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pathreview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pathreview);
        }

        // GET: Pathreview/Edit/5
        public async Task<IActionResult> Edit(int? instructorId, int? pathId)
        {
            if (instructorId == null || pathId == null)
            {
                return NotFound();
            }

            var pathreview = await _context.Pathreviews.FindAsync(instructorId, pathId);
            if (pathreview == null)
            {
                return NotFound();
            }
            return View(pathreview);
        }

        // POST: Pathreview/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int instructorId, int pathId, [Bind("InstructorId,PathId,Feedback")] Pathreview pathreview)
        {
            if (instructorId != pathreview.InstructorId || pathId != pathreview.PathId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pathreview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PathreviewExists(pathreview.InstructorId, pathreview.PathId))
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
            return View(pathreview);
        }

        // GET: Pathreview/Delete/5
        public async Task<IActionResult> Delete(int? instructorId, int? pathId)
        {
            if (instructorId == null || pathId == null)
            {
                return NotFound();
            }

            var pathreview = await _context.Pathreviews
                .Include(p => p.Instructor)
                .Include(p => p.Path)
                .FirstOrDefaultAsync(m => m.InstructorId == instructorId && m.PathId == pathId);
            if (pathreview == null)
            {
                return NotFound();
            }

            return View(pathreview);
        }

        // POST: Pathreview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int instructorId, int pathId)
        {
            var pathreview = await _context.Pathreviews.FindAsync(instructorId, pathId);
            _context.Pathreviews.Remove(pathreview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PathreviewExists(int instructorId, int pathId)
        {
            return _context.Pathreviews.Any(e => e.InstructorId == instructorId && e.PathId == pathId);
        }
    }
}
