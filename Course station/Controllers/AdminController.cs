using Course_station.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Course_station.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult AdminLogin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Check if the username and password are "admin"
            if (username == "admin" && password == "admin")
            {
                // Set the AdminId in the session
                HttpContext.Session.SetInt32("AdminId", 1);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("AdminPage", "Admin");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        private bool IsAdminLoggedIn()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            return adminId.HasValue && adminId.Value == 1;
        }

        public IActionResult AdminPage()
        {
            if (!IsAdminLoggedIn())
            {
                //HttpContext.Session.Clear();
                return RedirectToAction("AdminLogin");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
