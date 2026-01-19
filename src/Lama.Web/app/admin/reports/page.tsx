'use client';

/**
 * Página de Reportes
 * Genera y visualiza reportes del sistema
 */

import { Suspense } from 'react';
import { Breadcrumbs } from '@/components/layout/AppShell';
import { Loading } from '@/components/ui/loading';
import { TrendingUp } from 'lucide-react';

function ReportsContent() {
  return (
    <div className="space-y-6">
      <Breadcrumbs items={[{ label: 'Admin' }, { label: 'Reportes' }]} />

      <div className="px-6">
        <div className="flex items-center gap-3 mb-6">
          <TrendingUp className="text-primary-600" size={28} />
          <h1 className="text-3xl font-bold text-primary-900">Reportes</h1>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {/* Card de reportes disponibles */}
          <div className="rounded-lg border border-neutral-200 p-6 hover:shadow-md transition-shadow cursor-pointer hover:border-primary-300">
            <div className="flex items-center gap-3 mb-2">
              <TrendingUp className="text-primary-600" size={24} />
              <h3 className="font-semibold text-primary-900">Resumen de Eventos</h3>
            </div>
            <p className="text-sm text-neutral-600">
              Estadísticas generales de eventos registrados
            </p>
          </div>

          <div className="rounded-lg border border-neutral-200 p-6 hover:shadow-md transition-shadow cursor-pointer hover:border-primary-300">
            <div className="flex items-center gap-3 mb-2">
              <TrendingUp className="text-primary-600" size={24} />
              <h3 className="font-semibold text-primary-900">Reporte de Miembros</h3>
            </div>
            <p className="text-sm text-neutral-600">
              Análisis de participación de miembros
            </p>
          </div>

          <div className="rounded-lg border border-neutral-200 p-6 hover:shadow-md transition-shadow cursor-pointer hover:border-primary-300">
            <div className="flex items-center gap-3 mb-2">
              <TrendingUp className="text-primary-600" size={24} />
              <h3 className="font-semibold text-primary-900">Validaciones COR</h3>
            </div>
            <p className="text-sm text-neutral-600">
              Historial de validaciones realizadas
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default function ReportsPage() {
  return (
    <Suspense fallback={<Loading message="Cargando página de reportes..." />}>
      <ReportsContent />
    </Suspense>
  );
}
