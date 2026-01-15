# Configuración de Azure Blob Storage para Evidencias

## Descripción

El sistema de evidencias utiliza **Azure Blob Storage** con **SAS URLs** (Shared Access Signatures) para permitir upload directo desde el cliente sin pasar las imágenes por el backend. Esto mejora el rendimiento, reduce costos de bandwidth y escala automáticamente.

---

## Requisitos Previos

1. **Cuenta de Azure Storage** (Standard LRS recomendado)
2. **Container de blobs** llamado `evidences` (se crea automáticamente si no existe)
3. **Connection String** de la cuenta de Azure Storage

---

## Pasos de Configuración

### 1. Crear Cuenta de Azure Storage

Desde Azure Portal o Azure CLI:

```bash
# Crear Resource Group (si no existe)
az group create --name lama-rg --location eastus

# Crear Storage Account
az storage account create \
  --name lamastorage2026 \
  --resource-group lama-rg \
  --location eastus \
  --sku Standard_LRS \
  --kind StorageV2 \
  --access-tier Hot

# Obtener Connection String
az storage account show-connection-string \
  --name lamastorage2026 \
  --resource-group lama-rg \
  --query connectionString \
  --output tsv
```

**Output esperado:**
```
DefaultEndpointsProtocol=https;AccountName=lamastorage2026;AccountKey=YOUR_KEY_HERE;EndpointSuffix=core.windows.net
```

### 2. Crear Container de Blobs

El container `evidences` se crea automáticamente la primera vez que se genera una SAS URL. Si prefieres crearlo manualmente:

```bash
# Crear container con acceso privado (sin acceso público)
az storage container create \
  --name evidences \
  --account-name lamastorage2026 \
  --auth-mode key \
  --public-access off
```

### 3. Configurar Variables de Entorno

#### **Opción A: appsettings.json (Desarrollo)**

Editar `src/Lama.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LamaDb": "Server=localhost;Database=LamaDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "AzureStorage": "DefaultEndpointsProtocol=https;AccountName=lamastorage2026;AccountKey=YOUR_KEY_HERE;EndpointSuffix=core.windows.net"
  },
  "AzureStorage": {
    "EvidenceContainerName": "evidences"
  }
}
```

**⚠️ IMPORTANTE:** NO commitear el connection string en Git. Usar User Secrets para desarrollo local:

```bash
cd src/Lama.API

# Inicializar User Secrets
dotnet user-secrets init

# Agregar connection string
dotnet user-secrets set "ConnectionStrings:AzureStorage" "DefaultEndpointsProtocol=https;AccountName=lamastorage2026;AccountKey=YOUR_KEY_HERE;EndpointSuffix=core.windows.net"
```

#### **Opción B: Variables de Entorno (Producción)**

Para Azure App Service:

```bash
az webapp config appsettings set \
  --name lama-api \
  --resource-group lama-rg \
  --settings ConnectionStrings__AzureStorage="DefaultEndpointsProtocol=https;AccountName=lamastorage2026;AccountKey=YOUR_KEY_HERE;EndpointSuffix=core.windows.net"

az webapp config appsettings set \
  --name lama-api \
  --resource-group lama-rg \
  --settings AzureStorage__EvidenceContainerName="evidences"
```

Para Docker/Kubernetes:

```bash
# Variable de entorno
export ConnectionStrings__AzureStorage="DefaultEndpointsProtocol=https;AccountName=lamastorage2026;AccountKey=YOUR_KEY_HERE;EndpointSuffix=core.windows.net"
export AzureStorage__EvidenceContainerName="evidences"
```

---

## Estructura de Blobs

Los blobs se organizan siguiendo esta estructura jerárquica:

```
evidences/
├── {TenantId}/
│   ├── {Year}/
│   │   ├── {EventId|start-year}/
│   │   │   ├── {MemberId}/
│   │   │   │   ├── {VehicleId}/
│   │   │   │   │   ├── {EvidenceType}/
│   │   │   │   │   │   ├── pilot_{CorrelationId}.jpg
│   │   │   │   │   │   └── odometer_{CorrelationId}.jpg
```

**Ejemplo concreto:**

