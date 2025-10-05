using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class EmailSettingsViewModel
    {
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
        public string? WelcomeEmailTemplate { get; set; }

        [Display(Name = "Plantilla de Email de Asistencia")]
        [DataType(DataType.MultilineText)]
        public string? AttendanceEmailTemplate { get; set; }
    }
}





