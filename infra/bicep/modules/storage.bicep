// ============================================================================
// COR L.A.MA - Azure Storage Account Module
// ============================================================================
// Despliega Storage Account con container 'evidences' para fotos de miembros
// Incluye configuración de CORS, lifecycle policies y secretos en KeyVault
// ============================================================================

@description('Ubicación de Azure')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('Sufijo único para el nombre de storage')
param uniqueSuffix string

@description('SKU de Storage Account')
param storageAccountSku string

@description('Tags para los recursos')
param tags object

@description('Nombre del Key Vault para almacenar secretos')
param keyVaultName string

// ============================================================================
// VARIABLES
// ============================================================================

// Storage account name must be lowercase, no hyphens, 3-24 chars
var storageAccountName = 'st${projectPrefix}${environment}${uniqueSuffix}'
var evidencesContainerName = 'evidences'

// ============================================================================
// STORAGE ACCOUNT
// ============================================================================

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: storageAccountSku
  }
  properties: {
    accessTier: 'Hot'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    allowBlobPublicAccess: false // Seguridad: no permitir acceso público anónimo
    allowSharedKeyAccess: true
    networkAcls: {
      defaultAction: 'Allow' // Cambiar a 'Deny' + Private Endpoint en prod
      bypass: 'AzureServices'
    }
    encryption: {
      services: {
        blob: {
          enabled: true
          keyType: 'Account'
        }
        file: {
          enabled: true
          keyType: 'Account'
        }
      }
      keySource: 'Microsoft.Storage'
    }
  }
}

// ============================================================================
// BLOB SERVICE (CORS Configuration)
// ============================================================================

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    cors: {
      corsRules: [
        {
          allowedOrigins: environment == 'dev' 
            ? ['*'] 
            : [
                'https://lama-${environment}.azurestaticapps.net'
                'https://app-${projectPrefix}-${environment}.azurewebsites.net'
              ]
          allowedMethods: ['GET', 'POST', 'PUT', 'DELETE', 'HEAD', 'OPTIONS']
          allowedHeaders: ['*']
          exposedHeaders: ['*']
          maxAgeInSeconds: 3600
        }
      ]
    }
    deleteRetentionPolicy: {
      enabled: true
      days: environment == 'prod' ? 30 : 7
    }
    containerDeleteRetentionPolicy: {
      enabled: true
      days: environment == 'prod' ? 30 : 7
    }
  }
}

// ============================================================================
// BLOB CONTAINER (evidences)
// ============================================================================

resource evidencesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: evidencesContainerName
  properties: {
    publicAccess: 'None' // Privado, acceso solo vía SAS
    metadata: {
      description: 'Container para evidencias fotográficas de miembros (pilot + odometer)'
    }
  }
}

// ============================================================================
// LIFECYCLE MANAGEMENT (Auto-delete old blobs)
// ============================================================================

resource lifecyclePolicy 'Microsoft.Storage/storageAccounts/managementPolicies@2023-01-01' = {
  parent: storageAccount
  name: 'default'
  properties: {
    policy: {
      rules: [
        {
          enabled: true
          name: 'DeleteOldEvidences'
          type: 'Lifecycle'
          definition: {
            filters: {
              blobTypes: ['blockBlob']
              prefixMatch: ['${evidencesContainerName}/']
            }
            actions: {
              baseBlob: {
                delete: {
                  daysAfterModificationGreaterThan: environment == 'prod' ? 730 : 365 // 2 años prod, 1 año dev/test
                }
                tierToCool: {
                  daysAfterModificationGreaterThan: 90 // Mover a Cool tier después de 90 días
                }
              }
            }
          }
        }
      ]
    }
  }
}

// ============================================================================
// KEY VAULT SECRETS
// ============================================================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource storageConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'StorageConnectionString'
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=core.windows.net'
    contentType: 'text/plain'
  }
}

resource storageAccountKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'StorageAccountKey'
  properties: {
    value: storageAccount.listKeys().keys[0].value
    contentType: 'text/plain'
  }
}

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Nombre de la Storage Account')
output storageAccountName string = storageAccount.name

@description('ID del recurso de Storage Account')
output storageAccountId string = storageAccount.id

@description('Endpoint primario de Blob')
output blobEndpoint string = storageAccount.properties.primaryEndpoints.blob

@description('Nombre del container de evidencias')
output evidencesContainerName string = evidencesContainerName

@description('URI del secreto de connection string en KeyVault')
output storageConnectionStringSecretUri string = storageConnectionStringSecret.properties.secretUri
