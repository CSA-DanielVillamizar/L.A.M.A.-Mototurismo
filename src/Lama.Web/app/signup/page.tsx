'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/lib/auth-context';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card } from '@/components/ui/card';
import { Alert } from '@/components/ui/alert';

/**
 * Página de registro
 */
export default function SignupPage() {
  const router = useRouter();
  const { signup, error, clearError, isLoading } = useAuth();

  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: '',
    password: '',
    confirmPassword: '',
  });
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [localError, setLocalError] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const validateForm = (): boolean => {
    if (!formData.name.trim()) {
      setLocalError('El nombre es requerido');
      return false;
    }

    if (!formData.email.includes('@')) {
      setLocalError('Email inválido');
      return false;
    }

    if (!formData.phone.trim()) {
      setLocalError('El teléfono es requerido');
      return false;
    }

    if (formData.password.length < 8) {
      setLocalError('La contraseña debe tener al menos 8 caracteres');
      return false;
    }

    if (formData.password !== formData.confirmPassword) {
      setLocalError('Las contraseñas no coinciden');
      return false;
    }

    if (!acceptTerms) {
      setLocalError('Debes aceptar los términos y condiciones');
      return false;
    }

    return true;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLocalError(null);
    clearError();

    if (!validateForm()) {
      return;
    }

    try {
      await signup({
        name: formData.name,
        email: formData.email,
        phone: formData.phone,
        password: formData.password,
      });

      router.push('/member/dashboard');
    } catch (err) {
      setLocalError(err instanceof Error ? err.message : 'Error en registro');
    }
  };

  const displayError = localError || error;

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
            {/* Título */}
            <div>
              <h2 className="text-2xl font-bold text-white">Crear cuenta</h2>
              <p className="text-slate-400 text-sm mt-1">
                Únete a nuestra comunidad de mototurismo
              </p>
            </div>

            {/* Error Alert */}
            {displayError && (
              <Alert variant="destructive">
                <p className="text-sm">{displayError}</p>
              </Alert>
            )}

            {/* Form */}
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-200">
                  Nombre completo
                </label>
                <Input
                  type="text"
                  name="name"
                  placeholder="Juan Pérez"
                  value={formData.name}
                  onChange={handleChange}
                  disabled={isLoading}
                  className="bg-slate-700/50 border-slate-600 text-white placeholder:text-slate-500"
                />
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-200">
                  Email
                </label>
                <Input
                  type="email"
                  name="email"
                  placeholder="tu@email.com"
                  value={formData.email}
                  onChange={handleChange}
                  disabled={isLoading}
                  className="bg-slate-700/50 border-slate-600 text-white placeholder:text-slate-500"
                />
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-200">
                  Teléfono
                </label>
                <Input
                  type="tel"
                  name="phone"
                  placeholder="+57 300 000 0000"
                  value={formData.phone}
                  onChange={handleChange}
                  disabled={isLoading}
                  className="bg-slate-700/50 border-slate-600 text-white placeholder:text-slate-500"
                />
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-200">
                  Contraseña
                </label>
                <Input
                  type="password"
                  name="password"
                  placeholder="••••••••"
                  value={formData.password}
                  onChange={handleChange}
                  disabled={isLoading}
                  className="bg-slate-700/50 border-slate-600 text-white placeholder:text-slate-500"
                />
                <p className="text-xs text-slate-400">
                  Mínimo 8 caracteres
                </p>
              </div>

              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-200">
                  Confirmar contraseña
                </label>
                <Input
                  type="password"
                  name="confirmPassword"
                  placeholder="••••••••"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  disabled={isLoading}
                  className="bg-slate-700/50 border-slate-600 text-white placeholder:text-slate-500"
                />
              </div>

              <div className="flex items-start">
                <input
                  type="checkbox"
                  id="acceptTerms"
                  checked={acceptTerms}
                  onChange={(e) => setAcceptTerms(e.target.checked)}
                  disabled={isLoading}
                  className="w-4 h-4 rounded border-slate-600 bg-slate-700/50 text-purple-500 cursor-pointer mt-1"
                />
                <label
                  htmlFor="acceptTerms"
                  className="ml-2 text-sm text-slate-300 cursor-pointer"
                >
                  Acepto los{' '}
                  <Link
                    href="/terms"
                    className="text-purple-400 hover:text-purple-300 underline"
                  >
                    términos y condiciones
                  </Link>
                </label>
              </div>

              <Button
                type="submit"
                disabled={isLoading}
                className="w-full bg-purple-600 hover:bg-purple-700 text-white"
              >
                {isLoading ? 'Registrando...' : 'Crear cuenta'}
              </Button>
            </form>

            {/* Footer Links */}
            <div className="text-center text-sm">
              <p className="text-slate-400">
                ¿Ya tienes cuenta?{' '}
                <Link
                  href="/login"
                  className="text-purple-400 hover:text-purple-300 font-medium"
                >
                  Inicia sesión
                </Link>
              </p>
            </div>
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
