#!/usr/bin/env python3
"""
Script para generar MAPPING_REPORT.md validando que los nombres de columnas
del ETL coincidan exactamente con el Excel real
"""

import pandas as pd
import sys
from pathlib import Path

def generate_mapping_report():
    """Genera reporte de mapping entre Excel y ETL"""
    
    excel_path = r"C:\Users\DanielVillamizar\COR L.A.MA\INSUMOS\(COL) PEREIRA CORTE NACIONAL.xlsx"
    
    if not Path(excel_path).exists():
        print("ERROR: Archivo no encontrado: {}".format(excel_path))
        return False
    
    try:
        # Leer Excel con header en fila 7 (0-based = header=7)
        df = pd.read_excel(excel_path, sheet_name='ODOMETER', header=7, keep_default_na=False)
        
        # Columnas ESPERADAS por el ETL (nombres reales del Excel)
        expected_etl_columns = [
            'Order',
            ' Complete Names',      # CON espacio - Excel real tiene
            'Dama',
            'Country Birth',        # SIN espacio - Excel real tiene
            'In Lama Since',        # SIN espacio - Excel real tiene
            'STATUS',
            ' Motorcycle Data',     # CON espacio - Excel real tiene
            'Trike',                # SIN espacio - Excel real tiene
            'Lic Plate',            # SIN espacio - Excel real tiene
            'Photography',
            'Starting Odometer',    # SIN espacio - Excel real tiene
            'Final Odometer'        # SIN espacio - Excel real tiene
        ]
        
        # Analizar columnas reales del Excel
        excel_cols = list(df.columns)
        
        print("=" * 80)
        print("MAPPING REPORT: Excel Real vs ETL Implementation")
        print("=" * 80)
        print("\nFuente: (COL) PEREIRA CORTE NACIONAL.xlsx")
        print("Sheet: ODOMETER")
        print("Header Row: Fila 8 (0-based index=7)\n")
        
        print("=" * 80)
        print("COLUMNAS DEL EXCEL REAL (Total: {})".format(len(excel_cols)))
        print("=" * 80)
        
        for idx, col in enumerate(excel_cols, 1):
            has_space = col.startswith(' ') if isinstance(col, str) else False
            marker = "[ESPACIO]" if has_space else "[SIN ESP]"
            print("{:2d}. {} '{}'".format(idx, marker, col))
        
        print("\n" + "=" * 80)
        print("VALIDACION: Columnas Esperadas vs Reales")
        print("=" * 80 + "\n")
        
        found = 0
        missing = 0
        
        for expected in expected_etl_columns:
            if expected in excel_cols:
                print("[OK] Encontrada: '{}'".format(expected))
                found += 1
            else:
                print("[FALTA] No encontrada: '{}'".format(expected))
                missing += 1
        
        print("\n" + "=" * 80)
        print("RESULTADO")
        print("=" * 80)
        print("Columnas esperadas: {}".format(len(expected_etl_columns)))
        print("Columnas encontradas: {}".format(found))
        print("Columnas faltantes: {}".format(missing))
        
        if missing == 0:
            print("\nESTADO: EXITO - Mapeo 100% coincide con Excel real")
            print("=" * 80)
            return True
        else:
            print("\nESTADO: ERROR - Hay diferencias")
            print("=" * 80)
            return False
        
    except Exception as e:
        print("ERROR: {}".format(e))
        import traceback
        traceback.print_exc()
        return False


if __name__ == "__main__":
    success = generate_mapping_report()
    sys.exit(0 if success else 1)
