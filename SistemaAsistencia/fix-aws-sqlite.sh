#!/bin/bash

# Script de solución rápida para AWS - SQLite
# Ejecutar este script en tu VM de AWS para solucionar el problema de migraciones

echo "🔧 Solucionando problema de migraciones SQLite en AWS..."

# Configurar entorno
export ASPNETCORE_ENVIRONMENT=Production

# Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 1. Limpiar archivos de migración problemáticos
echo "🧹 Limpiando migraciones de SQL Server..."
rm -rf Migrations/
mkdir Migrations

# 2. Crear base de datos SQLite manualmente
echo "🗄️ Creando base de datos SQLite..."
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# 3. Verificar estructura
echo "✅ Verificando estructura de base de datos..."
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"

# 4. Verificar datos
echo "👤 Verificando usuario administrador..."
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# 5. Limpiar y compilar
echo "🔨 Limpiando y compilando..."
dotnet clean
dotnet restore
dotnet build -c Release

# 6. Detener aplicación actual
echo "🛑 Deteniendo aplicación actual..."
pkill -f "dotnet.*SistemaAsistencia" || true
sleep 2

# 7. Iniciar aplicación
echo "▶️ Iniciando aplicación..."
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# 8. Verificar que funciona
sleep 5
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo "✅ ¡Problema solucionado!"
    echo "🌐 Aplicación ejecutándose en: http://localhost:5000"
    echo "🗄️ Base de datos: /var/www/demo/sistema_asistencia.db"
    echo "📊 Logs: /var/www/demo/sistema_asistencia.log"
else
    echo "❌ Error al iniciar aplicación"
    echo "📋 Revisar logs: tail -f /var/www/demo/sistema_asistencia.log"
fi

echo "🎉 ¡Listo! El sistema debería funcionar correctamente ahora."
