import zipfile
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx')

print("=" * 70)
print("BUSCANDO LISTAS DESPLEGABLES EN ARCHIVOS VML")
print("=" * 70)

with zipfile.ZipFile(file_path) as zip_file:
    # VML files pueden contener comentarios y validaciones
    vml_files = [f for f in zip_file.namelist() if f.endswith('.vml')]
    
    print(f'\nArchivos VML encontrados: {len(vml_files)}\n')
    
    for vml_file in vml_files:
        print(f'Leyendo: {vml_file}')
        print("-" * 70)
        
        try:
            vml_content = zip_file.read(vml_file).decode('utf-8', errors='ignore')
            
            # Buscar listas desplegables (combobox, lista, etc)
            # Las validaciones en VML están entre etiquetas <v:comment> o <o:data>
            
            if 'combolist' in vml_content.lower() or 'combo' in vml_content.lower():
                print('Encontrado combolist')
                # Buscar patrones
                import re
                combos = re.findall(r'<o:combolist>(.*?)</o:combolist>', vml_content, re.DOTALL | re.IGNORECASE)
                for combo in combos:
                    print(f'  {combo}')
            
            if 'data type' in vml_content.lower() or 'validation' in vml_content.lower():
                print('Encontrada mención a validación')
                # Extraer contexto
                import re
                matches = re.finditer(r'(<.*?(?:validation|combolist|list|data type).*?>.*?<.*?>)', vml_content, re.IGNORECASE)
                for match in list(matches)[:5]:
                    print(f'  {match.group(0)[:200]}')
            
            # Mostrar primeras 2000 caracteres para inspeccionar
            if vml_content.strip():
                print('\nPrimeros 1500 caracteres:')
                print(vml_content[:1500])
        
        except Exception as e:
            print(f'Error: {e}')
        
        print()

print("=" * 70)
