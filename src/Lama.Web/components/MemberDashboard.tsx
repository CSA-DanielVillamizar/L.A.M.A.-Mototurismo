'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import {
  IconChart,
  IconFavorite,
  IconUpload,
  IconUser,
} from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Tarjeta de estadística
 */
interface StatsCardProps {
  icon: React.ReactNode;
  label: string;
  value: string | number;
  trend?: {
    value: number;
    direction: 'up' | 'down';
  };
  className?: string;
}

function StatsCard({ icon, label, value, trend, className }: StatsCardProps) {
  return (
    <Card className={cn('p-6', className)}>
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-neutral-600">{label}</p>
          <p className="mt-2 text-3xl font-bold text-neutral-900">{value}</p>
          {trend && (
            <p
              className={cn(
                'mt-2 text-xs font-medium',
                trend.direction === 'up'
                  ? 'text-success-600'
                  : 'text-danger-600'
              )}
            >
              {trend.direction === 'up' ? '↑' : '↓'} {Math.abs(
                trend.value
              )}% vs mes anterior
            </p>
          )}
        </div>
        <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary-100">
          {icon}
        </div>
      </div>
    </Card>
  );
}

/**
 * Dashboard principal del miembro
 */
export function MemberDashboard() {
  const [isLoading, setIsLoading] = useState(true);
  const [stats, setStats] = useState({
    totalPoints: 0,
    ranking: 0,
    evidences: 0,
    nextEvent: null as any,
  });

  useEffect(() => {
    // Simular carga de datos (aquí iría la llamada a la API)
    const timer = setTimeout(() => {
      setStats({
        totalPoints: 2450,
        ranking: 12,
        evidences: 8,
        nextEvent: {
          name: 'Mototurismo Nacional 2026',
          date: '2026-02-15',
          distance: 1200,
        },
      });
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  return (
    <LayoutWrapper
      title="Mi Portal"
      breadcrumbs={[
        { label: 'Mi Perfil', href: '/member' },
        { label: 'Dashboard', href: '/member/dashboard' },
      ]}
    >
      <div className="space-y-8">
        {/* Sección de Bienvenida */}
        <div>
          <h1 className="text-4xl font-bold text-neutral-900">
            Bienvenido de nuevo
          </h1>
          <p className="mt-2 text-lg text-neutral-600">
            Aquí puedes ver tu progreso y estadísticas de mototurismo
          </p>
        </div>

        {/* Grid de Estadísticas */}
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
          {isLoading ? (
            <>
              {[1, 2, 3, 4].map((i) => (
                <Skeleton key={i} className="h-48" />
              ))}
            </>
          ) : (
            <>
              <StatsCard
                icon={<IconChart className="h-6 w-6 text-primary-600" />}
                label="Puntos Totales"
                value={stats.totalPoints}
                trend={{ value: 12, direction: 'up' }}
              />
              <StatsCard
                icon={<IconFavorite className="h-6 w-6 text-warning-600" />}
                label="Ranking Nacional"
                value={`#${stats.ranking}`}
                trend={{ value: 3, direction: 'down' }}
              />
              <StatsCard
                icon={<IconUpload className="h-6 w-6 text-success-600" />}
                label="Evidencias Registradas"
                value={stats.evidences}
                trend={{ value: 5, direction: 'up' }}
              />
              <StatsCard
                icon={<IconUser className="h-6 w-6 text-secondary-600" />}
                label="Clase de Visitante"
                value="Premium"
              />
            </>
          )}
        </div>

        {/* Próximo Evento */}
        {stats.nextEvent && (
          <Card className="border-primary-200 bg-primary-50 p-6">
            <h2 className="text-lg font-semibold text-neutral-900">
              Próximo Evento
            </h2>
            <div className="mt-4 space-y-2">
              <p className="text-base font-medium text-neutral-900">
                {stats.nextEvent.name}
              </p>
              <p className="text-sm text-neutral-600">
                📅 {new Date(stats.nextEvent.date).toLocaleDateString('es-ES', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
                })}
              </p>
              <p className="text-sm text-neutral-600">
                📍 {stats.nextEvent.distance} km
              </p>
            </div>
            <button className="mt-4 rounded-lg bg-primary-600 px-4 py-2 text-sm font-medium text-white hover:bg-primary-700 transition-colors">
              Ver Detalles
            </button>
          </Card>
        )}

        {/* Acciones Rápidas */}
        <div className="grid gap-4 md:grid-cols-2">
          <Card className="p-6">
            <h3 className="text-lg font-semibold text-neutral-900">
              Registrar Evidencia
            </h3>
            <p className="mt-2 text-sm text-neutral-600">
              Sube fotos de tu participación en eventos
            </p>
            <a
              href="/admin/cor"
              className="mt-4 inline-block rounded-lg bg-primary-600 px-4 py-2 text-sm font-medium text-white hover:bg-primary-700 transition-colors"
            >
              Ir a Registro
            </a>
          </Card>
          <Card className="p-6">
            <h3 className="text-lg font-semibold text-neutral-900">
              Ver Mis Evidencias
            </h3>
            <p className="mt-2 text-sm text-neutral-600">
              Consulta tu historial de registros
            </p>
            <a
              href="/member/evidences"
              className="mt-4 inline-block rounded-lg bg-secondary-600 px-4 py-2 text-sm font-medium text-white hover:bg-secondary-700 transition-colors"
            >
              Ver Historial
            </a>
          </Card>
        </div>
      </div>
    </LayoutWrapper>
  );
}
