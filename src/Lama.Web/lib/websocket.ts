/**
 * WebSocket Service Layer
 * Gestiona conexiones en tiempo real para actualizaciones live
 */

type WebSocketEventType = 
  | 'ranking:update'
  | 'evidence:approved'
  | 'evidence:rejected'
  | 'evidence:new'
  | 'stats:update'
  | 'notification:new'
  | 'championship:update';

type WebSocketMessage = {
  type: WebSocketEventType;
  data: any;
  timestamp: number;
};

type WebSocketListener = (data: any) => void;

class WebSocketService {
  private ws: WebSocket | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 1000; // 1 segundo inicial
  private listeners: Map<WebSocketEventType, Set<WebSocketListener>> = new Map();
  private isIntentionallyClosed = false;
  private heartbeatInterval: NodeJS.Timeout | null = null;

  /**
   * Conectar al servidor WebSocket
   */
  connect(token?: string): void {
    if (this.ws?.readyState === WebSocket.OPEN) {
      console.log('WebSocket already connected');
      return;
    }

    this.isIntentionallyClosed = false;
    
    const wsUrl = process.env.NEXT_PUBLIC_WS_URL || 'ws://localhost:5000/ws';
    const urlWithToken = token ? `${wsUrl}?token=${token}` : wsUrl;

    try {
      this.ws = new WebSocket(urlWithToken);

      this.ws.onopen = this.handleOpen.bind(this);
      this.ws.onmessage = this.handleMessage.bind(this);
      this.ws.onerror = this.handleError.bind(this);
      this.ws.onclose = this.handleClose.bind(this);
    } catch (error) {
      console.error('Failed to create WebSocket:', error);
      this.scheduleReconnect();
    }
  }

  /**
   * Desconectar del servidor
   */
  disconnect(): void {
    this.isIntentionallyClosed = true;
    this.stopHeartbeat();
    
    if (this.ws) {
      this.ws.close();
      this.ws = null;
    }

    this.reconnectAttempts = 0;
  }

  /**
   * Subscribir a evento específico
   */
  subscribe(event: WebSocketEventType, listener: WebSocketListener): () => void {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, new Set());
    }

    this.listeners.get(event)!.add(listener);

    // Retornar función para desuscribirse
    return () => {
      this.listeners.get(event)?.delete(listener);
    };
  }

  /**
   * Enviar mensaje al servidor
   */
  send(type: WebSocketEventType, data: any): void {
    if (this.ws?.readyState === WebSocket.OPEN) {
      const message: WebSocketMessage = {
        type,
        data,
        timestamp: Date.now(),
      };
      this.ws.send(JSON.stringify(message));
    } else {
      console.warn('WebSocket not connected. Message not sent:', type);
    }
  }

  /**
   * Obtener estado de conexión
   */
  getState(): number {
    return this.ws?.readyState ?? WebSocket.CLOSED;
  }

  /**
   * Verificar si está conectado
   */
  isConnected(): boolean {
    return this.ws?.readyState === WebSocket.OPEN;
  }

  // Handlers privados

  private handleOpen(): void {
    console.log('WebSocket connected');
    this.reconnectAttempts = 0;
    this.reconnectDelay = 1000;
    this.startHeartbeat();
  }

  private handleMessage(event: MessageEvent): void {
    try {
      const message: WebSocketMessage = JSON.parse(event.data);
      
      // Notificar a listeners suscritos
      const listeners = this.listeners.get(message.type);
      if (listeners) {
        listeners.forEach(listener => listener(message.data));
      }
    } catch (error) {
      console.error('Failed to parse WebSocket message:', error);
    }
  }

  private handleError(error: Event): void {
    console.error('WebSocket error:', error);
  }

  private handleClose(event: CloseEvent): void {
    console.log('WebSocket closed:', event.code, event.reason);
    this.stopHeartbeat();

    if (!this.isIntentionallyClosed) {
      this.scheduleReconnect();
    }
  }

  private scheduleReconnect(): void {
    if (this.reconnectAttempts >= this.maxReconnectAttempts) {
      console.error('Max reconnection attempts reached');
      return;
    }

    const delay = this.reconnectDelay * Math.pow(2, this.reconnectAttempts);
    
    console.log(`Reconnecting in ${delay}ms... (attempt ${this.reconnectAttempts + 1})`);
    
    setTimeout(() => {
      this.reconnectAttempts++;
      this.connect();
    }, delay);
  }

  private startHeartbeat(): void {
    this.heartbeatInterval = setInterval(() => {
      if (this.ws?.readyState === WebSocket.OPEN) {
        this.ws.send(JSON.stringify({ type: 'ping' }));
      }
    }, 30000); // Ping cada 30 segundos
  }

  private stopHeartbeat(): void {
    if (this.heartbeatInterval) {
      clearInterval(this.heartbeatInterval);
      this.heartbeatInterval = null;
    }
  }
}

// Singleton instance
let wsServiceInstance: WebSocketService | null = null;

/**
 * Obtener instancia singleton del servicio WebSocket
 */
export function getWebSocketService(): WebSocketService {
  if (typeof window === 'undefined') {
    // Server-side rendering: retornar instancia dummy
    return new WebSocketService();
  }

  if (!wsServiceInstance) {
    wsServiceInstance = new WebSocketService();
  }

  return wsServiceInstance;
}

/**
 * Hook helper para conectar automáticamente
 */
export function connectWebSocket(token?: string): WebSocketService {
  const ws = getWebSocketService();
  ws.connect(token);
  return ws;
}

/**
 * Tipos de eventos exportados
 */
export type { WebSocketEventType, WebSocketMessage, WebSocketListener };
