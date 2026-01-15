// ============================================================================
// COR L.A.MA - Azure Redis Cache Module
// ============================================================================
// Despliega Redis Cache para caché distribuido (búsquedas, eventos, sesiones)
// Incluye configuración de seguridad y secretos en KeyVault
// ============================================================================

@description('Ubicación de Azure')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('SKU de Redis Cache')
param redisCacheSku object

@description('Tags para los recursos')
param tags object

@description('Nombre del Key Vault para almacenar secretos')
param keyVaultName string

// ============================================================================
// VARIABLES
// ============================================================================

var redisCacheName = 'redis-${projectPrefix}-${environment}'

// ============================================================================
// REDIS CACHE
// ============================================================================

resource redisCache 'Microsoft.Cache/redis@2023-08-01' = {
  name: redisCacheName
  location: location
  tags: tags
  properties: {
    sku: redisCacheSku
    enableNonSslPort: false // Seguridad: solo SSL
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled' // Cambiar a 'Disabled' + Private Endpoint en prod
    redisConfiguration: {
      'maxmemory-policy': 'allkeys-lru' // Evict least recently used keys
      'maxmemory-reserved': '50' // MB reservados para overhead
    }
    redisVersion: '6'
  }
}

// ============================================================================
// FIREWALL RULES (solo prod con private endpoint)
// ============================================================================

// Dev/Test: permitir acceso desde cualquier IP
// Prod: usar Private Endpoint (no incluido aquí, requiere VNet)

// ============================================================================
// KEY VAULT SECRETS
// ============================================================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource redisConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'RedisConnectionString'
  properties: {
    value: '${redisCache.properties.hostName}:${redisCache.properties.sslPort},password=${redisCache.listKeys().primaryKey},ssl=True,abortConnect=False'
    contentType: 'text/plain'
  }
}

resource redisPrimaryKeySecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'RedisPrimaryKey'
  properties: {
    value: redisCache.listKeys().primaryKey
    contentType: 'text/plain'
  }
}

// ============================================================================
// DIAGNOSTIC SETTINGS
// ============================================================================

resource diagnosticSettings 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  scope: redisCache
  name: 'SendToLogAnalytics'
  properties: {
    logs: [
      {
        category: 'ConnectedClientList'
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

@description('Nombre del Redis Cache')
output redisCacheName string = redisCache.name

@description('Hostname del Redis Cache')
output redisHostName string = redisCache.properties.hostName

@description('Puerto SSL del Redis Cache')
output redisSslPort int = redisCache.properties.sslPort

@description('URI del secreto de connection string en KeyVault')
output redisConnectionStringSecretUri string = redisConnectionStringSecret.properties.secretUri
