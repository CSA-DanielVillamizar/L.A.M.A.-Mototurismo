import pandas as pd
from pathlib import Path
import subprocess

chapters_config = {
    '(COL) PEREIRA CORTE NACIONAL.xlsx': {'id': 1, 'name': '(COL) PEREIRA CORTE NACIONAL'},
    '(COL) MEDELLÍN CORTE NACIONAL.xlsx': {'id': 2, 'name': '(COL) MEDELLÍN CORTE NACIONAL'},
    '(COL) SABANA CORTE NACIONAL.xls': {'id': 3, 'name': '(COL) SABANA CORTE NACIONAL'},
    '(COL) CUCUTA CORTE NACIONAL.xlsx': {'id': 4, 'name': '(COL) CÚCUTA CORTE NACIONAL'},
    '(COL) CARTAGENA CORTE NACIONAL.xlsx': {'id': 5, 'name': '(COL) CARTAGENA CORTE NACIONAL'},
    '(COL) BUCARAMANGA CORTE NACIONAL.xlsx': {'id': 6, 'name': '(COL) BUCARAMANGA CORTE NACIONAL'},
}

insumos_path = Path('INSUMOS')
all_members = {}
order_counter = 1

print('='*70)
print('IMPORTACIÓN DE MIEMBROS - TODOS LOS CAPÍTULOS')
print('='*70)

for filename, chapter_info in chapters_config.items():
    file_path = insumos_path / filename
    chapter_id = chapter_info['id']
    chapter_name = chapter_info['name']
    
    if not file_path.exists():
        print(f'\n[ERROR] Archivo no encontrado: {filename}')
        continue
    
    print(f'\n[OK] Leyendo: {filename}')
    print(f'     ChapterId: {chapter_id}, Nombre: {chapter_name}')
    
    try:
        # Para .xls, convertir a xlsx primero si es necesario
        if filename.endswith('.xls'):
            print(f'     [INFO] Convirtiendo XLS a XLSX...')
            # Intentar leer con xlrd si está disponible, sino saltar
            try:
                import xlrd
                df = pd.read_excel(file_path, sheet_name='ODOMETER', header=7)
            except ImportError:
                print(f'     [WARNING] xlrd no disponible, intentando con openpyxl + libreoffice...')
                # Convertir con libreoffice
                output_path = file_path.with_suffix('.xlsx')
                result = subprocess.run(
                    ['soffice', '--headless', '--convert-to', 'xlsx', '--outdir', str(insumos_path), str(file_path)],
                    capture_output=True, timeout=30
                )
                if result.returncode == 0:
                    df = pd.read_excel(output_path, sheet_name='ODOMETER', header=7)
                else:
                    print(f'     [ERROR] No se pudo convertir XLS')
                    continue
        else:
            # Leer xlsx normalmente - leer desde fila 10 en adelante (después de header en fila 8)
            df = pd.read_excel(file_path, sheet_name='ODOMETER', skiprows=9)
        
        # Las primeras dos columnas no nos interesan (None y Order), buscamos Complete Names
        # Identificar la columna correcta
        complete_names_col = None
        for col in df.columns:
            if 'Complete' in str(col) or 'NAME' in str(col).upper():
                complete_names_col = col
                break
        
        if complete_names_col is None:
            print(f'     [ERROR] No se encontró columna Complete Names')
            continue
        
        if len(df_clean) == 0:
            print(f'     [WARNING] No se encontraron miembros válidos')
            continue
        
        # Almacenar miembros
        all_members[chapter_id] = {
            'chapter_name': chapter_name,
            'members': df_clean[complete_names_col].tolist()
        }
        
        print(f'     [OK] {len(df_clean)} miembros encontrados')
        for idx, member in enumerate(df_clean[complete_names_col].head(3).tolist(), 1):
            print(f'        {idx}. {member}')
        if len(df_clean) > 3:
            print(f'        ... y {len(df_clean) - 3} más')
            
    except Exception as e:
        print(f'     [ERROR] {str(e)}')
        import traceback
        traceback.print_exc()

# Generar script SQL
print(f'\n\n' + '='*70)
print('GENERANDO SCRIPT SQL')
print('='*70)

sql_inserts = []
order_counter = 1

for chapter_id in sorted(all_members.keys()):
    chapter_info = all_members[chapter_id]
    chapter_name = chapter_info['chapter_name']
    members = chapter_info['members']
    
    print(f'\nCapítulo {chapter_id} ({chapter_name}): {len(members)} miembros')
    
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
output_file = 'migration_all_chapters.sql'
with open(output_file, 'w', encoding='utf-8-sig') as f:
    f.write("-- ============================================\n")
    f.write("-- INSERCIÓN MIEMBROS - TODOS LOS CAPÍTULOS\n")
    f.write("-- Auto-generado por import_all_chapters.py\n")
    f.write("-- Codificación: UTF-8 con BOM\n")
    f.write("-- ============================================\n\n")
    f.write("SET NOCOUNT ON;\n")
    f.write("BEGIN TRANSACTION;\n\n")
    f.write("BEGIN TRY\n\n")
    f.write("\n".join(sql_inserts))
    f.write("\n\nCOMMIT TRANSACTION;\n")
    f.write("PRINT 'Todos los miembros de capítulos importados exitosamente.';\n\n")
    f.write("END TRY\n")
    f.write("BEGIN CATCH\n")
    f.write("    ROLLBACK TRANSACTION;\n")
    f.write("    PRINT 'ERROR en inserción: ' + ERROR_MESSAGE();\n")
    f.write("    THROW;\n")
    f.write("END CATCH;\n")

print(f'\n\n' + '='*70)
print(f'[OK] Script generado: {output_file}')
print(f'[OK] Total de INSERTs: {len(sql_inserts)}')
print(f'[OK] Capítulos procesados: {len(all_members)}')
print(f'[OK] Codificación: UTF-8 con BOM (para acentos)')
print('='*70)
