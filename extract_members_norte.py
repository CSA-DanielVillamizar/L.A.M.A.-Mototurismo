import pandas as pd
import sys

file_path = 'INSUMOS/(COL) INDIVIDUAL REPORT - REGION NORTE.xlsm'
df = pd.read_excel(file_path, sheet_name='DATOS', header=7)

# Mapeo de capítulos a ChapterId
chapter_mapping = {
    '(COL) MEDELLIN': 2,
    '(COL) MEDELLIN ': 2,  # Con espacio
    '(COL) SABANA': 3,
    '(COL) CÚCUTA': 4,
    '(COL) CUCUTA': 4,  # Sin tilde
    '(COL) CARTAGENA': 5,
    '(COL) BUCARAMANGA': 6
}

# Filtrar solo los capítulos que necesitamos
chapters_of_interest = [
    '(COL) MEDELLIN',
    '(COL) MEDELLIN ',
    '(COL) SABANA',
    '(COL) CÚCUTA',
    '(COL) CUCUTA',
    '(COL) CARTAGENA',
    '(COL) BUCARAMANGA'
]

# Limpiar datos
df_clean = df[df['CHAPTER'].notna() & (df['MEMBER NAME'].notna())].copy()
df_clean['MEMBER NAME'] = df_clean['MEMBER NAME'].str.strip()
df_clean['CHAPTER'] = df_clean['CHAPTER'].str.strip()

# Filtrar por capítulos de interés
df_filtered = df_clean[df_clean['CHAPTER'].isin(chapters_of_interest)].copy()

# Eliminar duplicados por nombre dentro del mismo capítulo
df_filtered = df_filtered.drop_duplicates(subset=['MEMBER NAME', 'CHAPTER'])

print(f'[OK] Total de miembros para capítulos nuevos: {len(df_filtered)}')

# Agrupar por capítulo
for chapter in chapters_of_interest:
    chapter_members = df_filtered[df_filtered['CHAPTER'] == chapter]
    if len(chapter_members) > 0:
        chapter_id = chapter_mapping.get(chapter, None)
        if chapter_id:
            print(f'\n[OK] Capítulo: {chapter} (ChapterId={chapter_id}) - {len(chapter_members)} miembros')
            for idx, row in chapter_members.head(3).iterrows():
                print(f'     - {row["MEMBER NAME"]}')

# Generar script SQL
sql_inserts = []

# Agrupar miembros por capítulo y ChapterId
chapter_groups = {}
for chapter in chapters_of_interest:
    chapter_members = df_filtered[df_filtered['CHAPTER'] == chapter]
    chapter_id = chapter_mapping.get(chapter, None)
    if chapter_id and len(chapter_members) > 0:
        if chapter_id not in chapter_groups:
            chapter_groups[chapter_id] = []
        chapter_groups[chapter_id].extend(chapter_members['MEMBER NAME'].tolist())

# Crear inserts con orden secuencial por capítulo
order_counter = 1
for chapter_id in sorted(chapter_groups.keys()):
    members = chapter_groups[chapter_id]
    for member_name in members:
        # Escapar comillas simples
        safe_name = member_name.replace("'", "''")
        insert_sql = f"""INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    ({chapter_id}, {order_counter}, N'{safe_name}', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);"""
        sql_inserts.append(insert_sql)
        order_counter += 1

# Guardar script
output_file = 'migration_members_norte.sql'
with open(output_file, 'w', encoding='utf-8-sig') as f:
    f.write("-- ============================================\n")
    f.write("-- INSERCIÓN MIEMBROS - REGIÓN NORTE\n")
    f.write("-- Auto-generado por extract_members_norte.py\n")
    f.write("-- ============================================\n\n")
    f.write("SET NOCOUNT ON;\n")
    f.write("BEGIN TRANSACTION;\n\n")
    f.write("BEGIN TRY\n\n")
    f.write("\n".join(sql_inserts))
    f.write("\n\nCOMMIT TRANSACTION;\n")
    f.write("PRINT 'Miembros Región Norte insertados exitosamente.';\n\n")
    f.write("END TRY\n")
    f.write("BEGIN CATCH\n")
    f.write("    ROLLBACK TRANSACTION;\n")
    f.write("    PRINT 'ERROR en inserción: ' + ERROR_MESSAGE();\n")
    f.write("    THROW;\n")
    f.write("END CATCH;\n")

print(f'\n[OK] Script generado: {output_file}')
print(f'[OK] Total de INSERTs: {len(sql_inserts)}')
