-- Script para cargar datos de prueba en la base de datos
-- Ejecutar en SQL Server contra la base de datos LamaDb

DECLARE @TenantId UNIQUEIDENTIFIER = NEWID()

-- STEP 1: Insert Chapters (sin TenantId)
INSERT INTO Chapters ([Name], Country, CreatedAt)
VALUES 
    ('LAMA COR - REGIÓN NORTE', 'Colombia', GETUTCDATE()),
    ('LAMA COR - BOGOTÁ', 'Colombia', GETUTCDATE()),
    ('LAMA COR - MEDELLÍN', 'Colombia', GETUTCDATE())

-- STEP 2: Get Chapter IDs
DECLARE @ChapterId1 INT = (SELECT MAX(Id) - 2 FROM Chapters)
DECLARE @ChapterId2 INT = (SELECT MAX(Id) - 1 FROM Chapters)
DECLARE @ChapterId3 INT = (SELECT MAX(Id) FROM Chapters)

-- STEP 3: Insert Members Status Types
INSERT INTO MemberStatusTypes (StatusName, IsActive, CreatedAt)
SELECT 'ACTIVE', 1, GETUTCDATE() WHERE NOT EXISTS(SELECT 1 FROM MemberStatusTypes WHERE StatusName='ACTIVE')

-- STEP 4: Get Status ID
DECLARE @StatusId INT = (SELECT TOP 1 StatusId FROM MemberStatusTypes WHERE StatusName='ACTIVE')

-- STEP 5: Insert Members (con nombres de columna correctos con espacios)
INSERT INTO Members (TenantId, ChapterId, [Complete Names], [STATUS], MemberStatusTypeStatusId, [Order], CreatedAt, UpdatedAt)
VALUES 
    (@TenantId, @ChapterId1, 'Juan Pérez García', 'ACTIVE', @StatusId, 1, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 'María Rodríguez López', 'ACTIVE', @StatusId, 2, GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 'Carlos González Martínez', 'ACTIVE', @StatusId, 3, GETUTCDATE(), GETUTCDATE())

-- STEP 6: Get Member IDs
DECLARE @MemberId1 INT = (SELECT MAX(Id) - 2 FROM Members)
DECLARE @MemberId2 INT = (SELECT MAX(Id) - 1 FROM Members)
DECLARE @MemberId3 INT = (SELECT MAX(Id) FROM Members)

-- STEP 7: Insert Events (con nombres de columna exactos)
INSERT INTO Events (TenantId, ChapterId, [Name of the event], [Event Start Date (AAAA/MM/DD)], CreatedAt, UpdatedAt)
VALUES 
    (@TenantId, @ChapterId1, 'Rodada Inicial 2025', CAST(GETUTCDATE() AS DATE), GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 'Validación de Evidencias', CAST(DATEADD(day, 7, GETUTCDATE()) AS DATE), GETUTCDATE(), GETUTCDATE()),
    (@TenantId, @ChapterId1, 'Encuentro Regional', CAST(DATEADD(day, 14, GETUTCDATE()) AS DATE), GETUTCDATE(), GETUTCDATE())

-- STEP 8: Get Event IDs
DECLARE @EventId1 INT = (SELECT MAX(Id) - 2 FROM Events)
DECLARE @EventId2 INT = (SELECT MAX(Id) - 1 FROM Events)
DECLARE @EventId3 INT = (SELECT MAX(Id) FROM Events)

PRINT 'Datos de prueba cargados exitosamente:'
PRINT 'TenantId=' + CAST(@TenantId AS VARCHAR(50))
PRINT 'ChapterId1=' + CAST(@ChapterId1 AS VARCHAR(10)) 
PRINT 'MemberId1=' + CAST(@MemberId1 AS VARCHAR(10))
PRINT 'EventId1=' + CAST(@EventId1 AS VARCHAR(10))
