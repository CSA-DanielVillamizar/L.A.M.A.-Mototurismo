'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import {
  IconChart,
  IconFavorite,
  IconCalendar,
  IconUpload,
} from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Interfaz de datos de miembro en detalle
 */
interface RankingMemberDetail {
  id: string;
  rank: number;
  name: string;
  class: string;
  joinDate: string;
  points: number;
  totalPoints: number;
  events: number;
  evidences: number;
  avatar: string;
  bio: string;
  achievement: {
    title: string;
    date: string;
    points: number;
  }[];
  monthlyStats: {
    month: string;
    points: number;
  }[];
}

/**
 * Componente de Detalle de Ranking
 * Muestra información detallada de un miembro en el ranking
 */
export function RankingDetail() {
  const [isLoading, setIsLoading] = useState(true);
  const [member, setMember] = useState<RankingMemberDetail | null>(null);

  useEffect(() => {
    // Simular carga de datos
    const timer = setTimeout(() => {
      const mockData: RankingMemberDetail = {
        id: 'member-12',
        rank: 12,
        name: 'Daniel Villamizar',
        class: 'Premium',
        joinDate: '2023-06-15',
        points: 2450,
        totalPoints: 12500,
        events: 28,
        evidences: 156,
        avatar:
          'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=400&fit=crop',
        bio: 'Apasionado por el mototurismo y la aventura. Siempre listo para nuevas rutas.',
        achievement: [
          {
            title: 'Ruta Cafetera Completa',
            date: '2025-12-10',
            points: 500,
          },
          {
            title: 'Campeonato Regional 2025',
            date: '2025-11-20',
            points: 750,
          },
          {
            title: 'Miles de Kilómetros',
            date: '2025-10-05',
            points: 300,
          },
          {
            title: 'Fotografía Premium',
            date: '2025-09-18',
            points: 250,
          },
          {
            title: 'Miembro Fiel',
            date: '2025-08-30',
            points: 200,
          },
        ],
        monthlyStats: [
          { month: 'Enero', points: 245 },
          { month: 'Febrero', points: 320 },
          { month: 'Marzo', points: 280 },
          { month: 'Abril', points: 410 },
          { month: 'Mayo', points: 365 },
          { month: 'Junio', points: 290 },
          { month: 'Julio', points: 450 },
          { month: 'Agosto', points: 385 },
          { month: 'Septiembre', points: 420 },
          { month: 'Octubre', points: 375 },
          { month: 'Noviembre', points: 500 },
          { month: 'Diciembre', points: 470 },
        ],
      };

      setMember(mockData);
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  if (isLoading) {
    return (
      <LayoutWrapper
        title="Detalle de Ranking"
        breadcrumbs={[
          { label: 'Ranking', href: '/member/ranking' },
          { label: 'Detalle', href: '/member/ranking/detail' },
        ]}
      >
        <div className="space-y-6">
          <Skeleton className="h-64 w-full" />
          <Skeleton className="h-32 w-full" />
          <Skeleton className="h-48 w-full" />
        </div>
      </LayoutWrapper>
    );
  }

  if (!member) {
    return (
      <LayoutWrapper title="Detalle de Ranking">
        <div className="text-center py-12">
          <p className="text-neutral-600">No se encontró el miembro</p>
        </div>
      </LayoutWrapper>
    );
  }

  return (
    <LayoutWrapper
      title="Detalle de Ranking"
      breadcrumbs={[
        { label: 'Ranking', href: '/member/ranking' },
        { label: member.name, href: '/member/ranking/detail' },
      ]}
    >
      <div className="space-y-6">
        {/* Tarjeta de Perfil Principal */}
        <Card className="p-8">
          <div className="flex gap-6">
            <div className="flex-shrink-0">
              <img
                src={member.avatar}
                alt={member.name}
                className="h-32 w-32 rounded-full border-4 border-primary-200 object-cover"
              />
            </div>
            <div className="flex-1">
              <div className="flex items-start justify-between">
                <div>
                  <div className="flex items-center gap-3 mb-2">
                    <h1 className="text-3xl font-bold text-neutral-900">
                      {member.name}
                    </h1>
                    <span className="text-4xl font-bold text-warning-600">
                      #{member.rank}
                    </span>
                  </div>
                  <p className="text-neutral-600 mb-3">{member.bio}</p>
                </div>
                <Badge className="bg-primary-100 text-primary-700">
                  {member.class}
                </Badge>
              </div>

              {/* Grid de Estadísticas Principales */}
              <div className="grid grid-cols-4 gap-4 mt-6 pt-6 border-t border-neutral-200">
                <div className="text-center">
                  <p className="text-sm text-neutral-600 mb-1">Puntos Actuales</p>
                  <p className="text-2xl font-bold text-primary-600">
                    {member.points}
                  </p>
                </div>
                <div className="text-center">
                  <p className="text-sm text-neutral-600 mb-1">Puntos Totales</p>
                  <p className="text-2xl font-bold text-primary-600">
                    {member.totalPoints}
                  </p>
                </div>
                <div className="text-center">
                  <p className="text-sm text-neutral-600 mb-1">Eventos</p>
                  <p className="text-2xl font-bold text-success-600">
                    {member.events}
                  </p>
                </div>
                <div className="text-center">
                  <p className="text-sm text-neutral-600 mb-1">Evidencias</p>
                  <p className="text-2xl font-bold text-success-600">
                    {member.evidences}
                  </p>
                </div>
              </div>
            </div>
          </div>
        </Card>

        {/* Gráfico de Progreso Mensual */}
        <Card className="p-6">
          <div className="flex items-center gap-2 mb-6">
            <IconChart className="h-6 w-6 text-primary-600" />
            <h2 className="text-xl font-semibold text-neutral-900">
              Progreso Anual
            </h2>
          </div>

          <div className="flex items-end justify-around h-48 gap-1 bg-neutral-50 p-4 rounded-lg">
            {member.monthlyStats.map((stat, idx) => {
              const maxPoints = Math.max(
                ...member.monthlyStats.map((s) => s.points)
              );
              const height = (stat.points / maxPoints) * 100;

              return (
                <div key={idx} className="flex flex-col items-center gap-2">
                  <div
                    className="w-6 bg-gradient-to-t from-primary-500 to-primary-300 rounded-t transition-all hover:from-primary-600 hover:to-primary-400"
                    style={{ height: `${height}%` }}
                    title={`${stat.month}: ${stat.points} pts`}
                  />
                  <span className="text-xs text-neutral-600">
                    {stat.month.slice(0, 3)}
                  </span>
                </div>
              );
            })}
          </div>

          <div className="mt-4 p-3 bg-primary-50 rounded-lg border border-primary-200">
            <p className="text-sm text-neutral-700">
              <span className="font-semibold">Tendencia:</span> +15% de
              crecimiento en los últimos 3 meses
            </p>
          </div>
        </Card>

        {/* Logros y Reconocimientos */}
        <Card className="p-6">
          <div className="flex items-center gap-2 mb-6">
            <IconFavorite className="h-6 w-6 text-warning-600" />
            <h2 className="text-xl font-semibold text-neutral-900">
              Logros Recientes
            </h2>
          </div>

          <div className="space-y-3">
            {member.achievement.map((achievement, idx) => (
              <div
                key={idx}
                className="flex items-center justify-between p-4 border border-neutral-200 rounded-lg hover:bg-neutral-50 transition-colors"
              >
                <div>
                  <h3 className="font-medium text-neutral-900">
                    {achievement.title}
                  </h3>
                  <div className="flex items-center gap-2 mt-1">
                    <IconCalendar className="h-4 w-4 text-neutral-400" />
                    <p className="text-sm text-neutral-600">
                      {new Date(achievement.date).toLocaleDateString('es-ES', {
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                      })}
                    </p>
                  </div>
                </div>
                <Badge className="bg-success-100 text-success-700 font-semibold">
                  +{achievement.points} pts
                </Badge>
              </div>
            ))}
          </div>
        </Card>

        {/* Información de Afiliación */}
        <Card className="p-6 bg-gradient-to-br from-primary-50 to-secondary-50">
          <h2 className="text-lg font-semibold text-neutral-900 mb-4">
            Información de Afiliación
          </h2>
          <div className="grid grid-cols-2 gap-6">
            <div>
              <p className="text-sm text-neutral-600">Miembro Desde</p>
              <p className="text-lg font-semibold text-neutral-900 mt-1">
                {new Date(member.joinDate).toLocaleDateString('es-ES', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
                })}
              </p>
            </div>
            <div>
              <p className="text-sm text-neutral-600">Categoría</p>
              <p className="text-lg font-semibold text-neutral-900 mt-1">
                {member.class}
              </p>
            </div>
            <div>
              <p className="text-sm text-neutral-600">Años de Actividad</p>
              <p className="text-lg font-semibold text-neutral-900 mt-1">
                {new Date().getFullYear() -
                  new Date(member.joinDate).getFullYear()}{' '}
                año
                {new Date().getFullYear() -
                  new Date(member.joinDate).getFullYear() !== 1
                  ? 's'
                  : ''}
              </p>
            </div>
            <div>
              <p className="text-sm text-neutral-600">Tasa de Participación</p>
              <p className="text-lg font-semibold text-neutral-900 mt-1">
                {Math.round((member.events / 52) * 100)}%
              </p>
            </div>
          </div>
        </Card>
      </div>
    </LayoutWrapper>
  );
}
