-- ============================================
-- LAMA MOTOTURISMO - DATOS DE PRUEBA
-- Ejecutar después de schema.sql para testing
-- ============================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

-- ============================================
-- CHAPTER (Capítulo de prueba)
-- ============================================
INSERT INTO [dbo].[Chapters] ([Name], [Country], [IsActive])
VALUES 
    (N'Capítulo Pereira', N'Colombia', 1),
    (N'Capítulo Bogotá', N'Colombia', 1),
    (N'Capítulo Medellín', N'Colombia', 1),
    (N'Capítulo México', N'México', 1);

-- ============================================
-- MEMBERS (Miembros de prueba)
-- ============================================
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible], [Continent])
VALUES 
    (1, 1, N'Juan Carlos Pérez López', 0, N'Colombia', 2015, N'ACTIVE', 1, N'Americas'),
    (1, 2, N'María Elena Rodríguez García', 1, N'Colombia', 2018, N'ACTIVE', 1, N'Americas'),
    (1, 3, N'Carlos Andrés Moreno Díaz', 0, N'Colombia', 2016, N'ACTIVE', 1, N'Americas'),
    (2, 4, N'Ricardo Luis González Sánchez', 0, N'Colombia', 2017, N'ACTIVE', 1, N'Americas'),
    (3, 5, N'Ana María Velasco Torres', 1, N'Colombia', 2019, N'ACTIVE', 1, N'Americas'),
    (4, 6, N'Francisco López Martínez', 0, N'México', 2020, N'ACTIVE', 1, N'Americas'),
    (1, 7, N'Pedro Ramírez Castillo', 0, N'Colombia', 2021, N'ACTIVE', 1, N'Americas');

-- ============================================
-- VEHICLES (Vehículos de prueba)
-- Máximo 2 activos por miembro
-- ============================================
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [Motorcycle Data], [Lic Plate], [Trike], [Photography], 
     [OdometerUnit], [IsActiveForChampionship])
VALUES
    -- Miembro 1: 2 motos
    (1, N'Honda CB500X 2022', N'ABC-001', 0, N'PENDING', N'Miles', 1),
    (1, N'Kawasaki Ninja 400 2021', N'ABC-002', 0, N'PENDING', N'Miles', 1),
    
    -- Miembro 2: 1 moto
    (2, N'BMW R1200GS Adventure 2023', N'ABC-003', 0, N'PENDING', N'Kilometers', 1),
    
    -- Miembro 3: 2 motos
    (3, N'Harley-Davidson Street 750 2020', N'ABC-004', 0, N'PENDING', N'Miles', 1),
    (3, N'Yamaha MT-07 2022', N'ABC-005', 0, N'PENDING', N'Kilometers', 1),
    
    -- Miembro 4: 1 moto
    (4, N'Ducati Monster 2021', N'ABC-006', 0, N'PENDING', N'Miles', 1),
    
    -- Miembro 5: Triciclo
    (5, N'Can-Am Spyder RT 2023', N'ABC-007', 1, N'PENDING', N'Miles', 1),
    
    -- Miembro 6: 2 motos
    (6, N'Honda CB 500 2019', N'ABC-008', 0, N'PENDING', N'Miles', 1),
    (6, N'Suzuki GSX-S 1000 2020', N'ABC-009', 0, N'PENDING', N'Kilometers', 1);

-- ============================================
-- EVENTS (Eventos de prueba)
-- ============================================
INSERT INTO [dbo].[Events]
    ([ChapterId], [Order], [Event Start Date (AAAA/MM/DD)], [Name of the event ], 
     [Class], [Mileage], [Points per event], [Points per Distance], [Points awarded per member],
     [StartLocationCountry], [StartLocationContinent], [EndLocationCountry], [EndLocationContinent])
