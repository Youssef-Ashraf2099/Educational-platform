using Microsoft.AspNetCore.Mvc;
using Course_station.Models;
using Microsoft.EntityFrameworkCore;

namespace Course_station.Controllers
{
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateModule(Module module)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Modules.Add(module);
        //        await _context.SaveChangesAsync();

        //        // Redirect to Instructor Home
        //        return RedirectToAction("Home", "Instructor");
        //    }
        //    return View(module);
        //}

        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
        public IActionResult DetailsForLearner(int id)
        {
            var course = _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefault(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View("DetailsForLearner", course);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageCourses", "Instructor");
            }
            return View(course);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,Description")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
            return View(course);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }


        public async Task<IActionResult> Modules(int courseId)
        {
            var modules = await _context.Modules
                .Where(m => m.CourseId == courseId)
                .ToListAsync();
            return View(modules);
        }

        public IActionResult CreateModule(int courseId)
        {
            var module = new Module { CourseId = courseId };
            return View(module);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModule(Module module)
        {
            if (ModelState.IsValid)
            {
                _context.Modules.Add(module);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = module.CourseId });
            }
            return View(module);
        }

        public IActionResult CreateAssessment(int courseId)
        {
            var assessment = new Assessment { CourseId = courseId };
            return View(assessment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssessment(Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                _context.Assessments.Add(assessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = assessment.CourseId });
            }
            return View(assessment);
        }


        public async Task<IActionResult> TakenCourseDetails(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }


    }
}