```
evidences/
├── 550e8400-e29b-41d4-a716-446655440000/    # TenantId
│   ├── 2026/                                  # Year
│   │   ├── 123/                               # EventId
│   │   │   ├── 5/                             # MemberId
│   │   │   │   ├── 42/                        # VehicleId
│   │   │   │   │   ├── cutoff/                # EvidenceType
│   │   │   │   │   │   ├── pilot_a1b2c3d4.jpg
│   │   │   │   │   │   └── odometer_a1b2c3d4.jpg
│   │   ├── start-year/                        # Para evidencias de inicio de año
│   │   │   ├── 5/
│   │   │   │   ├── 42/
│   │   │   │   │   ├── start_year/
│   │   │   │   │   │   ├── pilot_e5f6g7h8.jpg
│   │   │   │   │   │   └── odometer_e5f6g7h8.jpg
```

**Beneficios de esta estructura:**
- ✅ Aislamiento por tenant (multi-tenancy)
- ✅ Organización cronológica (por año)
- ✅ Fácil auditoría y backup
- ✅ Permite políticas de lifecycle management por carpeta

---

## Configuración de Seguridad

### Permisos de SAS URLs

Las SAS URLs generadas tienen las siguientes restricciones:

| Característica | Valor | Motivo |
|----------------|-------|--------|
| **TTL** | 10 minutos | Limita ventana de ataque |
| **Permisos** | Write + Create | Solo permite subir, NO leer ni eliminar |
| **Resource** | Blob específico | No puede acceder a otros blobs |
| **Protocolo** | HTTPS only | Cifrado en tránsito |

### CORS (para clientes web)

Si usas frontend web (Next.js), debes configurar CORS en Azure Storage:

```bash
az storage cors add \
  --services b \
  --methods PUT OPTIONS \
  --origins "https://your-frontend-domain.com" \
  --allowed-headers "*" \
  --exposed-headers "*" \
  --max-age 3600 \
  --account-name lamastorage2026
```

Para desarrollo local:

```bash
az storage cors add \
  --services b \
  --methods PUT OPTIONS \
  --origins "http://localhost:3000" \
  --allowed-headers "*" \
  --exposed-headers "*" \
  --max-age 3600 \
  --account-name lamastorage2026
```

### Network Security (Opcional - Producción)

Limitar acceso solo desde App Service y VPN corporativa:

```bash
# Permitir solo desde App Service y IP corporativa
az storage account network-rule add \
  --resource-group lama-rg \
  --account-name lamastorage2026 \
  --ip-address 203.0.113.10  # IP de tu red

# Habilitar firewall
az storage account update \
  --name lamastorage2026 \
  --resource-group lama-rg \
  --default-action Deny
```

---

## Instalación de Paquete NuGet

El proyecto ya incluye la referencia, pero si necesitas agregarla manualmente:

```bash
cd src/Lama.Infrastructure
dotnet add package Azure.Storage.Blobs --version 12.19.1
```

**Paquete actual:**
```xml
<PackageReference Include="Azure.Storage.Blobs" Version="12.19.1" />
```

---

## Flujo de Upload Completo

### 1. Frontend solicita SAS URLs

```typescript
// En Next.js EvidenceUploader component
const response = await fetch('/api/evidence/upload-request', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    eventId: 123,
    memberId: 5,
    vehicleId: 42,
    evidenceType: 'CUTOFF',
    pilotPhotoContentType: 'image/jpeg',
    odometerPhotoContentType: 'image/jpeg'
  })
});

const data = await response.json();
// data.pilotPhotoSasUrl
// data.odometerPhotoSasUrl
// data.correlationId
// data.pilotPhotoBlobPath
// data.odometerPhotoBlobPath
```

### 2. Frontend sube fotos directamente a Blob

```typescript
// Upload foto de piloto
await fetch(data.pilotPhotoSasUrl, {
  method: 'PUT',
  headers: {
    'x-ms-blob-type': 'BlockBlob',
    'Content-Type': 'image/jpeg'
  },
  body: pilotPhotoFile
});

// Upload foto de odómetro
await fetch(data.odometerPhotoSasUrl, {
  method: 'PUT',
  headers: {
    'x-ms-blob-type': 'BlockBlob',
    'Content-Type': 'image/jpeg'
  },
  body: odometerPhotoFile
});
```

### 3. Frontend envía metadata al backend

