using Course_station.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Course_station.Controllers
{
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // [Authorize(Roles = "Instructor")]
        public IActionResult Home()
        {
            var instructorId = HttpContext.Session.GetInt32("InstructorId");
            if (instructorId == null)
            {
                return RedirectToAction("Login", "Instructor");
            }
            return View();
        }

        [AdminPageOnly]
        public async Task<IActionResult> Index()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            var isAdminLoggedIn = User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name == "admin";

            var instructors = await _context.Instructors.Include(i => i.Courses).ToListAsync();
            return View(instructors);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.Courses)
                .Include(i => i.EmotionalfeedbackReviews)
                .Include(i => i.Pathreviews)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.CourseEnrollments)
                .Include(i => i.Courses)
                    .ThenInclude(c => c.Modules)
                        .ThenInclude(m => m.Assessments)
                            .ThenInclude(a => a.TakenAssessments)
                .FirstOrDefaultAsync(m => m.InstructorId == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("InstructorId,InstructorName,LatestQualification,ExpertiseArea,Email,Password")] Instructor instructor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(instructor);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(instructor);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructorId,InstructorName,LatestQualification,ExpertiseArea,Email,Password")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = instructor.InstructorId });
            }
            return View(instructor);
        }



        // GET: Instructor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructorId,InstructorName,LatestQualification,ExpertiseArea,Email,Password")] Instructor instructor)
        {
            if (id != instructor.InstructorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instructor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructorExists(instructor.InstructorId))
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
            return View(instructor);
        }

        // GET: Instructor/ManageCourses
        public async Task<IActionResult> ManageCourses()
        {
            var instructorId = HttpContext.Session.GetInt32("InstructorId");
            if (instructorId == null)
            {
                return RedirectToAction("Login", "Instructor");
            }

            var courses = await _context.Courses
                .Where(c => c.Instructors.Any(i => i.InstructorId == instructorId))
                .ToListAsync();

            return View(courses);
        }


        // GET: Course /details/5
        public async Task<IActionResult> CourseDetails(int? id)
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

            return View("~/Views/Course/Details.cshtml", course);
        }




        // GET: Instructor/EditCourse/5
        public async Task<IActionResult> EditCourse(int? id)
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
            return View("~/Views/Course/Edit.cshtml", course);
        }

        // POST: Instructor/EditCourse/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(int id, [Bind("CourseId,Title,Description,DiffLevel,CreditPoint,LearningObjective")] Course course)
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
                return RedirectToAction(nameof(ManageCourses));
            }
            return View("~/Views/Course/Edit.cshtml", course);
        }



        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }

        // GET: Instructor/Delete/5
        // GET: Instructor/DeleteCourse/5
        public async Task<IActionResult> DeleteCourse(int? id)
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

            return View("~/Views/Course/Delete.cshtml", course);
        }

        // GET: Instructor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.InstructorId == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.FindAsync(id);
            if (instructor == null)
            {
                return NotFound();
            }

            try
            {
                _context.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Instructor deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting instructor: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Instructor model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var instructor = await _context.Instructors
                        .FirstOrDefaultAsync(l => l.InstructorId == model.InstructorId && l.Password == model.Password);

                    if (instructor != null)
                    {
                        // Set the InstructorId in the session
                        HttpContext.Session.SetInt32("InstructorId", instructor.InstructorId);

                        // Login successful, redirect to the instructor's home page
                        return RedirectToAction("Home", "Instructor");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid ID or Password.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }

            return View(model);
        }
        public async Task<IActionResult> SendNotification()
        {
            var instructorId = HttpContext.Session.GetInt32("InstructorId");
            if (instructorId == null)
            {
                return RedirectToAction("Login", "Instructor");
            }

            var courses = await _context.Courses
                .Where(c => c.Instructors.Any(i => i.InstructorId == instructorId))
                .ToListAsync();

            ViewBag.Courses = new SelectList(courses, "CourseId", "Title");
            return View();
        }

        // POST: Instructor/SendNotification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNotification(int courseId, string message, string urgency)
        {
            if (ModelState.IsValid)
            {
                var learners = await _context.Learners
                    .Where(l => l.CourseEnrollments.Any(ce => ce.CourseId == courseId))
                    .ToListAsync();

                var notifications = learners.Select(learner => new Notification
                {
                    TimeStamp = DateTime.Now,
                    Message = message,
                    Urgency = urgency,
                    Readstatus = false,
                    Learners = new List<Learner> { learner }
                }).ToList();

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Notifications sent successfully!";
                return RedirectToAction("Index", "Notifications");
            }

            var instructorId = HttpContext.Session.GetInt32("InstructorId");
            var courses = await _context.Courses
                .Where(c => c.Instructors.Any(i => i.InstructorId == instructorId))
                .ToListAsync();

            ViewBag.Courses = new SelectList(courses, "CourseId", "Title");
            return View();
        }

        // GET: Instructor/GetLearnersByCourse
        public async Task<IActionResult> GetLearnersByCourse(int courseId)
        {
            var learners = await _context.Learners
                .Where(l => l.CourseEnrollments.Any(ce => ce.CourseId == courseId))
                .Select(l => new { l.LearnerId, FullName = l.FirstName + " " + l.LastName })
                .ToListAsync();

            return Json(learners);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.Name != null)
            {
                // Clear user-specific cache or perform other cleanup tasks
                var cacheKey = $"UserCache_{User.Identity.Name}";
                // Assuming you have a cache service, you can clear the cache like this:
                // _cacheService.Remove(cacheKey);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(int instructorId, IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, $"{instructorId}_profile.jpg");

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

            return RedirectToAction(nameof(Details), new { id = instructorId });
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.InstructorId == id);
        }

        // 1. List all the learners that have a certain skill
        public async Task<IActionResult> SkillLearners(string skillName)
        {
            var learners = await _context.Learners
                .FromSqlRaw("EXEC SkillLearners @Skillname = {0}", skillName)
                .ToListAsync();

            return View(learners);
        }

        // 2. Add new Learning activities for a course module
        [HttpPost]
        public async Task<IActionResult> NewActivity(int courseId, int moduleId, string activityType, string instructionDetails, int maxPoints)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC NewActivity @CourseID = {0}, @ModuleID = {1}, @activitytype = {2}, @instructiondetails = {3}, @maxpoints = {4}",
                courseId, moduleId, activityType, instructionDetails, maxPoints
            );

            return RedirectToAction(nameof(Index));
        }

        // 3. Award a new achievement to a learner
        [HttpPost]
        public async Task<IActionResult> NewAchievement(int learnerId, int badgeId, string description, DateTime dateEarned, string type)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC NewAchievement @LearnerID = {0}, @BadgeID = {1}, @description = {2}, @date_earned = {3}, @type = {4}",
                learnerId, badgeId, description, dateEarned, type
            );

            return RedirectToAction(nameof(Index));
        }

        // 4. View all the learners who earned a certain badge
        public async Task<IActionResult> LearnerBadge(int badgeId)
        {
            var learners = await _context.Learners
                .FromSqlRaw("EXEC LearnerBadge @BadgeID = {0}", badgeId)
                .ToListAsync();

            return View(learners);
        }

        // 5. Add a new learning path for a learner
        [HttpPost]
        public async Task<IActionResult> NewPath(int learnerId, int profileId, string completionStatus, string customContent, string adaptiveRules)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC NewPath @LearnerID = {0}, @ProfileID = {1}, @completion_status = {2}, @custom_content = {3}, @adaptiverules = {4}",
                learnerId, profileId, completionStatus, customContent, adaptiveRules
            );

            return RedirectToAction(nameof(Index));
        }

        // 6. View all the courses that a learner took so far
        public async Task<IActionResult> TakenCourses(int learnerId)
        {
            var courses = await _context.Courses
                .FromSqlRaw("EXEC TakenCourses @LearnerID = {0}", learnerId)
                .ToListAsync();

            return View(courses);
        }

        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse([Bind("CourseId,Title,Description,DiffLevel,CreditPoint,LearningObjective")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        public IActionResult CreateAssessment()
        {
            ViewBag.Modules = new SelectList(_context.Modules, "ModuleId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssessment([Bind("AssessmentId,ModuleId,Title,Description,MaxPoints")] Assessment assessment)
        {
            if (ModelState.IsValid)
            {
                _context.Assessments.Add(assessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Modules = new SelectList(_context.Modules, "ModuleId", "Title", assessment.ModuleId);
            return View(assessment);
        }

        // 7. Add a new collaborative Quest
        [HttpPost]
        public async Task<IActionResult> CollaborativeQuest(string difficultyLevel, string criteria, string description, string title, int maxNumParticipants, DateTime deadline)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC CollaborativeQuest @difficulty_level = {0}, @criteria = {1}, @description = {2}, @title = {3}, @Maxnumparticipants = {4}, @deadline = {5}",
                difficultyLevel, criteria, description, title, maxNumParticipants, deadline
            );

            return RedirectToAction(nameof(Index));
        }

        // 8. Update the deadline of a quest
        [HttpPost]
        public async Task<IActionResult> DeadlineUpdate(int questId, DateTime deadline)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC DeadlineUpdate @QuestID = {0}, @deadline = {1}",
                questId, deadline
            );

            return RedirectToAction(nameof(Index));
        }

        // 9. Update an assessment grade for a learner
        [HttpPost]
        public async Task<IActionResult> GradeUpdate(int learnerId, int assessmentId, int points)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC GradeUpdate @LearnerID = {0}, @AssessmentID = {1}, @points = {2}",
                learnerId, assessmentId, points
            );

            TempData["Message"] = "Grade updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // 10. Send a notification to a learner about an upcoming assessment deadline
        [HttpPost]
        public async Task<IActionResult> AssessmentNot(int notificationId, DateTime timestamp, string message, string urgencyLevel, int learnerId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC AssessmentNot @NotificationID = {0}, @timestamp = {1}, @message = {2}, @urgencylevel = {3}, @LearnerID = {4}",
                notificationId, timestamp, message, urgencyLevel, learnerId
            );

            TempData["Message"] = "Notification sent successfully!";
            return RedirectToAction(nameof(Index));
        }

        // 11. Define new learning goal for the learners
        [HttpPost]
        public async Task<IActionResult> NewGoal(int goalId, string status, DateTime deadline, string description)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC NewGoal @GoalID = {0}, @status = {1}, @deadline = {2}, @description = {3}",
                goalId, status, deadline, description
            );

            return RedirectToAction(nameof(Index));
        }

        // 13. See the last time a discussion forum was active
        public async Task<IActionResult> LastActive(int forumId)
        {
            var lastActive = new SqlParameter("@lastactive", SqlDbType.DateTime) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC LastActive @ForumID = {0}, @lastactive = {1} OUTPUT",
                forumId, lastActive
            );

            ViewBag.LastActive = lastActive.Value;
            return View();
        }

        // 14. Find the most common emotional state for the learners
        public async Task<IActionResult> CommonEmotionalState()
        {
            var state = new SqlParameter("@state", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC CommonEmotionalState @state OUTPUT",
                state
            );

            ViewBag.State = state.Value;
            return View();
        }

        // 15. View all modules for a certain course sorted by their difficulty
        public async Task<IActionResult> ModuleDifficulty(int courseId)
        {
            var modules = await _context.Modules
                .FromSqlRaw("EXEC ModuleDifficulty @courseID = {0}", courseId)
                .ToListAsync();

            return View(modules);
        }

        // 16. View the skill with the highest proficiency level to a certain student
        public async Task<IActionResult> ProficiencyLevel(int learnerId)
        {
            var skill = new SqlParameter("@skill", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ProficiencyLevel @LearnerID = {0}, @skill OUTPUT",
                learnerId, skill
            );

            ViewBag.Skill = skill.Value;
            return View();
        }

        // 17. Update a learner proficiency level for a certain skill
        [HttpPost]
        public async Task<IActionResult> ProficiencyUpdate(string skill, int learnerId, string level)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC ProficiencyUpdate @Skill = {0}, @LearnerId = {1}, @Level = {2}",
                skill, learnerId, level
            );

            return RedirectToAction(nameof(Index));
        }

        // 18. Find the learner with the least number of badges earned
        public async Task<IActionResult> LeastBadge()
        {
            var learnerId = new SqlParameter("@LearnerID", SqlDbType.Int) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC LeastBadge @LearnerID OUTPUT",
                learnerId
            );

            ViewBag.LearnerId = learnerId.Value;
            return View();
        }

        // 19. Find the most preferred learning type for the learners
        public async Task<IActionResult> PreferredType()
        {
            var type = new SqlParameter("@type", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC PreferredType @type OUTPUT",
                type
            );

            ViewBag.Type = type.Value;
            return View();
        }

        // 20. Generate analytics on assessment scores across various modules or courses (average of scores)
        public async Task<IActionResult> AssessmentAnalytics(int courseId, int moduleId)
        {
            var analytics = await _context.Assessments
                .FromSqlRaw("EXEC AssessmentAnalytics @CourseID = {0}, @ModuleID = {1}", courseId, moduleId)
                .ToListAsync();

            return View(analytics);
        }

        // 21. View trends in learners’ emotional feedback to support well-being in courses I teach
        public async Task<IActionResult> EmotionalTrendAnalysisIns(int courseId, int moduleId, DateTime timePeriod)
        {
            var trends = await _context.EmotionalFeedbacks
                .FromSqlRaw("EXEC EmotionalTrendAnalysisIns @CourseID = {0}, @ModuleID = {1}, @TimePeriod = {2}", courseId, moduleId, timePeriod)
                .ToListAsync();

            return View(trends);
        }
        // Add a new action to display the form for adding activities
        /* public IActionResult AddActivity(int moduleId)
         {
             ViewBag.ModuleId = moduleId;
             return View();
         }*/
        public IActionResult AddActivity()
        {
            ViewBag.Modules = new SelectList(_context.Modules, "ModuleId", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddActivity([Bind("ModuleId,ActivityType,InstructionDetails,MaxPoints")] LearningActivity activity)
        {
            if (ModelState.IsValid)
            {
                _context.LearningActivities.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Module", new { id = activity.ModuleId });
            }
            return View(activity);
        }
    }
}
        /*
        // GET: Instructor/DeleteCourse/5
        public async Task<IActionResult> DeleteCourse(int? id)
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

            return View("~/Views/Course/Delete.cshtml", course);
        }*/
        /*
        // POST: Instructor/DeleteCourse/5
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageCourses));
        }

        */
        /*
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CourseEnrollments)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            if (course.CourseEnrollments.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete course with enrolled students.";
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }
        
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult CreateQuest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestWithDeadline([Bind("QuestId,DifficultyLevel,Criteria,Description,Title,Collaborative,SkillMastery")] Quest quest, DateTime deadline)
        {
            if (deadline == null)
            {
                ModelState.AddModelError("Deadline", "The Deadline field is required.");
            }

            if (ModelState.IsValid)
            {
                // Add the quest to the context
                _context.Quests.Add(quest);
                await _context.SaveChangesAsync();

                // Handle the deadline separately
                // You can store the deadline in a dictionary or any other structure as needed
                // For example, using a dictionary to store deadlines
                var questDeadlines = new Dictionary<int, DateTime>();
                questDeadlines[quest.QuestId] = deadline;

                // Save the deadline to the database or any other storage as needed
                // This is just an example, you need to implement the actual storage logic

                return RedirectToAction(nameof(Index));
            }

            // Pass the validation message to the view
            ViewBag.DeadlineValidationMessage = ModelState["Deadline"]?.Errors.FirstOrDefault()?.ErrorMessage;

            return View(quest);
        }





        // GET: Instructor/AddAchievement
        public IActionResult AddAchievement()
        {
            ViewBag.Learners = new SelectList(_context.Learners, "LearnerId", "FirstName");
            ViewBag.Badges = new SelectList(_context.Badges, "BadgeId", "Description");
            return View();
        }

        // POST: Instructor/AddAchievement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAchievement([Bind("LearnerId,BadgeId,Description,DateEarned")] Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                _context.Achievements.Add(achievement);
                await _context.SaveChangesAsync();

                // Send notification to the learner
                var notification = new Notification
                {
                    TimeStamp = DateTime.Now,
                    Message = $"You have earned a new achievement: {achievement.Description}",
                    Urgency = "Normal",
                    Readstatus = false
                };

                // Add the notification to the learner's notifications
                var learner = await _context.Learners.FindAsync(achievement.LearnerId);
                if (learner != null)
                {
                    learner.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Achievement added and learner notified successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Learners = new SelectList(_context.Learners, "LearnerId", "FirstName", achievement.LearnerId);
            ViewBag.Badges = new SelectList(_context.Badges, "BadgeId", "Description", achievement.BadgeId);
            return View(achievement);
        }



    }
}*/
