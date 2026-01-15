'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/lib/auth-context';
import { IconChevronDown, IconLogOut, IconSettings, IconUser } from '@/components/icons';
import { Button } from '@/components/ui/button';

/**
 * Menu de usuario autenticado
 * Muestra información del usuario y opciones de logout
 */
export function UserMenu() {
  const router = useRouter();
  const { user, logout, isAuthenticated } = useAuth();
  const [isOpen, setIsOpen] = useState(false);

  if (!isAuthenticated || !user) {
    return (
      <Link href="/login">
        <Button className="bg-purple-600 hover:bg-purple-700 text-white">
          Iniciar sesión
        </Button>
      </Link>
    );
  }

  const handleLogout = async () => {
    await logout();
    router.push('/login');
  };

  const initials = user.name
    ?.split(' ')
    .map((n: string) => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2) || 'U';

  return (
    <div className="relative">
      {/* User Avatar Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-slate-700 text-slate-200 transition"
      >
        {/* Avatar */}
        <div className="w-8 h-8 rounded-full bg-purple-600 flex items-center justify-center text-white text-sm font-medium">
          {initials}
        </div>

        {/* User info (desktop) */}
        <div className="hidden sm:block text-left">
          <p className="text-sm font-medium text-white">{user.name}</p>
          <p className="text-xs text-slate-400">{user.email}</p>
        </div>

        {/* Chevron */}
        <IconChevronDown
          size={16}
          className={`text-slate-400 transition ${isOpen ? 'rotate-180' : ''}`}
        />
      </button>

      {/* Dropdown Menu */}
      {isOpen && (
        <>
          {/* Overlay para cerrar */}
          <div
            className="fixed inset-0 z-40"
            onClick={() => setIsOpen(false)}
          />

          {/* Menu */}
          <div className="absolute right-0 mt-2 w-48 bg-slate-800 border border-slate-700 rounded-lg shadow-xl z-50">
            {/* User Info */}
            <div className="px-4 py-3 border-b border-slate-700">
              <p className="text-sm font-medium text-white">{user.name}</p>
              <p className="text-xs text-slate-400">{user.email}</p>
              {user.class && (
                <p className="text-xs text-purple-400 mt-1">Clase: {user.class}</p>
              )}
            </div>

            {/* Menu Items */}
            <nav className="p-2 space-y-1">
              <Link href="/member/profile">
                <button className="w-full flex items-center gap-2 px-3 py-2 text-sm text-slate-300 hover:bg-slate-700 rounded transition">
                  <IconUser size={16} />
                  Mi perfil
                </button>
              </Link>

              <Link href="/member/dashboard">
                <button className="w-full flex items-center gap-2 px-3 py-2 text-sm text-slate-300 hover:bg-slate-700 rounded transition">
                  <IconSettings size={16} />
                  Panel de control
                </button>
              </Link>

              {user.role === 'admin' && (
                <Link href="/admin/cor">
                  <button className="w-full flex items-center gap-2 px-3 py-2 text-sm text-slate-300 hover:bg-slate-700 rounded transition">
                    <IconSettings size={16} />
                    Admin COR
                  </button>
                </Link>
              )}
            </nav>

            {/* Logout */}
            <div className="border-t border-slate-700 p-2">
              <button
                onClick={handleLogout}
                className="w-full flex items-center gap-2 px-3 py-2 text-sm text-red-400 hover:bg-slate-700 rounded transition"
              >
                <IconLogOut size={16} />
                Cerrar sesión
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
