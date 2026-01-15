# 🧪 Testing & QA - L.A.M.A. Mototurismo

Guía de testing, validaciones y checklist para asegurar calidad en producción.

---

## ✅ Testing Responsivo

### Breakpoints a Validar

```
Mobile:     320px - 639px  (iPhone SE, 5S, XR)
Tablet:     640px - 1023px (iPad, iPad Mini)
Desktop:    1024px+        (Desktop, Laptop)
Large:      1920px+        (4K Monitors)
```

### Herramientas de Testing

**VS Code DevTools**:
1. Abrir DevTools: `F12`
2. Click en icono "Toggle Device Toolbar" (Ctrl+Shift+M)
3. Seleccionar dispositivos predefinidos
4. Probar en landscape/portrait

**Dispositivos a Probar**:
- iPhone 12 (390x844)
- iPhone SE (375x667)
- iPad (768x1024)
- iPad Pro (1024x1366)
- Desktop 1280px
- Desktop 1920px

### Checklist de Responsive

**Mobile (320px)**:
- [ ] No hay overflow horizontal
- [ ] Botones clickeables (min 44x44px)
- [ ] Texto legible sin zoom
- [ ] Imágenes se escalan correctamente
- [ ] Espaciado suficiente entre elementos
- [ ] Formularios bien espaciados

**Tablet (768px)**:
- [ ] Grids de 2 columnas funcionan
- [ ] Sidebar colapsable en lugar de fullscreen
- [ ] Botones y controles fáciles de tocar
- [ ] Tablas si existen, son horizontales

**Desktop (1024px+)**:
- [ ] Grids de 3+ columnas mostrados
- [ ] Contenido bien distribuido
- [ ] Máximo ancho respetado (1200px típico)
- [ ] Hover effects funcionan

---

## 🎯 Validaciones de Funcionalidad

### Dashboard & Navegación

```typescript
✓ Landing page carga sin errores
✓ Botón "Mi Portal" → /member/dashboard
✓ Breadcrumbs navegan correctamente
✓ Sidebar se expande/colapsa
✓ Links internos funcionan
✓ Redirecciones funcionan (/admin → /admin/cor)
```

### Componentes de Datos

```typescript
✓ Skeletons aparecen mientras carga (800ms)
✓ Datos se cargan correctamente
✓ Filtros funcionan (All/Approved/Pending)
✓ Ordenamiento ascendente/descendente
✓ Empty states se muestran cuando no hay datos
✓ Paginación (si aplica) funciona
```

### Formularios (Wizard COR)

```typescript
✓ Validación de campos obligatorios
✓ Mensajes de error claros
✓ Botón "Siguiente" desactivado sin datos
✓ Botón "Anterior" funciona en pasos 2+
✓ Progreso visual (6/6 steps) actualiza
✓ Confirmación final genera resumen
✓ Submit envía datos al backend
```

### Imágenes

```typescript
✓ Imágenes de Unsplash cargan correctamente
✓ Fallback para imágenes rotas
✓ Alt text presente en todas las imágenes
✓ Lazy loading funciona (scroll imágenes)
✓ Dimensiones correctas (no distorsionadas)
```

---

## ♿ Validaciones de Accesibilidad

### WCAG 2.1 Level AA

**Colores & Contraste**:
- [ ] Ratio de contraste ≥ 4.5:1 para texto
- [ ] Ratio de contraste ≥ 3:1 para elementos gráficos
- [ ] No solo color para transmitir información

**Teclado**:
- [ ] Todos los botones/links focusables con Tab
- [ ] Orden de Tab lógico (de arriba a abajo)
- [ ] Tecla Enter activa botones
- [ ] Escape cierra diálogos

**Semántica HTML**:
- [ ] Usar `<button>` para botones (no `<div>`)
- [ ] Usar `<a>` para links (no `<div>`)
- [ ] Headings (h1, h2, h3) en orden
- [ ] Labels asociados a inputs

**Screen Readers**:
- [ ] Alt text en todas las imágenes
- [ ] ARIA labels donde necesario
- [ ] Anuncios dinámicos (live regions)

---

## 📊 Auditoría de Performance

### Lighthouse (Chrome DevTools)

**Pasos**:
1. DevTools → Lighthouse
2. Select "Desktop" o "Mobile"
3. Click "Analyze page load"
4. Esperar resultados

**Targets Mínimos**:
```
Performance: ≥ 90
Accessibility: ≥ 90
Best Practices: ≥ 90
SEO: ≥ 90
```

**Métricas Clave**:
```
FCP (First Contentful Paint): < 1.8s
LCP (Largest Contentful Paint): < 2.5s
CLS (Cumulative Layout Shift): < 0.1
TTI (Time to Interactive): < 3.8s
```

### Bundle Size

```bash
# Analizar tamaño del bundle
npm run build

# Tamaño esperado: ~110 KB (gzipped)
# Si > 120 KB, revisar imports innecesarios
```

---

## 🔒 Validaciones de Seguridad

### Frontend Security

```typescript
✓ Sin dangerouslySetInnerHTML
✓ Inputs sanitizados antes de usar
✓ No loguear información sensible
✓ HTTPS en producción
✓ CSP headers configurados
✓ X-Frame-Options configurado
```

### Environmental Variables

```typescript
✓ NEXT_PUBLIC_* solo para valores públicos
✓ Secrets nunca en código
✓ .env.local nunca en Git
✓ .env.example tiene placeholders
✓ Variables en deploy correctamente configuradas
```

---

## 📱 Navegadores a Soportar

### Desktop
- Chrome 120+
- Firefox 121+
- Safari 17+
- Edge 120+

