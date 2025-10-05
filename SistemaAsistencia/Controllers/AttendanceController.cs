using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;
using SistemaAsistencia.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaAsistencia.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AttendanceController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Buscar el último registro del usuario para el día de hoy
            var lastRecordToday = await _context.AttendanceRecords
                .Where(r => r.UserId == userId.Value && r.Timestamp.Date == DateTime.Today)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            string attendanceStatus;
            if (lastRecordToday == null)
            {
                // No ha marcado nada hoy - puede marcar entrada
                attendanceStatus = "CanClockIn";
            }
            else if (lastRecordToday.RecordType == "Entrada")
            {
                // Su último registro fue una entrada - puede marcar salida
                attendanceStatus = "CanClockOut";
            }
            else
            {
                // Ya marcó su salida por hoy - jornada completada
                attendanceStatus = "CompletedDay";
            }

            ViewBag.AttendanceStatus = attendanceStatus;
            ViewBag.LastRecord = lastRecordToday;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(string recordType)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Validación extra en el servidor para mayor seguridad
            var lastRecordToday = await _context.AttendanceRecords
                .Where(r => r.UserId == userId.Value && r.Timestamp.Date == DateTime.Today)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            // Validaciones mejoradas
            if (recordType == "Entrada")
            {
                if (lastRecordToday != null && lastRecordToday.RecordType == "Entrada")
                {
                    TempData["Error"] = "Ya has marcado entrada hoy. No puedes marcar entrada nuevamente.";
                    return RedirectToAction("Index");
                }
            }
            else if (recordType == "Salida")
            {
                if (lastRecordToday == null || lastRecordToday.RecordType == "Salida")
                {
                    TempData["Error"] = "Debes marcar entrada antes de poder marcar salida.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["Error"] = "Tipo de registro no válido.";
                return RedirectToAction("Index");
            }

            var record = new AttendanceRecord
            {
                UserId = userId.Value,
                Timestamp = DateTime.Now,
                RecordType = recordType
            };
            _context.AttendanceRecords.Add(record);
            await _context.SaveChangesAsync();

            // Obtener información del usuario para el email
            var user = await _context.Users.FindAsync(userId.Value);
            
            // Enviar comprobante por email
            try
            {
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendAttendanceConfirmationAsync(user.Email, user.Name, recordType, record.Timestamp);
                }
            }
            catch (Exception ex)
            {
                // Log del error pero no fallar el registro de asistencia
                // El registro ya se guardó exitosamente
            }

            var timeString = record.Timestamp.ToString("HH:mm:ss");
            var dateString = record.Timestamp.ToString("dd/MM/yyyy");
            TempData["Message"] = $"✅ Se ha registrado tu '{recordType}' el {dateString} a las {timeString}. Se ha enviado un comprobante a tu correo.";
            return RedirectToAction("Index");
        }
    }
}