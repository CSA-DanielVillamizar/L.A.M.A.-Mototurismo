/**
 * DOCUMENTACIÓN DE ARQUITECTURA - L.A.M.A. Mototurismo Frontend v2.0
 * 
 * Principios y patrones implementados en la capa de presentación
 */

// =============================================================================
// 1. ARQUITECTURA GENERAL
// =============================================================================

/*
  Clean Architecture - Separación de capas:

  ┌─────────────────────────────────────────────────────────────┐
  │ PRESENTACIÓN (UI Components)                                │
  │ - Componentes React reutilizables                           │
  │ - Gestión de estado local (useState)                        │
  │ - Styles con Tailwind CSS                                   │
  └─────────────────────────────────────────────────────────────┘
              ↓
  ┌─────────────────────────────────────────────────────────────┐
  │ LÓGICA DE NEGOCIO (Hooks & Utils)                           │
  │ - Lógica de formateo de datos                               │
  │ - Validaciones                                              │
  │ - Utilidades compartidas (cn, etc.)                         │
  └─────────────────────────────────────────────────────────────┘
              ↓
  ┌─────────────────────────────────────────────────────────────┐
  │ CAPA DE DATOS (API Services)                                │
  │ - Llamadas HTTP al backend                                  │
  │ - Manejo de errores                                         │
  │ - Cacheo de datos                                           │
  └─────────────────────────────────────────────────────────────┘
*/

// =============================================================================
// 2. ESTRUCTURA DE CARPETAS
// =============================================================================

/*
src/Lama.Web/
├── app/                          # Next.js App Router
│   ├── page.tsx                  # Landing/Home
│   ├── layout.tsx                # Layout raíz
│   ├── globals.css               # Estilos globales
│   │
│   ├── admin/                    # Sección administrativa
│   │   ├── cor/                  # Sistema COR (Corte)
│   │   │   └── page.tsx          # Wizard de 6 pasos
│   │   └── queue/                # Cola de procesamiento
│   │
│   ├── evidence/                 # Carga de evidencias
│   │   └── upload/               # Subida de archivos
│   │
│   ├── member/                   # Portal de miembros
│   │   ├── page.tsx              # Landing del portal
│   │   ├── layout.tsx            # Layout del portal
│   │   ├── dashboard/            # Dashboard con stats
│   │   ├── ranking/              # Tabla de ranking
│   │   │   └── detail/           # Detalle de miembro
│   │   ├── championship/         # Historial de campeonatos
│   │   ├── evidences/            # Galería de evidencias
│   │   └── profile/              # Perfil de usuario
│   │
│   └── sponsors/                 # Catálogo de patrocinadores
│       └── page.tsx
│
├── components/
│   ├── ui/                       # Componentes base (primitivos)
│   │   ├── card.tsx              # Card container
│   │   ├── badge.tsx             # Badge / etiqueta
│   │   ├── button.tsx            # Button
│   │   ├── dialog.tsx            # Modal dialog
│   │   ├── dropdown-menu.tsx     # Dropdown menu
│   │   ├── select.tsx            # Select field
│   │   ├── input.tsx             # Input text
│   │   ├── skeleton.tsx          # Loading skeleton
│   │   ├── empty-state.tsx       # Empty state UI
│   │   └── alert.tsx             # Alert banner
│   │
│   ├── layout/                   # Componentes de layout
│   │   ├── app-shell.tsx         # Shell SaaS
│   │   ├── sidebar.tsx           # Sidebar colapsable
│   │   ├── topbar.tsx            # Top navigation bar
│   │   ├── breadcrumbs.tsx       # Breadcrumb navigation
│   │   └── layout-wrapper.tsx    # Wrapper genérico
│   │
│   ├── icons.ts                  # Centralizador de iconos (lucide-react)
│   │
│   ├── Member*.tsx               # Portal de miembros
│   │   ├── MemberDashboard.tsx
│   │   ├── MemberRanking.tsx
│   │   ├── MemberEvidences.tsx
│   │   └── MemberProfile.tsx
│   │
│   ├── RankingDetail.tsx         # Detalle de ranking
│   ├── Championship.tsx          # Historial de campeonatos
│   ├── Sponsors.tsx              # Catálogo de sponsors
│   │
│   └── Wizard*.tsx               # Wizard COR
│       ├── CORWizard.tsx
│       ├── CORWizardStep1.tsx
│       ├── CORWizardStep2.tsx
│       └── ... (6 pasos totales)
│
├── lib/
│   └── utils.ts                  # Utilidades (cn utility, etc.)
│
├── styles/
│   └── globals.css               # Estilos globales, tokens CSS
│
├── public/                       # Assets estáticos
│   └── images/
│
├── next.config.js                # Configuración Next.js
├── tailwind.config.ts            # Configuración Tailwind
├── tsconfig.json                 # Configuración TypeScript
├── package.json                  # Dependencias npm
└── README.md                     # Documentación principal
*/

