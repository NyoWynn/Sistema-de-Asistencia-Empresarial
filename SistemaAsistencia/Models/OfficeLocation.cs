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
        [Required(ErrorMessage = "La dirección es requerida")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Latitud")]
        [Required(ErrorMessage = "La latitud es requerida")]
        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public double Latitude { get; set; }

        [Display(Name = "Longitud")]
        [Required(ErrorMessage = "La longitud es requerida")]
        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public double Longitude { get; set; }

        [Display(Name = "Radio de Tolerancia (metros)")]
        [Required(ErrorMessage = "El radio de tolerancia es requerido")]
        [Range(10, 1000, ErrorMessage = "El radio debe estar entre 10 y 1000 metros")]
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
