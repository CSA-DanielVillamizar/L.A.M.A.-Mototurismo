'use client';

import React, { ReactNode } from 'react';
import { cn } from '@/lib/utils';

/**
 * Propiedades de FormCard
 */
interface FormCardProps {
  /** Título de la sección del formulario */
  title?: string;
  /** Descripción de la sección */
  description?: string;
  /** Contenido del formulario */
  children: ReactNode;
  /** Si true, muestra un ícono de carga */
  isLoading?: boolean;
  /** Clases CSS adicionales */
  className?: string;
  /** Variante visual */
  variant?: 'default' | 'success' | 'warning' | 'danger';
}

/**
 * Tarjeta contenedora para formularios
 * Proporciona estilos consistentes y estructura de formulario
 */
export function FormCard({
  title,
  description,
  children,
  isLoading = false,
  className,
  variant = 'default',
}: FormCardProps) {
  const variantClasses = {
    default: 'border-neutral-200 bg-white',
    success: 'border-success-200 bg-success-50',
    warning: 'border-warning-200 bg-warning-50',
    danger: 'border-danger-200 bg-danger-50',
  };

  return (
    <div
      className={cn(
        'rounded-lg border p-6 transition-colors',
        variantClasses[variant],
        className
      )}
    >
      {/* Encabezado */}
      {(title || description) && (
        <div className="mb-6">
          {title && (
            <h3 className="text-lg font-semibold text-neutral-900">
              {title}
            </h3>
          )}
          {description && (
            <p className="mt-2 text-sm text-neutral-600">
              {description}
            </p>
          )}
        </div>
      )}

      {/* Contenido */}
      <div className={cn('space-y-4', isLoading && 'opacity-50 pointer-events-none')}>
        {children}
      </div>
    </div>
  );
}
