
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaAsistencia.Models
{
    // Clase principal que contiene todos los datos para la vista del reporte
    public class MonthlyReportViewModel
    {
        [Display(Name = "Mes")]
        public int Month { get; set; }

        [Display(Name = "Año")]
        public int Year { get; set; }

        public List<string> DayHeaders { get; set; } = new List<string>();
        public List<EmployeeMonthlyStatus> EmployeeStatuses { get; set; } = new List<EmployeeMonthlyStatus>();
        
        // Propiedades para paginación y búsqueda
        public string SearchName { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalUsers { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalUsers / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    // Representa la fila de un solo empleado en el reporte
    public class EmployeeMonthlyStatus
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Dictionary<int, DailyStatusViewModel> DailyStatuses { get; set; } = new Dictionary<int, DailyStatusViewModel>();
    }

    // Representa el estado de un empleado en un solo día (una celda de la tabla)
    public class DailyStatusViewModel
    {
        public AttendanceStatus Status { get; set; }
        public TimeSpan? ClockInTime { get; set; }
        public TimeSpan? ClockOutTime { get; set; }
    }

    // Un 'enum' para manejar los estados de forma más limpia y segura
    public enum AttendanceStatus
    {
        Attended,
        Absent,
        LateArrival,
        EarlyDeparture,
        Incomplete // Solo marcó entrada o salida
    }
}