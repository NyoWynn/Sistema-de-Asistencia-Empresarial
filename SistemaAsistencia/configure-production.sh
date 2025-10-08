#!/bin/bash

# Script para configurar el sistema para producción con dominio público
# Sistema de Asistencia Empresarial

echo "🌐 CONFIGURANDO SISTEMA PARA PRODUCCIÓN"
echo "======================================="

# Variables
DOMAIN="sistema-asistencia.site"
APP_PORT="5000"

# 1. Detener aplicación actual
echo "🛑 Deteniendo aplicación actual..."
pkill -f "dotnet.*SistemaAsistencia" || true
sleep 2

# 2. Configurar aplicación para escuchar en todas las interfaces
echo "🔧 Configurando aplicación para producción..."
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://0.0.0.0:$APP_PORT

# 3. Iniciar aplicación
echo "▶️ Iniciando aplicación en puerto $APP_PORT..."
cd ~/Sistema-de-Asistencia-Empresarial/SistemaAsistencia
nohup dotnet run --configuration Release > /var/www/demo/sistema_asistencia.log 2>&1 &

# 4. Esperar que inicie
echo "⏳ Esperando que la aplicación inicie..."
sleep 10

# 5. Verificar que la aplicación esté ejecutándose
if pgrep -f "dotnet.*SistemaAsistencia" > /dev/null; then
    echo "✅ Aplicación iniciada correctamente"
else
    echo "❌ Error al iniciar aplicación"
    exit 1
fi

# 6. Instalar Nginx si no está instalado
echo "📦 Verificando Nginx..."
if ! command -v nginx &> /dev/null; then
    echo "Instalando Nginx..."
    sudo apt update
    sudo apt install nginx -y
fi

# 7. Crear configuración de Nginx
echo "🔧 Configurando Nginx..."
sudo tee /etc/nginx/sites-available/$DOMAIN > /dev/null <<EOF
server {
    listen 80;
    server_name $DOMAIN www.$DOMAIN;

    location / {
        proxy_pass http://127.0.0.1:$APP_PORT;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
    }
}
EOF

# 8. Habilitar sitio
echo "🔗 Habilitando sitio en Nginx..."
sudo ln -sf /etc/nginx/sites-available/$DOMAIN /etc/nginx/sites-enabled/
sudo rm -f /etc/nginx/sites-enabled/default

# 9. Verificar configuración de Nginx
echo "🔍 Verificando configuración de Nginx..."
if sudo nginx -t; then
    echo "✅ Configuración de Nginx válida"
else
    echo "❌ Error en configuración de Nginx"
    exit 1
fi

# 10. Reiniciar Nginx
echo "🔄 Reiniciando Nginx..."
sudo systemctl restart nginx
sudo systemctl enable nginx

# 11. Verificar que Nginx esté ejecutándose
if sudo systemctl is-active --quiet nginx; then
    echo "✅ Nginx ejecutándose correctamente"
else
    echo "❌ Error al iniciar Nginx"
    exit 1
fi

# 12. Mostrar estado final
echo ""
echo "🎉 ¡CONFIGURACIÓN COMPLETADA!"
echo "============================="
echo "🌐 Dominio: http://$DOMAIN"
echo "🔧 Aplicación: http://localhost:$APP_PORT"
echo "📊 Logs: /var/www/demo/sistema_asistencia.log"
echo ""
echo "👤 Usuario administrador:"
echo "   Email: admin@sistema.com"
echo "   Password: admin123"
echo ""
echo "📋 Próximos pasos:"
echo "1. Configurar DNS para apuntar $DOMAIN a esta IP"
echo "2. Ejecutar: sudo certbot --nginx -d $DOMAIN -d www.$DOMAIN"
echo "3. Acceder a https://$DOMAIN"
echo ""
echo "🔍 Verificaciones:"
echo "   Aplicación: curl http://localhost:$APP_PORT"
echo "   Nginx: curl http://$DOMAIN"
echo "   Procesos: ps aux | grep -E '(dotnet|nginx)' | grep -v grep"
