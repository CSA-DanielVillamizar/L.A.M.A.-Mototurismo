'use client';

import React, { createContext, useContext, useEffect, useState } from 'react';
import {
  getToken,
  getAuthUser,
  logout,
  loginWithEmail,
  loginWithGoogle,
  loginWithGitHub,
  signupWithEmail,
  clearAuth,
  type AuthToken,
  type LoginCredentials,
  type SignupData,
} from './auth';

/**
 * Contexto de autenticación
 */
interface AuthContextType {
  isAuthenticated: boolean;
  user: any | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
  
  loginEmail: (credentials: LoginCredentials) => Promise<void>;
  loginGoogle: (idToken: string) => Promise<void>;
  loginGitHub: (code: string) => Promise<void>;
  signup: (data: SignupData) => Promise<void>;
  logout: () => Promise<void>;
  clearError: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

/**
 * Provider de autenticación
 */
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<any>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Inicializar estado de autenticación
  useEffect(() => {
    const initAuth = () => {
      const storedToken = getToken();
      const storedUser = getAuthUser();

      if (storedToken) {
        setToken(storedToken);
        setUser(storedUser);
        setIsAuthenticated(true);
      }

      setIsLoading(false);
    };

    initAuth();
  }, []);

  const handleLoginEmail = async (credentials: LoginCredentials) => {
    setIsLoading(true);
    setError(null);

    try {
      const authToken = await loginWithEmail(credentials);
      const storedUser = getAuthUser();
      
      setToken(authToken.accessToken);
      setUser(storedUser);
      setIsAuthenticated(true);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Error en login';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const handleLoginGoogle = async (idToken: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const authToken = await loginWithGoogle(idToken);
      const storedUser = getAuthUser();
      
      setToken(authToken.accessToken);
      setUser(storedUser);
      setIsAuthenticated(true);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Error en login Google';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const handleLoginGitHub = async (code: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const authToken = await loginWithGitHub(code);
      const storedUser = getAuthUser();
      
      setToken(authToken.accessToken);
      setUser(storedUser);
      setIsAuthenticated(true);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Error en login GitHub';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const handleSignup = async (data: SignupData) => {
    setIsLoading(true);
    setError(null);

    try {
      const authToken = await signupWithEmail(data);
      const storedUser = getAuthUser();
      
      setToken(authToken.accessToken);
      setUser(storedUser);
      setIsAuthenticated(true);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Error en signup';
      setError(message);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = async () => {
    setIsLoading(true);
    
    try {
      await logout();
      setToken(null);
      setUser(null);
      setIsAuthenticated(false);
      setError(null);
    } catch (err) {
      console.error('Error en logout:', err);
    } finally {
      setIsLoading(false);
    }
  };

  const clearError = () => setError(null);

  const value: AuthContextType = {
    isAuthenticated,
    user,
    token,
    isLoading,
    error,
    loginEmail: handleLoginEmail,
    loginGoogle: handleLoginGoogle,
    loginGitHub: handleLoginGitHub,
    signup: handleSignup,
    logout: handleLogout,
    clearError,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

/**
 * Hook para acceder al contexto de autenticación
 */
export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth debe ser usado dentro de AuthProvider');
  }
  return context;
}

/**
 * Hook para verificar si usuario está autenticado
 */
export function useIsAuthenticated(): boolean {
  const { isAuthenticated } = useAuth();
  return isAuthenticated;
}

/**
 * Hook para obtener usuario autenticado
 */
export function useUser(): any {
  const { user } = useAuth();
  return user;
}

/**
 * Hook para obtener token
 */
export function useToken(): string | null {
  const { token } = useAuth();
  return token;
}
