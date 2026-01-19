'use client';

/**
 * Página Principal de Admin - Dashboard
 * Proporciona una vista general del sistema COR L.A.MA
 */

import { Suspense } from 'react';
import { Breadcrumbs } from '@/components/layout/AppShell';
import { Loading } from '@/components/ui/loading';
import { BarChart3, Calendar, Users, TrendingUp } from 'lucide-react';
import Link from 'next/link';

interface StatCard {
  label: string;
  value: string;
  icon: React.ReactNode;
  href: string;
  color: string;
}

function DashboardContent() {
  const stats: StatCard[] = [
    {
      label: 'Eventos',
      value: '0',
      icon: <Calendar size={24} />,
      href: '/admin/events',
      color: 'primary',
    },
    {
      label: 'Miembros',
      value: '0',
      icon: <Users size={24} />,
      href: '/admin/members',
      color: 'accent',
    },
    {
      label: 'Validaciones',
      value: '0',
      icon: <TrendingUp size={24} />,
      href: '/admin/cor',
      color: 'success',
    },
  ];

  return (
    <div className="space-y-6">
      <Breadcrumbs items={[{ label: 'Admin' }, { label: 'Dashboard' }]} />

      <div className="px-6">
        <div className="flex items-center gap-3 mb-8">
          <BarChart3 className="text-primary-600" size={32} />
          <h1 className="text-3xl font-bold text-primary-900">Dashboard</h1>
        </div>

        {/* Stats Grid */}
        <div className="grid gap-6 md:grid-cols-3 mb-8">
          {stats.map((stat) => (
            <Link
              key={stat.label}
              href={stat.href}
              className="group rounded-lg border border-neutral-200 p-6 hover:shadow-lg transition-all hover:border-primary-300"
            >
              <div className="flex items-start justify-between mb-4">
                <div className={`text-primary-600 group-hover:scale-110 transition-transform`}>
                  {stat.icon}
                </div>
              </div>
              <p className="text-sm text-neutral-600 mb-2">{stat.label}</p>
              <p className="text-3xl font-bold text-primary-900">{stat.value}</p>
            </Link>
          ))}
        </div>

        {/* Welcome Section */}
        <div className="rounded-lg bg-gradient-to-r from-primary-50 to-accent-50 border border-primary-200 p-8">
          <h2 className="text-2xl font-bold text-primary-900 mb-2">
            Bienvenido a COR L.A.MA
          </h2>
          <p className="text-neutral-700 max-w-2xl">
            Sistema de administración de Capítulos para Validación de Certificados de
            Odómetro de Referencia. Gestiona eventos, miembros y validaciones desde este
            panel.
          </p>
        </div>

        {/* Quick Links */}
        <div className="grid gap-4 md:grid-cols-2 mt-8">
          <Link
            href="/admin/cor"
            className="rounded-lg border border-neutral-200 p-4 hover:shadow-md transition-shadow hover:border-primary-300"
          >
            <h3 className="font-semibold text-primary-900 mb-1">Validación COR</h3>
            <p className="text-sm text-neutral-600">
              Accede al flujo de validación de certificados
            </p>
          </Link>

          <Link
            href="/admin/queue"
            className="rounded-lg border border-neutral-200 p-4 hover:shadow-md transition-shadow hover:border-primary-300"
          >
            <h3 className="font-semibold text-primary-900 mb-1">Cola de Validación</h3>
            <p className="text-sm text-neutral-600">
              Gestiona la cola de validaciones pendientes
            </p>
          </Link>
        </div>
      </div>
    </div>
  );
}

export default function AdminPage() {
  return (
    <Suspense fallback={<Loading message="Cargando dashboard..." />}>
      <DashboardContent />
    </Suspense>
  );
}
