using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;
using SistemaAsistencia.Services;

namespace SistemaAsistencia.Controllers
{
    public class OfficeLocationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeolocationService _geolocationService;

        public OfficeLocationController(ApplicationDbContext context, IGeolocationService geolocationService)
        {
            _context = context;
            _geolocationService = geolocationService;
        }

        // GET: OfficeLocation
        public async Task<IActionResult> Index()
        {
            return View(await _context.OfficeLocations.ToListAsync());
        }

        // GET: OfficeLocation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var officeLocation = await _context.OfficeLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officeLocation == null)
            {
                return NotFound();
            }

            return View(officeLocation);
        }

        // GET: OfficeLocation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OfficeLocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Latitude,Longitude,RadiusMeters,IsActive,Description")] OfficeLocation officeLocation)
        {
            if (ModelState.IsValid)
            {
                officeLocation.CreatedAt = DateTime.Now;
                officeLocation.UpdatedAt = DateTime.Now;

                // Si se marca como activo, desactivar las demás ubicaciones
                if (officeLocation.IsActive)
                {
                    var activeLocations = await _context.OfficeLocations
                        .Where(ol => ol.IsActive)
                        .ToListAsync();
                    
                    foreach (var location in activeLocations)
                    {
                        location.IsActive = false;
                    }
                }

                _context.Add(officeLocation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ubicación de oficina creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(officeLocation);
        }

        // GET: OfficeLocation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var officeLocation = await _context.OfficeLocations.FindAsync(id);
            if (officeLocation == null)
            {
                return NotFound();
            }
            return View(officeLocation);
        }

        // POST: OfficeLocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Latitude,Longitude,RadiusMeters,IsActive,Description")] OfficeLocation officeLocation)
        {
            if (id != officeLocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    officeLocation.UpdatedAt = DateTime.Now;

                    // Si se marca como activo, desactivar las demás ubicaciones
                    if (officeLocation.IsActive)
                    {
                        var activeLocations = await _context.OfficeLocations
                            .Where(ol => ol.IsActive && ol.Id != id)
                            .ToListAsync();
                        
                        foreach (var location in activeLocations)
                        {
                            location.IsActive = false;
                        }
                    }

                    _context.Update(officeLocation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Ubicación de oficina actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfficeLocationExists(officeLocation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(officeLocation);
        }

        // GET: OfficeLocation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var officeLocation = await _context.OfficeLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (officeLocation == null)
            {
                return NotFound();
            }

            return View(officeLocation);
        }

        // POST: OfficeLocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var officeLocation = await _context.OfficeLocations.FindAsync(id);
            if (officeLocation != null)
            {
                _context.OfficeLocations.Remove(officeLocation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Ubicación de oficina eliminada exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: OfficeLocation/SetCurrentLocation
        public IActionResult SetCurrentLocation()
        {
            return View();
        }

        // POST: OfficeLocation/SetCurrentLocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCurrentLocation(double latitude, double longitude, string name, string address, int radiusMeters = 100)
        {
            try
            {
                // Desactivar ubicaciones existentes
                var activeLocations = await _context.OfficeLocations
                    .Where(ol => ol.IsActive)
                    .ToListAsync();
                
                foreach (var location in activeLocations)
                {
                    location.IsActive = false;
                }

                // Crear nueva ubicación
                var officeLocation = new OfficeLocation
                {
                    Name = name ?? "Ubicación Principal",
                    Address = address ?? "Dirección no especificada",
                    Latitude = latitude,
                    Longitude = longitude,
                    RadiusMeters = radiusMeters,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Description = "Ubicación configurada desde el navegador"
                };

                _context.Add(officeLocation);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Ubicación de oficina configurada exitosamente. Radio: {radiusMeters}m";
                return Json(new { success = true, message = "Ubicación configurada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private bool OfficeLocationExists(int id)
        {
            return _context.OfficeLocations.Any(e => e.Id == id);
        }
    }
}
