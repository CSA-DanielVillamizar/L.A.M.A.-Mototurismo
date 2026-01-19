/**
 * Tipos para el módulo de Cola de Espera (Queue)
 * Define interfaces para registros pendientes de validación
 * 
 * NOTA: 'processing' NO es un estado de dominio (AttendanceStatus.cs enum).
 * Processing es solo un estado de UI para indicar carga en progreso.
 */

export type AttendanceStatus = 'pending' | 'confirmed' | 'rejected';

export interface QueueAttendee {
  id: string;
  memberId: string;
  memberName: string;
  memberEmail: string;
  eventId: string;
  eventName: string;
  eventDate: string;
  vehicleId: string;
  vehiclePlate: string;
  vehicleMake: string;
  vehicleModel: string;
  odometer: number;
  odometerUnit: 'km' | 'mi';
  status: AttendanceStatus;
  pointsAwarded?: number;
  submittedAt: string;
  validatedAt?: string;
  notes?: string;
}

export interface QueueFiltersState {
  search: string;
  eventId: string | null;
  status: AttendanceStatus | 'all' | null;
}

export interface QueuePagination {
  page: number;
  pageSize: number;
  total: number;
}

export interface QueueResponse {
  data: QueueAttendee[];
  pagination: QueuePagination;
}
