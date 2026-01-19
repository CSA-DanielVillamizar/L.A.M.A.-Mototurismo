'use client';

import { useEffect } from 'react';
import { useRouter, usePathname } from 'next/navigation';
import { useAuth } from '@/lib/auth/AuthProvider';
import { Loader2 } from 'lucide-react';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
}

/**
 * Componente para proteger rutas que requieren autenticación
 */
export function ProtectedRoute({ children, requiredRoles }: ProtectedRouteProps) {
  const { user, isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      // Redirigir a login preservando URL destino
      router.push(`/login?redirect=${encodeURIComponent(pathname)}`);
    }
  }, [isLoading, isAuthenticated, router, pathname]);

  // Verificar roles si se especifican
  const hasRequiredRole = !requiredRoles || requiredRoles.some(role => 
    user?.roles.includes(role)
  );

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="h-8 w-8 animate-spin text-gray-600" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return null; // Redireccionando...
  }

  if (!hasRequiredRole) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <h1 className="text-2xl font-semibold text-gray-900 mb-2">
            Acceso denegado
          </h1>
          <p className="text-sm text-gray-600">
            No tienes permisos para acceder a esta página
          </p>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
