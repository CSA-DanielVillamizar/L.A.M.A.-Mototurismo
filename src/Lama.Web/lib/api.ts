/**
 * API Service Layer
 * Centralizador de llamadas HTTP al backend
 * 
 * Uso: import { fetchMembers } from '@/lib/api'
 */

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';

/**
 * Configuración de headers por defecto
 */
function getHeaders(): HeadersInit {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  // Agregar token si existe
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('auth_token');
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
  }

  return headers;
}

/**
 * Clase para manejo de errores API
 */
export class APIError extends Error {
  constructor(
    public statusCode: number,
    public data: any
  ) {
    super(`API Error: ${statusCode}`);
    this.name = 'APIError';
  }
}

/**
 * Helper para llamadas HTTP
 */
async function fetchAPI<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const url = `${API_BASE_URL}${endpoint}`;
  
  const response = await fetch(url, {
    ...options,
    headers: {
      ...getHeaders(),
      ...(options?.headers || {}),
    },
  });

  const data = await response.json();

  if (!response.ok) {
    throw new APIError(response.status, data);
  }

  return data as T;
}

// =============================================================================
// MIEMBROS / USUARIOS
// =============================================================================

export interface Member {
  id: string;
  name: string;
  email: string;
  avatar: string;
  class: 'Basic' | 'Premium' | 'Elite';
  joinDate: string;
  points: number;
  rank: number;
}

/**
 * Obtener lista de miembros
 */
export async function fetchMembers(
  filters?: { classFilter?: string; sortBy?: string }
): Promise<Member[]> {
  const params = new URLSearchParams();
  if (filters?.classFilter) params.append('class', filters.classFilter);
  if (filters?.sortBy) params.append('sortBy', filters.sortBy);

  return fetchAPI<Member[]>(
    `/api/members${params.toString() ? '?' + params.toString() : ''}`
  );
}

/**
 * Obtener un miembro específico
 */
export async function fetchMemberById(id: string): Promise<Member> {
  return fetchAPI<Member>(`/api/members/${id}`);
}

/**
 * Obtener perfil del usuario actual
 */
export async function fetchCurrentMember(): Promise<Member> {
  return fetchAPI<Member>('/api/members/me');
}

/**
 * Actualizar perfil de usuario
 */
export async function updateMember(
  id: string,
  data: Partial<Member>
): Promise<Member> {
  return fetchAPI<Member>(`/api/members/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(data),
  });
}

// =============================================================================
// RANKING
// =============================================================================

export interface RankingEntry {
  rank: number;
  memberId: string;
  name: string;
  points: number;
  change: number; // Cambio desde última semana
}

/**
 * Obtener ranking nacional
 */
export async function fetchRanking(limit: number = 50): Promise<RankingEntry[]> {
  return fetchAPI<RankingEntry[]>(`/api/ranking?limit=${limit}`);
}

/**
 * Obtener detalles de miembro en ranking
 */
export async function fetchRankingDetail(memberId: string): Promise<{
  member: Member;
  rank: number;
  monthlyStats: Array<{ month: string; points: number }>;
  achievements: Array<{ title: string; date: string; points: number }>;
}> {
  return fetchAPI(`/api/ranking/${memberId}/detail`);
}

// =============================================================================
// EVIDENCIAS
// =============================================================================

export interface Evidence {
  id: string;
  memberId: string;
  eventName: string;
  eventDate: string;
  location: string;
  photo: string;
  status: 'pending' | 'approved' | 'rejected';
  points: number;
  uploadDate: string;
}

/**
 * Obtener evidencias del usuario
 */
export async function fetchEvidences(
  memberId?: string
): Promise<Evidence[]> {
  const endpoint = memberId
    ? `/api/evidences?memberId=${memberId}`
    : '/api/evidences/me';
  return fetchAPI<Evidence[]>(endpoint);
}

/**
 * Subir nueva evidencia
 */
export async function uploadEvidence(
  file: File,
  data: {
    eventName: string;
    eventDate: string;
    location: string;
  }
): Promise<Evidence> {
  const formData = new FormData();
  formData.append('photo', file);
  formData.append('eventName', data.eventName);
  formData.append('eventDate', data.eventDate);
  formData.append('location', data.location);

  const response = await fetch(`${API_BASE_URL}/api/evidences`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${localStorage.getItem('auth_token')}`,
    },
    body: formData,
  });

  if (!response.ok) {
    throw new APIError(response.status, await response.json());
  }

  return response.json();
}

/**
 * Obtener evidencias por estado
 */
export async function fetchEvidencesByStatus(
  status: 'pending' | 'approved' | 'rejected'
): Promise<Evidence[]> {
  return fetchAPI<Evidence[]>(`/api/evidences?status=${status}`);
}

// =============================================================================
// CAMPEONATOS
// =============================================================================

export interface Championship {
  id: string;
  year: number;
  name: string;
  status: 'completed' | 'ongoing' | 'upcoming';
  startDate: string;
  endDate: string;
  rounds: Array<{
    id: string;
    name: string;
    date: string;
    completed: boolean;
  }>;
}

/**
 * Obtener campeonatos del usuario
 */
export async function fetchChampionships(): Promise<Championship[]> {
  return fetchAPI<Championship[]>('/api/championships/me');
}

/**
 * Obtener detalles de un campeonato
 */
export async function fetchChampionshipDetail(id: string): Promise<Championship> {
  return fetchAPI<Championship>(`/api/championships/${id}`);
}

// =============================================================================
// PATROCINADORES
// =============================================================================

export interface Sponsor {
  id: string;
  name: string;
  logo: string;
  website: string;
  category: string;
  benefits: string[];
  discountPercentage?: number;
}

/**
 * Obtener lista de patrocinadores
 */
export async function fetchSponsors(
  category?: string
): Promise<Sponsor[]> {
  const endpoint = category
    ? `/api/sponsors?category=${category}`
    : '/api/sponsors';
  return fetchAPI<Sponsor[]>(endpoint);
}

/**
 * Obtener patrocinadores por categoría
 */
export async function fetchSponsorsByCategory(
  category: string
): Promise<Sponsor[]> {
  return fetchAPI<Sponsor[]>(`/api/sponsors?category=${category}`);
}

// =============================================================================
// MANEJO DE ERRORES
// =============================================================================

/**
 * Manejo consistente de errores API
 */
export function handleAPIError(error: unknown): string {
  if (error instanceof APIError) {
    // Error conocido de API
    if (error.statusCode === 401) {
      return 'Sesión expirada. Por favor, inicia sesión nuevamente.';
    }
    if (error.statusCode === 403) {
      return 'No tienes permiso para realizar esta acción.';
    }
    if (error.statusCode === 404) {
      return 'El recurso no fue encontrado.';
    }
    if (error.statusCode === 500) {
      return 'Error en el servidor. Intenta más tarde.';
    }
    return error.data?.message || 'Error desconocido de la API.';
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'Ocurrió un error inesperado.';
}

/**
 * Hook para retry automático
 */
export async function retryFetch<T>(
  fn: () => Promise<T>,
  maxRetries: number = 3,
  delayMs: number = 1000
): Promise<T> {
  let lastError: Error | null = null;

  for (let i = 0; i < maxRetries; i++) {
    try {
      return await fn();
    } catch (error) {
      lastError = error as Error;
      if (i < maxRetries - 1) {
        await new Promise((resolve) => setTimeout(resolve, delayMs * (i + 1)));
      }
    }
  }

  throw lastError;
}
