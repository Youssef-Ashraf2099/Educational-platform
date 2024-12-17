using Course_station.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Course_station.Controllers
{
    public class PersonalProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonalProfileController> _logger;

        public PersonalProfileController(ApplicationDbContext context, ILogger<PersonalProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var personalProfiles = await _context.PersonalProfiles
                .Include(p => p.Learner)
                .ToListAsync();
            return View(personalProfiles);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Create Personal Profile";

            // Get list of learners with both ID and full name
            var learners = _context.Learners
                .Select(l => new
                {
                    LearnerId = l.LearnerId,
                    FullName = $"{l.FirstName} {l.LastName}"
                })
                .ToList();

            // Log the number of learners found
            _logger.LogInformation($"Found {learners.Count} learners for dropdown");

            ViewData["Learners"] = new SelectList(learners, "LearnerId", "FullName");
            return View(new PersonalProfile());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfileId,LearnerId,PreferedContentType,EmotionalState,PersonalityType")] PersonalProfile personalProfile)
        {

            // Add this logging
            _logger.LogInformation($"Received LearnerId: {personalProfile.LearnerId}");
            foreach (var key in Request.Form.Keys)
            {
                _logger.LogInformation($"Form field {key}: {Request.Form[key]}");
            }
            // Log the received data
            _logger.LogInformation($"Received profile creation request - LearnerId: {personalProfile.LearnerId}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid");
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        _logger.LogError($"Validation error: {error.ErrorMessage}");
                    }
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Verify if the learner exists
                    var learnerExists = await _context.Learners
                        .AnyAsync(l => l.LearnerId == personalProfile.LearnerId);

                    if (!learnerExists)
                    {
                        ModelState.AddModelError("LearnerId", "Selected learner does not exist");
                        _logger.LogError($"Attempted to create profile for non-existent LearnerId: {personalProfile.LearnerId}");
                    }
                    else
                    {
                        _context.Add(personalProfile);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Successfully created profile for LearnerId: {personalProfile.LearnerId}");
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating personal profile");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the personal profile. Please try again.");
            }

            // If we get here, something went wrong - reload the form
            ViewData["Title"] = "Create Personal Profile";
            var learners = _context.Learners
                .Select(l => new
                {
                    LearnerId = l.LearnerId,
                    FullName = $"{l.FirstName} {l.LastName}"
                })
                .ToList();
            ViewData["Learners"] = new SelectList(learners, "LearnerId", "FullName", personalProfile.LearnerId);
            return View(personalProfile);
        }

        

public async Task<IActionResult> Edit(int? profileId, int? learnerId)
        {
            if (profileId == null || learnerId == null)
            {
                return NotFound();
            }

            var personalProfile = await _context.PersonalProfiles
                .FirstOrDefaultAsync(p => p.ProfileId == profileId && p.LearnerId == learnerId);
            if (personalProfile == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Edit Personal Profile";
            return View(personalProfile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfileId,LearnerId,PreferedContentType,EmotionalState,PersonalityType")] PersonalProfile personalProfile)
        {
            if (id != personalProfile.ProfileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personalProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonalProfileExists(personalProfile.ProfileId))
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
            ViewData["Title"] = "Edit Personal Profile";
            return View(personalProfile);
        }

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

            ViewData["Title"] = "Delete Personal Profile";
            return View(personalProfile);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personalProfile = await _context.PersonalProfiles.FindAsync(id);
            if (personalProfile != null)
            {
                _context.PersonalProfiles.Remove(personalProfile);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PersonalProfileExists(int id)
        {
            return _context.PersonalProfiles.Any(e => e.ProfileId == id);
        }
    }
}
