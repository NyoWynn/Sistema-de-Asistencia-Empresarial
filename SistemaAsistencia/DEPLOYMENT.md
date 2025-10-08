# 🚀 Guía de Despliegue - Sistema de Asistencia Empresarial

## 📋 Configuración por Entornos

El sistema ahora soporta múltiples entornos con diferentes configuraciones de base de datos:

### 🏠 Desarrollo Local (SQL Server)
- **Entorno**: `Development`
- **Base de datos**: SQL Server LocalDB
- **Archivo de configuración**: `appsettings.Development.json`

### ☁️ Producción AWS (SQLite)
- **Entorno**: `Production`
- **Base de datos**: SQLite
- **Archivo de configuración**: `appsettings.Production.json`

## 🔧 Configuración Automática

El sistema detecta automáticamente el entorno y configura la base de datos correspondiente:

```csharp
// En Program.cs
if (builder.Environment.IsDevelopment())
{
    // SQL Server para desarrollo
    options.UseSqlServer(connectionString);
}
else
{
    // SQLite para producción (AWS)
    options.UseSqlite(connectionString);
}
```

## 📦 Despliegue en AWS

### 1. Clonar el Repositorio
```bash
git clone https://github.com/NyoWynn/Sistema-de-Asistencia-Empresarial.git
cd Sistema-de-Asistencia-Empresarial/SistemaAsistencia
```

### 2. Solución Rápida (Recomendado)
```bash
# Hacer el script ejecutable
chmod +x fix-aws-sqlite.sh

# Ejecutar solución automática
./fix-aws-sqlite.sh
```

### 3. Despliegue Manual (Alternativo)
```bash
# Configurar variables de entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

# Crear directorio de datos
mkdir -p /var/www/demo

# Crear base de datos SQLite manualmente
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# Restaurar dependencias y compilar
dotnet restore
dotnet build -c Release

# Ejecutar aplicación
dotnet run --configuration Release
```

### 4. Verificación
```bash
# Verificar tablas creadas
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"

# Verificar usuario administrador
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# Verificar aplicación ejecutándose
curl http://localhost:5000
```

## 🔄 Actualizaciones sin Perder Datos

### Para Actualizar el Código:
```bash
# 1. Hacer backup de la base de datos
cp sistema_asistencia.db sistema_asistencia_backup.db

# 2. Actualizar código
git pull origin main

# 3. Restaurar dependencias
dotnet restore

# 4. Ejecutar migraciones (si hay cambios en BD)
dotnet ef database update

# 5. Reiniciar aplicación
dotnet run
```

### Para Cambios de Código (sin cambios en BD):
```bash
# 1. Actualizar código
git pull origin main

# 2. Restaurar dependencias
dotnet restore

# 3. Reiniciar aplicación
dotnet run
```

## 📱 Mejoras de Responsividad

### Próximas Mejoras Incluidas:
- ✅ **Navegación móvil mejorada**
- ✅ **Tablas responsivas con scroll horizontal**
- ✅ **Formularios optimizados para móviles**
- ✅ **Botones y elementos táctiles más grandes**
- ✅ **Vista de reportes adaptada a pantallas pequeñas**

## 🔧 Configuración de Email en Producción

Edita `appsettings.Production.json`:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Sistema de Asistencia"
  }
}
```

## 🛡️ Seguridad en Producción

### Variables de Entorno Recomendadas:
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000
export ASPNETCORE_HTTPS_PORT=5001
```

### Configuración de Firewall:
- Puerto 5000 para HTTP
- Puerto 5001 para HTTPS (si se configura SSL)

## 📊 Monitoreo

### Logs de la Aplicación:
```bash
# Ver logs en tiempo real
dotnet run --verbosity normal

# Logs específicos de errores
grep "Error" sistema_asistencia.log
```

## 🔄 Backup y Restauración

### Backup de Base de Datos:
```bash
# Crear backup
cp sistema_asistencia.db backup_$(date +%Y%m%d_%H%M%S).db

# Restaurar backup
cp backup_20250105_143022.db sistema_asistencia.db
```

## 🚨 Solución de Problemas

### Error de Conexión a Base de Datos:
1. Verificar que el archivo `sistema_asistencia.db` existe
2. Verificar permisos de escritura en el directorio
3. Revisar logs de la aplicación

### Error de Migraciones:
```bash
# Recrear base de datos
rm sistema_asistencia.db
dotnet ef database update
```

### Error de Dependencias:
```bash
# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

## 📞 Soporte

Para problemas específicos de despliegue:
- 📧 Email: soporte@sistemaasistencia.com
- 📱 Issues: [GitHub Issues](https://github.com/NyoWynn/Sistema-de-Asistencia-Empresarial/issues)

---

*Sistema de Asistencia Empresarial - Despliegue en AWS con SQLite*
