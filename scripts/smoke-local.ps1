# ============================================================================
# Script de pruebas de humo para COR L.A.MA en modo DEV local
# ============================================================================
# Ejecutar: .\scripts\smoke-local.ps1
# Prerequisitos: Backend en http://localhost:5000, Frontend en http://localhost:3002
# ============================================================================

param(
    [string]$BackendUrl = "http://localhost:5000",
    [string]$FrontendUrl = "http://localhost:3002"
)

$ErrorActionPreference = "Continue"
$passed = 0
$failed = 0
$results = @()

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Url,
        [int]$ExpectedStatus = 200,
        [string]$ExpectedContent = $null,
        [int]$MinContentLength = 0
    )
    
    Write-Host "`n[TEST] $Name" -ForegroundColor Cyan
    Write-Host "   URL: $Url"
    
    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
        
        $statusMatch = $response.StatusCode -eq $ExpectedStatus
        $contentMatch = $true
        $lengthMatch = $true
        
        if ($ExpectedContent) {
            $contentMatch = $response.Content -like "*$ExpectedContent*"
        }
        
        if ($MinContentLength -gt 0) {
            $lengthMatch = $response.Content.Length -ge $MinContentLength
        }
        
        if ($statusMatch -and $contentMatch -and $lengthMatch) {
            Write-Host "   ✅ PASS (Status: $($response.StatusCode), Length: $($response.Content.Length) bytes)" -ForegroundColor Green
            $script:passed++
            $script:results += [PSCustomObject]@{
                Test = $Name
                Status = "PASS"
                Details = "Status: $($response.StatusCode), Length: $($response.Content.Length)"
            }
        } else {
            Write-Host "   ❌ FAIL (Status: $($response.StatusCode), Expected: $ExpectedStatus)" -ForegroundColor Red
            if (-not $contentMatch) { Write-Host "      Content mismatch: Expected '$ExpectedContent' not found" -ForegroundColor Yellow }
            if (-not $lengthMatch) { Write-Host "      Length: $($response.Content.Length) less than $MinContentLength" -ForegroundColor Yellow }
            $script:failed++
            $script:results += [PSCustomObject]@{
                Test = $Name
                Status = "FAIL"
                Details = "Status: $($response.StatusCode), Expected: $ExpectedStatus"
            }
        }
    } catch {
        Write-Host "   ❌ FAIL (Exception: $($_.Exception.Message))" -ForegroundColor Red
        $script:failed++
        $script:results += [PSCustomObject]@{
            Test = $Name
            Status = "FAIL"
            Details = $_.Exception.Message
        }
    }
}

function Test-JsonArrayCount {
    param(
        [string]$Name,
        [string]$Url,
        [int]$MinCount = 1
    )
    
    Write-Host "`n[TEST] $Name" -ForegroundColor Cyan
    Write-Host "   URL: $Url"
    
    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
        
        if ($response.StatusCode -ne 200) {
            Write-Host "   ❌ FAIL (Status: $($response.StatusCode))" -ForegroundColor Red
            $script:failed++
            $script:results += [PSCustomObject]@{ Test = $Name; Status = "FAIL"; Details = "HTTP $($response.StatusCode)" }
            return
        }
        
        $data = $response.Content | ConvertFrom-Json
        $count = 0
        
        if ($data -is [Array]) {
            $count = $data.Count
        } elseif ($data) {
            $count = 1
        }
        
        if ($count -ge $MinCount) {
            Write-Host "   ✅ PASS (Status: 200, Count: $count, MinCount: $MinCount)" -ForegroundColor Green
            $script:passed++
            $script:results += [PSCustomObject]@{ Test = $Name; Status = "PASS"; Details = "Count: $count" }
        } else {
            Write-Host "   ❌ FAIL (Count: $count less than $MinCount)" -ForegroundColor Red
            $script:failed++
            $script:results += [PSCustomObject]@{ Test = $Name; Status = "FAIL"; Details = "Count: $count less than $MinCount" }
        }
    } catch {
        Write-Host "   ❌ FAIL (Exception: $($_.Exception.Message))" -ForegroundColor Red
        $script:failed++
        $script:results += [PSCustomObject]@{ Test = $Name; Status = "FAIL"; Details = $_.Exception.Message }
    }
}

# ============================================================================
# INICIO DE PRUEBAS
# ============================================================================
Write-Host "`n" -NoNewline
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Magenta
Write-Host "  COR L.A.MA - Pruebas de Humo Locales (DEV Mode)" -ForegroundColor Magenta
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Magenta
Write-Host "  Backend:  $BackendUrl" -ForegroundColor White
Write-Host "  Frontend: $FrontendUrl" -ForegroundColor White
Write-Host "  Fecha:    $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "═══════════════════════════════════════════════════════════════`n" -ForegroundColor Magenta

# ----------------------------------------------------------------------------
# BACKEND - Health & Swagger
# ----------------------------------------------------------------------------
Write-Host "`n[BACKEND] Endpoints de Sistema" -ForegroundColor Yellow
Test-Endpoint -Name "Health Check" -Url "$BackendUrl/health" -ExpectedStatus 200 -ExpectedContent "Healthy"
Test-Endpoint -Name "Swagger JSON" -Url "$BackendUrl/swagger/v1/swagger.json" -ExpectedStatus 200 -MinContentLength 5000
Test-Endpoint -Name "Swagger UI" -Url "$BackendUrl/swagger/index.html" -ExpectedStatus 200 -MinContentLength 1000

