#!/usr/bin/env python3
"""
Script de prueba para validar que la migración respeta espacios iniciales
en los nombres de columnas leídas desde Excel
"""

import pandas as pd
import sys
from pathlib import Path

def test_excel_column_detection():
    """Prueba la detección de espacios iniciales en nombres de columnas"""
    
    excel_path = r"C:\Users\DanielVillamizar\COR L.A.MA\INSUMOS\(COL) PEREIRA CORTE NACIONAL.xlsx"
    
    if not Path(excel_path).exists():
        print(f"Archivo no encontrado: {excel_path}")
        return False
    
    try:
        # Leer Excel con header en fila 7 (índice 0-based, así que header=7)
        df = pd.read_excel(excel_path, sheet_name='ODOMETER', header=7, keep_default_na=False)
        
        print(f"✓ Archivo leído exitosamente: {excel_path}")
        print(f"✓ Sheet: ODOMETER")
        print(f"✓ Filas: {len(df)}")
        print(f"✓ Columnas: {len(df.columns)}")
        print(f"\n{'='*80}")
        print("ANÁLISIS DE COLUMNAS CON ESPACIOS INICIALES")
        print(f"{'='*80}\n")
        
        # Analizar cada columna
        columns_with_spaces = []
        columns_without_spaces = []
        
        for col in df.columns:
            if isinstance(col, str):
                has_space = col.startswith(' ')
                has_special = any(c in col for c in ['(', ')', '/', ':'])
                
                if has_space:
                    columns_with_spaces.append(col)
                    print(f"ESPACIO INICIAL: '{col}'")
                    # Mostrar los primeros caracteres en ASCII
                    first_chars = [f"{ord(c)}({chr(c)})" for c in col[:5]]
                    print(f"  ASCII primeros 5 caracteres: {' '.join(first_chars)}")
                else:
                    columns_without_spaces.append(col)
        
        print(f"\n{'='*80}")
        print("RESUMEN")
        print(f"{'='*80}")
        print(f"Columnas con espacio inicial: {len(columns_with_spaces)}")
        for col in columns_with_spaces:
            print(f"  - '{col}'")
        
        print(f"\nColumnas sin espacio inicial: {len(columns_without_spaces)}")
        for col in columns_without_spaces[:10]:  # Mostrar primeras 10
            print(f"  - '{col}'")
        if len(columns_without_spaces) > 10:
            print(f"  ... y {len(columns_without_spaces) - 10} mas")
        
        # Validación contra el listado esperado
        expected_with_spaces = [
            ' Complete Names',
            ' Country Birth',
            ' In Lama Since',
            ' Motorcycle Data',
            ' Lic Plate',
            ' Trike',
            ' Starting Odometer',
            ' Final Odometer',
            ' Event Start Date (AAAA/MM/DD)',
            ' Name of the event',
            ' Mileage',
            ' Points per event',
            ' Points per Distance',
            ' Points awarded per member',
            ' Visitor Class'
        ]
        
        print(f"\n{'='*80}")
        print("VALIDACIÓN CONTRA LISTADO ESPERADO")
        print(f"{'='*80}\n")
        
        found_columns = []
        missing_columns = []
        
        for expected_col in expected_with_spaces:
            if expected_col in columns_with_spaces:
                found_columns.append(expected_col)
                print(f"ENCONTRADA: {expected_col}")
            else:
                # Buscar si existe sin espacio
                col_without_space = expected_col.lstrip()
                if col_without_space in columns_without_spaces:
                    print(f"SIN ESPACIO: {expected_col} -> encontrada sin espacio inicial")
                else:
                    missing_columns.append(expected_col)
                    print(f"NO ENCONTRADA: {expected_col}")
        
        print(f"\n{'='*80}")
        print(f"RESULTADO: {len(found_columns)}/{len(expected_with_spaces)} columnas esperadas")
        print(f"{'='*80}\n")
        
        if missing_columns:
            print(f"⚠️  Columnas faltantes:")
            for col in missing_columns:
                print(f"  - {col}")
        
        return len(missing_columns) == 0
        
    except Exception as e:
        print(f"✗ Error al leer Excel: {e}")
        return False


if __name__ == "__main__":
    success = test_excel_column_detection()
    sys.exit(0 if success else 1)
