using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class ContentLibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContentLibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContentLibrary
        public async Task<IActionResult> Index()
        {
            var contentLibraries = _context.ContentLibraries.Include(c => c.Module);
            return View(await contentLibraries.ToListAsync());
        }

        // GET: ContentLibrary/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentLibrary = await _context.ContentLibraries
                .Include(c => c.Module)
                .FirstOrDefaultAsync(m => m.LibId == id);
            if (contentLibrary == null)
            {
                return NotFound();
            }

            return View(contentLibrary);
        }

        // GET: ContentLibrary/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ContentLibrary/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LibId,ModuleId,CourseId,Title,Description,MetaData,Type,ContentUrl")] ContentLibrary contentLibrary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contentLibrary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contentLibrary);
        }

        // GET: ContentLibrary/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentLibrary = await _context.ContentLibraries.FindAsync(id);
            if (contentLibrary == null)
            {
                return NotFound();
            }
            return View(contentLibrary);
        }

        // POST: ContentLibrary/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LibId,ModuleId,CourseId,Title,Description,MetaData,Type,ContentUrl")] ContentLibrary contentLibrary)
        {
            if (id != contentLibrary.LibId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contentLibrary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentLibraryExists(contentLibrary.LibId))
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
            return View(contentLibrary);
        }

        // GET: ContentLibrary/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contentLibrary = await _context.ContentLibraries
                .Include(c => c.Module)
                .FirstOrDefaultAsync(m => m.LibId == id);
            if (contentLibrary == null)
            {
                return NotFound();
            }

            return View(contentLibrary);
        }

        // POST: ContentLibrary/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contentLibrary = await _context.ContentLibraries.FindAsync(id);
            _context.ContentLibraries.Remove(contentLibrary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContentLibraryExists(int id)
        {
            return _context.ContentLibraries.Any(e => e.LibId == id);
        }
    }
}
