-- REIMPORTACIÓN LIMPIA - MIEMBROS Y VEHÍCULOS CON STATUS REAL
SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

-- Limpiar datos previos
DELETE FROM [dbo].[Vehicles];
DELETE FROM [dbo].[Members];

-- ===== MEMBERS (154) =====
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 1, N'OSCAR ANGELO GARCIA BUITRAGO', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 2, N'GUILLERMO OLAYA ACEVEDO', N'NO',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 3, N'DIANA ROJAS MUÑOZ', N'SI',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 4, N'HARVEY CRUZ PINZON', N'NO',
     N'COLOMBIA', 2018, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 5, N'JOSE NICOLAS RAMIREZ ECHEVERRY', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 6, N'JHON JAIRO JIMENEZ GIRALDO', N'NO',
     N'COLOMBIA', 2018, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 7, N'MARTIN ALONSO GIRALDO CATAÑO', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 8, N'WILLIAM GABRIEL OCAMPO SUAREZ', N'NO',
     N'COLOMBIA', 2020, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 9, N'JORGE SOTO CARVAJAL', N'NO',
     N'COLOMBIA', 2023, N'CHAPTER VICEPRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 10, N'JORGE ROJAS', N'NO',
     N'COLOMBIA', 2023, N'CHAPTER TREASURER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 11, N'JUAN JOSÉ GÓMEZ', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 12, N'ALBERTO GUARIN ARREDONDO', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 13, N'CAMILO ANDRES MERCHAN CORREA', N'NO',
     N'COLOMBIA', 2023, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 14, N'JEISSON STIVEN  MARIN VALENCIA', N'NO',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 15, N'LIBARDO SALGADO', N'NO',
     N'COLOMBIA', 2024, N'CHAPTER BUSSINESS MANAGER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 16, N'LIBARDO SALGADO', N'NO',
     N'COLOMBIA', 2024, N'CHAPTER BUSSINESS MANAGER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 17, N'ROBISON LARGO GARCIA', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 18, N'GERMANICO GUTIERREZ BETANCURT', N'NO',
     N'COLOMBIA', 2024, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 19, N'GERMANICO GUTIERREZ BETANCURT', N'NO',
     N'COLOMBIA', 2024, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 20, N'LUISA FERNANDA VELA AGUDELO', N'SI',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 21, N'URIEL PEÑA TORRES', N'NO',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 22, N'LUIS ORLANDO PALACIO OSORIO', N'NO',
     N'COLOMBIA', 2024, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 23, N'LUIS CAMILO JOSE PEÑA GARCIA', N'NO',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 24, N'LUIS CAMILO JOSE PEÑA GARCIA', N'NO',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 25, N'CESAR AUGUSTO CASTRO OYOLA', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 26, N'RICARDO TELLEZ MENDOZA', N'NO',
     N'COLOMBIA', 2018, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 27, N'GERARDO ANTONIO MARIN RAMIREZ', N'NO',
     N'COLOMBIA', 2022, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 28, N'HERMAN ZULUAGA JARAMILLO', N'NO',
     N'COLOMBIA', 2018, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 29, N'EDWIN HERNANDEZ', N'NO',
     N'COLOMBIA', 2025, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 30, N'LUIS FELIPE BEDOYA', N'NO',
     N'COLOMBIA', 2018, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 31, N'LIBARDO ANTONIO VARELA', N'NO',
     N'COLOMBIA', 2020, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (1, 32, N'PAOLA ANDREA RAIGOSA ARREDONDO', N'SI',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 33, N'GONZALEZ CASTAÑO RAMÓN ANTONIO', N'NO',
     N'COLOMBIA', 2013, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 34, N'GONZALEZ CASTAÑO RAMÓN ANTONIO', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 35, N'GONZALEZ HENAO HECTOR MARIO', N'NO',
     N'COLOMBIA', 2013, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 36, N'RODRIGUEZ GALAN CESAR LEONEL', N'NO',
     N'COLOMBIA', 2015, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 37, N'GÓMEZ PATIÑO JHON HARVEY', N'NO',
     N'COLOMBIA', 2015, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 38, N'JIMÉNEZ PÉREZ WILLIAM HUMBERTO', N'NO',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 39, N'ARAQUE BETANCUR CARLOS ALBERTO', N'NO',
     N'COLOMBIA', 2019, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 40, N'ARAQUE BETANCUR CARLOS ALBERTO', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 41, N'CEBALLOS CARLOS MARIO', N'NO',
     N'COLOMBIA', 2020, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 42, N'PEREZ AREIZA CARLOS ANDRES', N'NO',
     N'COLOMBIA', 2020, N'CHAPTER VICEPRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 43, N'SUAREZ CORREA JUAN ESTEBAN', N'NO',
     N'COLOMBIA', 2020, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 44, N'BUITRAGO GIRLESA MARÍA', N'SI',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 45, N'ARZUZA PÁEZ JHON EMMANUEL', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 46, N'OSPINA CRUZ JOSÉ EDINSON', N'NO',
     N'COLOMBIA', 2021, N'CHAPTER BUSSINESS MANAGER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 47, N'OSPINA CRUZ JOSÉ EDINSON', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 48, N'MONTOYA MUÑOZ JEFFERSON', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 49, N'OSORIO JUAN ESTEBAN', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 50, N'GALVIS PARRA ROBINSON ALEJANDRO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER TREASURER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 51, N'GALVIS PARRA ROBINSON ALEJANDRO', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 52, N'DIAZ DIAZ CARLOS MARIO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 53, N'USUGA AGUDELO YEFERSON BAIRON', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 54, N'SANCHEZ JHON DAVID', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 55, N'VILLAMIZAR ARAQUE DANIEL ANDREY', N'NO',
     N'COLOMBIA', 2024, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 56, N'VILLAMIZAR ARAQUE DANIEL ANDREY', N'NO',
     N'COLOMBIA', 2025, N'REGIONAL PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 57, N'RODRIGUEZ OCHOA ANGELA MARIA', N'SI',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 58, N'RENDON DIAZ CARLOS JULIO', N'NO',
     N'COLOMBIA', 2021, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 59, N'CARDONA BENITEZ JENNIFER ANDREA', N'SI',
     N'COLOMBIA', 2025, N'ROCKET PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 60, N'GOMEZ RIVERA MILTON DARIO', N'NO',
     N'COLOMBIA', 2019, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 61, N'SALAZAR MORENO LAURA VIVIANA', N'SI',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 62, N'VILLAMIZAR ARAQUE JOSE JULIAN', N'NO',
     N'COLOMBIA', 2025, N'ROCKET PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 63, N'GOMEZ ZULUAGA GUSTAVO ADOLFO', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (2, 64, N'MONTOYA MATAUTE NELSON AUGUSTO', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 65, N'Luis Fernando  Peralta Franco', N'NO',
     N'COLOMBIA', 2015, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 66, N'Jaime Alberto Botero Ospina', N'NO',
     N'COLOMBIA', 2015, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 67, N'Carlos Angel Chaparro Lozano', N'NO',
     N'COLOMBIA', 2015, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 68, N'Jorge Alberto Valencia Callejas', N'NO',
     N'COLOMBIA', 2016, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 69, N'Freddy Raúl Bonilla Muñoz', N'NO',
     N'COLOMBIA', 2016, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 70, N'Sergio Ivan Pardo Leon', N'NO',
     N'COLOMBIA', 2016, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 71, N'Nelson Gustavo Hidalgo Baranza', N'NO',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 72, N'Roberto Salcedo Silva', N'NO',
     N'COLOMBIA', 2017, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 73, N'Roberto Salcedo Silva', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 74, N'Pedro Rodriguez Martinez', N'NO',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 75, N'Gilberto Forigua Rivera', N'NO',
     N'COLOMBIA', 2018, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 76, N'Jose Rafael  Pinilla Monsalve', N'NO',
     N'COLOMBIA', 2019, N'CHAPTER TREASURER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 77, N'Jose Rafael  Pinilla Monsalve', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 78, N'Angelo Napoleon Ruiz Perilla', N'NO',
     N'COLOMBIA', 2019, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 79, N'Mauricio Florez Velasquez', N'NO',
     N'COLOMBIA', 2020, N'CHAPTER BUSSINESS MANAGER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 80, N'Camilo Andrés Triviño Ulloa', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 81, N'Miguel Francisco Sierra Visbal', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 82, N'Elizabeth Gomez Rodríguez', N'SI',
     N'COLOMBIA', 2023, N'CHAPTER VICEPRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 83, N'Elizabeth Gomez Rodríguez', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 84, N'Rafael Eduardo Casas Gil', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 85, N'Rafael Eduardo Casas Gil', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 86, N'Oscar Hernández', N'NO',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 87, N'Cesar Augusto Echeverria Benavides', N'NO',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 88, N'Cesar Augusto Echeverria Benavides', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 89, N'Hernando Laguna Jaramillo', N'NO',
     N'COLOMBIA', 2024, N'ROCKET PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (3, 90, N'Vannia Paola Durán Pérez', N'SI',
     N'COLOMBIA', 2024, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 91, N'SERNA SANMARTIN ORLANDO', N'NO',
     N'COLOMBIA', 2012, N'CHAPTER TREASURER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 92, N'SERNA SANMARTIN ORLANDO', N'NO',
     N'COLOMBIA', 2025, N'CHAPTER TREASURER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 93, N'QUINTERO GUILLEN EDGAR OMAR', N'NO',
     N'COLOMBIA', 2015, N'CHAPTER BUSSINESS MANAGER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 94, N'VILLAMIZAR MARTINEZ JAVIER HUMBERTO', N'NO',
     N'COLOMBIA', 2015, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 95, N'MARIN ZAPATA YUNIS EDITH', N'SI',
     N'COLOMBIA', 2016, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 96, N'TINOCO VARGAS JAIRO ALEXANDER', N'NO',
     N'COLOMBIA', 2017, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 97, N'QUINTERO PEÑARANDA TEODULFO', N'NO',
     N'COLOMBIA', 2017, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 98, N'QUINTERO PEÑARANDA TEODULFO', N'NO',
     N'COLOMBIA', 2025, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 99, N'OSUNA RINCON ORLANDO', N'NO',
     N'COLOMBIA', 2019, N'CHAPTER VICEPRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 100, N'RAUL GUTIERREZ MORA', N'NO',
     N'COLOMBIA', 2021, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 101, N'RAUL GUTIERREZ MORA', N'NO',
     N'COLOMBIA', 2025, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 102, N'CANAL CARDENAS JAVIER', N'NO',
     N'COLOMBIA', 2022, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 103, N'JAVIER ALBERTO BUENO ESTEBAN', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 104, N'OSCAR MAURICIO SERRATO LEGUIZAMON', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 105, N'HENRY JOSE ARAUJO LEON', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (4, 106, N'ALBERTO JOSE GARCIA COMAS', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 107, N'ESTRADA  CASTAÑO MAURICIO ENRICO', N'NO',
     N'COLOMBIA', 2016, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 108, N'GIRALDO   FERNANDEZ  ALEJANDRO', N'NO',
     N'COLOMBIA', 2016, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 109, N'BUITRAGO  MEJIA  MARCO  FIDEL', N'NO',
     N'COLOMBIA', 2016, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 110, N'MEDRANO  CARCAMO  FRANKLIN', N'NO',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 111, N'VALENCIA ARIAS SANDRA PATRICIA', N'SI',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 112, N'RAMIREZ QUIROGA WILSON', N'NO',
     N'COLOMBIA', 2019, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 113, N'MEJIA ARANGO MAURICIO', N'NO',
     N'COLOMBIA', 2019, N'CHAPTER TREASURER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 114, N'GUERRERO SANCHEZ MIGUEL ANGEL', N'NO',
     N'COLOMBIA', 2020, N'CHAPTER VICEPRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 115, N'RANGEL NUMA GUSTAVO ADOLFO', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 116, N'MUNERA VARGAS JUAN STIVEN', N'NO',
     N'COLOMBIA', 2022, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 117, N'ESCORIHUELA SEVILLA JHONSON JESUS', N'NO',
     N'COLOMBIA', 2022, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 118, N'REYNOLDS LAMONT', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 119, N'TORO GARCIA HADDER DARIO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 120, N'TORO GARCIA HADDER DARIO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 121, N'MORENO ARTEAGA DAVID ALBEIRO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 122, N'MORENO ARTEAGA DAVID ALBEIRO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 123, N'KURT PETERSEN', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 124, N'JUAN FRANCISCO BRAN', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 125, N'DAVID ALBERTO ARZAYUS MERCADO', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 126, N'DAVID ALBERTO ARZAYUS MERCADO', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 127, N'MANUEL GUZMAN', N'NO',
     N'COLOMBIA', 2017, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (5, 128, N'ANIBAL BRU GOMEZ', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 129, N'LUIS ALFREDO OVIEDO MADRID', N'NO',
     N'COLOMBIA', 2019, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 130, N'SARA MONTOYA MUÑOZ', N'SI',
     N'COLOMBIA', 2019, N'CHAPTER PRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 131, N'HECTOR FERNEY GOMEZ LOAIZA', N'NO',
     N'COLOMBIA', 2020, N'CHAPTER SECRETARY', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 132, N'HECTOR FERNEY GOMEZ LOAIZA', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 133, N'WILMAR DE JESUS OCAMPO OSPINA', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 134, N'DANIEL ENRIQUE MAYA URAN', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 135, N'HECTOR ALEJANDRO SANTOS FERNANDEZ', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 136, N'VICTOR GONZALO MEJIA ARCILA', N'NO',
     N'COLOMBIA', 2021, N'CHAPTER VICEPRESIDENT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 137, N'ANDERSON OSWALDO ARIAS NIETO', N'NO',
     N'COLOMBIA', 2022, N'CHAPTER MTO', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 138, N'CARLOS ALBERTO RUEDA REYES', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 139, N'CARLOS ALBERTO RUEDA REYES', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 140, N'ALEXANDER VARGAS MATAMOROS', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 141, N'ALEXANDER VARGAS MATAMOROS', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 142, N'MARTIN EMILIO ACEVEDO  MONTOYA', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 143, N'JUAN DIEGO ZAPATA SERNA', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 144, N'ALVARO HERNAN QUINTERO GOMEZ', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 145, N'HENRY RIOS GOMEZ', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 146, N'ANDREA CASTAÑO OSPINA', N'SI',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 147, N'FELIX SOTO LOPEZ', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 148, N'JORGE FELIX  SOTO VALENCIA', N'NO',
     N'COLOMBIA', 2024, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 149, N'JORGE FELIX  SOTO VALENCIA', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 150, N'ANDRES FELIPE CARDENAS', N'NO',
     N'COLOMBIA', 2024, N'ROCKET PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 151, N'FREDY ALEXANDER MARIN', N'NO',
     N'COLOMBIA', 2021, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 152, N'FREDY ALEXANDER MARIN', N'NO',
     N'COLOMBIA', 2025, N'PROSPECT', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 153, N'SEBASTIAN PAREJA', N'NO',
     N'COLOMBIA', 2023, N'FUL COLOR MEMBER', 1);
INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    (6, 154, N'CARLOS ANDRES GUARIN VELASQUEZ', N'NO',
     N'COLOMBIA', 1905, N'PROSPECT', 1);

-- ===== VEHICLES (154) =====

DECLARE @MemberId_1 INT;
SELECT @MemberId_1 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 1;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_1, N'BMW  1200 Adventure GS  2016', N'VWJ200',
     0, N'SI', 39929.0, 43930.0, 1);

DECLARE @MemberId_2 INT;
SELECT @MemberId_2 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 2;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_2, N'KAWASAKI VULCAN 900cc', N'BRR-35',
     0, N'SI', 39325.0, 40063.0, 1);

