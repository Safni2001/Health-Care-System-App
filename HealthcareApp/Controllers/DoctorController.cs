using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HealthcareApp.Models;

namespace HealthcareApp.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.UserName = currentUser?.FullName ?? currentUser?.UserName;
            ViewBag.UserRole = "Doctor";
            ViewBag.UserEmail = currentUser?.Email;
            
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(currentUser);
        }
    }
}
