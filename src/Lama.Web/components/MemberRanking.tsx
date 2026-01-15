'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { Badge } from '@/components/ui/badge';
import {
  IconChevronUp,
  IconChevronDown,
} from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Fila de ranking
 */
interface RankingRowProps {
  rank: number;
  name: string;
  points: number;
  change?: number;
  isCurrentUser?: boolean;
}

function RankingRow({
  rank,
  name,
  points,
  change,
  isCurrentUser,
}: RankingRowProps) {
  const isTopThree = rank <= 3;
  const medalEmoji = {
    1: '🥇',
    2: '🥈',
    3: '🥉',
  }[rank] as any;

  return (
    <div
      className={cn(
        'flex items-center justify-between border-b border-neutral-200 px-6 py-4',
        isCurrentUser && 'bg-primary-50',
        isTopThree && 'bg-neutral-50'
      )}
    >
      <div className="flex items-center gap-4">
        <div className="w-12 text-center">
          {isTopThree ? (
            <span className="text-2xl">{medalEmoji}</span>
          ) : (
            <span className="text-lg font-semibold text-neutral-900">
              #{rank}
            </span>
          )}
        </div>
        <div>
          <p
            className={cn(
              'font-medium',
              isCurrentUser
                ? 'text-primary-900'
                : 'text-neutral-900'
            )}
          >
            {name}
            {isCurrentUser && (
              <span className="ml-2 text-xs font-normal text-primary-600">
                (Tú)
              </span>
            )}
          </p>
        </div>
      </div>
      <div className="flex items-center gap-4">
        <div className="text-right">
          <p className="text-lg font-bold text-neutral-900">{points}</p>
          <p className="text-xs text-neutral-600">puntos</p>
        </div>
        {change !== undefined && (
          <div className="w-12 text-center">
            {change > 0 ? (
              <div className="flex items-center justify-center gap-1 text-success-600">
                <IconChevronUp className="h-4 w-4" />
                <span className="text-xs font-medium">{change}</span>
              </div>
            ) : change < 0 ? (
              <div className="flex items-center justify-center gap-1 text-danger-600">
                <IconChevronDown className="h-4 w-4" />
                <span className="text-xs font-medium">{Math.abs(change)}</span>
              </div>
            ) : (
              <span className="text-xs text-neutral-400">−</span>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

/**
 * Tabla de Ranking Nacional
 */
export function MemberRanking() {
  const [isLoading, setIsLoading] = useState(true);
  const [members, setMembers] = useState<RankingRowProps[]>([]);
  const [currentUserRank, setCurrentUserRank] = useState<number | null>(null);

  useEffect(() => {
    // Simular carga de datos
    const timer = setTimeout(() => {
      const mockData: RankingRowProps[] = [
        { rank: 1, name: 'Carlos Mendoza', points: 3450, change: 5 },
        { rank: 2, name: 'Laura Rodríguez', points: 3200, change: 2 },
        { rank: 3, name: 'Juan García', points: 2980, change: -1 },
        {
          rank: 12,
          name: 'Daniel Villamizar',
          points: 2450,
          change: 3,
          isCurrentUser: true,
        },
        { rank: 4, name: 'María López', points: 2850, change: 0 },
        { rank: 5, name: 'Pedro Sánchez', points: 2750, change: -2 },
        { rank: 6, name: 'Ana Martínez', points: 2650, change: 4 },
        { rank: 7, name: 'Roberto Díaz', points: 2550, change: 1 },
        { rank: 8, name: 'Sofía Romero', points: 2480, change: 3 },
        { rank: 9, name: 'Miguel Torres', points: 2400, change: -3 },
        { rank: 10, name: 'Elena Castro', points: 2350, change: 2 },
        { rank: 11, name: 'Luis Jiménez', points: 2300, change: 0 },
      ];

      setMembers(mockData.sort((a, b) => a.rank - b.rank));
      setCurrentUserRank(12);
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  return (
    <LayoutWrapper
      title="Ranking Nacional"
      breadcrumbs={[
        { label: 'Mi Perfil', href: '/member' },
        { label: 'Ranking', href: '/member/ranking' },
      ]}
    >
      <div className="space-y-6">
        {/* Encabezado */}
        <div>
          <div className="flex items-center gap-3">
            <span className="text-3xl font-bold text-warning-600">#{currentUserRank}</span>
            <h1 className="text-3xl font-bold text-neutral-900">
              Ranking Nacional
            </h1>
          </div>
          <p className="mt-2 text-neutral-600">
            Posición actual de los miembros activos en mototurismo
          </p>
        </div>

        {/* Tarjeta de Posición del Usuario */}
        {currentUserRank && (
          <Card className="border-primary-200 bg-primary-50 p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-primary-700">Tu Posición Actual</p>
                <p className="mt-1 text-3xl font-bold text-primary-900">
                  Puesto #{currentUserRank}
                </p>
              </div>
              <Badge variant="outline">2450 puntos</Badge>
            </div>
            <p className="mt-4 text-sm text-primary-700">
              Te faltan 1000 puntos para alcanzar el top 3. ¡Sigue registrando
              evidencias!
            </p>
          </Card>
        )}

        {/* Tabla de Ranking */}
        <Card className="overflow-hidden">
          <div className="border-b border-neutral-200 bg-neutral-50 px-6 py-4">
            <div className="flex items-center justify-between">
              <h2 className="font-semibold text-neutral-900">
                Ranking de Miembros
              </h2>
              <span className="text-sm text-neutral-600">
                {members.length} miembros
              </span>
            </div>
          </div>

          {isLoading ? (
            <div className="space-y-2 p-6">
              {[1, 2, 3, 4, 5].map((i) => (
                <Skeleton key={i} className="h-16" />
              ))}
            </div>
          ) : (
            <div>
              {members.map((member) => (
                <RankingRow
                  key={`${member.rank}-${member.name}`}
                  {...member}
                />
              ))}
            </div>
          )}
        </Card>

        {/* Información */}
        <Card className="border-secondary-200 bg-secondary-50 p-6">
          <h3 className="font-semibold text-secondary-900">
            ¿Cómo funcionan los puntos?
          </h3>
          <ul className="mt-4 space-y-2 text-sm text-secondary-800">
            <li>
              <strong>Participación en Evento:</strong> Registra tu asistencia
              con evidencias (fotos)
            </li>
            <li>
              <strong>Distancia Recorrida:</strong> Gana puntos adicionales por
              kilómetros
            </li>
            <li>
              <strong>Actualización Diaria:</strong> Ranking actualizado cada 24
              horas
            </li>
          </ul>
        </Card>
      </div>
    </LayoutWrapper>
  );
}