DECLARE @MemberId_3 INT;
SELECT @MemberId_3 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 3;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_3, N'YAMAHA CZD-300 A 2019', N'EXR-26F',
     0, N'SI', 15544.0, 16233.0, 1);

DECLARE @MemberId_4 INT;
SELECT @MemberId_4 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 4;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_4, N'TRIUMPH TIGER 800cc  2013', N'BAH-88D',
     0, N'SI', 94617.0, 97828.0, 1);

DECLARE @MemberId_5 INT;
SELECT @MemberId_5 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 5;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_5, N'SUZUKI V-STROM 650cc 2017', N'NFP-55E',
     0, N'NO', 0.0, 24211.0, 1);

DECLARE @MemberId_6 INT;
SELECT @MemberId_6 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 6;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_6, N'SUZUKI V-STROM 1000cc 2018', N'SZD-58',
     0, N'SI', 62220.0, 66656.0, 1);

DECLARE @MemberId_7 INT;
SELECT @MemberId_7 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 7;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_7, N'HONDA AFRICA TWIN 1100', N'VVN96F',
     0, N'SI', 12997.0, 17051.0, 1);

DECLARE @MemberId_8 INT;
SELECT @MemberId_8 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 8;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_8, N'SIN MOTO', N'AUTO_ORD_8',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_9 INT;
SELECT @MemberId_9 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 9;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_9, N'SUZUKI VSTROM  1050CC', N'KBJ04G',
     0, N'SI', 11126.0, 18232.0, 1);

