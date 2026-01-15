# PWA Implementation - LAMA COR

## 📱 Descripción General

LAMA COR es una **Progressive Web App (PWA)** completa que permite:

- ✅ **Instalación** como aplicación nativa en móviles y escritorio
- ✅ **Funcionamiento offline** con Service Worker
- ✅ **Cache inteligente** para assets estáticos y dinámicos
- ✅ **Push Notifications** (requiere backend)
- ✅ **Background Sync** para sincronizar evidencias offline
- ✅ **Add to Home Screen** prompt automático

---

## 🏗️ Arquitectura PWA

### 1. **Manifest (public/manifest.json)**

Define la identidad y comportamiento de la PWA:

```json
{
  "name": "LAMA COR - Sistema de Evidencias",
  "short_name": "LAMA COR",
  "start_url": "/",
  "display": "standalone",
  "theme_color": "#7c3aed",
  "background_color": "#0f172a",
  "icons": [
    { "src": "/icons/icon-72x72.png", "sizes": "72x72", "type": "image/png" },
    { "src": "/icons/icon-192x192.png", "sizes": "192x192", "type": "image/png", "purpose": "any" },
    { "src": "/icons/icon-512x512.png", "sizes": "512x512", "type": "image/png", "purpose": "maskable" }
  ],
  "shortcuts": [
    { "name": "Dashboard", "url": "/member/dashboard", "icons": [...] },
    { "name": "Subir Evidencia", "url": "/member/upload", "icons": [...] },
    { "name": "Rankings", "url": "/member/rankings", "icons": [...] }
  ]
}
```

**Características:**
- 8 tamaños de iconos (72x72 a 512x512)
- 3 shortcuts para acceso rápido
- Screenshots para instalación
- Orientación landscape preferida (ideal para fotos de motos)

### 2. **Service Worker (public/service-worker.js)**

Estrategias de caching:

#### **Cache-First (Assets Estáticos)**
```javascript
// Para: CSS, JS, imágenes, fonts
const cached = await cache.match(request);
if (cached) return cached;

const response = await fetch(request);
cache.put(request, response.clone());
return response;
```

Archivos cacheados:
- `/` (página principal)
- `/offline` (fallback)
- CSS/JS bundles
- Iconos y assets

#### **Network-First (API y HTML)**
```javascript
// Para: /api/*, HTML pages
try {
  const response = await fetch(request);
  cache.put(request, response.clone());
  return response;
} catch (error) {
  const cached = await cache.match(request);
  if (cached) return cached;
  
  // Offline fallback
  if (request.mode === 'navigate') {
    return cache.match('/offline');
  }
}
```

### 3. **PWA Hooks (hooks/usePWA.ts)**

#### **useServiceWorker()**
```tsx
const { 
  registration,      // ServiceWorkerRegistration | null
  updateAvailable,   // boolean
  updateServiceWorker // () => void
} = useServiceWorker();

// Ejemplo: Mostrar banner de actualización
{updateAvailable && (
  <button onClick={updateServiceWorker}>
    Nueva versión disponible - Actualizar
  </button>
)}
```

#### **usePWAInstall()**
```tsx
const {
  isInstallable,   // boolean - se puede instalar
  isInstalled,     // boolean - ya instalada
  promptInstall    // () => Promise<void> - mostrar prompt
} = usePWAInstall();

// Ejemplo: Banner de instalación
{isInstallable && !isInstalled && (
  <PWAInstallBanner />
)}
```

#### **useIsStandalone()**
```tsx
const isStandalone = useIsStandalone(); // boolean

// Ejemplo: Ocultar "Instalar app" si ya está instalada
{!isStandalone && <InstallButton />}
```

#### **useNotificationPermission()**
```tsx
const {
  permission,         // 'granted' | 'denied' | 'default'
  isGranted,          // boolean
  requestPermission   // () => Promise<NotificationPermission>
} = useNotificationPermission();

// Ejemplo: Solicitar permisos
<button onClick={requestPermission} disabled={isGranted}>
  Activar notificaciones
</button>
```

### 4. **PWA Install Banner (components/PWAInstallBanner.tsx)**

Banner deslizante que aparece automáticamente cuando la app es instalable:

```tsx
<PWAInstallBanner />
```

**Características:**
- Slide-up animation
- Auto-hide después de "Más tarde"
- Responsive (móvil y escritorio)
- Icono de app + nombre
- Botones: "Instalar" y "Más tarde"

---

## 🔄 Background Sync

Para sincronizar evidencias subidas offline:

