#!/bin/bash
# =============================================================================
# Script: setup-oidc.sh
# Descripción: Configura OIDC Federation para GitHub Actions con Azure AD
# Uso: ./setup-oidc.sh
# =============================================================================

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "🔐 Azure AD OIDC Setup para GitHub Actions"
echo "=========================================="

# Validar Azure CLI
if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI no encontrada. Instala desde: https://aka.ms/InstallAzureCLIDeb${NC}"
    exit 1
fi

# Input: Variables de entorno
echo ""
echo -e "${YELLOW}📝 Ingresa la información requerida:${NC}"
read -p "Azure Subscription ID: " AZURE_SUBSCRIPTION_ID
read -p "Azure Tenant ID: " AZURE_TENANT_ID
read -p "GitHub Repo Owner (usuario/org): " GITHUB_REPO_OWNER
read -p "GitHub Repo Name: " GITHUB_REPO_NAME

# Validar inputs
if [[ -z "$AZURE_SUBSCRIPTION_ID" || -z "$AZURE_TENANT_ID" || -z "$GITHUB_REPO_OWNER" || -z "$GITHUB_REPO_NAME" ]]; then
    echo -e "${RED}❌ Todos los campos son requeridos${NC}"
    exit 1
fi

# Login a Azure
echo ""
echo -e "${YELLOW}🔑 Iniciando sesión en Azure...${NC}"
az login --tenant "$AZURE_TENANT_ID"
az account set --subscription "$AZURE_SUBSCRIPTION_ID"

# Crear App Registration
echo ""
echo -e "${YELLOW}🆕 Creando App Registration...${NC}"
APP_ID=$(az ad app create \
  --display-name "GitHub-LAMA-OIDC" \
  --query appId -o tsv)

echo -e "${GREEN}✅ App Registration creada${NC}"
echo "   App ID: $APP_ID"

# Crear Service Principal
echo ""
echo -e "${YELLOW}👤 Creando Service Principal...${NC}"
az ad sp create --id "$APP_ID" > /dev/null

SP_OBJECT_ID=$(az ad sp show --id "$APP_ID" --query id -o tsv)
echo -e "${GREEN}✅ Service Principal creado${NC}"
echo "   Object ID: $SP_OBJECT_ID"

# Crear Federated Credentials
echo ""
echo -e "${YELLOW}🔗 Creando Federated Credentials...${NC}"

# DEV (push a main)
az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters @- <<EOF
{
  "name": "github-lama-dev",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:${GITHUB_REPO_OWNER}/${GITHUB_REPO_NAME}:ref:refs/heads/main",
  "audiences": ["api://AzureADTokenExchange"],
  "description": "GitHub Actions OIDC for DEV environment"
}
EOF
echo -e "${GREEN}  ✅ DEV credential creada${NC}"

# TEST (environment:test)
az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters @- <<EOF
{
  "name": "github-lama-test",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:${GITHUB_REPO_OWNER}/${GITHUB_REPO_NAME}:environment:test",
  "audiences": ["api://AzureADTokenExchange"],
  "description": "GitHub Actions OIDC for TEST environment"
}
EOF
echo -e "${GREEN}  ✅ TEST credential creada${NC}"

# PROD (environment:prod)
az ad app federated-credential create \
  --id "$APP_ID" \
  --parameters @- <<EOF
{
  "name": "github-lama-prod",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:${GITHUB_REPO_OWNER}/${GITHUB_REPO_NAME}:environment:prod",
  "audiences": ["api://AzureADTokenExchange"],
  "description": "GitHub Actions OIDC for PROD environment"
}
EOF
echo -e "${GREEN}  ✅ PROD credential creada${NC}"

# Asignar Contributor Role
echo ""
echo -e "${YELLOW}🔐 Asignando permisos Contributor...${NC}"
az role assignment create \
  --assignee "$APP_ID" \
  --role "Contributor" \
  --scope "/subscriptions/$AZURE_SUBSCRIPTION_ID" \
  > /dev/null

echo -e "${GREEN}✅ Permisos asignados${NC}"

# Verificar role
ROLE_ASSIGNED=$(az role assignment list --assignee "$APP_ID" --query "[0].roleDefinitionName" -o tsv)
echo "   Role: $ROLE_ASSIGNED"

# Output: Secrets para GitHub
echo ""
echo -e "${GREEN}=========================================="
echo "✅ OIDC Setup completado exitosamente"
echo "==========================================${NC}"
echo ""
echo -e "${YELLOW}📋 Copia estos secretos a GitHub:${NC}"
echo ""
echo "Ve a: https://github.com/$GITHUB_REPO_OWNER/$GITHUB_REPO_NAME/settings/environments"
echo ""
echo "Crea 3 environments (dev, test, prod) y agrega estos secrets a cada uno:"
echo ""
echo -e "${GREEN}AZURE_CLIENT_ID${NC}=${APP_ID}"
echo -e "${GREEN}AZURE_TENANT_ID${NC}=${AZURE_TENANT_ID}"
echo -e "${GREEN}AZURE_SUBSCRIPTION_ID${NC}=${AZURE_SUBSCRIPTION_ID}"
echo ""
echo -e "${YELLOW}💡 Guarda estos valores en un lugar seguro${NC}"

# Guardar en archivo (para backup)
cat > .azure-secrets.txt <<EOF
AZURE_CLIENT_ID=$APP_ID
AZURE_TENANT_ID=$AZURE_TENANT_ID
AZURE_SUBSCRIPTION_ID=$AZURE_SUBSCRIPTION_ID
EOF

echo ""
echo -e "${GREEN}✅ Secrets guardados en .azure-secrets.txt (NO subir a Git)${NC}"
echo ""
echo -e "${YELLOW}📖 Próximos pasos:${NC}"
echo "1. Configura GitHub Environments (ver infra/SETUP-GUIDE.md)"
echo "2. Ejecuta el primer deployment: git push origin main"
echo "3. Verifica en GitHub Actions que el workflow se ejecute correctamente"
