import pandas as pd

df = pd.read_excel('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx', 
                   sheet_name='ODOMETER', 
                   header=7, 
                   nrows=5)

print("=" * 70)
print("COLUMNAS DISPONIBLES EN EXCEL")
print("=" * 70)

for i, col in enumerate(df.columns, 1):
    print(f'{i:2d}. {col}')

print("\n" + "=" * 70)
print("DATOS DEL PRIMER MIEMBRO (Fila 1)")
print("=" * 70)

member_data = df.iloc[1]
for col in df.columns:
    value = member_data[col]
    if pd.notna(value):
        print(f'{col:40s}: {value}')
