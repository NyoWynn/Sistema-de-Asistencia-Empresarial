using Microsoft.AspNetCore.Mvc;
using SistemaAsistencia.Models;
using SistemaAsistencia.Services;
using System.Linq;

namespace SistemaAsistencia.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly QrService _qrService;

        public AccountController(ApplicationDbContext context, QrService qrService)
        {
            _context = context;
            _qrService = qrService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Buscar primero en Users (usuarios del sistema)
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                HttpContext.Session.SetString("UserType", "User");

                return user.IsAdmin ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Attendance");
            }


            ViewBag.Error = "Correo o contraseña inválidos.";
            return View();
        }

        [HttpPost]
        public IActionResult LoginWithQr(string qrData)
        {
            // Buscar primero en Users
            var user = _context.Users.FirstOrDefault(u => u.QrCode == qrData);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserID", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
                HttpContext.Session.SetString("UserType", "User");

                return Json(new { 
                    success = true, 
                    redirectUrl = user.IsAdmin ? Url.Action("Index", "Admin") : Url.Action("Index", "Attendance")
                });
            }


            return Json(new { success = false, message = "Código QR inválido" });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}