-- ============================================
-- LAMA MOTOTURISMO - MIGRATION SCRIPT
-- Auto-generado por migration_generator.py
-- ============================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

-- INSERT CHAPTERS
SET IDENTITY_INSERT [dbo].[Chapters] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[Chapters] WHERE [ChapterId] = 1)
    INSERT INTO [dbo].[Chapters] ([ChapterId], [Name], [Country], [CreatedAt])
    VALUES (1, 'PEREIRA CORTE NACIONAL', 'COLOMBIA', GETDATE());
SET IDENTITY_INSERT [dbo].[Chapters] OFF;

-- Deshabilitar triggers temporalmente
-- ALTER TABLE [dbo].[Vehicles] DISABLE TRIGGER [tr_MaxTwoActiveVehiclesPerMember];

-- ============================================
-- INSERT MEMBERS
-- ============================================
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 1, N'OSCAR ANGELO GARCIA BUITRAGO ', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 2, N'GUILLERMO OLAYA ACEVEDO  ', 
     N'COLOMBIA', 
     2017, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 3, N'DIANA ROJAS MUÃ‘OZ ', 
     N'COLOMBIA', 
     2017, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 4, N'HARVEY CRUZ PINZON ', 
     N'COLOMBIA', 
     2018, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 5, N'JOSE NICOLAS RAMIREZ ECHEVERRY  ', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 6, N'JHON JAIRO JIMENEZ GIRALDO ', 
     N'COLOMBIA', 
     2018, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 7, N'MARTIN ALONSO GIRALDO CATAÃ‘O ', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 8, N'WILLIAM GABRIEL OCAMPO SUAREZ ', 
     N'COLOMBIA', 
     2020, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 9, N'JORGE SOTO CARVAJAL ', 
     N'COLOMBIA', 
     2023, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 10, N'JORGE ROJAS ', 
     N'COLOMBIA', 
     2023, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 11, N'JUAN JOSÃ‰ GÃ“MEZ  ', 
     N'COLOMBIA', 
     2023, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 12, N'ALBERTO GUARIN ARREDONDO ', 
     N'COLOMBIA', 
     2023, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 13, N'CAMILO ANDRES MERCHAN CORREA ', 
     N'COLOMBIA', 
     2023, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 14, N'JEISSON STIVEN  MARIN VALENCIA ', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 15, N'LIBARDO SALGADO', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 16, N'ROBISON LARGO GARCIA', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 17, N'GERMANICO GUTIERREZ BETANCURT', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 18, N'LUISA FERNANDA VELA AGUDELO', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 19, N'URIEL PEÃ‘A TORRES', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 20, N'LUIS ORLANDO PALACIO OSORIO', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 21, N'LUIS CAMILO JOSE PEÃ‘A GARCIA', 
     N'COLOMBIA', 
     2024, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 22, N'CESAR AUGUSTO CASTRO OYOLA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 23, N'RICARDO TELLEZ MENDOZA', 
     N'COLOMBIA', 
     2018, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 24, N'GERARDO ANTONIO MARIN RAMIREZ', 
     N'COLOMBIA', 
     NULL, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 25, N'HERMAN ZULUAGA JARAMILLO', 
     N'COLOMBIA', 
     2018, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 26, N'EDWIN HERNANDEZ', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 27, N'LUIS FELIPE BEDOYA', 
     N'COLOMBIA', 
     2018, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 28, N'LIBARDO ANTONIO VARELA', 
     N'COLOMBIA', 
     2020, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, 29, N'PAOLA ANDREA RAIGOSA ARREDONDO', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);

