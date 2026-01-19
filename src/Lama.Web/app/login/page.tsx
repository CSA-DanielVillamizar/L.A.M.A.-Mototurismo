'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/lib/auth/AuthProvider';
import { AlertCircle, Loader2 } from 'lucide-react';

export default function LoginPage() {
  const { signIn, isAuthenticated, isLoading, error } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (isAuthenticated) {
      router.push('/admin/cor');
    }
  }, [isAuthenticated, router]);

  async function handleSignIn() {
    try {
      await signIn();
    } catch (err) {
      // Error manejado en el hook
    }
  }

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100">
        <Loader2 className="h-8 w-8 animate-spin text-gray-600" />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 px-4">
      <div className="max-w-md w-full">
        {/* Card */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-8">
          {/* Logo / Header */}
          <div className="text-center mb-8">
            <h1 className="text-2xl font-semibold text-gray-900 mb-2">
              COR L.A.MA
            </h1>
            <p className="text-sm text-gray-600">
              Chapter Odometer Report
            </p>
          </div>

  {/* Error Message */}
          {error && (
            <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-md flex items-start gap-3">
              <AlertCircle className="h-5 w-5 text-red-600 flex-shrink-0 mt-0.5" />
              <div>
                <p className="text-sm font-medium text-red-900">
                  Error de autenticación
                </p>
                <p className="text-sm text-red-700 mt-1">{error}</p>
              </div>
            </div>
          )}

          {/* Sign In Button */}
          <button
            onClick={handleSignIn}
            disabled={isLoading}
            className="w-full flex items-center justify-center gap-2 px-4 py-3 bg-gray-900 text-white text-sm font-medium rounded-md hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-900 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            {isLoading ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin" />
                Iniciando sesión...
              </>
            ) : (
              'Iniciar sesión con Microsoft'
            )}
          </button>

          {/* Footer */}
          <p className="mt-6 text-center text-xs text-gray-500">
            Utiliza tu cuenta de Microsoft para acceder
          </p>
        </div>

        {/* Footer Info */}
        <p className="mt-8 text-center text-xs text-gray-500">
          Sistema de gestión de evidencia fotográfica
        </p>
      </div>
    </div>
  );
}
