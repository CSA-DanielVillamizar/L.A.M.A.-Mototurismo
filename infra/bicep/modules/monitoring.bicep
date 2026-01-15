// ============================================================================
// COR L.A.MA - Monitoring Module (Log Analytics + Application Insights)
// ============================================================================
// Despliega infraestructura de observabilidad y telemetría
// ============================================================================

@description('Ubicación de Azure')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('Tags para los recursos')
param tags object

// ============================================================================
// VARIABLES
// ============================================================================

var logAnalyticsWorkspaceName = 'law-${projectPrefix}-${environment}'
var applicationInsightsName = 'appi-${projectPrefix}-${environment}'

// ============================================================================
// LOG ANALYTICS WORKSPACE
// ============================================================================

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: environment == 'prod' ? 90 : 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// ============================================================================
// APPLICATION INSIGHTS
// ============================================================================

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  tags: tags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
    RetentionInDays: environment == 'prod' ? 90 : 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
    IngestionMode: 'LogAnalytics' // Usar Log Analytics como backend
  }
}

// ============================================================================
// ALERT RULES (solo en prod)
// ============================================================================

// Alert: CPU alto en App Service
resource highCpuAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = if (environment == 'prod') {
  name: 'alert-${projectPrefix}-high-cpu'
  location: 'global'
  tags: tags
  properties: {
    description: 'Alert cuando CPU del App Service supera 80%'
    severity: 2
    enabled: true
    scopes: []
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'HighCPU'
          metricName: 'CpuPercentage'
          operator: 'GreaterThan'
          threshold: 80
          timeAggregation: 'Average'
        }
      ]
    }
    actions: []
  }
}

// Alert: SQL DTU alto
resource highDtuAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = if (environment == 'prod') {
  name: 'alert-${projectPrefix}-high-dtu'
  location: 'global'
  tags: tags
  properties: {
    description: 'Alert cuando DTU de SQL supera 80%'
    severity: 2
    enabled: true
    scopes: []
    evaluationFrequency: 'PT5M'
    windowSize: 'PT15M'
    criteria: {
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
      allOf: [
        {
          name: 'HighDTU'
          metricName: 'dtu_consumption_percent'
          operator: 'GreaterThan'
          threshold: 80
          timeAggregation: 'Average'
        }
      ]
    }
    actions: []
  }
}

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Nombre del Log Analytics Workspace')
output logAnalyticsWorkspaceName string = logAnalyticsWorkspace.name

@description('ID del Log Analytics Workspace')
output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id

@description('Nombre de Application Insights')
output applicationInsightsName string = applicationInsights.name

@description('Instrumentation Key de Application Insights')
output applicationInsightsInstrumentationKey string = applicationInsights.properties.InstrumentationKey

@description('Connection String de Application Insights')
output applicationInsightsConnectionString string = applicationInsights.properties.ConnectionString
