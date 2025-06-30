using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class CourseEnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseEnrollment
        public async Task<IActionResult> Index()
        {
            var courseEnrollments = _context.CourseEnrollments.Include(c => c.Course).Include(c => c.Learner);
            return View(await courseEnrollments.ToListAsync());
        }

        // GET: CourseEnrollment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseEnrollment = await _context.CourseEnrollments
                .Include(c => c.Course)
                .Include(c => c.Learner)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (courseEnrollment == null)
            {
                return NotFound();
            }

            return View(courseEnrollment);
        }

        // GET: CourseEnrollment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseEnrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentId,LearnerId,CourseId,EnrollmentDate,CompletionDate,Status")] CourseEnrollment courseEnrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseEnrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(courseEnrollment);
        }

        // GET: CourseEnrollment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseEnrollment = await _context.CourseEnrollments.FindAsync(id);
            if (courseEnrollment == null)
            {
                return NotFound();
            }
            return View(courseEnrollment);
        }

        // POST: CourseEnrollment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentId,LearnerId,CourseId,EnrollmentDate,CompletionDate,Status")] CourseEnrollment courseEnrollment)
        {
            if (id != courseEnrollment.EnrollmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseEnrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseEnrollmentExists(courseEnrollment.EnrollmentId))
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
            return View(courseEnrollment);
        }

        // GET: CourseEnrollment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseEnrollment = await _context.CourseEnrollments
                .Include(c => c.Course)
                .Include(c => c.Learner)
                .FirstOrDefaultAsync(m => m.EnrollmentId == id);
            if (courseEnrollment == null)
            {
                return NotFound();
            }

            return View(courseEnrollment);
        }

        // POST: CourseEnrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseEnrollment = await _context.CourseEnrollments.FindAsync(id);
            _context.CourseEnrollments.Remove(courseEnrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseEnrollmentExists(int id)
        {
            return _context.CourseEnrollments.Any(e => e.EnrollmentId == id);
        }
    }
}