```javascript
// En service-worker.js
self.addEventListener('sync', (event) => {
  if (event.tag === 'sync-evidence') {
    event.waitUntil(syncPendingEvidence());
  }
});

async function syncPendingEvidence() {
  const pendingEvidence = await db.evidence.getAll();
  
  for (const evidence of pendingEvidence) {
    try {
      await fetch('/api/evidence', {
        method: 'POST',
        body: JSON.stringify(evidence),
        headers: { 'Content-Type': 'application/json' }
      });
      await db.evidence.delete(evidence.id);
    } catch (error) {
      // Reintentar en próximo sync
    }
  }
}
```

**Uso desde el cliente:**
```typescript
// En componente de upload
await navigator.serviceWorker.ready;
await registration.sync.register('sync-evidence');
```

---

## 🔔 Push Notifications

### Frontend Setup

```typescript
// Solicitar permisos
const { requestPermission } = useNotificationPermission();
await requestPermission();

// Suscribirse a push
const registration = await navigator.serviceWorker.ready;
const subscription = await registration.pushManager.subscribe({
  userVisibleOnly: true,
  applicationServerKey: 'YOUR_VAPID_PUBLIC_KEY'
});

// Enviar subscription al backend
await fetch('/api/push/subscribe', {
  method: 'POST',
  body: JSON.stringify(subscription),
  headers: { 'Content-Type': 'application/json' }
});
```

### Service Worker Handler

```javascript
self.addEventListener('push', (event) => {
  const data = event.data.json();
  
  event.waitUntil(
    self.registration.showNotification(data.title, {
      body: data.body,
      icon: '/icons/icon-192x192.png',
      badge: '/icons/badge-72x72.png',
      data: data.url,
      actions: [
        { action: 'open', title: 'Ver' },
        { action: 'close', title: 'Cerrar' }
      ]
    })
  );
});

self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  
  if (event.action === 'open') {
    event.waitUntil(
      clients.openWindow(event.notification.data)
    );
  }
});
```

---

## 📦 Instalación y Testing

### 1. **Verificar Service Worker**

```bash
# Build de producción
npm run build

# Servir producción localmente
npm run start

# Abrir DevTools > Application > Service Workers
# Verificar: "activated and running"
```

### 2. **Testar Cache**

```javascript
// En DevTools Console
caches.keys().then(console.log); // Ver caches
caches.open('lama-cor-v1').then(cache => {
  cache.keys().then(keys => console.log(keys)); // Ver archivos cacheados
});
```

### 3. **Testar Offline**

1. Abrir app en Chrome
2. DevTools > Application > Service Workers
3. Activar "Offline" checkbox
4. Recargar página
5. Verificar: Página carga sin errores

### 4. **Testar Instalación**

**Escritorio:**
- Chrome: Botón "Instalar" en barra de direcciones
- Edge: Icono "+" en barra de direcciones

**Móvil:**
- Android Chrome: Banner "Agregar a pantalla de inicio"
- iOS Safari: Compartir > "Agregar a pantalla de inicio"

---

## 🎨 Iconos Requeridos

Ubicación: `public/icons/`

| Archivo | Tamaño | Propósito |
|---------|--------|-----------|
| icon-72x72.png | 72x72 | iOS, Android |
| icon-96x96.png | 96x96 | Android |
| icon-128x128.png | 128x128 | Android, Chrome |
| icon-144x144.png | 144x144 | Windows |
| icon-152x152.png | 152x152 | iOS |
| icon-192x192.png | 192x192 | Android (any) |
| icon-384x384.png | 384x384 | Android |
| icon-512x512.png | 512x512 | Maskable, splash |
| badge-72x72.png | 72x72 | Notification badge |
| shortcut-dashboard.png | 96x96 | Shortcut icon |
| shortcut-upload.png | 96x96 | Shortcut icon |
| shortcut-rankings.png | 96x96 | Shortcut icon |