// =============================================================================
// 3. COMPONENTES BASE (UI PRIMITIVOS)
// =============================================================================

/*
Componentes reutilizables construidos sobre shadcn/ui + Radix UI

Ejemplos de uso:

  import { Card } from '@/components/ui/card';
  import { Badge } from '@/components/ui/badge';
  import { Button } from '@/components/ui/button';

  <Card className="p-6">
    <h2 className="text-lg font-bold">Título</h2>
    <p className="text-neutral-600">Descripción</p>
    <div className="mt-4 flex gap-2">
      <Badge variant="success">Activo</Badge>
      <Button>Acción</Button>
    </div>
  </Card>

Patrones:
- Componentes funcionales con TypeScript
- Props interfaces documentadas con JSDoc
- Clases con Tailwind CSS
- cn() utility para conditional classes
- Variantes usando class-variance-authority
*/

// =============================================================================
// 4. COMPONENTES DE CARACTERÍSTICA (FEATURE COMPONENTS)
// =============================================================================

/*
Componentes compuestos que combinan primitivos UI + lógica específica

Estructura típica:

  'use client';  // Componente cliente (usa useState, useEffect)

  import React, { useState, useEffect } from 'react';
  import { LayoutWrapper } from '@/components/layout';
  import { Card } from '@/components/ui/card';
  import { Skeleton } from '@/components/ui/skeleton';

  interface DataType {
    id: string;
    name: string;
    // ...
  }

  export function MemberDashboard() {
    const [isLoading, setIsLoading] = useState(true);
    const [data, setData] = useState<DataType[]>([]);

    useEffect(() => {
      // Simular carga de API
      const timer = setTimeout(() => {
        setData(mockData);
        setIsLoading(false);
      }, 800);
      return () => clearTimeout(timer);
    }, []);

    return (
      <LayoutWrapper title="Dashboard" breadcrumbs={[...]}>
        {isLoading ? (
          <Skeleton className="h-32" />
        ) : (
          <div className="space-y-6">
            {/* Contenido */}
          </div>
        )}
      </LayoutWrapper>
    );
  }

Patrones implementados:
- 'use client' para interactividad
- Mock data con setTimeout(800ms) simula latencia API
- Estados de carga con Skeleton
- LayoutWrapper para consistencia
- Responsive grids con Tailwind (md: lg: breakpoints)
- Empty states para no-data scenarios
*/

// =============================================================================
// 5. DESIGN SYSTEM
// =============================================================================

