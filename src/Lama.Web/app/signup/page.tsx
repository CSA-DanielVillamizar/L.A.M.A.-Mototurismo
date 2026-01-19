'use client';

import Link from 'next/link';

// Deshabilitar esta página temporalmente - autenticación via Entra ID
export const dynamic = 'error';

/**
 * Página de registro - DESHABILITADA
 * El registro se realiza a través de Microsoft Entra External ID
 */
export default function SignupPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full text-center">
        <h1 className="text-2xl font-semibold text-gray-900 mb-4">
          Registro no disponible
        </h1>
        <p className="text-sm text-gray-600 mb-6">
          El registro de usuarios se realiza a través de Microsoft Entra ID. 
          Contacta al administrador para obtener acceso.
        </p>
        <Link
          href="/login"
          className="inline-block px-4 py-2 bg-gray-900 text-white text-sm font-medium rounded-md hover:bg-gray-800"
        >
          Volver al login
        </Link>
      </div>
    </div>
  );
}
