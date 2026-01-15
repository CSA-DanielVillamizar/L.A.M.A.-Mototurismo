from pathlib import Path
import zipfile

f = Path('INSUMOS/(COL) SABANA CORTE NACIONAL.xlsx')

print(f'Archivo: {f.name}')
print(f'Existe: {f.exists()}')
print(f'Tamaño: {f.stat().st_size:,} bytes ({f.stat().st_size / (1024*1024):.2f} MB)')

print('\nIntentando abrir como ZIP (XLSX son archivos ZIP):')
try:
    with zipfile.ZipFile(f) as z:
        print(f'✓ Es ZIP válido')
        print(f'  Archivos internos: {len(z.namelist())}')
        
        # Buscar workbook.xml
        has_workbook = any('workbook' in name.lower() for name in z.namelist())
        print(f'  Tiene workbook.xml: {has_workbook}')
        
        if has_workbook:
            print('\nIntentando leer con pandas...')
            import pandas as pd
            try:
                df = pd.read_excel(f, sheet_name='ODOMETER', header=7, nrows=10)
                print(f'✓ Pandas puede leer el archivo')
                print(f'  Columnas: {len(df.columns)}')
                print(f'  Primeras columnas: {list(df.columns[:5])}')
            except Exception as e2:
                print(f'✗ Pandas error: {e2}')
        
except Exception as e:
    print(f'✗ ERROR ZIP: {e}')
    print('\nEl archivo puede estar corrupto o en formato incorrecto.')
