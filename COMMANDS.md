# Comandos rápidos para LAMA Mototurismo

## Compilar
```bash
dotnet build
```

## Tests
```bash
dotnet test tests\Lama.UnitTests\
```

## Ejecutar API (local)
```bash
cd src\Lama.API
dotnet run
```

## Crear BD
```bash
sqlcmd -S (localdb)\mssqllocaldb -Q "CREATE DATABASE LamaDb;"
```

## Aplicar migraciones SQL
```bash
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\schema.sql
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\views.sql
```

## Generar migration script (Python)
```bash
cd python
python migration_generator.py
cd ..
sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i python\migration_script.sql
```

## Acceder Swagger UI
```
https://localhost:7001/swagger
```

## Probar endpoint con curl
```bash
curl -X POST "https://localhost:7001/api/admin/evidence/upload?eventId=1" \
  -F "memberId=1" \
  -F "vehicleId=1" \
  -F "evidenceType=START_YEAR" \
  -F "pilotWithBikePhoto=@test_pilot.jpg" \
  -F "odometerCloseupPhoto=@test_odometer.jpg" \
  -F "odometerReading=25000.5" \
  -F "unit=Miles" \
  --insecure
```

## Ver datos en SQL
```sql
-- Vistas de reporte
SELECT * FROM [dbo].[vw_MasterOdometerReport];
SELECT * FROM [dbo].[vw_MemberGeneralRanking];

-- Verificar configuración
SELECT * FROM [dbo].[Configuration];

-- Ver asistencias
SELECT * FROM [dbo].[Attendance];
```

## Limpiar proyecto
```bash
dotnet clean
rmdir /s /q bin obj
```
