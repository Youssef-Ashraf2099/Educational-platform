using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Course_station.Models;

namespace Course_station.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notifications
        //[AdminPageOnly]
        public async Task<IActionResult> Index()
        {
          //var adminId = HttpContext.Session.GetInt32("AdminId");
          //  var isAdminLoggedIn = User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name == "admin";

            var instructorId = HttpContext.Session.GetInt32("InstructorId");
            var learnerId = HttpContext.Session.GetInt32("LearnerId");

            IQueryable<Notification> notificationsQuery = _context.Notifications;

            if (instructorId != null)
            {
                notificationsQuery = notificationsQuery.Where(n => n.Learners.Any(l => l.CourseEnrollments.Any(ce => ce.Course.Instructors.Any(i => i.InstructorId == instructorId))));
            }
            else if (learnerId != null)
            {
                notificationsQuery = notificationsQuery.Where(n => n.Learners.Any(l => l.LearnerId == learnerId));
            }

            var notifications = await notificationsQuery.ToListAsync();
            return View(notifications);
        }


       
        // GET: Notifications of the learner
        public async Task<IActionResult> IndexLearner()
        {
            return View(await _context.Notifications.ToListAsync());
        }
        // GET: Notifications of the instructor
        public async Task<IActionResult> IndexInstructor()
        {
            return View(await _context.Notifications.ToListAsync());
        }
        public async Task<IActionResult> IndexAdmin()
        {
            return View(await _context.Notifications.ToListAsync());
        }
        public async Task<IActionResult> NotUser()
        {
            return RedirectToAction("Create", "Learners");
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }
        

        // GET: Notifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificationId,TimeStamp,Message,Urgency,Readstatus")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificationId,TimeStamp,Message,Urgency,Readstatus")] Notification notification)
        {
            if (id != notification.NotificationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.NotificationId))
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
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotificationId == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.NotificationId == id);
        }
        
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            notification.Readstatus = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
