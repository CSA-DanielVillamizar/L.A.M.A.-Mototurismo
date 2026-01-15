#!/usr/bin/env python3
"""
Leer encabezados exactos del Excel COR L.A.MA
Validar espacios iniciales y caracteres especiales
"""
import pandas as pd
import os
import sys

# Cambiar a directorio INSUMOS
os.chdir(r"c:\Users\DanielVillamizar\COR L.A.MA\INSUMOS")

# Mostrar archivos disponibles
print("\n📁 ARCHIVOS EN INSUMOS:")
print("=" * 80)
for f in sorted(os.listdir('.')):
    if f.endswith(('.xlsx', '.xlsm')):
        size = os.path.getsize(f)
        print(f"  {f:50s} ({size:,} bytes)")

print("\n" + "=" * 80)
print("EXTRAYENDO ENCABEZADOS EXACTOS")
print("=" * 80)

file_path = "(COL) PEREIRA CORTE NACIONAL.xlsx"
sheet = "ODOMETER"
header_row = 6  # header=6 significa row 7 (0-indexed)

try:
    # Leer con pandas
    df = pd.read_excel(file_path, sheet_name=sheet, header=header_row)
    
    print(f"\n📄 Archivo: {file_path}")
    print(f"📊 Sheet: {sheet}")
    print(f"📍 Header Row: 7 (fila visible, index=6)")
    print(f"📈 Total columnas: {len(df.columns)}")
    print(f"📊 Total filas datos: {len(df)}")
    
    print("\n" + "=" * 130)
    print("ANÁLISIS DETALLADO DE ENCABEZADOS:")
    print("=" * 130)
    
    headers_with_space = []
    
    for i, col in enumerate(df.columns, 1):
        if col is None or (isinstance(col, float) and pd.isna(col)):
            print(f"{i:2d}. [NULL/NaN]")
            continue
            
        col_str = str(col)
        ascii_first = ord(col_str[0]) if col_str else 0
        len_col = len(col_str)
        has_space = ascii_first == 32
        
        if has_space:
            headers_with_space.append(col_str)
            marker = "⚠️ ESPACIO INICIAL"
        else:
            marker = ""
        
        # Mostrar representación con markers visuales
        repr_col = repr(col_str)
        print(f"{i:2d}. {repr_col:50s} | len={len_col:3d} | ASCII[0]={ascii_first:3d} {marker}")
    
    print("\n" + "=" * 130)
    print("RESUMEN:")
    print("=" * 130)
    print(f"Total de columnas: {len(df.columns)}")
    print(f"Columnas con ESPACIO INICIAL: {len(headers_with_space)}")
    
    if headers_with_space:
        print("\n⚠️ COLUMNAS CON ESPACIO INICIAL (CRÍTICAS):")
        for col in headers_with_space:
            print(f"   • '{col}'")
    
    # Mostrar primeras filas
    print("\n📋 PRIMERAS 3 FILAS DE DATOS:")
    print(df.head(3).to_string())
    
except FileNotFoundError as e:
    print(f"❌ ERROR: Archivo no encontrado - {e}")
    sys.exit(1)
except Exception as e:
    print(f"❌ ERROR: {e}")
    import traceback
    traceback.print_exc()
    sys.exit(1)

print("\n✅ LECTURA COMPLETADA")
