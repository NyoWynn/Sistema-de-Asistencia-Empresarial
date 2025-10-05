# 📊 Sistema de Asistencia Empresarial

Un sistema web completo y moderno para el control de asistencia de empleados desarrollado en **ASP.NET Core 8.0** con **Entity Framework Core** y **SQL Server**. Incluye funcionalidades avanzadas como códigos QR, notificaciones por email, reportes detallados y una interfaz intuitiva.

![Login QR](https://cdn.discordapp.com/attachments/1424448965457477844/1424449011661803743/image.png?ex=68e3fd05&is=68e2ab85&hm=dc1efd41ee3e5544ff9f3156a413d3001c89bceec5ac7d238213784661429757&)

## 🚀 Características Principales

### ✨ Funcionalidades Core
- **👤 Usuario Administrador Automático**: Creación automática del usuario administrador (admin@sistema.com / admin123)
- **🏢 Configuración de Empresa**: Gestión completa de horarios, tolerancias y parámetros específicos
- **📱 Sistema de Códigos QR**: Generación automática y login por código QR
- **📧 Notificaciones por Email**: Sistema completo de emails con plantillas personalizables
- **📊 Reportes Avanzados**: Reportes mensuales con paginación, búsqueda y filtros
- **🔍 Búsqueda Inteligente**: Búsqueda de empleados en tiempo real para registro manual
- **👥 Gestión de Perfiles**: Vista detallada de perfiles de usuario para administradores

### Para Empleados
- **🔐 Acceso Dual**: Login por email/contraseña o código QR
- **⏰ Registro de Asistencia**: Marcar entrada y salida con validaciones inteligentes
- **📱 Interfaz Responsive**: Diseño moderno y adaptable a cualquier dispositivo
- **📊 Dashboard Personal**: Vista de estadísticas personales y historial de asistencia
- **📧 Notificaciones**: Recibe emails de confirmación de registros de asistencia

![Panel Usuario](https://cdn.discordapp.com/attachments/1424448965457477844/1424450212839161876/image.png?ex=68e3fe23&is=68e2aca3&hm=9cca96f09063e2ae111aa08c8ed7f70a4b761352889f1a162fab1a749308bba6&)

### Para Administradores
- **👥 Gestión Completa de Usuarios**: CRUD completo con búsqueda avanzada
- **📊 Reportes Detallados**:
  - 📈 Reporte mensual con paginación y búsqueda por nombre
  - ⏰ Reporte de llegadas tardías
  - 🏃 Reporte de salidas tempranas  
  - ❌ Reporte de ausencias (excluye usuarios creados después de la fecha)
- **✏️ Registro Manual**: Sistema de búsqueda inteligente para selección de empleados
- **⚙️ Configuración Avanzada**: 
  - Horarios y tolerancias personalizables
  - Configuración completa de email SMTP
  - Plantillas de email personalizables
- **👤 Perfiles de Usuario**: Vista detallada con estadísticas y historial

![Panel Admin](https://cdn.discordapp.com/attachments/1424448965457477844/1424449115709771836/image.png?ex=68e3fd1e&is=68e2ab9e&hm=f26e4b7d810404fe83762b7d11081e1fb714f28104f45a0e0282f48e8226164a&)

## 🛠️ Tecnologías Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework web moderno
- **Entity Framework Core 9.0.8** - ORM para gestión de base de datos
- **SQL Server** - Base de datos robusta y escalable
- **MailKit** - Biblioteca para envío de emails SMTP

### Frontend
- **Razor Pages** - Motor de vistas de ASP.NET Core
- **Bootstrap 5** - Framework CSS responsive
- **JavaScript ES6+** - Funcionalidades dinámicas
- **Font Awesome** - Iconografía moderna

### Funcionalidades Adicionales
- **Códigos QR** - Generación automática con API externa
- **Sistema de Sesiones** - Autenticación personalizada
- **Migraciones EF** - Gestión automática de esquema de BD
- **Validaciones** - Prevención de registros duplicados e inválidos

## 📋 Requisitos del Sistema

### Desarrollo
- **.NET 8.0 SDK** o superior
- **SQL Server** (LocalDB, Express o Full)
- **Visual Studio 2022** o **Visual Studio Code**
- **Windows, macOS o Linux**

### Producción
- **IIS** o **Kestrel** como servidor web
- **SQL Server** (cualquier versión compatible)
- **Certificado SSL** (recomendado para HTTPS)

## 🔧 Instalación y Configuración

### 1. Clonar el Repositorio
```bash
git clone https://github.com/tu-usuario/sistema-asistencia.git
cd sistema-asistencia/SistemaAsistencia
```

### 2. Configuración Automática (Recomendado)
La aplicación se configura automáticamente al ejecutarse por primera vez:

```bash
dotnet run
```

**Configuración automática incluye:**
- 🔧 Aplicación de migraciones de base de datos
- 👤 Creación del usuario administrador (admin@sistema.com / admin123)
- 🏢 Configuración de empresa por defecto
- 📧 Configuración básica de email

### 3. Configuración Manual (Avanzada)

#### Base de Datos
Modifica `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=SistemaAsistenciaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  }
}
```

#### Email SMTP
Configura el email en **Configuración de Empresa**:
- **Servidor SMTP**: smtp.gmail.com (para Gmail)
- **Puerto**: 587
- **Usuario**: tu-email@gmail.com
- **Contraseña**: Contraseña de aplicación (para Gmail)

## 📱 Funcionalidades Detalladas

### 🔐 Sistema de Autenticación

#### Login por Email/Contraseña
![Login QR](https://cdn.discordapp.com/attachments/1424448965457477844/1424449011661803743/image.png?ex=68e3fd05&is=68e2ab85&hm=dc1efd41ee3e5544ff9f3156a413d3001c89bceec5ac7d238213784661429757&)

#### Login por Código QR
- Escaneo directo desde la interfaz web
- Códigos QR únicos por empleado
- Generación automática al crear usuarios

### 👥 Gestión de Usuarios

#### Crear Usuario
![Crear Usuario](https://cdn.discordapp.com/attachments/1424448965457477844/1424449319984955485/image.png?ex=68e3fd4e&is=68e2abce&hm=f1199cb7021fa9a1e5b4d862a5ca672b4cdd3eef224d894357baf269e2178631&)

#### Ver Código QR de Usuario
![Ver QR Usuario](https://cdn.discordapp.com/attachments/1424448965457477844/1424450036502106184/image.png?ex=68e3fdf9&is=68e2ac79&hm=fb317fc67481c84417226976e6a84c3d9bac9a0752f70f6428a31c285c961c65&)

### ⚙️ Configuración de Empresa

![Config Empresa](https://cdn.discordapp.com/attachments/1424448965457477844/1424449421063753849/image.png?ex=68e3fd66&is=68e2abe6&hm=510976e2dc43fcd031f271f427a8610b69055cb5775822456388ede03a5387c3&)

**Configuraciones disponibles:**
- **Horarios**: Hora de entrada y salida
- **Tolerancias**: Llegadas tardías y salidas tempranas
- **Días de trabajo**: Configuración de días laborales
- **Email SMTP**: Configuración completa de servidor de correo
- **Plantillas**: Personalización de emails de bienvenida y asistencia

### 📊 Reportes Avanzados

#### Reporte Mensual General
![Reporte Mensual](https://cdn.discordapp.com/attachments/1424448965457477844/1424449576252997832/image.png?ex=68e3fd8b&is=68e2ac0b&hm=d91ec8240b5a4a0e2664566396b20c16482f9993690511773e0ad0217d6495d0&)

**Características:**
- **Paginación**: Navegación por páginas (5, 10, 20, 50 empleados por página)
- **Búsqueda**: Filtro por nombre de empleado en tiempo real
- **Vista de calendario**: Estado visual de cada día del mes
- **Códigos de color**:
  - 🟢 Verde: Asistencia normal
  - 🟡 Amarillo: Llegada tardía
  - 🔴 Rojo: Salida temprana
  - ⚫ Negro: Ausente
  - 🟠 Naranja: Registro incompleto

### ✏️ Registro Manual de Asistencia

![Registro Manual](https://cdn.discordapp.com/attachments/1424448965457477844/1424449763851370597/image.png?ex=68e3fdb8&is=68e2ac38&hm=f730fcaa58805a745941c3efb91bfebeafc4871524f2f60ef394420f8386457f&)

**Funcionalidades:**
- **Búsqueda inteligente**: Sistema de búsqueda en tiempo real
- **Selección rápida**: Dropdown con resultados filtrados
- **Validaciones**: Prevención de registros duplicados
- **Flexibilidad**: Registro de entrada, salida o ausencia

### 👤 Dashboard de Usuario

![Dashboard Usuario](https://cdn.discordapp.com/attachments/1424448965457477844/1424450302395809802/image.png?ex=68e3fe39&is=68e2acb9&hm=a4c170277a82e670ca4646e88342091048f51b9d8fd2a78a22721a24da1b8b58&)

### 👤 Perfil de Usuario

![Perfil Usuario](https://cdn.discordapp.com/attachments/1424448965457477844/1424450402824224959/image.png?ex=68e3fe51&is=68e2acd1&hm=0b5c93e8b6db0067c1a08c77718bc9407670ca6dd5788a6a3c6d3a963f8bbab0&)

### 📊 Mi Asistencia

![Mi Asistencia](https://cdn.discordapp.com/attachments/1424448965457477844/1424450456473567292/image.png?ex=68e3fe5d&is=68e2acdd&hm=f5b64cea11e1335726427d42ea6170394c0561987ab6be849f3b8d52ec04cd94&)

## 📧 Sistema de Notificaciones por Email

### Configuración SMTP
El sistema incluye configuración completa para:
- **Gmail**: Con contraseñas de aplicación
- **Outlook**: Configuración estándar
- **Servidores personalizados**: Cualquier servidor SMTP

### Plantillas de Email

#### Email de Bienvenida
![Email Bienvenida](https://cdn.discordapp.com/attachments/1424448965457477844/1424451741658644532/image.png?ex=68e3ff90&is=68e2ae10&hm=259a46beb5b0f382fe694c92847f8ed53de5cd1040337aac4fb45f399a323fe0&)

**Variables disponibles:**
- `{CompanyName}` - Nombre de la empresa
- `{UserName}` - Nombre del usuario
- `{QrCode}` - Código QR del usuario
- `{SystemUrl}` - URL del sistema

#### Email de Confirmación de Asistencia
![Email Asistencia](https://cdn.discordapp.com/attachments/1424448965457477844/1424451842850684978/image.png?ex=68e3ffa8&is=68e2ae28&hm=03340d46a93457a5b390e7e69421804cdaba5ea7e1c95b371aad68373b8c6402&)

**Variables disponibles:**
- `{CompanyName}` - Nombre de la empresa
- `{UserName}` - Nombre del usuario
- `{RecordType}` - Tipo de registro (Entrada/Salida)
- `{Date}` - Fecha del registro
- `{Time}` - Hora del registro
- `{Status}` - Estado del registro

## 🗄️ Estructura de la Base de Datos

### Tabla Users
```sql
- Id (int, Primary Key)
- Email (string, Required, EmailAddress, Unique)
- Password (string, Required, DataType.Password)
- Name (string, Required, Display Name)
- Phone (string, nullable)
- Address (string, nullable)
- BaseSalary (decimal, nullable)
- HireDate (DateTime?, nullable)
- IsAdmin (bool, Display Name)
- IsActive (bool, Default: true)
- QrCode (string, nullable)
```

### Tabla AttendanceRecords
```sql
- Id (int, Primary Key)
- UserId (int, Foreign Key)
- Timestamp (DateTime, Display Name)
- RecordType (string, Display Name) -- "Entrada" o "Salida"
- User (virtual navigation property)
```

### Tabla CompanySettings
```sql
- Id (int, Primary Key)
- CompanyName (string, Required, Display Name)
- StartTime (TimeSpan, DataType.Time, Default: 8:00 AM)
- EndTime (TimeSpan, DataType.Time, Default: 5:00 PM)
- LateArrivalTolerance (int, Range 0-60, Default: 15)
- EarlyDepartureTolerance (int, Range 0-60, Default: 15)
- WorkingDays (string, Default: "Lunes,Martes,Miércoles,Jueves,Viernes")
- IsActive (bool, Default: true)
- CreatedAt (DateTime, Default: DateTime.Now)
- UpdatedAt (DateTime, Default: DateTime.Now)

-- Configuración de Email
- SmtpServer (string, nullable)
- SmtpPort (int, Default: 587)
- EmailUsername (string, nullable)
- EmailPassword (string, nullable, DataType.Password)
- FromEmail (string, nullable, EmailAddress)
- FromName (string, Default: "Sistema de Asistencia")
- EmailEnabled (bool, Default: false)

-- Plantillas de Email
- WelcomeEmailTemplate (string, nullable, DataType.MultilineText)
- AttendanceEmailTemplate (string, nullable, DataType.MultilineText)
```

## 🔒 Seguridad y Validaciones

### Autenticación
- **Sistema de sesiones** con timeout de 20 minutos
- **Tokens CSRF** en todos los formularios
- **Validación de roles** (empleado/administrador)
- **Prevención de ataques** de falsificación

### Validaciones de Negocio
- **Registros duplicados**: Prevención de múltiples entradas/salidas el mismo día
- **Horarios válidos**: Validación de horarios de entrada y salida
- **Usuarios activos**: Solo usuarios activos pueden registrar asistencia
- **Fechas futuras**: Prevención de registros en fechas futuras

### Seguridad de Email
- **Contraseñas de aplicación**: Soporte para Gmail App Passwords
- **Conexiones seguras**: SMTP con TLS/SSL
- **Validación de configuración**: Prueba de conexión antes de guardar

## 🚀 Despliegue

### Desarrollo Local
```bash
# Clonar repositorio
git clone https://github.com/tu-usuario/sistema-asistencia.git
cd sistema-asistencia/SistemaAsistencia

# Ejecutar aplicación
dotnet run
```

### Producción
1. **Configurar base de datos**:
   ```bash
   dotnet ef database update
   ```

2. **Configurar variables de entorno**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=PROD_SERVER;Database=SistemaAsistenciaDB;..."
     }
   }
   ```

3. **Configurar HTTPS** y certificados SSL

4. **Configurar email SMTP** desde la interfaz web

### Docker (Opcional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SistemaAsistencia.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaAsistencia.dll"]
```

## 📝 Guías de Configuración

### Configuración de Gmail
1. Habilitar **2FA** en tu cuenta de Gmail
2. Generar una **contraseña de aplicación**
3. Usar la contraseña de aplicación en lugar de tu contraseña normal
4. Configurar en **Configuración de Empresa** → **Email**

### Configuración de Horarios
1. Ve a **Configuración de Empresa**
2. Establece **Hora de Entrada** y **Hora de Salida**
3. Configura **Tolerancias** para llegadas tardías y salidas tempranas
4. Selecciona **Días de Trabajo**

## 🔄 Historial de Versiones

### v3.0.0 (2024-12-XX) - Sistema Completo con Email
- ✨ **NUEVO**: Sistema completo de notificaciones por email
- ✨ **NUEVO**: Plantillas de email personalizables
- ✨ **NUEVO**: Configuración SMTP avanzada
- ✨ **NUEVO**: Reportes con paginación y búsqueda
- ✨ **NUEVO**: Búsqueda inteligente de empleados
- ✨ **NUEVO**: Perfiles detallados de usuario
- ✨ **NUEVO**: Lógica mejorada de reportes de ausencias
- 🔧 Mejoras en la interfaz de usuario
- 🔧 Optimizaciones de rendimiento

### v2.0.0 (2024-XX-XX) - Sistema con Códigos QR
- ✨ Sistema de códigos QR para acceso rápido
- ✨ Generación automática de códigos QR
- ✨ Login por código QR
- ✨ Usuario administrador automático
- ✨ Configuración de empresa personalizable

### v1.0.0 (2024-01-XX) - Versión Base
- ✅ Sistema básico de asistencia
- ✅ Gestión de usuarios
- ✅ Reportes básicos
- ✅ Interfaz responsive

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Para contribuir:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.
---


