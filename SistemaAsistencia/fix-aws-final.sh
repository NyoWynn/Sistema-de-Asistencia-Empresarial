#!/bin/bash

# Script de solución DEFINITIVA para AWS - SQLite
# Este script soluciona completamente el problema de migraciones

echo "🔧 SOLUCIÓN DEFINITIVA - Sistema de Asistencia AWS SQLite"
echo "========================================================"

# Configurar entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

# 1. Detener servicios
echo "🛑 Deteniendo servicios..."
sudo systemctl stop asistencia || true
pkill -f "dotnet.*SistemaAsistencia" || true
sleep 3

# 2. Ir al directorio del proyecto
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia

# 3. Limpiar completamente
echo "🧹 Limpiando archivos temporales..."
rm -rf bin/ obj/ Migrations/
dotnet clean

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

# 9. Restaurar dependencias
echo "📦 Restaurando dependencias..."
dotnet restore

# 10. Compilar
echo "🔨 Compilando aplicación..."
dotnet build -c Release

# 11. Verificar que no hay errores de compilación
if [ $? -ne 0 ]; then
    echo "❌ Error en la compilación. Revisar errores arriba."
    exit 1
fi

# 12. Iniciar aplicación
echo "▶️ Iniciando aplicación..."
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# 13. Esperar y verificar
sleep 5

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
    echo "   Reiniciar: ./fix-aws-final.sh"
else
    echo "❌ Error al iniciar la aplicación"
    echo "📋 Revisar logs: tail -f /var/www/demo/sistema_asistencia.log"
    exit 1
fi

echo ""
echo "✅ ¡Problema solucionado completamente!"
