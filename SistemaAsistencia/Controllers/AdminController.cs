using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;
using SistemaAsistencia.Services;

namespace SistemaAsistencia.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly QrService _qrService;
        private readonly IEmailService _emailService;

        public AdminController(ApplicationDbContext context, QrService qrService, IEmailService emailService)
        {
            _context = context;
            _qrService = qrService;
            _emailService = emailService;
        }

        // GET: Admin
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var users = from u in _context.Users
                        select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Name.Contains(searchString) || u.Email.Contains(searchString));
            }

            // Aplicar paginación
            var totalRecords = await users.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            var pagedUsers = await users
                .OrderBy(u => u.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalRecords"] = totalRecords;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;
            ViewData["HasNextPage"] = pageNumber < totalPages;
            ViewData["PreviousPageNumber"] = pageNumber - 1;
            ViewData["NextPageNumber"] = pageNumber + 1;

            return View(pagedUsers);
        }

     

        // GET: Admin/LateArrivalsReport
        public async Task<IActionResult> LateArrivalsReport(int pageNumber = 1, int pageSize = 20)
        {
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
            var lateTime = companySettings?.StartTime.Add(TimeSpan.FromMinutes(companySettings?.LateArrivalTolerance ?? 15)) ?? new TimeSpan(9, 30, 0);
            
            var lateArrivalsQuery = _context.AttendanceRecords
                .Include(r => r.User) 
                .Where(r => r.RecordType == "Entrada" && r.Timestamp.TimeOfDay > lateTime)
                .OrderBy(r => r.Timestamp);

            // Aplicar paginación
            var totalRecords = await lateArrivalsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            var lateArrivals = await lateArrivalsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CompanySettings = companySettings;
            ViewData["TotalRecords"] = totalRecords;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;
            ViewData["HasNextPage"] = pageNumber < totalPages;
            ViewData["PreviousPageNumber"] = pageNumber - 1;
            ViewData["NextPageNumber"] = pageNumber + 1;

            return View(lateArrivals);
        }

        // GET: Admin/EarlyDeparturesReport
        public async Task<IActionResult> EarlyDeparturesReport(int pageNumber = 1, int pageSize = 20)
        {
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
            var earlyTime = companySettings?.EndTime.Subtract(TimeSpan.FromMinutes(companySettings?.EarlyDepartureTolerance ?? 15)) ?? new TimeSpan(17, 30, 0);
            
            var earlyDeparturesQuery = _context.AttendanceRecords
                .Include(r => r.User) 
                .Where(r => r.RecordType == "Salida" && r.Timestamp.TimeOfDay < earlyTime)
                .OrderBy(r => r.Timestamp);

            // Aplicar paginación
            var totalRecords = await earlyDeparturesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            var earlyDepartures = await earlyDeparturesQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CompanySettings = companySettings;
            ViewData["TotalRecords"] = totalRecords;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;
            ViewData["HasNextPage"] = pageNumber < totalPages;
            ViewData["PreviousPageNumber"] = pageNumber - 1;
            ViewData["NextPageNumber"] = pageNumber + 1;

            return View(earlyDepartures);
        }

        // GET: Admin/AbsenceReport
        public async Task<IActionResult> AbsenceReport(DateTime? reportDate, int pageNumber = 1, int pageSize = 20)
        {
            ViewData["CurrentPage"] = pageNumber;
            ViewData["PageSize"] = pageSize;

            // Si no se especifica una fecha, usamos el día de hoy
            var date = reportDate ?? DateTime.Today;
            ViewData["ReportDate"] = date.ToString("yyyy-MM-dd");

            // Obtenemos los IDs de los usuarios que SÍ registraron asistencia en esa fecha
            var usersWithAttendance = await _context.AttendanceRecords
                .Where(r => r.Timestamp.Date == date.Date)
                .Select(r => r.UserId)
                .Distinct()
                .ToListAsync();

            // Buscamos a todos los usuarios que NO están en la lista anterior
            // PERO solo si fueron creados ANTES o EN la fecha del reporte
            var absentUsersQuery = _context.Users
                .Where(u => !u.IsAdmin && 
                           !usersWithAttendance.Contains(u.Id))
                .OrderBy(u => u.Name);

            // Aplicar paginación
            var totalRecords = await absentUsersQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            
            var absentUsers = await absentUsersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalRecords"] = totalRecords;
            ViewData["TotalPages"] = totalPages;
            ViewData["HasPreviousPage"] = pageNumber > 1;
            ViewData["HasNextPage"] = pageNumber < totalPages;
            ViewData["PreviousPageNumber"] = pageNumber - 1;
            ViewData["NextPageNumber"] = pageNumber + 1;

            return View(absentUsers);
        }

      
        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) { return NotFound(); }
            return View(user);
        }
        // GET: Admin/Create
        public IActionResult Create() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,Name,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                // Generar código QR único para el usuario
                string qrData = $"USER_{user.Email}_{DateTime.Now.Ticks}";
                user.QrCode = qrData;
                
                _context.Add(user);
                await _context.SaveChangesAsync();

                // Enviar email de bienvenida
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.Name, qrData, user.Password);
                    TempData["SuccessMessage"] = $"Usuario creado exitosamente y email de bienvenida enviado a {user.Email}";
                }
                catch (Exception ex)
                {
                    // Log del error pero no fallar la creación del usuario
                    TempData["WarningMessage"] = $"Usuario creado exitosamente, pero hubo un error al enviar el email: {ex.Message}";
                }

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }
            var user = await _context.Users.FindAsync(id);
            if (user == null) { return NotFound(); }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Password,Name,IsAdmin")] User user)
        {
            if (id != user.Id) { return NotFound(); }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return NotFound(); }
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) { return NotFound(); }
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null) { _context.Users.Remove(user); }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool UserExists(int id) { return _context.Users.Any(e => e.Id == id); }
       

        // GET: Admin/ManualRecord
        public async Task<IActionResult> ManualRecord()
        {
            var viewModel = new ManualRecordViewModel
            {
             
                Users = new SelectList(await _context.Users.Where(u => !u.IsAdmin).ToListAsync(), "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: Admin/ManualRecord
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualRecord(ManualRecordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                
                viewModel.Users = new SelectList(await _context.Users.Where(u => !u.IsAdmin).ToListAsync(), "Id", "Name", viewModel.UserId);
                return View(viewModel);
            }

            var existingRecords = await _context.AttendanceRecords
                .Where(r => r.UserId == viewModel.UserId && r.Timestamp.Date == viewModel.Date.Date)
                .ToListAsync();

            if (existingRecords.Any())
            {
                _context.AttendanceRecords.RemoveRange(existingRecords);
            }

          
            if (viewModel.ClockInTime.HasValue)
            {
                var clockInDateTime = viewModel.Date.Date.Add(viewModel.ClockInTime.Value);
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    UserId = viewModel.UserId,
                    Timestamp = clockInDateTime,
                    RecordType = "Entrada"
                });
            }
          
            if (viewModel.ClockOutTime.HasValue)
            {
                var clockOutDateTime = viewModel.Date.Date.Add(viewModel.ClockOutTime.Value);
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    UserId = viewModel.UserId,
                    Timestamp = clockOutDateTime,
                    RecordType = "Salida"
                });
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "El registro de asistencia se ha actualizado correctamente.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> MonthlyReport(int? month, int? year, string searchName = "", int page = 1, int pageSize = 10)
        {
            // Usar el mes y año actual si no se especifican
            int reportMonth = month ?? DateTime.Now.Month;
            int reportYear = year ?? DateTime.Now.Year;

            var startDate = new DateTime(reportYear, reportMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // 1. Obtener usuarios con filtro de búsqueda y paginación
            var usersQuery = _context.Users.Where(u => !u.IsAdmin);
            
            if (!string.IsNullOrEmpty(searchName))
            {
                usersQuery = usersQuery.Where(u => u.Name.Contains(searchName));
            }
            
            var totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery
                .OrderBy(u => u.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            var recordsForMonth = await _context.AttendanceRecords
                .Where(r => r.Timestamp.Date >= startDate.Date && r.Timestamp.Date <= endDate.Date)
                .ToListAsync();

            var viewModel = new MonthlyReportViewModel
            {
                Month = reportMonth,
                Year = reportYear,
                SearchName = searchName,
                CurrentPage = page,
                PageSize = pageSize,
                TotalUsers = totalUsers
            };

            // 2. Crear las cabeceras de los días (1, 2, 3, ...)
            for (int i = 1; i <= endDate.Day; i++)
            {
                viewModel.DayHeaders.Add(i.ToString());
            }

            // 3. Procesar los datos para cada empleado
            foreach (var user in users)
            {
                var employeeStatus = new EmployeeMonthlyStatus
                {
                    UserId = user.Id,
                    UserName = user.Name
                };

                for (int day = 1; day <= endDate.Day; day++)
                {
                    var currentDate = new DateTime(reportYear, reportMonth, day);
                    var recordsForDay = recordsForMonth
                        .Where(r => r.UserId == user.Id && r.Timestamp.Date == currentDate.Date)
                        .ToList();

                    var clockInRecord = recordsForDay.FirstOrDefault(r => r.RecordType == "Entrada");
                    var clockOutRecord = recordsForDay.FirstOrDefault(r => r.RecordType == "Salida");

                    var dailyStatus = new DailyStatusViewModel();

                    if (clockInRecord == null && clockOutRecord == null)
                    {
                        dailyStatus.Status = AttendanceStatus.Absent;
                    }
                    else if (clockInRecord == null || clockOutRecord == null)
                    {
                        dailyStatus.Status = AttendanceStatus.Incomplete;
                    }
                    else
                    {
                        dailyStatus.ClockInTime = clockInRecord.Timestamp.TimeOfDay;
                        dailyStatus.ClockOutTime = clockOutRecord.Timestamp.TimeOfDay;

                        var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
                        var lateThreshold = companySettings?.StartTime.Add(TimeSpan.FromMinutes(companySettings?.LateArrivalTolerance ?? 15)) ?? new TimeSpan(9, 30, 0);
                        var earlyThreshold = companySettings?.EndTime.Subtract(TimeSpan.FromMinutes(companySettings?.EarlyDepartureTolerance ?? 15)) ?? new TimeSpan(17, 30, 0);
                        
                        bool isLate = dailyStatus.ClockInTime > lateThreshold;
                        bool isEarly = dailyStatus.ClockOutTime < earlyThreshold;

                        if (isLate) dailyStatus.Status = AttendanceStatus.LateArrival;
                        else if (isEarly) dailyStatus.Status = AttendanceStatus.EarlyDeparture;
                        else dailyStatus.Status = AttendanceStatus.Attended;
                    }
                    employeeStatus.DailyStatuses.Add(day, dailyStatus);
                }
                viewModel.EmployeeStatuses.Add(employeeStatus);
            }

            return View(viewModel);
        }

        // GET: Admin/CompanySettings
        public async Task<IActionResult> CompanySettings()
        {
            var settings = await _context.CompanySettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new CompanySettings();
            }
            else
            {
                // Asegurar que los campos de email tengan valores por defecto si son null
                if (string.IsNullOrEmpty(settings.SmtpServer))
                    settings.SmtpServer = "smtp.gmail.com";
                if (settings.SmtpPort == 0)
                    settings.SmtpPort = 587;
                if (string.IsNullOrEmpty(settings.FromName))
                    settings.FromName = "Sistema de Asistencia";
                
                // Limpiar campos de email si contienen valores de prueba
                if (settings.EmailUsername == "ncamposmaldonado@gmail.com" || 
                    settings.EmailUsername == "tu-email@gmail.com")
                {
                    settings.EmailUsername = "";
                }
                if (settings.FromEmail == "ncamposmaldonado@gmail.com" || 
                    settings.FromEmail == "tu-email@gmail.com")
                {
                    settings.FromEmail = "";
                }
            }
            return View(settings);
        }

        // POST: Admin/CompanySettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanySettings(CompanySettings model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar configuración de email si está habilitada
                    if (model.EmailEnabled)
                    {
                        if (string.IsNullOrEmpty(model.SmtpServer))
                        {
                            ModelState.AddModelError("SmtpServer", "El servidor SMTP es requerido cuando el email está habilitado");
                        }
                        if (model.SmtpPort <= 0)
                        {
                            ModelState.AddModelError("SmtpPort", "El puerto SMTP debe ser mayor a 0");
                        }
                        if (string.IsNullOrEmpty(model.EmailUsername))
                        {
                            ModelState.AddModelError("EmailUsername", "El usuario de email es requerido cuando el email está habilitado");
                        }
                        if (string.IsNullOrEmpty(model.EmailPassword))
                        {
                            ModelState.AddModelError("EmailPassword", "La contraseña de email es requerida cuando el email está habilitado");
                        }
                        if (string.IsNullOrEmpty(model.FromEmail))
                        {
                            ModelState.AddModelError("FromEmail", "El email de envío es requerido cuando el email está habilitado");
                        }
                    }

                    // Si hay errores de validación, mostrar la vista con errores
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    var existingSettings = await _context.CompanySettings.FirstOrDefaultAsync();
                    if (existingSettings != null)
                    {
                        existingSettings.CompanyName = model.CompanyName;
                        existingSettings.StartTime = model.StartTime;
                        existingSettings.EndTime = model.EndTime;
                        existingSettings.LateArrivalTolerance = model.LateArrivalTolerance;
                        existingSettings.EarlyDepartureTolerance = model.EarlyDepartureTolerance;
                        existingSettings.WorkingDays = model.WorkingDays;
                        
                        // Configuración de email - solo actualizar si se proporcionan valores
                        if (!string.IsNullOrEmpty(model.SmtpServer))
                            existingSettings.SmtpServer = model.SmtpServer.Trim();
                        if (model.SmtpPort > 0)
                            existingSettings.SmtpPort = model.SmtpPort;
                        if (!string.IsNullOrEmpty(model.EmailUsername))
                            existingSettings.EmailUsername = model.EmailUsername.Trim();
                        if (!string.IsNullOrEmpty(model.EmailPassword))
                            existingSettings.EmailPassword = model.EmailPassword.Replace(" ", "").Trim(); // Quitar espacios
                        if (!string.IsNullOrEmpty(model.FromEmail))
                            existingSettings.FromEmail = model.FromEmail.Trim();
                        if (!string.IsNullOrEmpty(model.FromName))
                            existingSettings.FromName = model.FromName.Trim();
                        
                        existingSettings.EmailEnabled = model.EmailEnabled;
                        
                        // Actualizar plantillas de email
                        if (!string.IsNullOrEmpty(model.WelcomeEmailTemplate))
                            existingSettings.WelcomeEmailTemplate = model.WelcomeEmailTemplate;
                        if (!string.IsNullOrEmpty(model.AttendanceEmailTemplate))
                            existingSettings.AttendanceEmailTemplate = model.AttendanceEmailTemplate;
                        
                        existingSettings.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        // Asegurar valores por defecto para email
                        if (string.IsNullOrEmpty(model.SmtpServer))
                            model.SmtpServer = "smtp.gmail.com";
                        if (model.SmtpPort == 0)
                            model.SmtpPort = 587;
                        if (string.IsNullOrEmpty(model.FromName))
                            model.FromName = "Sistema de Asistencia";
                            
                        model.CreatedAt = DateTime.Now;
                        model.UpdatedAt = DateTime.Now;
                        _context.CompanySettings.Add(model);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Configuración de empresa actualizada exitosamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar la configuración: {ex.Message}");
                }
            }

            return View(model);
        }

        // POST: Admin/UpdateEmailSettings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmailSettings(EmailSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar configuración de email si está habilitada
                    if (model.EmailEnabled)
                    {
                        if (string.IsNullOrEmpty(model.SmtpServer))
                        {
                            ModelState.AddModelError("SmtpServer", "El servidor SMTP es requerido cuando el email está habilitado");
                        }
                        if (model.SmtpPort <= 0)
                        {
                            ModelState.AddModelError("SmtpPort", "El puerto SMTP debe ser mayor a 0");
                        }
                        if (string.IsNullOrEmpty(model.EmailUsername))
                        {
                            ModelState.AddModelError("EmailUsername", "El usuario de email es requerido cuando el email está habilitado");
                        }
                        if (string.IsNullOrEmpty(model.EmailPassword))
                        {
                            ModelState.AddModelError("EmailPassword", "La contraseña de email es requerida cuando el email está habilitado");
                        }
                        if (string.IsNullOrEmpty(model.FromEmail))
                        {
                            ModelState.AddModelError("FromEmail", "El email de envío es requerido cuando el email está habilitado");
                        }
                    }

                    // Si hay errores de validación, volver a la vista principal
                    if (!ModelState.IsValid)
                    {
                        var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
                        if (companySettings == null)
                        {
                            companySettings = new CompanySettings();
                        }
                        return View("CompanySettings", companySettings);
                    }

                    var existingSettings = await _context.CompanySettings.FirstOrDefaultAsync();
                    if (existingSettings != null)
                    {
                        // Solo actualizar campos de email
                        if (!string.IsNullOrEmpty(model.SmtpServer))
                            existingSettings.SmtpServer = model.SmtpServer.Trim();
                        if (model.SmtpPort > 0)
                            existingSettings.SmtpPort = model.SmtpPort;
                        if (!string.IsNullOrEmpty(model.EmailUsername))
                            existingSettings.EmailUsername = model.EmailUsername.Trim();
                        if (!string.IsNullOrEmpty(model.EmailPassword))
                            existingSettings.EmailPassword = model.EmailPassword.Replace(" ", "").Trim();
                        if (!string.IsNullOrEmpty(model.FromEmail))
                            existingSettings.FromEmail = model.FromEmail.Trim();
                        if (!string.IsNullOrEmpty(model.FromName))
                            existingSettings.FromName = model.FromName.Trim();
                        
                        existingSettings.EmailEnabled = model.EmailEnabled;
                        
                        // Actualizar plantillas de email
                        if (!string.IsNullOrEmpty(model.WelcomeEmailTemplate))
                            existingSettings.WelcomeEmailTemplate = model.WelcomeEmailTemplate;
                        if (!string.IsNullOrEmpty(model.AttendanceEmailTemplate))
                            existingSettings.AttendanceEmailTemplate = model.AttendanceEmailTemplate;
                        
                        existingSettings.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        // Crear nueva configuración solo con datos de email
                        var newSettings = new CompanySettings
                        {
                            CompanyName = "Mi Empresa", // Valor por defecto
                            StartTime = new TimeSpan(8, 0, 0),
                            EndTime = new TimeSpan(17, 0, 0),
                            LateArrivalTolerance = 15,
                            EarlyDepartureTolerance = 15,
                            WorkingDays = "Lunes,Martes,Miércoles,Jueves,Viernes",
                            IsActive = true,
                            SmtpServer = model.SmtpServer?.Trim(),
                            SmtpPort = model.SmtpPort,
                            EmailUsername = model.EmailUsername?.Trim(),
                            EmailPassword = model.EmailPassword?.Replace(" ", "").Trim(),
                            FromEmail = model.FromEmail?.Trim(),
                            FromName = model.FromName?.Trim(),
                            EmailEnabled = model.EmailEnabled,
                            WelcomeEmailTemplate = model.WelcomeEmailTemplate,
                            AttendanceEmailTemplate = model.AttendanceEmailTemplate,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.CompanySettings.Add(newSettings);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Configuración de email actualizada exitosamente.";
                    return RedirectToAction("CompanySettings");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al guardar la configuración de email: {ex.Message}");
                }
            }

            // Si hay errores, volver a la vista principal
            var settings = await _context.CompanySettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new CompanySettings();
            }
            return View("CompanySettings", settings);
        }

        // POST: Admin/TestEmailConnection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestEmailConnection()
        {
            try
            {
                var emailService = HttpContext.RequestServices.GetRequiredService<IEmailService>();
                var isConnected = await emailService.TestEmailConnectionAsync();
                
                return Json(new { success = isConnected, message = isConnected ? "Conexión exitosa" : "Error de conexión" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult ShowQr(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(user.QrCode))
            {
                // Generar QR si no existe
                string qrData = $"USER_{user.Email}_{DateTime.Now.Ticks}";
                user.QrCode = qrData;
                _context.SaveChanges();
            }

            ViewBag.User = user;
            ViewBag.QrImageBase64 = _qrService.GenerateQrCode(user.QrCode);
            return View();
        }

        public IActionResult DownloadQr(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null || string.IsNullOrEmpty(user.QrCode))
            {
                return NotFound();
            }

            var qrBytes = _qrService.GenerateQrCodeBytes(user.QrCode);
            return File(qrBytes, "image/png", $"{user.Name}_QR.png");
        }

        // GET: Admin/ViewUserProfile/5
        public async Task<IActionResult> ViewUserProfile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Obtener el historial de asistencia del usuario
            var attendanceHistory = await _context.AttendanceRecords
                .Where(r => r.UserId == id)
                .OrderByDescending(r => r.Timestamp)
                .Take(30) // Últimos 30 registros
                .ToListAsync();

            ViewBag.AttendanceHistory = attendanceHistory;
            return View(user);
        }

        // GET: Admin/SearchUsers
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                return Json(new List<object>());
            }

            var users = await _context.Users
                .Where(u => !u.IsAdmin && u.Name.Contains(query))
                .Select(u => new { id = u.Id, name = u.Name, email = u.Email })
                .Take(10)
                .ToListAsync();

            return Json(users);
        }

        [HttpPost]
        public async Task<IActionResult> TestEmail(string testEmail, string smtpServer, int smtpPort, string emailUsername, string emailPassword, string fromEmail, string fromName)
        {
            try
            {
                if (string.IsNullOrEmpty(testEmail))
                {
                    return Json(new { success = false, message = "Por favor, ingresa un email de prueba" });
                }

                // Validar campos requeridos
                if (string.IsNullOrEmpty(smtpServer))
                {
                    return Json(new { success = false, message = "Servidor SMTP no configurado" });
                }

                if (smtpPort <= 0)
                {
                    return Json(new { success = false, message = "Puerto SMTP debe ser mayor a 0" });
                }

                if (string.IsNullOrEmpty(emailUsername))
                {
                    return Json(new { success = false, message = "Usuario de email no configurado" });
                }

                if (string.IsNullOrEmpty(emailPassword))
                {
                    return Json(new { success = false, message = "Contraseña de email no configurada" });
                }

                if (string.IsNullOrEmpty(fromEmail))
                {
                    return Json(new { success = false, message = "Email de envío no configurado" });
                }

                // Crear configuración temporal para la prueba
                var tempSettings = new CompanySettings
                {
                    SmtpServer = smtpServer.Trim(),
                    SmtpPort = smtpPort,
                    EmailUsername = emailUsername.Trim(),
                    EmailPassword = emailPassword.Replace(" ", "").Trim(),
                    FromEmail = fromEmail.Trim(),
                    FromName = fromName?.Trim() ?? "Sistema de Asistencia",
                    EmailEnabled = true
                };

                // Enviar email de prueba usando la configuración del formulario
                await _emailService.SendEmailAsync(testEmail, 
                    "Prueba de Email - Sistema de Asistencia", 
                    "<h2>✅ Email de Prueba Exitoso</h2><p>La configuración de email está funcionando correctamente.</p>",
                    tempSettings);

                return Json(new { success = true, message = "Email de prueba enviado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al enviar email de prueba: {ex.Message}" });
            }
        }

        // GET: Admin/OfficeLocation
        public IActionResult OfficeLocation()
        {
            return RedirectToAction("Index", "OfficeLocation");
        }

        // GET: Admin/TestGeolocation
        public IActionResult TestGeolocation()
        {
            return View();
        }
    }
}