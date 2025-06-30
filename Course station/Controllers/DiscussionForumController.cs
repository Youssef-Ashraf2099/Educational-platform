using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class DiscussionForumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscussionForumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DiscussionForum
        public async Task<IActionResult> Index()
        {
            var discussionForums = _context.DiscussionForums.Include(d => d.Module);
            return View(await discussionForums.ToListAsync());
        }

        // GET: DiscussionForum/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionForum = await _context.DiscussionForums
                .Include(d => d.Module)
                .FirstOrDefaultAsync(m => m.ForumId == id);
            if (discussionForum == null)
            {
                return NotFound();
            }

            return View(discussionForum);
        }

        // GET: DiscussionForum/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiscussionForum/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ForumId,CourseId,ModuleId,Title,LastActive,Timestamp,Description")] DiscussionForum discussionForum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discussionForum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discussionForum);
        }

        // GET: DiscussionForum/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionForum = await _context.DiscussionForums.FindAsync(id);
            if (discussionForum == null)
            {
                return NotFound();
            }
            return View(discussionForum);
        }

        // POST: DiscussionForum/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ForumId,CourseId,ModuleId,Title,LastActive,Timestamp,Description")] DiscussionForum discussionForum)
        {
            if (id != discussionForum.ForumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussionForum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionForumExists(discussionForum.ForumId))
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
            return View(discussionForum);
        }

        // GET: DiscussionForum/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionForum = await _context.DiscussionForums
                .Include(d => d.Module)
                .FirstOrDefaultAsync(m => m.ForumId == id);
            if (discussionForum == null)
            {
                return NotFound();
            }

            return View(discussionForum);
        }

        // POST: DiscussionForum/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussionForum = await _context.DiscussionForums.FindAsync(id);
            _context.DiscussionForums.Remove(discussionForum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionForumExists(int id)
        {
            return _context.DiscussionForums.Any(e => e.ForumId == id);
        }
    }
}
