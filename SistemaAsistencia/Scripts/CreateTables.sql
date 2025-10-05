-- Script para crear las tablas necesarias para la gestión avanzada de personal

-- Crear tabla Departments
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
CREATE TABLE [Departments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);

-- Crear tabla Shifts
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Shifts' AND xtype='U')
CREATE TABLE [Shifts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [IsNightShift] bit NOT NULL DEFAULT 0,
    [IsPartTime] bit NOT NULL DEFAULT 0,
    [IsRotating] bit NOT NULL DEFAULT 0,
    [WorkingDays] nvarchar(max) NOT NULL DEFAULT 'Lunes-Viernes',
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Shifts] PRIMARY KEY ([Id])
);

-- Crear tabla EmployeeProfiles
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeProfiles' AND xtype='U')
CREATE TABLE [EmployeeProfiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NULL,
    [WorkingHoursPerDay] int NOT NULL DEFAULT 8,
    [WorkingDaysPerWeek] int NOT NULL DEFAULT 5,
    [AllowsOvertime] bit NOT NULL DEFAULT 1,
    [MaxOvertimeHoursPerDay] int NOT NULL DEFAULT 4,
    [MaxOvertimeHoursPerWeek] int NOT NULL DEFAULT 20,
    [VacationDaysPerYear] int NOT NULL DEFAULT 20,
    [RequiresVacationApproval] bit NOT NULL DEFAULT 1,
    [RequiresPermissionApproval] bit NOT NULL DEFAULT 1,
    [AllowsRemoteWork] bit NOT NULL DEFAULT 0,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_EmployeeProfiles] PRIMARY KEY ([Id])
);

-- Crear tabla Employees
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Employees' AND xtype='U')
CREATE TABLE [Employees] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmployeeId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [HireDate] datetime2 NOT NULL,
    [BaseSalary] decimal(18,2) NOT NULL,
    [IsAdmin] bit NOT NULL DEFAULT 0,
    [IsActive] bit NOT NULL DEFAULT 1,
    [QrCode] nvarchar(max) NULL,
    [DepartmentId] int NULL,
    [ShiftId] int NULL,
    [EmployeeProfileId] int NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
);

-- Crear tabla VacationRequests
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VacationRequests' AND xtype='U')
CREATE TABLE [VacationRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [RequestedDays] int NOT NULL,
    [Reason] nvarchar(max) NULL,
    [Status] int NOT NULL DEFAULT 0,
    [RequestDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [ProcessedDate] datetime2 NULL,
    [ProcessedBy] int NULL,
    [ApproverComments] nvarchar(max) NULL,
    CONSTRAINT [PK_VacationRequests] PRIMARY KEY ([Id])
);

-- Crear tabla PermissionRequests
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PermissionRequests' AND xtype='U')
CREATE TABLE [PermissionRequests] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [Date] datetime2 NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [RequestedHours] decimal(18,2) NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [PermissionType] int NOT NULL DEFAULT 0,
    [Status] int NOT NULL DEFAULT 0,
    [RequestDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [ProcessedDate] datetime2 NULL,
    [ProcessedBy] int NULL,
    [ApproverComments] nvarchar(max) NULL,
    CONSTRAINT [PK_PermissionRequests] PRIMARY KEY ([Id])
);

-- Crear tabla OvertimeRecords
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OvertimeRecords' AND xtype='U')
CREATE TABLE [OvertimeRecords] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [Date] datetime2 NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [OvertimeHours] decimal(18,2) NOT NULL,
    [Reason] nvarchar(max) NULL,
    [IsApproved] bit NOT NULL DEFAULT 0,
    [IsPaid] bit NOT NULL DEFAULT 0,
    [PaymentType] int NOT NULL DEFAULT 0,
    [Amount] decimal(18,2) NOT NULL,
    [RecordDate] datetime2 NOT NULL DEFAULT GETDATE(),
    [ApprovedBy] int NULL,
    [ApprovalDate] datetime2 NULL,
    [Comments] nvarchar(max) NULL,
    CONSTRAINT [PK_OvertimeRecords] PRIMARY KEY ([Id])
);

-- Crear tabla DepartmentShift (relación many-to-many)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DepartmentShift' AND xtype='U')
CREATE TABLE [DepartmentShift] (
    [DepartmentsId] int NOT NULL,
    [ShiftsId] int NOT NULL,
    CONSTRAINT [PK_DepartmentShift] PRIMARY KEY ([DepartmentsId], [ShiftsId])
);

-- Agregar columna EmployeeId a AttendanceRecords si no existe
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AttendanceRecords') AND name = 'EmployeeId')
ALTER TABLE [AttendanceRecords] ADD [EmployeeId] int NULL;

