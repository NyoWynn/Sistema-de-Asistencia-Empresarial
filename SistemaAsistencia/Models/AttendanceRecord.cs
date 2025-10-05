using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Display(Name = "Fecha y Hora")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Tipo de Registro")]
        public string RecordType { get; set; } // "Entrada" o "Salida"

        // Propiedad de navegación para que EF sepa la relación
        public virtual User? User { get; set; }
    }
}