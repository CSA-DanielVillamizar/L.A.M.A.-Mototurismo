# PWA Icons - Placeholder

## 📋 Iconos Requeridos

Los siguientes iconos deben ser generados a partir del logo de LAMA COR:

### Iconos principales
- `icon-72x72.png` (72x72)
- `icon-96x96.png` (96x96)
- `icon-128x128.png` (128x128)
- `icon-144x144.png` (144x144)
- `icon-152x152.png` (152x152)
- `icon-192x192.png` (192x192) - Purpose: any
- `icon-384x384.png` (384x384)
- `icon-512x512.png` (512x512) - Purpose: maskable

### Iconos adicionales
- `badge-72x72.png` (72x72) - Badge para notificaciones
- `shortcut-dashboard.png` (96x96) - Icono para shortcut Dashboard
- `shortcut-upload.png` (96x96) - Icono para shortcut Upload
- `shortcut-rankings.png` (96x96) - Icono para shortcut Rankings

---

## 🛠️ Generar Iconos

### Opción 1: PWA Asset Generator (Recomendado)
```bash
npx pwa-asset-generator logo.png public/icons --manifest public/manifest.json
```

### Opción 2: RealFaviconGenerator
1. Visitar: https://realfavicongenerator.net/
2. Subir logo de alta resolución (al menos 512x512)
3. Configurar opciones para Android/iOS
4. Descargar y extraer en `public/icons/`

### Opción 3: Manual con Photoshop/GIMP
1. Abrir logo en resolución máxima
2. Redimensionar a cada tamaño listado
3. Exportar como PNG con transparencia
4. Guardar en `public/icons/`

---

## 🎨 Especificaciones del Logo

Para mejores resultados:

- **Formato:** PNG con transparencia
- **Resolución mínima:** 512x512 px
- **Colores:** 
  - Principal: `#7c3aed` (Violeta)
  - Secundario: `#f59e0b` (Ámbar)
  - Fondo: Transparente o `#0f172a` (Slate 900)
- **Estilo:** Icono simple, sin texto extenso
- **Padding:** 10% alrededor del icono para maskable

---

## 📁 Estructura Esperada

```
public/icons/
├── icon-72x72.png
├── icon-96x96.png
├── icon-128x128.png
├── icon-144x144.png
├── icon-152x152.png
├── icon-192x192.png
├── icon-384x384.png
├── icon-512x512.png
├── badge-72x72.png
├── shortcut-dashboard.png
├── shortcut-upload.png
└── shortcut-rankings.png
```

---

## ⚠️ Nota

Actualmente se están usando placeholders. Reemplazar con los iconos oficiales de LAMA COR antes de deployment.
