using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Course_station.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string indexPage)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    if (indexPage == "Instructor")
                    {
                        return RedirectToAction("InstructorIndex", "Admin");
                    }
                    else if (indexPage == "Learners")
                    {
                        return RedirectToAction("LearnerIndex", "Admin");
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    return Ok("Role assigned successfully");
                }
                else
                {
                    return BadRequest("Failed to assign role");
                }
            }
            return NotFound("User not found");
        }
    }
}
