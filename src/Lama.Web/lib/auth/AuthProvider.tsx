'use client';

import React, { createContext, useContext, useEffect, useState, useCallback } from 'react';
import { PublicClientApplication, AccountInfo } from '@azure/msal-browser';
import { msalConfig, loginRequest } from './msalConfig';
import {
  exchangeEntraToken,
  refreshSession,
  logoutSession,
  getCurrentUser,
  AuthSessionResponse,
  UserInfo,
} from './session';
import { setAccessToken as setAccessTokenInClient } from '@/lib/api-client';

interface AuthContextType {
  // Estado
  user: UserInfo | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // Métodos
  signIn: () => Promise<void>;
  signOut: () => Promise<void>;
  refresh: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Inicializar MSAL (singleton)
let msalInstance: PublicClientApplication | null = null;

function getMsalInstance(): PublicClientApplication {
  if (!msalInstance) {
    msalInstance = new PublicClientApplication(msalConfig);
  }
  return msalInstance;
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const isAuthenticated = !!user && !!accessToken;

  /**
   * ✅ SINCRONIZAR token con API Client cuando cambie
   */
  useEffect(() => {
    setAccessTokenInClient(accessToken);
  }, [accessToken]);

  /**
   * Inicializar MSAL y verificar si hay sesión activa
   */
  useEffect(() => {
    async function initialize() {
      try {
        const msal = getMsalInstance();
        await msal.initialize();

        // Verificar si hay sesión de app activa (intentar refresh)
        try {
          const session = await refreshSession();
          setAccessToken(session.accessToken);
          setUser(session.user);
        } catch {
          // No hay sesión activa, OK
        }
      } catch (err) {
        console.error('Auth initialization error:', err);
        setError('Failed to initialize authentication');
      } finally {
        setIsLoading(false);
      }
    }

    initialize();
  }, []);

  /**
   * Sign In: Redirige a Entra ID, obtiene token, exchange por sesión app
   */
  const signIn = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const msal = getMsalInstance();

      try {
        // 1. Login con Entra ID (redirect)
        const response = await msal.loginPopup(loginRequest);

        if (!response.idToken) {
          throw new Error('No ID token received from Entra ID');
        }

        // 2. Exchange token de Entra por sesión de app
        const session = await exchangeEntraToken(response.idToken);

        // 3. Guardar access token en memoria (sincroniza automáticamente con api-client)
        setAccessToken(session.accessToken);
        setUser(session.user);
      } catch (entraError: any) {
        // En desarrollo local sin Entra ID configurado, permitir login sin autenticación
        // (solo para testing del UI/API sin credentials reales)
        if (
          entraError?.errorCode === 'endpoints_resolution_error' ||
          entraError?.message?.includes('endpoints_resolution_error')
        ) {
          console.warn(
            '⚠️  Entra ID not configured (development mode). Using mock session for UI testing.',
            'To enable real authentication, configure NEXT_PUBLIC_ENTRA_* variables.'
          );

          // Mock session para desarrollo sin Entra
          const mockUser: UserInfo = {
            id: 123,
            email: 'dev@localhost.local',
            displayName: 'Desarrollo',
            memberId: null,
            memberName: null,
            chapterName: null,
            roles: ['Admin'],
            scopes: ['admin'],
            tenantId: '00000000-0000-0000-0000-000000000001',
          };

          const mockAccessToken = 'mock-token-dev-' + Date.now();

          setAccessToken(mockAccessToken);
          setUser(mockUser);
          setAccessTokenInClient(mockAccessToken);
        } else {
          // Otros errores son reales
          throw entraError;
        }
      }
    } catch (err: any) {
      console.error('Sign in error:', err);
      setError(err.message || 'Sign in failed');
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Sign Out: Revoca sesión app + logout de Entra ID
   */
  const signOut = useCallback(async () => {
    try {
      setIsLoading(true);

      // 1. Revocar refresh token del backend
      await logoutSession();

      // 2. Limpiar estado local (sincroniza automáticamente con api-client)
      setAccessToken(null);
      setUser(null);

      // 3. Logout de MSAL (opcional, limpia sesión Entra)
      const msal = getMsalInstance();
      const accounts = msal.getAllAccounts();
      if (accounts.length > 0) {
        await msal.logoutPopup({
          account: accounts[0],
        });
      }
    } catch (err) {
      console.error('Sign out error:', err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Refresh: Rota refresh token y obtiene nuevo access token
   */
  const refresh = useCallback(async () => {
    try {
      const session = await refreshSession();
      setAccessToken(session.accessToken);
      setUser(session.user);
    } catch (err) {
      console.error('Refresh error:', err);
      // Si falla refresh, usuario debe re-autenticarse
      setAccessToken(null);
      setUser(null);
      throw err;
    }
  }, []);

  const value: AuthContextType = {
    user,
    accessToken,
    isAuthenticated,
    isLoading,
    error,
    signIn,
    signOut,
    refresh,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
