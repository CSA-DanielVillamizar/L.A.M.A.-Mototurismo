/**
 * Cliente de autenticación para manejo de tokens y sesiones
 * Implementa Exchange Flow: Entra token → App session
 */

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';

export interface AuthSessionResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  user: UserInfo;
}

export interface UserInfo {
  id: number;
  email: string;
  displayName: string | null;
  memberId: number | null;
  memberName: string | null;
  chapterName: string | null;
  roles: string[];
  scopes: string[];
  tenantId: string;
}

export interface ExchangeTokenRequest {
  idToken: string;
}

/**
 * Intercambia un token de Entra ID por una sesión de la aplicación
 */
export async function exchangeEntraToken(idToken: string): Promise<AuthSessionResponse> {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/exchange`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ idToken } as ExchangeTokenRequest),
    credentials: 'include', // CRÍTICO: enviar/recibir cookies httpOnly
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ detail: 'Authentication failed' }));
    throw new Error(error.detail || `Exchange failed: ${response.status}`);
  }

  return response.json();
}

/**
 * Refresca el access token usando el refresh token en cookie
 */
export async function refreshSession(): Promise<AuthSessionResponse> {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/refresh-session`, {
    method: 'POST',
    credentials: 'include', // Cookie auto-enviada
  });

  if (!response.ok) {
    throw new Error('Session refresh failed');
  }

  return response.json();
}

/**
 * Cierra sesión (revoca refresh token)
 */
export async function logoutSession(): Promise<void> {
  await fetch(`${API_BASE_URL}/api/v1/auth/logout-session`, {
    method: 'POST',
    credentials: 'include',
  });
}

/**
 * Obtiene información del usuario autenticado
 */
export async function getCurrentUser(accessToken: string): Promise<UserInfo> {
  const response = await fetch(`${API_BASE_URL}/api/v1/auth/me`, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
    credentials: 'include',
  });

  if (!response.ok) {
    throw new Error('Failed to get current user');
  }

  return response.json();
}