DECLARE @MemberId_10 INT;
SELECT @MemberId_10 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 10;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_10, N'SUZUKI VSTROM 650CC ABS 2017', N'SXK06',
     0, N'SI', 55894.0, 58385.0, 1);

DECLARE @MemberId_11 INT;
SELECT @MemberId_11 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 11;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_11, N'SIN MOTO', N'AUTO_ORD_11',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_12 INT;
SELECT @MemberId_12 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 12;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_12, N'SUZUKY VSTROM 650 XT 2021', N'KGG56F',
     0, N'SI', 12734.0, 19882.0, 1);

DECLARE @MemberId_13 INT;
SELECT @MemberId_13 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 13;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_13, N'SUZUKY VSTROM 650 ABS 2017', N'ELK14E',
     0, N'SI', 38802.0, 0.0, 1);

DECLARE @MemberId_14 INT;
SELECT @MemberId_14 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 14;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_14, N'BMW F750 GS', N'UZF08G',
     0, N'SI', 5786.0, 13905.0, 1);

DECLARE @MemberId_15 INT;
SELECT @MemberId_15 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 15;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_15, N'BMW R1200', N'BUO69E',
     0, N'SI', 39432.0, 42238.0, 1);

DECLARE @MemberId_16 INT;
SELECT @MemberId_16 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 16;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_16, N'BMW F750 GS', N'UYD57G',
     0, N'SI', 6656.0, 8756.0, 1);

