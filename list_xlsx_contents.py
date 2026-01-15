import zipfile
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx')

print("=" * 70)
print("LISTANDO TODOS LOS ARCHIVOS DENTRO DEL XLSX")
print("=" * 70)

with zipfile.ZipFile(file_path) as zip_file:
    all_files = zip_file.namelist()
    
    print(f'\nTotal de archivos: {len(all_files)}\n')
    
    # Agrupar por tipo
    xml_files = [f for f in all_files if f.endswith('.xml')]
    rels_files = [f for f in all_files if f.endswith('.rels')]
    other_files = [f for f in all_files if not f.endswith('.xml') and not f.endswith('.rels')]
    
    print(f'Archivos XML ({len(xml_files)}):')
    for f in sorted(xml_files):
        print(f'  {f}')
    
    print(f'\nArchivos RELS ({len(rels_files)}):')
    for f in sorted(rels_files):
        print(f'  {f}')
    
    if other_files:
        print(f'\nOtros ({len(other_files)}):')
        for f in sorted(other_files):
            print(f'  {f}')
    
    # Buscar archivos de validación o extensiones especiales
    print(f'\n{"=" * 70}')
    print('ARCHIVOS POTENCIALES CON VALIDACIÓN:')
    print("=" * 70)
    
    for f in all_files:
        if 'validation' in f.lower() or 'macro' in f.lower() or 'vb' in f.lower() or 'custom' in f.lower():
            print(f'  - {f}')

print("\n" + "=" * 70)