/*
Sistema de tokens CSS en globals.css

Colores Primarios:
- primary-50 a primary-950 (Violeta vibrante)
- secondary-50 a secondary-950 (Verde esmeralda)

Estados:
- success: Verde (#10b981)
- warning: Ámbar (#f59e0b)
- danger: Rojo (#ef4444)
- neutral: Grises (900-50)

Tipografía:
- h1: text-3xl font-bold (32px)
- h2: text-2xl font-bold (24px)
- h3: text-lg font-semibold (20px)
- body: text-base font-normal (16px)
- small: text-sm font-normal (14px)

Espaciado (múltiplos de 4px):
- p-0, p-2, p-4, p-6, p-8 (padding)
- m-0, m-2, m-4, m-6, m-8 (margin)
- gap-2, gap-4, gap-6 (gaps)
- space-y-2, space-y-4, space-y-6 (vertical space)

Sombras:
- shadow: sutil
- shadow-lg: más prominente
- shadow-xl: muy prominente

Esquinas redondeadas:
- rounded: 4px
- rounded-lg: 8px
- rounded-full: 9999px
*/

// =============================================================================
// 6. ICONOS - SISTEMA CENTRALIZADO
// =============================================================================

/*
Todos los iconos se importan desde components/icons.ts

Ventaja: cambiar iconos globalmente es trivial

Uso correcto:

  import { IconChart, IconUser, IconUpload } from '@/components/icons';

  <IconChart className="h-6 w-6" />

Los iconos están mapeados desde lucide-react con nombres semánticos:
- IconChart → TrendingUp
- IconUser → User
- IconUpload → Upload
- IconTrophy → (usar alternativa disponible)
- etc.

Convención de nombres:
Icon + [Concepto] + [Opcional: Estado]
- IconUser
- IconChevronUp
- IconCheckmark
- IconDelete
- IconSuccess
*/

// =============================================================================
// 7. PATRONES DE ESTADO Y DATOS
// =============================================================================

/*
Mock Data Pattern (usado en desarrollo):

  useEffect(() => {
    const timer = setTimeout(() => {
      setData(mockData);    // Datos simulados
      setIsLoading(false);
    }, 800);                // 800ms = latencia API
    return () => clearTimeout(timer);
  }, []);

Cuando se integre API real, solo cambiar:

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch('/api/endpoint');
        const json = await response.json();
        setData(json);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

Loading States:
- isLoading boolean controla Skeleton vs Content
- Skeleton mantiene layout estable (no CLS)
- Tiempos: 800ms para fetch, 200ms para UI transitions

Error Handling:
- Usar try/catch en useEffect
- Mostrar Alert o EmptyState en caso de error
- Logging de errores para debugging
*/

// =============================================================================
// 8. CONVENCIONES DE CÓDIGO
// =============================================================================

/*
TypeScript:
  interface MemberData {
    id: string;
    name: string;
    points: number;
    isActive: boolean;
  }

  const member: MemberData = { ... };

Componentes:
  export function MemberDashboard() { }  // PascalCase, export
  function StatsCard() { }               // Subcomponentes sin export

Variables:
  const isLoading = true;       // bool: is/has prefix
  const userData = { ... };     // camelCase
  const CONSTANT = 100;         // SCREAMING_CASE para const

Funciones:
  const handleClick = () => {};
  const fetchData = async () => {};
  const formatDate = (date: Date) => '';

Estilos:
  className={cn(
    'base styles',
    condition && 'conditional styles',
    variant === 'filled' && 'variant styles'
  )}

Comentarios:
  // Breve descripción en línea
  
  /**
   * Función descripción
   * @param param - Descripción del parámetro
   * @returns Descripción del retorno
   */

Imports:
  - Absolute imports: import { Component } from '@/components/...'
  - Agrupar: React → Components → Utils → Styles
*/

// =============================================================================
// 9. RESPONSIVE DESIGN
// =============================================================================

/*
Breakpoints de Tailwind:
  base     320px
  sm       640px
  md       768px  ← Tablets
  lg       1024px ← Desktops
  xl       1280px
  2xl      1536px

Uso:

  <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3">
    {/* 1 columna en mobile, 2 en tablet, 3 en desktop */}
  </div>

  <div className="text-sm md:text-base lg:text-lg">
    {/* Texto responsivo */}
  </div>

  <div className="hidden md:block">
    {/* Solo visible en tablet y desktop */}
  </div>

