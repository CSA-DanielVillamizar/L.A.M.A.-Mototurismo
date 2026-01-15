'use client';

import { useState } from 'react';
import Link from 'next/link';
import { IconMenu, IconClose, IconChevronDown, IconLogOut, IconSettings, IconUser } from '@/components/icons';
import { cn } from '@/lib/utils';

const navigationItems = [
  { label: 'Dashboard', href: '/admin', icon: '📊' },
  { label: 'COR Admin', href: '/admin/cor', icon: '🏆' },
  { label: 'Eventos', href: '/admin/events', icon: '📅' },
  { label: 'Miembros', href: '/admin/members', icon: '👥' },
  { label: 'Reportes', href: '/admin/reports', icon: '📈' },
];

/**
 * Sidebar colapsable con navegación principal
 * Diseño responsive: colapsado en mobile, expandido en desktop
 */
export function Sidebar() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      {/* Botón Hamburger (Mobile) */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="fixed top-4 left-4 z-40 rounded-lg bg-primary-700 p-2 text-white md:hidden"
        aria-label="Toggle menu"
      >
        {isOpen ? <IconClose size={24} /> : <IconMenu size={24} />}
      </button>

      {/* Overlay (Mobile) */}
      {isOpen && (
        <div
          className="fixed inset-0 z-30 bg-black/50 md:hidden"
          onClick={() => setIsOpen(false)}
        />
      )}

      {/* Sidebar */}
      <aside
        className={cn(
          'fixed left-0 top-0 h-screen w-64 bg-primary-900 text-white transition-transform duration-300 ease-out md:static md:translate-x-0',
          isOpen ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        {/* Logo / Branding */}
        <div className="border-b border-primary-800 px-6 py-8">
          <Link href="/" className="flex items-center gap-2 font-bold text-xl">
            <span className="text-2xl">🏍️</span>
            <span>LAMA COR</span>
          </Link>
        </div>

        {/* Navigation */}
        <nav className="space-y-1 px-3 py-6">
          {navigationItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className="group relative flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium transition-colors hover:bg-primary-800 active:bg-primary-700"
              onClick={() => setIsOpen(false)}
            >
              <span className="text-lg">{item.icon}</span>
              <span>{item.label}</span>
            </Link>
          ))}
        </nav>

        {/* User Menu at Bottom */}
        <div className="absolute bottom-0 left-0 right-0 border-t border-primary-800 px-3 py-4 space-y-1">
          <button className="w-full flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium transition-colors hover:bg-primary-800 active:bg-primary-700">
            <IconSettings size={18} />
            <span>Configuración</span>
          </button>
          <button className="w-full flex items-center gap-3 rounded-lg px-4 py-3 text-sm font-medium transition-colors hover:bg-primary-800 active:bg-primary-700 text-danger-300">
            <IconLogOut size={18} />
            <span>Cerrar Sesión</span>
          </button>
        </div>
      </aside>
    </>
  );
}

/**
 * Topbar con información del usuario y controles rápidos
 */
export function Topbar() {
  const [showUserMenu, setShowUserMenu] = useState(false);

  return (
    <header className="sticky top-0 z-20 border-b border-neutral-200 bg-white ml-0 md:ml-64">
      <div className="flex items-center justify-between px-6 py-4">
        {/* Spacer para mobile */}
        <div className="md:hidden w-12" />

        {/* Title */}
        <div>
          <h1 className="hidden md:block text-lg font-semibold text-primary-900">
            COR Admin Panel
          </h1>
        </div>

        {/* User Menu */}
        <div className="relative">
          <button
            onClick={() => setShowUserMenu(!showUserMenu)}
            className="flex items-center gap-2 rounded-lg px-3 py-2 hover:bg-neutral-100 transition-colors"
            aria-label="User menu"
          >
            <div className="h-8 w-8 rounded-full bg-primary-600 flex items-center justify-center text-white text-sm font-bold">
              DV
            </div>
            <span className="hidden md:block text-sm font-medium text-primary-900">
              Daniel
            </span>
            <IconChevronDown size={16} className="hidden md:block" />
          </button>

          {/* User Dropdown */}
          {showUserMenu && (
            <div className="absolute right-0 mt-2 w-48 rounded-lg bg-white border border-neutral-200 shadow-lg">
              <div className="px-4 py-3 border-b border-neutral-200">
                <p className="text-sm font-medium text-primary-900">Daniel Villamizar</p>
                <p className="text-xs text-neutral-500">MTO Admin</p>
              </div>
              <button className="w-full text-left px-4 py-2 text-sm text-primary-700 hover:bg-neutral-50 transition-colors">
                Mi Perfil
              </button>
              <button className="w-full text-left px-4 py-2 text-sm text-danger-600 hover:bg-neutral-50 transition-colors border-t border-neutral-200">
                Cerrar Sesión
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}

/**
 * Breadcrumbs para navegación contextual
 */
export function Breadcrumbs({ items }: { items: { label: string; href?: string }[] }) {
  return (
    <nav className="flex items-center gap-2 text-sm px-6 py-3 border-b border-neutral-200 ml-0 md:ml-64 bg-white" aria-label="Breadcrumb">
      {items.map((item, index) => (
        <div key={index} className="flex items-center gap-2">
          {index > 0 && <span className="text-neutral-400">/</span>}
          {item.href ? (
            <Link href={item.href} className="text-primary-600 hover:text-primary-700 transition-colors">
              {item.label}
            </Link>
          ) : (
            <span className="text-neutral-600">{item.label}</span>
          )}
        </div>
      ))}
    </nav>
  );
}
