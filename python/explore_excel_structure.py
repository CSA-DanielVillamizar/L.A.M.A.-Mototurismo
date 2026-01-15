#!/usr/bin/env python3
"""
Script para explorar la estructura exacta del Excel y encontrar el header correcto
"""

import pandas as pd
from pathlib import Path

excel_path = r"C:\Users\DanielVillamizar\COR L.A.MA\INSUMOS\(COL) PEREIRA CORTE NACIONAL.xlsx"

print("="*80)
print("EXPLORACIÓN DE ESTRUCTURA DEL EXCEL")
print("="*80)

# Leer sin header para ver toda la estructura
df_raw = pd.read_excel(excel_path, sheet_name='ODOMETER', header=None)

print(f"\nDimensiones totales: {df_raw.shape[0]} filas x {df_raw.shape[1]} columnas\n")

# Mostrar primeras 15 filas para encontrar el header
print("Primeras 15 filas del Excel:")
print("-"*80)
for idx, row in df_raw.head(15).iterrows():
    print(f"Fila {idx}: {list(row)}")

print("\n" + "="*80)
print("BÚSQUEDA DE COLUMNA 'Order' (indicador del header)")
print("="*80 + "\n")

# Buscar dónde aparece 'Order'
for idx, row in df_raw.iterrows():
    for col_idx, val in enumerate(row):
        if isinstance(val, str) and 'Order' in val:
            print(f"✓ Encontrado 'Order' en: Fila {idx}, Columna {col_idx}")
            print(f"  Contenido de la fila {idx}: {list(row[:10])}")
            
            # Mostrar 3 filas alrededor
            print(f"\n  Contexto (filas alrededor):")
            for context_idx in range(max(0, idx-1), min(df_raw.shape[0], idx+3)):
                print(f"    Fila {context_idx}: {list(df_raw.iloc[context_idx][:10])}")
            break
    else:
        continue
    break
