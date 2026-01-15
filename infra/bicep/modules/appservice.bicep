// ============================================================================
// COR L.A.MA - App Service Module (API .NET 8)
// ============================================================================
// Despliega App Service Plan + App Service para API
// Incluye configuración de Managed Identity, KeyVault references y app settings
// ============================================================================

@description('Ubicación de Azure')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('SKU de App Service Plan')
param appServicePlanSku object

@description('Tags para los recursos')
param tags object

@description('Nombre del Key Vault')
param keyVaultName string

@description('URI del secreto SQL connection string')
@secure()
param sqlConnectionStringSecretUri string

@description('URI del secreto Storage connection string')
@secure()
param storageConnectionStringSecretUri string

@description('URI del secreto Redis connection string')
@secure()
param redisConnectionStringSecretUri string

@description('Instrumentation key de Application Insights')
@secure()
param applicationInsightsInstrumentationKey string

@description('Connection string de Application Insights')
@secure()
param applicationInsightsConnectionString string

@description('Azure AD Authority (Entra ID B2C)')
@secure()
param azureAdAuthority string

@description('Azure AD Client ID')
@secure()
param azureAdClientId string

@description('Azure AD Audience')
@secure()
param azureAdAudience string

// ============================================================================
// VARIABLES
// ============================================================================

var appServicePlanName = 'plan-${projectPrefix}-${environment}'
var appServiceName = 'app-${projectPrefix}-${environment}'

// ============================================================================
// APP SERVICE PLAN
// ============================================================================

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: appServicePlanSku
  kind: 'linux'
  properties: {
    reserved: true // Linux required
    zoneRedundant: environment == 'prod' // HA solo en prod
  }
}

// ============================================================================
// APP SERVICE (API)
// ============================================================================

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: appServiceName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: appServicePlanSku.name != 'F1' && appServicePlanSku.name != 'D1' // AlwaysOn solo en paid tiers
      http20Enabled: true
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      healthCheckPath: '/health'
      cors: {
        allowedOrigins: environment == 'dev' 
          ? ['*'] 
          : [
              'https://lama-${environment}.azurestaticapps.net'
            ]
        supportCredentials: true
      }
      appSettings: [
        // ASP.NET Core
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : environment == 'test' ? 'Staging' : 'Development'
        }
        // Application Insights
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        // Database (KeyVault reference)
        {
          name: 'ConnectionStrings__DefaultConnection'
          value: '@Microsoft.KeyVault(SecretUri=${sqlConnectionStringSecretUri})'
        }
        // Storage (KeyVault reference)
        {
          name: 'ConnectionStrings__AzureStorage'
          value: '@Microsoft.KeyVault(SecretUri=${storageConnectionStringSecretUri})'
        }
        // Redis (KeyVault reference)
        {
          name: 'ConnectionStrings__Redis'
          value: '@Microsoft.KeyVault(SecretUri=${redisConnectionStringSecretUri})'
        }
        // Azure AD (Entra ID B2C)
        {
          name: 'AzureAd__Authority'
          value: azureAdAuthority
        }
        {
          name: 'AzureAd__ClientId'
          value: azureAdClientId
        }
        {
          name: 'AzureAd__Audience'
          value: azureAdAudience
        }
        // Configuración LAMA
        {
          name: 'Lama__DefaultTenantId'
          value: '1'
        }
        {
          name: 'Lama__EnableSwagger'
          value: environment != 'prod' ? 'true' : 'false'
        }
      ]
    }
  }
}

// ============================================================================
// KEY VAULT ACCESS POLICY (Managed Identity)
// ============================================================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: appService.identity.tenantId
        objectId: appService.identity.principalId
        permissions: {
          secrets: ['get', 'list']
        }
      }
    ]
  }
}

// ============================================================================
// DIAGNOSTIC SETTINGS (Logs to Log Analytics)
// ============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: appService
  name: 'SendToLogAnalytics'
  properties: {
    logs: [
      {
        category: 'AppServiceHTTPLogs'
        enabled: true
        retentionPolicy: {
          enabled: false
          days: 0
        }
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
        retentionPolicy: {
          enabled: false
          days: 0
        }
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
        retentionPolicy: {
          enabled: false
          days: 0
        }
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
        retentionPolicy: {
          enabled: false
          days: 0
        }
      }
    ]
  }
}

// ============================================================================
// AUTO-SCALING (solo en prod)
// ============================================================================

resource autoScaleSettings 'Microsoft.Insights/autoscalesettings@2022-10-01' = if (environment == 'prod') {
  name: 'autoscale-${appServiceName}'
  location: location
  tags: tags
  properties: {
    enabled: true
    targetResourceUri: appServicePlan.id
    profiles: [
      {
        name: 'Auto scale based on CPU'
        capacity: {
          minimum: '2'
          maximum: '10'
          default: '2'
        }
        rules: [
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT5M'
              timeAggregation: 'Average'
              operator: 'GreaterThan'
              threshold: 70
            }
            scaleAction: {
              direction: 'Increase'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT5M'
            }
          }
          {
            metricTrigger: {
              metricName: 'CpuPercentage'
              metricResourceUri: appServicePlan.id
              timeGrain: 'PT1M'
              statistic: 'Average'
              timeWindow: 'PT10M'
              timeAggregation: 'Average'
              operator: 'LessThan'
              threshold: 30
            }
            scaleAction: {
              direction: 'Decrease'
              type: 'ChangeCount'
              value: '1'
              cooldown: 'PT10M'
            }
          }
        ]
      }
    ]
  }
}

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Nombre del App Service')
output appServiceName string = appService.name

@description('URL del App Service')
output appServiceUrl string = 'https://${appService.properties.defaultHostName}'

@description('Principal ID del App Service (Managed Identity)')
output appServicePrincipalId string = appService.identity.principalId

@description('Nombre del App Service Plan')
output appServicePlanName string = appServicePlan.name
