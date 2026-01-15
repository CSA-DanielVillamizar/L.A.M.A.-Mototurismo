import pandas as pd
from pathlib import Path

chapters_config = {
    '(COL) PEREIRA CORTE NACIONAL.xlsx': {'id': 1},
    '(COL) MEDELLÍN CORTE NACIONAL.xlsx': {'id': 2},
    '(COL) SABANA CORTE NACIONAL.xls': {'id': 3},
    '(COL) CUCUTA CORTE NACIONAL.xlsx': {'id': 4},
    '(COL) CARTAGENA CORTE NACIONAL.xlsx': {'id': 5},
    '(COL) BUCARAMANGA CORTE NACIONAL.xlsx': {'id': 6},
}

insumos_path = Path('INSUMOS')
all_members = {}

print('='*70)
print('IMPORTACIÓN DE MIEMBROS - TODOS LOS CAPÍTULOS')
print('='*70)

for filename, chapter_info in chapters_config.items():
    file_path = insumos_path / filename
    chapter_id = chapter_info['id']
    
    if not file_path.exists():
        print(f'\n[ERROR] No existe: {filename}')
        continue
    
    print(f'\n[OK] Leyendo: {filename}')
    
    try:
        # Saltar .xls por ahora
        if filename.endswith('.xls'):
            print(f'     [WARNING] Archivo XLS - requiere conversión')
            continue
        
        # Leer con header en fila 8 (index=7)
        df = pd.read_excel(file_path, sheet_name='ODOMETER', header=7, nrows=300)
        
        # Buscar columna Complete Names (con o sin espacio)
        # Priorizar "Complete" sobre "NAME" para evitar "Unnamed"
        complete_col = None
        for col in df.columns:
            col_str = str(col).strip()
            if 'Complete' in col_str:
                complete_col = col
                break
        
        # Si no encuentra "Complete", buscar "Names" específicamente
        if complete_col is None:
            for col in df.columns:
                col_str = str(col).strip().upper()
                if 'NAMES' in col_str and 'UNNAMED' not in col_str:
                    complete_col = col
                    break
        
        if complete_col is None:
            print(f'     [ERROR] No encontró Complete Names')
            continue
        
        # Limpiar
        df_clean = df[[complete_col]].copy()
        df_clean = df_clean[df_clean[complete_col].notna()]
        df_clean[complete_col] = df_clean[complete_col].astype(str).str.strip()
        df_clean = df_clean[df_clean[complete_col] != '']
        df_clean = df_clean.drop_duplicates()
        
        if len(df_clean) == 0:
            print(f'     [WARNING] Sin miembros')
            continue
        
        members_list = df_clean[complete_col].tolist()
        all_members[chapter_id] = members_list
        
        print(f'     [OK] {len(members_list)} miembros')
        for member in members_list[:3]:
            print(f'        - {member}')
        if len(members_list) > 3:
            print(f'        ... +{len(members_list)-3}')
            
    except Exception as e:
        print(f'     [ERROR] {str(e)}')

# Generar SQL
print(f'\n{"="*70}')
print('GENERANDO SQL')
print(f'{"="*70}')

sql_lines = []
order_counter = 1

for chapter_id in sorted(all_members.keys()):
    members = all_members[chapter_id]
    print(f'\nCapítulo {chapter_id}: {len(members)} miembros')
    
    for member in members:
        safe_name = member.replace("'", "''")
        sql = f"""INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    ({chapter_id}, {order_counter}, N'{safe_name}', 
     N'COLOMBIA', 
     2025, 'ACTIVE', 1);"""
        sql_lines.append(sql)
        order_counter += 1

# Guardar
with open('migration_all_chapters.sql', 'w', encoding='utf-8-sig') as f:
    f.write('-- IMPORTACIÓN DE MIEMBROS\n')
    f.write('SET NOCOUNT ON;\n')
    f.write('BEGIN TRANSACTION;\n\n')
    f.write('BEGIN TRY\n\n')
    f.write('\n'.join(sql_lines))
    f.write('\n\nCOMMIT TRANSACTION;\n')
    f.write("PRINT 'Miembros importados exitosamente.';\n\n")
    f.write('END TRY\n')
    f.write('BEGIN CATCH\n')
    f.write("    PRINT 'ERROR: ' + ERROR_MESSAGE();\n")
    f.write('    ROLLBACK TRANSACTION;\n')
    f.write('    THROW;\n')
    f.write('END CATCH;\n')

print(f'\n{"="*70}')
print(f'[OK] Script: migration_all_chapters.sql')
print(f'[OK] Total INSERTs: {len(sql_lines)}')
print(f'{"="*70}')
