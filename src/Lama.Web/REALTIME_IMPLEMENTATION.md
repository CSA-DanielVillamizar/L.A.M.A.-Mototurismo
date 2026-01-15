# Real-time Updates Implementation - ETAPA 8 Phase 2

## 📋 Overview

Sistema completo de actualizaciones en tiempo real usando WebSockets para L.A.M.A. Mototurismo.

**Files Created:**
- `lib/websocket.ts` (320+ lines) - WebSocket service layer
- `hooks/useWebSocket.ts` (280+ lines) - Custom hooks para real-time data
- `components/RealtimeIndicator.tsx` (30+ lines) - Connection status indicator
- `components/NotificationBell.tsx` (120+ lines) - Notification bell with counter
- `components/RealtimeMemberDashboard.tsx` (130+ lines) - Example integration

**Total Lines of Code:** 880+ lines

---

## 🏗️ Architecture

### 1. **WebSocket Service Layer** (`lib/websocket.ts`)

Servicio singleton que gestiona la conexión WebSocket:

```typescript
class WebSocketService {
  // Connection management
  connect(token?: string): void
  disconnect(): void
  
  // Event subscription
  subscribe(event: WebSocketEventType, listener: WebSocketListener): () => void
  
  // Send messages
  send(type: WebSocketEventType, data: any): void
  
  // Connection status
  isConnected(): boolean
  getState(): number
}
```

**Key Features:**
- ✅ Automatic reconnection with exponential backoff
- ✅ Heartbeat mechanism (ping every 30s)
- ✅ Token-based authentication
- ✅ Event-based pub/sub pattern
- ✅ Singleton pattern for global state
- ✅ Max 5 reconnection attempts
- ✅ Graceful disconnect handling

**Event Types:**
```typescript
type WebSocketEventType = 
  | 'ranking:update'
  | 'evidence:approved'
  | 'evidence:rejected'
  | 'evidence:new'
  | 'stats:update'
  | 'notification:new'
  | 'championship:update'
```

---

### 2. **Custom Hooks** (`hooks/useWebSocket.ts`)

Hooks React para fácil integración:

#### `useWebSocket()`
Hook principal para conectar y gestionar WebSocket:
```typescript
const { isConnected, send, ws } = useWebSocket();
```

#### `useWebSocketEvent<T>(event, handler)`
Subscribir a eventos específicos:
```typescript
useWebSocketEvent('ranking:update', (data) => {
  console.log('Ranking updated:', data);
});
```

#### `useRealtimeRanking()`
Recibir actualizaciones de ranking en tiempo real:
```typescript
const { rankingData, lastUpdate } = useRealtimeRanking();
```

#### `useEvidenceNotifications()`
Notificaciones de evidencias:
```typescript
const { 
  notifications, 
  clearNotifications, 
  removeNotification, 
  hasNotifications 
} = useEvidenceNotifications();
```

#### `useRealtimeStats()`
Estadísticas en tiempo real:
```typescript
const { stats, lastUpdate } = useRealtimeStats();
```

#### `useRealtimeChampionship(id)`
Actualizaciones de campeonato:
```typescript
const { championship, lastUpdate } = useRealtimeChampionship('2024');
```

#### `useNotifications()`
Sistema de notificaciones generales:
```typescript
const {
  notifications,
  unreadCount,
  markAsRead,
  markAllAsRead,
  clearAll
} = useNotifications();
```

#### `useWebSocketStatus()`
Estado de conexión:
```typescript
const { isConnected } = useWebSocketStatus();
```

---

### 3. **UI Components**

#### `RealtimeIndicator`
Indicador visual de estado de conexión:
```tsx
<RealtimeIndicator />
// Muestra: 🟢 Conectado | 🔴 Desconectado
```

#### `NotificationBell`
Campana de notificaciones con contador:
```tsx
<NotificationBell />
// Badge con número de notificaciones sin leer
// Dropdown con lista de notificaciones
```

**Features:**
- ✅ Badge con contador de no leídas
- ✅ Dropdown con lista completa
- ✅ Marcar como leída
- ✅ Marcar todas como leídas
- ✅ Limpiar todas
- ✅ Timestamp de cada notificación

#### `RealtimeMemberDashboard`
Dashboard con actualizaciones en tiempo real:
```tsx
<RealtimeMemberDashboard />
```

**Features:**
- ✅ Stats cards con animación en update
- ✅ Notificaciones de evidencias inline
- ✅ Indicador de última actualización
- ✅ Badge "Actualizado" en cards con cambios

---

## 🔄 Message Flow

### Client → Server
```typescript
// Enviar mensaje
const { send } = useWebSocket();
send('ranking:update', { memberId: '123' });
```

### Server → Client
```typescript
// Recibir mensaje
useWebSocketEvent('ranking:update', (data) => {
  // data = { rank: 10, points: 250 }
  updateRanking(data);
});
```

---

## 🎯 Use Cases

### 1. Live Ranking Updates
```typescript
function RankingTable() {
  const { rankingData, lastUpdate } = useRealtimeRanking();
  
  return (
    <div>
      <p>Última actualización: {lastUpdate?.toLocaleTimeString()}</p>
      <table>
        {rankingData.map(member => (
          <tr key={member.id}>
            <td>{member.rank}</td>
            <td>{member.name}</td>
            <td>{member.points}</td>
          </tr>
        ))}
      </table>
    </div>
  );
}
```

### 2. Evidence Approval Notifications
```typescript
function EvidencesList() {
  const { notifications } = useEvidenceNotifications();
  
  return (
    <div>
      {notifications.map(notif => (
        <Alert variant={notif.type === 'approved' ? 'success' : 'error'}>
          {notif.message}
        </Alert>
      ))}
    </div>
  );
}
```