-- ============================================
-- INSERT VEHICLES
-- ============================================
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 1 ORDER BY [MemberId] DESC), 
     N'BMW  1200 Adventure GS  2016', 
     N'VWJ200', 
     1, 'Miles',
     39929, 
     43930, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 2 ORDER BY [MemberId] DESC), 
     N'KAWASAKI VULCAN 900cc', 
     N'BRR-35', 
     1, 'Miles',
     39325, 
     40063, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 3 ORDER BY [MemberId] DESC), 
     N'YAMAHA CZD-300 A 2019', 
     N'EXR-26F', 
     1, 'Miles',
     15544, 
     16233, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 4 ORDER BY [MemberId] DESC), 
     N'TRIUMPH TIGER 800cc  2013', 
     N'BAH-88D', 
     1, 'Miles',
     94617, 
     97828, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 5 ORDER BY [MemberId] DESC), 
     N'SUZUKI V-STROM 650cc 2017', 
     N'NFP-55E', 
     1, 'Miles',
     NULL, 
     24211, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 6 ORDER BY [MemberId] DESC), 
     N'SUZUKI V-STROM 1000cc 2018', 
     N'SZD-58', 
     1, 'Miles',
     62220, 
     66656, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 7 ORDER BY [MemberId] DESC), 
     N'HONDA AFRICA TWIN 1100', 
     N'VVN96F', 
     1, 'Miles',
     12997, 
     17051, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 8 ORDER BY [MemberId] DESC), 
     N'SIN MOTO', 
     N'AUTO_ORD_8', 
     1, 'Miles',
     NULL, 
     NULL, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 9 ORDER BY [MemberId] DESC), 
     N'SUZUKI VSTROM  1050CC ', 
     N'KBJ04G', 
     1, 'Miles',
     11126, 
     18232, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 10 ORDER BY [MemberId] DESC), 
     N'SUZUKI VSTROM 650CC ABS 2017', 
     N'SXK06', 
     1, 'Miles',
     55894, 
     58385, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 11 ORDER BY [MemberId] DESC), 
     N'SIN MOTO', 
     N'AUTO_ORD_11', 
     1, 'Miles',
     NULL, 
     NULL, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 12 ORDER BY [MemberId] DESC), 
     N'SUZUKY VSTROM 650 XT 2021', 
     N'KGG56F', 
     1, 'Miles',
     12734, 
     19882, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 13 ORDER BY [MemberId] DESC), 
     N'SUZUKY VSTROM 650 ABS 2017', 
     N'ELK14E', 
     1, 'Miles',
     38802, 
     NULL, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 14 ORDER BY [MemberId] DESC), 
     N'BMW F750 GS', 
     N'UZF08G', 
     1, 'Miles',
     5786, 
     13905, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 15 ORDER BY [MemberId] DESC), 
     N'BMW R1200', 
     N'BUO69E', 
     1, 'Miles',
     39432, 
     42238, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 16 ORDER BY [MemberId] DESC), 
     N'SUZUKY VSTROM 650 ABS 2017', 
     N'CNC67E', 
     1, 'Miles',
     60428, 
     66549, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 17 ORDER BY [MemberId] DESC), 
     N'SUZUKI VSTROM DL650', 
     N'ITT99D', 
     1, 'Miles',
     89059, 
     106088, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 18 ORDER BY [MemberId] DESC), 
     N'AKT ADVENTURE 250', 
     N'XGL29D', 
     1, 'Miles',
     44018, 
     58679, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 19 ORDER BY [MemberId] DESC), 
     N'KAWASAKI  VERSYS 1000', 
     N'RPG59C', 
     1, 'Miles',
     NULL, 
     36601, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 20 ORDER BY [MemberId] DESC), 
     N'SUZUKI VSTROM DL650', 
     N'SVR07', 
     1, 'Miles',
     NULL, 
     35898, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 21 ORDER BY [MemberId] DESC), 
     N'BMW R18 1800CC', 
     N'GLF56G', 
     1, 'Miles',
     NULL, 
     NULL, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 22 ORDER BY [MemberId] DESC), 
     N'BMW R1200 GS', 
     N'BYY14E', 
     1, 'Miles',
     28415, 
     31410, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 23 ORDER BY [MemberId] DESC), 
     N'SUZUKI 1000 DL', 
     N'NKN52D', 
     1, 'Miles',
     58629, 
     61141, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 24 ORDER BY [MemberId] DESC), 
     N'SUZUKI DL650 XT', 
     N'XKK99F', 
     1, 'Miles',
     36746, 
     39995, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 25 ORDER BY [MemberId] DESC), 
     N'BMW R1200 GS', 
     N'DSI46D', 
     1, 'Miles',
     52028, 
     54503, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 26 ORDER BY [MemberId] DESC), 
     N'YAMAHA TRACER 900', 
     N'EDI94F', 
     1, 'Miles',
     18221, 
     19463, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 27 ORDER BY [MemberId] DESC), 
     N'KAWASAKI VOYAGER VII 1200', 
     N'439G4S', 
     1, 'Miles',
     NULL, 
     18234, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 28 ORDER BY [MemberId] DESC), 
     N'SUZUKI VSTROM 650', 
     N'HOT21A', 
     1, 'Miles',
     64210, 
     66656, 
     'PENDING', 1);
INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = 29 ORDER BY [MemberId] DESC), 
     N'SUZUKI GIXXER 250', 
     N'GMX10G', 
     1, 'Miles',
     NULL, 
     12264, 
     'PENDING', 1);

-- Habilitar triggers nuevamente
-- ALTER TABLE [dbo].[Vehicles] ENABLE TRIGGER [tr_MaxTwoActiveVehiclesPerMember];

COMMIT TRANSACTION;
PRINT 'Migracion completada exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'ERROR en migracion: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
