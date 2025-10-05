using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;
using System.Security.Cryptography;
using System.Text;

namespace SistemaAsistencia.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener registros de asistencia del usuario
            var attendanceRecords = await _context.AttendanceRecords
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Timestamp)
                .Take(10)
                .ToListAsync();

            // Obtener estadísticas del mes actual
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            
            var monthlyRecords = await _context.AttendanceRecords
                .Where(r => r.UserId == userId && 
                           r.Timestamp.Month == currentMonth && 
                           r.Timestamp.Year == currentYear)
                .ToListAsync();

            var totalDays = monthlyRecords.Count(r => r.RecordType == "Entrada");
            var totalHours = CalculateTotalHours(monthlyRecords);

            ViewBag.AttendanceRecords = attendanceRecords;
            ViewBag.TotalDays = totalDays;
            ViewBag.TotalHours = totalHours;
            ViewBag.CurrentMonth = GetMonthName(currentMonth);

            return View(user);
        }

        // GET: User/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        // GET: User/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Verificar contraseña actual
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta");
                return View(model);
            }

            // Actualizar contraseña
            user.Password = model.NewPassword;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña cambiada exitosamente";
            return RedirectToAction("Profile");
        }

        // GET: User/AttendanceHistory
        public async Task<IActionResult> AttendanceHistory(int pageNumber = 1, int pageSize = 20)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var totalRecords = await _context.AttendanceRecords
                .Where(r => r.UserId == userId)
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var records = await _context.AttendanceRecords
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalRecords"] = totalRecords;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;
            ViewData["HasNextPage"] = pageNumber < totalPages;
            ViewData["PreviousPageNumber"] = pageNumber - 1;
            ViewData["NextPageNumber"] = pageNumber + 1;

            return View(records);
        }

        private double CalculateTotalHours(List<AttendanceRecord> records)
        {
            var totalHours = 0.0;
            var dailyRecords = records.GroupBy(r => r.Timestamp.Date);

            foreach (var day in dailyRecords)
            {
                var entries = day.Where(r => r.RecordType == "Entrada").OrderBy(r => r.Timestamp).ToList();
                var exits = day.Where(r => r.RecordType == "Salida").OrderBy(r => r.Timestamp).ToList();

                for (int i = 0; i < Math.Min(entries.Count, exits.Count); i++)
                {
                    var entry = entries[i];
                    var exit = exits[i];
                    var hours = (exit.Timestamp - entry.Timestamp).TotalHours;
                    totalHours += hours;
                }
            }

            return Math.Round(totalHours, 2);
        }

        private string GetMonthName(int month)
        {
            var months = new[] { "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            return months[month];
        }
    }

    public class ChangePasswordViewModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}





