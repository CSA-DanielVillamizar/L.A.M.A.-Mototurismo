#!/bin/bash
# Script para comandos rápidos de LAMA Mototurismo

echo ""
echo "========================================"
echo "LAMA MOTOTURISMO - UTILIDADES"
echo "========================================"
echo ""
echo "1. Compilar solución"
echo "2. Ejecutar tests"
echo "3. Ejecutar API"
echo "4. Crear base de datos"
echo "5. Aplicar migraciones SQL"
echo "6. Generar migration script (Python)"
echo "7. Salir"
echo ""
read -p "Selecciona una opción (1-7): " choice

case $choice in
    1)
        echo "Compilando..."
        dotnet build
        ;;
    2)
        echo "Ejecutando tests..."
        dotnet test tests/Lama.UnitTests/
        ;;
    3)
        echo "Iniciando API en https://localhost:7001..."
        cd src/Lama.API
        dotnet run
        ;;
    4)
        echo "Creando base de datos LamaDb..."
        sqlcmd -S "(localdb)\\mssqllocaldb" -Q "CREATE DATABASE LamaDb;"
        echo "✓ Base de datos creada"
        ;;
    5)
        echo "Aplicando schema SQL..."
        sqlcmd -S "(localdb)\\mssqllocaldb" -d LamaDb -i sql/schema.sql
        echo ""
        echo "Aplicando views SQL..."
        sqlcmd -S "(localdb)\\mssqllocaldb" -d LamaDb -i sql/views.sql
        echo "✓ Migraciones aplicadas"
        ;;
    6)
        echo "Generando migration script..."
        cd python
        python migration_generator.py
        cd ..
        ;;
    7)
        echo "Saliendo..."
        exit 0
        ;;
    *)
        echo "Opción inválida"
        ;;
esac

echo ""
