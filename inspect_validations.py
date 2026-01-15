import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx')

print("=" * 70)
print("INSPECCIONANDO VALIDACIONES DE DATOS - BÚSQUEDA AVANZADA")
print("=" * 70)

with zipfile.ZipFile(file_path) as zip_file:
    worksheet_xml = zip_file.read('xl/worksheets/sheet1.xml')

# Parsear sin namespaces para búsqueda simple
root = ET.fromstring(worksheet_xml)

# Buscar todos los elementos dataValidation
print('\nTodas las validaciones en el archivo:\n')

# Con namespace completo
ns = {'x': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'}

for dv in root.findall('.//x:dataValidation', ns):
    sqref = dv.get('sqref')
    dv_type = dv.get('type')
    allow = dv.get('allow')
    formula1_elem = dv.find('x:formula1', ns)
    formula2_elem = dv.find('x:formula2', ns)
    
    print(f'Rango: {sqref}')
    print(f'  type: {dv_type}')
    print(f'  allow: {allow}')
    
    if formula1_elem is not None:
        print(f'  formula1: {formula1_elem.text}')
    if formula2_elem is not None:
        print(f'  formula2: {formula2_elem.text}')
    
    # Mostrar todos los atributos
    if dv.attrib:
        print(f'  Otros atributos: {dv.attrib}')
    
    print()

# Verificar si hay hojas de datos o rangos nombrados
print("\n" + "=" * 70)
print("BUSCANDO RANGOS NOMBRADOS Y REFERENCIAS")
print("=" * 70)

# Ver workbook.xml para named ranges
try:
    with zipfile.ZipFile(file_path) as zip_file:
        workbook_xml = zip_file.read('xl/workbook.xml')
    
    wb_root = ET.fromstring(workbook_xml)
    ns_wb = {'x': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'}
    
    named_ranges = wb_root.findall('.//x:definedName', ns_wb)
    if named_ranges:
        print('\nRangos nombrados encontrados:')
        for nr in named_ranges:
            name = nr.get('name')
            content = nr.text
            print(f'  {name}: {content}')
    else:
        print('\nNo hay rangos nombrados definidos.')
        
except Exception as e:
    print(f'Error al leer workbook: {e}')

print("\n" + "=" * 70)
