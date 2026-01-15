import type {
  MemberStatusType,
  UploadEvidenceRequest,
  EvidenceUploadResponse,
  ApiError,
} from '@/types/api';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';

/**
 * Clase centralizada para todas las llamadas a la API del backend
 */
class ApiClient {
  private baseUrl: string;
  Event,
  MemberSearchResult,
  Vehicle,
  Attendee,

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  /**
   * Obtener todos los tipos de estado de miembros (33 valores)
   */
  async getMemberStatusTypes(): Promise<MemberStatusType[]> {
    const response = await fetch(`${this.baseUrl}/api/MemberStatusTypes`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Obtener tipos de estado por categoría
   */
  async getMemberStatusTypesByCategory(category: string): Promise<MemberStatusType[]> {
    const response = await fetch(
      `${this.baseUrl}/api/MemberStatusTypes/by-category/${encodeURIComponent(category)}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (!response.ok) {
      if (response.status === 404) {
        return []; // Categoría no encontrada
      }
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Obtener todas las categorías de estados
   */
  async getMemberStatusCategories(): Promise<string[]> {
    const response = await fetch(`${this.baseUrl}/api/MemberStatusTypes/categories`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Subir evidencia fotográfica con multipart/form-data
   */
  async uploadEvidence(
    request: UploadEvidenceRequest
  ): Promise<EvidenceUploadResponse> {
    const formData = new FormData();

    // Append form fields
    formData.append('memberId', request.memberId.toString());
    formData.append('vehicleId', request.vehicleId.toString());
    formData.append('evidenceType', request.evidenceType);
    formData.append('pilotWithBikePhoto', request.pilotWithBikePhoto);
    formData.append('odometerCloseupPhoto', request.odometerCloseupPhoto);
    formData.append('odometerReading', request.odometerReading.toString());
    formData.append('unit', request.unit);

    if (request.readingDate) {
      formData.append('readingDate', request.readingDate);
    }

    if (request.notes) {
      formData.append('notes', request.notes);
    }

    const response = await fetch(
      `${this.baseUrl}/api/admin/evidence/upload?eventId=${request.eventId}`,
      {
        method: 'POST',
        body: formData,
        // No establecer Content-Type - fetch lo hará automáticamente para multipart/form-data
      }
    );

    if (!response.ok) {
      const error: ApiError = await response.json().catch(() => ({
        error: `HTTP ${response.status}: ${response.statusText}`,
      }));

      throw new Error(error.error || 'Error desconocido al subir evidencia');
    }

    return response.json();
  }

  /**
   * Obtener todos los eventos disponibles
   */
  async getEvents(): Promise<Event[]> {
    const response = await fetch(`${this.baseUrl}/api/events`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Buscar miembros por nombre o apellido (autocomplete)
   */
  async searchMembers(query: string): Promise<MemberSearchResult[]> {
    if (!query || query.length < 2) {
      return [];
    }

    const response = await fetch(
      `${this.baseUrl}/api/members/search?q=${encodeURIComponent(query)}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (!response.ok) {
      if (response.status === 400) {
        return []; // Query inválido
      }
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Obtener vehículos de un miembro específico
   */
  async getMemberVehicles(memberId: number): Promise<Vehicle[]> {
    const response = await fetch(
      `${this.baseUrl}/api/members/${memberId}/vehicles`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (!response.ok) {
      if (response.status === 404) {
        return []; // Miembro no encontrado
      }
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }
}

// Singleton instance
export const apiClient = new ApiClient(API_BASE_URL);
