import pandas as pd
import openpyxl
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

for filename in chapters_config.keys():
    file_path = insumos_path / filename
    
    if not file_path.exists():
        print(f'\n[ERROR] No existe: {filename}')
        continue
    
    print(f'\n{"="*70}')
    print(f'ARCHIVO: {filename}')
    print(f'{"="*70}')
    
    try:
        # Leer sin header para ver estructura
        if filename.endswith('.xls'):
            print('[INFO] Formato XLS - requiere xlrd')
            continue
        
        # Primero, obtener hojas disponibles
        xl_file = pd.ExcelFile(file_path)
        print(f'Hojas: {xl_file.sheet_names}')
        
        # Leer con diferentes configuraciones de header
        for header_row in [0, 7, None]:
            print(f'\n--- Intentando con header={header_row} ---')
            df = pd.read_excel(file_path, header=header_row, nrows=5)
            print(f'Columnas encontradas:')
            for col in df.columns:
                print(f'  - {col}')
            
            # Si header es None, mostrar primeras 5 filas
            if header_row is None:
                print('\nPrimeras 5 filas:')
                print(df.head())
            break
            
    except Exception as e:
        print(f'[ERROR] {str(e)}')
