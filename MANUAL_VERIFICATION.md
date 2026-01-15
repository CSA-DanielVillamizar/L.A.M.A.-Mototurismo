# ✅ VERIFICACIÓN MANUAL - QA CHECKLIST L.A.M.A.

**Instrucciones paso a paso para verificar que todo funciona correctamente**

---

## 📋 Verificación 1: Conectar a la BD

### Ejecutar en Terminal PowerShell:

```powershell
# Test 1: Conectar simple
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -Q "SELECT @@VERSION;"

# Resultado esperado:
# Microsoft SQL Server 2019 (RTM) - 15.0.2000.5 (X64) Standard Edition...
```

**Si pasa ✅**: BD está accesible

---

## 📊 Verificación 2: Validar Estructura de Tablas

### Ejecutar en Terminal:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME;"
```

**Resultado esperado:**
```
TABLE_NAME
─────────────────────
Attendance
Chapters
Configuration
Events
Members
Vehicles
(6 rows affected)
```

**Si pasa ✅**: 6 tablas presentes

---

## 📋 Verificación 3: Validar Campos en Vehicles

### Ejecutar en Terminal:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Vehicles' ORDER BY ORDINAL_POSITION;"
```

**Campos esperados:**
```
VehicleId
MemberId
Motorcycle Data
Lic Plate
Trike
OdometerUnit
Starting Odometer
Final Odometer
StartYearEvidenceUrl
StartYearEvidenceValidatedAt
CutOffEvidenceUrl
CutOffEvidenceValidatedAt
EvidenceValidatedBy
Photography
IsActiveForChampionship
CreatedAt
(16 rows)
```

**Si pasa ✅**: Todos los campos están presentes

---

## 📊 Verificación 4: Validar Datos de Prueba

### Miembros cargados:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT COUNT(*) AS MemberCount FROM Members;"
```

**Resultado esperado:** `7`

### Vehículos cargados:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT COUNT(*) AS VehicleCount FROM Vehicles;"
```

**Resultado esperado:** `9`

### Eventos cargados:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT COUNT(*) AS EventCount FROM Events;"
```

**Resultado esperado:** `5`

---

## 🔍 Verificación 5: Validar Conversión KM→Miles

### Ejecutar query en Terminal:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT VehicleId, [Lic Plate], [OdometerUnit], [Final Odometer] - [Starting Odometer] AS DistanceOriginal, CASE WHEN OdometerUnit='Kilometers' THEN ([Final Odometer] - [Starting Odometer]) * 0.621371 ELSE [Final Odometer] - [Starting Odometer] END AS DistanceInMiles FROM Vehicles WHERE [Lic Plate] IN ('HDS-001', 'HCB-500');"
```

**Resultado esperado:**
```
VehicleId  Lic Plate  OdometerUnit  DistanceOriginal  DistanceInMiles
─────────────────────────────────────────────────────────────────────
1          HDS-001    Miles         3000              3000
2          HCB-500    Kilometers    5000              3106.855
```

**Si pasa ✅**: Conversión correcta (5000 KM = 3106.86 millas)

---

## 🏆 Verificación 6: Validar Vista Multi-Moto

### Ejecutar en Terminal:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT MemberId, [Complete Names], [Total Miles All Vehicles], [Active Vehicles Count] FROM vw_MemberGeneralRanking WHERE MemberId=1;"
```

**Resultado esperado:**
```
MemberId  Complete Names     Total Miles All Vehicles  Active Vehicles Count
─────────────────────────────────────────────────────────────────────────────
1         Germánico García   6106.85                   2
```

**Si pasa ✅**: Vista suma correctamente (3000 + 3106.86)

---

## 🔐 Verificación 7: Validar Constraints

### Test: Max 2 motos por miembro

```powershell
# Intentar insertar una 3ra moto activa (debería fallar)
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "INSERT INTO Vehicles (MemberId, [Lic Plate], OdometerUnit, IsActiveForChampionship) VALUES (1, 'TEST-999', 'Miles', 1);"
```

**Resultado esperado:**
```
Msg 50000, Level 16, State 1
'Un miembro no puede tener más de 2 vehículos activos en el campeonato.'
```

**Si pasa ✅**: Trigger funciona correctamente

### Test: Unique constraint (EventId, MemberId)

```powershell
# Intentar insertar asistencia duplicada
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "INSERT INTO Attendance (EventId, MemberId, VehicleId, Status) VALUES (1, 1, 1, 'PENDING');"
```

**Resultado esperado:**
```
Msg 2627, Level 14, State 1
Violation of UNIQUE KEY constraint 'UQ_Attendance_EventMember'
```

**Si pasa ✅**: Unique constraint funciona correctamente

---

## 📊 Verificación 8: Validar Configuración

### Ejecutar en Terminal:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT [Key], [Value] FROM Configuration ORDER BY [Key];"
```

