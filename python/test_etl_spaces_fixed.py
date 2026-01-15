#!/usr/bin/env python3
"""
Script para validar que los espacios iniciales en Excel se detectan correctamente
"""

import pandas as pd
import sys
from pathlib import Path

def test_excel_column_detection():
    """Prueba la detección de espacios iniciales en nombres de columnas"""
    
    excel_path = r"C:\Users\DanielVillamizar\COR L.A.MA\INSUMOS\(COL) PEREIRA CORTE NACIONAL.xlsx"
    
    if not Path(excel_path).exists():
        print("Archivo no encontrado: {}".format(excel_path))
        return False
    
    try:
        # Leer Excel con header en fila 7 (índice 0-based, así que header=7)
        df = pd.read_excel(excel_path, sheet_name='ODOMETER', header=7, keep_default_na=False)
        
        print("Archivo leído exitosamente: {}".format(excel_path))
        print("Sheet: ODOMETER")
        print("Filas: {}".format(len(df)))
        print("Columnas: {}".format(len(df.columns)))
        print("\n" + "="*80)
        print("ANALISIS DE COLUMNAS CON ESPACIOS INICIALES")
        print("="*80 + "\n")
        
        # Analizar cada columna
        columns_with_spaces = []
        columns_without_spaces = []
        
        for col in df.columns:
            if isinstance(col, str):
                has_space = col.startswith(' ')
                
                if has_space:
                    columns_with_spaces.append(col)
                    print("[ESPACIO] '{}'".format(col))
                else:
                    columns_without_spaces.append(col)
        
        print("\n" + "="*80)
        print("RESUMEN")
        print("="*80)
        print("Columnas con espacio inicial: {}".format(len(columns_with_spaces)))
        for col in columns_with_spaces:
            print("  - '{}'".format(col))
        
        print("\nColumnas sin espacio inicial: {}".format(len(columns_without_spaces)))
        for col in columns_without_spaces[:10]:
            print("  - '{}'".format(col))
        if len(columns_without_spaces) > 10:
            print("  ... y {} mas".format(len(columns_without_spaces) - 10))
        
        # Validación contra el listado esperado
        expected_with_spaces = [
            ' Complete Names',
            ' Country Birth',
            ' In Lama Since',
            ' Motorcycle Data',
            ' Lic Plate',
            ' Trike',
            ' Starting Odometer',
            ' Final Odometer'
        ]
        
        print("\n" + "="*80)
        print("VALIDACION CONTRA LISTADO ESPERADO")
        print("="*80 + "\n")
        
        found_columns = []
        missing_columns = []
        
        for expected_col in expected_with_spaces:
            if expected_col in columns_with_spaces:
                found_columns.append(expected_col)
                print("[OK] {}".format(expected_col))
            else:
                col_without_space = expected_col.lstrip()
                if col_without_space in columns_without_spaces:
                    print("[SIN ESPACIO] {} -> encontrada sin espacio inicial".format(expected_col))
                else:
                    missing_columns.append(expected_col)
                    print("[NO ENCONTRADA] {}".format(expected_col))
        
        print("\n" + "="*80)
        print("RESULTADO: {}/{} columnas esperadas con espacios".format(len(found_columns), len(expected_with_spaces)))
        print("="*80 + "\n")
        
        if missing_columns:
            print("[ADVERTENCIA] Columnas faltantes:")
            for col in missing_columns:
                print("  - {}".format(col))
            return False
        
        print("[EXITO] Todas las columnas esperadas encontradas con espacios iniciales")
        return True
        
    except Exception as e:
        print("Error al leer Excel: {}".format(e))
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    success = test_excel_column_detection()
    sys.exit(0 if success else 1)
