using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAsistencia.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string Name { get; set; }

        [Display(Name = "¿Es Administrador?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Código QR")]
        public string? QrCode { get; set; }

        // Nuevos campos para funcionalidades avanzadas
        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Fecha de Contratación")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Salario Base")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BaseSalary { get; set; }

        [Display(Name = "¿Está Activo?")]
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}