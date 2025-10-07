#!/bin/bash

# Script de despliegue para AWS
# Sistema de Asistencia Empresarial

echo "🚀 Iniciando despliegue en AWS..."

# Configurar variables de entorno
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:5000

# Crear backup de la base de datos si existe
if [ -f "sistema_asistencia.db" ]; then
    echo "📦 Creando backup de la base de datos..."
    cp sistema_asistencia.db "sistema_asistencia_backup_$(date +%Y%m%d_%H%M%S).db"
fi

# Actualizar código desde Git
echo "📥 Actualizando código desde Git..."
git pull origin main

# Restaurar dependencias
echo "📦 Restaurando dependencias..."
dotnet restore

# Compilar la aplicación
echo "🔨 Compilando aplicación..."
dotnet build -c Release

# Aplicar migraciones si es necesario
echo "🗄️ Aplicando migraciones de base de datos..."
dotnet ef database update

# Detener procesos existentes
echo "🛑 Deteniendo procesos existentes..."
pkill -f "dotnet.*SistemaAsistencia" || true

# Iniciar la aplicación
echo "▶️ Iniciando aplicación..."
nohup dotnet run --configuration Release > sistema_asistencia.log 2>&1 &

# Esperar un momento para que la aplicación inicie
sleep 5

# Verificar que la aplicación esté ejecutándose
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo "✅ Aplicación desplegada exitosamente!"
    echo "📊 Logs disponibles en: sistema_asistencia.log"
    echo "🌐 Aplicación ejecutándose en: http://localhost:5000"
else
    echo "❌ Error al iniciar la aplicación"
    echo "📋 Revisar logs en: sistema_asistencia.log"
    exit 1
fi

echo "🎉 Despliegue completado!"
