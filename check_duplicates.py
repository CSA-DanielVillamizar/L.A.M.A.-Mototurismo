import pandas as pd

file_path = 'INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx'
df = pd.read_excel(file_path, sheet_name='ODOMETER', header=7)

# Mostrar duplicados en Lic Plate
lic_plates = df['Lic Plate'].dropna()
duplicados = lic_plates[lic_plates.duplicated(keep=False)]

if len(duplicados) > 0:
    print('Placas duplicadas encontradas:')
    for idx, row in df[df['Lic Plate'].isin(duplicados)].iterrows():
        print(f"  Order={row.get('Order', 'N/A')}, Lic Plate='{row.get('Lic Plate', 'N/A')}'")
else:
    print('No hay duplicados de Lic Plate')

# Mostrar NULLs/vacios
print('\n\nFilas con Lic Plate vacia:')
empty_count = 0
for idx, row in df[df['Lic Plate'].isna() | (df['Lic Plate'].astype(str).str.strip() == '')].iterrows():
    order = row.get('Order', 'N/A')
    lic_plate = row.get('Lic Plate', 'N/A')
    print(f"  Order={order}, Lic Plate='{lic_plate}'")
    empty_count += 1

print(f"\nTotal de Lic Plate vacias: {empty_count}")
print(f"Total de Members: {len(df)}")
