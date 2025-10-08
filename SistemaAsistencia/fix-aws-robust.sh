#!/bin/bash

# Script ROBUSTO para AWS - SQLite
# Maneja timeouts y procesos colgados

echo "🔧 SOLUCIÓN ROBUSTA - Sistema de Asistencia AWS SQLite"
echo "======================================================"

# Configurar entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

# Función para matar procesos colgados
kill_hanging_processes() {
    echo "🔄 Matando procesos colgados..."
    pkill -9 -f "dotnet.*SistemaAsistencia" || true
    pkill -9 -f "dotnet.*build" || true
    pkill -9 -f "dotnet.*restore" || true
    sleep 2
}

# Función para compilar con timeout
compile_with_timeout() {
    echo "🔨 Compilando con timeout de 60 segundos..."
    timeout 60s dotnet build -c Release --no-restore
    return $?
}

# 1. Detener servicios
echo "🛑 Deteniendo servicios..."
sudo systemctl stop asistencia || true
kill_hanging_processes

# 2. Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 3. Limpiar completamente
echo "🧹 Limpiando archivos temporales..."
rm -rf bin/ obj/ Migrations/
dotnet clean --verbosity quiet

# 4. Crear directorio de datos
echo "📁 Creando directorio de datos..."
sudo mkdir -p /var/www/demo
sudo chown ubuntu:ubuntu /var/www/demo

# 5. Crear base de datos SQLite manualmente
echo "🗄️ Creando base de datos SQLite..."
rm -f /var/www/demo/sistema_asistencia.db
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# 6. Verificar estructura
echo "✅ Verificando estructura de base de datos..."
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"

# 7. Verificar datos iniciales
echo "👤 Verificando usuario administrador..."
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# 8. Verificar configuración de empresa
echo "🏢 Verificando configuración de empresa..."
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, CompanyName, IsActive FROM CompanySettings;"

# 9. Restaurar dependencias con timeout
echo "📦 Restaurando dependencias (timeout 120s)..."
timeout 120s dotnet restore --verbosity quiet
if [ $? -ne 0 ]; then
    echo "❌ Error en restore. Intentando sin cache..."
    rm -rf ~/.nuget/packages/SistemaAsistencia* || true
    timeout 120s dotnet restore --no-cache --verbosity quiet
fi

# 10. Compilar con timeout
echo "🔨 Compilando aplicación..."
compile_with_timeout
if [ $? -ne 0 ]; then
    echo "❌ Error en compilación. Intentando limpiar y recompilar..."
    kill_hanging_processes
    rm -rf bin/ obj/
    dotnet clean --verbosity quiet
    timeout 60s dotnet restore --verbosity quiet
    compile_with_timeout
fi

# 11. Verificar que no hay errores de compilación
if [ $? -ne 0 ]; then
    echo "❌ Error persistente en la compilación."
    echo "📋 Intentando compilación manual..."
    kill_hanging_processes
    dotnet build -c Release --verbosity normal
    exit 1
fi

# 12. Iniciar aplicación
echo "▶️ Iniciando aplicación..."
kill_hanging_processes
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# 13. Esperar y verificar
sleep 8

if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo ""
    echo "🎉 ¡ÉXITO! Sistema funcionando correctamente"
    echo "=========================================="
    echo "🌐 Aplicación: http://localhost:5000"
    echo "🗄️ Base de datos: /var/www/demo/sistema_asistencia.db"
    echo "📊 Logs: /var/www/demo/sistema_asistencia.log"
    echo ""
    echo "👤 Usuario administrador:"
    echo "   Email: admin@sistema.com"
    echo "   Password: admin123"
    echo ""
    echo "📋 Comandos útiles:"
    echo "   Ver logs: tail -f /var/www/demo/sistema_asistencia.log"
    echo "   Verificar BD: sqlite3 /var/www/demo/sistema_asistencia.db '.tables'"
    echo "   Detener: pkill -f 'dotnet.*SistemaAsistencia'"
    echo "   Reiniciar: ./fix-aws-robust.sh"
else
    echo "❌ Error al iniciar la aplicación"
    echo "📋 Revisar logs: tail -f /var/www/demo/sistema_asistencia.log"
    echo "📋 Últimas líneas del log:"
    tail -20 /var/www/demo/sistema_asistencia.log
    exit 1
fi

echo ""
echo "✅ ¡Problema solucionado completamente!"
