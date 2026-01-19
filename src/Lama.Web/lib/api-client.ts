import type {
  MemberStatusType,
  UploadEvidenceRequest,
  EvidenceUploadResponse,
  ApiError,
  Event,
  MemberSearchResult,
  Vehicle,
  Attendee,
  PagedResult,
  QueueItem,
} from '@/types/api';
import { refreshSession } from './auth/session';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';

/**
 * Variable global para almacenar access token en memoria
 * NUNCA en localStorage/sessionStorage
 */
let _accessToken: string | null = null;

/**
 * Flag para evitar múltiples refreshes simultáneos
 */
let _isRefreshing = false;
let _refreshPromise: Promise<string> | null = null;

/**
 * Almacenar access token en memoria
 */
export function setAccessToken(token: string | null) {
  _accessToken = token;
}

/**
 * Obtener access token desde memoria
 */
export function getAccessToken(): string | null {
  return _accessToken;
}

/**
 * Refrescar access token con single-flight pattern
 */
async function refreshAccessToken(): Promise<string> {
  if (_isRefreshing && _refreshPromise) {
    return _refreshPromise;
  }

  _isRefreshing = true;
  _refreshPromise = (async () => {
    try {
      const session = await refreshSession();
      _accessToken = session.accessToken;
      return session.accessToken;
    } catch (error) {
      _accessToken = null;
      throw error;
    } finally {
      _isRefreshing = false;
      _refreshPromise = null;
    }
  })();

  return _refreshPromise;
}

/**
 * Clase centralizada para todas las llamadas a la API del backend
 * Con interceptor 401 automático para refresh
 */
class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  /**
   * Fetch con auto-retry en 401
   */
  private async fetchWithAuth(
    url: string,
    options: RequestInit = {}
  ): Promise<Response> {
    // Agregar Authorization header si hay token
    const headers = new Headers(options.headers);
    if (_accessToken && !headers.has('Authorization')) {
      headers.set('Authorization', `Bearer ${_accessToken}`);
    }

    // CRÍTICO: incluir cookies para refresh token
    const response = await fetch(url, {
      ...options,
      headers,
      credentials: 'include',
    });

    // Si 401, intentar refresh y reintentar (solo si no es retry)
    const isRetry = headers.has('X-Retry');
    if (response.status === 401 && !isRetry) {
      try {
        const newToken = await refreshAccessToken();
        
        // Reintentar con nuevo token
        const retryHeaders = new Headers(options.headers);
        retryHeaders.set('Authorization', `Bearer ${newToken}`);
        retryHeaders.set('X-Retry', 'true'); // Evitar loop infinito

        return fetch(url, {
          ...options,
          headers: retryHeaders,
          credentials: 'include',
        });
      } catch (error) {
        // Refresh falló, lanzar 401 original
        return response;
      }
    }

    return response;
  }

  /**
   * Obtener todos los tipos de estado de miembros (33 valores)
   */
  async getMemberStatusTypes(): Promise<MemberStatusType[]> {
    const response = await this.fetchWithAuth(`${this.baseUrl}/api/v1/MemberStatusTypes`, {
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
    const response = await this.fetchWithAuth(
      `${this.baseUrl}/api/v1/MemberStatusTypes/by-category/${encodeURIComponent(category)}`,
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
    const response = await this.fetchWithAuth(`${this.baseUrl}/api/v1/MemberStatusTypes/categories`, {
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
    formData.append('vehicleId', request.vehicleId.toString());
    formData.append('evidenceType', request.evidenceType);
    formData.append('pilotWithBikePhoto', request.pilotWithBikePhoto);
    formData.append('odometerCloseupPhoto', request.odometerCloseupPhoto);
    formData.append('odometerReading', request.odometerReading.toString());
    formData.append('unit', request.unit);
    formData.append('trike', request.trike.toString());

    if (request.readingDate) {
      formData.append('readingDate', request.readingDate);
    }

    if (request.notes) {
      formData.append('notes', request.notes);
    }

    const response = await this.fetchWithAuth(
      `${this.baseUrl}/api/v1/admin/evidence/upload?eventId=${request.eventId}`,
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
    const response = await this.fetchWithAuth(`${this.baseUrl}/api/v1/events`, {
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
   * Obtener eventos por año
   */
  async getEventsByYear(year: number): Promise<Event[]> {
    const response = await this.fetchWithAuth(`${this.baseUrl}/api/v1/events?year=${year}`, {
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

    const response = await this.fetchWithAuth(
      `${this.baseUrl}/api/v1/members/search?q=${encodeURIComponent(query)}`,
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
    const response = await this.fetchWithAuth(
      `${this.baseUrl}/api/v1/members/${memberId}/vehicles`,
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

  /**
   * Obtener asistentes de un evento
   */
  async getEventAttendees(
    eventId: number, 
    status?: 'PENDING' | 'CONFIRMED'
  ): Promise<Attendee[]> {
    const url = status 
      ? `${this.baseUrl}/api/v1/events/${eventId}/attendees?status=${status}`
      : `${this.baseUrl}/api/v1/events/${eventId}/attendees`;
    
    const response = await this.fetchWithAuth(url, {
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
   * Obtener cola de admin con filtros, búsqueda y paginación
   */
  async getAdminQueue(params: {
    eventId?: number;
    status?: 'PENDING' | 'CONFIRMED' | 'REJECTED';
    q?: string;
    page?: number;
    pageSize?: number;
    sort?: string;
  }): Promise<PagedResult<QueueItem>> {
    const queryParams = new URLSearchParams();
    if (params.eventId !== undefined) queryParams.append('eventId', params.eventId.toString());
    if (params.status) queryParams.append('status', params.status);
    if (params.q) queryParams.append('q', params.q);
    if (params.page) queryParams.append('page', params.page.toString());
    if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    if (params.sort) queryParams.append('sort', params.sort);

    const url = `${this.baseUrl}/api/v1/admin/queue${queryParams.toString() ? `?${queryParams}` : ''}`;

    const response = await this.fetchWithAuth(url, {
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
   * Obtener evento por ID (para deep-linking)
   */
  async getEventById(eventId: number): Promise<Event> {
    const response = await this.fetchWithAuth(
      `${this.baseUrl}/api/v1/events/${eventId}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (!response.ok) {
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }

  /**
   * Obtener miembro por ID (para deep-linking)
   */
  async getMemberById(memberId: number): Promise<MemberSearchResult> {
    const response = await this.fetchWithAuth(
      `${this.baseUrl}/api/v1/members/${memberId}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    );

    if (!response.ok) {
      throw new Error(`Error ${response.status}: ${response.statusText}`);
    }

    return response.json();
  }
}

// Singleton instance
export const apiClient = new ApiClient(API_BASE_URL);
