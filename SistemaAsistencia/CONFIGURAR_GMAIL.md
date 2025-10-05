# Configuración de Gmail para el Sistema de Asistencia

## 🔧 Configuración Paso a Paso

### 1. Habilitar Autenticación de 2 Factores
1. Ve a tu cuenta de Google: https://myaccount.google.com/
2. Selecciona "Seguridad"
3. Activa "Verificación en 2 pasos"

### 2. Generar Contraseña de Aplicación
1. En la misma sección de "Seguridad"
2. Busca "Contraseñas de aplicaciones"
3. Selecciona "Correo" y "Otro (nombre personalizado)"
4. Escribe "Sistema de Asistencia"
5. Copia la contraseña generada (16 caracteres)

### 3. Configuración en el Sistema
- **Servidor SMTP:** `smtp.gmail.com`
- **Puerto:** `587`
- **Usuario:** Tu email completo de Gmail
- **Contraseña:** La contraseña de aplicación generada (NO tu contraseña normal)
- **Email de Envío:** Tu email de Gmail
- **Nombre del Remitente:** "Sistema de Asistencia"

## 🔍 Solución de Problemas

### Error: "No se puede conectar al servidor SMTP"
**Posibles causas:**
1. **Firewall corporativo** - Contacta a tu administrador de TI
2. **Red bloqueada** - Prueba desde otra red
3. **Credenciales incorrectas** - Verifica la contraseña de aplicación
4. **2FA no habilitado** - Debes tener 2FA activado en Gmail

### Error: "Autenticación fallida"
**Soluciones:**
1. Verifica que la contraseña de aplicación sea correcta
2. Asegúrate de que el email sea el completo (ejemplo@gmail.com)
3. Confirma que 2FA esté habilitado

### Error: "Timeout de conexión"
**Soluciones:**
1. Verifica tu conexión a internet
2. Prueba desde otra red
3. Contacta a tu proveedor de internet si persiste

## 📧 Configuración Alternativa

Si Gmail no funciona, puedes usar otros proveedores:

### Outlook/Hotmail
- **Servidor:** `smtp-mail.outlook.com`
- **Puerto:** `587`

### Yahoo
- **Servidor:** `smtp.mail.yahoo.com`
- **Puerto:** `587`

## ✅ Verificación

Usa el botón "Probar Conexión" en la configuración para verificar que todo esté funcionando correctamente.

## 🆘 Soporte

Si continúas teniendo problemas:
1. Verifica que tu firewall permita conexiones SMTP
2. Prueba desde una red diferente
3. Contacta a tu administrador de TI
4. Revisa los logs del sistema para más detalles