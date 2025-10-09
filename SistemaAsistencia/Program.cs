using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de base de datos por entorno
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // SQL Server para desarrollo local
        options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
    }
    else
    {
        // SQLite para producción (AWS)
        options.UseSqlite(connectionString);
    }
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<SistemaAsistencia.Services.QrService>();
builder.Services.AddScoped<SistemaAsistencia.Services.IEmailService, SistemaAsistencia.Services.EmailService>();
builder.Services.AddScoped<SistemaAsistencia.Services.IGeolocationService, SistemaAsistencia.Services.GeolocationService>();

builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(20);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicialización de base de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Aplicar migraciones en desarrollo (SQL Server)
        if (builder.Environment.IsDevelopment())
        {
            context.Database.Migrate();
        }
        else
        {
            // En producción (SQLite), usar EnsureCreated
            context.Database.EnsureCreated();
        }

        // Verificar y crear datos por defecto si no existen
        if (!context.CompanySettings.Any())
        {
            context.CompanySettings.Add(new CompanySettings
            {
                CompanyName = "Mi Empresa",
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                LateArrivalTolerance = 15,
                EarlyDepartureTolerance = 15,
                WorkingDays = "Lunes,Martes,Miércoles,Jueves,Viernes",
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
            context.SaveChanges();
        }

        if (!context.Users.Any(u => u.IsAdmin))
        {
            context.Users.Add(new User
            {
                Name = "Administrador",
                Email = "admin@sistema.com",
                Password = "admin123",
                IsAdmin = true
            });
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Si no tienes HTTPS en Nginx aún, puedes desactivar esta línea en prod.
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