DECLARE @MemberId_17 INT;
SELECT @MemberId_17 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 17;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_17, N'SUZUKY VSTROM 650 ABS 2017', N'CNC67E',
     0, N'SI', 60428.0, 66549.0, 1);

DECLARE @MemberId_18 INT;
SELECT @MemberId_18 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 18;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_18, N'SUZUKI VSTROM DL650', N'ITT99D',
     0, N'SI', 89059.0, 106088.0, 1);

DECLARE @MemberId_19 INT;
SELECT @MemberId_19 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 19;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_19, N'HUSQVARNA NORDEN 901 EXPEDITION', N'UZI89G',
     0, N'SI', 5682.0, 17976.0, 1);

DECLARE @MemberId_20 INT;
SELECT @MemberId_20 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 20;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_20, N'AKT ADVENTURE 250', N'XGL29D',
     0, N'SI', 44018.0, 58679.0, 1);

DECLARE @MemberId_21 INT;
SELECT @MemberId_21 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 21;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_21, N'KAWASAKI  VERSYS 1000', N'RPG59C',
     0, N'NO', 0.0, 36601.0, 1);

DECLARE @MemberId_22 INT;
SELECT @MemberId_22 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 22;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_22, N'SUZUKI VSTROM DL650', N'SVR07',
     0, N'NO', 0.0, 35898.0, 1);

DECLARE @MemberId_23 INT;
SELECT @MemberId_23 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 23;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_23, N'BMW R18 1800CC', N'GLF56G',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_24 INT;
SELECT @MemberId_24 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 24;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_24, N'BMW R1250 GS', N'WRT19E',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_25 INT;
SELECT @MemberId_25 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 25;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_25, N'BMW R1200 GS', N'BYY14E',
     0, N'SI', 28415.0, 31410.0, 1);

DECLARE @MemberId_26 INT;
SELECT @MemberId_26 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 26;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_26, N'SUZUKI 1000 DL', N'NKN52D',
     0, N'SI', 58629.0, 61141.0, 1);

DECLARE @MemberId_27 INT;
SELECT @MemberId_27 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 27;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_27, N'SUZUKI DL650 XT', N'XKK99F',
     0, N'SI', 36746.0, 39995.0, 1);

DECLARE @MemberId_28 INT;
SELECT @MemberId_28 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 28;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_28, N'BMW R1200 GS', N'DSI46D',
     0, N'SI', 52028.0, 54503.0, 1);

DECLARE @MemberId_29 INT;
SELECT @MemberId_29 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 29;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_29, N'YAMAHA TRACER 900', N'EDI94F',
     0, N'SI', 18221.0, 19463.0, 1);

DECLARE @MemberId_30 INT;
SELECT @MemberId_30 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 30;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_30, N'KAWASAKI VOYAGER VII 1200', N'439G4S',
     0, N'NO', 0.0, 18234.0, 1);

DECLARE @MemberId_31 INT;
SELECT @MemberId_31 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 31;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_31, N'SUZUKI VSTROM 650', N'HOT21A',
     0, N'SI', 64210.0, 66656.0, 1);

DECLARE @MemberId_32 INT;
SELECT @MemberId_32 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 32;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_32, N'SUZUKI GIXXER 250', N'GMX10G',
     0, N'NO', 0.0, 12264.0, 1);

DECLARE @MemberId_33 INT;
SELECT @MemberId_33 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 33;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_33, N'BMW 120GS mod 2018', N'SFE 65E',
     0, N'SI', 17894.0, 21121.0, 1);

DECLARE @MemberId_34 INT;
SELECT @MemberId_34 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 34;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_34, N'Honda VTX 1300 MOD. 2007', N'HWA 49A',
     0, N'SI', 55454.0, 59744.0, 1);

DECLARE @MemberId_35 INT;
SELECT @MemberId_35 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 35;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_35, N'Yamaha superteneré 1200 MOD.2018', N'JVH 83E',
     0, N'SI', 31927.0, 37132.0, 1);

DECLARE @MemberId_36 INT;
SELECT @MemberId_36 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 36;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_36, N'Suzuki Boulevard 1800 Mod 2007', N'BMM 75',
     0, N'SI', 41573.0, 0.0, 1);

DECLARE @MemberId_37 INT;
SELECT @MemberId_37 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 37;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_37, N'BMW F1250GS MOD 2020', N'JLO 90F',
     0, N'SI', 16496.0, 22961.0, 1);

DECLARE @MemberId_38 INT;
SELECT @MemberId_38 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 38;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_38, N'Suzuki Vstrom Dl 650 MOD. 2008', N'BYF 19',
     0, N'SI', 75730.0, 75742.0, 1);

DECLARE @MemberId_39 INT;
SELECT @MemberId_39 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 39;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_39, N'Suzuki  Vstrom 650 MOD  2015', N'PKR 85A',
     0, N'SI', 18153.0, 28546.0, 1);

DECLARE @MemberId_40 INT;
SELECT @MemberId_40 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 40;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_40, N'Triumph Tiger 800 XC Mod 2013', N'YZJ53C',
     0, N'NO', 0.0, 80269.0, 1);

DECLARE @MemberId_41 INT;
SELECT @MemberId_41 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 41;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_41, N'Suzuki Bandit GSF650 MOD 2008', N'JBR 79B',
     0, N'SI', 99755.0, 106418.0, 1);

DECLARE @MemberId_42 INT;
SELECT @MemberId_42 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 42;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_42, N'BMW R1200 MOD 2016', N'GLJ21F',
     0, N'SI', 15529.0, 21145.0, 1);

DECLARE @MemberId_43 INT;
SELECT @MemberId_43 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 43;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_43, N'Suzuki Vstrom DL 650 MOD. 2015', N'NKF 76D',
     0, N'SI', 61770.0, 70869.0, 1);

