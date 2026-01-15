// ============================================================================
// COR L.A.MA - Static Web App Module (Next.js Frontend)
// ============================================================================
// Despliega Azure Static Web App para hosting de aplicación Next.js
// ============================================================================

@description('Ubicación de Azure (Static Web Apps tiene disponibilidad limitada)')
param location string

@description('Entorno (dev/test/prod)')
param environment string

@description('Prefijo del proyecto')
param projectPrefix string

@description('SKU de Static Web App')
@allowed(['Free', 'Standard'])
param staticWebAppSku string

@description('Tags para los recursos')
param tags object

@description('URL de la API (App Service)')
param apiUrl string

// ============================================================================
// VARIABLES
// ============================================================================

var staticWebAppName = 'swa-${projectPrefix}-${environment}'

// ============================================================================
// STATIC WEB APP
// ============================================================================

resource staticWebApp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: staticWebAppName
  location: location
  tags: tags
  sku: {
    name: staticWebAppSku
    tier: staticWebAppSku
  }
  properties: {
    repositoryUrl: 'https://github.com/CSA-DanielVillamizar/L.A.M.A.-Mototurismo'
    branch: environment == 'prod' ? 'main' : environment
    buildProperties: {
      appLocation: '/frontend'
      apiLocation: ''
      outputLocation: '.next'
      appBuildCommand: 'npm run build'
      apiBuildCommand: ''
    }
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
    enterpriseGradeCdnStatus: staticWebAppSku == 'Standard' ? 'Enabled' : 'Disabled'
  }
}

// ============================================================================
// APP SETTINGS (Environment Variables para Next.js)
// ============================================================================

resource appSettings 'Microsoft.Web/staticSites/config@2023-01-01' = {
  parent: staticWebApp
  name: 'appsettings'
  properties: {
    NEXT_PUBLIC_API_URL: apiUrl
    NEXT_PUBLIC_ENVIRONMENT: environment
  }
}

// ============================================================================
// CUSTOM DOMAIN (se configura manualmente post-deployment)
// ============================================================================

// Custom domain se agrega después del deployment vía Azure Portal o CLI:
// az staticwebapp hostname set --name swa-lama-prod --hostname lama.com

// ============================================================================
// OUTPUTS
// ============================================================================

@description('Nombre del Static Web App')
output staticWebAppName string = staticWebApp.name

@description('URL por defecto del Static Web App')
output staticWebAppUrl string = 'https://${staticWebApp.properties.defaultHostname}'

@description('ID del recurso Static Web App')
output staticWebAppId string = staticWebApp.id

@description('Deployment token (para GitHub Actions)')
output staticWebAppDeploymentToken string = staticWebApp.listSecrets().properties.apiKey
