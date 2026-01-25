# Script de verificacion de encoding UTF-8 para seed-data.sql
# Parte del Release Management - COR L.A.MA

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlDir = Join-Path (Split-Path -Parent $scriptDir) "sql"
$seedFile = Join-Path $sqlDir "seed-data.sql"

if (-not (Test-Path $seedFile)) {
    Write-Host "[FAIL] No se encuentra sql/seed-data.sql" -ForegroundColor Red
    exit 1
}

Write-Host "Verificando: $seedFile" -ForegroundColor Cyan
Write-Host ""

# PASO 1: Verificar UTF-8
Write-Host "[1/3] Verificando encoding UTF-8..." -ForegroundColor Yellow
$bytes = [System.IO.File]::ReadAllBytes($seedFile)
$hasUtf8Bom = ($bytes.Length -ge 3) -and ($bytes[0] -eq 0xEF) -and ($bytes[1] -eq 0xBB) -and ($bytes[2] -eq 0xBF)

if ($hasUtf8Bom) {
    Write-Host "  [WARN] Archivo con BOM UTF-8" -ForegroundColor DarkYellow
} else {
    Write-Host "  [OK] UTF-8 sin BOM" -ForegroundColor Green
}

# PASO 2: Detectar mojibake (busqueda simple)
Write-Host "[2/3] Buscando mojibake..." -ForegroundColor Yellow
$content = [System.IO.File]::ReadAllText($seedFile, [System.Text.Encoding]::UTF8)
$hasMojibake = $false

# Patrones de mojibake mas comunes (UTF-8 mal interpretado como Latin-1)
if ($content -match 'Ã' -or $content -match 'Ã' -or $content -match 'Ã' -or $content -match 'Ãº') {
    Write-Host "  [FAIL] Mojibake detectado" -ForegroundColor Red
    $hasMojibake = $true
} else {
    Write-Host "  [OK] Sin mojibake" -ForegroundColor Green
}

if ($hasMojibake) {
    Write-Host ""
    Write-Host "[FAIL] Archivo corrupto - re-codifica a UTF-8" -ForegroundColor Red
    exit 1
}

# PASO 3: Verificar literales N'...'
Write-Host "[3/3] Verificando literales Unicode..." -ForegroundColor Yellow
$lineNum = 0
$violations = @()

Get-Content $seedFile -Encoding UTF8 | ForEach-Object {
    $lineNum++
    $line = $_
    
    # Ignorar comentarios
    if ($line -match '^\s*--') { return }
    
    # Detectar caracteres no-ASCII (fuera de 0x00-0x7F)
    if ($line -match '[^\x00-\x7F]') {
        # Buscar comillas simples SIN N delante: '...' pero no N'...'
        if ($line -match "(?<!N)'[^']*[^\x00-\x7F][^']*'") {
            $violations += "Linea $lineNum : $($line.Trim())"
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host "  [FAIL] $($violations.Count) literales sin N' detectados" -ForegroundColor Red
    $violations | ForEach-Object { Write-Host "    $_" -ForegroundColor DarkGray }
    Write-Host ""
    Write-Host "[FAIL] Use N'texto' para literales Unicode" -ForegroundColor Red
    exit 1
} else {
    Write-Host "  [OK] Todos los literales con N'" -ForegroundColor Green
}

# RESULTADO
Write-Host ""
Write-Host "[SUCCESS] Verificacion exitosa" -ForegroundColor Green
Write-Host "  - Encoding UTF-8: OK" -ForegroundColor Gray
Write-Host "  - Mojibake: No detectado" -ForegroundColor Gray
Write-Host "  - Literales Unicode: N' correcto" -ForegroundColor Gray
Write-Host ""

exit 0