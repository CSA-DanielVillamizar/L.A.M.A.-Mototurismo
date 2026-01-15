import pandas as pd
from pathlib import Path
from datetime import datetime

print("=" * 70)
print("IMPORTACIÓN COMPLETA - TODOS LOS CAPÍTULOS CON TODAS LAS COLUMNAS")
print("=" * 70)

# Mapeo de archivos a ChapterId
files_chapters = [
    ('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx', 1, 'PEREIRA'),
    ('INSUMOS/(COL) MEDELLÍN CORTE NACIONAL.xlsx', 2, 'MEDELLÍN'),
    ('INSUMOS/(COL) SABANA CORTE NACIONAL.xlsx', 3, 'SABANA'),
    ('INSUMOS/(COL) CUCUTA CORTE NACIONAL.xlsx', 4, 'CUCUTA'),
    ('INSUMOS/(COL) CARTAGENA CORTE NACIONAL.xlsx', 5, 'CARTAGENA'),
    ('INSUMOS/(COL) BUCARAMANGA CORTE NACIONAL.xlsx', 6, 'BUCARAMANGA'),
]

all_members = []
all_vehicles = []
order_counter = 1
used_lic_plates = set()  # Para detectar duplicados

for file_path_str, chapter_id, chapter_name in files_chapters:
    file_path = Path(file_path_str)
    
    if not file_path.exists():
        print(f'\n[SKIP] {file_path.name}')
        continue
    
    print(f'\n[OK] Leyendo: {file_path.name}')
    
    # Leer Excel
    df = pd.read_excel(file_path, sheet_name='ODOMETER', header=7, nrows=300)
    
    # Buscar columna Complete Names
    complete_col = None
    for col in df.columns:
        col_str = str(col).strip()
        if 'Complete' in col_str:
            complete_col = col
            break
    
    if complete_col is None:
        print(f'     [ERROR] No encontró Complete Names')
        continue
    
    # Buscar otras columnas importantes
    col_map = {
        'complete_names': complete_col,
        'order': None,
        'dama': None,
        'country_birth': None,
        'in_lama_since': None,
        'status': None,
        'motorcycle_data': None,
        'trike': None,
        'lic_plate': None,
        'photography': None,
        'starting_odometer': None,
        'final_odometer': None
    }
    
    # Mapear columnas
    for col in df.columns:
        col_str = str(col).strip().upper()
        if 'ORDER' in col_str and col_map['order'] is None:
            col_map['order'] = col
        elif 'DAMA' in col_str:
            col_map['dama'] = col
        elif 'COUNTRY' in col_str and 'BIRTH' in col_str:
            col_map['country_birth'] = col
        elif 'LAMA' in col_str and 'SINCE' in col_str:
            col_map['in_lama_since'] = col
        elif 'STATUS' in col_str:
            col_map['status'] = col
        elif 'MOTORCYCLE' in col_str and 'DATA' in col_str:
            col_map['motorcycle_data'] = col
        elif 'TRIKE' in col_str:
            col_map['trike'] = col
        elif 'LIC' in col_str or 'PLATE' in col_str:
            col_map['lic_plate'] = col
        elif 'PHOTO' in col_str:
            col_map['photography'] = col
        elif 'STARTING' in col_str and 'ODOMETER' in col_str:
            col_map['starting_odometer'] = col
        elif 'FINAL' in col_str and 'ODOMETER' in col_str:
            col_map['final_odometer'] = col
    
    print(f'     [OK] Columnas mapeadas')
    
    # Filtrar solo filas con nombres válidos
    df_clean = df[df[complete_col].notna()].copy()
    df_clean = df_clean[df_clean[complete_col].astype(str).str.strip() != '']
    
    if len(df_clean) == 0:
        print(f'     [WARNING] Sin miembros')
        continue
    
    print(f'     [OK] {len(df_clean)} miembros')
    
    # Procesar cada miembro
    for idx, row in df_clean.iterrows():
        # Datos del miembro
        complete_name = str(row[complete_col]).strip()
        dama = str(row[col_map['dama']]).strip().upper() if col_map['dama'] and pd.notna(row[col_map['dama']]) else 'NO'
        country_birth = str(row[col_map['country_birth']]).strip() if col_map['country_birth'] and pd.notna(row[col_map['country_birth']]) else 'COLOMBIA'
        
        # In Lama Since (extraer año)
        in_lama_since = None
        if col_map['in_lama_since'] and pd.notna(row[col_map['in_lama_since']]):
            val = row[col_map['in_lama_since']]
            if isinstance(val, datetime):
                in_lama_since = val.year
            elif isinstance(val, (int, float)):
                in_lama_since = int(val)
            else:
                try:
                    in_lama_since = int(str(val)[:4])
                except:
                    in_lama_since = 2025
        else:
            in_lama_since = 2025
        
        status = str(row[col_map['status']]).strip() if col_map['status'] and pd.notna(row[col_map['status']]) else 'ACTIVE'
        
        # Datos del vehículo
        motorcycle_data = str(row[col_map['motorcycle_data']]).strip() if col_map['motorcycle_data'] and pd.notna(row[col_map['motorcycle_data']]) else None
        trike = str(row[col_map['trike']]).strip().upper() if col_map['trike'] and pd.notna(row[col_map['trike']]) else 'NO'
        lic_plate = str(row[col_map['lic_plate']]).strip() if col_map['lic_plate'] and pd.notna(row[col_map['lic_plate']]) else None
        
        # Manejar placas duplicadas
        if lic_plate:
            if lic_plate in used_lic_plates:
                # Agregar sufijo para hacerla única
                lic_plate = f"{lic_plate}_ORD{order_counter}"
            used_lic_plates.add(lic_plate)
        else:
            lic_plate = f'AUTO_ORD_{order_counter}'
        
        photography = str(row[col_map['photography']]).strip().upper() if col_map['photography'] and pd.notna(row[col_map['photography']]) else 'NO'
        
        starting_odometer = None
        if col_map['starting_odometer'] and pd.notna(row[col_map['starting_odometer']]):
            try:
                starting_odometer = float(row[col_map['starting_odometer']])
            except:
                pass
        
        final_odometer = None
        if col_map['final_odometer'] and pd.notna(row[col_map['final_odometer']]):
            try:
                final_odometer = float(row[col_map['final_odometer']])
            except:
                pass
        
        # Agregar miembro
        all_members.append({
            'chapter_id': chapter_id,
            'order': order_counter,
            'complete_name': complete_name,
            'dama': dama,
            'country_birth': country_birth,
            'in_lama_since': in_lama_since,
            'status': status
        })
        
        # Agregar vehículo si tiene datos
        if motorcycle_data or lic_plate:
            all_vehicles.append({
                'order': order_counter,  # Usaremos Order para vincular temporalmente
                'motorcycle_data': motorcycle_data,
                'lic_plate': lic_plate,
                'trike': trike,
                'photography': photography,
                'starting_odometer': starting_odometer,
                'final_odometer': final_odometer
            })
        
        order_counter += 1

