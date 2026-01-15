@echo off
REM Batch file para comandos rápidos de LAMA Mototurismo

setlocal enabledelayedexpansion

echo.
echo ========================================
echo LAMA MOTOTURISMO - UTILIDADES
echo ========================================
echo.
echo 1. Compilar solución
echo 2. Ejecutar tests
echo 3. Ejecutar API
echo 4. Crear base de datos
echo 5. Aplicar migraciones SQL
echo 6. Generar migration script (Python)
echo 7. Salir
echo.

set /p choice="Selecciona una opción (1-7): "

if "%choice%"=="1" (
    echo Compilando...
    dotnet build
    goto end
)

if "%choice%"=="2" (
    echo Ejecutando tests...
    dotnet test tests\Lama.UnitTests\
    goto end
)

if "%choice%"=="3" (
    echo Iniciando API en https://localhost:7001...
    cd src\Lama.API
    dotnet run
    goto end
)

if "%choice%"=="4" (
    echo Creando base de datos LamaDb...
    sqlcmd -S (localdb)\mssqllocaldb -Q "CREATE DATABASE LamaDb;"
    echo ✓ Base de datos creada
    goto end
)

if "%choice%"=="5" (
    echo Aplicando schema SQL...
    sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\schema.sql
    echo.
    echo Aplicando views SQL...
    sqlcmd -S (localdb)\mssqllocaldb -d LamaDb -i sql\views.sql
    echo ✓ Migraciones aplicadas
    goto end
)

if "%choice%"=="6" (
    echo Generando migration script...
    cd python
    python migration_generator.py
    cd ..
    goto end
)

if "%choice%"=="7" (
    goto end
)

echo Opción inválida
goto end

:end
echo.
pause