**Resultado esperado: (10 filas)**
```
Key                                    Value
────────────────────────────────────   ─────
DistanceThreshold_1Point_OneWayMiles   200
DistanceThreshold_2Points_OneWayMiles  800
PointsPerClassMultiplier_1             1
PointsPerClassMultiplier_2             3
PointsPerClassMultiplier_3             5
PointsPerClassMultiplier_4             10
PointsPerClassMultiplier_5             15
TripWindowDays_ABInheritance           15
VisitorBonus_DifferentContinent        2
VisitorBonus_SameContinent             1
```

**Si pasa ✅**: Configuración completa

---

## 🎯 Verificación 9: Validar Vistas

### Vistas existentes:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_TYPE='VIEW' ORDER BY TABLE_NAME;"
```

**Resultado esperado:**
```
TABLE_NAME
──────────────────────────
vw_MasterOdometerReport
vw_MemberGeneralRanking
```

**Si pasa ✅**: Vistas presentes

---

## 🔄 Verificación 10: Validar Triggers

### Triggers existentes:

```powershell
sqlcmd -S P-DVILLAMIZARA -U sa -P "Mc901128365-2**" -d LamaDb -Q "SELECT name FROM sys.triggers WHERE type='TR';"
```

**Resultado esperado:**
```
name
──────────────────────────────────────
tr_MaxTwoActiveVehiclesPerMember
```

**Si pasa ✅**: Trigger presente

---

## 🏁 CHECKLIST MANUAL - MARCAR MIENTRAS EJECUTAS

Copia esto y ve marcando según ejecutes cada verificación:

```
VERIFICACIÓN MANUAL - QA CHECKLIST L.A.M.A.

[ ] 1. Conectar a BD (VersionCheck)
[ ] 2. Validar 6 tablas presentes
[ ] 3. Validar 16 columnas en Vehicles
[ ] 4. Validar 7 miembros cargados
[ ] 5. Validar 9 vehículos cargados
[ ] 6. Validar 5 eventos cargados
[ ] 7. Validar conversión KM→Miles (5000 KM = 3106.86)
[ ] 8. Validar vista multi-moto (Germánico = 6106.86 total)
[ ] 9. Validar trigger max 2 motos (rechaza 3ra)
[ ] 10. Validar unique constraint (rechaza duplicado)
[ ] 11. Validar 10 parámetros en Configuration
[ ] 12. Validar 2 vistas presentes
[ ] 13. Validar 1 trigger presente

TOTAL: 13/13 ✅
```

---

## 📝 NOTAS

- Si alguna verificación falla, revisar los archivos de errores SQL
- Los datos de prueba son reales y están listos para usar
- Las vistas calculan sobre la marcha (no almacenan datos)
- Los triggers se ejecutan automáticamente

---

## 🎯 SI TODO PASA

Entonces la BD está 100% lista y puedes proceder a:

1. ✅ Compilar solución .NET (`dotnet build`)
2. ✅ Ejecutar tests unitarios
3. ✅ Probar endpoint AdminController
4. ✅ Deploy a producción

---

**Ejecutado:** 14 Enero 2026  
**Estatus:** Todas las verificaciones deberían PASAR ✅
