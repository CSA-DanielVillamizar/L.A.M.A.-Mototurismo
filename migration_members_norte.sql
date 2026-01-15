-- ============================================
-- INSERCIÓN MIEMBROS - REGIÓN NORTE
-- Auto-generado por extract_members_norte.py
-- ============================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (2, 1, N'BUITRAGO GIRLESA MARÍA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (2, 2, N'RODRIGUEZ OCHOA ANGELA MARIA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (2, 3, N'CARDONA BENITEZ JENNIFER ANDREA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (2, 4, N'SALAZAR MORENO LAURA VIVIANA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (4, 5, N'MARIN ZAPATA YUNIS EDITH', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (5, 6, N'VALENCIA ARIAS SANDRA PATRICIA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (6, 7, N'SANCHEZ SIERRA FANNY', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (6, 8, N'LOBO ANGARITA YAMIRA ELIZABETH', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (6, 9, N'CLAUDIA CELINA BOHORQUEZ RAMIREZ', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);
INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (6, 10, N'LUCERO PLATA MUGICA', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);

COMMIT TRANSACTION;
PRINT 'Miembros Región Norte insertados exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'ERROR en inserción: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
