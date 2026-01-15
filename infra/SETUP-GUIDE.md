# 🚀 Guía de Configuración - COR L.A.MA Azure Infrastructure

Esta guía proporciona los pasos detallados para configurar el proyecto desde cero, incluyendo Azure OIDC, GitHub Environments y primeros despliegues.

---

## 📋 Tabla de Contenidos

1. [Prerrequisitos](#1-prerrequisitos)
2. [Configuración Azure AD (OIDC)](#2-configuración-azure-ad-oidc)
3. [Configuración GitHub Environments](#3-configuración-github-environments)
4. [Primer Despliegue Infraestructura](#4-primer-despliegue-infraestructura)
5. [Primer Despliegue Aplicación](#5-primer-despliegue-aplicación)
6. [Verificación Post-Deployment](#6-verificación-post-deployment)
7. [Troubleshooting](#7-troubleshooting)

---

## 1. Prerrequisitos

### Software Necesario
- **Azure CLI**: `az --version` >= 2.50
- **Bicep CLI**: `az bicep version` >= 0.24
- **Git**: `git --version` >= 2.40
- **.NET SDK**: `dotnet --version` >= 8.0
- **Node.js**: `node --version` >= 20.0

### Accesos Requeridos
- Cuenta Azure con permisos de **Owner** o **Contributor** en la subscription
- Repositorio GitHub con permisos de **Admin**
- Azure AD con permisos para crear App Registrations (si no tienes, pide a tu admin)

### Variables Base
Guarda estas variables en un archivo `.env.local` (no subir a Git):

```bash
AZURE_SUBSCRIPTION_ID="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
AZURE_TENANT_ID="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
GITHUB_REPO_OWNER="tu-usuario-github"
GITHUB_REPO_NAME="COR-LAMA"
```

---

## 2. Configuración Azure AD (OIDC)

### 2.1 Crear App Registration

```bash
# Login a Azure
az login --tenant $AZURE_TENANT_ID

# Crear App Registration
APP_ID=$(az ad app create \
  --display-name "GitHub-LAMA-OIDC" \
  --query appId -o tsv)

echo "App ID: $APP_ID"

# Crear Service Principal
az ad sp create --id $APP_ID

# Obtener Object ID del Service Principal
SP_OBJECT_ID=$(az ad sp show --id $APP_ID --query id -o tsv)
echo "Service Principal Object ID: $SP_OBJECT_ID"
```

### 2.2 Configurar Federated Credentials (3 ambientes)

```bash
# Credential para DEV (push a main)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters @- <<EOF
{
  "name": "github-lama-dev",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:${GITHUB_REPO_OWNER}/${GITHUB_REPO_NAME}:ref:refs/heads/main",
  "audiences": ["api://AzureADTokenExchange"],
  "description": "GitHub Actions OIDC for DEV environment"
}
EOF

# Credential para TEST (workflow_dispatch)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters @- <<EOF
{
  "name": "github-lama-test",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:${GITHUB_REPO_OWNER}/${GITHUB_REPO_NAME}:environment:test",
  "audiences": ["api://AzureADTokenExchange"],
  "description": "GitHub Actions OIDC for TEST environment"
}
EOF

# Credential para PROD (workflow_dispatch)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters @- <<EOF
{
  "name": "github-lama-prod",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:${GITHUB_REPO_OWNER}/${GITHUB_REPO_NAME}:environment:prod",
  "audiences": ["api://AzureADTokenExchange"],
  "description": "GitHub Actions OIDC for PROD environment"
}
EOF
```

### 2.3 Asignar Permisos Azure (Contributor Role)

```bash
# Asignar Contributor role a nivel de subscription
az role assignment create \
  --assignee $APP_ID \
  --role "Contributor" \
  --scope "/subscriptions/$AZURE_SUBSCRIPTION_ID"

# Verificar role assignment
az role assignment list --assignee $APP_ID --output table
```

### 2.4 Guardar IDs Generados

Copia estos valores para GitHub Secrets:
```bash
echo "AZURE_CLIENT_ID=$APP_ID"
echo "AZURE_TENANT_ID=$AZURE_TENANT_ID"
echo "AZURE_SUBSCRIPTION_ID=$AZURE_SUBSCRIPTION_ID"
```

---

## 3. Configuración GitHub Environments

### 3.1 Crear Environments via GitHub UI

1. Ve a tu repositorio en GitHub
2. Settings → Environments → New Environment
3. Crea 3 environments: `dev`, `test`, `prod`

### 3.2 Configurar Secrets por Environment

#### **Environment: dev**
```
Settings → Environments → dev → Add Secret
```

| Secret Name | Value | Descripción |
|-------------|-------|-------------|
| `AZURE_CLIENT_ID` | `$APP_ID` | App Registration ID |
| `AZURE_TENANT_ID` | `tu-tenant-id` | Azure AD Tenant ID |
| `AZURE_SUBSCRIPTION_ID` | `tu-subscription-id` | Azure Subscription ID |

#### **Environment: test**
```
Settings → Environments → test → Add Secret
```
Mismos secrets que `dev` (comparten el mismo Service Principal).

**Protection Rules:**
- ☑ Required reviewers: 1 persona mínimo
- ☑ Wait timer: 0 minutes

#### **Environment: prod**
```
Settings → Environments → prod → Add Secret
```
Mismos secrets que `dev` y `test`.

**Protection Rules:**
- ☑ Required reviewers: 2 personas mínimo
- ☑ Wait timer: 5 minutes
- ☑ Prevent self-review
- ☐ Deployment branches: Only protected branches

### 3.3 Configurar Branch Protection (main)

```
Settings → Branches → Add branch protection rule
```

- Branch name pattern: `main`
- ☑ Require a pull request before merging
- ☑ Require approvals: 1
- ☑ Require status checks to pass before merging
  - Add check: `validate` (Bicep validation)
- ☑ Require branches to be up to date before merging
- ☑ Do not allow bypassing the above settings

---

## 4. Primer Despliegue Infraestructura

### 4.1 Validar Bicep Localmente

```bash
cd infra/bicep

# Validar sintaxis
az bicep build --file main.bicep

# Ejecutar what-if para DEV (ver cambios sin aplicar)
az deployment sub what-if \
  --location eastus \
  --template-file main.bicep \
  --parameters parameters.dev.bicepparam
```

### 4.2 Desplegar DEV (automático con push)

```bash
# Commit y push (activa workflow automáticamente)
git add .
git commit -m "feat: add Bicep infrastructure"
git push origin main
```

Ve a GitHub Actions:
- `Actions` → `Deploy Infrastructure` → Ver ejecución
- Job `validate` debe pasar primero
- Job `deploy-dev` se ejecuta automáticamente

**Tiempo estimado**: 8-12 minutos

### 4.3 Desplegar TEST (manual workflow_dispatch)

```bash
# Desde GitHub UI
Actions → Deploy Infrastructure → Run workflow
- Branch: main
- Environment: test
- Click "Run workflow"
```

**Aprobación requerida**: 1 reviewer

### 4.4 Desplegar PROD (manual workflow_dispatch)

```bash
# Desde GitHub UI
Actions → Deploy Infrastructure → Run workflow
- Branch: main
- Environment: prod
- Click "Run workflow"
```

**Aprobación requerida**: 2 reviewers + 5 min wait time

---

## 5. Primer Despliegue Aplicación

### 5.1 Obtener Static Web App API Tokens

```bash
# Para DEV
az staticwebapp secrets list \
  --name stapp-lama-dev \
  --resource-group rg-lama-dev \
  --query properties.apiKey -o tsv

# Guardar en GitHub Secret (Environment dev):
# AZURE_STATIC_WEB_APPS_API_TOKEN_DEV = <token obtenido>

# Repetir para TEST y PROD
```

### 5.2 Configurar Environment Variables en App Service

```bash
# DEV - Verificar que KeyVault references estén correctos
az webapp config appsettings list \
  --name app-lama-dev \
  --resource-group rg-lama-dev

# Agregar variable de entorno adicional si falta
az webapp config appsettings set \
  --name app-lama-dev \
  --resource-group rg-lama-dev \
  --settings ASPNETCORE_ENVIRONMENT=Development
```

### 5.3 Crear Primera Migration EF Core (local)

```bash
cd src/Lama.Infrastructure

# Crear migration inicial
dotnet ef migrations add InitialCreate \
  --startup-project ../Lama.API/Lama.API.csproj

# Verificar migration files generados
ls Migrations/

# Commit migration
git add Migrations/
git commit -m "feat: add initial EF Core migration"
git push origin main
```

### 5.4 Desplegar API + Frontend

```bash
# Automático con push a main (despliega a DEV)
git push origin main

# Verificar en Actions → Deploy Application

# Para TEST o PROD: Manual workflow_dispatch
Actions → Deploy Application → Run workflow
- Environment: test o prod
```

**Orden de deployment**:
1. Build API + Frontend (paralelo)
2. Deploy API → Run migrations → Health check
3. Deploy Frontend → Health check

---

## 6. Verificación Post-Deployment

### 6.1 Verificar Recursos Creados

```bash
# Listar recursos en DEV
az resource list \
  --resource-group rg-lama-dev \
  --output table

# Verificar App Service está corriendo
az webapp show \
  --name app-lama-dev \
  --resource-group rg-lama-dev \
  --query state

# Verificar Static Web App
az staticwebapp show \
  --name stapp-lama-dev \
  --resource-group rg-lama-dev \
  --query defaultHostname -o tsv
```

### 6.2 Test Endpoints API

```bash
# Health check
curl https://app-lama-dev.azurewebsites.net/health

# Swagger UI
open https://app-lama-dev.azurewebsites.net/swagger

# Test API endpoint
curl https://app-lama-dev.azurewebsites.net/api/v1/activities \
  -H "Accept: application/json"
```

### 6.3 Test Frontend

```bash
# Abrir en navegador
open https://stapp-lama-dev.azurestaticapps.net

# Verificar consola del navegador (F12):
# - No errores de CORS
# - API_URL correcta
# - Requests exitosos
```

### 6.4 Verificar Logs Application Insights

```bash
# Query logs (últimos 30 minutos)
az monitor app-insights query \
  --app appi-lama-dev \
  --resource-group rg-lama-dev \
  --analytics-query "requests | where timestamp > ago(30m) | summarize count() by resultCode"
```

---

## 7. Troubleshooting

### Error: "OIDC token validation failed"

**Causa**: Federated credential mal configurado

**Solución**:
```bash
# Verificar federated credentials
az ad app federated-credential list --id $APP_ID

# Recrear credential si subject no coincide
az ad app federated-credential delete --id $APP_ID --federated-credential-id <id>
# Ejecutar paso 2.2 de nuevo
```

### Error: "KeyVault access denied"

**Causa**: Managed Identity sin permisos en KeyVault

**Solución**:
```bash
# Obtener Managed Identity del App Service
APP_IDENTITY=$(az webapp identity show \
  --name app-lama-dev \
  --resource-group rg-lama-dev \
  --query principalId -o tsv)

# Asignar access policy
az keyvault set-policy \
  --name kv-lama-dev-* \
  --object-id $APP_IDENTITY \
  --secret-permissions get list
```

### Error: "EF Core migration failed"

**Causa**: Connection string incorrecta o SQL firewall

**Solución**:
```bash
# Agregar IP del runner de GitHub a SQL firewall
az sql server firewall-rule create \
  --resource-group rg-lama-dev \
  --server sql-lama-dev \
  --name AllowGitHubActions \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 255.255.255.255

# Verificar connection string en KeyVault
az keyvault secret show \
  --vault-name kv-lama-dev-* \
  --name SqlConnectionString
```

### Error: "Static Web App deployment failed"

**Causa**: API Token incorrecto o expirado

**Solución**:
```bash
# Regenerar API token
az staticwebapp secrets reset \
  --name stapp-lama-dev \
  --resource-group rg-lama-dev

# Actualizar secret en GitHub Environment
```

---

## 📞 Soporte

- **Documentación Azure**: https://learn.microsoft.com/azure
- **Bicep Reference**: https://learn.microsoft.com/azure/azure-resource-manager/bicep
- **GitHub Actions OIDC**: https://docs.github.com/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect

---

## ✅ Checklist Final

Antes de ir a producción, verifica:

- [ ] OIDC configurado correctamente (3 federated credentials)
- [ ] GitHub Environments creados (dev/test/prod) con secrets
- [ ] Branch protection en `main` habilitado
- [ ] Infraestructura desplegada en DEV y TEST exitosamente
- [ ] Aplicación desplegada en DEV y TEST exitosamente
- [ ] Health checks pasan en todos los ambientes
- [ ] Application Insights recibiendo telemetría
- [ ] Logs centralizados en Log Analytics
- [ ] KeyVault con todos los secretos necesarios
- [ ] SQL Database con migrations aplicadas
- [ ] Storage Account con container `evidences` creado
- [ ] Redis Cache funcionando (test con cache API)
- [ ] Static Web App sirviendo frontend correctamente
- [ ] CORS configurado en API para frontend
- [ ] Custom domains configurados (opcional)
- [ ] Budget alerts configuradas en Azure
- [ ] Reviewers asignados para PROD environment

**¡Listo para producción! 🚀**
