using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class InteractionLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InteractionLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InteractionLog
        public async Task<IActionResult> Index()
        {
            var interactionLogs = _context.InteractionLogs.Include(i => i.Activity).Include(i => i.Learner);
            return View(await interactionLogs.ToListAsync());
        }

        // GET: InteractionLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interactionLog = await _context.InteractionLogs
                .Include(i => i.Activity)
                .Include(i => i.Learner)
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (interactionLog == null)
            {
                return NotFound();
            }

            return View(interactionLog);
        }

        // GET: InteractionLog/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InteractionLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LogId,ActivityId,LearnerId,Duration,Timestamp,ActionType")] InteractionLog interactionLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(interactionLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(interactionLog);
        }

        // GET: InteractionLog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interactionLog = await _context.InteractionLogs.FindAsync(id);
            if (interactionLog == null)
            {
                return NotFound();
            }
            return View(interactionLog);
        }

        // POST: InteractionLog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LogId,ActivityId,LearnerId,Duration,Timestamp,ActionType")] InteractionLog interactionLog)
        {
            if (id != interactionLog.LogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(interactionLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InteractionLogExists(interactionLog.LogId))
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
            return View(interactionLog);
        }

        // GET: InteractionLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var interactionLog = await _context.InteractionLogs
                .Include(i => i.Activity)
                .Include(i => i.Learner)
                .FirstOrDefaultAsync(m => m.LogId == id);
            if (interactionLog == null)
            {
                return NotFound();
            }

            return View(interactionLog);
        }

        // POST: InteractionLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var interactionLog = await _context.InteractionLogs.FindAsync(id);
            _context.InteractionLogs.Remove(interactionLog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InteractionLogExists(int id)
        {
            return _context.InteractionLogs.Any(e => e.LogId == id);
        }
    }
}
