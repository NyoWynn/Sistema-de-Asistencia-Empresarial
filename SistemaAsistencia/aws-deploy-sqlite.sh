#!/bin/bash

# Script de despliegue específico para AWS con SQLite
# Sistema de Asistencia Empresarial

echo "🚀 Iniciando despliegue en AWS con SQLite..."

# Configurar variables de entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

# Crear directorio de datos si no existe
mkdir -p /var/www/demo
cd /var/www/demo

# Crear backup de la base de datos si existe
if [ -f "sistema_asistencia.db" ]; then
    echo "📦 Creando backup de la base de datos..."
    cp sistema_asistencia.db "sistema_asistencia_backup_$(date +%Y%m%d_%H%M%S).db"
fi

# Actualizar código desde Git
echo "📥 Actualizando código desde Git..."
git pull origin main

# Ir al directorio del proyecto
cd SistemaAsistencia

# Restaurar dependencias
echo "📦 Restaurando dependencias..."
dotnet restore

# Limpiar y compilar
echo "🔨 Compilando aplicación..."
dotnet clean
dotnet build -c Release

# Crear base de datos SQLite manualmente
echo "🗄️ Creando base de datos SQLite..."
sqlite3 /var/www/demo/sistema_asistencia.db < Scripts/CreateSQLiteDatabase.sql

# Verificar que las tablas se crearon correctamente
echo "✅ Verificando estructura de base de datos..."
sqlite3 /var/www/demo/sistema_asistencia.db ".tables"

# Verificar datos iniciales
echo "👤 Verificando usuario administrador..."
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, Email, IsAdmin FROM Users;"

# Verificar configuración de empresa
echo "🏢 Verificando configuración de empresa..."
sqlite3 /var/www/demo/sistema_asistencia.db "SELECT Id, CompanyName, IsActive FROM CompanySettings;"

# Detener procesos existentes
echo "🛑 Deteniendo procesos existentes..."
pkill -f "dotnet.*SistemaAsistencia" || true
sleep 2

# Iniciar la aplicación
echo "▶️ Iniciando aplicación..."
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# Esperar un momento para que la aplicación inicie
sleep 5

# Verificar que la aplicación esté ejecutándose
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo "✅ Aplicación desplegada exitosamente!"
    echo "📊 Logs disponibles en: /var/www/demo/sistema_asistencia.log"
    echo "🌐 Aplicación ejecutándose en: http://localhost:5000"
    echo "🗄️ Base de datos SQLite: /var/www/demo/sistema_asistencia.db"
else
    echo "❌ Error al iniciar la aplicación"
    echo "📋 Revisar logs en: /var/www/demo/sistema_asistencia.log"
    exit 1
fi

echo "🎉 Despliegue completado!"
echo ""
echo "📋 Comandos útiles:"
echo "  - Ver logs: tail -f /var/www/demo/sistema_asistencia.log"
echo "  - Verificar BD: sqlite3 /var/www/demo/sistema_asistencia.db '.tables'"
echo "  - Detener app: pkill -f 'dotnet.*SistemaAsistencia'"
echo "  - Reiniciar: ./aws-deploy-sqlite.sh"
