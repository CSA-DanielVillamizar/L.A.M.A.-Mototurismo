import pandas as pd

file_path = 'INSUMOS/(COL) INDIVIDUAL REPORT - REGION NORTE.xlsm'
df = pd.read_excel(file_path, sheet_name='DATOS', header=7)

# Limpiar datos
df_clean = df[df['CHAPTER'].notna() & (df['MEMBER NAME'].notna())].copy()
df_clean['MEMBER NAME'] = df_clean['MEMBER NAME'].str.strip()
df_clean['CHAPTER'] = df_clean['CHAPTER'].str.strip()

# Buscar capítulos específicos
chapters_search = [
    '(COL) MEDELLIN',
    '(COL) MEDELLIN ',
    '(COL) SABANA',
    '(COL) CÚCUTA',
    '(COL) CUCUTA',
    '(COL) CARTAGENA',
    '(COL) BUCARAMANGA'
]

total_members = 0
for chapter in chapters_search:
    members = df_clean[df_clean['CHAPTER'] == chapter]
    if len(members) > 0:
        print(f'\n{chapter}: {len(members)} miembros')
        for idx, row in members.iterrows():
            member_name = row['MEMBER NAME']
            print(f'  - {member_name}')
        total_members += len(members)

print(f'\n\nTOTAL EN ARCHIVO: {total_members} miembros')

# Verificar si hay SABANA pero con otro nombre
print('\n\n=== Búsqueda de SABANA ===')
sabana = df_clean[df_clean['CHAPTER'].str.contains('SABANA', case=False, na=False)]
print(f'Registros con SABANA: {len(sabana)}')
if len(sabana) > 0:
    print(sabana[['CHAPTER', 'MEMBER NAME']].head(10))
