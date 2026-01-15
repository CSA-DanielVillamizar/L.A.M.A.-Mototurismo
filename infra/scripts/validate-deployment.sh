#!/bin/bash
# =============================================================================
# Script: validate-deployment.sh
# Descripción: Valida que todos los recursos estén desplegados correctamente
# Uso: ./validate-deployment.sh dev
# =============================================================================

set -e

ENVIRONMENT=$1

if [[ -z "$ENVIRONMENT" ]]; then
    echo "❌ Uso: ./validate-deployment.sh <dev|test|prod>"
    exit 1
fi

if [[ ! "$ENVIRONMENT" =~ ^(dev|test|prod)$ ]]; then
    echo "❌ Environment inválido. Usa: dev, test o prod"
    exit 1
fi

# Colores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo ""
echo -e "${BLUE}🔍 Validando deployment en $ENVIRONMENT${NC}"
echo "=========================================="

# Resource Group
RG="rg-lama-$ENVIRONMENT"

# Verificar Azure CLI login
if ! az account show &> /dev/null; then
    echo -e "${RED}❌ No estás autenticado en Azure. Ejecuta: az login${NC}"
    exit 1
fi

# Función para verificar recurso
check_resource() {
    local resource_type=$1
    local resource_name=$2
    local display_name=$3
    
    echo -n "Verificando $display_name... "
    
    if az resource show --resource-group "$RG" --name "$resource_name" --resource-type "$resource_type" &> /dev/null; then
        echo -e "${GREEN}✅${NC}"
        return 0
    else
        echo -e "${RED}❌${NC}"
        return 1
    fi
}

# Función para verificar endpoint HTTP
check_endpoint() {
    local url=$1
    local display_name=$2
    
    echo -n "Verificando $display_name... "
    
    http_code=$(curl -s -o /dev/null -w "%{http_code}" "$url" || echo "000")
    
    if [[ "$http_code" == "200" || "$http_code" == "401" || "$http_code" == "404" ]]; then
        echo -e "${GREEN}✅ ($http_code)${NC}"
        return 0
    else
        echo -e "${RED}❌ ($http_code)${NC}"
        return 1
    fi
}

echo ""
echo -e "${YELLOW}📦 Verificando recursos Azure...${NC}"

# Contador de errores
ERRORS=0

# 1. Resource Group
echo -n "Resource Group... "
if az group show --name "$RG" &> /dev/null; then
    echo -e "${GREEN}✅${NC}"
else
    echo -e "${RED}❌${NC}"
    ((ERRORS++))
fi

# 2. SQL Server
SQL_SERVER="sql-lama-$ENVIRONMENT"
check_resource "Microsoft.Sql/servers" "$SQL_SERVER" "SQL Server" || ((ERRORS++))

# 3. SQL Database
SQL_DB="sqldb-lama-$ENVIRONMENT"
if az sql db show --resource-group "$RG" --server "$SQL_SERVER" --name "$SQL_DB" &> /dev/null; then
    echo -e "SQL Database... ${GREEN}✅${NC}"
else
    echo -e "SQL Database... ${RED}❌${NC}"
    ((ERRORS++))
fi

# 4. Storage Account
STORAGE_PATTERN="stlama${ENVIRONMENT}*"
echo -n "Storage Account... "
STORAGE_ACCOUNT=$(az storage account list --resource-group "$RG" --query "[?starts_with(name, 'stlama$ENVIRONMENT')].name" -o tsv | head -n 1)
if [[ -n "$STORAGE_ACCOUNT" ]]; then
    echo -e "${GREEN}✅ ($STORAGE_ACCOUNT)${NC}"
    
    # Verificar container 'evidences'
    echo -n "  Container 'evidences'... "
    if az storage container show --account-name "$STORAGE_ACCOUNT" --name "evidences" &> /dev/null; then
        echo -e "${GREEN}✅${NC}"
    else
        echo -e "${RED}❌${NC}"
        ((ERRORS++))
    fi
else
    echo -e "${RED}❌${NC}"
    ((ERRORS++))
fi

# 5. App Service Plan
ASP="asp-lama-$ENVIRONMENT"
check_resource "Microsoft.Web/serverfarms" "$ASP" "App Service Plan" || ((ERRORS++))

# 6. App Service
APP_SERVICE="app-lama-$ENVIRONMENT"
check_resource "Microsoft.Web/sites" "$APP_SERVICE" "App Service" || ((ERRORS++))

# 7. Redis Cache
REDIS="redis-lama-$ENVIRONMENT"
check_resource "Microsoft.Cache/redis" "$REDIS" "Redis Cache" || ((ERRORS++))

# 8. Key Vault
echo -n "Key Vault... "
KV_NAME=$(az keyvault list --resource-group "$RG" --query "[?starts_with(name, 'kv-lama-$ENVIRONMENT')].name" -o tsv | head -n 1)
if [[ -n "$KV_NAME" ]]; then
    echo -e "${GREEN}✅ ($KV_NAME)${NC}"
    
    # Verificar secretos
    echo -n "  Secretos... "
    SECRET_COUNT=$(az keyvault secret list --vault-name "$KV_NAME" --query "length([])" -o tsv)
    if [[ "$SECRET_COUNT" -ge 3 ]]; then
        echo -e "${GREEN}✅ ($SECRET_COUNT secretos)${NC}"
    else
        echo -e "${YELLOW}⚠️  ($SECRET_COUNT secretos - esperado: 3+)${NC}"
    fi
