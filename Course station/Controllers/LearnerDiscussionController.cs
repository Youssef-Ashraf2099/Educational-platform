using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class LearnerDiscussionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearnerDiscussionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LearnerDiscussion
        public async Task<IActionResult> Index()
        {
            var learnerDiscussions = _context.LearnerDiscussions.Include(ld => ld.Forum).Include(ld => ld.Learner);
            return View(await learnerDiscussions.ToListAsync());
        }

        // GET: LearnerDiscussion/Details/5
        public async Task<IActionResult> Details(int? forumId, int? learnerId, string post)
        {
            if (forumId == null || learnerId == null || post == null)
            {
                return NotFound();
            }

            var learnerDiscussion = await _context.LearnerDiscussions
                .Include(ld => ld.Forum)
                .Include(ld => ld.Learner)
                .FirstOrDefaultAsync(m => m.ForumId == forumId && m.LearnerId == learnerId && m.Post == post);
            if (learnerDiscussion == null)
            {
                return NotFound();
            }

            return View(learnerDiscussion);
        }

        // GET: LearnerDiscussion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LearnerDiscussion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ForumId,LearnerId,Post,Time")] LearnerDiscussion learnerDiscussion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learnerDiscussion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learnerDiscussion);
        }

        // GET: LearnerDiscussion/Edit/5
        public async Task<IActionResult> Edit(int? forumId, int? learnerId, string post)
        {
            if (forumId == null || learnerId == null || post == null)
            {
                return NotFound();
            }

            var learnerDiscussion = await _context.LearnerDiscussions.FindAsync(forumId, learnerId, post);
            if (learnerDiscussion == null)
            {
                return NotFound();
            }
            return View(learnerDiscussion);
        }

        // POST: LearnerDiscussion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int forumId, int learnerId, string post, [Bind("ForumId,LearnerId,Post,Time")] LearnerDiscussion learnerDiscussion)
        {
            if (forumId != learnerDiscussion.ForumId || learnerId != learnerDiscussion.LearnerId || post != learnerDiscussion.Post)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learnerDiscussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerDiscussionExists(learnerDiscussion.ForumId, learnerDiscussion.LearnerId, learnerDiscussion.Post))
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
            return View(learnerDiscussion);
        }

        // GET: LearnerDiscussion/Delete/5
        public async Task<IActionResult> Delete(int? forumId, int? learnerId, string post)
        {
            if (forumId == null || learnerId == null || post == null)
            {
                return NotFound();
            }

            var learnerDiscussion = await _context.LearnerDiscussions
                .Include(ld => ld.Forum)
                .Include(ld => ld.Learner)
                .FirstOrDefaultAsync(m => m.ForumId == forumId && m.LearnerId == learnerId && m.Post == post);
            if (learnerDiscussion == null)
            {
                return NotFound();
            }

            return View(learnerDiscussion);
        }

        // POST: LearnerDiscussion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int forumId, int learnerId, string post)
        {
            var learnerDiscussion = await _context.LearnerDiscussions.FindAsync(forumId, learnerId, post);
            _context.LearnerDiscussions.Remove(learnerDiscussion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearnerDiscussionExists(int forumId, int learnerId, string post)
        {
            return _context.LearnerDiscussions.Any(e => e.ForumId == forumId && e.LearnerId == learnerId && e.Post == post);
        }
    }
}
