/**
 * Custom Hooks para Autenticación
 * Proporciona acceso fácil a datos y funciones de autenticación
 */

'use client';

import { useAuth } from '@/lib/auth-context';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

/**
 * Hook para componentes que requieren autenticación
 * Redirige a login si no está autenticado
 */
export function useRequireAuth() {
  const { isAuthenticated, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isAuthenticated, isLoading, router]);

  return { isAuthenticated, isLoading };
}

/**
 * Hook para verificar si usuario tiene rol específico
 */
export function useRequireRole(requiredRole: string) {
  const { user, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && (!user || user.role !== requiredRole)) {
      router.push('/unauthorized');
    }
  }, [user, requiredRole, isLoading, router]);

  return { authorized: user?.role === requiredRole, isLoading };
}

/**
 * Hook para componentes que requieren admin
 */
export function useRequireAdmin() {
  return useRequireRole('admin');
}

/**
 * Hook para componentes que requieren organizer
 */
export function useRequireOrganizer() {
  return useRequireRole('organizer');
}

/**
 * Hook para información del usuario actual
 */
export function useCurrentUser() {
  const { user, isLoading, isAuthenticated } = useAuth();

  return {
    user,
    isLoading,
    isAuthenticated,
    name: user?.name || '',
    email: user?.email || '',
    role: user?.role || '',
    id: user?.id || '',
  };
}

/**
 * Hook para manejo de logout
 */
export function useLogout() {
  const { logout, isLoading } = useAuth();
  const router = useRouter();

  const handleLogout = async () => {
    try {
      await logout();
      router.push('/login');
    } catch (error) {
      console.error('Error logging out:', error);
    }
  };

  return { logout: handleLogout, isLoading };
}

/**
 * Hook para verificar si usuario es admin
 */
export function useIsAdmin(): boolean {
  const { user } = useAuth();
  return user?.role === 'admin';
}

/**
 * Hook para verificar si usuario es organizer
 */
export function useIsOrganizer(): boolean {
  const { user } = useAuth();
  return user?.role === 'organizer' || user?.role === 'admin';
}

/**
 * Hook para obtener información de miembro autenticado
 */
export function useAuthenticatedMember() {
  const { user, isLoading } = useAuth();

  return {
    memberId: user?.id,
    memberName: user?.name,
    memberEmail: user?.email,
    memberClass: user?.class,
    isLoading,
  };
}
