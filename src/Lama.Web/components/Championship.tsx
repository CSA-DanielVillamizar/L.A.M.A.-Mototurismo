'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import {
  IconChart,
  IconCalendar,
  IconCheckmark,
} from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Interfaz de campeonato
 */
interface Championship {
  id: string;
  year: number;
  name: string;
  season: string;
  startDate: string;
  endDate: string;
  status: 'completed' | 'ongoing' | 'upcoming';
  totalParticipants: number;
  totalEvents: number;
  winner: {
    name: string;
    points: number;
  };
  userRank: number;
  userPoints: number;
  rounds: {
    name: string;
    date: string;
    completed: boolean;
    userPoints?: number;
  }[];
}

/**
 * Componente de Historial de Campeonatos
 * Muestra todos los campeonatos en los que el usuario ha participado
 */
export function Championship() {
  const [isLoading, setIsLoading] = useState(true);
  const [championships, setChampionships] = useState<Championship[]>([]);
  const [selectedFilter, setSelectedFilter] = useState<
    'all' | 'completed' | 'ongoing'
  >('all');

  useEffect(() => {
    // Simular carga de datos
    const timer = setTimeout(() => {
      const mockData: Championship[] = [
        {
          id: 'champ-2025',
          year: 2025,
          name: 'Campeonato Nacional de Mototurismo 2025',
          season: 'Anual',
          startDate: '2025-01-15',
          endDate: '2025-12-31',
          status: 'ongoing',
          totalParticipants: 156,
          totalEvents: 12,
          winner: {
            name: 'Carlos Mendoza',
            points: 8500,
          },
          userRank: 12,
          userPoints: 2450,
          rounds: [
            {
              name: 'Ronda 1 - Eje Cafetero',
              date: '2025-01-15',
              completed: true,
              userPoints: 245,
            },
            {
              name: 'Ronda 2 - Pacífico',
              date: '2025-02-20',
              completed: true,
              userPoints: 320,
            },
            {
              name: 'Ronda 3 - Caribe',
              date: '2025-03-18',
              completed: true,
              userPoints: 280,
            },
            {
              name: 'Ronda 4 - Andina',
              date: '2025-05-10',
              completed: true,
              userPoints: 410,
            },
            {
              name: 'Ronda 5 - Llanos',
              date: '2025-07-05',
              completed: true,
              userPoints: 450,
            },
            {
              name: 'Ronda 6 - Amazonia',
              date: '2025-09-15',
              completed: true,
              userPoints: 420,
            },
            {
              name: 'Ronda 7 - Atlántica',
              date: '2025-11-20',
              completed: false,
            },
            {
              name: 'Ronda 8 - Gran Final',
              date: '2025-12-28',
              completed: false,
            },
          ],
        },
        {
          id: 'champ-2024',
          year: 2024,
          name: 'Campeonato Nacional de Mototurismo 2024',
          season: 'Anual',
          startDate: '2024-01-10',
          endDate: '2024-12-20',
          status: 'completed',
          totalParticipants: 142,
          totalEvents: 10,
          winner: {
            name: 'Laura Rodríguez',
            points: 7850,
          },
          userRank: 15,
          userPoints: 5800,
          rounds: [
            {
              name: 'Ronda 1 - Eje Cafetero',
              date: '2024-01-15',
              completed: true,
              userPoints: 520,
            },
            {
              name: 'Ronda 2 - Pacífico',
              date: '2024-02-20',
              completed: true,
              userPoints: 480,
            },
            {
              name: 'Ronda 3 - Caribe',
              date: '2024-03-18',
              completed: true,
              userPoints: 620,
            },
            {
              name: 'Ronda 4 - Andina',
              date: '2024-05-10',
              completed: true,
              userPoints: 710,
            },
            {
              name: 'Ronda 5 - Llanos',
              date: '2024-07-05',
              completed: true,
              userPoints: 650,
            },
            {
              name: 'Ronda 6 - Amazonia',
              date: '2024-09-15',
              completed: true,
              userPoints: 580,
            },
            {
              name: 'Ronda 7 - Atlántica',
              date: '2024-11-20',
              completed: true,
              userPoints: 640,
            },
            {
              name: 'Ronda 8 - Gran Final',
              date: '2024-12-20',
              completed: true,
              userPoints: 600,
            },
          ],
        },
        {
          id: 'champ-2023',
          year: 2023,
          name: 'Campeonato Nacional de Mototurismo 2023',
          season: 'Anual',
          startDate: '2023-01-10',
          endDate: '2023-12-15',
          status: 'completed',
          totalParticipants: 128,
          totalEvents: 10,
          winner: {
            name: 'Juan García',
            points: 7200,
          },
          userRank: 28,
          userPoints: 3450,
          rounds: [
            {
              name: 'Ronda 1 - Eje Cafetero',
              date: '2023-01-15',
              completed: true,
              userPoints: 320,
            },
            {
              name: 'Ronda 2 - Pacífico',
              date: '2023-02-20',
              completed: true,
              userPoints: 380,
            },
            {
              name: 'Ronda 3 - Caribe',
              date: '2023-03-18',
              completed: true,
              userPoints: 280,
            },
            {
              name: 'Ronda 4 - Andina',
              date: '2023-05-10',
              completed: true,
              userPoints: 420,
            },
            {
              name: 'Ronda 5 - Llanos',
              date: '2023-07-05',
              completed: true,
              userPoints: 350,
            },
            {
              name: 'Ronda 6 - Amazonia',
              date: '2023-09-15',
              completed: true,
              userPoints: 310,
            },
            {
              name: 'Ronda 7 - Atlántica',
              date: '2023-11-20',
              completed: true,
              userPoints: 280,
            },
            {
              name: 'Ronda 8 - Gran Final',
              date: '2023-12-15',
              completed: true,
              userPoints: 310,
            },
          ],
        },
      ];

      setChampionships(mockData);
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  const filteredChampionships = championships.filter((champ) => {
    if (selectedFilter === 'all') return true;
    return champ.status === selectedFilter;
  });

  if (isLoading) {
    return (
      <LayoutWrapper
        title="Campeonatos"
        breadcrumbs={[
          { label: 'Mi Perfil', href: '/member' },
          { label: 'Campeonatos', href: '/member/championship' },
        ]}
      >
        <div className="space-y-6">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-48 w-full" />
          ))}
        </div>
      </LayoutWrapper>
    );
  }

  return (
    <LayoutWrapper
      title="Campeonatos"
      breadcrumbs={[
        { label: 'Mi Perfil', href: '/member' },
        { label: 'Campeonatos', href: '/member/championship' },
      ]}
    >
      <div className="space-y-6">
        {/* Filtros */}
        <div className="flex gap-3">
          <button
            onClick={() => setSelectedFilter('all')}
            className={cn(
              'px-4 py-2 rounded-lg font-medium transition-colors',
              selectedFilter === 'all'
                ? 'bg-primary-600 text-white'
                : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
            )}
          >
            Todos ({championships.length})
          </button>
          <button
            onClick={() => setSelectedFilter('ongoing')}
            className={cn(
              'px-4 py-2 rounded-lg font-medium transition-colors',
              selectedFilter === 'ongoing'
                ? 'bg-primary-600 text-white'
                : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
            )}
          >
            En Curso ({championships.filter((c) => c.status === 'ongoing').length})
          </button>
          <button
            onClick={() => setSelectedFilter('completed')}
            className={cn(
              'px-4 py-2 rounded-lg font-medium transition-colors',
              selectedFilter === 'completed'
                ? 'bg-primary-600 text-white'
                : 'bg-neutral-100 text-neutral-700 hover:bg-neutral-200'
            )}
          >
            Completados ({championships.filter((c) => c.status === 'completed').length})
          </button>
        </div>

        {/* Lista de Campeonatos */}
        <div className="space-y-4">
          {filteredChampionships.map((championship) => (
            <Card key={championship.id} className="p-6">
              {/* Encabezado */}
              <div className="flex items-start justify-between mb-4">
                <div>
                  <div className="flex items-center gap-3">
                    <h2 className="text-2xl font-bold text-neutral-900">
                      {championship.name}
                    </h2>
                    <Badge
                      className={cn(
                        championship.status === 'completed'
                          ? 'bg-success-100 text-success-700'
                          : championship.status === 'ongoing'
                            ? 'bg-warning-100 text-warning-700'
                            : 'bg-neutral-100 text-neutral-700'
                      )}
                    >
                      {championship.status === 'completed'
                        ? 'Completado'
                        : championship.status === 'ongoing'
                          ? 'En Curso'
                          : 'Próximo'}
                    </Badge>
                  </div>
                  <p className="text-sm text-neutral-600 mt-1">
                    {new Date(championship.startDate).toLocaleDateString('es-ES', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}{' '}
                    -{' '}
                    {new Date(championship.endDate).toLocaleDateString('es-ES', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}
                  </p>
                </div>
                <div className="text-right">
                  <p className="text-4xl font-bold text-primary-600">
                    #{championship.userRank}
                  </p>
                  <p className="text-sm text-neutral-600 mt-1">Tu posición</p>
                </div>
              </div>

              {/* Estadísticas Principales */}
              <div className="grid grid-cols-4 gap-4 mb-6 pb-6 border-b border-neutral-200">
                <div>
                  <p className="text-sm text-neutral-600 mb-1">Tus Puntos</p>
                  <p className="text-2xl font-bold text-primary-600">
                    {championship.userPoints}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-neutral-600 mb-1">Ganador</p>
                  <p className="text-base font-semibold text-neutral-900">
                    {championship.winner.name}
                  </p>
                  <p className="text-sm text-neutral-500">
                    {championship.winner.points} pts
                  </p>
                </div>
                <div>
                  <p className="text-sm text-neutral-600 mb-1">Participantes</p>
                  <p className="text-2xl font-bold text-neutral-900">
                    {championship.totalParticipants}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-neutral-600 mb-1">Rondas</p>
                  <p className="text-2xl font-bold text-neutral-900">
                    {championship.totalEvents}
                  </p>
                </div>
              </div>

              {/* Progreso de Rondas */}
              <div>
                <h3 className="text-sm font-semibold text-neutral-900 mb-3">
                  Progreso de Rondas
                </h3>
                <div className="space-y-2">
                  {championship.rounds.map((round, idx) => (
                    <div
                      key={idx}
                      className="flex items-center justify-between p-3 bg-neutral-50 rounded-lg"
                    >
                      <div className="flex items-center gap-3">
                        {round.completed ? (
                          <div className="flex-shrink-0 h-6 w-6 rounded-full bg-success-100 flex items-center justify-center">
                            <IconCheckmark className="h-4 w-4 text-success-600" />
                          </div>
                        ) : (
                          <div className="flex-shrink-0 h-6 w-6 rounded-full bg-neutral-200" />
                        )}
                        <div>
                          <p className="font-medium text-neutral-900">
                            {round.name}
                          </p>
                          <p className="text-xs text-neutral-500">
                            {new Date(round.date).toLocaleDateString('es-ES')}
                          </p>
                        </div>
                      </div>
                      {round.userPoints && (
                        <Badge className="bg-primary-100 text-primary-700">
                          +{round.userPoints} pts
                        </Badge>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            </Card>
          ))}
        </div>
      </div>
    </LayoutWrapper>
  );
}
