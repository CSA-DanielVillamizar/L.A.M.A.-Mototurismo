'use client';

import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';

/**
 * Página de acceso denegado
 */
export default function UnauthorizedPage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        <Card className="bg-slate-800/50 border-slate-700 backdrop-blur">
          <div className="p-12 text-center space-y-6">
            {/* Icon */}
            <div className="text-6xl">🔐</div>

            {/* Title */}
            <div>
              <h1 className="text-3xl font-bold text-white mb-2">
                Acceso Denegado
              </h1>
              <p className="text-slate-400">
                No tienes permiso para acceder a esta página
              </p>
            </div>

            {/* Description */}
            <p className="text-slate-300 text-sm">
              Si crees que esto es un error, por favor contacta al administrador.
            </p>

            {/* Actions */}
            <div className="flex gap-3 flex-col sm:flex-row">
              <Link href="/member/dashboard" className="flex-1">
                <Button className="w-full bg-purple-600 hover:bg-purple-700 text-white">
                  Ir al Dashboard
                </Button>
              </Link>
              <Link href="/" className="flex-1">
                <Button variant="outline" className="w-full border-slate-600 text-slate-200 hover:bg-slate-700">
                  Ir al Inicio
                </Button>
              </Link>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
