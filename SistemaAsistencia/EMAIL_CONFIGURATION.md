# Configuración de Email - Sistema de Asistencia

## Configuración para Gmail

Para configurar el envío de emails con Gmail, sigue estos pasos:

### 1. Habilitar la verificación en 2 pasos
- Ve a tu cuenta de Google
- Seguridad → Verificación en 2 pasos
- Actívala si no está activada

### 2. Generar una contraseña de aplicación
- Ve a Seguridad → Contraseñas de aplicaciones
- Selecciona "Correo" y "Otro (nombre personalizado)"
- Escribe "Sistema de Asistencia"
- Copia la contraseña generada (16 caracteres)

### 3. Actualizar appsettings.json
Reemplaza los valores en `appsettings.json` y `appsettings.Development.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "tu-email@gmail.com",
    "Password": "tu-contraseña-de-aplicacion-16-caracteres",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Sistema de Asistencia"
  }
}
```

### 4. Otras configuraciones de email

#### Outlook/Hotmail
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp-mail.outlook.com",
    "SmtpPort": "587",
    "Username": "tu-email@outlook.com",
    "Password": "tu-contraseña",
    "FromEmail": "tu-email@outlook.com",
    "FromName": "Sistema de Asistencia"
  }
}
```

#### Yahoo
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.mail.yahoo.com",
    "SmtpPort": "587",
    "Username": "tu-email@yahoo.com",
    "Password": "tu-contraseña-de-aplicacion",
    "FromEmail": "tu-email@yahoo.com",
    "FromName": "Sistema de Asistencia"
  }
}
```

## Funcionalidades implementadas

- ✅ Email de bienvenida al crear usuario
- ✅ Código QR incluido en el email
- ✅ Diseño HTML profesional
- ✅ Manejo de errores
- ✅ Notificaciones en la interfaz

## Próximas funcionalidades

- 📧 Email de notificación de asistencia
- 📧 Recordatorios de marcado
- 📧 Reportes por email
- 📧 Notificaciones de ausencias


