/**
 * Custom Hooks para WebSocket
 * Facilita la integración de actualizaciones en tiempo real
 */

'use client';

import { useEffect, useState, useCallback, useRef } from 'react';
import { getWebSocketService, type WebSocketEventType, type WebSocketListener } from '@/lib/websocket';
import { useToken } from '@/lib/auth-context';

/**
 * Hook principal para conectar y gestionar WebSocket
 */
export function useWebSocket() {
  const token = useToken();
  const [isConnected, setIsConnected] = useState(false);
  const wsRef = useRef(getWebSocketService());

  useEffect(() => {
    const ws = wsRef.current;

    // Conectar con token si está disponible
    if (token) {
      ws.connect(token);
    } else {
      ws.connect();
    }

    // Verificar estado de conexión cada segundo
    const interval = setInterval(() => {
      setIsConnected(ws.isConnected());
    }, 1000);

    return () => {
      clearInterval(interval);
      ws.disconnect();
    };
  }, [token]);

  const send = useCallback((type: WebSocketEventType, data: any) => {
    wsRef.current.send(type, data);
  }, []);

  return {
    isConnected,
    send,
    ws: wsRef.current,
  };
}

/**
 * Hook para subscribirse a un evento específico
 */
export function useWebSocketEvent<T = any>(
  event: WebSocketEventType,
  handler: (data: T) => void
): void {
  const ws = getWebSocketService();

  useEffect(() => {
    const unsubscribe = ws.subscribe(event, handler as WebSocketListener);
    return unsubscribe;
  }, [event, handler, ws]);
}

/**
 * Hook para recibir actualizaciones de ranking en tiempo real
 */
export function useRealtimeRanking() {
  const [rankingData, setRankingData] = useState<any[]>([]);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);

  useWebSocketEvent('ranking:update', (data) => {
    setRankingData(data);
    setLastUpdate(new Date());
  });

  return { rankingData, lastUpdate };
}

/**
 * Hook para recibir notificaciones de evidencias
 */
export function useEvidenceNotifications() {
  const [notifications, setNotifications] = useState<any[]>([]);

  const handleEvidenceApproved = useCallback((data: any) => {
    setNotifications(prev => [...prev, {
      type: 'approved',
      message: `Tu evidencia "${data.eventName}" fue aprobada`,
      data,
      timestamp: Date.now(),
    }]);
  }, []);

  const handleEvidenceRejected = useCallback((data: any) => {
    setNotifications(prev => [...prev, {
      type: 'rejected',
      message: `Tu evidencia "${data.eventName}" fue rechazada`,
      data,
      timestamp: Date.now(),
    }]);
  }, []);

  const handleNewEvidence = useCallback((data: any) => {
    setNotifications(prev => [...prev, {
      type: 'new',
      message: `Nueva evidencia subida por ${data.memberName}`,
      data,
      timestamp: Date.now(),
    }]);
  }, []);

  useWebSocketEvent('evidence:approved', handleEvidenceApproved);
  useWebSocketEvent('evidence:rejected', handleEvidenceRejected);
  useWebSocketEvent('evidence:new', handleNewEvidence);

  const clearNotifications = useCallback(() => {
    setNotifications([]);
  }, []);

  const removeNotification = useCallback((index: number) => {
    setNotifications(prev => prev.filter((_, i) => i !== index));
  }, []);

  return {
    notifications,
    clearNotifications,
    removeNotification,
    hasNotifications: notifications.length > 0,
  };
}

/**
 * Hook para recibir actualizaciones de stats en tiempo real
 */
export function useRealtimeStats() {
  const [stats, setStats] = useState<any>(null);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);

  useWebSocketEvent('stats:update', (data) => {
    setStats(data);
    setLastUpdate(new Date());
  });

  return { stats, lastUpdate };
}

/**
 * Hook para recibir actualizaciones de campeonato
 */
export function useRealtimeChampionship(championshipId?: string) {
  const [championship, setChampionship] = useState<any>(null);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);

  useWebSocketEvent('championship:update', (data) => {
    // Solo actualizar si es el campeonato correcto
    if (!championshipId || data.id === championshipId) {
      setChampionship(data);
      setLastUpdate(new Date());
    }
  });

  return { championship, lastUpdate };
}

/**
 * Hook para notificaciones generales
 */
export function useNotifications() {
  const [notifications, setNotifications] = useState<any[]>([]);

  useWebSocketEvent('notification:new', (data) => {
    setNotifications(prev => [...prev, {
      ...data,
      timestamp: Date.now(),
      read: false,
    }]);
  });

  const markAsRead = useCallback((index: number) => {
    setNotifications(prev =>
      prev.map((notif, i) =>
        i === index ? { ...notif, read: true } : notif
      )
    );
  }, []);

  const markAllAsRead = useCallback(() => {
    setNotifications(prev =>
      prev.map(notif => ({ ...notif, read: true }))
    );
  }, []);

  const clearAll = useCallback(() => {
    setNotifications([]);
  }, []);

  const unreadCount = notifications.filter(n => !n.read).length;

  return {
    notifications,
    unreadCount,
    markAsRead,
    markAllAsRead,
    clearAll,
  };
}

/**
 * Hook para estado de conexión WebSocket
 */
export function useWebSocketStatus() {
  const [isConnected, setIsConnected] = useState(false);
  const ws = getWebSocketService();

  useEffect(() => {
    const interval = setInterval(() => {
      setIsConnected(ws.isConnected());
    }, 1000);

    return () => clearInterval(interval);
  }, [ws]);

  return { isConnected };
}
