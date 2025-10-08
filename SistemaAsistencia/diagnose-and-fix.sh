#!/bin/bash

# Script de diagnóstico y solución para problemas de inicio
# Sistema de Asistencia Empresarial

echo "🔍 DIAGNÓSTICO Y SOLUCIÓN - Sistema de Asistencia"
echo "================================================="

# Variables
APP_PORT="5000"
LOG_FILE="/var/www/demo/sistema_asistencia.log"

# 1. Verificar estado actual
echo "📊 Estado actual del sistema:"
echo "============================="

# Verificar procesos dotnet
echo "🔍 Procesos dotnet activos:"
ps aux | grep dotnet | grep -v grep || echo "No hay procesos dotnet activos"

# Verificar puertos
echo ""
echo "🔍 Puertos en uso:"
ss -tlnp | grep -E ':(5000|5183)' || echo "Puertos 5000 y 5183 libres"

# Verificar logs
echo ""
echo "📋 Últimas líneas del log:"
if [ -f "$LOG_FILE" ]; then
    tail -20 "$LOG_FILE"
else
    echo "Archivo de log no encontrado: $LOG_FILE"
fi

# 2. Limpiar procesos colgados
echo ""
echo "🧹 Limpiando procesos colgados..."
pkill -9 -f "dotnet.*SistemaAsistencia" || true
pkill -9 -f "dotnet.*run" || true
sleep 3

# 3. Verificar que no hay procesos
echo "🔍 Verificando que no hay procesos:"
ps aux | grep dotnet | grep -v grep || echo "✅ No hay procesos dotnet"

# 4. Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 5. Verificar que el proyecto existe
if [ ! -f "SistemaAsistencia.csproj" ]; then
    echo "❌ Error: No se encuentra el archivo del proyecto"
    exit 1
fi

# 6. Configurar entorno
echo "🔧 Configurando entorno..."
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:$APP_PORT

# 7. Verificar base de datos
echo "🗄️ Verificando base de datos..."
if [ -f "/var/www/demo/sistema_asistencia.db" ]; then
    echo "✅ Base de datos encontrada"
    sqlite3 /var/www/demo/sistema_asistencia.db "SELECT COUNT(*) as 'Total Users' FROM Users;"
else
    echo "❌ Base de datos no encontrada, creando..."
    sudo mkdir -p /var/www/demo
    sudo chown ubuntu:ubuntu /var/www/demo
    sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql
fi

# 8. Limpiar y compilar
echo "🔨 Limpiando y compilando..."
dotnet clean --verbosity quiet
dotnet restore --verbosity quiet
dotnet build -c Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Error en la compilación"
    exit 1
fi

# 9. Iniciar aplicación con timeout
echo "▶️ Iniciando aplicación en puerto $APP_PORT..."
echo "Ejecutando: dotnet run --configuration Release"
echo "Variables de entorno:"
echo "  ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "  ASPNETCORE_URLS=$ASPNETCORE_URLS"

# Iniciar en segundo plano
nohup dotnet run --configuration Release > "$LOG_FILE" 2>&1 &
APP_PID=$!

echo "PID de la aplicación: $APP_PID"

# 10. Esperar y verificar
echo "⏳ Esperando 15 segundos para que inicie..."
sleep 15

# 11. Verificar que esté ejecutándose
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
    echo "📊 Información del sistema:"
    echo "=========================="
    echo "🌐 URL local: http://localhost:$APP_PORT"
    echo "📊 Logs: $LOG_FILE"
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
echo "✅ Diagnóstico completado"
