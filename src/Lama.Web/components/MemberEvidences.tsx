'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { EmptyState } from '@/components/ui/empty-state';
import {
  IconUpload,
  IconCalendar,
  IconFileText,
  IconCheckmark,
} from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Interfaz de evidencia
 */
interface Evidence {
  id: number;
  eventName: string;
  eventDate: string;
  location: string;
  photo: string;
  status: 'pending' | 'approved' | 'rejected';
  points: number;
  uploadDate: string;
}

/**
 * Tarjeta de evidencia
 */
function EvidenceCard({ evidence }: { evidence: Evidence }) {
  const statusConfig = {
    pending: {
      label: 'Pendiente',
      color: 'bg-warning-100 text-warning-800',
      icon: '⏳',
    },
    approved: {
      label: 'Aprobada',
      color: 'bg-success-100 text-success-800',
      icon: '✓',
    },
    rejected: {
      label: 'Rechazada',
      color: 'bg-danger-100 text-danger-800',
      icon: '✕',
    },
  };

  const config = statusConfig[evidence.status];

  return (
    <Card className="overflow-hidden transition-transform hover:shadow-lg">
      {/* Imagen */}
      <div className="relative h-48 overflow-hidden bg-neutral-200">
        <img
          src={evidence.photo}
          alt={evidence.eventName}
          className="h-full w-full object-cover"
        />
        <Badge className={cn('absolute right-2 top-2', config.color)}>
          {config.icon} {config.label}
        </Badge>
      </div>

      {/* Contenido */}
      <div className="p-4">
        <h3 className="font-semibold text-neutral-900">{evidence.eventName}</h3>
        <p className="mt-2 flex items-center gap-2 text-sm text-neutral-600">
          <IconCalendar className="h-4 w-4" />
          {new Date(evidence.eventDate).toLocaleDateString('es-ES')}
        </p>
        <p className="mt-1 flex items-center gap-2 text-sm text-neutral-600">
          <IconFileText className="h-4 w-4" />
          {evidence.location}
        </p>

        {/* Puntos */}
        {evidence.status === 'approved' && (
          <div className="mt-4 flex items-center justify-between rounded-lg bg-success-50 px-3 py-2">
            <span className="text-xs font-medium text-success-700">
              Puntos ganados
            </span>
            <span className="text-lg font-bold text-success-700">
              +{evidence.points}
            </span>
          </div>
        )}

        {/* Fecha de Carga */}
        <p className="mt-3 text-xs text-neutral-500">
          Cargado: {new Date(evidence.uploadDate).toLocaleDateString('es-ES')}
        </p>
      </div>
    </Card>
  );
}

/**
 * Galería de Evidencias
 */
