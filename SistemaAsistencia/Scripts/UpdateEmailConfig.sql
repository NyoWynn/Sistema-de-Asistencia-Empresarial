-- Script para actualizar la configuración de email en CompanySettings
USE [SistemaAsistenciaDB-Core-Dev]

-- Verificar si existe configuración
IF EXISTS (SELECT 1 FROM CompanySettings)
BEGIN
    -- Actualizar configuración existente
    UPDATE CompanySettings 
    SET 
        SmtpServer = 'smtp.gmail.com',
        SmtpPort = 587,
        EmailUsername = 'ncamposmaldonado@gmail.com',
        EmailPassword = 'klmr dhfi uosz jozv',
        FromEmail = 'ncamposmaldonado@gmail.com',
        FromName = 'Sistema de Asistencia',
        EmailEnabled = 1,
        UpdatedAt = GETDATE()
    WHERE Id = (SELECT TOP 1 Id FROM CompanySettings);
    
    PRINT 'Configuración de email actualizada exitosamente'
END
ELSE
BEGIN
    -- Crear nueva configuración
    INSERT INTO CompanySettings (
        CompanyName, StartTime, EndTime, LateArrivalTolerance, 
        EarlyDepartureTolerance, WorkingDays, IsActive, CreatedAt, UpdatedAt,
        SmtpServer, SmtpPort, EmailUsername, EmailPassword, 
        FromEmail, FromName, EmailEnabled
    ) VALUES (
        'Mi Empresa', '09:00:00', '18:00:00', 15, 15, 
        'Lunes,Martes,Miércoles,Jueves,Viernes', 1, GETDATE(), GETDATE(),
        'smtp.gmail.com', 587, 'ncamposmaldonado@gmail.com', 'klmr dhfi uosz jozv',
        'ncamposmaldonado@gmail.com', 'Sistema de Asistencia', 1
    );
    
    PRINT 'Nueva configuración de email creada exitosamente'
END

-- Verificar la configuración
SELECT 
    SmtpServer, SmtpPort, EmailUsername, EmailPassword, 
    FromEmail, FromName, EmailEnabled, UpdatedAt
FROM CompanySettings;

PRINT 'Script completado'


