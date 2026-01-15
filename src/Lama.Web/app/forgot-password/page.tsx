'use client';

import { useState } from 'react';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card } from '@/components/ui/card';
import { Alert } from '@/components/ui/alert';
import { requestPasswordReset } from '@/lib/auth';

/**
 * Página de solicitud de reset de contraseña
 */
export default function ForgotPasswordPage() {
  const [email, setEmail] = useState('');
  const [submitted, setSubmitted] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsLoading(true);

    try {
      if (!email.includes('@')) {
        throw new Error('Email inválido');
      }

      await requestPasswordReset(email);
      setSubmitted(true);
      setEmail('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error solicitando reset');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-900 via-purple-900 to-slate-900 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {/* Logo/Header */}
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-white mb-2">L.A.M.A.</h1>
          <p className="text-slate-300">Mototurismo Community</p>
        </div>

        <Card className="bg-slate-800/50 border-slate-700 backdrop-blur">
          <div className="p-6 space-y-6">
            {/* Title */}
            <div>
              <h2 className="text-2xl font-bold text-white">
                Recuperar contraseña
              </h2>
              <p className="text-slate-400 text-sm mt-1">
                Te enviaremos un enlace para resetear tu contraseña
              </p>
            </div>

            {submitted ? (
              <>
                {/* Success Message */}
                <Alert variant="default" className="bg-green-900/50 border-green-700">
                  <p className="text-sm text-green-200">
                    ✅ Email enviado. Por favor revisa tu bandeja de entrada y spam.
                  </p>
                </Alert>

                <p className="text-slate-300 text-sm">
                  Hemos enviado un enlace de reset a <strong>{email}</strong>. 
                  Sigue las instrucciones para crear una nueva contraseña.
                </p>

                <Link href="/login">
                  <Button className="w-full bg-purple-600 hover:bg-purple-700 text-white">
                    Volver a Login
                  </Button>
                </Link>
              </>
            ) : (
              <>
                {/* Error Alert */}
                {error && (
                  <Alert variant="destructive">
                    <p className="text-sm">{error}</p>
                  </Alert>
                )}

                {/* Form */}
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="space-y-2">
                    <label className="block text-sm font-medium text-slate-200">
                      Email asociado a tu cuenta
                    </label>
                    <Input
                      type="email"
                      placeholder="tu@email.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      disabled={isLoading}
                      className="bg-slate-700/50 border-slate-600 text-white placeholder:text-slate-500"
                    />
                  </div>

                  <Button
                    type="submit"
                    disabled={isLoading}
                    className="w-full bg-purple-600 hover:bg-purple-700 text-white"
                  >
                    {isLoading ? 'Enviando...' : 'Enviar enlace de reset'}
                  </Button>
                </form>

                {/* Back to Login */}
                <p className="text-center text-sm text-slate-400">
                  <Link
                    href="/login"
                    className="text-purple-400 hover:text-purple-300"
                  >
                    Volver a login
                  </Link>
                </p>
              </>
            )}
          </div>
        </Card>

        {/* Footer */}
        <p className="text-center text-slate-400 text-xs mt-8">
          © 2024 L.A.M.A. Mototurismo. Todos los derechos reservados.
        </p>
      </div>
    </div>
  );
}