export function MemberEvidences() {
  const [isLoading, setIsLoading] = useState(true);
  const [evidences, setEvidences] = useState<Evidence[]>([]);
  const [filter, setFilter] = useState<'all' | 'approved' | 'pending'>(
    'all'
  );

  useEffect(() => {
    // Simular carga de datos
    const timer = setTimeout(() => {
      const mockEvidences: Evidence[] = [
        {
          id: 1,
          eventName: 'Mototurismo Nacional 2025',
          eventDate: '2025-12-15',
          location: 'Colombia - Medellín',
          photo: 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=500&h=400&fit=crop',
          status: 'approved',
          points: 150,
          uploadDate: '2025-12-16',
        },
        {
          id: 2,
          eventName: 'Ruta Cafetera',
          eventDate: '2025-11-20',
          location: 'Colombia - Armenia',
          photo: 'https://images.unsplash.com/photo-1568772585407-9e7c72f9b8f5?w=500&h=400&fit=crop',
          status: 'approved',
          points: 120,
          uploadDate: '2025-11-21',
        },
        {
          id: 3,
          eventName: 'Ruta Costera',
          eventDate: '2025-10-10',
          location: 'Colombia - Cartagena',
          photo: 'https://images.unsplash.com/photo-1516438773669-d25cf4e43a39?w=500&h=400&fit=crop',
          status: 'pending',
          points: 0,
          uploadDate: '2025-10-11',
        },
        {
          id: 4,
          eventName: 'Aventura Andina',
          eventDate: '2025-09-15',
          location: 'Colombia - Bogotá',
          photo: 'https://images.unsplash.com/photo-1511379938547-c1f69b13d835?w=500&h=400&fit=crop',
          status: 'approved',
          points: 140,
          uploadDate: '2025-09-16',
        },
        {
          id: 5,
          eventName: 'Mototurismo del Pacífico',
          eventDate: '2025-08-22',
          location: 'Colombia - Buenaventura',
          photo: 'https://images.unsplash.com/photo-1533473359331-35e06ff4e794?w=500&h=400&fit=crop',
          status: 'approved',
          points: 160,
          uploadDate: '2025-08-23',
        },
        {
          id: 6,
          eventName: 'Ruta Guanacana',
          eventDate: '2025-07-05',
          location: 'Colombia - Cali',
          photo: 'https://images.unsplash.com/photo-1477525348391-fb97991a76a2?w=500&h=400&fit=crop',
          status: 'rejected',
          points: 0,
          uploadDate: '2025-07-06',
        },
      ];

      setEvidences(mockEvidences.sort(
        (a, b) =>
          new Date(b.eventDate).getTime() - new Date(a.eventDate).getTime()
      ));
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  const filteredEvidences = evidences.filter((e) => {
    if (filter === 'all') return true;
    return e.status === filter;
  });

  const stats = {
    total: evidences.length,
    approved: evidences.filter((e) => e.status === 'approved').length,
    pending: evidences.filter((e) => e.status === 'pending').length,
    totalPoints: evidences
      .filter((e) => e.status === 'approved')
      .reduce((sum, e) => sum + e.points, 0),
  };

  return (
    <LayoutWrapper
      title="Mis Evidencias"
      breadcrumbs={[
        { label: 'Mi Perfil', href: '/member' },
        { label: 'Evidencias', href: '/member/evidences' },
      ]}
    >
      <div className="space-y-6">
        {/* Encabezado */}
        <div>
          <div className="flex items-center gap-3">
            <IconUpload className="h-8 w-8 text-primary-600" />
            <h1 className="text-3xl font-bold text-neutral-900">
              Mis Evidencias
            </h1>
          </div>
          <p className="mt-2 text-neutral-600">
            Historial de fotos cargadas en eventos de mototurismo
          </p>
        </div>

        {/* Tarjetas de Estadísticas */}
        <div className="grid gap-4 md:grid-cols-4">
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Total Cargadas</p>
            <p className="mt-2 text-2xl font-bold text-neutral-900">
              {stats.total}
            </p>
          </Card>
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Aprobadas</p>
            <p className="mt-2 text-2xl font-bold text-success-600">
              {stats.approved}
            </p>
          </Card>
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Pendientes</p>
            <p className="mt-2 text-2xl font-bold text-warning-600">
              {stats.pending}
            </p>
          </Card>
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Puntos Ganados</p>
            <p className="mt-2 text-2xl font-bold text-primary-600">
              +{stats.totalPoints}
            </p>
          </Card>
        </div>

        {/* Filtros */}
        <div className="flex gap-2">
          <button
            onClick={() => setFilter('all')}
            className={cn(
              'rounded-lg px-4 py-2 text-sm font-medium transition-colors',
              filter === 'all'
                ? 'bg-primary-600 text-white'
                : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
            )}
          >
            Todas ({stats.total})
          </button>
          <button
            onClick={() => setFilter('approved')}
            className={cn(
              'rounded-lg px-4 py-2 text-sm font-medium transition-colors',
              filter === 'approved'
                ? 'bg-success-600 text-white'
                : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
            )}
          >
            Aprobadas ({stats.approved})
          </button>
          <button
            onClick={() => setFilter('pending')}
            className={cn(
              'rounded-lg px-4 py-2 text-sm font-medium transition-colors',
              filter === 'pending'
                ? 'bg-warning-600 text-white'
                : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
            )}
          >
            Pendientes ({stats.pending})
          </button>
        </div>

        {/* Grid de Evidencias */}
        {isLoading ? (
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <Skeleton key={i} className="h-80" />
            ))}
          </div>
        ) : filteredEvidences.length > 0 ? (
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {filteredEvidences.map((evidence) => (
              <EvidenceCard key={evidence.id} evidence={evidence} />
            ))}
          </div>
        ) : (
          <EmptyState
            title="No hay evidencias"
            description={
              filter === 'all'
                ? 'Aún no has cargado ninguna evidencia. ¡Comienza ahora!'
                : `No hay evidencias ${filter === 'approved' ? 'aprobadas' : 'pendientes'}`
            }
            action={
              <a
                href="/admin/cor"
                className="inline-flex items-center justify-center rounded-lg bg-primary-600 px-6 py-2 text-sm font-medium text-white hover:bg-primary-700 transition-colors"
              >
                Cargar Evidencia
              </a>
            }
          />
        )}
      </div>
    </LayoutWrapper>
  );
}
