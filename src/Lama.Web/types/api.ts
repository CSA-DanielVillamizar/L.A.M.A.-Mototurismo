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
 * Request para subir evidencia
 */
export interface UploadEvidenceRequest {
  eventId: number;
    order?: number; // Número de orden del miembro (nuevo)
  vehicleId: number;
  evidenceType: 'START_YEAR' | 'CUTOFF';
  pilotWithBikePhoto: File;
  odometerCloseupPhoto: File;
  odometerReading: number;
  unit: 'Miles' | 'Kilometers';
  readingDate?: string; // DateOnly en formato YYYY-MM-DD
  notes?: string;
    licPlate: string; // Cambiado a licPlate
    motorcycleData: string; // "Marca Model Año Color"
    trike: boolean; // Nuevo campo para indicar si es un trike
    displayName?: string; // Generado automáticamente "MotorcycleData - LicPlate"
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
