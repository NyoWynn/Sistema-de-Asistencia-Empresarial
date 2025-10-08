#!/bin/bash

# Script de solución DEFINITIVA para producción
# Soluciona problemas de SQL Server y puertos

echo "🚀 SOLUCIÓN DEFINITIVA - Producción AWS"
echo "======================================="

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

# 5. VERIFICAR Y CORREGIR Program.cs
echo "🔧 Verificando Program.cs..."
if grep -q "if (builder.Environment.IsDevelopment())" Program.cs; then
    echo "✅ Program.cs tiene la condición correcta"
else
    echo "❌ Program.cs NO tiene la condición correcta"
    echo "📋 Contenido actual de las líneas 40-50:"
    sed -n '40,50p' Program.cs
    exit 1
fi

# 6. LIMPIAR COMPLETAMENTE
echo "🧹 Limpieza completa..."
rm -rf bin/ obj/ Migrations/
dotnet clean --verbosity quiet

# 7. CREAR BASE DE DATOS SQLITE
echo "🗄️ Creando base de datos SQLite..."
sudo mkdir -p /var/www/demo
sudo chown ubuntu:ubuntu /var/www/demo
rm -f /var/www/demo/sistema_asistencia.db
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# 8. VERIFICAR BD
echo "✅ Verificando base de datos..."
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# 9. RESTAURAR Y COMPILAR
echo "📦 Restaurando dependencias..."
dotnet restore --verbosity quiet

echo "🔨 Compilando..."
dotnet build -c Release --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Error en compilación"
    exit 1
fi

# 10. CONFIGURAR ENTORNO PRODUCCIÓN
echo "🔧 Configurando entorno de producción..."
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

echo "Variables de entorno configuradas:"
echo "  ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "  ASPNETCORE_URLS=$ASPNETCORE_URLS"

# 11. INICIAR APLICACIÓN
echo "▶️ Iniciando aplicación en puerto 5000..."
echo "Ejecutando: dotnet run --configuration Release"

# Iniciar en segundo plano
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &
APP_PID=$!

echo "PID de la aplicación: $APP_PID"

# 12. ESPERAR Y VERIFICAR
echo "⏳ Esperando 20 segundos para que inicie completamente..."
sleep 20

# 13. VERIFICAR PROCESO
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo "✅ Aplicación iniciada correctamente"
else
    echo "❌ Error: La aplicación no se inició"
    echo "📋 Log completo:"
    cat /var/www/demo/sistema_asistencia.log
    exit 1
fi

# 14. VERIFICAR PUERTO 5000
if ss -tlnp | grep ":5000" > /dev/null; then
    echo "✅ Puerto 5000 activo"
else
    echo "⚠️ Puerto 5000 no detectado, verificando logs..."
    tail -20 /var/www/demo/sistema_asistencia.log
fi

# 15. VERIFICAR PUERTO 5183 (no debería estar activo)
if ss -tlnp | grep ":5183" > /dev/null; then
    echo "⚠️ Puerto 5183 aún activo, matando proceso..."
    lsof -ti:5183 | xargs kill -9 || true
    sleep 2
fi

# 16. MOSTRAR ESTADO FINAL
echo ""
echo "🎉 ¡SISTEMA FUNCIONANDO EN PRODUCCIÓN!"
echo "======================================"
echo "🌐 URL local: http://localhost:5000"
echo "🌐 URL externa: http://0.0.0.0:5000"
echo "📊 Logs: /var/www/demo/sistema_asistencia.log"
echo ""
echo "👤 Usuario administrador:"
echo "   Email: admin@sistema.com"
echo "   Password: admin123"
echo ""
echo "🔍 Procesos activos:"
ps aux | grep dotnet | grep -v grep
echo ""
echo "🔍 Puertos activos:"
ss -tlnp | grep -E ':(5000|5183)' || echo "Solo puerto 5000 activo (correcto)"
echo ""
echo "📋 Últimas líneas del log:"
tail -10 /var/www/demo/sistema_asistencia.log
echo ""
echo "✅ Sistema listo para configuración de Nginx"
