import zipfile
import xml.etree.ElementTree as ET
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx')

print("=" * 70)
print("BUSCANDO LISTA DESPLEGABLE EN TODAS LAS HOJAS")
print("=" * 70)

with zipfile.ZipFile(file_path) as zip_file:
    # Listar todas las hojas disponibles
    xml_files = [f for f in zip_file.namelist() if f.startswith('xl/worksheets/') and f.endswith('.xml')]
    
    print(f'\nHojas encontradas: {len(xml_files)}')
    for xml_file in sorted(xml_files):
        print(f'  - {xml_file}')
    
    # Buscar en cada hoja
    ns = {'x': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'}
    
    for xml_file in xml_files:
        worksheet_xml = zip_file.read(xml_file)
        root = ET.fromstring(worksheet_xml)
        
        validations = root.findall('.//x:dataValidation', ns)
        
        if validations:
            print(f'\n{"=" * 70}')
            print(f'VALIDACIONES EN: {xml_file}')
            print("=" * 70)
            
            for dv in validations:
                sqref = dv.get('sqref')
                dv_type = dv.get('type')
                allow = dv.get('allow')
                formula1_elem = dv.find('x:formula1', ns)
                formula2_elem = dv.find('x:formula2', ns)
                
                print(f'\nRango: {sqref}')
                print(f'  type: {dv_type}')
                print(f'  allow: {allow}')
                
                if formula1_elem is not None and formula1_elem.text:
                    text = formula1_elem.text
                    # Si es una lista separada por comas
                    if ',' in text:
                        values = [v.strip().strip('"').strip("'") for v in text.split(',')]
                        print(f'  Formula1 (lista): {len(values)} valores')
                        for v in values:
                            if v:
                                print(f'    • {v}')
                    else:
                        print(f'  Formula1: {text}')
                
                if formula2_elem is not None:
                    print(f'  Formula2: {formula2_elem.text}')

print("\n" + "=" * 70)
