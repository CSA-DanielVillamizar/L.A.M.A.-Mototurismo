import win32com.client
from pathlib import Path

file_path = Path('INSUMOS/(COL) PEREIRA CORTE NACIONAL.xlsx').absolute()

print("=" * 70)
print("LEYENDO LISTA DE VALORES DESDE LA HOJA 'RESUMEN'")
print("=" * 70)

try:
    # Crear instancia de Excel
    excel = win32com.client.Dispatch("Excel.Application")
    excel.Visible = False
    
    # Abrir archivo
    print(f'\nAbriendo: {file_path}')
    wb = excel.Workbooks.Open(str(file_path))
    
    # Buscar hoja "Resumen"
    print('\nBuscando hoja "Resumen"...')
    
    # Listar todas las hojas
    print(f'Hojas disponibles:')
    for i in range(1, wb.Sheets.Count + 1):
        sheet_name = wb.Sheets(i).Name
        print(f'  {i}. {sheet_name}')
    
    # Obtener la hoja Resumen
    try:
        ws_resumen = wb.Sheets("Resumen")
        print(f'\n✓ Hoja "Resumen" encontrada')
    except:
        ws_resumen = wb.Sheets(3)  # Tercera hoja (sheet3.xml)
        print(f'\n✓ Usando tercera hoja: {ws_resumen.Name}')
    
    # Leer rango E4:E36
    print('\nLeyendo rango E4:E36 (lista de valores de STATUS):')
    print("-" * 70)
    
    values = []
    for row in range(4, 37):
        cell_value = ws_resumen.Cells(row, 5).Value  # Columna E = 5
        
        if cell_value is not None:
            # Limpiar valor
            cell_str = str(cell_value).strip()
            if cell_str:
                values.append(cell_str)
                print(f'E{row}: {cell_str}')
    
    print(f'\n{"=" * 70}')
    print(f'TOTAL DE VALORES EN LISTA: {len(values)}')
    print("=" * 70)
    
    print('\nVALORES COMPLETOS:')
    for i, val in enumerate(values, 1):
        print(f'{i:2d}. {val}')
    
    wb.Close(False)
    excel.Quit()
    print('\n✓ Completado')
    
except Exception as e:
    print(f'\n✗ Error: {e}')
    import traceback
    traceback.print_exc()

print("\n" + "=" * 70)
