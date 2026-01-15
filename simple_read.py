import pandas as pd
df = pd.read_excel(r"c:\Users\DanielVillamizar\COR L.A.MA\INSUMOS\(COL) PEREIRA CORTE NACIONAL.xlsx", sheet_name="ODOMETER", header=6)
cols = list(df.columns)
for i, c in enumerate(cols):
    print(f"{i+1}|{c}|{len(c)}|{ord(c[0]) if c else 0}")
