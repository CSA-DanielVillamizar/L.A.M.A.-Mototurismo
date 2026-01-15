-- ============================================
-- LAMA MOTOTURISMO - SQL SERVER SCHEMA
-- ============================================

-- ============================================
-- 1. CHAPTERS
-- ============================================
CREATE TABLE [dbo].[Chapters] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(255) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
);

-- ============================================
-- 2. MEMBERS
-- ============================================
CREATE TABLE [dbo].[Members] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [ChapterId] INT NOT NULL,
    [Order] INT NOT NULL,
    [Complete Names] NVARCHAR(255) NOT NULL,
    [Dama] BIT NOT NULL DEFAULT 0,
    [Country Birth] NVARCHAR(100) NULL,
    [In Lama Since] INT NULL, -- Year
    [STATUS] NVARCHAR(50) NOT NULL DEFAULT 'ACTIVE', -- ACTIVE, INACTIVE
    [is_eligible] BIT NOT NULL DEFAULT 1,
    [Continent] NVARCHAR(100) NULL, -- Africa, Americas, Asia, Europe, Oceania
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_Members_Chapters] FOREIGN KEY ([ChapterId]) REFERENCES [dbo].[Chapters]([Id])
);

CREATE INDEX [IX_Members_ChapterId] ON [dbo].[Members]([ChapterId]);

-- ============================================
-- 3. VEHICLES
-- ============================================
CREATE TABLE [dbo].[Vehicles] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [MemberId] INT NOT NULL,
    [Motorcycle Data] NVARCHAR(MAX) NOT NULL,
    [Lic Plate] NVARCHAR(50) NOT NULL UNIQUE,
    [Trike] BIT NOT NULL DEFAULT 0,
    [Photography] NVARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, VALIDATED
    [StartYearEvidenceUrl] NVARCHAR(MAX) NULL,
    [CutOffEvidenceUrl] NVARCHAR(MAX) NULL,
    [StartYearEvidenceValidatedAt] DATETIME2 NULL,
    [CutOffEvidenceValidatedAt] DATETIME2 NULL,
    [EvidenceValidatedBy] INT NULL,
    [OdometerUnit] NVARCHAR(20) NOT NULL, -- Miles, Kilometers
    [Starting Odometer] FLOAT NULL,
    [Final Odometer] FLOAT NULL,
    [Starting Odometer Date] DATE NULL,
    [Final Odometer Date] DATE NULL,
    [IsActiveForChampionship] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_Vehicles_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Vehicles_MemberId] ON [dbo].[Vehicles]([MemberId]);
CREATE INDEX [IX_Vehicles_IsActive] ON [dbo].[Vehicles]([IsActiveForChampionship]);

-- ============================================
-- 4. EVENTS
-- ============================================
CREATE TABLE [dbo].[Events] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [ChapterId] INT NOT NULL,
    [Order] INT NOT NULL,
    [Event Start Date (AAAA/MM/DD)] DATE NOT NULL,
    [Name of the event ] NVARCHAR(255) NOT NULL,
    [Class] INT NOT NULL, -- 1, 2, 3, 4, 5
    [Mileage] FLOAT NOT NULL, -- one-way distance in Miles
    [Points per event] INT NOT NULL,
    [Points per Distance] INT NOT NULL,
    [Points awarded per member] INT NOT NULL,
    [StartLocationCountry] NVARCHAR(100) NULL,
    [StartLocationContinent] NVARCHAR(100) NULL,
    [EndLocationCountry] NVARCHAR(100) NULL,
    [EndLocationContinent] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_Events_Chapters] FOREIGN KEY ([ChapterId]) REFERENCES [dbo].[Chapters]([Id])
);

CREATE INDEX [IX_Events_ChapterId] ON [dbo].[Events]([ChapterId]);