```typescript
await fetch('/api/evidence/submit', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    correlationId: data.correlationId,
    eventId: 123,
    memberId: 5,
    vehicleId: 42,
    evidenceType: 'CUTOFF',
    pilotPhotoBlobPath: data.pilotPhotoBlobPath,
    odometerPhotoBlobPath: data.odometerPhotoBlobPath,
    odometerReading: 25000.5,
    odometerUnit: 'Kilometers'
  })
});
```

### 4. Admin/MTO revisa evidencia

```typescript
// Obtener evidencias pendientes
const evidences = await fetch('/api/evidence/pending', {
  headers: { 'Authorization': `Bearer ${token}` }
});

// Aprobar evidencia
await fetch('/api/evidence/review', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    evidenceId: 456,
    action: 'approve',
    reviewNotes: 'Evidencia válida'
  })
});
```

---

## Monitoreo y Troubleshooting

### Verificar Connection String

```bash
# Desde terminal en el servidor
dotnet run --ConnectionStrings:AzureStorage="..."

# Logs del backend mostrarán:
# [INFO] Generando SAS URLs para evidencia. CorrelationId: abc123
# [INFO] SAS URLs generadas exitosamente. ExpiresAt: 2026-01-15T10:15:00Z
```

### Verificar que Container Existe

```bash
az storage container show \
  --name evidences \
  --account-name lamastorage2026 \
  --auth-mode key
```

### Ver Blobs en Container

```bash
az storage blob list \
  --container-name evidences \
  --account-name lamastorage2026 \
  --auth-mode key \
  --output table
```

### Eliminar Blob Huérfano

```bash
az storage blob delete \
  --container-name evidences \
  --name "550e8400-e29b-41d4-a716-446655440000/2026/123/5/42/cutoff/pilot_abc123.jpg" \
  --account-name lamastorage2026 \
  --auth-mode key
```

### Logs de Azure Storage

Habilitar logs de diagnóstico:

```bash
az monitor diagnostic-settings create \
  --name storage-logs \
  --resource /subscriptions/{subscription-id}/resourceGroups/lama-rg/providers/Microsoft.Storage/storageAccounts/lamastorage2026 \
  --logs '[{"category": "StorageRead", "enabled": true}, {"category": "StorageWrite", "enabled": true}]' \
  --workspace {log-analytics-workspace-id}
```

---

## Lifecycle Management (Opcional)

Eliminar blobs antiguos automáticamente (ej: después de 2 años):

```bash
az storage account management-policy create \
  --account-name lamastorage2026 \
  --resource-group lama-rg \
  --policy '{
    "rules": [{
      "enabled": true,
      "name": "delete-old-evidences",
      "type": "Lifecycle",
      "definition": {
        "filters": {
          "blobTypes": ["blockBlob"],
          "prefixMatch": ["evidences/"]
        },
        "actions": {
          "baseBlob": {
            "delete": {
              "daysAfterModificationGreaterThan": 730
            }
          }
        }
      }
    }]
  }'
```

---

## Costos Estimados

**Almacenamiento Standard LRS (región US East):**
- Primeros 50 TB: $0.0208 / GB-mes
- Transacciones Write (Class 2): $0.10 / 10,000 ops
- Transacciones Read (Class 1): $0.004 / 10,000 ops

**Ejemplo: 1,000 evidencias/mes**
- 1,000 evidencias × 2 fotos × 2 MB = 4 GB/mes
- Almacenamiento: 4 GB × $0.0208 = $0.08/mes
- Transacciones Write: 2,000 ops × $0.10/10,000 = $0.02/mes
- **Total: ~$0.10/mes** 💰

**Escalado a 100,000 evidencias/año:**
- 200 GB almacenamiento × $0.0208 = $4.16/mes
- 200,000 ops × $0.10/10,000 = $2.00/mes
- **Total: ~$6/mes** 💰

---

## Referencias

- [Azure Blob Storage Documentation](https://learn.microsoft.com/en-us/azure/storage/blobs/)
- [SAS URLs Best Practices](https://learn.microsoft.com/en-us/azure/storage/common/storage-sas-overview)
- [Azure.Storage.Blobs SDK](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/storage/Azure.Storage.Blobs)
- [CORS Configuration](https://learn.microsoft.com/en-us/rest/api/storageservices/cross-origin-resource-sharing--cors--support-for-the-azure-storage-services)

---

**Última actualización:** 2026-01-15  
**Versión:** 1.0  
**Estado:** Implementado y listo para configuración ✅
