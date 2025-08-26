using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthcareApp.Models;
using System.Security.Claims;

namespace HealthcareApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.UserName = currentUser?.FullName ?? currentUser?.UserName;
            ViewBag.UserRole = "Admin";
            
            // Get all users for admin dashboard
            var users = _userManager.Users.ToList();
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalDoctors = users.Count(u => u.Role == "Doctor");
            ViewBag.TotalPatients = users.Count(u => u.Role == "Patient");
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserManagementViewModel>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role,
                    IsActive = user.EmailConfirmed
                });
            }
            
            return View(userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            
            user.Role = newRole;
            await _userManager.UpdateAsync(user);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Message"] = $"User {user.FullName ?? user.UserName ?? "Unknown"} role changed to {newRole}";
            return RedirectToAction(nameof(ManageUsers));
        }
    }
}
