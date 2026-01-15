/**
 * Authentication Service Layer
 * Gestiona login, logout, token management
 */

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000';

export interface AuthToken {
  accessToken: string;
  refreshToken?: string;
  expiresIn: number; // segundos
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface SignupData extends LoginCredentials {
  name: string;
  phone: string;
}

export interface AuthState {
  isAuthenticated: boolean;
  user: any | null;
  token: string | null;
  error: string | null;
  loading: boolean;
}

/**
 * Almacenar token en localStorage
 */
function storeToken(token: string, expiresIn: number): void {
  localStorage.setItem('auth_token', token);
  localStorage.setItem('token_expires_at', String(Date.now() + expiresIn * 1000));
}

/**
 * Obtener token del almacenamiento
 */
export function getToken(): string | null {
  if (typeof window === 'undefined') return null;

  const token = localStorage.getItem('auth_token');
  const expiresAt = localStorage.getItem('token_expires_at');

  // Verificar si token está expirado
  if (token && expiresAt && Date.now() > parseInt(expiresAt)) {
    clearAuth();
    return null;
  }

  return token;
}

/**
 * Limpiar autenticación
 */
export function clearAuth(): void {
  localStorage.removeItem('auth_token');
  localStorage.removeItem('token_expires_at');
  localStorage.removeItem('auth_user');
}

/**
 * Login con email y contraseña
 */
export async function loginWithEmail(
  credentials: LoginCredentials
): Promise<AuthToken> {
  const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Error en login');
  }

  const data = await response.json();
  storeToken(data.accessToken, data.expiresIn);

  if (data.user) {
    localStorage.setItem('auth_user', JSON.stringify(data.user));
  }

  return data;
}

/**
 * Sign up / Registro de nuevo usuario
 */
export async function signupWithEmail(data: SignupData): Promise<AuthToken> {
  const response = await fetch(`${API_BASE_URL}/api/auth/signup`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Error en signup');
  }

  const result = await response.json();
  storeToken(result.accessToken, result.expiresIn);

  if (result.user) {
    localStorage.setItem('auth_user', JSON.stringify(result.user));
  }

  return result;
}

/**
 * Login con Google OAuth
 */
export async function loginWithGoogle(idToken: string): Promise<AuthToken> {
  const response = await fetch(`${API_BASE_URL}/api/auth/google`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ idToken }),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Error en login Google');
  }

  const data = await response.json();
  storeToken(data.accessToken, data.expiresIn);

  if (data.user) {
    localStorage.setItem('auth_user', JSON.stringify(data.user));
  }

  return data;
}

/**
 * Login con GitHub OAuth
 */
export async function loginWithGitHub(code: string): Promise<AuthToken> {
  const response = await fetch(`${API_BASE_URL}/api/auth/github`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ code }),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Error en login GitHub');
  }

  const data = await response.json();
  storeToken(data.accessToken, data.expiresIn);

  if (data.user) {
    localStorage.setItem('auth_user', JSON.stringify(data.user));
  }

  return data;
}

/**
 * Logout
 */
export async function logout(): Promise<void> {
  const token = getToken();
  
  if (token) {
    try {
      await fetch(`${API_BASE_URL}/api/auth/logout`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });
    } catch (error) {
      console.error('Error en logout:', error);
    }
  }

  clearAuth();
}

/**
 * Refresh token
 */
export async function refreshToken(
  refreshToken: string
): Promise<AuthToken> {
  const response = await fetch(`${API_BASE_URL}/api/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken }),
  });

  if (!response.ok) {
    clearAuth();
    throw new Error('Error refrescando token');
  }

  const data = await response.json();
  storeToken(data.accessToken, data.expiresIn);
  return data;
}

/**
 * Obtener usuario autenticado del almacenamiento
 */
export function getAuthUser(): any {
  if (typeof window === 'undefined') return null;
  
  const user = localStorage.getItem('auth_user');
  return user ? JSON.parse(user) : null;
}

/**
 * Verificar si usuario está autenticado
 */
export function isAuthenticated(): boolean {
  return getToken() !== null;
}

/**
 * Solicitar reset de contraseña
 */
export async function requestPasswordReset(email: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/auth/password-reset/request`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email }),
  });

  if (!response.ok) {
    throw new Error('Error solicitando reset de contraseña');
  }
}

/**
 * Confirmar reset de contraseña
 */
export async function confirmPasswordReset(
  token: string,
  newPassword: string
): Promise<void> {
  const response = await fetch(
    `${API_BASE_URL}/api/auth/password-reset/confirm`,
    {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token, newPassword }),
    }
  );

  if (!response.ok) {
    throw new Error('Error confirmando reset de contraseña');
  }
}

/**
 * Verify email address
 */
export async function verifyEmail(token: string): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/api/auth/verify-email`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token }),
  });

  if (!response.ok) {
    throw new Error('Error verificando email');
  }
}
