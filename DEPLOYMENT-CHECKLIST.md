# 🎯 Checklist de Configuración - COR L.A.MA

## Pre-Deployment

### Azure Setup
- [ ] Cuenta Azure con subscription activa
- [ ] Permisos Owner o Contributor en subscription
- [ ] Azure CLI instalado (`az --version >= 2.50`)
- [ ] Bicep CLI instalado (`az bicep version >= 0.24`)

### GitHub Setup
- [ ] Repositorio creado en GitHub
- [ ] Branch `main` como default
- [ ] Permisos Admin en el repositorio

### Local Development
- [ ] .NET 8 SDK instalado
- [ ] Node.js 20+ instalado
- [ ] SQL Server / LocalDB instalado
- [ ] Git configurado

---

## OIDC Configuration

- [ ] Azure AD App Registration creada
- [ ] Service Principal creado
- [ ] 3 Federated Credentials configurados:
  - [ ] DEV (subject: `repo:owner/name:ref:refs/heads/main`)
  - [ ] TEST (subject: `repo:owner/name:environment:test`)
  - [ ] PROD (subject: `repo:owner/name:environment:prod`)
- [ ] Contributor role asignado al Service Principal
- [ ] IDs guardados de forma segura:
  - [ ] `AZURE_CLIENT_ID`
  - [ ] `AZURE_TENANT_ID`
  - [ ] `AZURE_SUBSCRIPTION_ID`

**Script**: `infra/scripts/setup-oidc.sh`

---

## GitHub Environments

### Environment: dev
- [ ] Environment creado
- [ ] Secrets configurados:
  - [ ] `AZURE_CLIENT_ID`
  - [ ] `AZURE_TENANT_ID`
  - [ ] `AZURE_SUBSCRIPTION_ID`
- [ ] No protection rules (auto-deploy)

### Environment: test
- [ ] Environment creado
- [ ] Secrets configurados (mismos que dev)
- [ ] Protection rules:
  - [ ] Required reviewers: 1 persona
  - [ ] Wait timer: 0 minutes

### Environment: prod
- [ ] Environment creado
- [ ] Secrets configurados (mismos que dev)
- [ ] Protection rules:
  - [ ] Required reviewers: 2 personas
  - [ ] Wait timer: 5 minutes
  - [ ] Prevent self-review

---

## Branch Protection (main)

- [ ] Branch protection rule creado para `main`
- [ ] Require pull request before merging
- [ ] Require approvals: 1+
- [ ] Require status checks to pass:
  - [ ] `validate` (Bicep validation)
- [ ] Require branches to be up to date

---

## Infrastructure Deployment

### DEV Environment
- [ ] Push inicial a `main` ejecutado
- [ ] Workflow `deploy-infra.yml` ejecutado exitosamente
- [ ] Recursos creados (verificar en Azure Portal):
  - [ ] Resource Group: `rg-lama-dev`
  - [ ] SQL Server + Database
  - [ ] App Service Plan + App Service
  - [ ] Storage Account + Container `evidences`
  - [ ] Redis Cache
  - [ ] Key Vault (con secretos)
  - [ ] Log Analytics + Application Insights
  - [ ] Static Web App
- [ ] Validación ejecutada: `./validate-deployment.sh dev`

### TEST Environment
- [ ] Workflow `deploy-infra.yml` ejecutado manualmente (workflow_dispatch)
- [ ] Aprobación de reviewer obtenida
- [ ] Recursos creados en `rg-lama-test`
- [ ] Validación ejecutada: `./validate-deployment.sh test`

### PROD Environment
- [ ] Workflow `deploy-infra.yml` ejecutado manualmente
- [ ] 2 aprobaciones obtenidas + 5 min wait
- [ ] Recursos creados en `rg-lama-prod`
- [ ] Smoke tests pasados
- [ ] Validación ejecutada: `./validate-deployment.sh prod`

---

## Application Deployment

