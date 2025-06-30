using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class BadgeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BadgeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Badge
        public async Task<IActionResult> Index()
        {
            return View(await _context.Badges.ToListAsync());
        }

        // GET: Badge/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badges
                .FirstOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return NotFound();
            }

            return View(badge);
        }

        // GET: Badge/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Badge/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BadgeId,Title,Description,Criteria,Points")] Badge badge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(badge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(badge);
        }

        // GET: Badge/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badges.FindAsync(id);
            if (badge == null)
            {
                return NotFound();
            }
            return View(badge);
        }

        // POST: Badge/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BadgeId,Title,Description,Criteria,Points")] Badge badge)
        {
            if (id != badge.BadgeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(badge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BadgeExists(badge.BadgeId))
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
            return View(badge);
        }

        // GET: Badge/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badges
                .FirstOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return NotFound();
            }

            return View(badge);
        }

        // POST: Badge/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var badge = await _context.Badges.FindAsync(id);
            _context.Badges.Remove(badge);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BadgeExists(int id)
        {
            return _context.Badges.Any(e => e.BadgeId == id);
        }
    }
}
