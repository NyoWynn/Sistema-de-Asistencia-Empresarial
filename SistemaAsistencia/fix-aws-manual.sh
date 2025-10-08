#!/bin/bash

# Script MANUAL paso a paso para AWS - SQLite
# Para cuando los scripts automáticos fallan

echo "🔧 SOLUCIÓN MANUAL - Sistema de Asistencia AWS SQLite"
echo "===================================================="

# Configurar entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

echo "📋 Ejecutando pasos manuales..."
echo ""

# 1. Detener procesos
echo "1️⃣ Deteniendo procesos..."
sudo systemctl stop asistencia || true
pkill -9 -f "dotnet.*SistemaAsistencia" || true
pkill -9 -f "dotnet.*build" || true
pkill -9 -f "dotnet.*restore" || true
sleep 3

# 2. Ir al directorio
echo "2️⃣ Yendo al directorio del proyecto..."
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 3. Limpiar
echo "3️⃣ Limpiando archivos..."
rm -rf bin/ obj/ Migrations/
dotnet clean

# 4. Crear directorio
echo "4️⃣ Creando directorio de datos..."
sudo mkdir -p /var/www/demo
sudo chown ubuntu:ubuntu /var/www/demo

# 5. Crear base de datos
echo "5️⃣ Creando base de datos SQLite..."
rm -f /var/www/demo/sistema_asistencia.db
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# 6. Verificar BD
echo "6️⃣ Verificando base de datos..."
echo "Tablas:"
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"
echo ""
echo "Usuario admin:"
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"
echo ""
echo "Configuración empresa:"
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, CompanyName, IsActive FROM CompanySettings;"

# 7. Restaurar dependencias
echo ""
echo "7️⃣ Restaurando dependencias..."
echo "Ejecutando: dotnet restore"
dotnet restore

# 8. Compilar
echo ""
echo "8️⃣ Compilando aplicación..."
echo "Ejecutando: dotnet build -c Release"
dotnet build -c Release

# 9. Verificar compilación
if [ $? -eq 0 ]; then
    echo "✅ Compilación exitosa"
else
    echo "❌ Error en compilación"
    echo "📋 Revisar errores arriba"
    exit 1
fi

# 10. Iniciar aplicación
echo ""
echo "9️⃣ Iniciando aplicación..."
echo "Ejecutando: dotnet run --configuration Release"
echo "La aplicación se iniciará en segundo plano..."
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# 11. Esperar
echo ""
echo "🔟 Esperando 10 segundos para que inicie..."
sleep 10

# 12. Verificar
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo ""
    echo "🎉 ¡ÉXITO! Sistema funcionando"
    echo "============================="
    echo "🌐 Aplicación: http://localhost:5000"
    echo "🗄️ Base de datos: /var/www/demo/sistema_asistencia.db"
    echo "📊 Logs: /var/www/demo/sistema_asistencia.log"
    echo ""
    echo "👤 Usuario administrador:"
    echo "   Email: admin@sistema.com"
    echo "   Password: admin123"
else
    echo ""
    echo "❌ Error al iniciar aplicación"
    echo "📋 Revisar logs:"
    tail -20 /var/www/demo/sistema_asistencia.log
fi

echo ""
echo "✅ Proceso manual completado"
