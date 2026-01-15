import openpyxl
from pathlib import Path

file_path = Path('INSUMOS') / '(COL) PEREIRA CORTE NACIONAL.xlsx'

wb = openpyxl.load_workbook(file_path)
ws = wb['ODOMETER']

print('Primeras 15 filas (valores crudos):')
for row_idx in range(1, 16):
    row_data = []
    for col_idx in range(1, 10):
        cell = ws.cell(row_idx, col_idx)
        row_data.append(str(cell.value)[:20] if cell.value else 'None')
    print(f'Fila {row_idx}: {row_data}')