DECLARE @MemberId_44 INT;
SELECT @MemberId_44 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 44;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_44, N'Yamaha Vstar Xvs 650 MOD 2007', N'HOK 68A',
     0, N'SI', 38057.0, 44913.0, 1);

DECLARE @MemberId_45 INT;
SELECT @MemberId_45 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 45;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_45, N'BMW R1200 GS MOD 2016', N'UGP 49D',
     0, N'SI', 41818.0, 48935.0, 1);

DECLARE @MemberId_46 INT;
SELECT @MemberId_46 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 46;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_46, N'BMW GS1200 MOD2005', N'NKK39C',
     0, N'SI', 90592.0, 98913.0, 1);

DECLARE @MemberId_47 INT;
SELECT @MemberId_47 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 47;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_47, N'BMW F800 GS MOD 2011', N'RRU28B',
     0, N'SI', 55134.0, 62416.0, 1);

DECLARE @MemberId_48 INT;
SELECT @MemberId_48 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 48;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_48, N'Suzuki Freewind 650 MOD 2005', N'ZIZ 69A',
     0, N'SI', 23336.0, 28166.0, 1);

DECLARE @MemberId_49 INT;
SELECT @MemberId_49 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 49;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_49, N'Suzuki Freewind DR-650', N'BRN 14A',
     0, N'SI', 58301.0, 0.0, 1);

DECLARE @MemberId_50 INT;
SELECT @MemberId_50 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 50;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_50, N'BMW R1200 GS MOD 2016', N'YTY 25D',
     0, N'SI', 59650.0, 69113.0, 1);

DECLARE @MemberId_51 INT;
SELECT @MemberId_51 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 51;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_51, N'BMW R1200 GS MOD 2016', N'UGP 49D_ORD51',
     0, N'SI', 39361.0, 41818.0, 1);

DECLARE @MemberId_52 INT;
SELECT @MemberId_52 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 52;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_52, N'BMW F850 MOD  2020', N'EQO 08F',
     0, N'SI', 42118.0, 49868.0, 1);

DECLARE @MemberId_53 INT;
SELECT @MemberId_53 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 53;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_53, N'Yamaha Superteneré 1200', N'GGS 47C',
     0, N'SI', 42118.0, 0.0, 1);

DECLARE @MemberId_54 INT;
SELECT @MemberId_54 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 54;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_54, N'Suzuki DR 650 Mod 2010', N'AFE 43C',
     0, N'SI', 45405.0, 50397.0, 1);

DECLARE @MemberId_55 INT;
SELECT @MemberId_55 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 55;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_55, N'BMW R1250GS 2024', N'SYN 25G',
     0, N'SI', 12454.0, 37811.0, 1);

DECLARE @MemberId_56 INT;
SELECT @MemberId_56 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 56;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_56, N'BMW F850GS 2023', N'IXG 84G',
     0, N'SI', 14246.0, 18889.0, 1);

DECLARE @MemberId_57 INT;
SELECT @MemberId_57 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 57;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_57, N'Royal Enfield Himalayan 400 2025', N'WEJ 73G',
     0, N'SI', 7657.0, 17312.0, 1);

DECLARE @MemberId_58 INT;
SELECT @MemberId_58 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 58;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_58, N'Kawasaki Vulcan LT . 903cc. Modelo 2012', N'PUT 09C',
     0, N'SI', 68299.0, 73292.0, 1);

DECLARE @MemberId_59 INT;
SELECT @MemberId_59 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 59;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_59, N'BMW F650GS 2016', N'ZTE 69E',
     0, N'SI', 31377.0, 0.0, 1);

DECLARE @MemberId_60 INT;
SELECT @MemberId_60 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 60;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_60, N'KTM Adventure 1090', N'BRC 59F',
     0, N'SI', 15982.0, 17584.0, 1);

DECLARE @MemberId_61 INT;
SELECT @MemberId_61 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 61;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_61, N'CF MT450', N'QGQ 40H',
     0, N'SI', 0.0, 4198.0, 1);

DECLARE @MemberId_62 INT;
SELECT @MemberId_62 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 62;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_62, N'BMW F850GS Adventure Triple Black 2023', N'IXG 84G_ORD62',
     0, N'SI', 18889.0, 25336.0, 1);

DECLARE @MemberId_63 INT;
SELECT @MemberId_63 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 63;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_63, N'KAWASAKI VERSYS 1000 2012', N'CZL59D',
     0, N'SI', 31099.0, 0.0, 1);

DECLARE @MemberId_64 INT;
SELECT @MemberId_64 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 64;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_64, N'BMW F850 GS 2023', N'FXS11G',
     0, N'SI', 10830.0, 13881.0, 1);

DECLARE @MemberId_65 INT;
SELECT @MemberId_65 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 65;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_65, N'', N'AUTO_ORD_65',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_66 INT;
SELECT @MemberId_66 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 66;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_66, N'YAMAHA XV 1900 MIDNIGHT 2003', N'LQY55A',
     0, N'SI', 45733.0, 56896.0, 1);

DECLARE @MemberId_67 INT;
SELECT @MemberId_67 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 67;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_67, N'SUZUKI VSTROM 650  2008', N'BQO65',
     0, N'SI', 86862.0, 0.0, 1);

DECLARE @MemberId_68 INT;
SELECT @MemberId_68 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 68;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_68, N'YAMAHA S.TENERE 1200 2014', N'JAQ68D',
     0, N'SI', 66110.0, 68069.0, 1);

DECLARE @MemberId_69 INT;
SELECT @MemberId_69 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 69;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_69, N'BMW R1200GS   2015', N'MJC48D',
     0, N'SI', 52122.0, 53589.0, 1);

DECLARE @MemberId_70 INT;
SELECT @MemberId_70 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 70;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_70, N'VSTROM 650 XT 2020', N'OEF62E',
     0, N'SI', 20777.0, 22155.0, 1);

DECLARE @MemberId_71 INT;
SELECT @MemberId_71 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 71;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_71, N'BMW F850GS 2024', N'LDR56F',
     0, N'SI', 5934.0, 12024.0, 1);

DECLARE @MemberId_72 INT;
SELECT @MemberId_72 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 72;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_72, N'TRIUMPH ROCKET III 2015', N'YUW11D',
     0, N'SI', 36845.0, 37078.0, 1);

