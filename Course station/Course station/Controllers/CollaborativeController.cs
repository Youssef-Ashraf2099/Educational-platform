using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class CollaborativeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollaborativeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Collaborative
        public async Task<IActionResult> Index()
        {
            var collaboratives = _context.Collaboratives.Include(c => c.Quest);
            return View(await collaboratives.ToListAsync());
        }

        // GET: Collaborative/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collaborative = await _context.Collaboratives
                .Include(c => c.Quest)
                .FirstOrDefaultAsync(m => m.QuestId == id);
            if (collaborative == null)
            {
                return NotFound();
            }

            return View(collaborative);
        }

        // GET: Collaborative/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collaborative/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestId,Deadline,MaxNumParticipants")] Collaborative collaborative)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collaborative);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collaborative);
        }

        // GET: Collaborative/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collaborative = await _context.Collaboratives.FindAsync(id);
            if (collaborative == null)
            {
                return NotFound();
            }
            return View(collaborative);
        }

        // POST: Collaborative/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestId,Deadline,MaxNumParticipants")] Collaborative collaborative)
        {
            if (id != collaborative.QuestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collaborative);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollaborativeExists(collaborative.QuestId))
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
            return View(collaborative);
        }

        // GET: Collaborative/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collaborative = await _context.Collaboratives
                .Include(c => c.Quest)
                .FirstOrDefaultAsync(m => m.QuestId == id);
            if (collaborative == null)
            {
                return NotFound();
            }

            return View(collaborative);
        }

        // POST: Collaborative/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collaborative = await _context.Collaboratives
                .Include(c => c.Quest)
                .FirstOrDefaultAsync(m => m.QuestId == id);
            if (collaborative == null)
            {
                return NotFound();
            }

            try
            {
                _context.Collaboratives.Remove(collaborative);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists, see your system administrator.");
                return View(collaborative);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CollaborativeExists(int id)
        {
            return _context.Collaboratives.Any(e => e.QuestId == id);
        }

    }
}
