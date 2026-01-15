-- ============================================================================
-- Test Data para L.A.M.A. Mototurismo
-- Datos de prueba para validar toda la funcionalidad
-- ============================================================================

USE [LamaDb];
GO

-- Limpiar datos existentes (mantener estructura)
DELETE FROM [dbo].[Attendance];
DELETE FROM [dbo].[Events];
DELETE FROM [dbo].[Vehicles];
DELETE FROM [dbo].[Members];
DELETE FROM [dbo].[Chapters];
GO

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT 'LOADING TEST DATA';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';

-- ============================================================================
-- INSERT: Chapters
-- ============================================================================

PRINT '[1] Insertando Capítulos...';

INSERT INTO [dbo].[Chapters] ([Name], [Country], [CreatedAt])
VALUES 
    ('PEREIRA CORTE NACIONAL', 'Colombia', GETUTCDATE()),
    ('BOGOTÁ', 'Colombia', GETUTCDATE()),
    ('MEDELLÍN', 'Colombia', GETUTCDATE()),
    ('CALI', 'Colombia', GETUTCDATE());

DECLARE @ChapterId_Pereira INT = (SELECT TOP 1 ChapterId FROM Chapters WHERE [Name] = 'PEREIRA CORTE NACIONAL');
DECLARE @ChapterId_Bogota INT = (SELECT TOP 1 ChapterId FROM Chapters WHERE [Name] = 'BOGOTÁ');

PRINT '✓ 4 capítulos insertados';
PRINT '';

-- ============================================================================
-- INSERT: Members (miembros con 2 motos)
-- ============================================================================

PRINT '[2] Insertando Miembros...';

INSERT INTO [dbo].[Members] ([ChapterId], [Complete Names], [Country Birth], [Continent], [In Lama Since], [STATUS], [is_eligible], [Order], [CreatedAt])
VALUES 
    (@ChapterId_Pereira, 'Germánico García', 'Colombia', 'South America', 2015, 'ACTIVE', 1, 1, GETUTCDATE()),
    (@ChapterId_Pereira, 'Carlos Pérez', 'Colombia', 'South America', 2018, 'ACTIVE', 1, 2, GETUTCDATE()),
    (@ChapterId_Pereira, 'Juan González', 'Colombia', 'South America', 2016, 'ACTIVE', 1, 3, GETUTCDATE()),
    (@ChapterId_Bogota, 'Pedro Martínez', 'Ecuador', 'South America', 2017, 'ACTIVE', 1, 4, GETUTCDATE()),
    (@ChapterId_Bogota, 'Miguel López', 'USA', 'North America', 2019, 'ACTIVE', 1, 5, GETUTCDATE()),
    (@ChapterId_Bogota, 'Francisco Rodríguez', 'Brazil', 'South America', 2014, 'ACTIVE', 1, 6, GETUTCDATE()),
    (@ChapterId_Pereira, 'Hernán Ruiz', 'Colombia', 'South America', 2016, 'ACTIVE', 1, 7, GETUTCDATE());

DECLARE @Germanico INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Germánico García');
DECLARE @Carlos INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Carlos Pérez');
DECLARE @Juan INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Juan González');
DECLARE @Pedro INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Pedro Martínez');
DECLARE @Miguel INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Miguel López');
DECLARE @Francisco INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Francisco Rodríguez');
DECLARE @Hernan INT = (SELECT TOP 1 MemberId FROM Members WHERE [Complete Names] = 'Hernán Ruiz');

PRINT '✓ 7 miembros insertados';
PRINT '';

-- ============================================================================
-- INSERT: Vehicles (2 motos para Germánico con diferentes unidades)
-- ============================================================================

PRINT '[3] Insertando Vehículos (prueba multi-moto)...';

