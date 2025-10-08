#!/bin/bash

# Script ULTIMATE para AWS - Solución definitiva
# Fuerza actualización completa y soluciona todos los problemas

echo "🚀 SOLUCIÓN ULTIMATE - Sistema de Asistencia AWS SQLite"
echo "======================================================"

# Configurar entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

# 1. MATAR TODOS LOS PROCESOS
echo "🛑 Matando TODOS los procesos dotnet..."
sudo systemctl stop asistencia || true
pkill -9 -f "dotnet" || true
pkill -9 -f "SistemaAsistencia" || true
sleep 5

# 2. FORZAR ACTUALIZACIÓN COMPLETA
echo "🔄 Forzando actualización completa del código..."
cd ~/Sistema-de-Asistencia-Empresarial
git fetch --all
git reset --hard origin/main
git clean -fd

# 3. Ir al proyecto
cd SistemaAsistencia

# 4. VERIFICAR QUE Program.cs ESTÉ CORRECTO
echo "🔍 Verificando Program.cs..."
if grep -q "if (builder.Environment.IsDevelopment())" Program.cs; then
    echo "✅ Program.cs está correcto"
else
    echo "❌ Program.cs NO está actualizado"
    echo "📋 Contenido actual:"
    head -50 Program.cs
    exit 1
fi

# 5. LIMPIAR COMPLETAMENTE
echo "🧹 Limpieza completa..."
rm -rf bin/ obj/ Migrations/
dotnet clean --verbosity quiet

# 6. CREAR DIRECTORIO Y BD
echo "📁 Creando directorio y base de datos..."
sudo mkdir -p /var/www/demo
sudo chown ubuntu:ubuntu /var/www/demo
rm -f /var/www/demo/sistema_asistencia.db
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# 7. VERIFICAR BD
echo "✅ Verificando base de datos..."
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# 8. RESTAURAR Y COMPILAR
echo "📦 Restaurando dependencias..."
timeout 120s dotnet restore --verbosity quiet

echo "🔨 Compilando..."
timeout 60s dotnet build -c Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Error en compilación"
    exit 1
fi

# 9. VERIFICAR PUERTOS
echo "🔍 Verificando puertos..."
PORT_5183=$(lsof -ti:5183 || true)
PORT_5000=$(lsof -ti:5000 || true)

if [ ! -z "$PORT_5183" ]; then
    echo "🛑 Matando proceso en puerto 5183: $PORT_5183"
    kill -9 $PORT_5183 || true
fi

if [ ! -z "$PORT_5000" ]; then
    echo "🛑 Matando proceso en puerto 5000: $PORT_5000"
    kill -9 $PORT_5000 || true
fi

sleep 2

# 10. INICIAR APLICACIÓN
echo "▶️ Iniciando aplicación..."
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# 11. ESPERAR Y VERIFICAR
echo "⏳ Esperando 10 segundos..."
sleep 10

# 12. VERIFICAR PROCESO
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo ""
    echo "🎉 ¡ÉXITO TOTAL! Sistema funcionando perfectamente"
    echo "================================================="
    echo "🌐 Aplicación: http://localhost:5000"
    echo "🗄️ Base de datos: /var/www/demo/sistema_asistencia.db"
    echo "📊 Logs: /var/www/demo/sistema_asistencia.log"
    echo ""
    echo "👤 Usuario administrador:"
    echo "   Email: admin@sistema.com"
    echo "   Password: admin123"
    echo ""
    echo "🔍 Verificación de procesos:"
    ps aux | grep dotnet | grep -v grep
    echo ""
    echo "🔍 Verificación de puertos:"
    netstat -tlnp | grep :5000 || echo "Puerto 5000 no encontrado"
    echo ""
    echo "📋 Comandos útiles:"
    echo "   Ver logs: tail -f /var/www/demo/sistema_asistencia.log"
    echo "   Verificar BD: sqlite3 /var/www/demo/sistema_asistencia.db '.tables'"
    echo "   Detener: pkill -f 'dotnet.*SistemaAsistencia'"
else
    echo "❌ Error al iniciar aplicación"
    echo "📋 Últimas líneas del log:"
    tail -30 /var/www/demo/sistema_asistencia.log
    echo ""
    echo "🔍 Procesos dotnet activos:"
    ps aux | grep dotnet | grep -v grep || echo "No hay procesos dotnet"
    echo ""
    echo "🔍 Puertos en uso:"
    netstat -tlnp | grep -E ':(5000|5183)' || echo "Puertos 5000 y 5183 libres"
fi

echo ""
echo "✅ Script ultimate completado"
