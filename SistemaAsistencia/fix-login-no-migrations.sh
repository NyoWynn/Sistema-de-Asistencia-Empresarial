#!/bin/bash

# Script de solución DEFINITIVA sin migraciones
# Sistema de Asistencia Empresarial

echo "🔧 SOLUCIÓN DEFINITIVA - Sin Migraciones"
echo "========================================"

# Variables
APP_PORT="5000"
LOG_FILE="/var/www/demo/sistema_asistencia.log"
DB_FILE="sistema_asistencia.db"

# 1. Detener todos los procesos
echo "🛑 Deteniendo todos los procesos..."
pkill -9 -f "dotnet" || true
sleep 5

# 2. Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 3. Actualizar código
echo "📥 Actualizando código..."
git pull origin main

# 4. Eliminar migraciones problemáticas
echo "🗑️ Eliminando migraciones problemáticas..."
rm -rf Migrations/

# 5. Limpiar completamente
echo "🧹 Limpieza completa..."
rm -rf bin/ obj/
dotnet clean --verbosity quiet

# 6. Eliminar base de datos anterior
echo "🗑️ Eliminando base de datos anterior..."
rm -f "$DB_FILE"
rm -f /var/www/demo/sistema_asistencia.db

# 7. Crear base de datos SQLite manualmente
echo "🗄️ Creando base de datos SQLite..."
sqlite3 "$DB_FILE" << 'EOF'
-- Crear tabla Users
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Email" TEXT NOT NULL UNIQUE,
    "Password" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Phone" TEXT NULL,
    "Address" TEXT NULL,
    "BaseSalary" REAL NULL,
    "HireDate" TEXT NULL,
    "IsAdmin" INTEGER NOT NULL DEFAULT 0,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "QrCode" TEXT NULL
);

-- Crear tabla AttendanceRecords
CREATE TABLE IF NOT EXISTS "AttendanceRecords" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "Timestamp" TEXT NOT NULL,
    "RecordType" TEXT NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Crear tabla CompanySettings
CREATE TABLE IF NOT EXISTS "CompanySettings" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "CompanyName" TEXT NOT NULL,
    "StartTime" TEXT NOT NULL,
    "EndTime" TEXT NOT NULL,
    "LateArrivalTolerance" INTEGER NOT NULL DEFAULT 15,
    "EarlyDepartureTolerance" INTEGER NOT NULL DEFAULT 15,
    "WorkingDays" TEXT NOT NULL DEFAULT 'Lunes,Martes,Miércoles,Jueves,Viernes',
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "SmtpServer" TEXT NULL,
    "SmtpPort" INTEGER NOT NULL DEFAULT 587,
    "EmailUsername" TEXT NULL,
    "EmailPassword" TEXT NULL,
    "FromEmail" TEXT NULL,
    "FromName" TEXT NULL DEFAULT 'Sistema de Asistencia',
    "EmailEnabled" INTEGER NOT NULL DEFAULT 0,
    "WelcomeEmailTemplate" TEXT NULL,
    "AttendanceEmailTemplate" TEXT NULL
);

-- Crear tabla OfficeLocations
CREATE TABLE IF NOT EXISTS "OfficeLocations" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Address" TEXT NULL,
    "Latitude" REAL NOT NULL,
    "Longitude" REAL NOT NULL,
    "RadiusMeters" INTEGER NOT NULL DEFAULT 100,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "Description" TEXT NULL
);

-- Crear índices
CREATE INDEX IF NOT EXISTS "IX_AttendanceRecords_UserId" ON "AttendanceRecords" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_AttendanceRecords_Timestamp" ON "AttendanceRecords" ("Timestamp");
CREATE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE INDEX IF NOT EXISTS "IX_Users_IsAdmin" ON "Users" ("IsAdmin");
CREATE INDEX IF NOT EXISTS "IX_OfficeLocations_IsActive" ON "OfficeLocations" ("IsActive");

-- Insertar datos iniciales
INSERT OR IGNORE INTO "CompanySettings" (
    "CompanyName", "StartTime", "EndTime", "LateArrivalTolerance", 
    "EarlyDepartureTolerance", "WorkingDays", "IsActive", "CreatedAt", "UpdatedAt"
) VALUES (
    'Mi Empresa', '08:00:00', '17:00:00', 15, 15, 
    'Lunes,Martes,Miércoles,Jueves,Viernes', 1, 
    datetime('now'), datetime('now')
);

INSERT OR IGNORE INTO "Users" (
    "Name", "Email", "Password", "IsAdmin", "IsActive"
) VALUES (
    'Administrador', 'admin@sistema.com', 'admin123', 1, 1
);
EOF

# 8. Verificar base de datos
echo "✅ Verificando base de datos..."
sqlite3 "$DB_FILE" ".tables"
sqlite3 "$DB_FILE" "SELECT Id, Email, IsAdmin FROM Users;"
sqlite3 "$DB_FILE" "SELECT Id, CompanyName, IsActive FROM CompanySettings;"

# 9. Restaurar y compilar
echo "📦 Restaurando dependencias..."
dotnet restore --verbosity quiet

echo "🔨 Compilando..."
dotnet build -c Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Error en la compilación"
    exit 1
fi

# 10. Configurar entorno
echo "🔧 Configurando entorno..."
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://0.0.0.0:$APP_PORT

# 11. Iniciar aplicación
echo "▶️ Iniciando aplicación..."
nohup dotnet run --configuration Release > "$LOG_FILE" 2>&1 &
APP_PID=$!

echo "PID de la aplicación: $APP_PID"

# 12. Esperar y verificar
echo "⏳ Esperando 20 segundos para que inicie..."
sleep 20

# 13. Verificar que esté ejecutándose
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo "✅ Aplicación iniciada correctamente"
    
    # Verificar puerto
    if ss -tlnp | grep ":$APP_PORT" > /dev/null; then
        echo "✅ Puerto $APP_PORT activo"
    else
        echo "⚠️ Puerto $APP_PORT no detectado, pero proceso ejecutándose"
    fi
    
    # Mostrar información
    echo ""
    echo "🎉 ¡ÉXITO! Sistema funcionando perfectamente"
    echo "============================================"
    echo "🌐 URL local: http://localhost:$APP_PORT"
    echo "🌐 URL pública: http://18.216.129.127:$APP_PORT"
    echo "📊 Logs: $LOG_FILE"
    echo "🗄️ Base de datos: $DB_FILE"
    echo ""
    echo "👤 Usuario administrador:"
    echo "   Email: admin@sistema.com"
    echo "   Password: admin123"
    echo ""
    echo "🔍 Procesos:"
    ps aux | grep dotnet | grep -v grep
    
    echo ""
    echo "🔍 Puertos:"
    ss -tlnp | grep -E ':(5000|5183)'
    
    echo ""
    echo "📋 Últimas líneas del log:"
    tail -20 "$LOG_FILE"
    
    echo ""
    echo "🧪 PRUEBA DE LOGIN:"
    echo "Ejecutando prueba de login..."
    curl -X POST http://localhost:5000 \
      -H "Content-Type: application/x-www-form-urlencoded" \
      -d "email=admin@sistema.com&password=admin123" \
      -c cookies.txt -b cookies.txt -L -s | head -10
    
else
    echo "❌ Error: La aplicación no se inició correctamente"
    echo "📋 Log completo:"
    cat "$LOG_FILE"
    exit 1
fi

echo ""
echo "✅ Solución definitiva completada"