-- Crear índices únicos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_EmployeeId')
CREATE UNIQUE INDEX [IX_Employees_EmployeeId] ON [Employees] ([EmployeeId]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Employees_Email')
CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Departments_Name')
CREATE UNIQUE INDEX [IX_Departments_Name] ON [Departments] ([Name]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Shifts_Name')
CREATE UNIQUE INDEX [IX_Shifts_Name] ON [Shifts] ([Name]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EmployeeProfiles_Name')
CREATE UNIQUE INDEX [IX_EmployeeProfiles_Name] ON [EmployeeProfiles] ([Name]);

-- Crear claves foráneas
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Employees_Departments_DepartmentId')
ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Departments_DepartmentId] 
FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE SET NULL;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Employees_Shifts_ShiftId')
ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Shifts_ShiftId] 
FOREIGN KEY ([ShiftId]) REFERENCES [Shifts] ([Id]) ON DELETE SET NULL;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Employees_EmployeeProfiles_EmployeeProfileId')
ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_EmployeeProfiles_EmployeeProfileId] 
FOREIGN KEY ([EmployeeProfileId]) REFERENCES [EmployeeProfiles] ([Id]) ON DELETE SET NULL;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VacationRequests_Employees_EmployeeId')
ALTER TABLE [VacationRequests] ADD CONSTRAINT [FK_VacationRequests_Employees_EmployeeId] 
FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VacationRequests_Employees_ProcessedBy')
ALTER TABLE [VacationRequests] ADD CONSTRAINT [FK_VacationRequests_Employees_ProcessedBy] 
FOREIGN KEY ([ProcessedBy]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PermissionRequests_Employees_EmployeeId')
ALTER TABLE [PermissionRequests] ADD CONSTRAINT [FK_PermissionRequests_Employees_EmployeeId] 
FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PermissionRequests_Employees_ProcessedBy')
ALTER TABLE [PermissionRequests] ADD CONSTRAINT [FK_PermissionRequests_Employees_ProcessedBy] 
FOREIGN KEY ([ProcessedBy]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OvertimeRecords_Employees_EmployeeId')
ALTER TABLE [OvertimeRecords] ADD CONSTRAINT [FK_OvertimeRecords_Employees_EmployeeId] 
FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OvertimeRecords_Employees_ApprovedBy')
ALTER TABLE [OvertimeRecords] ADD CONSTRAINT [FK_OvertimeRecords_Employees_ApprovedBy] 
FOREIGN KEY ([ApprovedBy]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DepartmentShift_Departments_DepartmentsId')
ALTER TABLE [DepartmentShift] ADD CONSTRAINT [FK_DepartmentShift_Departments_DepartmentsId] 
FOREIGN KEY ([DepartmentsId]) REFERENCES [Departments] ([Id]) ON DELETE CASCADE;

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DepartmentShift_Shifts_ShiftsId')
ALTER TABLE [DepartmentShift] ADD CONSTRAINT [FK_DepartmentShift_Shifts_ShiftsId] 
FOREIGN KEY ([ShiftsId]) REFERENCES [Shifts] ([Id]) ON DELETE CASCADE;

-- Insertar datos de ejemplo
INSERT INTO [Departments] ([Name], [Description], [IsActive]) VALUES 
('Recursos Humanos', 'Gestión de personal y administración', 1),
('Tecnología', 'Desarrollo de software y sistemas', 1),
('Ventas', 'Equipo comercial y atención al cliente', 1),
('Administración', 'Gestión financiera y administrativa', 1);

INSERT INTO [Shifts] ([Name], [Description], [StartTime], [EndTime], [IsNightShift], [IsPartTime], [IsRotating], [WorkingDays], [IsActive]) VALUES 
('Turno Matutino', '8:00 AM - 5:00 PM', '08:00:00', '17:00:00', 0, 0, 0, 'Lunes-Viernes', 1),
('Turno Vespertino', '2:00 PM - 11:00 PM', '14:00:00', '23:00:00', 0, 0, 0, 'Lunes-Viernes', 1),
('Turno Nocturno', '10:00 PM - 6:00 AM', '22:00:00', '06:00:00', 1, 0, 0, 'Lunes-Viernes', 1),
('Turno Parcial', '4 horas diarias', '09:00:00', '13:00:00', 0, 1, 0, 'Lunes-Viernes', 1);

INSERT INTO [EmployeeProfiles] ([Name], [Description], [WorkingHoursPerDay], [WorkingDaysPerWeek], [AllowsOvertime], [MaxOvertimeHoursPerDay], [MaxOvertimeHoursPerWeek], [VacationDaysPerYear], [RequiresVacationApproval], [RequiresPermissionApproval], [AllowsRemoteWork], [IsActive]) VALUES 
('Empleado Regular', 'Perfil estándar para empleados', 8, 5, 1, 4, 20, 20, 1, 1, 0, 1),
('Gerente', 'Perfil para gerentes y supervisores', 8, 5, 1, 6, 30, 25, 0, 0, 1, 1),
('Tiempo Parcial', 'Perfil para empleados de medio tiempo', 4, 5, 0, 0, 0, 10, 1, 1, 0, 1),
('Trabajador Remoto', 'Perfil para trabajadores remotos', 8, 5, 1, 4, 20, 20, 1, 1, 1, 1);

INSERT INTO [Employees] ([EmployeeId], [Name], [Email], [Password], [Phone], [Address], [HireDate], [BaseSalary], [IsAdmin], [IsActive], [DepartmentId], [ShiftId], [EmployeeProfileId]) VALUES 
('EMP001', 'Juan Pérez', 'juan.perez@empresa.com', '123456', '555-0001', 'Calle Principal 123', GETDATE()-730, 50000, 0, 1, 1, 1, 1),
('EMP002', 'María García', 'maria.garcia@empresa.com', '123456', '555-0002', 'Avenida Central 456', GETDATE()-365, 60000, 0, 1, 2, 1, 2),
('EMP003', 'Carlos López', 'carlos.lopez@empresa.com', '123456', '555-0003', 'Plaza Mayor 789', GETDATE()-180, 45000, 0, 1, 3, 2, 1);

