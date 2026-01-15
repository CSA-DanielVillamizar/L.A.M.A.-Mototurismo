import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx')

print("=" * 70)
print("EXTRAYENDO LISTA DESPLEGABLE COMPLETA DE STATUS")
print("=" * 70)

# Los XLSX son archivos ZIP que contienen XML
with zipfile.ZipFile(file_path) as zip_file:
    # Leer el worksheet XML
    worksheet_xml = zip_file.read('xl/worksheets/sheet1.xml')

# Parsear el XML
root = ET.fromstring(worksheet_xml)

# Namespaces
namespaces = {
    'mc': 'http://schemas.openxmlformats.org/markup-compatibility/2006',
    'r': 'http://schemas.openxmlformats.org/officeDocument/2006/relationships',
    'x': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'
}

# Buscar dataValidations
print('\nBuscando validaciones de datos en el XML...\n')

# Encontrar el elemento dataValidations
found_validation = False
for dataValidations in root.findall('.//x:dataValidations', namespaces):
    for dataValidation in dataValidations.findall('x:dataValidation', namespaces):
        sqref = dataValidation.get('sqref')
        val_type = dataValidation.get('type')
        
        # Buscar validaciones que apliquen a columna G (STATUS)
        if sqref and 'G' in sqref:
            found_validation = True
            print(f'Validación encontrada:')
            print(f'  Rango: {sqref}')
            print(f'  Tipo: {val_type}')
            
            # Buscar elemento formula1
            formula1 = dataValidation.find('x:formula1', namespaces)
            if formula1 is not None and formula1.text:
                print(f'\nLista desplegable (valores separados por coma):')
                # La lista viene como "valor1,valor2,..." o con saltos de línea
                values = formula1.text.split(',')
                
                # Limpiar y mostrar valores
                status_list = []
                for val in values:
                    cleaned = val.strip().strip('"').strip("'")
                    if cleaned:
                        status_list.append(cleaned)
                        print(f'  • {cleaned}')
                
                print(f'\nTotal de valores en la lista: {len(status_list)}')
                
                # Mostrar los que NO están siendo usados
                print('\n' + '=' * 70)
                print('COMPARACIÓN CON VALORES ACTUALMENTE EN LA BD')
                print('=' * 70)
                
                import subprocess
                result = subprocess.run(
                    ['sqlcmd', '-S', 'P-DVILLAMIZARA', '-d', 'LamaDb',
                     '-Q', 'SELECT DISTINCT [STATUS] FROM [dbo].[Members] ORDER BY [STATUS]', 
                     '-h', '-1', '-W'],
                    capture_output=True, text=True
                )
                
                used_values = set()
                for line in result.stdout.strip().split('\n'):
                    line = line.strip()
                    if line and not line.startswith('-'):
                        used_values.add(line)
                
                print(f'\nValores EN LA BD: {len(used_values)}')
                for val in sorted(used_values):
                    print(f'  ✓ {val}')
                
                unused = set(status_list) - used_values
                print(f'\nValores EN LISTA PERO NO EN BD: {len(unused)}')
                for val in sorted(unused):
                    print(f'  ⚠ {val}')
                
                all_in_list = set(status_list)
                extra_in_bd = used_values - all_in_list
                if extra_in_bd:
                    print(f'\nValores EN BD PERO NO EN LISTA: {len(extra_in_bd)}')
                    for val in sorted(extra_in_bd):
                        print(f'  ❌ {val}')

if not found_validation:
    print('[INFO] No se encontró validación en columna G')
    print('\nIntentando buscar TODAS las validaciones en el archivo...')
    
    for dataValidations in root.findall('.//x:dataValidations', namespaces):
        for dataValidation in dataValidations.findall('x:dataValidation', namespaces):
            sqref = dataValidation.get('sqref')
            val_type = dataValidation.get('type')
            print(f'\nValidación encontrada en: {sqref} (tipo: {val_type})')

print("\n" + "=" * 70)
