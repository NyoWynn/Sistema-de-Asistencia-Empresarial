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
        private readonly IGeolocationService _geolocationService;

        public AttendanceController(ApplicationDbContext context, IEmailService emailService, IGeolocationService geolocationService)
        {
            _context = context;
            _emailService = emailService;
            _geolocationService = geolocationService;
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
        public async Task<IActionResult> MarkAttendance(string recordType, double? latitude = null, double? longitude = null)
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

            // Validación de geolocalización
            if (latitude.HasValue && longitude.HasValue)
            {
                var isWithinRadius = await _geolocationService.IsWithinOfficeRadiusAsync(latitude.Value, longitude.Value);
                if (!isWithinRadius)
                {
                    var officeLocation = await _geolocationService.GetActiveOfficeLocationAsync();
                    if (officeLocation != null)
                    {
                        var distance = _geolocationService.CalculateDistance(
                            latitude.Value, longitude.Value,
                            officeLocation.Latitude, officeLocation.Longitude
                        );
                        TempData["Error"] = $"❌ No puedes marcar asistencia desde esta ubicación. Debes estar dentro del radio de {officeLocation.RadiusMeters}m de la oficina. Distancia actual: {distance:F0}m";
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                // Si no se proporciona ubicación, verificar si hay una oficina configurada
                var officeLocation = await _geolocationService.GetActiveOfficeLocationAsync();
                if (officeLocation != null)
                {
                    TempData["Error"] = "❌ Se requiere acceso a la ubicación GPS para marcar asistencia. Por favor, permite el acceso a tu ubicación.";
                    return RedirectToAction("Index");
                }
            }

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