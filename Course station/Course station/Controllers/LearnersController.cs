using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Course_station.Models;
using Course_station.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Course_station.Controllers
{
   // [Authorize]
    public class LearnersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly LearnerService _learnerService;
        private readonly CoursePrerequisiteService _coursePrerequisiteService;

        public LearnersController(ApplicationDbContext context, LearnerService learnerService, CoursePrerequisiteService coursePrerequisiteService)
        {
            _context = context;
            _learnerService = learnerService;
            _coursePrerequisiteService = coursePrerequisiteService;
        }
    //    var instructorId = HttpContext.Session.GetInt32("InstructorId");
    //        if (instructorId == null)
    //        {
    //            return RedirectToAction("Login", "Instructor");
    //}
    //        return View();
    // GET: Learners/Home
   // [Authorize]
        public async Task<IActionResult> Home()
        {
            var learnerId = HttpContext.Session.GetInt32("LearnerId");
            if (learnerId == null)
            {
                return RedirectToAction("Login", "Learners");
            }

            return View();
        }


        // GET: Learners
        [AdminPageOnly]
        public async Task<IActionResult> Index()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            var isAdminLoggedIn = User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name == "admin";

            return View(await _context.Learners.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learner = await _context.Learners
                .Include(l => l.CourseEnrollments)
                .ThenInclude(ce => ce.Course)
                .Include(l => l.TakenAssessments)
                .ThenInclude(ta => ta.Assessment)
                .Include(l => l.Skills)
                .Include(l => l.Rankings)
                .Include(l => l.PersonalProfiles)
                .ThenInclude(pp => pp.HealthConditions)
                .Include(l => l.PersonalProfiles)
                .ThenInclude(pp => pp.LearningPaths)
                .FirstOrDefaultAsync(m => m.LearnerId == id);

            if (learner == null)
            {
                return NotFound();
            }

            var enrolledCourses = await _context.Courses
                .FromSqlRaw("EXEC EnrolledCourses @LearnerID = {0}", id)
                .ToListAsync();

            var takenAssessments = await _context.TakenAssessments
                .Where(ta => ta.LearnerId == id)
                .Include(ta => ta.Assessment)
                .ToListAsync();

            var personalProfile = learner.PersonalProfiles.FirstOrDefault();
            var learningPaths = personalProfile?.LearningPaths.ToList() ?? new List<LearningPath>();
            var healthConditions = personalProfile?.HealthConditions.ToList() ?? new List<HealthCondition>();

            var viewModel = new LearnerDetailsViewModel
            {
                Learner = learner,
                EnrolledCourses = enrolledCourses,
                TakenAssessments = takenAssessments, // Ensure this property is populated
                PersonalProfile = personalProfile ?? new PersonalProfile(),
                LearningPaths = learningPaths,
                HealthConditions = healthConditions,
                Rankings = learner.Rankings.ToList(),
                Modules = learner.CourseEnrollments?.SelectMany(ce => ce.Course?.Modules ?? Enumerable.Empty<Module>()).ToList() ?? new List<Module>(),
                CoursePrerequisites = learner.CourseEnrollments?.SelectMany(ce => ce.Course?.CoursePrerequisites ?? Enumerable.Empty<CoursePrerequisite>()).ToList() ?? new List<CoursePrerequisite>(),
                CourseEnrollments = learner.CourseEnrollments.ToList()
            };

            return View(viewModel);
        }

        // GET: Learners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Learners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LearnerId,FirstName,LastName,Birthday,Gender,Country,CulturalBackground,Password,Email")] Learner learner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(learner);
                await _context.SaveChangesAsync();
                //   return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", "Learners", new { id = learner.LearnerId });
            }
            return View(learner);
        }

        // GET: Learners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learner = await _context.Learners.FindAsync(id);
            if (learner == null)
            {
                return NotFound();
            }
            return View(learner);
        }

        // POST: Learners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LearnerId,FirstName,LastName,Birthday,Gender,Country,CulturalBackground,Password,Email")] Learner learner)
        {
            if (id != learner.LearnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerExists(learner.LearnerId))
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
            return View(learner);
        }

        // GET: Learners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learner = await _context.Learners
                .FirstOrDefaultAsync(m => m.LearnerId == id);
            if (learner == null)
            {
                return NotFound();
            }

            return View(learner);
        }
        
        public IActionResult CreateFeedback(int learnerId)
        {
            var viewModel = new EmotionalFeedback
            {
                LearnerId = learnerId
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeedback(EmotionalFeedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.EmotionalFeedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Home));
            }
            return View(feedback);
        }

        // POST: Learners/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var learner = await _context.Learners.FindAsync(id);
            if (learner != null)
            {
                _context.Learners.Remove(learner);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Learner/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Learner model)
        {
            if (ModelState.IsValid)
            {
                var learner = await _context.Learners
                    .FirstOrDefaultAsync(l => l.LearnerId == model.LearnerId && l.Password == model.Password);

                if (learner != null)
                {
                    // Set the LearnerId in the session
                    HttpContext.Session.SetInt32("LearnerId", learner.LearnerId);

                    // Login successful, redirect to the learner's home page
                    return RedirectToAction("Home", "Learners");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid Learner ID or Password.";
                }
            }

            return View(model);
        }

        private bool LearnerExists(int id)
        {
            return _context.Learners.Any(e => e.LearnerId == id);
        }

        // 1. Retrieve Learner Info
        public async Task<IActionResult> ViewInfo(int learnerId)
        {
            var learner = await _context.Learners
                .Include(l => l.Notifications)
                .FirstOrDefaultAsync(l => l.LearnerId == learnerId);

            if (learner == null)
            {
                return NotFound();
            }

            var viewModel = new LearnerInfoViewModel
            {
                Learner = learner,
                Notifications = learner.Notifications.ToList()
            };

            return View(viewModel);
        }
        public async Task<IActionResult> MarkAsRead(int learnerId, int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.Readstatus = true;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewInfo", new { learnerId });
        }

        // 2. Update Profile
        public IActionResult UpdateProfile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(int learnerId, IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, $"{learnerId}_profile.jpg");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                TempData["Message"] = "Profile picture uploaded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a valid image file.";
            }

            return RedirectToAction(nameof(Details), new { id = learnerId });
        }

        // 4. Check Prerequisites
        public async Task<IActionResult> CheckPrerequisites(int learnerId, int courseId)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC Prerequisites @LearnerID = {0}, @CourseID = {1}",
                learnerId, courseId
            );

            ViewBag.PrerequisitesResult = result;
            return View();
        }

        // 5. View all modules for a course that train specific traits
        public async Task<IActionResult> ModuleTraits(string targetTrait, int courseId)
        {
            var modules = await _context.Modules
                .FromSqlRaw("EXEC Moduletraits @TargetTrait = {0}, @CourseID = {1}", targetTrait, courseId)
                .ToListAsync();

            return View(modules);
        }

        // 6. View all participants in a leaderboard and their ranking
        public async Task<IActionResult> LeaderboardRank(int leaderboardId)
        {
            var rankings = await _context.Leaderboards
                .FromSqlRaw("EXEC LeaderboardRank @LeaderboardID = {0}", leaderboardId)
                .ToListAsync();

            return View(rankings);
        }

        // 7. Submit emotional feedback for an activity
        [HttpPost]
        public async Task<IActionResult> SubmitEmotionalFeedback(int activityId, int learnerId, string emotionalState)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ActivityEmotionalFeedback @ActivityID = {0}, @LearnerID = {1}, @timestamp = {2}, @emotionalstate = {3}",
                activityId, learnerId, DateTime.Now, emotionalState
            );

            TempData["Message"] = "Feedback submitted successfully!";
            return RedirectToAction("ActivityDetails", new { activityId });
        }

        // 8. Join a collaborative quest if space is available
        [HttpPost]
        public async Task<IActionResult> JoinQuest(int learnerId, int questId)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC JoinQuest @LearnerID = {0}, @QuestID = {1}",
                learnerId, questId
            );

            TempData["Message"] = result > 0 ? "Successfully joined the quest!" : "Quest is full.";
            return RedirectToAction("QuestDetails", new { questId });
        }

        // 9. View skill proficiency levels
        public async Task<IActionResult> SkillsProficiency(int learnerId)
        {
            var skills = await _context.Skills
                .FromSqlRaw("EXEC SkillsProfeciency @LearnerID = {0}", learnerId)
                .ToListAsync();

            return View(skills);
        }

        // 10. View assessment score
        public async Task<IActionResult> ViewScore(int learnerId, int assessmentId)
        {
            var score = await _context.Database.ExecuteSqlRawAsync(
                "EXEC Viewscore @LearnerID = {0}, @AssessmentID = {1}",
                learnerId, assessmentId
            );

            ViewBag.Score = score;
            return View();
        }

        // 11. View assessments and grades for a module
        public async Task<IActionResult> AssessmentsList(int courseId, int moduleId, int learnerId)
        {
            var assessments = await _context.Assessments
                .FromSqlRaw("EXEC AssessmentsList @CourseID = {0}, @ModuleID = {1}, @LearnerID = {2}",
                courseId, moduleId, learnerId)
                .ToListAsync();

            return View(assessments);
        }

        // 12. Register for a course
        [HttpPost]
        public async Task<IActionResult> RegisterCourse(int learnerId, int courseId)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC Courseregister @LearnerID = {0}, @CourseID = {1}",
                learnerId, courseId
            );

            TempData["Message"] = result > 0 ? "Registered successfully!" : "Could not register. Check prerequisites.";
            return RedirectToAction("CourseDetails", new { courseId });
        }

        public IActionResult Enroll(int learnerId)
        {
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title");
            var viewModel = new EnrollViewModel
            {
                LearnerId = learnerId
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(EnrollViewModel model)
        {
            if (ModelState.IsValid)
            {
                var prerequisitesCompleted = await _coursePrerequisiteService.CheckPrerequisitesCompleted(model.LearnerId, model.CourseId);
                if (!prerequisitesCompleted)
                {
                    TempData["ErrorMessage"] = "You have not completed the prerequisites for this course. Please complete the prerequisites and try again.";
                    TempData["PrerequisiteCourseId"] = model.CourseId;
                    return RedirectToAction(nameof(ViewPrerequisites), new { learnerId = model.LearnerId, courseId = model.CourseId });
                }

                var enrollment = new CourseEnrollment
                {
                    CourseId = model.CourseId,
                    LearnerId = model.LearnerId,
                    //  EnrollmentDate = DateTime.Now
                };
                _context.CourseEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Enrollment successful!";
                return RedirectToAction(nameof(EnrolledCourses), new { learnerId = model.LearnerId });
            }
            ViewBag.Courses = new SelectList(_context.Courses, "CourseId", "Title");
            return View(model);
        }

        public async Task<IActionResult> ViewPrerequisites(int learnerId, int courseId)
        {
            var prerequisites = await _context.CoursePrerequisites
                .Where(cp => cp.CourseId == courseId)
                .Include(cp => cp.Course)
                .ToListAsync();

            var viewModel = new PrerequisiteViewModel
            {
                LearnerId = learnerId,
                CourseId = courseId,
                Prerequisites = prerequisites
            };

            return View(viewModel);
        }

        public async Task<IActionResult> EnrolledCourses(int learnerId)
        {
            var courses = await _context.Courses
                .FromSqlRaw("EXEC EnrolledCourses @LearnerID = {0}", learnerId)
                .ToListAsync();

            return View(courses);
        }


        public IActionResult QuestProgress()
        {
            var quests = _context.Quests.ToList(); // Ensure this is a list of Quest objects
            return View(quests);
        }
        // Join a collaborative quest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinCollaborativeQuest(int learnerId, int questId)
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC JoinCollaborativeQuest @LearnerID = {0}, @QuestID = {1}",
                learnerId, questId
            );

            TempData["Message"] = result > 0 ? "Successfully joined the collaborative quest!" : "Quest is full or an error occurred.";
            return RedirectToAction(nameof(AvailableQuests));
        }

        public async Task<IActionResult> AvailableQuests()
        {
            var quests = await _context.Quests
                .Where(q => q.Collaborative != null)
                .ToListAsync();

            return View(quests);
        }
        public async Task<IActionResult> AchievementProgress(int learnerId)
        {
            var learner = await _context.Learners
                .Include(l => l.Achievements)
                .ThenInclude(a => a.Badge)
                .FirstOrDefaultAsync(l => l.LearnerId == learnerId);

            if (learner == null)
            {
                return NotFound();
            }

            var achievements = learner.Achievements.ToList();
            return View(achievements);
        }

        public class ActiveQuestParticipantsViewModel
        {
            public Learner Learner { get; set; }
            public List<Collaborative> ActiveQuests { get; set; }
        }
        public async Task<IActionResult> ActiveQuestParticipants(int learnerId)
        {
            var learner = await _context.Learners
                .Include(l => l.LearnerCollaborations)
                .ThenInclude(lc => lc.Quest)
                .ThenInclude(q => q.LearnerCollaborations)
                .ThenInclude(lc => lc.Learner)
                .FirstOrDefaultAsync(l => l.LearnerId == learnerId);

            if (learner == null)
            {
                return NotFound();
            }

            var activeQuests = learner.LearnerCollaborations
                .Where(lc => lc.CompletionStatus != "Completed")
                .Select(lc => lc.Quest)
                .ToList();

            var viewModel = new ActiveQuestParticipantsViewModel
            {
                Learner = learner,
                ActiveQuests = activeQuests
            };

            return View(viewModel);
        }






    }
}
