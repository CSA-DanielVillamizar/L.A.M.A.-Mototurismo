'use client';

import { useWebSocketStatus } from '@/hooks/useWebSocket';

/**
 * Indicador visual del estado de conexión WebSocket
 */
export function RealtimeIndicator() {
  const { isConnected } = useWebSocketStatus();

  return (
    <div className="flex items-center gap-2">
      <div
        className={`w-2 h-2 rounded-full ${
          isConnected ? 'bg-green-500 animate-pulse' : 'bg-red-500'
        }`}
      />
      <span className="text-xs text-slate-400">
        {isConnected ? 'Conectado' : 'Desconectado'}
      </span>
    </div>
  );
}
