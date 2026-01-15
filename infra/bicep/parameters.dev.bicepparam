// ============================================================================
// COR L.A.MA - Development Environment Parameters
// ============================================================================
// Configuración optimizada para desarrollo local y testing
// SKUs de menor costo para experimentación
// ============================================================================

using './main.bicep'

param environment = 'dev'
param location = 'eastus'
param projectPrefix = 'lama'

param tags = {
  Environment: 'Development'
  Project: 'COR-LAMA'
  ManagedBy: 'Bicep'
  CostCenter: 'Development'
}

// SQL Database - Standard S0 (10 DTU, $15/mes)
param sqlDatabaseSku = {
  name: 'Standard'
  tier: 'Standard'
  capacity: 10 // 10 GB storage
}

// Storage Account - Standard LRS (redundancia local)
param storageAccountSku = 'Standard_LRS'

// App Service Plan - Basic B1 ($13/mes, 1.75 GB RAM, 1 core)
param appServicePlanSku = {
  name: 'B1'
  tier: 'Basic'
  size: 'B1'
  family: 'B'
  capacity: 1
}

// Redis Cache - Basic C0 ($17/mes, 250 MB)
param redisCacheSku = {
  name: 'Basic'
  family: 'C'
  capacity: 0
}

// Static Web App - Free tier
param staticWebAppSku = 'Free'

// SQL Admin credentials (desde GitHub Secrets)
param sqlAdminUsername = 'sqladmin'
// sqlAdminPassword viene de GitHub Secret: SQL_ADMIN_PASSWORD

// Entra ID B2C (desde GitHub Secrets)
// azureAdAuthority viene de: AZURE_AD_AUTHORITY
// azureAdClientId viene de: AZURE_AD_CLIENT_ID
// azureAdAudience viene de: AZURE_AD_AUDIENCE
