import win32com.client
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx').absolute()

print("=" * 70)
print("EXTRAYENDO LISTA DESPLEGABLE CON COM EXCEL")
print("=" * 70)

try:
    # Crear instancia de Excel
    excel = win32com.client.Dispatch("Excel.Application")
    excel.Visible = False
    
    # Abrir archivo
    print(f'\nAbriendo: {file_path}')
    wb = excel.Workbooks.Open(str(file_path))
    
    # Primera hoja
    ws = wb.Sheets(1)
    
    # Obtener validaciones de columna G (STATUS)
    print('\nBuscando validación en columna G (STATUS)...\n')
    
    # Verificar cada celda G10:G300
    target_range = ws.Range("G10:G300")
    dv = target_range.Validation
    
    if dv:
        print(f'Validación encontrada en G10:G300:')
        print(f'  Tipo: {dv.Type}')  # xlValidateList, xlValidateWholeNumber, etc
        
        # Type 3 = xlValidateList
        if dv.Type == 3:
            print(f'  Fórmula1: {dv.Formula1}')
            
            # Si es una lista separada por comas
            if dv.Formula1:
                values = dv.Formula1.split(',')
                print(f'\n  LISTA DESPLEGABLE ({len(values)} valores):')
                for i, val in enumerate(values, 1):
                    clean_val = val.strip().strip('"').strip("'")
                    print(f'    {i:2d}. {clean_val}')
        else:
            print(f'  [Tipo de validación: {dv.Type}]')
    else:
        print('No se encontró validación en G10:G300')
    
    # Verificar también otras columnas por si hay más validaciones
    print('\n' + "=" * 70)
    print('BUSCANDO TODAS LAS VALIDACIONES EN LA HOJA')
    print("=" * 70)
    
    for row in range(10, 50):  # Primeras 40 filas
        for col in range(1, 10):  # Primeras 9 columnas
            cell = ws.Cells(row, col)
            dv = cell.Validation
            
            if dv and dv.Type == 3:  # List type
                col_letter = chr(64 + col)
                print(f'\nValidación lista en {col_letter}{row}:')
                print(f'  Fórmula: {dv.Formula1[:100]}...')
    
    wb.Close(False)
    excel.Quit()
    print('\n✓ Completado')
    
except Exception as e:
    print(f'\n✗ Error: {e}')
    import traceback
    traceback.print_exc()

print("\n" + "=" * 70)
