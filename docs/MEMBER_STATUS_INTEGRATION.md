-- INSTRUCCIONES DE INTEGRACIÓN: MEMBER STATUS TYPES

-- 1. ASEGURARSE DE AGREGAR LA ENTIDAD AL DbContext
-- En: src/Lama.Infrastructure/Data/LamaDbContext.cs

-- Agregar esta línea en la clase LamaDbContext:
/*
    public DbSet<MemberStatusType> MemberStatusTypes { get; set; } = null!;
*/

-- Y en OnModelCreating(), agregar:
/*
    modelBuilder.ApplyConfiguration(new MemberStatusTypeConfiguration());
*/

-- 2. REGISTRAR EL SERVICIO EN DI
-- En: src/Lama.API/Program.cs o startup de DI

-- Agregar en ConfigureServices:
/*
    services.AddScoped<IMemberStatusService, MemberStatusService>();
*/

-- 3. LA TABLA YA ESTÁ CREADA CON:
-- - 33 valores de STATUS provenientes de Excel (Resumen!E4:E36)
-- - Organizados por categoría (CHAPTER, REGIONAL, NATIONAL, etc.)
-- - Ordenados por DisplayOrder para dropdown
-- - Todos activos (IsActive = 1)

-- 4. ENDPOINTS DISPONIBLES EN API:
-- GET /api/memberstatus -- Obtiene todos los estados
-- GET /api/memberstatus/by-category/{category} -- Filtra por categoría
-- GET /api/memberstatus/categories -- Lista todas las categorías
-- GET /api/memberstatus/by-name/{statusName} -- Obtiene uno específico

-- 5. PARA EL DROPDOWN EN COR, LLAMAR:
-- GET /api/memberstatus?includeInactive=false

-- O filtrar por categoría específica:
-- GET /api/memberstatus/by-category/CHAPTER_OFFICER

-- 6. VALORES DISPONIBLES PARA SELECCIONAR:

PROSPECT
ROCKET PROSPECT
FUL COLOR MEMBER

CHAPTER PRESIDENT
CHAPTER VICEPRESIDENT
CHAPTER TREASURER
CHAPTER BUSSINESS MANAGER
CHAPTER SECRETARY
CHAPTER MTO

REGIONAL PRESIDENT
REGIONAL VICEPRESIDENT
REGIONAL TREASURER
REGIONAL BUSSINESS MANAGER
REGIONAL SECRETARY
REGIONAL MTO

NATIONAL PRESIDENT
NATIONAL  VICEPRESIDENT
NATIONAL TREASURER
NATIONAL  BUSSINESS MANAGER
NATIONAL  SECRETARY
NATIONAL  MTO

CONTINENTAL PRESIDENT
CONTINENTAL VICEPRESIDENT
CONTINENTAL TREASURER
CONTINENTAL  BUSSINESS MANAGER
CONTINENTAL SECRETARY
CONTINENTAL MTO

INTERNATIONAL PRESIDENT
INTERNATIONAL  VICEPRESIDENT
INTERNATIONAL  TREASURER
INTERNATIONAL BUSSINESS MANAGER
INTERNATIONAL SECRETARY
INTERNATIONAL  MTO

-- 7. CONSIDERACIONES:
-- - El campo Members.STATUS sigue siendo NVARCHAR(100) por compatibilidad
-- - Puedes hacer un FK adicional si necesitas validación estricta
-- - Los valores mantienen exactamente la capitalización del Excel
-- - Se incluyen espacios dobles como en el Excel original ("NATIONAL  VICEPRESIDENT")
-- - DisplayOrder permite agrupar por niveles (1-3: Prospect, 10-15: Chapter Officers, etc)
