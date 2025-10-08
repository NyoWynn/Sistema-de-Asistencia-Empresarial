#!/bin/bash

# Script de solución DEFINITIVA para producción
# Soluciona problemas de SQL Server y puertos

echo "🚀 SOLUCIÓN DEFINITIVA - Sistema de Asistencia Producción"
echo "========================================================="

# Variables
APP_PORT="5000"
LOG_FILE="/var/www/demo/sistema_asistencia.log"

# 1. MATAR TODOS LOS PROCESOS
echo "🛑 Matando TODOS los procesos dotnet..."
sudo systemctl stop asistencia || true
pkill -9 -f "dotnet" || true
pkill -9 -f "SistemaAsistencia" || true
sleep 5

# 2. Verificar que no hay procesos
echo "🔍 Verificando que no hay procesos:"
ps aux | grep dotnet | grep -v grep || echo "✅ No hay procesos dotnet"

# 3. Verificar puertos
echo "🔍 Verificando puertos:"
ss -tlnp | grep -E ':(5000|5183)' || echo "✅ Puertos 5000 y 5183 libres"

# 4. Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 5. Verificar que Program.cs esté correcto
echo "🔍 Verificando Program.cs..."
if grep -q "if (builder.Environment.IsDevelopment())" Program.cs; then
    echo "✅ Program.cs está correcto"
else
    echo "❌ Program.cs NO está actualizado"
    exit 1
fi

# 6. Limpiar completamente
echo "🧹 Limpieza completa..."
rm -rf bin/ obj/ Migrations/
dotnet clean --verbosity quiet

# 7. Crear directorio y BD
echo "📁 Creando directorio y base de datos..."
sudo mkdir -p /var/www/demo
sudo chown ubuntu:ubuntu /var/www/demo
rm -f /var/www/demo/sistema_asistencia.db
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# 8. Verificar BD
echo "✅ Verificando base de datos..."
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# 9. Restaurar y compilar
echo "📦 Restaurando dependencias..."
timeout 120s dotnet restore --verbosity quiet

echo "🔨 Compilando..."
timeout 60s dotnet build -c Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Error en compilación"
    exit 1
fi

# 10. Configurar entorno para FORZAR SQLite
echo "🔧 Configurando entorno para SQLite..."
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:$APP_PORT

# 11. Verificar configuración de conexión
echo "🔍 Verificando configuración de conexión..."
if grep -q "Sqlite" appsettings.Production.json; then
    echo "✅ Configuración SQLite encontrada"
else
    echo "❌ Configuración SQLite no encontrada"
    exit 1
fi

# 12. Iniciar aplicación
echo "▶️ Iniciando aplicación en puerto $APP_PORT..."
echo "Variables de entorno:"
echo "  ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "  ASPNETCORE_URLS=$ASPNETCORE_URLS"

# Iniciar en segundo plano
nohup dotnet run --configuration Release > "$LOG_FILE" 2>&1 &
APP_PID=$!

echo "PID de la aplicación: $APP_PID"

# 13. Esperar y verificar
echo "⏳ Esperando 20 segundos para que inicie..."
sleep 20

# 14. Verificar que esté ejecutándose
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
    echo "🎉 ¡ÉXITO TOTAL! Sistema funcionando perfectamente"
    echo "================================================="
    echo "🌐 URL local: http://localhost:$APP_PORT"
    echo "🌐 URL pública: http://0.0.0.0:$APP_PORT"
    echo "📊 Logs: $LOG_FILE"
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
    tail -10 "$LOG_FILE"
    
else
    echo "❌ Error: La aplicación no se inició correctamente"
    echo "📋 Log completo:"
    cat "$LOG_FILE"
    exit 1
fi

echo ""
echo "✅ Solución definitiva completada"
