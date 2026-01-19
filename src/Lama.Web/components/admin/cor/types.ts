/**
 * Tipos para el flujo de validación de COR
 * Define interfaces para eventos, miembros, vehículos y evidencia
 */

export type CorStep = 'event' | 'member' | 'vehicle' | 'evidence' | 'success';

export interface Event {
  id: string;
  name: string;
  year: number;
  date: string;
  location: string;
}

export interface Member {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  chapter: string;
}

export interface Vehicle {
  id: string;
  licensePlate: string;
  make: string;
  model: string;
  year: number;
}

export interface OdometerReading {
  value: number;
  unit: 'km' | 'mi';
}

export interface EvidenceForm {
  odometer: OdometerReading;
  photos: File[];
  notes?: string;
}

export interface CorFlowState {
  currentStep: CorStep;
  event: Event | null;
  member: Member | null;
  vehicle: Vehicle | null;
  evidence: EvidenceForm | null;
  loading: boolean;
  error: string | null;
  successData?: CorValidationResponse;
}

export interface CorValidationResponse {
  attendanceId: string;
  memberId: string;
  eventId: string;
  pointsAwarded: number;
  pointsBreakdown?: {
    basePoints: number;
    bonusPoints: number;
    description: string;
  };
  timestamp: string;
}

export interface CorStepperProps {
  onComplete?: (data: CorValidationResponse) => void;
  initialEventId?: string;
  initialMemberId?: string;
  initialVehicleId?: string;
}
