#!/bin/bash

# Script de despliegue SIMPLE para SQLite
# Sistema de Asistencia Empresarial

echo "🚀 DESPLIEGUE SIMPLE - Sistema de Asistencia SQLite"
echo "=================================================="

# Variables
APP_PORT="5000"
LOG_FILE="/var/www/demo/sistema_asistencia.log"

# 1. Detener procesos existentes
echo "🛑 Deteniendo procesos existentes..."
pkill -f "dotnet.*SistemaAsistencia" || true
sleep 3

# 2. Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 3. Limpiar y compilar
echo "🔨 Limpiando y compilando..."
dotnet clean --verbosity quiet
dotnet restore --verbosity quiet
dotnet build -c Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Error en la compilación"
    exit 1
fi

# 4. Configurar entorno
echo "🔧 Configurando entorno..."
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:$APP_PORT

# 5. Iniciar aplicación
echo "▶️ Iniciando aplicación en puerto $APP_PORT..."
nohup dotnet run --configuration Release > "$LOG_FILE" 2>&1 &
APP_PID=$!

echo "PID de la aplicación: $APP_PID"

# 6. Esperar y verificar
echo "⏳ Esperando 15 segundos para que inicie..."
sleep 15

# 7. Verificar que esté ejecutándose
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
echo "✅ Despliegue completado exitosamente"
