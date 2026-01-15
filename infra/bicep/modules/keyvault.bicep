// ============================================================================
// COR L.A.MA - Azure Key Vault Module
// ============================================================================
// Despliega Key Vault para gestión centralizada de secretos
// Incluye configuración de RBAC y audit logs
// ============================================================================

@description('Ubicación de Azure')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('Sufijo único')
param uniqueSuffix string

@description('Tenant ID de Azure AD')
param tenantId string

@description('Tags para los recursos')
param tags object

// ============================================================================
// VARIABLES
// ============================================================================

var keyVaultName = 'kv-${projectPrefix}-${environment}-${uniqueSuffix}'

// ============================================================================
// KEY VAULT
// ============================================================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: true // Permitir acceso durante deployment de Bicep
    enableRbacAuthorization: false // Usar access policies por ahora
    enableSoftDelete: true
    softDeleteRetentionInDays: environment == 'prod' ? 90 : 7
    enablePurgeProtection: environment == 'prod' ? true : null // Prod: no permitir purge manual
    publicNetworkAccess: 'Enabled' // Cambiar a 'Disabled' + Private Endpoint en prod
    networkAcls: {
      defaultAction: 'Allow' // Cambiar a 'Deny' en prod con whitelist
      bypass: 'AzureServices'
    }
    accessPolicies: [] // Se agregan dinámicamente desde otros módulos
  }
}

// ============================================================================
// DIAGNOSTIC SETTINGS (Audit Logs)
// ============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: keyVault
  name: 'SendToLogAnalytics'
  properties: {
    logs: [
      {
        category: 'AuditEvent'
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
// OUTPUTS
// ============================================================================

@description('Nombre del Key Vault')
output keyVaultName string = keyVault.name

@description('URI del Key Vault')
output keyVaultUri string = keyVault.properties.vaultUri

@description('ID del recurso Key Vault')
output keyVaultId string = keyVault.id
