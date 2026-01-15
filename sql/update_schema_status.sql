/* =========================================================
   1) CATÁLOGO: MemberStatusTypes (33 valores del dropdown)
   ========================================================= */

IF OBJECT_ID('[dbo].[MemberStatusTypes]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[MemberStatusTypes] (
        [StatusId] INT IDENTITY(1,1) PRIMARY KEY,
        [StatusName] NVARCHAR(100) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        [DisplayOrder] INT NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [UQ_MemberStatusTypes_StatusName] UNIQUE ([StatusName])
    );
END
GO

/* Limpia y reinserta (si quieres garantizar exactitud) */
DELETE FROM [dbo].[MemberStatusTypes];
GO

INSERT INTO [dbo].[MemberStatusTypes] ([StatusName],[Category],[DisplayOrder]) VALUES
('PROSPECT','CHAPTER',1),
('ROCKET PROSPECT','CHAPTER',2),
('FUL COLOR MEMBER','CHAPTER',3),

('CHAPTER PRESIDENT','CHAPTER_OFFICER',10),
('CHAPTER VICEPRESIDENT','CHAPTER_OFFICER',11),
('CHAPTER TREASURER','CHAPTER_OFFICER',12),
('CHAPTER BUSSINESS MANAGER','CHAPTER_OFFICER',13),
('CHAPTER SECRETARY','CHAPTER_OFFICER',14),
('CHAPTER MTO','CHAPTER_OFFICER',15),

('REGIONAL PRESIDENT','REGIONAL_OFFICER',20),
('REGIONAL VICEPRESIDENT','REGIONAL_OFFICER',21),
('REGIONAL TREASURER','REGIONAL_OFFICER',22),
('REGIONAL BUSSINESS MANAGER','REGIONAL_OFFICER',23),
('REGIONAL SECRETARY','REGIONAL_OFFICER',24),
('REGIONAL MTO','REGIONAL_OFFICER',25),

('NATIONAL PRESIDENT','NATIONAL_OFFICER',30),
('NATIONAL VICEPRESIDENT','NATIONAL_OFFICER',31),
('NATIONAL TREASURER','NATIONAL_OFFICER',32),
('NATIONAL BUSSINESS MANAGER','NATIONAL_OFFICER',33),
('NATIONAL SECRETARY','NATIONAL_OFFICER',34),
('NATIONAL MTO','NATIONAL_OFFICER',35),

('CONTINENTAL PRESIDENT','CONTINENTAL_OFFICER',40),
('CONTINENTAL VICEPRESIDENT','CONTINENTAL_OFFICER',41),
('CONTINENTAL TREASURER','CONTINENTAL_OFFICER',42),
('CONTINENTAL BUSSINESS MANAGER','CONTINENTAL_OFFICER',43),
('CONTINENTAL SECRETARY','CONTINENTAL_OFFICER',44),
('CONTINENTAL MTO','CONTINENTAL_OFFICER',45),

('INTERNATIONAL PRESIDENT','INTERNATIONAL_OFFICER',50),
('INTERNATIONAL VICEPRESIDENT','INTERNATIONAL_OFFICER',51),
('INTERNATIONAL TREASURER','INTERNATIONAL_OFFICER',52),
('INTERNATIONAL BUSSINESS MANAGER','INTERNATIONAL_OFFICER',53),
('INTERNATIONAL SECRETARY','INTERNATIONAL_OFFICER',54),
('INTERNATIONAL MTO','INTERNATIONAL_OFFICER',55);
GO


/* =========================================================
   2) MEMBERS: STATUS = dropdown 33 (sin ACTIVE/INACTIVE)
   ========================================================= */

-- Eliminar constraint viejo si existe
IF OBJECT_ID('[dbo].[CK_Members_Status]', 'C') IS NOT NULL
    ALTER TABLE [dbo].[Members] DROP CONSTRAINT [CK_Members_Status];
GO

-- Quitar default 'ACTIVE' si existe (nombre puede variar)
DECLARE @dfName NVARCHAR(200);
SELECT @dfName = dc.name
FROM sys.default_constraints dc
JOIN sys.columns c ON c.default_object_id = dc.object_id
JOIN sys.tables t ON t.object_id = c.object_id
WHERE t.name = 'Members' AND c.name = 'STATUS';

IF @dfName IS NOT NULL
    EXEC('ALTER TABLE [dbo].[Members] DROP CONSTRAINT [' + @dfName + ']');
GO

-- Asegurar tipo/nullable
ALTER TABLE [dbo].[Members] ALTER COLUMN [STATUS] NVARCHAR(100) NOT NULL;
GO

-- Crear FK para que STATUS solo admita los 33 valores
IF OBJECT_ID('[dbo].[FK_Members_StatusName]', 'F') IS NULL
BEGIN
    ALTER TABLE [dbo].[Members]
    ADD CONSTRAINT [FK_Members_StatusName]
        FOREIGN KEY ([STATUS])
        REFERENCES [dbo].[MemberStatusTypes]([StatusName]);
END
GO

PRINT 'Schema actualizado: MemberStatusTypes con 33 valores y FK desde Members.STATUS';
