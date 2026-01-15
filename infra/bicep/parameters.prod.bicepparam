// ============================================================================
// COR L.A.MA - Production Environment Parameters
// ============================================================================
// Configuración para usuarios finales
// SKUs premium con alta disponibilidad y redundancia geográfica
// ============================================================================

using './main.bicep'

param environment = 'prod'
param location = 'eastus'
param projectPrefix = 'lama'

param tags = {
  Environment: 'Production'
  Project: 'COR-LAMA'
  ManagedBy: 'Bicep'
  CostCenter: 'Production'
  Compliance: 'Required'
}

// SQL Database - Standard S3 (100 DTU, $150/mes) con zone redundancy
param sqlDatabaseSku = {
  name: 'Standard'
  tier: 'Standard'
  capacity: 100 // 100 GB storage
}

// Storage Account - Standard ZRS (redundancia zonal)
param storageAccountSku = 'Standard_ZRS'

// App Service Plan - Premium P1v3 ($150/mes, 8 GB RAM, 2 cores, auto-scale)
param appServicePlanSku = {
  name: 'P1v3'
  tier: 'PremiumV3'
  size: 'P1v3'
  family: 'Pv3'
  capacity: 1
}

// Redis Cache - Standard C1 ($75/mes, 1 GB, replication)
param redisCacheSku = {
  name: 'Standard'
  family: 'C'
  capacity: 1
}

// Static Web App - Standard tier ($9/mes, custom domains, SLA 99.95%)
param staticWebAppSku = 'Standard'

// SQL Admin credentials
param sqlAdminUsername = 'sqladmin'
// sqlAdminPassword desde GitHub Secret: SQL_ADMIN_PASSWORD (FUERTE)

// Entra ID B2C Production (desde GitHub Secrets)
// azureAdAuthority desde: AZURE_AD_AUTHORITY
// azureAdClientId desde: AZURE_AD_CLIENT_ID
// azureAdAudience desde: AZURE_AD_AUDIENCE
