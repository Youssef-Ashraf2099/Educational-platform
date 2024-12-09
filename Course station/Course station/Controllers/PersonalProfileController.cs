using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Course_station.Models;
using System.Threading.Tasks;

namespace Course_station.Controllers
{
    public class PersonalProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PersonalProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PersonalProfile
        public async Task<IActionResult> Index()
        {
            return View(await _context.PersonalProfiles.ToListAsync());
        }

        // GET: PersonalProfile/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personalProfile = await _context.PersonalProfiles
                .FirstOrDefaultAsync(m => m.ProfileId == id);
            if (personalProfile == null)
            {
                return NotFound();
            }

            return View(personalProfile);
        }


        //[HttpGet("PersonalProfile/Create/{learnerId}")]
        //public IActionResult Create(int learnerId)
        //{
        //    var personalProfile = new PersonalProfile
        //    {
        //        LearnerId = learnerId
        //    };
        //    return View(personalProfile);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PreferedContentType,EmotionalState,PersonalityType,LearnerId")] PersonalProfile personalProfile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(personalProfile);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(personalProfile);
        //}
        [HttpGet("PersonalProfile/Create/{learnerId}")]
        public IActionResult Create(int learnerId)
        {
            var personalProfile = new PersonalProfile
            {
                LearnerId = learnerId
            };
            return View(personalProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PreferedContentType,EmotionalState,PersonalityType,LearnerId")] PersonalProfile personalProfile)
        {
            if (personalProfile.LearnerId == 0)
            {
                ModelState.AddModelError("LearnerId", "LearnerId is required.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(personalProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           // Console.WriteLine(personalProfile.GetType()); // Should output PersonalProfile
            return View(personalProfile);
        }



        // GET: PersonalProfile/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personalProfile = await _context.PersonalProfiles
                .FirstOrDefaultAsync(m => m.ProfileId == id);
            if (personalProfile == null)
            {
                return NotFound();
            }

            return View(personalProfile);
        }

        // POST: PersonalProfile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personalProfile = await _context.PersonalProfiles.FindAsync(id);
            _context.PersonalProfiles.Remove(personalProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonalProfileExists(int id)
        {
            return _context.PersonalProfiles.Any(e => e.ProfileId == id);
        }
    }
}
