-- IMPORTACIÓN SABANA CORTE NACIONAL
SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 110, N'Luis Fernando  Peralta Franco',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 111, N'Jaime Alberto Botero Ospina',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 112, N'Carlos Angel Chaparro Lozano',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 113, N'Jorge Alberto Valencia Callejas',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 114, N'Freddy Raúl Bonilla Muñoz',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 115, N'Sergio Ivan Pardo Leon',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 116, N'Nelson Gustavo Hidalgo Baranza',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 117, N'Roberto Salcedo Silva',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 118, N'Pedro Rodriguez Martinez',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 119, N'Gilberto Forigua Rivera',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 120, N'Jose Rafael  Pinilla Monsalve',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 121, N'Angelo Napoleon Ruiz Perilla',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 122, N'Mauricio Florez Velasquez',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 123, N'Camilo Andrés Triviño Ulloa',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 124, N'Miguel Francisco Sierra Visbal',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 125, N'Elizabeth Gomez Rodríguez',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 126, N'Rafael Eduardo Casas Gil',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 127, N'Oscar Hernández',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 128, N'Cesar Augusto Echeverria Benavides',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 129, N'Hernando Laguna Jaramillo',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 130, N'Vannia Paola Durán Pérez',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);

COMMIT TRANSACTION;
PRINT 'Miembros de Sabana importados exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;