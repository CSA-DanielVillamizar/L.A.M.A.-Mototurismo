-- ============================================
-- LAMA MOTOTURISMO - VIEWS
-- ============================================

-- ============================================
-- vw_MasterOdometerReport
-- Calcula total de millas viajadas por vehiculo
-- ============================================
CREATE VIEW [dbo].[vw_MasterOdometerReport]
AS
SELECT
    [MemberId],
    [Id] AS [VehicleId],
    [Motorcycle Data],
    [Lic Plate],
    [OdometerUnit],
    [Starting Odometer],
    [Final Odometer],
    CASE 
        WHEN [Final Odometer] IS NULL OR [Starting Odometer] IS NULL THEN 0
        WHEN [OdometerUnit] = 'Miles' THEN [Final Odometer] - [Starting Odometer]
        WHEN [OdometerUnit] = 'Kilometers' THEN ([Final Odometer] - [Starting Odometer]) * 0.621371
        ELSE 0
    END AS [Total Miles Traveled],
    [IsActiveForChampionship],
    [CreatedAt],
    [UpdatedAt]
FROM [dbo].[Vehicles];

-- ============================================
-- vw_MemberGeneralRanking
-- Agrupa por miembro, suma millas de motos activas
-- ============================================
CREATE VIEW [dbo].[vw_MemberGeneralRanking]
AS
WITH MilesByVehicle AS (
    SELECT
        [MemberId],
        [Id] AS [VehicleId],
        [Motorcycle Data],
        [Lic Plate],
        [IsActiveForChampionship],
        CASE 
            WHEN [Final Odometer] IS NULL OR [Starting Odometer] IS NULL THEN 0
            WHEN [OdometerUnit] = 'Miles' THEN [Final Odometer] - [Starting Odometer]
            WHEN [OdometerUnit] = 'Kilometers' THEN ([Final Odometer] - [Starting Odometer]) * 0.621371
            ELSE 0
        END AS [Total Miles]
    FROM [dbo].[Vehicles]
    WHERE [IsActiveForChampionship] = 1
)
SELECT
    m.[Id] AS [MemberId],
    m.[Complete Names],
    m.[Country Birth],
    m.[Dama],
    m.[STATUS],
    ISNULL(SUM(mv.[Total Miles]), 0) AS [Total Miles All Vehicles],
    COUNT(DISTINCT mv.[VehicleId]) AS [Active Vehicles Count],
    STRING_AGG(CONCAT(mv.[Lic Plate], ' (', FORMAT(mv.[Total Miles], '0.00'), ' mi)'), ', ') AS [Vehicles Breakdown]
FROM [dbo].[Members] m
LEFT JOIN MilesByVehicle mv ON m.[Id] = mv.[MemberId]
GROUP BY 
    m.[Id],
    m.[Complete Names],
    m.[Country Birth],
    m.[Dama],
    m.[STATUS];
