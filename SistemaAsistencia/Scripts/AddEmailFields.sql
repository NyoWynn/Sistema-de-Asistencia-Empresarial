-- Script para agregar campos de email a CompanySettings si no existen
USE [SistemaAsistenciaDB-Core-Dev]

-- Verificar si las columnas existen y agregarlas si no
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'SmtpServer')
BEGIN
    ALTER TABLE [CompanySettings] ADD [SmtpServer] nvarchar(max) NULL;
    PRINT 'Columna SmtpServer agregada'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'SmtpPort')
BEGIN
    ALTER TABLE [CompanySettings] ADD [SmtpPort] int NOT NULL DEFAULT 587;
    PRINT 'Columna SmtpPort agregada'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'EmailUsername')
BEGIN
    ALTER TABLE [CompanySettings] ADD [EmailUsername] nvarchar(max) NULL;
    PRINT 'Columna EmailUsername agregada'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'EmailPassword')
BEGIN
    ALTER TABLE [CompanySettings] ADD [EmailPassword] nvarchar(max) NULL;
    PRINT 'Columna EmailPassword agregada'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'FromEmail')
BEGIN
    ALTER TABLE [CompanySettings] ADD [FromEmail] nvarchar(max) NULL;
    PRINT 'Columna FromEmail agregada'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'FromName')
BEGIN
    ALTER TABLE [CompanySettings] ADD [FromName] nvarchar(max) NULL;
    PRINT 'Columna FromName agregada'
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CompanySettings' AND COLUMN_NAME = 'EmailEnabled')
BEGIN
    ALTER TABLE [CompanySettings] ADD [EmailEnabled] bit NOT NULL DEFAULT 0;
    PRINT 'Columna EmailEnabled agregada'
END

-- Mostrar la estructura actual de la tabla
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'CompanySettings'
ORDER BY ORDINAL_POSITION;

PRINT 'Script completado'