else
    echo -e "${RED}❌${NC}"
    ((ERRORS++))
fi

# 9. Log Analytics Workspace
LAW="law-lama-$ENVIRONMENT"
check_resource "Microsoft.OperationalInsights/workspaces" "$LAW" "Log Analytics" || ((ERRORS++))

# 10. Application Insights
APPI="appi-lama-$ENVIRONMENT"
check_resource "Microsoft.Insights/components" "$APPI" "Application Insights" || ((ERRORS++))

# 11. Static Web App
SWA="stapp-lama-$ENVIRONMENT"
check_resource "Microsoft.Web/staticSites" "$SWA" "Static Web App" || ((ERRORS++))

echo ""
echo -e "${YELLOW}🌐 Verificando endpoints...${NC}"

# Endpoint API
API_URL="https://app-lama-$ENVIRONMENT.azurewebsites.net"
check_endpoint "$API_URL/health" "API Health Check" || ((ERRORS++))
check_endpoint "$API_URL/swagger" "Swagger UI" || ((ERRORS++))

# Endpoint Frontend
SWA_HOSTNAME=$(az staticwebapp show --name "$SWA" --resource-group "$RG" --query "defaultHostname" -o tsv 2>/dev/null || echo "")
if [[ -n "$SWA_HOSTNAME" ]]; then
    check_endpoint "https://$SWA_HOSTNAME" "Frontend" || ((ERRORS++))
else
    echo -e "Frontend... ${RED}❌ (hostname no encontrado)${NC}"
    ((ERRORS++))
fi

echo ""
echo -e "${YELLOW}🔐 Verificando seguridad...${NC}"

# Managed Identity en App Service
echo -n "Managed Identity... "
MI_ENABLED=$(az webapp identity show --name "$APP_SERVICE" --resource-group "$RG" --query "type" -o tsv 2>/dev/null || echo "")
if [[ "$MI_ENABLED" == "SystemAssigned" ]]; then
    echo -e "${GREEN}✅${NC}"
else
    echo -e "${RED}❌${NC}"
    ((ERRORS++))
fi

# SSL/TLS en Redis
echo -n "Redis SSL... "
REDIS_SSL=$(az redis show --name "$REDIS" --resource-group "$RG" --query "enableNonSslPort" -o tsv 2>/dev/null || echo "true")
if [[ "$REDIS_SSL" == "false" ]]; then
    echo -e "${GREEN}✅ (Non-SSL deshabilitado)${NC}"
else
    echo -e "${YELLOW}⚠️  (Non-SSL habilitado)${NC}"
fi

# SQL Firewall Rules
echo -n "SQL Firewall... "
FW_RULES=$(az sql server firewall-rule list --resource-group "$RG" --server "$SQL_SERVER" --query "length([])" -o tsv 2>/dev/null || echo "0")
if [[ "$FW_RULES" -gt 0 ]]; then
    echo -e "${GREEN}✅ ($FW_RULES reglas)${NC}"
else
    echo -e "${YELLOW}⚠️  (Sin reglas)${NC}"
fi

echo ""
echo -e "${YELLOW}📊 Verificando configuración...${NC}"

# App Service Settings (KeyVault references)
echo -n "KeyVault References... "
APP_SETTINGS=$(az webapp config appsettings list --name "$APP_SERVICE" --resource-group "$RG" 2>/dev/null || echo "[]")
KV_REFS=$(echo "$APP_SETTINGS" | grep -c "@Microsoft.KeyVault" || echo "0")
if [[ "$KV_REFS" -ge 3 ]]; then
    echo -e "${GREEN}✅ ($KV_REFS referencias)${NC}"
else
    echo -e "${YELLOW}⚠️  ($KV_REFS referencias - esperado: 3+)${NC}"
fi

# CORS en App Service
echo -n "CORS configurado... "
CORS=$(az webapp cors show --name "$APP_SERVICE" --resource-group "$RG" --query "allowedOrigins" -o tsv 2>/dev/null || echo "")
if [[ -n "$CORS" ]]; then
    echo -e "${GREEN}✅${NC}"
else
    echo -e "${YELLOW}⚠️  (Sin CORS)${NC}"
fi

# Application Insights connection
echo -n "App Insights integrado... "
APPI_KEY=$(az webapp config appsettings list --name "$APP_SERVICE" --resource-group "$RG" --query "[?name=='APPLICATIONINSIGHTS_CONNECTION_STRING'].value" -o tsv 2>/dev/null || echo "")
if [[ -n "$APPI_KEY" ]]; then
    echo -e "${GREEN}✅${NC}"
else
    echo -e "${RED}❌${NC}"
    ((ERRORS++))
fi

# Resumen final
echo ""
echo "=========================================="
if [[ $ERRORS -eq 0 ]]; then
    echo -e "${GREEN}✅ Deployment válido - 0 errores${NC}"
    echo "=========================================="
    echo ""
    echo -e "${BLUE}🚀 URLs:${NC}"
    echo "  API: $API_URL"
    echo "  Swagger: $API_URL/swagger"
    echo "  Frontend: https://$SWA_HOSTNAME"
    echo "  Portal Azure: https://portal.azure.com/#@/resource/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RG"
    exit 0
else
    echo -e "${RED}❌ Deployment con errores: $ERRORS${NC}"
    echo "=========================================="
    echo ""
    echo -e "${YELLOW}📖 Revisa la documentación en infra/SETUP-GUIDE.md${NC}"
    exit 1
fi
