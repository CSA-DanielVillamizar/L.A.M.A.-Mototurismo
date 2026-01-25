# Scripts de Verificación - COR L.A.MA

## `verify-seed.ps1` - Verificación de Encoding UTF-8 en SQL Seeds

### Propósito
Este script asegura que los archivos SQL de seed data mantengan encoding UTF-8 correcto y que todos los literales con caracteres Unicode (acentos, eñes, etc.) usen el prefijo `N'...'` requerido por SQL Server.

### Uso Local

```powershell
# Desde la raíz del repositorio
.\scripts\verify-seed.ps1
```

### Salida Exitosa
```
[SUCCESS] Verificacion exitosa
  - Encoding UTF-8: OK
  - Mojibake: No detectado
  - Literales Unicode: N' correcto
```

### Checks Realizados

1. **Encoding UTF-8**: Verifica que `sql/seed-data.sql` esté en UTF-8 (con o sin BOM)
2. **Mojibake Detection**: Busca patrones de corrupción de encoding (Ã©, Ã±, etc.)
3. **Unicode Literals**: Asegura que strings con caracteres especiales usen `N'...'` en lugar de `'...'`

### Integración CI/CD

El script está integrado en el pipeline de GitHub Actions (`.github/workflows/deploy-app.yml`):

```yaml
verify-encoding:
  name: Verify SQL Encoding
  runs-on: ubuntu-latest
  steps:
    - name: Checkout code
      uses: actions/checkout@v4
    - name: Verify seed-data.sql encoding
      shell: pwsh
      run: ./scripts/verify-seed.ps1
```

El job `verify-encoding` se ejecuta ANTES de `build-api` y `build-frontend`, bloqueando el pipeline si se detectan problemas.

### Exit Codes
- `0`: Verificación exitosa
- `1`: Fallo detectado (mojibake, encoding incorrecto, o literales sin N')

### Reglas de EditorConfig

El archivo `.editorconfig` en la raíz del repositorio asegura que todos los editores mantengan encoding consistente:

```ini
[*.sql]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
```

### Prevención de Mojibake

**Problema**: Al editar archivos SQL en Windows con editores que no respetan UTF-8, los caracteres Unicode (á, é, í, ó, ú, ñ) se corrompen, causando errores al ejecutar los scripts en SQL Server.

**Solución**:
1. `.editorconfig` fuerza UTF-8 en todos los editores compatibles (VS Code, Visual Studio, etc.)
2. `verify-seed.ps1` detecta corrupción automáticamente en local y en CI
3. Literales SQL con `N'...'` aseguran interpretación Unicode correcta

### Referencias
- [EditorConfig](https://editorconfig.org/)
- [SQL Server Unicode Strings](https://learn.microsoft.com/en-us/sql/t-sql/data-types/nchar-and-nvarchar-transact-sql)
- [GitHub Actions PowerShell](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions#jobsjob_idstepsshell)
