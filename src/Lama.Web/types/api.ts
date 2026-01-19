/**
 * Tipos de estado de miembro (33 valores del backend)
 */
export interface MemberStatusType {
  statusId: number;
  statusName: string;
  category: string;
  displayOrder: number;
}

/**
 * Evento disponible para selección
 */
export interface Event {
  eventId: number;
  eventName: string;
  eventDate: string; // DateOnly en formato YYYY-MM-DD
  chapterId: number;
  eventType: string;
}

/**
 * Resultado de búsqueda de miembro (autocomplete)
 */
export interface MemberSearchResult {
  memberId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  status: string;
  chapterId: number;
}

/**
 * Vehículo de un miembro
 */
export interface Vehicle {
  vehicleId: number;
  memberId: number;
  licensePlate: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  displayName: string; // "Brand Model Year (Placa)"
}

/**
 * Asistente a un evento
 */
export interface Attendee {
  attendanceId: number;
  eventId: number;
  memberId: number;
  vehicleId: number;
  memberName: string;
  vehicleInfo: string;
  order?: number;
  status: string;
  confirmedAt?: string;
}

/**
 * Request para subir evidencia
 */
export interface UploadEvidenceRequest {
  eventId: number;
  vehicleId: number;
  evidenceType: 'START_YEAR' | 'CUTOFF';
  pilotWithBikePhoto: File;
  odometerCloseupPhoto: File;
  odometerReading: number;
  unit: 'Miles' | 'Kilometers';
  readingDate?: string;
  notes?: string;
  licPlate: string;
  motorcycleData: string;
  trike: boolean;
}

/**
 * Response de subida de evidencia
 */
export interface EvidenceUploadResponse {
  message: string;
  pointsAwarded: number;
  pointsPerEvent: number;
  pointsPerDistance: number;
  visitorClass: string;
  memberId: number;
  vehicleId: number;
  attendanceId: number;
  evidenceType: string;
}

/**
 * Error response del backend
 */
export interface ApiError {
  error: string;
  details?: string;
}

/**
 * Resultado paginado genérico
 */
export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

/**
 * Item de la cola admin (asistencia pendiente de validación)
 */
export interface QueueItem {
  attendanceId: number;
  tenantId: string;
  eventId: number;
  eventName: string;
  eventDate: string; // DateOnly YYYY-MM-DD
  memberId: number;
  memberName: string;
  memberEmail: string;
  chapterId: number;
  vehicleId: number;
  vehicleLicPlate: string;
  vehicleMotorcycleData: string;
  status: 'PENDING' | 'CONFIRMED' | 'REJECTED';
  pointsPerEvent?: number;
  pointsPerDistance?: number;
  pointsAwardedPerMember?: number;
  visitorClass?: string;
  confirmedAt?: string;
  confirmedBy?: number;
  createdAt: string;
  updatedAt: string;
}
