using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class OfficeLocation
    {
        public int Id { get; set; }

        [Display(Name = "Nombre de la Ubicación")]
        [Required(ErrorMessage = "El nombre de la ubicación es requerido")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Latitud")]
        [Required(ErrorMessage = "La latitud es requerida")]
        public double Latitude { get; set; }

        [Display(Name = "Longitud")]
        [Required(ErrorMessage = "La longitud es requerida")]
        public double Longitude { get; set; }

        [Display(Name = "Radio de Tolerancia (metros)")]
        public int RadiusMeters { get; set; } = 100;

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Actualización")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }
    }
}
