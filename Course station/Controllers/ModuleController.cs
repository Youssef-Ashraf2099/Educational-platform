using Microsoft.AspNetCore.Mvc;
using Course_station.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Course_station.Controllers
{
    public class ModuleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Module
        public async Task<IActionResult> Index()
        {
            var modules = await _context.Modules.Include(m => m.Course).ToListAsync();
            return View(modules);
        }

        // GET: Module/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(m => m.Course)
                .Include(m => m.Assessments)
                .Include(m => m.ContentLibraries)
                .Include(m => m.DiscussionForums)
                .Include(m => m.LearningActivities)
                .Include(m => m.ModuleContents)
                .Include(m => m.TargetTraits)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // GET: Module/Create
        public IActionResult Create()
        {
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,CourseId,Title,DifficultyLevel,ContentUrl")] Module module)
        {
            bool isValid = true;

            // Manual validation for Title
            if (string.IsNullOrWhiteSpace(module.Title))
            {
                ModelState.AddModelError("Title", "Title is required.");
                isValid = false;
            }

            // Manual validation for CourseId
            if (module.CourseId <= 0)
            {
                ModelState.AddModelError("CourseId", "Please select a valid course.");
                isValid = false;
            }

           
            // Optional: Manual validation for ContentUrl
            if (!string.IsNullOrWhiteSpace(module.ContentUrl) && !Uri.IsWellFormedUriString(module.ContentUrl, UriKind.Absolute))
            {
                ModelState.AddModelError("ContentUrl", "Please provide a valid URL.");
                isValid = false;
            }

            // Check if all manual validations passed
            if (isValid)
            {
                try
                {
                    _context.Add(module);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Home", "Instructor");
                }
                catch (Exception ex)
                {
                    // Catch any database or other errors
                    ModelState.AddModelError(string.Empty, $"An error occurred while saving the module: {ex.Message}");
                }
            }

            // If validation fails, reload the form with errors
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", module.CourseId);
            return View(module);
        }




        //// GET: Module/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var module = await _context.Modules.FindAsync(id);
        //    if (module == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", module.CourseId);
        //    return View(module);
        //}

        //// POST: Module/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ModuleId,CourseId,Title,DifficultyLevel,ContentUrl")] Module module)
        //{
        //    if (id != module.ModuleId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(module);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ModuleExists(module.ModuleId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title", module.CourseId);
        //    return View(module);
        //}

        // GET: Module/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(m => m.Course)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: Module/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.ModuleId == id);
        }
    }
}