print(f'\n{"=" * 70}')
print(f'TOTAL: {len(all_members)} miembros, {len(all_vehicles)} vehículos')
print(f'{"=" * 70}')

# Generar SQL
print('\nGenerando SQL...')

sql_lines = []
sql_lines.append("-- IMPORTACIÓN COMPLETA - MIEMBROS Y VEHÍCULOS")
sql_lines.append("SET NOCOUNT ON;")
sql_lines.append("BEGIN TRANSACTION;")
sql_lines.append("")
sql_lines.append("BEGIN TRY")
sql_lines.append("")

# INSERTs de Members
sql_lines.append("-- ===== MEMBERS =====")
for member in all_members:
    safe_name = member['complete_name'].replace("'", "''")
    safe_country = member['country_birth'].replace("'", "''")
    safe_status = member['status'].replace("'", "''")
    safe_dama = member['dama'].replace("'", "''")
    
    sql = f"""INSERT INTO [dbo].[Members]
    ([ChapterId], [Order], [ Complete Names], [Dama], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES
    ({member['chapter_id']}, {member['order']}, N'{safe_name}', N'{safe_dama}',
     N'{safe_country}', {member['in_lama_since']}, N'{safe_status}', 1);"""
    
    sql_lines.append(sql)

sql_lines.append("")
sql_lines.append("-- ===== VEHICLES =====")

# INSERTs de Vehicles (vinculados por Order)
for vehicle in all_vehicles:
    # Obtener MemberId basado en Order
    sql_lines.append(f"""
DECLARE @MemberId_{vehicle['order']} INT;
SELECT @MemberId_{vehicle['order']} = [MemberId] FROM [dbo].[Members] WHERE [Order] = {vehicle['order']};
""")
    
    safe_motorcycle = vehicle['motorcycle_data'].replace("'", "''") if vehicle['motorcycle_data'] else ''
    safe_lic_plate = vehicle['lic_plate'].replace("'", "''") if vehicle['lic_plate'] else f'AUTO_ORD_{vehicle["order"]}'
    safe_photo = vehicle['photography'].replace("'", "''")
    
    trike_bit = 1 if vehicle['trike'] == 'SI' else 0
    starting_odo = vehicle['starting_odometer'] if vehicle['starting_odometer'] is not None else 'NULL'
    final_odo = vehicle['final_odometer'] if vehicle['final_odometer'] is not None else 'NULL'
    
    sql = f"""INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [Photography], [Starting Odometer], [Final Odometer], [IsActiveForChampionship])
VALUES
    (@MemberId_{vehicle['order']}, N'{safe_motorcycle}', N'{safe_lic_plate}',
     {trike_bit}, N'{safe_photo}', {starting_odo}, {final_odo}, 1);"""
    
    sql_lines.append(sql)

sql_lines.append("")
sql_lines.append("COMMIT TRANSACTION;")
sql_lines.append("PRINT 'Importación completa exitosa.';")
sql_lines.append("")
sql_lines.append("END TRY")
sql_lines.append("BEGIN CATCH")
sql_lines.append("    ROLLBACK TRANSACTION;")
sql_lines.append("    THROW;")
sql_lines.append("END CATCH;")

# Guardar
output_file = 'migration_complete_all_data.sql'
with open(output_file, 'w', encoding='utf-8-sig') as f:
    f.write('\n'.join(sql_lines))

print(f'\n[OK] Script generado: {output_file}')
print(f'[OK] {len(all_members)} Members + {len(all_vehicles)} Vehicles')
print("=" * 70)
