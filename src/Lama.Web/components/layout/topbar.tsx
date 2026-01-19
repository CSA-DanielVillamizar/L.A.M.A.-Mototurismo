'use client';

/**
 * Topbar Component
 * Barra superior con menú, breadcrumbs y menú de usuario
 */

import React from 'react';
import { Menu, Bell } from 'lucide-react';
import { UserMenu } from './UserMenu';

interface TopbarProps {
  onMenuToggle: () => void;
}

export function Topbar({
  onMenuToggle,
}: TopbarProps) {
  return (
    <header className="sticky top-0 z-40 border-b border-gray-200 bg-white">
      <div className="mx-auto flex items-center justify-between px-4 py-4 sm:px-6 lg:px-8">
        {/* Left: Menu Toggle */}
        <button
          onClick={onMenuToggle}
          className="rounded-lg p-2 hover:bg-gray-100 lg:hidden"
          aria-label="Alternar menú"
        >
          <Menu className="h-6 w-6 text-gray-600" />
        </button>

        {/* Center: Breadcrumbs (optional - can be filled dynamically) */}
        <div className="flex-1 px-4">
          <div className="text-sm text-gray-600"></div>
        </div>

        {/* Right: Actions + User Menu */}
        <div className="flex items-center gap-4">
          {/* Notifications */}
          <button
            className="rounded-lg p-2 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            aria-label="Notificaciones"
          >
            <Bell className="h-5 w-5 text-gray-600" />
          </button>

          {/* User Menu */}
          <UserMenu />
        </div>
      </div>
    </header>
  );
}
