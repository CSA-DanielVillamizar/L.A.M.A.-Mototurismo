import pandas as pd
from pathlib import Path

print("=" * 70)
print("IMPORTACIÓN SABANA CORTE NACIONAL")
print("=" * 70)

file_path = Path('INSUMOS/(COL) SABANA CORTE NACIONAL.xlsx')
chapter_id = 3
chapter_name = "(COL) SABANA CORTE NACIONAL"

# Leer Excel XLSX
print(f"\n[OK] Leyendo: {file_path.name}")
df = pd.read_excel(file_path, sheet_name='ODOMETER', header=7, nrows=300)

# Buscar columna Complete Names
complete_col = None
for col in df.columns:
    col_str = str(col).strip()
    if 'Complete' in col_str:
        complete_col = col
        break

if complete_col is None:
    for col in df.columns:
        col_str = str(col).strip().upper()
        if 'NAMES' in col_str and 'UNNAMED' not in col_str:
            complete_col = col
            break

if complete_col is None:
    print(f'     [ERROR] No encontró Complete Names')
    exit(1)

print(f'     [OK] Columna: {complete_col}')

# Limpiar datos
df_clean = df[[complete_col]].copy()
df_clean = df_clean[df_clean[complete_col].notna()]
df_clean[complete_col] = df_clean[complete_col].astype(str).str.strip()
df_clean = df_clean[df_clean[complete_col] != '']
df_clean = df_clean.drop_duplicates()

if len(df_clean) == 0:
    print(f'     [WARNING] Sin miembros')
    exit(1)

members_list = df_clean[complete_col].tolist()
print(f'     [OK] {len(members_list)} miembros')
for member in members_list[:5]:
    print(f'        - {member}')
if len(members_list) > 5:
    print(f'        ... +{len(members_list)-5}')

# Obtener último Order de la BD
import subprocess
result = subprocess.run(
    ['sqlcmd', '-S', 'P-DVILLAMIZARA', '-d', 'LamaDb', '-Q', 
     'SELECT ISNULL(MAX([Order]), 0) AS MaxOrder FROM [dbo].[Members];', '-h', '-1', '-W'],
    capture_output=True, text=True
)
# Limpiar output de sqlcmd (quitar espacios y líneas extra)
output_lines = [line.strip() for line in result.stdout.split('\n') if line.strip()]
max_order = int(output_lines[0]) if output_lines else 0
print(f'\n[OK] Último Order en BD: {max_order}')

# Generar SQL
print("\n" + "=" * 70)
print("GENERANDO SQL")
print("=" * 70)

sql_lines = []
sql_lines.append("-- IMPORTACIÓN SABANA CORTE NACIONAL")
sql_lines.append("SET NOCOUNT ON;")
sql_lines.append("BEGIN TRANSACTION;")
sql_lines.append("")
sql_lines.append("BEGIN TRY")
sql_lines.append("")

order_counter = max_order + 1

for member in members_list:
    safe_name = member.replace("'", "''")
    
    sql_insert = f"""INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    ({chapter_id}, {order_counter}, N'{safe_name}',
     N'COLOMBIA',
     2025, 'ACTIVE', 1);"""
    
    sql_lines.append(sql_insert)
    order_counter += 1

sql_lines.append("")
sql_lines.append("COMMIT TRANSACTION;")
sql_lines.append("PRINT 'Miembros de Sabana importados exitosamente.';")
sql_lines.append("")
sql_lines.append("END TRY")
sql_lines.append("BEGIN CATCH")
sql_lines.append("    ROLLBACK TRANSACTION;")
sql_lines.append("    THROW;")
sql_lines.append("END CATCH;")

# Guardar con UTF-8 BOM
output_file = 'migration_sabana.sql'
with open(output_file, 'w', encoding='utf-8-sig') as f:
    f.write('\n'.join(sql_lines))

print(f"\n[OK] Script: {output_file}")
print(f"[OK] Total INSERTs: {len(members_list)}")
print("=" * 70)
