using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class CompanySettings
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [Display(Name = "Nombre de la Empresa")]
        public string CompanyName { get; set; }

        [Display(Name = "Hora de Entrada")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM por defecto

        [Display(Name = "Hora de Salida")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0); // 5:00 PM por defecto

        [Display(Name = "Tolerancia de Llegada Tarde (minutos)")]
        [Range(0, 60, ErrorMessage = "La tolerancia debe estar entre 0 y 60 minutos")]
        public int LateArrivalTolerance { get; set; } = 15; // 15 minutos por defecto

        [Display(Name = "Tolerancia de Salida Temprana (minutos)")]
        [Range(0, 60, ErrorMessage = "La tolerancia debe estar entre 0 y 60 minutos")]
        public int EarlyDepartureTolerance { get; set; } = 15; // 15 minutos por defecto

        [Display(Name = "Días de Trabajo")]
        public string WorkingDays { get; set; } = "Lunes,Martes,Miércoles,Jueves,Viernes";

        [Display(Name = "Configuración Activa")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Actualización")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Configuración de Email
        [Display(Name = "Servidor SMTP")]
        public string? SmtpServer { get; set; }

        [Display(Name = "Puerto SMTP")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "Usuario de Email")]
        public string? EmailUsername { get; set; }

        [Display(Name = "Contraseña de Email")]
        [DataType(DataType.Password)]
        public string? EmailPassword { get; set; }

        [Display(Name = "Email de Envío")]
        [EmailAddress]
        public string? FromEmail { get; set; }

        [Display(Name = "Nombre del Remitente")]
        public string? FromName { get; set; } = "Sistema de Asistencia";

        [Display(Name = "Email Habilitado")]
        public bool EmailEnabled { get; set; } = false;

        // Plantillas de Email
        [Display(Name = "Plantilla de Email de Bienvenida")]
        [DataType(DataType.MultilineText)]
        public string? WelcomeEmailTemplate { get; set; } = @"<h2>¡Bienvenido a {CompanyName}!</h2>
<p>Hola {UserName},</p>
<p>Te damos la bienvenida a nuestro sistema de asistencia.</p>
<p>Tu código QR para marcar asistencia es: <strong>{QrCode}</strong></p>
<p>Puedes acceder al sistema en: {SystemUrl}</p>
<p>Saludos,<br>Equipo de {CompanyName}";

        [Display(Name = "Plantilla de Email de Asistencia")]
        [DataType(DataType.MultilineText)]
        public string? AttendanceEmailTemplate { get; set; } = @"<h2>Registro de Asistencia</h2>
<p>Hola {UserName},</p>
<p>Se ha registrado tu {RecordType} el {Date} a las {Time}.</p>
<p>Detalles del registro:</p>
<ul>
    <li><strong>Tipo:</strong> {RecordType}</li>
    <li><strong>Fecha:</strong> {Date}</li>
    <li><strong>Hora:</strong> {Time}</li>
    <li><strong>Estado:</strong> {Status}</li>
</ul>
<p>Saludos,<br>Sistema de Asistencia de {CompanyName}";
    }
}