-- Moto 1: Germánico - Miles (3000 millas)
INSERT INTO [dbo].[Vehicles] ([MemberId], [Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
                               [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship], [CreatedAt])
VALUES 
    (@Germanico, 'Harley Davidson Street 750', 'HDS-001', 0, 'Miles', 0, 3000, 'PENDING', 1, GETUTCDATE());

-- Moto 2: Germánico - Kilometers (5000 KM = 3106.855 millas aprox)
INSERT INTO [dbo].[Vehicles] ([MemberId], [Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
                               [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship], [CreatedAt])
VALUES 
    (@Germanico, 'Honda CB 500F', 'HCB-500', 0, 'Kilometers', 0, 5000, 'PENDING', 1, GETUTCDATE());

-- Otras motos para otros miembros
INSERT INTO [dbo].[Vehicles] ([MemberId], [Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
                               [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship], [CreatedAt])
VALUES 
    (@Carlos, 'Yamaha YZF R3', 'YZF-001', 0, 'Miles', 0, 2500, 'PENDING', 1, GETUTCDATE()),
    (@Carlos, 'Kawasaki Ninja 400', 'KNI-400', 0, 'Miles', 0, 2800, 'PENDING', 1, GETUTCDATE()),
    (@Juan, 'Suzuki GSX-S 125', 'GSX-125', 0, 'Miles', 0, 1800, 'PENDING', 1, GETUTCDATE()),
    (@Juan, 'KTM Duke 390', 'KTM-390', 0, 'Kilometers', 0, 3200, 'PENDING', 1, GETUTCDATE()),
    (@Pedro, 'Royal Enfield Classic', 'REC-001', 0, 'Miles', 0, 2200, 'PENDING', 1, GETUTCDATE()),
    (@Miguel, 'Harley Davidson Road King', 'HDR-001', 0, 'Miles', 0, 5500, 'PENDING', 1, GETUTCDATE()),
    (@Francisco, 'BMW F 850 GS', 'BMW-850', 0, 'Kilometers', 0, 6000, 'PENDING', 1, GETUTCDATE());

DECLARE @Moto_Germanico_1 INT = (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'HDS-001');
DECLARE @Moto_Germanico_2 INT = (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'HCB-500');

PRINT '✓ 9 vehículos insertados';
PRINT '';

-- ============================================================================
-- INSERT: Events (5 eventos del campeonato)
-- ============================================================================

PRINT '[4] Insertando Eventos...';

INSERT INTO [dbo].[Events] ([ChapterId], [Event Start Date (AAAA/MM/DD)], [Name of the event], [Class], 
                             [Mileage], [Points per event], [Points per Distance], [Points awarded per member], 
                             [Country], [Continent], [CreatedAt])
VALUES 
    (@ChapterId_Pereira, '2026-02-14', 'Desafío Pereira 2026', 3, 500, 5, 1, 6, 'Colombia', 'South America', GETUTCDATE()),
    (@ChapterId_Pereira, '2026-03-01', 'Ruta por el Café', 2, 199, 3, 0, 3, 'Colombia', 'South America', GETUTCDATE()),
    (@ChapterId_Pereira, '2026-03-15', 'Circuito Largo', 5, 850, 15, 2, 17, 'Colombia', 'South America', GETUTCDATE()),
    (@ChapterId_Bogota, '2026-02-28', 'Bogotá Challenge', 1, 120, 1, 0, 1, 'Colombia', 'South America', GETUTCDATE()),
    (@ChapterId_Bogota, '2026-04-10', 'Gran Viaje Internacional', 4, 320, 10, 1, 11, 'Ecuador', 'South America', GETUTCDATE());

DECLARE @Event_Pereira_Feb INT = (SELECT TOP 1 EventId FROM Events WHERE [Name of the event] = 'Desafío Pereira 2026');
DECLARE @Event_Pereira_Mar01 INT = (SELECT TOP 1 EventId FROM Events WHERE [Name of the event] = 'Ruta por el Café');
DECLARE @Event_Pereira_Mar15 INT = (SELECT TOP 1 EventId FROM Events WHERE [Name of the event] = 'Circuito Largo');
DECLARE @Event_Bogota_Feb INT = (SELECT TOP 1 EventId FROM Events WHERE [Name of the event] = 'Bogotá Challenge');
DECLARE @Event_Ecuador INT = (SELECT TOP 1 EventId FROM Events WHERE [Name of the event] = 'Gran Viaje Internacional');

PRINT '✓ 5 eventos insertados';
PRINT '';

-- ============================================================================
-- INSERT: Attendance (registros PENDING para pruebas)
-- ============================================================================

PRINT '[5] Insertando Asistencias (PENDING)...';

INSERT INTO [dbo].[Attendance] ([EventId], [MemberId], [VehicleId], [Status], [CreatedAt])
VALUES 
    -- Evento Pereira Feb: Germánico con 2 motos
    (@Event_Pereira_Feb, @Germanico, @Moto_Germanico_1, 'PENDING', GETUTCDATE()),
    (@Event_Pereira_Feb, @Germanico, @Moto_Germanico_2, 'PENDING', GETUTCDATE()),
    -- Evento Pereira Feb: otros miembros
    (@Event_Pereira_Feb, @Carlos, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'YZF-001'), 'PENDING', GETUTCDATE()),
    (@Event_Pereira_Feb, @Juan, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'GSX-125'), 'PENDING', GETUTCDATE()),
    (@Event_Pereira_Feb, @Pedro, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'REC-001'), 'PENDING', GETUTCDATE()),
    
    -- Evento Pereira Mar01 (distancia <200: 0 puntos distancia)
    (@Event_Pereira_Mar01, @Carlos, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'YZF-001'), 'PENDING', GETUTCDATE()),
    (@Event_Pereira_Mar01, @Juan, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'GSX-125'), 'PENDING', GETUTCDATE()),
    
    -- Evento Pereira Mar15 (distancia >800: 2 puntos distancia)
    (@Event_Pereira_Mar15, @Germanico, @Moto_Germanico_1, 'PENDING', GETUTCDATE()),
    (@Event_Pereira_Mar15, @Carlos, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'KNI-400'), 'PENDING', GETUTCDATE()),
    
    -- Evento Bogotá Feb
    (@Event_Bogota_Feb, @Miguel, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'HDR-001'), 'PENDING', GETUTCDATE()),
    (@Event_Bogota_Feb, @Francisco, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'BMW-850'), 'PENDING', GETUTCDATE()),
    
    -- Evento Ecuador (visitante B: continente diferente para USA)
    (@Event_Ecuador, @Miguel, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'HDR-001'), 'PENDING', GETUTCDATE()),
    (@Event_Ecuador, @Pedro, (SELECT TOP 1 VehicleId FROM Vehicles WHERE [Lic Plate] = 'REC-001'), 'PENDING', GETUTCDATE());