### 3. Real-time Stats Dashboard
```typescript
function Dashboard() {
  const { stats } = useRealtimeStats();
  
  return (
    <div>
      <Card>
        <h3>Puntos: {stats?.points}</h3>
        <p>Posición: #{stats?.rank}</p>
      </Card>
    </div>
  );
}
```

### 4. Notification Bell in Header
```typescript
function Header() {
  return (
    <header>
      <NotificationBell />
      <RealtimeIndicator />
    </header>
  );
}
```

---

## 🔐 Backend Integration

### WebSocket Server Expected Behavior

**Connection:**
```
Client → ws://localhost:5000/ws?token={jwt_token}
Server → Accept connection
Server → Start sending events
```

**Message Format:**
```typescript
interface WebSocketMessage {
  type: WebSocketEventType;
  data: any;
  timestamp: number;
}
```

**Example Messages:**

**Ranking Update:**
```json
{
  "type": "ranking:update",
  "data": [
    { "rank": 1, "memberId": "123", "name": "Juan", "points": 500 },
    { "rank": 2, "memberId": "456", "name": "María", "points": 450 }
  ],
  "timestamp": 1704067200000
}
```

**Evidence Approved:**
```json
{
  "type": "evidence:approved",
  "data": {
    "evidenceId": "ev-123",
    "eventName": "Rally Boyacá",
    "points": 50,
    "memberId": "123"
  },
  "timestamp": 1704067200000
}
```

**Stats Update:**
```json
{
  "type": "stats:update",
  "data": {
    "points": 125,
    "rank": 12,
    "evidences": 8,
    "nextEvent": "Rally Boyacá 2024"
  },
  "timestamp": 1704067200000
}
```

**Notification:**
```json
{
  "type": "notification:new",
  "data": {
    "message": "Tienes una nueva evidencia por revisar",
    "description": "Rally Bogotá - Juan Pérez",
    "link": "/admin/evidences/123"
  },
  "timestamp": 1704067200000
}
```

---

## 🧪 Testing

### Manual Testing

1. **Connect to WebSocket:**
```typescript
const { isConnected } = useWebSocket();
// Should show: 🟢 Conectado
```

2. **Send Test Message:**
```typescript
const { send } = useWebSocket();
send('ranking:update', { test: true });
```

3. **Subscribe to Events:**
```typescript
useWebSocketEvent('ranking:update', (data) => {
  console.log('Received:', data);
});
```

4. **Test Reconnection:**
- Disconnect server
- Client should attempt reconnection (max 5 times)
- Should show: 🔴 Desconectado
- Reconnect server
- Should auto-reconnect and show: 🟢 Conectado

---

## 🔧 Configuration

### Environment Variables

Add to `.env.local`:

```env
# WebSocket Server URL
NEXT_PUBLIC_WS_URL=ws://localhost:5000/ws

# Or production:
# NEXT_PUBLIC_WS_URL=wss://api.lama-mototurismo.com/ws
```

### WebSocket Server Configuration

Expected backend setup:
- URL: `ws://localhost:5000/ws` (dev) or `wss://...` (prod)
- Authentication: JWT token via query parameter `?token={jwt}`
- Heartbeat: Respond to `ping` with `pong`
- Auto-disconnect: After 60s of inactivity

---

## 📊 Performance Considerations

### Connection Management
- ✅ Single WebSocket connection per client (singleton)
- ✅ Automatic reconnection with exponential backoff
- ✅ Heartbeat to keep connection alive
- ✅ Clean disconnect on unmount

### Message Handling
- ✅ Event-based subscription (no polling)
- ✅ Selective updates (only subscribed events)
- ✅ Optimistic UI updates
- ✅ Minimal re-renders

### Memory Management
- ✅ Automatic cleanup on unmount
- ✅ Unsubscribe functions for all hooks
- ✅ No memory leaks
- ✅ Proper ref usage

---

## 🎯 Integration Checklist

- ✅ WebSocket service layer created
- ✅ 8 custom hooks for real-time data
- ✅ RealtimeIndicator component
- ✅ NotificationBell component
- ✅ Example dashboard integration
- ⏳ Backend WebSocket server (pending)
- ⏳ Integration in existing components
- ⏳ Production testing

---

## 🚀 Next Steps

### Phase 2 Completion Tasks:

1. **Integrate into existing components:**
   - Add `<RealtimeIndicator />` to Topbar
   - Add `<NotificationBell />` to Header
   - Update `MemberRanking` to use `useRealtimeRanking()`
   - Update `MemberDashboard` to use `useRealtimeStats()`

2. **Backend implementation:**
   - Implement WebSocket server in Lama.API
   - Set up event broadcasting
   - Implement authentication
   - Add heartbeat/ping-pong

3. **Testing:**
   - Test all event types
   - Test reconnection logic
   - Test with multiple clients
   - Performance testing

---

## 📝 Files Summary

| File | Lines | Purpose |
|------|-------|---------|
| lib/websocket.ts | 320+ | WebSocket service layer |
| hooks/useWebSocket.ts | 280+ | Custom hooks |
| components/RealtimeIndicator.tsx | 30+ | Connection indicator |
| components/NotificationBell.tsx | 120+ | Notification UI |
| components/RealtimeMemberDashboard.tsx | 130+ | Example integration |
| **TOTAL** | **880+** | **Real-time system** |

---

**Time Invested:** ~2.5 hours
**Status:** Frontend infrastructure complete
**Next:** Backend WebSocket server + Integration
