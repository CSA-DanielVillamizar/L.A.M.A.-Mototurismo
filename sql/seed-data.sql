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
-- Chapters no tienen TenantId, los dejamos para evitar conflictos

-- STEP 1: Insert Chapters (sin TenantId) - idempotente
IF NOT EXISTS (SELECT 1 FROM Chapters WHERE [Name] = N'LAMA COR - REGIÃ“N NORTE')
BEGIN
    INSERT INTO Chapters ([Name], Country, CreatedAt)
    VALUES
        (N'LAMA COR - REGIÃ“N NORTE', N'Colombia', GETUTCDATE()),
        (N'LAMA COR - BOGOTÃ', N'Colombia', GETUTCDATE()),
        (N'LAMA COR - MEDELLÃN', N'Colombia', GETUTCDATE())
END

-- STEP 2: Get Chapter IDs
DECLARE @ChapterId1 INT = (SELECT TOP 1 Id FROM Chapters WHERE [Name] = N'LAMA COR - REGIÃ“N NORTE')
DECLARE @ChapterId2 INT = (SELECT TOP 1 Id FROM Chapters WHERE [Name] = N'LAMA COR - BOGOTÃ')
DECLARE @ChapterId3 INT = (SELECT TOP 1 Id FROM Chapters WHERE [Name] = N'LAMA COR - MEDELLÃN')

-- STEP 3: Insert Members Status Types
INSERT INTO MemberStatusTypes (StatusName, Category, DisplayOrder, IsActive, CreatedAt)
SELECT N'ACTIVE', N'Regular', 1, 1, GETUTCDATE() WHERE NOT EXISTS(SELECT 1 FROM MemberStatusTypes WHERE StatusName=N'ACTIVE')

-- STEP 4: Get Status ID
DECLARE @StatusId INT = (SELECT TOP 1 StatusId FROM MemberStatusTypes WHERE StatusName=N'ACTIVE')

-- STEP 5: Insert Members (con nombres de columna correctos y normalized para bÃºsqueda)
INSERT INTO Members (TenantId, ChapterId, [ Complete Names], CompleteNamesNormalized, [STATUS], MemberStatusTypeStatusId, [Order], CreatedAt, UpdatedAt)
VALUES
    (@TenantId, @ChapterId1, N'Juan PÃ©rez GarcÃ­a', N'JUAN PEREZ GARCIA', N'ACTIVE', @StatusId, 1, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, N'MarÃ­a RodrÃ­guez LÃ³pez', N'MARIA RODRIGUEZ LOPEZ', N'ACTIVE', @StatusId, 2, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, N'Carlos GonzÃ¡lez MartÃ­nez', N'CARLOS GONZALEZ MARTINEZ', N'ACTIVE', @StatusId, 3, GETUTCDATE(), GETUTCDATE())

-- STEP 6: Get Member IDs
DECLARE @MemberId1 INT = (SELECT MAX(Id) - 2 FROM Members)
DECLARE @MemberId2 INT = (SELECT MAX(Id) - 1 FROM Members)
DECLARE @MemberId3 INT = (SELECT MAX(Id) FROM Members)

-- STEP 7: Insert Events (con todas las columnas NOT NULL)
INSERT INTO Events (
    TenantId, ChapterId, [Order], [Name of the event], [Event Start Date (AAAA/MM/DD)],
    [Class], [Mileage], [Points per event], [Points per Distance], [Points awarded per member],
    CreatedAt, UpdatedAt
)
VALUES
    (@TenantId, @ChapterId1, 1, N'Rodada Inicial 2025', CAST(GETUTCDATE() AS DATE), 1, 100, 10, 0.1, 15, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 2, N'ValidaciÃ³n de Evidencias', CAST(DATEADD(day, 7, GETUTCDATE()) AS DATE), 2, 50, 5, 0.05, 10, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 3, N'Encuentro Regional', CAST(DATEADD(day, 14, GETUTCDATE()) AS DATE), 1, 200, 20, 0.2, 25, GETUTCDATE(), GETUTCDATE())

-- STEP 8: Get Event IDs
DECLARE @EventId1 INT = (SELECT MAX(Id) - 2 FROM Events)
DECLARE @EventId2 INT = (SELECT MAX(Id) - 1 FROM Events)
DECLARE @EventId3 INT = (SELECT MAX(Id) FROM Events)

PRINT 'Datos de prueba cargados exitosamente:'
PRINT 'TenantId=' + CAST(@TenantId AS VARCHAR(50))
PRINT 'ChapterId1=' + CAST(@ChapterId1 AS VARCHAR(10))
PRINT 'MemberId1=' + CAST(@MemberId1 AS VARCHAR(10))
PRINT 'EventId1=' + CAST(@EventId1 AS VARCHAR(10))