DECLARE @MemberId_73 INT;
SELECT @MemberId_73 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 73;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_73, N'DUCATI MULTISTRADA V4 2024', N'LDJ04F',
     0, N'SI', 448.0, 3268.0, 1);

DECLARE @MemberId_74 INT;
SELECT @MemberId_74 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 74;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_74, N'YAMAHA MT 10 1000  2017', N'MUY19E',
     0, N'SI', 14203.0, 14622.0, 1);

DECLARE @MemberId_75 INT;
SELECT @MemberId_75 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 75;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_75, N'BMW F 700 GS 2013', N'AVA90D',
     0, N'SI', 48106.0, 48210.0, 1);

DECLARE @MemberId_76 INT;
SELECT @MemberId_76 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 76;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_76, N'BMW R1200 GS ADV 2017', N'YSZ34D',
     0, N'SI', 34041.0, 34701.0, 1);

DECLARE @MemberId_77 INT;
SELECT @MemberId_77 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 77;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_77, N'HARLEY DAVIDSON STREET GLADE 2019', N'OBI89E',
     0, N'SI', 11410.0, 12622.0, 1);

DECLARE @MemberId_78 INT;
SELECT @MemberId_78 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 78;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_78, N'YAMAHA S.TENERE 1200 2012', N'ZJM34C',
     0, N'SI', 17810.0, 25539.0, 1);

DECLARE @MemberId_79 INT;
SELECT @MemberId_79 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 79;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_79, N'SUZUKI VSTROM 650 2015', N'CAA50E',
     0, N'SI', 77166.0, 79399.0, 1);

DECLARE @MemberId_80 INT;
SELECT @MemberId_80 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 80;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_80, N'SUZUKI VSTROM 650 2016', N'NKS15E',
     0, N'SI', 89891.0, 93521.0, 1);

DECLARE @MemberId_81 INT;
SELECT @MemberId_81 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 81;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_81, N'SUZUKI VSTROM 650  2022', N'AQV03G',
     0, N'NO', 19699.0, 19699.0, 1);

DECLARE @MemberId_82 INT;
SELECT @MemberId_82 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 82;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_82, N'YAMAHA S.TENERE 1200 2023', N'XDN99G',
     0, N'SI', 7317.0, 9862.0, 1);

DECLARE @MemberId_83 INT;
SELECT @MemberId_83 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 83;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_83, N'HARLEY DAVIDSON STREET GLADE 2019', N'ODG69E',
     0, N'SI', 24717.0, 27076.0, 1);

DECLARE @MemberId_84 INT;
SELECT @MemberId_84 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 84;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_84, N'YAMAHA S.TENERE 1200 2023', N'XDO01G',
     0, N'SI', 6661.0, 8802.0, 1);

DECLARE @MemberId_85 INT;
SELECT @MemberId_85 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 85;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_85, N'HARLEY DAVIDSON STREET GLADE 2019', N'RNZ65E',
     0, N'SI', 30416.0, 32699.0, 1);

DECLARE @MemberId_86 INT;
SELECT @MemberId_86 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 86;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_86, N'VSTROM 650 XT 2020', N'KUE04F',
     0, N'SI', 11836.0, 19305.0, 1);

DECLARE @MemberId_87 INT;
SELECT @MemberId_87 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 87;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_87, N'YAMAHA S.TENERE 1200 2012', N'TEG91C',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_88 INT;
SELECT @MemberId_88 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 88;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_88, N'YAMAHA S.TENERE 1200 2013', N'EPU35D',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_89 INT;
SELECT @MemberId_89 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 89;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_89, N'BMW R1200GS   2014', N'EXE13D',
     0, N'SI', 33649.0, 37584.0, 1);

DECLARE @MemberId_90 INT;
SELECT @MemberId_90 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 90;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_90, N'HONDA XRE 300 2024', N'YBP50G',
     0, N'SI', 5026.0, 0.0, 1);

DECLARE @MemberId_91 INT;
SELECT @MemberId_91 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 91;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_91, N'HARLEY DAVIDSON HERITAGE SOFTAIL 2004', N'PHW22',
     0, N'SI', 15250.0, 16006.0, 1);

DECLARE @MemberId_92 INT;
SELECT @MemberId_92 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 92;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_92, N'BMW 1250 BMW (NUEVA)', N'LSA84G',
     0, N'SI', 21067.0, 23008.0, 1);

DECLARE @MemberId_93 INT;
SELECT @MemberId_93 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 93;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_93, N'BMW  R1200 GS ADVENTURE 2016', N'VGC71D',
     0, N'SI', 17287.0, 26416.0, 1);

DECLARE @MemberId_94 INT;
SELECT @MemberId_94 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 94;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_94, N'VULCAN 900 VN KAWASAKI 2017', N'NJL-23E',
     0, N'SI', 29341.0, 29991.0, 1);

DECLARE @MemberId_95 INT;
SELECT @MemberId_95 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 95;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_95, N'', N'AUTO_ORD_95',
     0, N'NO', 0.0, 0.0, 1);

DECLARE @MemberId_96 INT;
SELECT @MemberId_96 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 96;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_96, N'SUZUKI V-STROM 1000AB cc', N'HGX-33E',
     0, N'SI', 25164.0, 28895.0, 1);

DECLARE @MemberId_97 INT;
SELECT @MemberId_97 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 97;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_97, N'SUZUKI VSTROM 1000 CLASICA 2012', N'ASV-51D',
     0, N'NO', 0.0, 63430.0, 1);

DECLARE @MemberId_98 INT;
SELECT @MemberId_98 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 98;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_98, N'VULCAN 900 KAWASAKI', N'SNO25E',
     0, N'SI', 20186.0, 23588.0, 1);

DECLARE @MemberId_99 INT;
SELECT @MemberId_99 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 99;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_99, N'SUZUKI DL 1000-2008', N'MHR-35B',
     0, N'SI', 58067.0, 65675.0, 1);

DECLARE @MemberId_100 INT;
SELECT @MemberId_100 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 100;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_100, N'YAMAHA DARG STAR 1100   2000', N'AC5B85G',
     0, N'NO', 0.0, NULL, 1);

