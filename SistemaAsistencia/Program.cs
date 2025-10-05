using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));




builder.Services.AddControllersWithViews();
builder.Services.AddScoped<SistemaAsistencia.Services.QrService>();
builder.Services.AddScoped<SistemaAsistencia.Services.IEmailService, SistemaAsistencia.Services.EmailService>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicializar la base de datos y crear datos por defecto
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Aplicar migraciones automáticamente
        context.Database.Migrate();
        
        // Crear configuración de empresa por defecto si no existe
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

        // Crear usuario administrador por defecto si no existe
        if (!context.Users.Any(u => u.IsAdmin))
        {
            context.Users.Add(new User
            {
                Name = "Administrador",
                Email = "admin@sistema.com",
                Password = "admin123", // Contraseña por defecto
                IsAdmin = true
            });
            context.SaveChanges();
        }

    }
    catch (Exception ex)
    {
        // Log del error si es necesario
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();