# =============================================================================
# Script: get-secrets.ps1
# Descripción: Obtiene secretos de Azure KeyVault para debugging local
# Uso: .\get-secrets.ps1 -Environment dev
# =============================================================================

[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('dev', 'test', 'prod')]
    [string]$Environment
)

$ErrorActionPreference = "Stop"

Write-Host "🔐 Obteniendo secretos de KeyVault ($Environment)" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# Validar Azure CLI
if (!(Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azure CLI no encontrada. Instala desde: https://aka.ms/InstallAzureCLI" -ForegroundColor Red
    exit 1
}

# Construir nombres de recursos
$resourceGroup = "rg-lama-$Environment"
$keyVaultPattern = "kv-lama-$Environment-*"

Write-Host ""
Write-Host "📦 Resource Group: $resourceGroup" -ForegroundColor Yellow

# Verificar login Azure
Write-Host ""
Write-Host "🔑 Verificando sesión Azure..." -ForegroundColor Yellow
$account = az account show 2>&1 | ConvertFrom-Json
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ No estás autenticado en Azure. Ejecuta: az login" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Autenticado como: $($account.user.name)" -ForegroundColor Green
Write-Host "   Subscription: $($account.name)" -ForegroundColor Gray

# Obtener nombre del KeyVault (nombre real con sufijo único)
Write-Host ""
Write-Host "🔍 Buscando KeyVault..." -ForegroundColor Yellow
$keyVaults = az keyvault list --resource-group $resourceGroup --query "[?starts_with(name, 'kv-lama-$Environment')].name" -o tsv 2>&1

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($keyVaults)) {
    Write-Host "❌ KeyVault no encontrado en $resourceGroup" -ForegroundColor Red
    Write-Host "   Asegúrate de que la infraestructura esté desplegada" -ForegroundColor Yellow
    exit 1
}

$keyVaultName = $keyVaults.Split([Environment]::NewLine)[0]
Write-Host "✅ KeyVault encontrado: $keyVaultName" -ForegroundColor Green

# Listar secretos
Write-Host ""
Write-Host "📋 Obteniendo secretos..." -ForegroundColor Yellow

$secrets = @{
    "SqlConnectionString" = ""
    "StorageConnectionString" = ""
    "RedisConnectionString" = ""
}

foreach ($secretName in $secrets.Keys) {
    try {
        $secretValue = az keyvault secret show `
            --vault-name $keyVaultName `
            --name $secretName `
            --query value -o tsv 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            $secrets[$secretName] = $secretValue
            Write-Host "  ✅ $secretName" -ForegroundColor Green
        } else {
            Write-Host "  ⚠️  $secretName (no encontrado)" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  ❌ Error obteniendo $secretName" -ForegroundColor Red
    }
}

# Generar archivo appsettings.Development.json
Write-Host ""
Write-Host "📝 Generando appsettings.$Environment.json..." -ForegroundColor Yellow

$appSettings = @{
    "ConnectionStrings" = @{
        "DefaultConnection" = $secrets["SqlConnectionString"]
        "RedisConnection" = $secrets["RedisConnectionString"]
    }
    "BlobStorage" = @{
        "ConnectionString" = $secrets["StorageConnectionString"]
        "ContainerName" = "evidences"
    }
    "Logging" = @{
        "LogLevel" = @{
            "Default" = "Information"
            "Microsoft.AspNetCore" = "Warning"
        }
    }
}

$outputPath = "..\..\src\Lama.API\appsettings.$Environment.json"
$appSettings | ConvertTo-Json -Depth 10 | Set-Content -Path $outputPath -Encoding UTF8

Write-Host "✅ Archivo generado: $outputPath" -ForegroundColor Green

# Generar .env para frontend
Write-Host ""
Write-Host "📝 Generando .env.local para frontend..." -ForegroundColor Yellow

$apiUrl = switch ($Environment) {
    "dev" { "https://app-lama-dev.azurewebsites.net" }
    "test" { "https://app-lama-test.azurewebsites.net" }
    "prod" { "https://app-lama-prod.azurewebsites.net" }
}

$envContent = @"
# Generado automáticamente - NO subir a Git
NEXT_PUBLIC_API_URL=$apiUrl
NEXT_PUBLIC_ENVIRONMENT=$Environment
"@

$envOutputPath = "..\..\frontend\.env.$Environment"
$envContent | Set-Content -Path $envOutputPath -Encoding UTF8

Write-Host "✅ Archivo generado: $envOutputPath" -ForegroundColor Green

# Mostrar resumen
Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "✅ Secretos obtenidos exitosamente" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Resumen:" -ForegroundColor Yellow
Write-Host "  - Environment: $Environment" -ForegroundColor White
Write-Host "  - KeyVault: $keyVaultName" -ForegroundColor White
Write-Host "  - Secretos obtenidos: $($secrets.Keys.Count)" -ForegroundColor White
Write-Host ""
Write-Host "🚀 Próximos pasos:" -ForegroundColor Yellow
Write-Host "  1. Copia appsettings.$Environment.json a src/Lama.API/" -ForegroundColor White
Write-Host "  2. Copia .env.$Environment a frontend/ como .env.local" -ForegroundColor White
Write-Host "  3. Ejecuta la API localmente: dotnet run --project src/Lama.API" -ForegroundColor White
Write-Host "  4. Ejecuta el frontend: cd frontend && npm run dev" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  IMPORTANTE: NO subir estos archivos a Git" -ForegroundColor Yellow
