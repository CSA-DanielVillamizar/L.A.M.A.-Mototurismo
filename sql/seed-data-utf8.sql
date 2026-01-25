-- Script para cargar datos de prueba en la base de datos
-- Ejecutar en SQL Server contra la base de datos LamaDb
-- TenantId debe coincidir con TenantContext.DefaultTenantId (00000000-0000-0000-0000-000000000001)

SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO

DECLARE @TenantId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'

-- Limpiar datos previos del tenant por defecto (DEV mode)
DELETE FROM Events WHERE TenantId = @TenantId
DELETE FROM Members WHERE TenantId = @TenantId

-- STEP 1: Insert Chapters (sin TenantId) - idempotente
IF NOT EXISTS (SELECT 1 FROM Chapters WHERE [Name] = N'LAMA COR - REGIÓN NORTE')
BEGIN
    INSERT INTO Chapters ([Name], Country, CreatedAt)
    VALUES
        (N'LAMA COR - REGIÓN NORTE', N'Colombia', GETUTCDATE()),
        (N'LAMA COR - BOGOTÁ', N'Colombia', GETUTCDATE()),
        (N'LAMA COR - MEDELLÍN', N'Colombia', GETUTCDATE())
END

-- STEP 2: Get Chapter IDs
DECLARE @ChapterId1 INT = (SELECT TOP 1 Id FROM Chapters WHERE [Name] = N'LAMA COR - REGIÓN NORTE')
DECLARE @ChapterId2 INT = (SELECT TOP 1 Id FROM Chapters WHERE [Name] = N'LAMA COR - BOGOTÁ')
DECLARE @ChapterId3 INT = (SELECT TOP 1 Id FROM Chapters WHERE [Name] = N'LAMA COR - MEDELLÍN')

-- STEP 3: Insert Members Status Types
INSERT INTO MemberStatusTypes (StatusName, Category, DisplayOrder, IsActive, CreatedAt)
SELECT N'ACTIVE', N'Regular', 1, 1, GETUTCDATE() WHERE NOT EXISTS(SELECT 1 FROM MemberStatusTypes WHERE StatusName=N'ACTIVE')

-- STEP 4: Get Status ID
DECLARE @StatusId INT = (SELECT TOP 1 StatusId FROM MemberStatusTypes WHERE StatusName=N'ACTIVE')

-- STEP 5: Insert Members
INSERT INTO Members (TenantId, ChapterId, [ Complete Names], CompleteNamesNormalized, [STATUS], MemberStatusTypeStatusId, [Order], CreatedAt, UpdatedAt)
VALUES
    (@TenantId, @ChapterId1, N'Juan Pérez García', N'JUAN PEREZ GARCIA', N'ACTIVE', @StatusId, 1, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, N'María Rodríguez López', N'MARIA RODRIGUEZ LOPEZ', N'ACTIVE', @StatusId, 2, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, N'Carlos González Martínez', N'CARLOS GONZALEZ MARTINEZ', N'ACTIVE', @StatusId, 3, GETUTCDATE(), GETUTCDATE())

-- STEP 6: Insert Events
INSERT INTO Events (
    TenantId, ChapterId, [Order], [Name of the event], [Event Start Date (AAAA/MM/DD)], 
    [Class], [Mileage], [Points per event], [Points per Distance], [Points awarded per member],
    CreatedAt, UpdatedAt
)
VALUES
    (@TenantId, @ChapterId1, 1, N'Rodada Inicial 2025', CAST(GETUTCDATE() AS DATE), 1, 100, 10, 0.1, 15, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 2, N'Validación de Evidencias', CAST(DATEADD(day, 7, GETUTCDATE()) AS DATE), 2, 50, 5, 0.05, 10, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 3, N'Encuentro Regional', CAST(DATEADD(day, 14, GETUTCDATE()) AS DATE), 1, 200, 20, 0.2, 25, GETUTCDATE(), GETUTCDATE())

PRINT 'Datos de prueba cargados exitosamente con UTF-8'
PRINT 'TenantId=' + CAST(@TenantId AS VARCHAR(50))