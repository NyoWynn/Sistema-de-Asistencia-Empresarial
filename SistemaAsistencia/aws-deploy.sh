#!/usr/bin/env bash
set -euo pipefail

# === Parámetros del despliegue ===
APP_ROOT="$HOME/Sistema-de-Asistencia-Empresarial/SistemaAsistencia"
PUBLISH_DIR="/var/www/demo"
SERVICE="asistencia"
DB_FILE="$PUBLISH_DIR/app.db"

echo "🏷  APP_ROOT=$APP_ROOT"
echo "🏷  PUBLISH_DIR=$PUBLISH_DIR"
echo "🏷  SERVICE=$SERVICE"
echo "🏷  DB_FILE=$DB_FILE"

# === Preparar entorno ===
echo "📦 Restaurando dependencias..."
cd "$APP_ROOT"
~/.dotnet/dotnet restore

echo "🔨 Compilando (Release)..."
~/.dotnet/dotnet build -c Release

# === Parar servicio antes de publicar ===
echo "🛑 Deteniendo servicio $SERVICE..."
sudo systemctl stop "$SERVICE" || true

# === Publicar artefactos a /var/www/demo ===
echo "📤 Publicando a $PUBLISH_DIR ..."
sudo mkdir -p "$PUBLISH_DIR"
~/.dotnet/dotnet publish -c Release -o "$PUBLISH_DIR"

# Propietarios/permiso sensatos
echo "🔧 Ajustando permisos..."
sudo chown -R ubuntu:www-data "$PUBLISH_DIR"
sudo find "$PUBLISH_DIR" -type f -name "*.dll" -exec chmod 640 {} \;

# === Migraciones en SQLite (usa el archivo de prod) ===
echo "🗄️ Aplicando migraciones en SQLite..."
# Asegura herramientas EF disponibles
~/.dotnet/dotnet tool install --global dotnet-ef >/dev/null 2>&1 || true
export PATH="$PATH:$HOME/.dotnet/tools"

# Ejecuta migraciones apuntando explícitamente al archivo SQLite de prod
dotnet-ef database update \
  --project "$APP_ROOT" \
  --connection "Data Source=$DB_FILE"

# === Reiniciar servicio con las variables adecuadas ===
# (El unit ya debe tener estas Environment=, pero las recordamos por si faltan)
echo "🚀 Iniciando servicio $SERVICE..."
sudo systemctl daemon-reload
sudo systemctl start "$SERVICE"
sleep 2
sudo systemctl status "$SERVICE" --no-pager || true

echo "✅ Despliegue completado."
echo "ℹ️  Prueba localmente:  curl -I http://127.0.0.1:5000"
