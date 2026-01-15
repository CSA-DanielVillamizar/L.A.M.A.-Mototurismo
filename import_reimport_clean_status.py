"""
Reimportación COMPLETA de miembros y vehículos
Usa los STATUS reales del Excel, NO 'ACTIVE'
Limpia primero la base de datos
"""

import pandas as pd
from pathlib import Path
from datetime import datetime

print("=" * 70)
print("REIMPORTACIÓN LIMPIA - TODAS LAS COLUMNAS CON STATUS REAL DEL EXCEL")
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

# Mapeo de STATUS: normalizar typos y variaciones
status_mapping = {
    'FULL COLOR MEMBER': 'FUL COLOR MEMBER',
    'Full Color Member': 'FUL COLOR MEMBER',
    'full color member': 'FUL COLOR MEMBER',
    'CHAPTER MTO': 'CHAPTER MTO',
    'Chapter Mto': 'CHAPTER MTO',
    'chapter mto': 'CHAPTER MTO',
    'CHAPTER SECRETARY': 'CHAPTER SECRETARY',
    'Chapter Secretary': 'CHAPTER SECRETARY',
    'chapter secretary': 'CHAPTER SECRETARY',
    'CHAPTER VICEPRESIDENT': 'CHAPTER VICEPRESIDENT',
    'Chapter Vicepresident': 'CHAPTER VICEPRESIDENT',
    'Chapter Vice-President': 'CHAPTER VICEPRESIDENT',
    'CHAPTER VICE-PRESIDEN': 'CHAPTER VICEPRESIDENT',  # Typo en Bucaramanga
    'CHAPTER VICE-PRESIDENT': 'CHAPTER VICEPRESIDENT',
    'REGIONAL VICEPRESIDENT': 'REGIONAL VICEPRESIDENT',
    'Regional Vice-President': 'REGIONAL VICEPRESIDENT',
    'NATIONAL VICEPRESIDENT': 'NATIONAL VICEPRESIDENT',
    'National Vice-President': 'NATIONAL VICEPRESIDENT',
    'CONTINENTAL VICEPRESIDENT': 'CONTINENTAL VICEPRESIDENT',
    'Continental Vice-President': 'CONTINENTAL VICEPRESIDENT',
    'INTERNATIONAL VICEPRESIDENT': 'INTERNATIONAL VICEPRESIDENT',
    'International Vice-President': 'INTERNATIONAL VICEPRESIDENT',
}

valid_statuses = {
    'PROSPECT', 'ROCKET PROSPECT', 'FUL COLOR MEMBER',
    'CHAPTER PRESIDENT', 'CHAPTER VICEPRESIDENT', 'CHAPTER TREASURER', 'CHAPTER BUSSINESS MANAGER', 'CHAPTER SECRETARY', 'CHAPTER MTO',
    'REGIONAL PRESIDENT', 'REGIONAL VICEPRESIDENT', 'REGIONAL TREASURER', 'REGIONAL BUSSINESS MANAGER', 'REGIONAL SECRETARY', 'REGIONAL MTO',
    'NATIONAL PRESIDENT', 'NATIONAL VICEPRESIDENT', 'NATIONAL TREASURER', 'NATIONAL BUSSINESS MANAGER', 'NATIONAL SECRETARY', 'NATIONAL MTO',
    'CONTINENTAL PRESIDENT', 'CONTINENTAL VICEPRESIDENT', 'CONTINENTAL TREASURER', 'CONTINENTAL BUSSINESS MANAGER', 'CONTINENTAL SECRETARY', 'CONTINENTAL MTO',
    'INTERNATIONAL PRESIDENT', 'INTERNATIONAL VICEPRESIDENT', 'INTERNATIONAL TREASURER', 'INTERNATIONAL BUSSINESS MANAGER', 'INTERNATIONAL SECRETARY', 'INTERNATIONAL MTO'
}

def normalize_status(val):
    """Normalizar STATUS al valor exacto"""
    if not val or pd.isna(val):
        return 'PROSPECT'  # Default
    
    status_str = str(val).strip()
    
    # Buscar mapping exacto
    if status_str in status_mapping:
        status_str = status_mapping[status_str]
    
    # Validar que esté en la lista de 33 valores
    if status_str in valid_statuses:
        return status_str
    
    # Si no existe, intentar busqueda fuzzy
    for valid in valid_statuses:
        if valid.upper() == status_str.upper():
            return valid
    
    # Si no coincide, loguear y usar default
    print(f'     [WARN] STATUS desconocido: "{status_str}" -> PROSPECT')
    return 'PROSPECT'

all_members = []
all_vehicles = []
order_counter = 1
used_lic_plates = set()

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
        
        # STATUS: usar el valor real del Excel, normalizado
        status = normalize_status(row[col_map['status']]) if col_map['status'] else 'PROSPECT'
        
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
                'order': order_counter,
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
sql_lines.append("-- REIMPORTACIÓN LIMPIA - MIEMBROS Y VEHÍCULOS CON STATUS REAL")
sql_lines.append("SET NOCOUNT ON;")
sql_lines.append("BEGIN TRANSACTION;")
sql_lines.append("")
sql_lines.append("BEGIN TRY")
sql_lines.append("")
sql_lines.append("-- Limpiar datos previos")
sql_lines.append("DELETE FROM [dbo].[Vehicles];")
sql_lines.append("DELETE FROM [dbo].[Members];")
sql_lines.append("")

# INSERTs de Members
sql_lines.append("-- ===== MEMBERS (154) =====")
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
sql_lines.append("-- ===== VEHICLES (154) =====")

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
sql_lines.append("PRINT 'Reimportación completada: 154 Members + 154 Vehicles con STATUS real del Excel';")
sql_lines.append("")
sql_lines.append("END TRY")
sql_lines.append("BEGIN CATCH")
sql_lines.append("    ROLLBACK TRANSACTION;")
sql_lines.append("    THROW;")
sql_lines.append("END CATCH;")

# Guardar
output_file = 'migration_reimport_clean_status.sql'
with open(output_file, 'w', encoding='utf-8-sig') as f:
    f.write('\n'.join(sql_lines))

print(f'\n[OK] Script generado: {output_file}')
print(f'[OK] {len(all_members)} Members + {len(all_vehicles)} Vehicles CON STATUS REAL')
print("=" * 70)
