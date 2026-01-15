# COR L.A.MA - Infrastructure as Code (Azure Bicep)

Infraestructura Azure para la API y frontend de COR L.A.MA, con soporte multi-tenant y entornos dev/test/prod.

## 📋 Arquitectura

### Recursos Azure Desplegados

| Recurso | Propósito | SKU Recomendado |
|---------|-----------|-----------------|
| **Azure SQL Database** | Base de datos principal (multi-tenant) | Standard S1 (dev), S3 (prod) |
| **Storage Account** | Blob storage para evidencias fotográficas | Standard LRS (dev), ZRS (prod) |
| **App Service** | Host para API .NET 8 | B1 (dev), P1v3 (prod) |
| **Static Web App** | Host para Next.js frontend | Free (dev), Standard (prod) |
| **Redis Cache** | Caché distribuido (búsquedas, eventos) | Basic C0 (dev), Standard C1 (prod) |
| **Key Vault** | Gestión de secretos y connection strings | Standard |
| **Application Insights** | Telemetría y APM | Standard |
| **Log Analytics** | Logs centralizados | PerGB2018 |

### Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                     GitHub Actions CI/CD                     │
│  (GitHub Environments: dev, test, prod + OIDC)              │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                     Azure Resource Group                     │
│                    rg-lama-{environment}                     │
│                                                              │
│  ┌───────────────────┐      ┌──────────────────┐           │
│  │ Azure SQL Database│◄─────┤   Key Vault      │           │
│  │ sql-lama-{env}    │      │ kv-lama-{env}    │           │
│  │ (Multi-tenant DB) │      │ (Secrets/Conn)   │           │
│  └───────────────────┘      └──────────────────┘           │
│           ▲                          ▲                       │
│           │                          │                       │
│  ┌────────┴─────────┐       ┌───────┴──────────┐           │
│  │  App Service     │       │  Storage Account  │           │
│  │ app-lama-{env}   │       │ stlama{env}      │           │
│  │ (.NET 8 API)     │───────►│ Container:       │           │
│  │ /api/v1/*        │       │  evidences/      │           │
│  └──────────────────┘       └──────────────────┘           │
│           │                                                  │
│           │                  ┌──────────────────┐           │
│           └──────────────────┤  Redis Cache     │           │
│                              │ redis-lama-{env} │           │
│                              │ (Distributed)    │           │
│                              └──────────────────┘           │
│                                                              │
│  ┌─────────────────────────────────────────────┐           │
│  │     Static Web App (Next.js)                │           │
│  │     swa-lama-{env}                          │           │
│  │     https://lama-{env}.azurestaticapps.net  │           │
│  └─────────────────────────────────────────────┘           │
│                                                              │
│  ┌─────────────────────────────────────────────┐           │
│  │  Application Insights + Log Analytics       │           │
│  │  appi-lama-{env} + law-lama-{env}          │           │
│  │  (Telemetry, Logs, Metrics)                │           │
│  └─────────────────────────────────────────────┘           │
└─────────────────────────────────────────────────────────────┘
```

## 🚀 Quick Start

### Prerrequisitos

1. **Azure CLI** instalado y autenticado:
   ```bash
   az login
   az account set --subscription "YOUR_SUBSCRIPTION_ID"
   ```

2. **Bicep CLI** (incluido con Azure CLI 2.20+):
   ```bash
   az bicep version
   ```

3. **GitHub CLI** (opcional, para configurar secrets):
   ```bash
   gh auth login
   ```

### 1. Configurar GitHub Environments

Crea 3 environments en GitHub: `dev`, `test`, `prod`:

```bash
gh api repos/CSA-DanielVillamizar/L.A.M.A.-Mototurismo/environments/dev -X PUT
gh api repos/CSA-DanielVillamizar/L.A.M.A.-Mototurismo/environments/test -X PUT
gh api repos/CSA-DanielVillamizar/L.A.M.A.-Mototurismo/environments/prod -X PUT
```

### 2. Configurar OIDC para GitHub Actions

```bash
# Crear service principal para OIDC
az ad sp create-for-rbac \
  --name "sp-lama-github" \
  --role contributor \
  --scopes /subscriptions/{SUBSCRIPTION_ID} \
  --sdk-auth

# Configurar federated credential
az ad app federated-credential create \
  --id {APP_ID} \
  --parameters @github-federated-credential.json
```

Archivo `github-federated-credential.json`:
```json
{
  "name": "GithubActionsLAMA",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:CSA-DanielVillamizar/L.A.M.A.-Mototurismo:environment:prod",
  "audiences": ["api://AzureADTokenExchange"]
}
```

### 3. Agregar Secrets a GitHub Environments

Para **cada environment** (dev, test, prod), configura estos secrets:

```bash
# Azure credentials
gh secret set AZURE_CLIENT_ID --env dev --body "{CLIENT_ID}"
gh secret set AZURE_TENANT_ID --env dev --body "{TENANT_ID}"
gh secret set AZURE_SUBSCRIPTION_ID --env dev --body "{SUBSCRIPTION_ID}"

# SQL Admin password (genera uno seguro por entorno)
gh secret set SQL_ADMIN_PASSWORD --env dev --body "{GENERATED_PASSWORD}"

# Entra ID B2C (copia de appsettings.json)
gh secret set AZURE_AD_AUTHORITY --env dev --body "https://{tenant}.b2clogin.com/{tenant}.onmicrosoft.com/{policy}"
gh secret set AZURE_AD_CLIENT_ID --env dev --body "{B2C_CLIENT_ID}"
```

### 4. Deploy Infraestructura (Local)

```bash
# Dev environment
az deployment sub create \
  --name lama-infra-dev \
  --location eastus \
  --template-file infra/bicep/main.bicep \
  --parameters infra/bicep/parameters.dev.bicepparam

# Prod environment
az deployment sub create \
  --name lama-infra-prod \
  --location eastus \
  --template-file infra/bicep/main.bicep \
  --parameters infra/bicep/parameters.prod.bicepparam
```

### 5. Deploy via GitHub Actions

Push a `main` branch triggers el workflow automáticamente:

```bash
git push origin main
```

O ejecuta manualmente desde GitHub UI: **Actions** → **Deploy Infrastructure** → **Run workflow**

## 📁 Estructura de Archivos

```
infra/
├── bicep/
│   ├── modules/
│   │   ├── sql.bicep              # Azure SQL Database + Firewall rules
│   │   ├── storage.bicep          # Storage Account + container evidences
│   │   ├── appservice.bicep       # App Service Plan + App Service (API)
│   │   ├── staticwebapp.bicep     # Static Web App (Next.js)
│   │   ├── redis.bicep            # Redis Cache
│   │   ├── keyvault.bicep         # Key Vault + access policies
│   │   └── monitoring.bicep       # Application Insights + Log Analytics
│   ├── main.bicep                 # Orquestador principal
│   ├── parameters.dev.bicepparam  # Variables dev
│   ├── parameters.test.bicepparam # Variables test
│   └── parameters.prod.bicepparam # Variables prod
├── .github/
│   └── workflows/
│       ├── deploy-infra.yml       # Deploy infraestructura
│       └── deploy-app.yml         # Deploy aplicación (API + Frontend)
└── README.md
```

## 🔐 Gestión de Secretos

### Flujo de Secretos

1. **GitHub Environments** → secrets por entorno (dev/test/prod)
2. **GitHub Actions** → consume secrets y los pasa a Bicep como `@secure()` params
3. **Bicep** → almacena secretos en Key Vault durante deployment
4. **App Service** → referencia secretos desde Key Vault vía app settings
5. **API .NET** → lee secretos de configuración (KeyVault integrado)

### Ejemplo: Connection String SQL

```bicep
// Bicep escribe a Key Vault
resource sqlConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'SqlConnectionString'
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;...'
  }
}

// App Service referencia desde KeyVault
resource appSettings 'Microsoft.Web/sites/config@2023-01-01' = {
  name: 'appsettings'
  properties: {
    ConnectionStrings__DefaultConnection: '@Microsoft.KeyVault(SecretUri=${sqlConnectionSecret.properties.secretUri})'
  }
}
```

### Rotación de Secretos

```bash
# Rotar SQL admin password
az keyvault secret set \
  --vault-name kv-lama-prod \
  --name SqlAdminPassword \
  --value "{NEW_PASSWORD}"

# Actualizar SQL Server
az sql server update \
  --name sql-lama-prod \
  --resource-group rg-lama-prod \
  --admin-password "{NEW_PASSWORD}"
```

## 🌍 Entornos

### Development (dev)

- **Propósito:** Desarrollo local + CI testing
- **SKUs:** Tier más bajo (cost-effective)
- **URL API:** `https://app-lama-dev.azurewebsites.net`
- **URL Frontend:** `https://lama-dev.azurestaticapps.net`
- **Características:**
  - CORS permisivo (`*`)
  - Logs verbose
  - Debug symbols habilitados
  - SQL Database: Standard S0 (10 DTU)

### Test (test)

- **Propósito:** QA + Pre-producción
- **SKUs:** Tier medio
- **URL API:** `https://app-lama-test.azurewebsites.net`
- **URL Frontend:** `https://lama-test.azurestaticapps.net`
- **Características:**
  - CORS restringido a dominios conocidos
  - Logs moderados
  - Performance testing habilitado
  - SQL Database: Standard S1 (20 DTU)

### Production (prod)

- **Propósito:** Usuarios finales
- **SKUs:** Tier producción con HA
- **URL API:** `https://api.lama.com` (custom domain)
- **URL Frontend:** `https://lama.com` (custom domain)
- **Características:**
  - CORS solo dominios específicos
  - Logs esenciales + telemetry
  - Auto-scaling habilitado
  - Geo-redundancy (SQL ZRS, Storage ZRS)
  - SQL Database: Standard S3 (100 DTU) o Premium P1

## 📊 Costos Estimados (USD/mes)

| Entorno | SQL | Storage | App Service | Static Web | Redis | KeyVault | Insights | **Total** |
|---------|-----|---------|-------------|------------|-------|----------|----------|-----------|
| **Dev** | $15 | $1 | $13 | $0 | $17 | $0.03 | $5 | **~$51** |
| **Test** | $30 | $2 | $55 | $0 | $17 | $0.03 | $10 | **~$114** |
| **Prod** | $150 | $5 | $150 | $9 | $75 | $0.03 | $20 | **~$409** |

> **Nota:** Costos aproximados. Varían según región, uso de datos y DTUs de SQL.

## 🔧 Configuración Post-Deployment

### 1. Ejecutar Migraciones de Base de Datos

```bash
# Desde local con VPN/Firewall abierto
dotnet ef database update --project src/Lama.Infrastructure --startup-project src/Lama.API

# O desde GitHub Actions (recomendado)
# Ver workflow: .github/workflows/deploy-app.yml
```

### 2. Configurar Custom Domains

```bash
# Static Web App (frontend)
az staticwebapp hostname set \
  --name swa-lama-prod \
  --hostname lama.com

# App Service (API)
az webapp config hostname add \
  --webapp-name app-lama-prod \
  --resource-group rg-lama-prod \
  --hostname api.lama.com
```

### 3. Habilitar Managed Identity para KeyVault

```bash
# Asignar identity al App Service
az webapp identity assign \
  --name app-lama-prod \
  --resource-group rg-lama-prod

# Otorgar acceso a KeyVault
az keyvault set-policy \
  --name kv-lama-prod \
  --object-id {PRINCIPAL_ID} \
  --secret-permissions get list
```

### 4. Configurar CORS para API

```bash
az webapp cors add \
  --name app-lama-prod \
  --resource-group rg-lama-prod \
  --allowed-origins https://lama.com
```

## 🧪 Testing de Infraestructura

### Validar Bicep (Lint + Dry-run)

```bash
# Lint
az bicep build --file infra/bicep/main.bicep

# Dry-run (What-if)
az deployment sub what-if \
  --location eastus \
  --template-file infra/bicep/main.bicep \
  --parameters infra/bicep/parameters.dev.bicepparam
```

### Smoke Tests Post-Deploy

```bash
# API Health Check
curl https://app-lama-dev.azurewebsites.net/health

# SQL Connectivity
sqlcmd -S sql-lama-dev.database.windows.net -U sqladmin -d LamaDb -Q "SELECT @@VERSION"

# Blob Storage (SAS test)
az storage blob list \
  --account-name stlamadev \
  --container-name evidences \
  --auth-mode login

# Redis connectivity
redis-cli -h redis-lama-dev.redis.cache.windows.net -p 6380 -a {PASSWORD} --tls PING
```

## 🚨 Troubleshooting

### Error: "Deployment failed - SQL firewall blocking"

**Solución:** Agregar IP de GitHub Actions runner al firewall SQL:

```bicep
// En sql.bicep, agregar:
resource githubActionsFirewallRule 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowGitHubActions'
  properties: {
    startIpAddress: '0.0.0.0'  // Temporal para deploy
    endIpAddress: '0.0.0.0'
  }
}
```

### Error: "KeyVault secret not found"

**Causa:** App Service identity sin permisos.

**Solución:**
```bash
az keyvault set-policy \
  --name kv-lama-prod \
  --object-id {APP_SERVICE_PRINCIPAL_ID} \
  --secret-permissions get list
```

### Error: "Static Web App build failed"

**Causa:** Faltan variables de entorno para Next.js.

**Solución:** Configurar en Static Web App settings:
```bash
az staticwebapp appsettings set \
  --name swa-lama-prod \
  --setting-names NEXT_PUBLIC_API_URL=https://api.lama.com
```

## 📚 Referencias

- [Azure Bicep Best Practices](https://learn.microsoft.com/azure/azure-resource-manager/bicep/best-practices)
- [GitHub Actions Azure Login](https://github.com/marketplace/actions/azure-login)
- [App Service KeyVault References](https://learn.microsoft.com/azure/app-service/app-service-key-vault-references)
- [Static Web Apps with Next.js](https://learn.microsoft.com/azure/static-web-apps/deploy-nextjs-hybrid)
- [Azure SQL Security Best Practices](https://learn.microsoft.com/azure/azure-sql/database/security-best-practice)

## 🤝 Soporte

- **Infraestructura:** [CSA-DanielVillamizar](https://github.com/CSA-DanielVillamizar)
- **Issues:** [GitHub Issues](https://github.com/CSA-DanielVillamizar/L.A.M.A.-Mototurismo/issues)

---

**Última actualización:** 15 Enero 2026  
**Versión:** 1.0.0
