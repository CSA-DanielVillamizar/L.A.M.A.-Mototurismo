'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Loader2 } from 'lucide-react';

/**
 * Página de callback para MSAL redirect flow
 * MSAL manejará el callback automáticamente
 */
export default function AuthCallbackPage() {
  const router = useRouter();

  useEffect(() => {
    // MSAL manejará el redirect automáticamente
    // Después de procesar, redirigir a home
    const timer = setTimeout(() => {
      router.push('/admin/cor');
    }, 2000);

    return () => clearTimeout(timer);
  }, [router]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="text-center">
        <Loader2 className="h-8 w-8 animate-spin text-gray-600 mx-auto mb-4" />
        <p className="text-sm text-gray-600">Completando autenticación...</p>
      </div>
    </div>
  );
}