# ----------------------------------------------------------------------------
# BACKEND - API Endpoints con Datos
# ----------------------------------------------------------------------------
Write-Host "`n[BACKEND] API con Datos (TenantId Default)" -ForegroundColor Yellow
Test-JsonArrayCount -Name "GET /api/v1/events (>=1 evento)" -Url "$BackendUrl/api/v1/events" -MinCount 1
Test-JsonArrayCount -Name "GET /api/v1/members/search?q=ma (>=1 miembro)" -Url "$BackendUrl/api/v1/members/search?q=ma" -MinCount 1
Test-Endpoint -Name "GET /api/v1/chapters (>=1 chapter)" -Url "$BackendUrl/api/v1/chapters" -ExpectedStatus 200 -MinContentLength 50

# ----------------------------------------------------------------------------
# BACKEND - CORS Preflight
# ----------------------------------------------------------------------------
Write-Host "`n[BACKEND] CORS Configuration" -ForegroundColor Yellow
try {
    $corsResponse = Invoke-WebRequest -Uri "$BackendUrl/api/v1/events" `
        -Method OPTIONS `
        -Headers @{
            "Origin" = $FrontendUrl
            "Access-Control-Request-Method" = "GET"
        } `
        -UseBasicParsing `
        -TimeoutSec 10 `
        -ErrorAction Stop
    
    $allowOrigin = $corsResponse.Headers["Access-Control-Allow-Origin"]
    $allowCreds = $corsResponse.Headers["Access-Control-Allow-Credentials"]
    
    if ($corsResponse.StatusCode -eq 204 -and $allowOrigin -eq $FrontendUrl -and $allowCreds -eq "true") {
        Write-Host "`n[TEST] CORS Preflight" -ForegroundColor Cyan
        Write-Host "   ✅ PASS (Status: 204, Allow-Origin: $allowOrigin, Allow-Credentials: $allowCreds)" -ForegroundColor Green
        $passed++
        $results += [PSCustomObject]@{ Test = "CORS Preflight"; Status = "PASS"; Details = "Headers OK" }
    } else {
        Write-Host "`n[TEST] CORS Preflight" -ForegroundColor Cyan
        Write-Host "   ❌ FAIL (Status: $($corsResponse.StatusCode), Headers incorrect)" -ForegroundColor Red
        $failed++
        $results += [PSCustomObject]@{ Test = "CORS Preflight"; Status = "FAIL"; Details = "Headers incorrect" }
    }
} catch {
    Write-Host "`n[TEST] CORS Preflight" -ForegroundColor Cyan
    Write-Host "   ❌ FAIL (Exception: $($_.Exception.Message))" -ForegroundColor Red
    $failed++
    $results += [PSCustomObject]@{ Test = "CORS Preflight"; Status = "FAIL"; Details = $_.Exception.Message }
}

# ----------------------------------------------------------------------------
# FRONTEND - Páginas Clave
# ----------------------------------------------------------------------------
Write-Host "`n[FRONTEND] Paginas" -ForegroundColor Yellow
Test-Endpoint -Name "Home /" -Url $FrontendUrl -ExpectedStatus 200 -MinContentLength 1000
Test-Endpoint -Name "Login /login" -Url "$FrontendUrl/login" -ExpectedStatus 200 -MinContentLength 500
Test-Endpoint -Name "Evidence Upload /evidence/upload" -Url "$FrontendUrl/evidence/upload" -ExpectedStatus 200 -MinContentLength 1000
Test-Endpoint -Name "Admin COR /admin/cor" -Url "$FrontendUrl/admin/cor" -ExpectedStatus 200 -MinContentLength 500

# ============================================================================
# RESUMEN FINAL
# ============================================================================
Write-Host "`n" -NoNewline
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Magenta
Write-Host "  RESUMEN DE PRUEBAS" -ForegroundColor Magenta
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor Magenta

$results | Format-Table -AutoSize

Write-Host "`nTotal: $($passed + $failed) pruebas" -ForegroundColor White
Write-Host "   ✅ Pasadas: $passed" -ForegroundColor Green
Write-Host "   ❌ Fallidas: $failed" -ForegroundColor Red

if ($failed -eq 0) {
    Write-Host "`nTODAS LAS PRUEBAS PASARON!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`nHAY PRUEBAS FALLIDAS. Revisar logs arriba." -ForegroundColor Yellow
    exit 1
}