### Static Web App Tokens
- [ ] Tokens obtenidos con `get-swa-tokens.ps1`
- [ ] Secrets configurados en GitHub Environments:
  - [ ] `AZURE_STATIC_WEB_APPS_API_TOKEN_DEV` (env: dev)
  - [ ] `AZURE_STATIC_WEB_APPS_API_TOKEN_TEST` (env: test)
  - [ ] `AZURE_STATIC_WEB_APPS_API_TOKEN_PROD` (env: prod)

### Database Migrations
- [ ] Firewall rule SQL Server configurado para GitHub Actions:
  - [ ] `AllowGitHubActions` rule (0.0.0.0 - 255.255.255.255)
- [ ] EF Core migrations creadas localmente
- [ ] Migrations pusheadas a repositorio

### Application Deployment - DEV
- [ ] Workflow `deploy-app.yml` ejecutado (auto con push a main)
- [ ] API desplegada a App Service
- [ ] Migrations ejecutadas en SQL Database
- [ ] Frontend desplegado a Static Web App
- [ ] Health checks pasados:
  - [ ] API: `https://app-lama-dev.azurewebsites.net/health`
  - [ ] Frontend: `https://stapp-lama-dev.azurestaticapps.net`

### Application Deployment - TEST
- [ ] Workflow ejecutado manualmente (workflow_dispatch)
- [ ] Aprobación obtenida
- [ ] Deployments exitosos
- [ ] Health checks pasados

### Application Deployment - PROD
- [ ] Workflow ejecutado manualmente
- [ ] 2 aprobaciones + 5 min wait
- [ ] API desplegada a staging slot primero
- [ ] Smoke tests en staging pasados
- [ ] Slot swap ejecutado (staging → production)
- [ ] Health checks en producción pasados
- [ ] Summary generado en GitHub Actions

---

## Post-Deployment Configuration

### Custom Domains (Opcional)
- [ ] DNS configurado:
  - [ ] `api.corlama.com` → App Service
  - [ ] `app.corlama.com` → Static Web App
- [ ] SSL certificates configurados
- [ ] CORS actualizado con dominios custom

### Monitoring & Alerts
- [ ] Application Insights recibiendo telemetría
- [ ] Log Analytics con queries guardadas
- [ ] Alertas configuradas (solo prod):
  - [ ] CPU > 80% (5 min)
  - [ ] DTU > 90%
  - [ ] HTTP 5xx > 10/min
  - [ ] Response time P95 > 2s

### Cost Management
- [ ] Budget alert configurado en Azure
- [ ] Notificaciones por email:
  - [ ] 80% del presupuesto
  - [ ] 100% del presupuesto
- [ ] Revisión mensual de costos agendada

### Security Review
- [ ] Secrets rotation schedule definido
- [ ] Access reviews configurados (cada 90 días)
- [ ] Security alerts habilitados
- [ ] Defender for Cloud recomendaciones revisadas

---

## Local Development Setup

### API
- [ ] Connection string local configurado
- [ ] EF Core migrations aplicadas localmente
- [ ] `dotnet run` ejecutado exitosamente
- [ ] Swagger UI accesible: `https://localhost:7001/swagger`

### Frontend
- [ ] Dependencias instaladas: `npm install`
- [ ] `.env.local` configurado con API URL
- [ ] `npm run dev` ejecutado exitosamente
- [ ] Frontend accesible: `http://localhost:3000`

### Testing
- [ ] Unit tests ejecutados: `dotnet test`
- [ ] Coverage report generado
- [ ] Todos los tests pasando

---

## Documentation

- [ ] README.md actualizado con URLs finales
- [ ] Equipo onboarded en arquitectura
- [ ] Runbook para incidentes creado
- [ ] Knowledge base actualizada

---

## Sign-Off

### Dev Team
- [ ] Arquitecto: ______________________
- [ ] Backend Lead: ______________________
- [ ] Frontend Lead: ______________________
- [ ] DevOps Engineer: ______________________

### Stakeholders
- [ ] Product Owner: ______________________
- [ ] Project Manager: ______________________

**Fecha de completado**: _______________  
**Versión**: 1.0  
**Next Review**: Q2 2024