### Mobile
- Chrome Mobile (iOS/Android)
- Safari Mobile (iOS)
- Firefox Mobile (Android)

### Testing

```bash
# Verificar compatibilidad en Can I Use
# https://caniuse.com

Verificar:
  ✓ Flexbox
  ✓ CSS Grid
  ✓ CSS Variables
  ✓ Fetch API
  ✓ Dynamic imports
```

---

## 🎨 Validaciones Visuales

### Consistencia de Diseño

- [ ] Colores de brand usados consistentemente
- [ ] Tipografía consistente (h1, h2, h3, body)
- [ ] Espaciado sigue escala de 4px
- [ ] Esquinas redondeadas consistentes
- [ ] Sombras consistentes

### Componentes

- [ ] Cards tienen mismo padding/border
- [ ] Botones mismo tamaño/altura
- [ ] Badges mismo estilo
- [ ] Inputs mismo estilo
- [ ] Empty states consistent

### Iconos

- [ ] Todos los iconos usan sistema centralizado
- [ ] Tamaños consistentes (h-6 w-6, etc.)
- [ ] Colores coherentes con contexto

---

## 🚀 Checklist Pre-Deploy

### Código

- [ ] npm run lint sin errores
- [ ] npm run build sin errores
- [ ] npm run type-check sin errores
- [ ] No hay console.log en producción
- [ ] No hay console.error en desarrollo
- [ ] Imports utilizados (no dead code)

### Configuración

- [ ] .env.local configurado
- [ ] next.config.js optimizado
- [ ] tailwind.config.ts completo
- [ ] tsconfig.json correcto
- [ ] package.json versiones locked

### Contenido

- [ ] Metadatos en todas las páginas
- [ ] Títulos descriptivos
- [ ] Descripciones < 160 caracteres
- [ ] Open Graph tags presentes
- [ ] Favicon configurado

### Performance

- [ ] Lighthouse score ≥ 90
- [ ] Bundle size < 120 KB
- [ ] Imágenes optimizadas
- [ ] Code splitting funcionando
- [ ] Caching headers configurados

### Seguridad

- [ ] Secrets no en código
- [ ] HTTPS validado
- [ ] CSP headers configurados
- [ ] CORS correctamente
- [ ] Rate limiting en API

### Testing

- [ ] Responsive en 320px-1920px
- [ ] Mobile-first validado
- [ ] Keyboard navigation funciona
- [ ] Screen reader compatible
- [ ] Contraste de color OK
- [ ] 4-5 navegadores testeados

---

## 📋 Pruebas Manuales Esenciales

### Flujo de Usuario Principal

```
1. Abrir https://lama-mototurismo.com
   ✓ Landing carga rápido
   ✓ Imagen hero visible
   ✓ Botones clickeables

2. Click "Mi Portal"
   ✓ Redirige a /member/dashboard
   ✓ Dashboard carga
   ✓ Skeletons → Datos en 800ms

3. Click "Ranking"
   ✓ Navega a /member/ranking
   ✓ Tabla con 12 miembros
   ✓ Usuario actual (#12) destacado

4. Click "Detalle" en usuario
   ✓ Abre /member/ranking/detail
   ✓ Gráfico anual visible
   ✓ Logros listados

5. Click "Campeonatos"
   ✓ Navega a /member/championship
   ✓ Filtros funcionan
   ✓ Rondas se muestran

6. Click "Patrocinadores"
   ✓ Navega a /sponsors
   ✓ Grilla de sponsors
   ✓ Links a sitios web funcionan

7. Click "Mis Evidencias"
   ✓ Navega a /member/evidences
   ✓ Galería con imágenes
   ✓ Filtros funcionan
   ✓ EmptyState si no hay datos

8. Click "Mi Perfil"
   ✓ Navega a /member/profile
   ✓ Avatar y datos presentes
   ✓ Información contacto visible
```

---

## 🐛 Debugging Común

### Problema: Componente no renderiza

```typescript
// ❌ Olvidar 'use client' en componente interactivo
import { useState } from 'react';
export function MyComponent() { ... }

// ✅ Solución
'use client';
import { useState } from 'react';
export function MyComponent() { ... }
```

### Problema: Estilos no aplican

```typescript
// ❌ Usar clase antes de ser definida
className="my-custom-class"

// ✅ Solución: usar solo clases de Tailwind
className="p-4 bg-primary-50"

// O usar cn() para conditional
className={cn('base', condition && 'variant')}
```

### Problema: Imágenes no cargan

```typescript
// ❌ Usar URL que no está en remotePatterns
<img src="https://example.com/image.jpg" />

// ✅ Agregar dominio a next.config.js
images: {
  remotePatterns: [
    { protocol: 'https', hostname: 'example.com' }
  ]
}
```

### Problema: Layout shift

```typescript
// ❌ Sin especificar dimensiones
<img src="image.jpg" alt="test" />

// ✅ Especificar width/height
<img 
  src="image.jpg" 
  alt="test" 
  width={400} 
  height={300}
  style={{ width: '100%', height: 'auto' }}
/>
```

---

## 📞 Recursos de Testing

**Herramientas Online**:
- Google Lighthouse: https://web.dev/measure/
- WAVE Accessibility: https://wave.webaim.org/
- GTmetrix Performance: https://gtmetrix.com
- Can I Use: https://caniuse.com

**Documentación**:
- WCAG 2.1: https://www.w3.org/WAI/WCAG21/quickref/
- MDN Accessibility: https://developer.mozilla.org/en-US/docs/Web/Accessibility
- Next.js Performance: https://nextjs.org/docs/app/building-your-application/optimizing

---

**Última actualización**: Enero 15, 2026