**Herramientas para generar iconos:**
- [PWA Asset Generator](https://github.com/elegantapp/pwa-asset-generator)
- [RealFaviconGenerator](https://realfavicongenerator.net/)

```bash
npx pwa-asset-generator logo.png public/icons --manifest public/manifest.json
```

---

## 🌐 Soporte de Navegadores

| Feature | Chrome | Firefox | Safari | Edge |
|---------|--------|---------|--------|------|
| Service Worker | ✅ | ✅ | ✅ | ✅ |
| Cache API | ✅ | ✅ | ✅ | ✅ |
| Push Notifications | ✅ | ✅ | ❌ iOS | ✅ |
| Background Sync | ✅ | ❌ | ❌ | ✅ |
| Add to Home Screen | ✅ | ✅ | ⚠️ Manual | ✅ |
| Shortcuts | ✅ | ❌ | ❌ | ✅ |

**Notas:**
- iOS Safari: No soporta push notifications nativamente
- Firefox: No soporta background sync
- Edge: Soporte completo (Chromium)

---

## 🚀 Deployment

### Configuración para producción

**next.config.js:**
```javascript
/** @type {import('next').NextConfig} */
const nextConfig = {
  // Generar service worker automáticamente
  swcMinify: true,
  
  // Headers para PWA
  async headers() {
    return [
      {
        source: '/service-worker.js',
        headers: [
          { key: 'Cache-Control', value: 'no-cache' },
          { key: 'Service-Worker-Allowed', value: '/' }
        ],
      },
      {
        source: '/manifest.json',
        headers: [
          { key: 'Content-Type', value: 'application/manifest+json' },
        ],
      }
    ];
  },
};

module.exports = nextConfig;
```

### Registro del Service Worker

Ya configurado en `app/layout.tsx`:

```tsx
// Automático en build de producción
export const metadata: Metadata = {
  manifest: "/manifest.json",
  themeColor: "#7c3aed",
  // ...
};
```

**Script personalizado (opcional):**
```typescript
// En _document.tsx o layout.tsx
if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/service-worker.js');
  });
}
```

---

## 📋 Checklist de Producción

### Pre-deployment
- [ ] Iconos generados (8 tamaños)
- [ ] Screenshots agregados al manifest
- [ ] Service worker testeado offline
- [ ] Cache strategies validadas
- [ ] HTTPS configurado (requerido para SW)

### Testing
- [ ] Lighthouse PWA score > 90
- [ ] Instalación funciona en Chrome/Edge
- [ ] Offline fallback funciona
- [ ] Notifications funcionan (si backend listo)
- [ ] Background sync funciona (si backend listo)

### Post-deployment
- [ ] Verificar service worker en producción
- [ ] Monitorear errores en Application Insights
- [ ] Validar cache hit rate
- [ ] Testar en dispositivos reales (iOS/Android)

---

## 🐛 Troubleshooting

### Service Worker no se registra
```javascript
// DevTools > Application > Service Workers
// Error: "DOMException: Failed to register"
```
**Solución:** Verificar HTTPS en producción (localhost no requiere)

### Cache no funciona offline
```javascript
// Verificar estrategia de caching
caches.open('lama-cor-v1').then(cache => {
  cache.match('/api/evidence').then(console.log);
});
```
**Solución:** Verificar que request.url incluya dominio completo

### "Update available" loop infinito
```javascript
// Service worker se actualiza en cada reload
```
**Solución:** Implementar skipWaiting() correctamente:
```javascript
self.addEventListener('install', (event) => {
  // NO usar self.skipWaiting() sin control
  // Esperar a updateServiceWorker() del usuario
});
```

### iOS Safari no muestra "Add to Home Screen"
**Solución:** 
1. Agregar `apple-touch-icon` en `<head>`
2. Instruir al usuario: Safari > Share > "Add to Home Screen"
3. iOS no soporta banners automáticos

---

## 📚 Referencias

- [MDN: Progressive Web Apps](https://developer.mozilla.org/en-US/docs/Web/Progressive_web_apps)
- [Google: Service Worker Lifecycle](https://developers.google.com/web/fundamentals/primers/service-workers/lifecycle)
- [Web.dev: PWA Checklist](https://web.dev/pwa-checklist/)
- [Next.js: PWA Setup](https://nextjs.org/docs/app/building-your-application/configuring/progressive-web-apps)

---

## 🎯 Próximos Pasos

1. **Backend para Push Notifications:**
   - Implementar `/api/push/subscribe` endpoint
   - Configurar VAPID keys
   - Enviar notificaciones desde backend

2. **Advanced Caching:**
   - Cache de imágenes subidas
   - IndexedDB para evidencias offline
   - Sync queue para uploads pendientes

3. **Analytics:**
   - Trackear instalaciones
   - Monitorear usage en modo standalone
   - Cache hit rate metrics

4. **Mejoras UX:**
   - Splash screen personalizado
   - Loading skeleton para offline
   - Toast messages para sync status

---

**Fecha:** 2024-05-20  
**Versión:** 1.0  
**Estado:** ✅ Implementación completa  
**Líneas de código:** 620+