DECLARE @MemberId_101 INT;
SELECT @MemberId_101 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 101;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_101, N'BMW 1200 RT', N'MOL 01B',
     0, N'SI', 17901.0, 19053.0, 1);

DECLARE @MemberId_102 INT;
SELECT @MemberId_102 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 102;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_102, N'HARLEY DAVIDSON STREET GLIDE 2014', N'KHN-22D',
     0, N'SI', 37796.0, 42233.0, 1);

DECLARE @MemberId_103 INT;
SELECT @MemberId_103 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 103;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_103, N'SUZUKI DL VSTROM 650  2014', N'JTC-65D',
     0, N'SI', 47488.0, 51923.0, 1);

DECLARE @MemberId_104 INT;
SELECT @MemberId_104 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 104;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_104, N'SUZUKI VSTROM 650 2020', N'NKS-73F',
     0, N'SI', 33849.0, 39409.0, 1);

DECLARE @MemberId_105 INT;
SELECT @MemberId_105 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 105;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_105, N'HONDA CHADOW 750   1999', N'AI2N59M',
     0, N'SI', 16397.0, 19487.0, 1);

DECLARE @MemberId_106 INT;
SELECT @MemberId_106 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 106;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_106, N'HONDA CHADOW 750   2007', N'EDO21B',
     0, N'SI', 11273.0, 12349.0, 1);

DECLARE @MemberId_107 INT;
SELECT @MemberId_107 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 107;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_107, N'KAWASAKI VERSYS 1000 2016', N'FWC-60E',
     0, N'SI', 30643.0, 34822.0, 1);

DECLARE @MemberId_108 INT;
SELECT @MemberId_108 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 108;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_108, N'SUZUKI V-STROM 2012 650cc', N'CAG-45D',
     0, N'SI', 40691.0, 47582.0, 1);

DECLARE @MemberId_109 INT;
SELECT @MemberId_109 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 109;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_109, N'SUZUKI BOULEVARD 800', N'XSS71C',
     0, N'SI', 30340.0, 33440.0, 1);

DECLARE @MemberId_110 INT;
SELECT @MemberId_110 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 110;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_110, N'2007 HONDA VTX1300', N'TEG-82C',
     0, N'SI', 23300.0, 23615.0, 1);

DECLARE @MemberId_111 INT;
SELECT @MemberId_111 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 111;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_111, N'KTM DUKE 200CC', N'TJO-45D',
     0, N'SI', 23139.0, 24677.0, 1);

DECLARE @MemberId_112 INT;
SELECT @MemberId_112 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 112;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_112, N'YAMAHA XVS 950 A', N'YHC96C',
     0, N'SI', 42624.0, 49303.0, 1);

DECLARE @MemberId_113 INT;
SELECT @MemberId_113 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 113;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_113, N'BMW F 750 GS', N'OCR59E',
     0, N'SI', 13300.0, 17073.0, 1);

DECLARE @MemberId_114 INT;
SELECT @MemberId_114 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 114;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_114, N'ROYAL ENFIELD SUPERMETEOR 650', N'ZHV14G',
     0, N'SI', 2363.0, 6135.0, 1);

DECLARE @MemberId_115 INT;
SELECT @MemberId_115 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 115;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_115, N'SUZUKI DL650XT 2021', N'PQP11F',
     0, N'SI', 15055.0, 21599.0, 1);

DECLARE @MemberId_116 INT;
SELECT @MemberId_116 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 116;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_116, N'SUZUKI V-STROM 650 XT', N'LUF-32H',
     0, N'SI', 2681.0, 5569.0, 1);

DECLARE @MemberId_117 INT;
SELECT @MemberId_117 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 117;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_117, N'HARLEY DAVIDSON FAT BOY', N'LCK02F',
     0, N'SI', 3927.0, 7234.0, 1);

DECLARE @MemberId_118 INT;
SELECT @MemberId_118 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 118;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_118, N'YAMAHA SUPERTENERE 1200cc', N'ZXH72D',
     0, N'SI', 61243.0, 72023.0, 1);

DECLARE @MemberId_119 INT;
SELECT @MemberId_119 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 119;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_119, N'BMW R18', N'LCR73F',
     0, N'SI', 9963.0, 12789.0, 1);

DECLARE @MemberId_120 INT;
SELECT @MemberId_120 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 120;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_120, N'BMW F1250GS', N'FMK71G',
     0, N'SI', 1970.0, 2085.0, 1);

DECLARE @MemberId_121 INT;
SELECT @MemberId_121 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 121;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_121, N'HARLEY DAVIDSON FLSB SPORT GLIDE', N'HNF74G',
     0, N'SI', 6917.0, 12341.0, 1);

DECLARE @MemberId_122 INT;
SELECT @MemberId_122 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 122;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_122, N'BMW F800GS', N'XOE78H',
     0, N'NO', 0.0, 1.0, 1);

DECLARE @MemberId_123 INT;
SELECT @MemberId_123 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 123;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_123, N'HONDA AFRICA TWIN', N'LRK21G',
     0, N'SI', 10530.0, 14916.0, 1);

DECLARE @MemberId_124 INT;
SELECT @MemberId_124 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 124;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_124, N'HARLEY DAVIDSON CLASSIC HERITAGE 2022', N'LCF86F',
     0, N'SI', 4432.0, 10274.0, 1);

DECLARE @MemberId_125 INT;
SELECT @MemberId_125 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 125;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_125, N'ROYAL ENFIELD CONTINENTAL GT 650', N'MNL78G',
     0, N'SI', 7680.0, 18614.0, 1);

DECLARE @MemberId_126 INT;
SELECT @MemberId_126 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 126;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_126, N'HARLEY DAVIDSON XL 883N IRON 2013', N'BAL23D',
     0, N'SI', 32168.0, 35096.0, 1);

DECLARE @MemberId_127 INT;
SELECT @MemberId_127 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 127;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_127, N'SUZUKI V-STROM 650 cc', N'BKE-36',
     0, N'SI', 63651.0, 73702.0, 1);

DECLARE @MemberId_128 INT;
SELECT @MemberId_128 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 128;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_128, N'BMW 1200 GS', N'EHW97E',
     0, N'NO', 0.0, NULL, 1);

