#!/usr/bin/env python3
import pandas as pd
import os

os.chdir(r"c:\Users\DanielVillamizar\COR L.A.MA\INSUMOS")

file_path = "(COL) PEREIRA CORTE NACIONAL.xlsx"
df = pd.read_excel(file_path, sheet_name="ODOMETER", header=6)

print("\nCOLUMNAS ENCONTRADAS:")
print("=" * 100)

for i, col in enumerate(df.columns, 1):
    col_str = str(col)
    ascii_first = ord(col_str[0])
    len_col = len(col_str)
    has_space = "✓ ESPACIO INICIAL" if ascii_first == 32 else ""
    print(f"{i:2d}. '{col_str:40s}' len={len_col:3d} ASCII[0]={ascii_first:3d} {has_space}")

print("\n" + "=" * 100)
print(f"Total: {len(df.columns)} columnas")
