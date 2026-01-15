// ============================================================================
// COR L.A.MA - Azure SQL Database Module
// ============================================================================
// Despliega SQL Server + Database con configuración multi-tenant
// Incluye firewall rules, auditing y almacenamiento de secretos en KeyVault
// ============================================================================

@description('Ubicación de Azure')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('Usuario administrador de SQL')
param sqlAdminUsername string

@description('Password del administrador SQL')
@secure()
param sqlAdminPassword string

@description('SKU de la base de datos')
param databaseSku object

@description('Tags para los recursos')
param tags object

@description('Nombre del Key Vault para almacenar secretos')
param keyVaultName string

// ============================================================================
// VARIABLES
// ============================================================================

var sqlServerName = 'sql-${projectPrefix}-${environment}'
var databaseName = 'LamaDb'

// ============================================================================
// SQL SERVER
// ============================================================================

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled' // Cambiar a 'Disabled' + Private Endpoint en prod
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// ============================================================================
// FIREWALL RULES
// ============================================================================

// Permitir servicios de Azure (para App Service, GitHub Actions)
resource allowAzureServicesRule 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Dev: permitir acceso desde cualquier IP (solo para desarrollo local)
resource allowAllIpsRule 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = if (environment == 'dev') {
  parent: sqlServer
  name: 'AllowAllIPs'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

// ============================================================================
// DATABASE
// ============================================================================

resource database 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: databaseSku
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: databaseSku.capacity * 1073741824 // capacity en GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: environment == 'prod' // HA solo en prod
    readScale: environment == 'prod' ? 'Enabled' : 'Disabled'
    requestedBackupStorageRedundancy: environment == 'prod' ? 'Geo' : 'Local'
  }
}

// ============================================================================
// AUDITING (Log Analytics integration)
// ============================================================================

resource auditingSettings 'Microsoft.Sql/servers/auditingSettings@2023-05-01-preview' = {
  parent: sqlServer
  name: 'default'
  properties: {
    state: 'Enabled'
    isAzureMonitorTargetEnabled: true
    retentionDays: environment == 'prod' ? 90 : 30
  }
}

// ============================================================================
// TRANSPARENT DATA ENCRYPTION (TDE)
// ============================================================================

resource transparentDataEncryption 'Microsoft.Sql/servers/databases/transparentDataEncryption@2023-05-01-preview' = {
  parent: database
  name: 'current'
  properties: {
    state: 'Enabled'
  }
}

// ============================================================================
// KEY VAULT SECRET (Connection String)
// ============================================================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'SqlConnectionString'
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${sqlAdminUsername};Password=${sqlAdminPassword};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
    contentType: 'text/plain'
  }
}

resource sqlAdminPasswordSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'SqlAdminPassword'
  properties: {
    value: sqlAdminPassword
    contentType: 'text/plain'
  }
}

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Nombre del SQL Server')
output sqlServerName string = sqlServer.name

@description('FQDN del SQL Server')
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName

@description('Nombre de la base de datos')
output sqlDatabaseName string = database.name

@description('URI del secreto de connection string en KeyVault')
output sqlConnectionStringSecretUri string = sqlConnectionStringSecret.properties.secretUri

@description('Principal ID del SQL Server (Managed Identity)')
output sqlServerPrincipalId string = sqlServer.identity.principalId