DECLARE @MemberId_129 INT;
SELECT @MemberId_129 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 129;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_129, N'Honda Goldwing 1800 Mod 2023', N'GSE39G',
     0, N'SI', 31967.0, 36920.0, 1);

DECLARE @MemberId_130 INT;
SELECT @MemberId_130 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 130;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_130, N'YAMAHA 300 XMAX MOD 2023', N'ALW18G',
     0, N'SI', 12857.0, 18562.0, 1);

DECLARE @MemberId_131 INT;
SELECT @MemberId_131 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 131;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_131, N'Yamaha Virago 1100 cc Mod 1995', N'AKE64',
     0, N'SI', 38045.0, 38045.0, 1);

DECLARE @MemberId_132 INT;
SELECT @MemberId_132 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 132;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_132, N'HARLEY DAVIDSON XL883C SPORTSTER 2004', N'AMK08',
     0, N'SI', 45689.0, 45689.0, 1);

DECLARE @MemberId_133 INT;
SELECT @MemberId_133 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 133;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_133, N'Honda Magna 750 MOD 1994', N'CKK15',
     0, N'SI', 48012.0, 51533.0, 1);

DECLARE @MemberId_134 INT;
SELECT @MemberId_134 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 134;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_134, N'Yamaha Supertenere 1200 Mod 2018', N'WIL02E',
     0, N'SI', 36056.0, 39060.0, 1);

DECLARE @MemberId_135 INT;
SELECT @MemberId_135 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 135;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_135, N'Yamaha Supertenere 1200 Mod 2015', N'AHV45E',
     0, N'SI', 62221.0, 62221.0, 1);

DECLARE @MemberId_136 INT;
SELECT @MemberId_136 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 136;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_136, N'BMW R 1200 MOD 2016', N'DAK56E',
     0, N'SI', 28640.0, 33366.0, 1);

DECLARE @MemberId_137 INT;
SELECT @MemberId_137 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 137;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_137, N'Suzuki Vstrom 650 DL MOD 2007', N'HPH60A',
     0, N'SI', 57914.0, 67278.0, 1);

DECLARE @MemberId_138 INT;
SELECT @MemberId_138 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 138;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_138, N'Suzuki Vstrom 650 DL MOD 2011', N'JWO34C',
     0, N'SI', 14208.0, 14208.0, 1);

DECLARE @MemberId_139 INT;
SELECT @MemberId_139 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 139;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_139, N'Suzuki Vstrom AT 650 MOD 2018', N'PZJ12E',
     0, N'SI', 37527.0, 37527.0, 1);

DECLARE @MemberId_140 INT;
SELECT @MemberId_140 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 140;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_140, N'Suzuki Vstrom 650 DL MOD 2008', N'LYR61A',
     0, N'SI', 93296.0, 93296.0, 1);

DECLARE @MemberId_141 INT;
SELECT @MemberId_141 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 141;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_141, N'YAMAHA VIrago 1100 cc Mod 1996', N'MXE58A',
     0, N'SI', 1294.0, 1294.0, 1);

DECLARE @MemberId_142 INT;
SELECT @MemberId_142 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 142;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_142, N'YAMAHA 1300 MOD 1996', N'LAM03A',
     0, N'SI', 49427.0, 52536.0, 1);

DECLARE @MemberId_143 INT;
SELECT @MemberId_143 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 143;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_143, N'HONDA VTX 1300S MOD 2006', N'SMR92',
     0, N'SI', 20297.0, 20297.0, 1);

DECLARE @MemberId_144 INT;
SELECT @MemberId_144 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 144;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_144, N'HONDA  1100 MOD 1993', N'CJG06',
     0, N'SI', 28082.0, 29477.0, 1);

DECLARE @MemberId_145 INT;
SELECT @MemberId_145 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 145;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_145, N'HONDA SHADOW 1100 MOD 1996', N'ENW33A',
     0, N'SI', 43291.0, 43291.0, 1);

DECLARE @MemberId_146 INT;
SELECT @MemberId_146 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 146;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_146, N'HONDA SHADOW 600 MOD 1994', N'FGM16',
     0, N'SI', 18490.0, 18490.0, 1);

DECLARE @MemberId_147 INT;
SELECT @MemberId_147 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 147;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_147, N'HONDA SHADOW 750 MOD', N'BZM85',
     0, N'SI', 40014.0, 42903.0, 1);

DECLARE @MemberId_148 INT;
SELECT @MemberId_148 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 148;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_148, N'HONDA SHADOW 1100 MOD', N'OMK47B',
     0, N'SI', 29066.0, 29066.0, 1);

DECLARE @MemberId_149 INT;
SELECT @MemberId_149 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 149;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_149, N'Yamaha Supertenere 1200 Mod 2020', N'OIJ75F',
     0, N'SI', 18490.0, 22919.0, 1);

DECLARE @MemberId_150 INT;
SELECT @MemberId_150 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 150;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_150, N'HONDA SHADOW AERO745 MOD  2007', N'FTC30B',
     0, N'SI', 12663.0, 14146.0, 1);

DECLARE @MemberId_151 INT;
SELECT @MemberId_151 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 151;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_151, N'Suzuki Vstrom 650 DL MOD 2006', N'CIK40B',
     0, N'SI', 97210.0, 97210.0, 1);

DECLARE @MemberId_152 INT;
SELECT @MemberId_152 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 152;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_152, N'Yamaha Supertenere 1200 Mod 2015', N'CIK40B_ORD152',
     0, N'SI', 65786.0, 76046.0, 1);

DECLARE @MemberId_153 INT;
SELECT @MemberId_153 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 153;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_153, N'Yamaha Supertenere 1200 Mod 2015', N'SRE90D',
     0, N'SI', 70300.0, 70300.0, 1);

DECLARE @MemberId_154 INT;
SELECT @MemberId_154 = [MemberId] FROM [dbo].[Members] WHERE [Order] = 154;

INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_154, N'Yamaha Supertenere 1200 Mod 2016', N'DRP80E',
     0, N'SI', 46167.0, 49163.0, 1);

COMMIT TRANSACTION;
PRINT 'Reimportación completada: 154 Members + 154 Vehicles con STATUS real del Excel';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;