-- ============================================
-- 5. ATTENDANCE
-- ============================================
CREATE TABLE [dbo].[Attendance] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [EventId] INT NOT NULL,
    [MemberId] INT NOT NULL,
    [VehicleId] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, CONFIRMED, REJECTED
    [Points per event] INT NULL,
    [Points per Distance] INT NULL,
    [Points awarded per member] INT NULL,
    [Visitor Class] NVARCHAR(50) NULL, -- LOCAL, VISITOR_A, VISITOR_B
    [ConfirmedAt] DATETIME2 NULL,
    [ConfirmedBy] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_Attendance_Events] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Events]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Attendance_Members] FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Members]([Id]),
    CONSTRAINT [FK_Attendance_Vehicles] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([Id]),
    CONSTRAINT [UQ_Attendance_MemberEvent] UNIQUE ([EventId], [MemberId])
);

CREATE INDEX [IX_Attendance_EventId] ON [dbo].[Attendance]([EventId]);
CREATE INDEX [IX_Attendance_MemberId] ON [dbo].[Attendance]([MemberId]);
CREATE INDEX [IX_Attendance_Status] ON [dbo].[Attendance]([Status]);

-- ============================================
-- 6. CONFIGURATION
-- ============================================
CREATE TABLE [dbo].[Configuration] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [Key] NVARCHAR(255) NOT NULL UNIQUE,
    [Value] NVARCHAR(MAX) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Insert configuration defaults
INSERT INTO [dbo].[Configuration] ([Key], [Value], [Description])
VALUES
    ('DistanceThreshold_1Point_OneWayMiles', '200', 'Umbral de millas para 1 punto'),
    ('DistanceThreshold_2Points_OneWayMiles', '800', 'Umbral de millas para 2 puntos'),
    ('TripWindowDays_ABInheritance', '15', 'Ventana de d\u00edas para herencia A/B (futuro)'),
    ('ConversionFactor_KMToMiles', '0.621371', 'Factor de conversi\u00f3n KM a Millas'),
    ('PointsPerClassMultiplier_1', '1', 'Multiplicador de puntos para Class 1'),
    ('PointsPerClassMultiplier_2', '3', 'Multiplicador de puntos para Class 2'),
    ('PointsPerClassMultiplier_3', '5', 'Multiplicador de puntos para Class 3'),
    ('PointsPerClassMultiplier_4', '10', 'Multiplicador de puntos para Class 4'),
    ('PointsPerClassMultiplier_5', '15', 'Multiplicador de puntos para Class 5'),
    ('VisitorBonus_DifferentContinent', '2', 'Bonus para visitante de otro continente'),
    ('VisitorBonus_SameContinent', '1', 'Bonus para visitante del mismo continente');

-- ============================================
-- 7. TRIGGER: MAX 2 MOTOS ACTIVAS POR MIEMBRO
-- ============================================
CREATE TRIGGER [tr_MaxTwoActiveVehiclesPerMember]
ON [dbo].[Vehicles]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar que no haya m\u00e1s de 2 motos activas por miembro
    IF EXISTS (
        SELECT [MemberId]
        FROM [dbo].[Vehicles]
        WHERE [IsActiveForChampionship] = 1
        GROUP BY [MemberId]
        HAVING COUNT(*) > 2
    )
    BEGIN
        RAISERROR('Un miembro no puede tener m\u00e1s de 2 motos activas en el campeonato.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;

-- ============================================
-- 8. FOREIGN KEY: EvidenceValidatedBy -> Members
-- ============================================
ALTER TABLE [dbo].[Vehicles]
ADD CONSTRAINT [FK_Vehicles_EvidenceValidatedBy]
FOREIGN KEY ([EvidenceValidatedBy]) REFERENCES [dbo].[Members]([Id]);

ALTER TABLE [dbo].[Attendance]
ADD CONSTRAINT [FK_Attendance_ConfirmedBy]
FOREIGN KEY ([ConfirmedBy]) REFERENCES [dbo].[Members]([Id]);
