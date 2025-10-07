using SistemaAsistencia.Models;

namespace SistemaAsistencia.Services
{
    public interface IGeolocationService
    {
        Task<bool> IsWithinOfficeRadiusAsync(double userLatitude, double userLongitude);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
        Task<OfficeLocation?> GetActiveOfficeLocationAsync();
    }

    public class GeolocationService : IGeolocationService
    {
        private readonly ApplicationDbContext _context;

        public GeolocationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsWithinOfficeRadiusAsync(double userLatitude, double userLongitude)
        {
            var officeLocation = await GetActiveOfficeLocationAsync();
            
            if (officeLocation == null)
            {
                // Si no hay ubicación configurada, permitir el registro
                return true;
            }

            var distance = CalculateDistance(
                userLatitude, userLongitude,
                officeLocation.Latitude, officeLocation.Longitude
            );

            return distance <= officeLocation.RadiusMeters;
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Radio de la Tierra en metros
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distancia en metros
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public async Task<OfficeLocation?> GetActiveOfficeLocationAsync()
        {
            return await Task.FromResult(_context.OfficeLocations
                .FirstOrDefault(ol => ol.IsActive));
        }
    }
}
