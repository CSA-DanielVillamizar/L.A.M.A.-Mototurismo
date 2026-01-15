'use client';

import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import Link from 'next/link';

/**
 * Página offline para PWA
 */
export default function OfflinePage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center p-4">
      <Card className="bg-slate-800/50 border-slate-700 backdrop-blur max-w-md">
        <div className="p-12 text-center space-y-6">
          {/* Icon */}
          <div className="text-6xl mb-4">📡</div>

          {/* Title */}
          <div>
            <h1 className="text-3xl font-bold text-white mb-2">
              Sin conexión
            </h1>
            <p className="text-slate-400">
              No hay conexión a internet en este momento
            </p>
          </div>

          {/* Description */}
          <p className="text-slate-300 text-sm">
            Algunas funciones pueden estar limitadas. Intenta conectarte a una red wifi o datos móviles.
          </p>

          {/* Actions */}
          <div className="space-y-2">
            <Button
              onClick={() => window.location.reload()}
              className="w-full bg-purple-600 hover:bg-purple-700 text-white"
            >
              Reintentar
            </Button>

            <Link href="/member/dashboard">
              <Button
                variant="outline"
                className="w-full border-slate-600 text-slate-200 hover:bg-slate-700"
              >
                Ver contenido guardado
              </Button>
            </Link>
          </div>

          {/* Info */}
          <p className="text-xs text-slate-500 mt-6">
            El contenido guardado estará disponible sin conexión
          </p>
        </div>
      </Card>
    </div>
  );
}
