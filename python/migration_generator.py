#!/usr/bin/env python3
"""
Script de migracion: Lee Excel y genera INSERT SQL para Members y Vehicles
"""

import pandas as pd
import sys
from pathlib import Path
from typing import List, Tuple, Dict

class MigrationGenerator:
    def __init__(self, excel_path: str):
        self.excel_path = excel_path
        self.members = {}  # Dict de members por Order
        self.vehicles = []  # Lista de vehiculos
        
    def read_excel(self) -> pd.DataFrame:
        """Lee el Excel y retorna DataFrame con datos de odometro"""
        try:
            df = pd.read_excel(self.excel_path, sheet_name='ODOMETER', header=7, keep_default_na=False)
            print(f"[OK] Leyendo Excel: {self.excel_path}")
            print(f"[OK] Columnas encontradas:")
            for col in df.columns:
                has_space = col.startswith(' ') if isinstance(col, str) else False
                marker = "[ESPACIO]" if has_space else ""
                print(f"  - '{col}' {marker}")
            return df
        except FileNotFoundError:
            print(f"[ERROR] Archivo no encontrado {self.excel_path}")
            sys.exit(1)
        except KeyError:
            print(f"[ERROR] Hoja 'ODOMETER' no encontrada")
            sys.exit(1)
    
    def safe_parse_int(self, value) -> int:
        """Parsea un valor de forma segura a int, retorna None si no es posible"""
        try:
            if pd.isna(value) or value == '' or str(value).strip() == '':
                return None
            # Si es datetime, extraer year
            if hasattr(value, 'year'):
                return value.year
            # Si es string o numero, convertir
            return int(float(str(value).strip()))
        except (ValueError, TypeError, AttributeError):
            return None
    
    def escape_sql_string(self, value: str) -> str:
        """Escapa caracteres especiales para SQL"""
        if value is None or (isinstance(value, float) and pd.isna(value)):
            return "NULL"
        value = str(value).replace("'", "''")
        return f"N'{value}'"
    
    def find_odometer_column(self, df: pd.DataFrame, prefix: str) -> str:
        """Busca columna que contenga el prefijo dado"""
        for col in df.columns:
            if isinstance(col, str) and col.startswith(prefix):
                return col
        return None

    def generate_members_inserts(self, df: pd.DataFrame) -> List[str]:
        """Genera INSERTs para Members SOLO con columnas que existen en schema"""
        inserts = []
        
        order_col = 'Order'
        complete_names_col = ' Complete Names'
        country_birth_col = 'Country Birth'
        in_lama_since_col = 'In Lama Since'
        
        if order_col not in df.columns or complete_names_col not in df.columns:
            print(f"[ERROR] Columnas requeridas no encontradas")
            return inserts
        
        for _, row in df.drop_duplicates(subset=[order_col]).iterrows():
            order = self.safe_parse_int(row[order_col])
            if not order or order == 0:
                continue
            
            complete_names = row.get(complete_names_col, 'Unknown')
            country_birth = row.get(country_birth_col, None) if country_birth_col in df.columns else None
            in_lama_since = self.safe_parse_int(row[in_lama_since_col]) if in_lama_since_col in df.columns else None
            
            self.members[order] = {
                'Order': order,
                'CompleteNames': complete_names,
                'CountryBirth': country_birth,
                'InLamaSince': in_lama_since
            }
            
            insert_sql = f"""INSERT INTO [dbo].[Members] 
    ([ChapterId], [Order], [ Complete Names], [Country Birth], [In Lama Since], [STATUS], [is_eligible])
VALUES 
    (1, {order}, {self.escape_sql_string(str(complete_names))}, 
     {self.escape_sql_string(str(country_birth)) if country_birth else 'NULL'}, 
     {in_lama_since if in_lama_since else 'NULL'}, 'ACTIVE', 1);"""
            
            inserts.append(insert_sql.strip())
        
        return inserts

    def generate_vehicles_inserts(self, df: pd.DataFrame) -> List[str]:
        """Genera INSERTs para Vehicles SOLO con columnas que existen en schema"""
        inserts = []
        
        order_col = 'Order'
        motorcycle_data_col = ' Motorcycle Data'
        lic_plate_col = 'Lic Plate'
        trike_col = 'Trike'
        
        required_cols = [order_col, motorcycle_data_col, lic_plate_col]
        missing_cols = [col for col in required_cols if col not in df.columns]
        
        if missing_cols:
            print(f"[ERROR] Columnas faltantes: {missing_cols}")
            return inserts
        
        starting_odo_col = self.find_odometer_column(df, 'Starting Odometer')
        final_odo_col = self.find_odometer_column(df, 'Final Odometer')
        
        if not starting_odo_col:
            print(f"[WARNING] No se encontro columna 'Starting Odometer'")
        if not final_odo_col:
            print(f"[WARNING] No se encontro columna 'Final Odometer'")
        
        for idx, row in df.iterrows():
            order = self.safe_parse_int(row[order_col])
            if not order or order == 0 or order not in self.members:
                continue
            
            motorcycle_data = row.get(motorcycle_data_col, 'Unknown')
            lic_plate_raw = row.get(lic_plate_col, '')
            # Si Lic Plate esta vacia, generar valor AUTO basado en Order para unicidad
            if not lic_plate_raw or (isinstance(lic_plate_raw, str) and not lic_plate_raw.strip()):
                lic_plate_sql = self.escape_sql_string(f'AUTO_ORD_{order}')
            else:
                lic_plate_sql = self.escape_sql_string(str(lic_plate_raw).strip())
            trike = 1 if trike_col in df.columns and pd.notna(row.get(trike_col, None)) and row[trike_col] else 0
            
            starting_odo = None
            final_odo = None
            
            if starting_odo_col:
                starting_odo = row.get(starting_odo_col, None)
            if final_odo_col:
                final_odo = row.get(final_odo_col, None)
            
            starting_reading = starting_odo if pd.notna(starting_odo) and starting_odo > 0 else 0
            final_reading = final_odo if pd.notna(final_odo) and final_odo > 0 else 0
            
            insert_sql = f"""INSERT INTO [dbo].[Vehicles]
    ([MemberId], [ Motorcycle Data], [Lic Plate], [Trike], [OdometerUnit], 
     [Starting Odometer], [Final Odometer], [Photography], [IsActiveForChampionship])
VALUES
    ((SELECT TOP 1 [MemberId] FROM [dbo].[Members] WHERE [Order] = {order} ORDER BY [MemberId] DESC), 
     {self.escape_sql_string(str(motorcycle_data))}, 
     {lic_plate_sql}, 
     {trike}, 'Miles',
     {starting_reading if starting_reading > 0 else 'NULL'}, 
     {final_reading if final_reading > 0 else 'NULL'}, 
     'PENDING', 1);"""
            
            inserts.append(insert_sql.strip())
        
        return inserts
    
    def generate_migration_script(self, output_path: str = "migration_script.sql"):
        """Genera el script SQL completo de migracion"""
        df = self.read_excel()
        
        print(f"\n{'='*60}")
        print("Generando script de migracion...")
        print(f"{'='*60}\n")
        
        members_inserts = self.generate_members_inserts(df)
        vehicles_inserts = self.generate_vehicles_inserts(df)
        
        print(f"[OK] {len(members_inserts)} Members para INSERT")
        print(f"[OK] {len(vehicles_inserts)} Vehicles para INSERT")
        
        script_content = """-- ============================================
-- LAMA MOTOTURISMO - MIGRATION SCRIPT
-- Auto-generado por migration_generator.py
-- ============================================

SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY

-- Deshabilitar triggers temporalmente
-- ALTER TABLE [dbo].[Vehicles] DISABLE TRIGGER [tr_MaxTwoActiveVehiclesPerMember];

-- ============================================
-- INSERT MEMBERS
-- ============================================
"""
        
        script_content += "\n".join(members_inserts) + "\n\n"
        
        script_content += """-- ============================================
-- INSERT VEHICLES
-- ============================================
"""
        
        script_content += "\n".join(vehicles_inserts) + "\n\n"
        
        script_content += """-- Habilitar triggers nuevamente
-- ALTER TABLE [dbo].[Vehicles] ENABLE TRIGGER [tr_MaxTwoActiveVehiclesPerMember];

COMMIT TRANSACTION;
PRINT 'Migracion completada exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'ERROR en migracion: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
"""
        
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(script_content)
        
        print(f"\n[OK] Script generado: {output_path}")
        print(f"[OK] Total de lineas: {len(script_content.splitlines())}")
        return output_path


def main():
    base_path = Path(__file__).parent.parent / "INSUMOS" / "(COL) PEREIRA CORTE NACIONAL.xlsx"
    
    alternate_paths = [
        base_path,
        Path("c:\\Users\\DanielVillamizar\\COR L.A.MA\\INSUMOS\\(COL) PEREIRA CORTE NACIONAL.xlsx"),
        Path("./INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx"),
    ]
    
    excel_path = None
    for path in alternate_paths:
        if path.exists():
            excel_path = str(path)
            print(f"[OK] Excel encontrado: {excel_path}")
            break
    
    if not excel_path:
        print(f"[ERROR] Archivo Excel no encontrado")
        sys.exit(1)
    
    generator = MigrationGenerator(excel_path)
    output = generator.generate_migration_script()
    
    print(f"\n{'='*60}")
    print("[OK] Migracion lista para ejecutar")
    print(f"{'='*60}\n")
    print(f"Ejecutar en SQL Server:")
    print(f"  sqlcmd -S P-DVILLAMIZARA -d LamaDb -i {output}\n")


if __name__ == "__main__":
    main()
