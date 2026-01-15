'use client';

import { useState } from 'react';
import { useNotifications } from '@/hooks/useWebSocket';
import { IconNotifications } from '@/components/icons';
import { Badge } from '@/components/ui/badge';

/**
 * Campana de notificaciones con contador
 */
export function NotificationBell() {
  const { notifications, unreadCount, markAsRead, markAllAsRead, clearAll } = useNotifications();
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="relative">
      {/* Bell Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="relative p-2 rounded-lg hover:bg-slate-700 transition"
      >
        <IconNotifications size={20} className="text-slate-300" />
        
        {unreadCount > 0 && (
          <Badge
            variant="destructive"
            className="absolute -top-1 -right-1 w-5 h-5 flex items-center justify-center text-xs p-0"
          >
            {unreadCount > 9 ? '9+' : unreadCount}
          </Badge>
        )}
      </button>

      {/* Dropdown */}
      {isOpen && (
        <>
          {/* Overlay */}
          <div
            className="fixed inset-0 z-40"
            onClick={() => setIsOpen(false)}
          />

          {/* Dropdown Menu */}
          <div className="absolute right-0 mt-2 w-80 bg-slate-800 border border-slate-700 rounded-lg shadow-xl z-50 max-h-96 overflow-y-auto">
            {/* Header */}
            <div className="p-4 border-b border-slate-700 flex items-center justify-between">
              <h3 className="font-semibold text-white">Notificaciones</h3>
              {notifications.length > 0 && (
                <div className="flex gap-2">
                  <button
                    onClick={markAllAsRead}
                    className="text-xs text-purple-400 hover:text-purple-300"
                  >
                    Marcar todas
                  </button>
                  <button
                    onClick={clearAll}
                    className="text-xs text-slate-400 hover:text-slate-300"
                  >
                    Limpiar
                  </button>
                </div>
              )}
            </div>

            {/* Notifications List */}
            <div className="divide-y divide-slate-700">
              {notifications.length === 0 ? (
                <div className="p-8 text-center text-slate-400">
                  <IconNotifications size={32} className="mx-auto mb-2 opacity-50" />
                  <p className="text-sm">No hay notificaciones</p>
                </div>
              ) : (
                notifications.map((notif, index) => (
                  <div
                    key={index}
                    onClick={() => markAsRead(index)}
                    className={`p-3 hover:bg-slate-700 cursor-pointer ${
                      !notif.read ? 'bg-slate-700/50' : ''
                    }`}
                  >
                    <div className="flex items-start gap-2">
                      {!notif.read && (
                        <div className="w-2 h-2 bg-purple-500 rounded-full mt-1.5" />
                      )}
                      <div className="flex-1">
                        <p className="text-sm text-white">{notif.message}</p>
                        {notif.description && (
                          <p className="text-xs text-slate-400 mt-1">
                            {notif.description}
                          </p>
                        )}
                        <p className="text-xs text-slate-500 mt-1">
                          {new Date(notif.timestamp).toLocaleString('es-ES', {
                            hour: '2-digit',
                            minute: '2-digit',
                          })}
                        </p>
                      </div>
                    </div>
                  </div>
                ))
              )}
            </div>
          </div>
        </>
      )}
    </div>
  );
}
