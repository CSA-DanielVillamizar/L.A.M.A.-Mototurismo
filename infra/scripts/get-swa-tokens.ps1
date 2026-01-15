# =============================================================================
# Script: get-swa-tokens.ps1
# Descripción: Obtiene API tokens de Static Web Apps para GitHub Actions
# Uso: .\get-swa-tokens.ps1
# =============================================================================

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

Write-Host "🔑 Obteniendo Static Web App API Tokens" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

# Validar Azure CLI
if (!(Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azure CLI no encontrada. Instala desde: https://aka.ms/InstallAzureCLI" -ForegroundColor Red
    exit 1
}

# Verificar login
$account = az account show 2>&1 | ConvertFrom-Json
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ No estás autenticado en Azure. Ejecuta: az login" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Autenticado como: $($account.user.name)" -ForegroundColor Green
Write-Host ""

$environments = @("dev", "test", "prod")
$tokens = @{}

foreach ($env in $environments) {
    $resourceGroup = "rg-lama-$env"
    $swaName = "stapp-lama-$env"
    
    Write-Host "🔍 Obteniendo token para $env..." -ForegroundColor Yellow
    
    # Verificar si existe el Static Web App
    $swaExists = az staticwebapp show `
        --name $swaName `
        --resource-group $resourceGroup `
        --query "name" -o tsv 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ⚠️  Static Web App no encontrado (infraestructura no desplegada)" -ForegroundColor Yellow
        continue
    }
    
    # Obtener API token
    $token = az staticwebapp secrets list `
        --name $swaName `
        --resource-group $resourceGroup `
        --query "properties.apiKey" -o tsv 2>&1
    
    if ($LASTEXITCODE -eq 0 -and $token) {
        $tokens[$env] = $token
        Write-Host "  ✅ Token obtenido" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Error obteniendo token" -ForegroundColor Red
    }
}

if ($tokens.Count -eq 0) {
    Write-Host ""
    Write-Host "❌ No se pudo obtener ningún token" -ForegroundColor Red
    Write-Host "   Asegúrate de que la infraestructura esté desplegada" -ForegroundColor Yellow
    exit 1
}

# Mostrar resultados
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "✅ Tokens obtenidos exitosamente" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Copia estos secretos a GitHub Environments:" -ForegroundColor Yellow
Write-Host ""

foreach ($env in $tokens.Keys) {
    $secretName = "AZURE_STATIC_WEB_APPS_API_TOKEN_$($env.ToUpper())"
    Write-Host "Environment: $env" -ForegroundColor Cyan
    Write-Host "Secret Name: $secretName" -ForegroundColor White
    Write-Host "Secret Value:" -ForegroundColor White
    Write-Host $tokens[$env] -ForegroundColor Gray
    Write-Host ""
}

# Guardar en archivo
$outputFile = ".swa-tokens.txt"
$content = @"
# Static Web App API Tokens
# Generado: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
# ⚠️ NO SUBIR ESTE ARCHIVO A GIT

"@

foreach ($env in $tokens.Keys) {
    $secretName = "AZURE_STATIC_WEB_APPS_API_TOKEN_$($env.ToUpper())"
    $content += @"

# Environment: $env
$secretName=$($tokens[$env])
"@
}

$content | Set-Content -Path $outputFile -Encoding UTF8

Write-Host "✅ Tokens guardados en $outputFile (NO subir a Git)" -ForegroundColor Green
Write-Host ""
Write-Host "🔗 Configuración en GitHub:" -ForegroundColor Yellow
Write-Host "  1. Ve a: Settings → Environments" -ForegroundColor White
Write-Host "  2. Para cada environment (dev/test/prod):" -ForegroundColor White
Write-Host "     - Crea un secret con el nombre indicado arriba" -ForegroundColor White
Write-Host "     - Pega el token correspondiente" -ForegroundColor White
Write-Host ""
Write-Host "📖 Documentación completa en: infra/SETUP-GUIDE.md" -ForegroundColor Cyan