Testing responsivo:
  - DevTools: F12 → Toggle Device Toolbar
  - Simular: iPhone 12, iPad, Desktop (1920px)
  - Verificar: No overflow, texto legible, botones clickeables
*/

// =============================================================================
// 10. PERFORMANCE OPTIMIZATIONS
// =============================================================================

/*
Next.js Optimizaciones:

1. Image Optimization:
   - Usar next/image para imágenes dinámicas
   - Especificar width/height para evitar layout shift
   - Lazy load por defecto

2. Code Splitting:
   - Next.js splitea automáticamente por ruta
   - Chunks de ~110KB gzipped

3. Caching:
   - Static pages prerendered en build
   - Browser cache: 1 año para assets, 1 mes para API

4. Bundle Optimization:
   - Importar solo lo necesario
   - Evitar default exports en librerías grandes
   - Tree-shaking automático

Metricas:
  - FCP (First Contentful Paint): < 1.8s
  - LCP (Largest Contentful Paint): < 2.5s
  - CLS (Cumulative Layout Shift): < 0.1
*/

// =============================================================================
// 11. SEGURIDAD & BEST PRACTICES
// =============================================================================

/*
Frontend Security:
  - Escape de contenido: React lo hace automático
  - XSS prevention: No usar dangerouslySetInnerHTML
  - CSRF: Depende del backend (tokens)
  - CSP: Configurar en headers de Next.js

Environmental Variables:
  - NEXT_PUBLIC_* → Visible en cliente
  - Sin prefix → Solo servidor (NO usar en componentes)

Validaciones:
  - Validar entrada de usuario antes de API
  - Usar tipos TypeScript para seguridad
  - Sanitizar datos antes de mostrar en DOM

Logs:
  - Usar console.log solo en desarrollo
  - Usar console.error para errores reales
  - NO loguear info sensible (tokens, passwords)
*/

// =============================================================================
// 12. PRÓXIMOS PASOS - INTEGRACIÓN CON BACKEND
// =============================================================================

/*
Para conectar con API Backend:

1. Crear API service:

  // lib/api.ts
  export async function fetchMembers() {
    const response = await fetch(
      `${process.env.NEXT_PUBLIC_API_BASE_URL}/api/members`
    );
    if (!response.ok) throw new Error('Failed to fetch');
    return response.json();
  }

2. Usar en componentes:

  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await fetchMembers();
        setMembers(data);
      } catch (error) {
        console.error(error);
        setError('No se pudieron cargar los datos');
      } finally {
        setIsLoading(false);
      }
    };
    loadData();
  }, []);

3. Manejo de errores:

  {error && (
    <Alert variant="danger">
      {error} - {' '}
      <button onClick={() => window.location.reload()}>
        Reintentar
      </button>
    </Alert>
  )}

4. Estados de caché:

  Para mejorar UX, implementar:
  - React Query / SWR para caching
  - Revalidación de datos
  - Optimistic updates
*/

// =============================================================================
// REFERENCIAS Y RECURSOS
// =============================================================================

/*
Documentación:
  - Next.js: https://nextjs.org/docs
  - React: https://react.dev
  - TypeScript: https://www.typescriptlang.org/docs
  - Tailwind CSS: https://tailwindcss.com/docs
  - Radix UI: https://www.radix-ui.com/docs
  - Lucide Icons: https://lucide.dev

Herramientas:
  - VS Code: Editor recomendado
  - ESLint: Linter de código
  - Prettier: Formateador de código
  - Git: Control de versiones

Mejores Prácticas:
  - Atomic Design: Componentes pequeños y reutilizables
  - DRY (Don't Repeat Yourself): Evitar duplicación
  - SOLID Principles: Código limpio y mantenible
  - Clean Code: Nombres claros, funciones pequeñas
*/

export {}; // Para que TypeScript considere esto como módulo