PRINT '✓ 13 registros de asistencia insertados (status=PENDING)';
PRINT '';

-- ============================================================================
-- VERIFICATION QUERIES
-- ============================================================================

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT 'VERIFICATION';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';

PRINT '[CHECK 1] Miembros registrados:';
SELECT MemberId, [Complete Names], [Country Birth] FROM Members ORDER BY MemberId;
PRINT '';

PRINT '[CHECK 2] Vehículos por miembro:';
SELECT m.MemberId, m.[Complete Names], COUNT(v.VehicleId) AS VehicleCount 
FROM Members m 
LEFT JOIN Vehicles v ON m.MemberId = v.MemberId 
GROUP BY m.MemberId, m.[Complete Names]
ORDER BY m.MemberId;
PRINT '';

PRINT '[CHECK 3] Asistencias pendientes por evento:';
SELECT 
    e.EventId,
    e.[Name of the event],
    e.Mileage,
    COUNT(a.AttendanceId) AS PendingCount
FROM Events e
LEFT JOIN Attendance a ON e.EventId = a.EventId AND a.Status = 'PENDING'
GROUP BY e.EventId, e.[Name of the event], e.Mileage
ORDER BY e.EventId;
PRINT '';

PRINT '[CHECK 4] Vista: vw_MasterOdometerReport (Germánico - 2 motos)';
SELECT VehicleId, MemberName, [Lic Plate], [OdometerUnit], [Starting Odometer Original], [Final Odometer Original], [Total Miles Traveled]
FROM vw_MasterOdometerReport
WHERE MemberId = @Germanico
ORDER BY VehicleId;
PRINT '';

PRINT '[CHECK 5] Vista: vw_MemberGeneralRanking (Germánico total)';
SELECT MemberId, [Complete Names], [Total Miles All Vehicles], [Vehicles Breakdown], [Active Vehicles Count]
FROM vw_MemberGeneralRanking
WHERE MemberId = @Germanico;
PRINT '';

PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '✅ TEST DATA LOADED SUCCESSFULLY';
PRINT '═══════════════════════════════════════════════════════════════════════';
PRINT '';
PRINT 'Estadísticas:';
PRINT '  - Capítulos: 4';
PRINT '  - Miembros: 7';
PRINT '  - Vehículos: 9';
PRINT '  - Eventos: 5';
PRINT '  - Asistencias (PENDING): 13';
PRINT '';
PRINT 'CASO DE PRUEBA CLAVE:';
PRINT '  Germánico García tiene 2 motos:';
PRINT '    - Moto 1: HDS-001 (3000 Miles)';
PRINT '    - Moto 2: HCB-500 (5000 KM → 3106.855 Miles aprox.)';
PRINT '    - TOTAL en vista: 6106.855 Miles';
PRINT '';
PRINT 'Próximo paso: Ejecutar pruebas funcionales (validar puntos, transacciones, etc.)';
PRINT '';
GO
