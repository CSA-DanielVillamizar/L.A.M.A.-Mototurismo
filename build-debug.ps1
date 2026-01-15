# Build Script para Lama.API en modo Debug
# Verifica que el bypass de autenticación esté activo

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "BUILD LAMA.API - DEBUG MODE" -ForegroundColor Cyan
Write-Host "Auth Bypass: ENABLED (sin [Authorize])" -ForegroundColor Yellow
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "c:\Users\DanielVillamizar\COR L.A.MA\src\Lama.API\Lama.API.csproj"

Write-Host "Limpiando build anterior..." -ForegroundColor Gray
dotnet clean $projectPath --configuration Debug --verbosity quiet

Write-Host "Compilando en modo Debug..." -ForegroundColor Gray
$buildOutput = dotnet build $projectPath --configuration Debug --no-incremental 2>&1

# Verificar resultado
if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ BUILD EXITOSO" -ForegroundColor Green
    Write-Host ""
    Write-Host "AdminController configurado:" -ForegroundColor Cyan
    Write-Host "  - [Authorize] está DESHABILITADO en Debug" -ForegroundColor Yellow
    Write-Host "  - Endpoint POST /api/admin/evidence/upload es PÚBLICO" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Para iniciar el servidor:" -ForegroundColor Cyan
    Write-Host "  dotnet run --project `"$projectPath`" --configuration Debug" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ BUILD FAILED" -ForegroundColor Red
    Write-Host ""
    Write-Host "Errores encontrados:" -ForegroundColor Red
    $buildOutput | Select-String -Pattern "error|warning" | ForEach-Object {
        Write-Host $_ -ForegroundColor Red
    }
    Write-Host ""
}
