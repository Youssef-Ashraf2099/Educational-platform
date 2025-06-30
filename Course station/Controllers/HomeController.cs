using Course_station.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;

namespace Course_station.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
       
            public IActionResult Credits()
            {
                return View();
            }
        

        // GET: AdminLogin
        public IActionResult AdminLogin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(string username, string password, string returnUrl = null)
        {
            if ((username == "youssef.ashraf" && password == "1234") || (username == "admin" && password == "admin"))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

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
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Add your delete logic here
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            // Add your details logic here
            return View();
        }

        public IActionResult Create()
        {
            // Add your create logic here
            return View();
        }
        public IActionResult LogIn()
        {
            // Add your create logic here
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SubmitForm(string firstname, string lastname, string email, string country, string subject)
        {
            // Handle form submission logic here
            // For example, save the data to the database or send an email

            // Redirect to a thank you page or return a view with a thank you message
            return RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
