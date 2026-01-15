// ============================================================================
// COR L.A.MA - Test Environment Parameters
// ============================================================================
// Configuración para QA y pre-producción
// SKUs intermedios para testing de performance
// ============================================================================

using './main.bicep'

param environment = 'test'
param location = 'eastus'
param projectPrefix = 'lama'

param tags = {
  Environment: 'Test'
  Project: 'COR-LAMA'
  ManagedBy: 'Bicep'
  CostCenter: 'Testing'
}

// SQL Database - Standard S1 (20 DTU, $30/mes)
param sqlDatabaseSku = {
  name: 'Standard'
  tier: 'Standard'
  capacity: 20 // 20 GB storage
}

// Storage Account - Standard LRS
param storageAccountSku = 'Standard_LRS'

// App Service Plan - Standard S1 ($55/mes, 1.75 GB RAM, 1 core, staging slots)
param appServicePlanSku = {
  name: 'S1'
  tier: 'Standard'
  size: 'S1'
  family: 'S'
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

// SQL Admin credentials
param sqlAdminUsername = 'sqladmin'
// sqlAdminPassword desde GitHub Secret: SQL_ADMIN_PASSWORD

// Entra ID B2C (desde GitHub Secrets)
// azureAdAuthority desde: AZURE_AD_AUTHORITY
// azureAdClientId desde: AZURE_AD_CLIENT_ID
// azureAdAudience desde: AZURE_AD_AUDIENCE
