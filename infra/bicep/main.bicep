// ============================================================================
// COR L.A.MA - Main Infrastructure Deployment
// ============================================================================
// Orquestador principal que despliega toda la infraestructura Azure
// Usa módulos separados para cada recurso (SQL, Storage, Redis, etc.)
// ============================================================================

targetScope = 'subscription'

// ============================================================================
// PARAMETERS
// ============================================================================

@description('Entorno de despliegue')
@allowed(['dev', 'test', 'prod'])
param environment string

@description('Ubicación primaria de Azure')
param location string = 'eastus'

@description('Prefijo para nombres de recursos (ej: lama)')
@minLength(3)
@maxLength(10)
param projectPrefix string = 'lama'

@description('Tags comunes para todos los recursos')
param tags object = {
  Environment: environment
  Project: 'COR-LAMA'
  ManagedBy: 'Bicep'
  DeployedFrom: 'GitHub-Actions'
}

// SQL Database parameters
@description('Usuario administrador de SQL')
param sqlAdminUsername string = 'sqladmin'

@description('Password del administrador SQL (desde KeyVault/GitHub Secret)')
@secure()
param sqlAdminPassword string

@description('SKU de SQL Database')
param sqlDatabaseSku object

// Storage parameters
@description('SKU de Storage Account')
param storageAccountSku string

// App Service parameters
@description('SKU de App Service Plan')
param appServicePlanSku object

// Redis parameters
@description('SKU de Redis Cache')
param redisCacheSku object

// Static Web App parameters
@description('SKU de Static Web App')
@allowed(['Free', 'Standard'])
param staticWebAppSku string = 'Free'

// Entra ID B2C parameters (desde GitHub Secrets)
@secure()
param azureAdAuthority string

@secure()
param azureAdClientId string

@secure()
param azureAdAudience string

// ============================================================================
// VARIABLES
// ============================================================================

var resourceGroupName = 'rg-${projectPrefix}-${environment}'
var uniqueSuffix = uniqueString(subscription().subscriptionId, resourceGroupName)
var shortUniqueSuffix = substring(uniqueSuffix, 0, 6)

// ============================================================================
// RESOURCE GROUP
// ============================================================================

resource resourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

// ============================================================================
// MONITORING (Log Analytics + Application Insights)
// ============================================================================

module monitoring 'modules/monitoring.bicep' = {
  scope: resourceGroup
  name: 'monitoring-deployment'
  params: {
    location: location
    environment: environment
    projectPrefix: projectPrefix
    tags: tags
  }
}

// ============================================================================
// KEY VAULT
// ============================================================================

module keyVault 'modules/keyvault.bicep' = {
  scope: resourceGroup
  name: 'keyvault-deployment'
  params: {
    location: location
    environment: environment
    projectPrefix: projectPrefix
    uniqueSuffix: shortUniqueSuffix
    tags: tags
    tenantId: subscription().tenantId
  }
}

// ============================================================================
// SQL DATABASE
// ============================================================================

module sqlDatabase 'modules/sql.bicep' = {
  scope: resourceGroup
  name: 'sql-deployment'
  params: {
    location: location
    environment: environment
    projectPrefix: projectPrefix
    sqlAdminUsername: sqlAdminUsername
    sqlAdminPassword: sqlAdminPassword
    databaseSku: sqlDatabaseSku
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
  }
}

// ============================================================================
// STORAGE ACCOUNT
// ============================================================================

module storage 'modules/storage.bicep' = {
  scope: resourceGroup
  name: 'storage-deployment'
  params: {
    location: location
    environment: environment
    projectPrefix: projectPrefix
    uniqueSuffix: shortUniqueSuffix
    storageAccountSku: storageAccountSku
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
  }
}

// ============================================================================
// REDIS CACHE
// ============================================================================

module redis 'modules/redis.bicep' = {
  scope: resourceGroup
  name: 'redis-deployment'
  params: {
    location: location
    environment: environment
    projectPrefix: projectPrefix
    redisCacheSku: redisCacheSku
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
  }
}

// ============================================================================
// APP SERVICE (API .NET 8)
// ============================================================================

module appService 'modules/appservice.bicep' = {
  scope: resourceGroup
  name: 'appservice-deployment'
  params: {
    location: location
    environment: environment
    projectPrefix: projectPrefix
    appServicePlanSku: appServicePlanSku
    tags: tags
    keyVaultName: keyVault.outputs.keyVaultName
    sqlConnectionStringSecretUri: sqlDatabase.outputs.sqlConnectionStringSecretUri
    storageConnectionStringSecretUri: storage.outputs.storageConnectionStringSecretUri
    redisConnectionStringSecretUri: redis.outputs.redisConnectionStringSecretUri
    applicationInsightsInstrumentationKey: monitoring.outputs.applicationInsightsInstrumentationKey
    applicationInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    azureAdAuthority: azureAdAuthority
    azureAdClientId: azureAdClientId
    azureAdAudience: azureAdAudience
  }
}

// ============================================================================
// STATIC WEB APP (Next.js Frontend)
// ============================================================================

module staticWebApp 'modules/staticwebapp.bicep' = {
  scope: resourceGroup
  name: 'staticwebapp-deployment'
  params: {
    location: location == 'eastus' ? 'eastus2' : location // SWA no disponible en todas las regiones
    environment: environment
    projectPrefix: projectPrefix
    staticWebAppSku: staticWebAppSku
    tags: tags
    apiUrl: appService.outputs.appServiceUrl
  }
}

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Nombre del Resource Group desplegado')
output resourceGroupName string = resourceGroup.name

@description('URL de la API (App Service)')
output apiUrl string = appService.outputs.appServiceUrl

@description('URL del Frontend (Static Web App)')
output frontendUrl string = staticWebApp.outputs.staticWebAppUrl

@description('Nombre del SQL Server')
output sqlServerName string = sqlDatabase.outputs.sqlServerName

@description('Nombre de la base de datos')
output sqlDatabaseName string = sqlDatabase.outputs.sqlDatabaseName

@description('Nombre de la Storage Account')
output storageAccountName string = storage.outputs.storageAccountName

@description('Nombre del Redis Cache')
output redisCacheName string = redis.outputs.redisCacheName

@description('Nombre del Key Vault')
output keyVaultName string = keyVault.outputs.keyVaultName

@description('Workspace ID de Log Analytics')
output logAnalyticsWorkspaceId string = monitoring.outputs.logAnalyticsWorkspaceId

@description('Nombre de Application Insights')
output applicationInsightsName string = monitoring.outputs.applicationInsightsName

@description('Principal ID del App Service (Managed Identity)')
output appServicePrincipalId string = appService.outputs.appServicePrincipalId
