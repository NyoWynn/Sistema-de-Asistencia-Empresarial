-- Script de migración manual para SQLite
-- Ejecutar este script si ya tienes datos en SQL Server y quieres migrar a SQLite

-- 1. Crear las tablas si no existen
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Email" TEXT NOT NULL UNIQUE,
    "Password" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Phone" TEXT NULL,
    "Address" TEXT NULL,
    "BaseSalary" REAL NULL,
    "HireDate" TEXT NULL,
    "IsAdmin" INTEGER NOT NULL DEFAULT 0,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "QrCode" TEXT NULL
);

CREATE TABLE IF NOT EXISTS "AttendanceRecords" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "Timestamp" TEXT NOT NULL,
    "RecordType" TEXT NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CompanySettings" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "CompanyName" TEXT NOT NULL,
    "StartTime" TEXT NOT NULL,
    "EndTime" TEXT NOT NULL,
    "LateArrivalTolerance" INTEGER NOT NULL DEFAULT 15,
    "EarlyDepartureTolerance" INTEGER NOT NULL DEFAULT 15,
    "WorkingDays" TEXT NOT NULL DEFAULT 'Lunes,Martes,Miércoles,Jueves,Viernes',
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "SmtpServer" TEXT NULL,
    "SmtpPort" INTEGER NOT NULL DEFAULT 587,
    "EmailUsername" TEXT NULL,
    "EmailPassword" TEXT NULL,
    "FromEmail" TEXT NULL,
    "FromName" TEXT NULL DEFAULT 'Sistema de Asistencia',
    "EmailEnabled" INTEGER NOT NULL DEFAULT 0,
    "WelcomeEmailTemplate" TEXT NULL,
    "AttendanceEmailTemplate" TEXT NULL
);

-- 2. Agregar tabla OfficeLocations si no existe
CREATE TABLE IF NOT EXISTS "OfficeLocations" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "Latitude" REAL NOT NULL,
    "Longitude" REAL NOT NULL,
    "RadiusMeters" INTEGER NOT NULL DEFAULT 100,
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "Description" TEXT NULL
);

-- 3. Crear índices
CREATE INDEX IF NOT EXISTS "IX_AttendanceRecords_UserId" ON "AttendanceRecords" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_AttendanceRecords_Timestamp" ON "AttendanceRecords" ("Timestamp");
CREATE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE INDEX IF NOT EXISTS "IX_Users_IsAdmin" ON "Users" ("IsAdmin");
CREATE INDEX IF NOT EXISTS "IX_OfficeLocations_IsActive" ON "OfficeLocations" ("IsActive");

-- 4. Insertar datos por defecto si no existen
INSERT OR IGNORE INTO "CompanySettings" (
    "CompanyName", "StartTime", "EndTime", "LateArrivalTolerance", 
    "EarlyDepartureTolerance", "WorkingDays", "IsActive", "CreatedAt", "UpdatedAt"
) VALUES (
    'Mi Empresa', '08:00:00', '17:00:00', 15, 15, 
    'Lunes,Martes,Miércoles,Jueves,Viernes', 1, 
    datetime('now'), datetime('now')
);

INSERT OR IGNORE INTO "Users" (
    "Name", "Email", "Password", "IsAdmin", "IsActive"
) VALUES (
    'Administrador', 'admin@sistema.com', 'admin123', 1, 1
);

-- 5. Verificar que todo se creó correctamente
SELECT 'Tablas creadas:' as Status;
.tables

SELECT 'Usuarios:' as Status;
SELECT Id, Email, IsAdmin FROM Users;

SELECT 'Configuración de empresa:' as Status;
SELECT Id, CompanyName, IsActive FROM CompanySettings;
