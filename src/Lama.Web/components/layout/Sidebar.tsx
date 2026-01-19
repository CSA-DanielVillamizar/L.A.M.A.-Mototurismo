'use client';

/**
 * Sidebar Component
 * Navegación lateral collapsible con menú basado en roles
 */

import React from 'react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  ChevronLeft,
  LayoutDashboard,
  CheckCircle2,
  ListChecks,
  Calendar,
  Users,
  Trophy,
  BadgeDollarSign,
  UploadCloud,
  Settings,
} from 'lucide-react';

interface SidebarProps {
  isOpen: boolean;
  onToggle: () => void;
}

const menuItems = [
  {
    icon: LayoutDashboard,
    label: 'Panel de Control',
    href: '/admin',
    roles: ['admin', 'staff'],
  },
  {
    icon: CheckCircle2,
    label: 'Validar COR',
    href: '/admin/cor',
    roles: ['admin', 'staff'],
  },
  {
    icon: ListChecks,
    label: 'Cola de Espera',
    href: '/admin/queue',
    roles: ['admin', 'staff'],
  },
  {
    icon: Calendar,
    label: 'Eventos',
    href: '/admin/events',
    roles: ['admin'],
  },
  {
    icon: Users,
    label: 'Usuarios',
    href: '/admin/members',
    roles: ['admin'],
  },


  {
    icon: UploadCloud,
    label: 'Reportes',
    href: '/admin/reports',
    roles: ['admin'],
  },

];

export function Sidebar({ isOpen, onToggle }: SidebarProps) {
  const pathname = usePathname();

  return (
    <>
      {/* Overlay for mobile */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-gray-900/50 lg:hidden"
          onClick={onToggle}
          aria-hidden="true"
        />
      )}

      {/* Sidebar */}
      <aside
        className={`fixed left-0 top-0 z-50 h-screen w-64 transform bg-white transition-transform duration-300 ease-in-out lg:relative lg:translate-x-0 ${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        {/* Header */}
        <div className="flex items-center justify-between border-b border-gray-200 px-6 py-4">
          <div className="text-lg font-bold text-gray-900">LAMA</div>
          <button
            onClick={onToggle}
            className="rounded-lg hover:bg-gray-100 p-1 lg:hidden"
            aria-label="Cerrar menú"
          >
            <ChevronLeft className="h-5 w-5 text-gray-600" />
          </button>
        </div>

        {/* Navigation */}
        <nav className="space-y-1 px-4 py-6">
          {menuItems.map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href;

            return (
              <Link
                key={item.href}
                href={item.href}
                className={`flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium transition-colors ${
                  isActive
                    ? 'bg-indigo-50 text-indigo-600'
                    : 'text-gray-700 hover:bg-gray-50'
                }`}
                aria-current={isActive ? 'page' : undefined}
              >
                <Icon className="h-5 w-5 flex-shrink-0" aria-hidden="true" />
                <span>{item.label}</span>
              </Link>
            );
          })}
        </nav>

        {/* Footer */}
        <div className="absolute bottom-0 left-0 right-0 border-t border-gray-200 bg-gray-50 p-4">
          <p className="text-xs text-gray-600">
            LAMA v1.0 • Sistema de Gestión
          </p>
        </div>
      </aside>
    </>
  );
}
