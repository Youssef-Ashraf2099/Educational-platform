using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class QuestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Quests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Quests.ToListAsync());
        }

        // GET: Quests/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quest = await _context.Quests
                .FirstOrDefaultAsync(m => m.QuestId == id);
            if (quest == null)
            {
                return NotFound();
            }

            // Retrieve the deadline from the storage
            var questDeadlines = new Dictionary<int, DateTime>(); // Replace with actual storage retrieval logic
            DateTime? deadline = questDeadlines.ContainsKey(quest.QuestId) ? questDeadlines[quest.QuestId] : (DateTime?)null;

            ViewBag.Deadline = deadline;

            return View(quest);
        }



        // GET: Quests/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestId,DifficultyLevel,Criteria,Description,Title,Collaborative,SkillMastery")] Quest quest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = quest.QuestId });
            }
            return View(quest);
        }


        // GET: Quests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quest = await _context.Quests.FindAsync(id);
            if (quest == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Quests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestId,DifficultyLevel,Criteria,Description,Title,Collaborative,SkillMastery")] Quest quest)
        {
            if (id != quest.QuestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestExists(quest.QuestId))
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
            return View(quest);
        }



        



        // GET: Quests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quest = await _context.Quests
                .FirstOrDefaultAsync(m => m.QuestId == id);
            if (quest == null)
            {
                return NotFound();
            }

            return View(quest);
        }

        // POST: Quests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quest = await _context.Quests.FindAsync(id);
            if (quest != null)
            {
                _context.Quests.Remove(quest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool QuestExists(int id)
        {
            return _context.Quests.Any(e => e.QuestId == id);
        }




        // POST: Quests/Join
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int questId, int learnerId)
        {
            // Check if the learner has already joined the quest
            var existingCollaboration = await _context.LearnerCollaborations
                .FirstOrDefaultAsync(lc => lc.LearnerId == learnerId && lc.QuestId == questId);

            if (existingCollaboration != null)
            {
                TempData["ErrorMessage"] = "You have already joined this quest.";
                return RedirectToAction(nameof(Details), new { id = questId });
            }

            // Retrieve the quest from the database, including its collaborative details
            var quest = await _context.Quests
                .Include(q => q.Collaborative)
                .FirstOrDefaultAsync(q => q.QuestId == questId);

            // Check if the quest or its collaborative details are not found
            if (quest == null || quest.Collaborative == null)
            {
                return NotFound();
            }

            // Check if the quest has reached the maximum number of participants
            if (quest.Collaborative.LearnerCollaborations.Count >= quest.Collaborative.MaxNumParticipants)
            {
                TempData["ErrorMessage"] = "Quest is full.";
                return RedirectToAction(nameof(Details), new { id = questId });
            }

            // Create a new learner collaboration entry
            var learnerCollaboration = new LearnerCollaboration
            {
                QuestId = questId,
                LearnerId = learnerId
            };

            // Add the learner collaboration to the database
            _context.LearnerCollaborations.Add(learnerCollaboration);
            await _context.SaveChangesAsync();

            // Create a new achievement for joining the quest
            var achievement = new Achievement
            {
                Description = "Joined a Collaborative Quest",
                DateEarned = DateOnly.FromDateTime(DateTime.Now),
                BadgeId = 1, // Assign appropriate BadgeId
                LearnerId = learnerId
            };

            // Retrieve the learner from the database
            var learner = await _context.Learners.FindAsync(learnerId);
            if (learner != null)
            {
                // Add the achievement to the learner's profile
                learner.Achievements.Add(achievement);
                await _context.SaveChangesAsync();
            }

            // Set a success message and redirect to the quest details page
            TempData["Message"] = "Successfully joined the quest!";
            return RedirectToAction(nameof(Details), new { id = questId });
        }

        // GET: Quests/Participants/5
        public async Task<IActionResult> Participants(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quest = await _context.Quests
                .Include(q => q.Collaborative)
                .ThenInclude(c => c.LearnerCollaborations)
                .ThenInclude(lc => lc.Learner)
                .FirstOrDefaultAsync(m => m.QuestId == id);

            if (quest == null)
            {
                return NotFound();
            }

            return View(quest);
        }


    }
}
