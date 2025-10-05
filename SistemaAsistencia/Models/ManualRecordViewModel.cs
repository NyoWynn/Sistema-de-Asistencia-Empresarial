using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    public class ManualRecordViewModel
    {
        [Display(Name = "Seleccionar Empleado")]
        public int UserId { get; set; }

        [Display(Name = "Fecha del Registro")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Display(Name = "Hora de Entrada")]
        [DataType(DataType.Time)]
        public TimeSpan? ClockInTime { get; set; }

        [Display(Name = "Hora de Salida")]
        [DataType(DataType.Time)]
        public TimeSpan? ClockOutTime { get; set; }

        // Lista de empleados para mostrar en el formulario
        public SelectList? Users { get; set; }
    }
}