# 📦 Guía de Componentes - L.A.M.A. Mototurismo

Documentación de componentes reutilizables y su uso correcto.

---

## 🎨 Componentes Base (UI Primitivos)

### Card

Contenedor genérico para agrupar contenido.

```typescript
import { Card } from '@/components/ui/card';

<Card className="p-6">
  <h2>Título</h2>
  <p>Contenido</p>
</Card>
```

**Propiedades**:
- `className`: Clases Tailwind (incluir padding/margin)
- `children`: Contenido dentro de la tarjeta

**Ejemplos**:
```typescript
// Tarjeta básica
<Card className="p-6">Contenido</Card>

// Tarjeta con sombra
<Card className="p-6 shadow-lg">Contenido</Card>

// Tarjeta con fondo
<Card className="p-6 bg-primary-50">Contenido</Card>
```

---

### Badge

Etiqueta pequeña para estados, categorías o tags.

```typescript
import { Badge } from '@/components/ui/badge';

<Badge className="bg-success-100 text-success-700">Aprobado</Badge>
```

**Propiedades**:
- `className`: Incluir colores (bg-* text-*)
- `children`: Texto de la etiqueta

**Variantes de Color**:
```typescript
// Success (Verde)
<Badge className="bg-success-100 text-success-700">Aprobado</Badge>

// Warning (Ámbar)
<Badge className="bg-warning-100 text-warning-700">Pendiente</Badge>

// Danger (Rojo)
<Badge className="bg-danger-100 text-danger-700">Rechazado</Badge>

// Primary (Violeta)
<Badge className="bg-primary-100 text-primary-700">Destacado</Badge>

// Neutral
<Badge className="bg-neutral-100 text-neutral-700">Normal</Badge>
```

---

### Button

Botón interactivo con múltiples estilos.

```typescript
import { Button } from '@/components/ui/button';

<Button onClick={handleClick}>Acción</Button>
```

**Propiedades**:
- `onClick`: Función a ejecutar
- `disabled`: Desactivar botón
- `className`: Estilos personalizados
- `children`: Texto del botón

**Variantes**:
```typescript
// Primario (por defecto)
<Button>Guardar</Button>

// Secundario
<Button className="bg-neutral-100 text-neutral-900">Cancelar</Button>

// Peligroso
<Button className="bg-danger-600 hover:bg-danger-700">Eliminar</Button>

// Desactivado
<Button disabled>Cargando...</Button>

// Con ícono
<Button>
  <IconUpload className="h-4 w-4 mr-2" />
  Subir archivo
</Button>
```

---

### Skeleton

Placeholder durante carga de datos.

```typescript
import { Skeleton } from '@/components/ui/skeleton';

{isLoading ? (
  <Skeleton className="h-12 w-full" />
) : (
  <Card>Contenido</Card>
)}
```

**Propiedades**:
- `className`: Alto y ancho (h-* w-*)

**Ejemplos**:
```typescript
// Texto de una línea
<Skeleton className="h-4 w-full" />

// Párrafo (3 líneas)
<div className="space-y-2">
  <Skeleton className="h-4 w-full" />
  <Skeleton className="h-4 w-full" />
  <Skeleton className="h-4 w-3/4" />
</div>

// Card completa
<Skeleton className="h-32 w-full rounded-lg" />

// Avatar
<Skeleton className="h-12 w-12 rounded-full" />

// Grid de skeletons
<div className="grid grid-cols-3 gap-4">
  {[1, 2, 3].map(i => (
    <Skeleton key={i} className="h-32 w-full" />
  ))}
</div>
```

---

### EmptyState

Pantalla para cuando no hay datos.

```typescript
import { EmptyState } from '@/components/ui/empty-state';

{items.length === 0 ? (
  <EmptyState
    title="No hay datos"
    description="Comienza cargando tu primer item"
    action={<a href="/crear">Crear ahora</a>}
  />
) : (
  <div>Contenido</div>
)}
```

**Propiedades**:
- `title`: Título principal
- `description`: Descripción (opcional)
- `icon`: Icono LucideIcon (opcional)
- `action`: Botón/link de acción (opcional)

---

## 🏗️ Componentes de Layout

### LayoutWrapper

Envuelve contenido con título, breadcrumbs y espaciado estándar.

```typescript
import { LayoutWrapper } from '@/components/layout';

<LayoutWrapper
  title="Mi Dashboard"
  breadcrumbs={[
    { label: 'Inicio', href: '/' },
    { label: 'Dashboard', href: '/dashboard' }
  ]}
>
  {/* Contenido */}
</LayoutWrapper>
```

**Propiedades**:
- `title`: Título de la página
- `breadcrumbs`: Array de { label, href }
- `children`: Contenido principal

---

### AppShell

Shell SaaS con sidebar, topbar y contenido principal.

```typescript
import { AppShell } from '@/components/layout';

<AppShell>
  {/* Contenido */}
</AppShell>
```

---

## 📱 Componentes de Característica (Features)

### MemberDashboard

Dashboard principal del miembro con estadísticas.