VALUES
    -- Evento 1: Rally local, distancia corta
    (1, 1, '2026-02-14', N'Rally de Bienvenida 2026', 1, 150.0, 1, 0, 1,
     N'Colombia', N'Americas', N'Colombia', N'Americas'),
    
    -- Evento 2: Rally mediano, distancia media
    (1, 2, N'2026-03-20', N'Pereira a Salento y vuelta', 2, 350.0, 3, 1, 4,
     N'Colombia', N'Americas', N'Colombia', N'Americas'),
    
    -- Evento 3: Rally importante, distancia larga
    (1, 3, N'2026-04-10', N'Gran Rally Pereira - Bogotá - Pereira', 3, 900.0, 5, 2, 7,
     N'Colombia', N'Americas', N'Colombia', N'Americas'),
    
    -- Evento 4: Rally de clase alta, distancia larga
    (2, 4, N'2026-05-15', N'Ruta Internacional Colombia - Panamá', 4, 1200.0, 10, 2, 12,
     N'Colombia', N'Americas', N'Panamá', N'Americas'),
    
    -- Evento 5: Rally épico, distancia muy larga, visitantes internacionales
    (1, 5, N'2026-06-20', N'Aventura Norte América', 5, 2500.0, 15, 2, 17,
     N'Colombia', N'Americas', N'Canadá', N'Americas');

-- ============================================
-- ATTENDANCE (Asistencias - Pending)
-- Para testing del endpoint
-- ============================================
INSERT INTO [dbo].[Attendance]
    ([EventId], [MemberId], [VehicleId], [Status])
VALUES
    -- Evento 1
    (1, 1, 1, N'PENDING'),
    (1, 2, 3, N'PENDING'),
    (1, 3, 4, N'PENDING'),
    (1, 4, 6, N'PENDING'),
    (1, 5, 7, N'PENDING'),
    
    -- Evento 2
    (2, 1, 1, N'PENDING'),
    (2, 2, 3, N'PENDING'),
    (2, 3, 4, N'PENDING'),
    (2, 6, 8, N'PENDING'),
    
    -- Evento 3
    (3, 1, 2, N'PENDING'),
    (3, 2, 3, N'PENDING'),
    (3, 7, 9, N'PENDING'),
    
    -- Evento 4
    (4, 4, 6, N'PENDING'),
    (4, 6, 8, N'PENDING'),
    
    -- Evento 5
    (5, 1, 1, N'PENDING'),
    (5, 2, 3, N'PENDING'),
    (5, 6, 9, N'PENDING');

-- ============================================
-- VERIFICACIÓN
-- ============================================
PRINT 'Datos de prueba insertados exitosamente.';
PRINT '';
PRINT 'Resumen:';
PRINT '  Chapters: ' + CAST((SELECT COUNT(*) FROM [dbo].[Chapters]) AS NVARCHAR(10));
PRINT '  Members: ' + CAST((SELECT COUNT(*) FROM [dbo].[Members]) AS NVARCHAR(10));
PRINT '  Vehicles: ' + CAST((SELECT COUNT(*) FROM [dbo].[Vehicles]) AS NVARCHAR(10));
PRINT '  Events: ' + CAST((SELECT COUNT(*) FROM [dbo].[Events]) AS NVARCHAR(10));
PRINT '  Attendance (Pending): ' + CAST((SELECT COUNT(*) FROM [dbo].[Attendance]) AS NVARCHAR(10));

COMMIT TRANSACTION;

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    THROW;
END CATCH;

-- ============================================
-- CONSULTAS DE VERIFICACIÓN
-- ============================================

-- Ver miembros con sus vehículos
SELECT 
    m.[Id],
    m.[Complete Names],
    m.[Country Birth],
    COUNT(v.[Id]) AS [Active Vehicles]
FROM [dbo].[Members] m
LEFT JOIN [dbo].[Vehicles] v ON m.[Id] = v.[MemberId] AND v.[IsActiveForChampionship] = 1
GROUP BY m.[Id], m.[Complete Names], m.[Country Birth]
ORDER BY m.[Order];

-- Ver eventos programados
SELECT 
    [Order],
    [Name of the event ],
    [Class],
    FORMAT([Event Start Date (AAAA/MM/DD)], 'yyyy-MM-dd') AS [Fecha],
    [Mileage],
    (SELECT COUNT(*) FROM [dbo].[Attendance] WHERE [EventId] = [Events].[Id]) AS [Attendees]
FROM [dbo].[Events]
ORDER BY [Event Start Date (AAAA/MM/DD)];

-- Ver asistencias pendientes
SELECT 
    a.[Id],
    m.[Complete Names],
    e.[Name of the event ],
    a.[Status]
FROM [dbo].[Attendance] a
JOIN [dbo].[Members] m ON a.[MemberId] = m.[Id]
JOIN [dbo].[Events] e ON a.[EventId] = e.[Id]
WHERE a.[Status] = N'PENDING'
ORDER BY e.[Event Start Date (AAAA/MM/DD)];
