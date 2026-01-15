'use client';

import React, { useState, useEffect } from 'react';
import { LayoutWrapper } from '@/components/layout';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import {
  IconUser,
  IconFileText,
  IconSettings,
  IconUpload,
  IconCalendar,
} from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Interfaz de perfil de usuario
 */
interface MemberProfile {
  id: number;
  name: string;
  email: string;
  phone: string;
  location: string;
  joinDate: string;
  visitorClass: string;
  bio: string;
  totalEvents: number;
  totalEvidences: number;
  totalPoints: number;
  favoriteRoutes: string[];
  avatar?: string;
}

/**
 * Componente Perfil del Miembro
 */
export function MemberProfile() {
  const [isLoading, setIsLoading] = useState(true);
  const [profile, setProfile] = useState<MemberProfile | null>(null);

  useEffect(() => {
    // Simular carga de datos
    const timer = setTimeout(() => {
      setProfile({
        id: 1,
        name: 'Daniel Villamizar',
        email: 'daniel.villamizar@example.com',
        phone: '+57 (123) 456-7890',
        location: 'Medellín, Colombia',
        joinDate: '2023-06-15',
        visitorClass: 'Premium',
        bio: 'Apasionado por el mototurismo y la aventura en carreteras colombianas. Miembro activo desde 2023.',
        totalEvents: 12,
        totalEvidences: 28,
        totalPoints: 2450,
        favoriteRoutes: [
          'Ruta Cafetera',
          'Mototurismo Andino',
          'Aventura Costera',
        ],
        avatar:
          'https://images.unsplash.com/photo-1633332755192-727a72618b9b?w=400&h=400&fit=crop',
      });
      setIsLoading(false);
    }, 800);

    return () => clearTimeout(timer);
  }, []);

  if (isLoading || !profile) {
    return (
      <LayoutWrapper
        title="Mi Perfil"
        breadcrumbs={[{ label: 'Mi Perfil', href: '/member' }]}
      >
        <Skeleton className="h-96" />
      </LayoutWrapper>
    );
  }

  return (
    <LayoutWrapper
      title="Mi Perfil"
      breadcrumbs={[{ label: 'Mi Perfil', href: '/member' }]}
    >
      <div className="space-y-6">
        {/* Header con Avatar */}
        <Card className="overflow-hidden">
          <div className="h-32 bg-gradient-to-r from-primary-500 to-secondary-500" />
          <div className="px-6 pb-6 pt-0">
            <div className="flex flex-col gap-4 sm:flex-row sm:items-end">
              {/* Avatar */}
              <div className="-mt-16">
                <img
                  src={profile.avatar}
                  alt={profile.name}
                  className="h-32 w-32 rounded-lg border-4 border-white object-cover shadow-lg"
                />
              </div>

              {/* Info Básica */}
              <div className="flex-1">
                <div>
                  <h1 className="text-3xl font-bold text-neutral-900">
                    {profile.name}
                  </h1>
                  <div className="mt-2 flex flex-wrap gap-2">
                    <Badge className="bg-primary-100 text-primary-800">
                      {profile.visitorClass}
                    </Badge>
                    <Badge className="bg-secondary-100 text-secondary-800">
                      Miembro desde{' '}
                      {new Date(profile.joinDate).toLocaleDateString(
                        'es-ES',
                        { year: 'numeric', month: 'short' }
                      )}
                    </Badge>
                  </div>
                </div>
              </div>

              {/* Botones de Acción */}
              <div className="flex gap-2">
                <button className="rounded-lg bg-primary-600 px-4 py-2 text-sm font-medium text-white hover:bg-primary-700 transition-colors">
                  Editar Perfil
                </button>
              </div>
            </div>

            {/* Bio */}
            {profile.bio && (
              <p className="mt-4 text-neutral-700">{profile.bio}</p>
            )}
          </div>
        </Card>

        {/* Grid de Estadísticas */}
        <div className="grid gap-4 md:grid-cols-4">
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Eventos Participados</p>
            <p className="mt-2 text-3xl font-bold text-neutral-900">
              {profile.totalEvents}
            </p>
          </Card>
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Evidencias Cargadas</p>
            <p className="mt-2 text-3xl font-bold text-neutral-900">
              {profile.totalEvidences}
            </p>
          </Card>
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Puntos Acumulados</p>
            <p className="mt-2 text-3xl font-bold text-primary-600">
              {profile.totalPoints}
            </p>
          </Card>
          <Card className="p-4">
            <p className="text-sm text-neutral-600">Clase de Visitante</p>
            <p className="mt-2 text-3xl font-bold text-secondary-600">
              {profile.visitorClass}
            </p>
          </Card>
        </div>

        {/* Información de Contacto */}
        <Card className="p-6">
          <h2 className="text-xl font-semibold text-neutral-900">
            Información de Contacto
          </h2>
          <div className="mt-4 space-y-4">
            <div className="flex items-center gap-3">
              <IconFileText className="h-5 w-5 text-neutral-400" />
              <div>
                <p className="text-xs text-neutral-600">Correo Electrónico</p>
                <p className="font-medium text-neutral-900">{profile.email}</p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <IconSettings className="h-5 w-5 text-neutral-400" />
              <div>
                <p className="text-xs text-neutral-600">Teléfono</p>
                <p className="font-medium text-neutral-900">{profile.phone}</p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <IconUpload className="h-5 w-5 text-neutral-400" />
              <div>
                <p className="text-xs text-neutral-600">Ubicación</p>
                <p className="font-medium text-neutral-900">
                  {profile.location}
                </p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <IconCalendar className="h-5 w-5 text-neutral-400" />
              <div>
                <p className="text-xs text-neutral-600">Miembro Desde</p>
                <p className="font-medium text-neutral-900">
                  {new Date(profile.joinDate).toLocaleDateString('es-ES', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',
                  })}
                </p>
              </div>
            </div>
          </div>
        </Card>

        {/* Rutas Favoritas */}
        <Card className="p-6">
          <h2 className="text-xl font-semibold text-neutral-900">
            Rutas Favoritas
          </h2>
          <div className="mt-4 flex flex-wrap gap-2">
            {profile.favoriteRoutes.map((route) => (
              <Badge
                key={route}
                className="bg-secondary-100 text-secondary-800"
              >
                {route}
              </Badge>
            ))}
          </div>
          {profile.favoriteRoutes.length === 0 && (
            <p className="mt-4 text-neutral-600">
              Aún no tienes rutas favoritas marcadas
            </p>
          )}
        </Card>

        {/* Acciones Rápidas */}
        <Card className="border-primary-200 bg-primary-50 p-6">
          <h2 className="text-lg font-semibold text-primary-900">
            Próximos Pasos
          </h2>
          <ul className="mt-4 space-y-3 text-sm text-primary-800">
            <li className="flex items-start gap-2">
              <span>✓</span>
              <span>
                Completa tu perfil con información adicional (foto de perfil,
                bio)
              </span>
            </li>
            <li className="flex items-start gap-2">
              <span>✓</span>
              <span>
                Registra más evidencias para subir tu posición en el ranking
              </span>
            </li>
            <li className="flex items-start gap-2">
              <span>✓</span>
              <span>
                Alcanza el siguiente nivel (Premium Plus) con 500 puntos más
              </span>
            </li>
          </ul>
        </Card>
      </div>
    </LayoutWrapper>
  );
}
