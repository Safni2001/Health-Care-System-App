using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HealthcareApp.Models;

namespace HealthcareApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalDoctors = users.Count(u => u.Role == "Doctor");
            ViewBag.TotalPatients = users.Count(u => u.Role == "Patient");
            return View();
        }

        // ---------- Doctors ----------
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _db.Doctors.Include(d => d.Specialty).ToListAsync();
            return View(doctors);
        }

        [HttpGet]
        public IActionResult CreateDoctor()
        {
            ViewBag.Specialties = _db.Specialties.ToList();
            return View(new Doctor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(Doctor model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Specialties = _db.Specialties.ToList();
                return View(model);
            }
            _db.Doctors.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Doctors));
        }

        [HttpGet]
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = await _db.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            ViewBag.Specialties = _db.Specialties.ToList();
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(Doctor model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Specialties = _db.Specialties.ToList();
                return View(model);
            }
            _db.Doctors.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Doctors));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _db.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();
            _db.Doctors.Remove(doctor);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Doctors));
        }

        // ---------- Specialties ----------
        public async Task<IActionResult> Specialties()
        {
            var specialties = await _db.Specialties.ToListAsync();
            return View(specialties);
        }

        [HttpGet]
        public IActionResult CreateSpecialty()
        {
            return View(new Specialty());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSpecialty(Specialty model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _db.Specialties.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Specialties));
        }

        [HttpGet]
        public async Task<IActionResult> EditSpecialty(int id)
        {
            var specialty = await _db.Specialties.FindAsync(id);
            if (specialty == null) return NotFound();
            return View(specialty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSpecialty(Specialty model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _db.Specialties.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Specialties));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSpecialty(int id)
        {
            var specialty = await _db.Specialties.FindAsync(id);
            if (specialty == null) return NotFound();
            _db.Specialties.Remove(specialty);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Specialties));
        }

        // ---------- Users listing ----------
        public IActionResult Patients()
        {
            var patients = _userManager.Users.Where(u => u.Role == "Patient").ToList();
            return View(patients);
        }

        public IActionResult AllDoctorsUsers()
        {
            var doctors = _userManager.Users.Where(u => u.Role == "Doctor").ToList();
            return View(doctors);
        }

        // ---------- Reports ----------
        public async Task<IActionResult> Reports()
        {
            var totalAppointments = await _db.Appointments.CountAsync();
            var totalPayments = await _db.Appointments.SumAsync(a => (decimal?)a.Payment) ?? 0m;
            var patientCount = _userManager.Users.Count(u => u.Role == "Patient");
            var doctorCount = _userManager.Users.Count(u => u.Role == "Doctor");

            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.TotalPayments = totalPayments;
            ViewBag.PatientCount = patientCount;
            ViewBag.DoctorCount = doctorCount;

            var recent = await _db.Appointments
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.ScheduledAt)
                .Take(10)
                .ToListAsync();

            return View(recent);
        }

        // ---------- User Management ----------
        [HttpGet]
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var vm = new List<UserManagementViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                vm.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? user.UserName ?? string.Empty,
                    Role = user.Role,
                    IsActive = user.EmailConfirmed
                });
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(newRole))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            user.Role = newRole;
            await _userManager.UpdateAsync(user);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Message"] = $"User {user.FullName ?? user.UserName ?? "Unknown"} role changed to {newRole}";
            return RedirectToAction(nameof(ManageUsers));
        }
    }
}