```typescript
import { MemberDashboard } from '@/components/MemberDashboard';

<MemberDashboard />
```

**Características**:
- 4 tarjetas de estadísticas
- Próximo evento
- Acciones rápidas
- Loading skeleton

---

### MemberRanking

Tabla de ranking nacional.

```typescript
import { MemberRanking } from '@/components/MemberRanking';

<MemberRanking />
```

**Características**:
- 12 miembros del ranking
- Cambios de posición (↑/↓)
- Posición actual destacada
- Información de puntos

---

### MemberEvidences

Galería de evidencias con filtros.

```typescript
import { MemberEvidences } from '@/components/MemberEvidences';

<MemberEvidences />
```

**Características**:
- Filtros: All, Approved, Pending
- Estadísticas de evidencias
- Grid responsivo (2-3 columnas)
- EmptyState con CTA

---

### MemberProfile

Perfil de usuario con información completa.

```typescript
import { MemberProfile } from '@/components/MemberProfile';

<MemberProfile />
```

**Características**:
- Avatar con fondo gradiente
- Estadísticas principales
- Información de contacto
- Rutas favoritas
- Próximos pasos

---

### RankingDetail

Detalle de miembro en el ranking.

```typescript
import { RankingDetail } from '@/components/RankingDetail';

<RankingDetail />
```

**Características**:
- Perfil del miembro
- Gráfico anual de puntos
- Logros recientes
- Información de afiliación

---

### Championship

Historial de campeonatos.

```typescript
import { Championship } from '@/components/Championship';

<Championship />
```

**Características**:
- Filtros por estado
- Detalle de campeonatos
- Progreso de rondas
- Posición y puntos

---

### Sponsors

Catálogo de patrocinadores.

```typescript
import { Sponsors } from '@/components/Sponsors';

<Sponsors />
```

**Características**:
- Filtros por categoría
- Patrocinadores destacados
- Descuentos y beneficios
- Links a sitios web

---

## 🎨 Utilidades y Helpers

### cn() - Conditional Classnames

Combina clases Tailwind condicionalmente.

```typescript
import { cn } from '@/lib/utils';

<div className={cn(
  'base-styles',
  isActive && 'active-styles',
  variant === 'filled' && 'filled-styles'
)}>
  Contenido
</div>
```

**Ejemplos**:
```typescript
// Botón con estados
<Button className={cn(
  'px-4 py-2 rounded',
  isLoading && 'opacity-50 cursor-not-allowed',
  disabled && 'bg-neutral-300'
)}>
  {isLoading ? 'Cargando...' : 'Guardar'}
</Button>

// Tarjeta con variante
<Card className={cn(
  'p-6',
  variant === 'highlighted' && 'border-2 border-primary-600 bg-primary-50'
)}>
  Contenido
</Card>
```

---

## 🎯 Patrones Comunes

### Formulario Controlado

```typescript
import { useState } from 'react';
import { Button } from '@/components/ui/button';

export function MyForm() {
  const [formData, setFormData] = useState({
    name: '',
    email: ''
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log(formData);
    // Enviar al backend
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <input
        type="text"
        name="name"
        value={formData.name}
        onChange={handleChange}
        className="w-full px-3 py-2 border rounded"
        placeholder="Nombre"
      />
      <input
        type="email"
        name="email"
        value={formData.email}
        onChange={handleChange}
        className="w-full px-3 py-2 border rounded"
        placeholder="Email"
      />
      <Button type="submit">Enviar</Button>
    </form>
  );
}
```

### Datos Asincronos con Loading

```typescript
import { useEffect, useState } from 'react';
import { Skeleton } from '@/components/ui/skeleton';
import { Card } from '@/components/ui/card';

export function DataLoader() {
  const [isLoading, setIsLoading] = useState(true);
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadData = async () => {
      try {
        const response = await fetch('/api/data');
        if (!response.ok) throw new Error('Failed to fetch');
        const json = await response.json();
        setData(json);
      } catch (err) {
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, []);

  if (isLoading) return <Skeleton className="h-32 w-full" />;
  if (error) return <div className="text-danger-600">Error: {error}</div>;
  if (!data) return <div>Sin datos</div>;

  return <Card className="p-6">{/* Renderizar data */}</Card>;
}
```

### Grid Responsivo

```typescript
<div className="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
  {items.map(item => (
    <Card key={item.id} className="p-4">
      {/* Item content */}
    </Card>
  ))}
</div>
```

---

## 📋 Checklist para Componentes Nuevos

- [ ] Crear archivo con nombre PascalCase
- [ ] Agregar `'use client'` si usa interactividad
- [ ] Documentar con JSDoc
- [ ] Crear interface de props con TypeScript
- [ ] Usar `cn()` para clases condicionales
- [ ] Importar desde paths absolutos (`@/...`)
- [ ] Implementar loading states si aplica
- [ ] Hacer responsive con Tailwind
- [ ] Testar en diferentes breakpoints
- [ ] Agregar al archivo de exports si necesario

---

**Última actualización**: Enero 15, 2026
