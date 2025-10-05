using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;
using QRCoder;
using System.Drawing;

namespace SistemaAsistencia.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string userName, string qrCodeData, string password);
        Task SendAttendanceConfirmationAsync(string toEmail, string userName, string recordType, DateTime timestamp);
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(string toEmail, string subject, string body, CompanySettings customSettings, bool isHtml = true);
        Task<bool> TestEmailConnectionAsync();
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly ApplicationDbContext _context;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        private string GenerateQrCodeUrl(string qrData)
        {
            try
            {
                // Usar un servicio online para generar QR (más compatible con clientes de email)
                var encodedData = Uri.EscapeDataString(qrData);
                return $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={encodedData}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar URL de código QR: {Message}", ex.Message);
                return null;
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName, string qrCodeData, string password)
        {
            // Obtener configuración de empresa para el nombre
            var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
            var companyName = companySettings?.CompanyName ?? "nuestra empresa";
            
            var subject = $"¡Bienvenido a {companyName}! - Sistema de Asistencia";
            
            // Generar URL de imagen QR
            var qrCodeUrl = GenerateQrCodeUrl(qrCodeData);
            
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .qr-section {{ text-align: center; margin: 20px 0; }}
                        .qr-code {{ border: 2px solid #ddd; padding: 20px; background: white; border-radius: 10px; display: inline-block; }}
                        .qr-image {{ max-width: 200px; height: auto; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                        .button {{ background: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 20px 0; }}
                        .credentials {{ background: #e3f2fd; border: 1px solid #2196f3; padding: 15px; border-radius: 8px; margin: 20px 0; }}
                        .credential-item {{ margin: 10px 0; }}
                        .credential-label {{ font-weight: bold; color: #1976d2; }}
                        .credential-value {{ font-family: monospace; background: white; padding: 5px 10px; border-radius: 4px; border: 1px solid #ddd; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>¡Bienvenido a {companyName}!</h1>
                            <p>Sistema de Control de Asistencia</p>
                        </div>
                        <div class='content'>
                            <h2>Hola {userName},</h2>
                            <p>Te damos la bienvenida a {companyName}. Tu cuenta ha sido creada exitosamente en nuestro sistema de control de asistencia.</p>
                            
                            <div class='credentials'>
                                <h3>🔐 Tus Credenciales de Acceso</h3>
                                <div class='credential-item'>
                                    <div class='credential-label'>Email:</div>
                                    <div class='credential-value'>{toEmail}</div>
                                </div>
                                <div class='credential-item'>
                                    <div class='credential-label'>Contraseña:</div>
                                    <div class='credential-value'>{password}</div>
                                </div>
                                <p><strong>⚠️ Importante:</strong> Guarda estas credenciales en un lugar seguro. Te recomendamos cambiar tu contraseña en tu primer acceso.</p>
                            </div>
                            
                            <div class='qr-section'>
                                <h3>📱 Tu Código QR Personal</h3>
                                <p>Utiliza este código QR para marcar tu asistencia diaria:</p>
                                <div class='qr-code'>
                                    {(string.IsNullOrEmpty(qrCodeUrl) ? 
                                        $"<div style='font-family: monospace; font-size: 12px; word-break: break-all; max-width: 200px;'>{qrCodeData}</div>" :
                                        $"<img src='{qrCodeUrl}' alt='Código QR' class='qr-image' />")}
                                </div>
                            </div>
                            
                            <h3>📋 Instrucciones de uso:</h3>
                            <ul>
                                <li>Inicia sesión con tus credenciales en el sistema</li>
                                <li>Presenta este código QR en el sistema de asistencia</li>
                                <li>Marca tu entrada al llegar al trabajo</li>
                                <li>Marca tu salida al finalizar tu jornada</li>
                                <li>Mantén este código seguro y no lo compartas</li>
                            </ul>
                            
                            <p>Si tienes alguna pregunta o necesitas ayuda, no dudes en contactar al administrador del sistema.</p>
                            
                            <div class='footer'>
                                <p>Este es un mensaje automático del Sistema de Control de Asistencia de {companyName}</p>
                                <p>Por favor, no respondas a este correo</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAttendanceConfirmationAsync(string toEmail, string userName, string recordType, DateTime timestamp)
        {
            // Obtener configuración de empresa para el nombre
            var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
            var companyName = companySettings?.CompanyName ?? "nuestra empresa";
            
            var isEntry = recordType == "Entrada";
            var actionText = isEntry ? "entrada" : "salida";
            var actionIcon = isEntry ? "🚪" : "🚪";
            var actionColor = isEntry ? "#28a745" : "#dc3545";
            var actionBadge = isEntry ? "Entrada" : "Salida";
            
            var subject = $"✅ Comprobante de {actionText} - {companyName}";
            
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, {actionColor} 0%, #6c757d 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .confirmation {{ background: white; border: 2px solid {actionColor}; padding: 20px; border-radius: 10px; margin: 20px 0; text-align: center; }}
                        .timestamp {{ font-size: 24px; font-weight: bold; color: {actionColor}; margin: 10px 0; }}
                        .date {{ font-size: 18px; color: #666; margin: 5px 0; }}
                        .badge {{ background: {actionColor}; color: white; padding: 8px 16px; border-radius: 20px; font-weight: bold; display: inline-block; margin: 10px 0; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                        .info-box {{ background: #e3f2fd; border: 1px solid #2196f3; padding: 15px; border-radius: 8px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>{actionIcon} Comprobante de {actionText}</h1>
                            <p>Sistema de Control de Asistencia - {companyName}</p>
                        </div>
                        <div class='content'>
                            <h2>Hola {userName},</h2>
                            <p>Te confirmamos que tu {actionText} ha sido registrada exitosamente en nuestro sistema.</p>
                            
                            <div class='confirmation'>
                                <div class='badge'>{actionBadge}</div>
                                <div class='timestamp'>{timestamp.ToString("HH:mm")}</div>
                                <div class='date'>{timestamp.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES"))}</div>
                            </div>
                            
                            <div class='info-box'>
                                <h3>📋 Detalles del registro:</h3>
                                <ul style='text-align: left; margin: 10px 0;'>
                                    <li><strong>Usuario:</strong> {userName}</li>
                                    <li><strong>Acción:</strong> {actionBadge}</li>
                                    <li><strong>Fecha:</strong> {timestamp.ToString("dd/MM/yyyy")}</li>
                                    <li><strong>Hora:</strong> {timestamp.ToString("HH:mm:ss")}</li>
                                    <li><strong>Empresa:</strong> {companyName}</li>
                                </ul>
                            </div>
                            
                            <p>Este comprobante es tu respaldo oficial del registro de asistencia. Guárdalo para tus registros personales.</p>
                            
                            <div class='footer'>
                                <p>Este es un mensaje automático del Sistema de Control de Asistencia de {companyName}</p>
                                <p>Por favor, no respondas a este correo</p>
                                <p><strong>ID de registro:</strong> {timestamp.Ticks}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            // Obtener configuración de email de la base de datos
            var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
            await SendEmailAsync(toEmail, subject, body, companySettings, isHtml);
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, CompanySettings customSettings, bool isHtml = true)
        {
            try
            {
                // Usar configuración personalizada si se proporciona, sino obtener de la base de datos
                var companySettings = customSettings ?? await _context.CompanySettings.FirstOrDefaultAsync();
                
                _logger.LogInformation($"Intentando enviar email a {toEmail}");
                _logger.LogInformation($"Configuración de email - Habilitado: {companySettings?.EmailEnabled}");
                _logger.LogInformation($"Servidor SMTP: {companySettings?.SmtpServer}");
                _logger.LogInformation($"Puerto: {companySettings?.SmtpPort}");
                _logger.LogInformation($"Usuario: {companySettings?.EmailUsername}");
                
                if (companySettings == null)
                {
                    _logger.LogWarning("No se encontró configuración de empresa en la base de datos");
                    throw new InvalidOperationException("No se encontró configuración de empresa en la base de datos");
                }
                
                if (!companySettings.EmailEnabled)
                {
                    _logger.LogWarning("El envío de emails está deshabilitado en la configuración");
                    throw new InvalidOperationException("El envío de emails está deshabilitado en la configuración");
                }
                
                if (string.IsNullOrEmpty(companySettings.SmtpServer))
                {
                    _logger.LogWarning("Servidor SMTP no configurado");
                    throw new InvalidOperationException("Servidor SMTP no configurado");
                }
                
                if (string.IsNullOrEmpty(companySettings.EmailUsername))
                {
                    _logger.LogWarning("Usuario de email no configurado");
                    throw new InvalidOperationException("Usuario de email no configurado");
                }

                if (string.IsNullOrEmpty(companySettings.EmailPassword))
                {
                    _logger.LogWarning("Contraseña de email no configurada");
                    throw new InvalidOperationException("Contraseña de email no configurada");
                }

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(companySettings.FromEmail ?? companySettings.EmailUsername));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain)
                {
                    Text = body
                };

                _logger.LogInformation($"Conectando al servidor SMTP {companySettings.SmtpServer}:{companySettings.SmtpPort}");
                
                using var smtp = new SmtpClient();
                
                // Configurar timeout y opciones de conexión
                smtp.Timeout = 30000; // 30 segundos
                
                try
                {
                    await smtp.ConnectAsync(companySettings.SmtpServer, 
                        companySettings.SmtpPort, 
                        SecureSocketOptions.StartTls);
                }
                catch (Exception connectEx)
                {
                    _logger.LogWarning($"Error con StartTls, intentando con Auto: {connectEx.Message}");
                    // Si falla StartTls, intentar con Auto
                    await smtp.ConnectAsync(companySettings.SmtpServer, 
                        companySettings.SmtpPort, 
                        SecureSocketOptions.Auto);
                }
                
                _logger.LogInformation("Conectado al servidor SMTP, autenticando...");
                await smtp.AuthenticateAsync(companySettings.EmailUsername, 
                    companySettings.EmailPassword);
                
                _logger.LogInformation("Autenticación exitosa, enviando email...");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"✅ Email enviado exitosamente a {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al enviar email a {toEmail}: {ex.Message}");
                _logger.LogError($"Detalles del error: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<bool> TestEmailConnectionAsync()
        {
            try
            {
                var companySettings = await _context.CompanySettings.FirstOrDefaultAsync();
                
                if (companySettings == null || !companySettings.EmailEnabled)
                {
                    _logger.LogWarning("Configuración de email no disponible o deshabilitada");
                    return false;
                }

                _logger.LogInformation($"Probando conexión SMTP: {companySettings.SmtpServer}:{companySettings.SmtpPort}");
                
                using var smtp = new SmtpClient();
                smtp.Timeout = 10000; // 10 segundos para prueba
                
                try
                {
                    await smtp.ConnectAsync(companySettings.SmtpServer, 
                        companySettings.SmtpPort, 
                        SecureSocketOptions.StartTls);
                }
                catch (Exception connectEx)
                {
                    _logger.LogWarning($"Error con StartTls, intentando con Auto: {connectEx.Message}");
                    await smtp.ConnectAsync(companySettings.SmtpServer, 
                        companySettings.SmtpPort, 
                        SecureSocketOptions.Auto);
                }
                
                await smtp.AuthenticateAsync(companySettings.EmailUsername, companySettings.EmailPassword);
                await smtp.DisconnectAsync(true);
                
                _logger.LogInformation("✅ Conexión SMTP exitosa");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error en prueba de conexión SMTP: {ex.Message}");
                return false;
            }
        }
    }
}
