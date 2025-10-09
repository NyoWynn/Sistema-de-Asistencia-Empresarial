using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "¿Es Administrador?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Código QR")]
        public string? QrCode { get; set; }

        // Propiedades de navegación
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}