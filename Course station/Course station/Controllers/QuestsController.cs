﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                .Include(q => q.Collaborative)
                .ThenInclude(c => c.LearnerCollaborations)
                .FirstOrDefaultAsync(m => m.QuestId == id);
            if (quest == null)
            {
                return NotFound();
            }

            return View(quest);
        }

        // GET: Quests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestId,DifficultyLevel,Criteria,Description,Title,Deadline")] Quest quest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
            return View(quest);
        }

        // POST: Quests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuestId,DifficultyLevel,Criteria,Description,Title,Deadline")] Quest quest)
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
            var quest = await _context.Quests
                .Include(q => q.Collaborative)
                .FirstOrDefaultAsync(q => q.QuestId == questId);

            if (quest == null || quest.Collaborative == null)
            {
                return NotFound();
            }

            if (quest.Collaborative.LearnerCollaborations.Count >= quest.Collaborative.MaxNumParticipants)
            {
                TempData["ErrorMessage"] = "Quest is full.";
                return RedirectToAction(nameof(Details), new { id = questId });
            }

            var learnerCollaboration = new LearnerCollaboration
            {
                QuestId = questId,
                LearnerId = learnerId
            };

            _context.LearnerCollaborations.Add(learnerCollaboration);
            await _context.SaveChangesAsync();

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