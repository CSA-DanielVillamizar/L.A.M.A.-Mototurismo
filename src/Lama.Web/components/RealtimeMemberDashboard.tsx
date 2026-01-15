'use client';

import { useEffect, useState } from 'react';
import { useRealtimeStats, useEvidenceNotifications } from '@/hooks/useWebSocket';
import { Card } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Alert } from '@/components/ui/alert';
import { IconCalendar, IconSuccess } from '@/components/icons';

/**
 * Dashboard de miembro con actualizaciones en tiempo real
 */
export function RealtimeMemberDashboard() {
  const { stats, lastUpdate } = useRealtimeStats();
  const { notifications, removeNotification } = useEvidenceNotifications();
  const [localStats, setLocalStats] = useState({
    points: 125,
    rank: 12,
    evidences: 8,
    nextEvent: 'Rally Boyacá 2024',
  });

  // Actualizar stats locales cuando lleguen actualizaciones en tiempo real
  useEffect(() => {
    if (stats) {
      setLocalStats(stats);
    }
  }, [stats]);

  return (
    <div className="space-y-6">
      {/* Indicador de última actualización */}
      {lastUpdate && (
        <div className="text-xs text-slate-400 flex items-center gap-2">
          <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse" />
          Última actualización: {lastUpdate.toLocaleTimeString('es-ES')}
        </div>
      )}

      {/* Notificaciones de evidencias */}
      {notifications.length > 0 && (
        <div className="space-y-2">
          {notifications.map((notif, index) => (
            <Alert
              key={index}
              variant={notif.type === 'approved' ? 'default' : 'destructive'}
              className="flex items-center justify-between"
            >
              <p className="text-sm">{notif.message}</p>
              <button
                onClick={() => removeNotification(index)}
                className="text-xs underline"
              >
                Cerrar
              </button>
            </Alert>
          ))}
        </div>
      )}

      {/* Cards de stats con animación en updates */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card className="p-6 bg-gradient-to-br from-purple-900 to-purple-800 border-purple-700">
          <div className="flex items-center justify-between mb-2">
            <span className="text-2xl">🏆</span>
            {lastUpdate && (
              <Badge className="bg-green-500 text-white text-xs">
                Actualizado
              </Badge>
            )}
          </div>
          <p className="text-3xl font-bold text-white animate-pulse">
            {localStats.points} pts
          </p>
          <p className="text-sm text-purple-200">Puntos totales</p>
        </Card>

        <Card className="p-6 bg-slate-800 border-slate-700">
          <div className="flex items-center justify-between mb-2">
            <span className="text-2xl">🏆</span>
          </div>
          <p className="text-3xl font-bold text-white">#{localStats.rank}</p>
          <p className="text-sm text-slate-400">Posición actual</p>
        </Card>

        <Card className="p-6 bg-slate-800 border-slate-700">
          <div className="flex items-center justify-between mb-2">
            <IconSuccess size={24} className="text-green-500" />
          </div>
          <p className="text-3xl font-bold text-white">{localStats.evidences}</p>
          <p className="text-sm text-slate-400">Evidencias aprobadas</p>
        </Card>
      </div>

      {/* Próximo evento */}
      <Card className="p-6 bg-slate-800 border-slate-700">
        <div className="flex items-center gap-3 mb-4">
          <IconCalendar size={24} className="text-purple-500" />
          <h3 className="text-lg font-semibold text-white">Próximo evento</h3>
        </div>
        <p className="text-slate-300">{localStats.nextEvent}</p>
        <p className="text-sm text-slate-400 mt-2">15 de Febrero, 2024</p>
      </Card>
    </div>
  );
}
