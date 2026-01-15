import pandas as pd

df = pd.read_excel('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx', sheet_name='ODOMETER', header=7, nrows=300)

print('=== COLUMNAS ENCONTRADAS ===')
for i, col in enumerate(df.columns):
    col_str = str(col).strip()
    found_complete = 'Complete' in col_str
    found_name = 'NAME' in col_str.upper()
    print(f'{i:2d}: [{col}]')
    print(f'     Strip: [{col_str}]')
    print(f'     Complete: {found_complete}, NAME: {found_name}')
    if found_complete or found_name:
        print(f'     *** MATCH ***')

# Buscar igual que el script
complete_col = None
for col in df.columns:
    col_str = str(col).strip()
    if 'Complete' in col_str or 'NAME' in col_str.upper():
        complete_col = col
        break

print(f'\n=== RESULTADO BÚSQUEDA ===')
print(f'Columna encontrada: {complete_col}')

if complete_col:
    df_clean = df[[complete_col]].copy()
    df_clean = df_clean[df_clean[complete_col].notna()]
    df_clean[complete_col] = df_clean[complete_col].astype(str).str.strip()
    df_clean = df_clean[df_clean[complete_col] != '']
    df_clean = df_clean.drop_duplicates()
    
    print(f'Miembros después de limpieza: {len(df_clean)}')
    print('\nPrimeros 5:')
    print(df_clean.head(5))
