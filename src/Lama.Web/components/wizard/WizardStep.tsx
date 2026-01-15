'use client';

import React, { ReactNode } from 'react';
import { cn } from '@/lib/utils';

/**
 * Propiedades de un paso individual del wizard
 */
interface WizardStepProps {
  /** Número del paso (1-based para mostrar) */
  stepNumber: number;
  /** Título del paso */
  title: string;
  /** Descripción opcional del paso */
  description?: string;
  /** Si true, el paso está activo (visible) */
  isActive: boolean;
  /** Contenido del paso */
  children: ReactNode;
  /** Clases CSS adicionales */
  className?: string;
}

/**
 * Componente individual de paso en el wizard
 * Envuelve el contenido de cada paso con animación de entrada/salida
 */
export function WizardStep({
  stepNumber,
  title,
  description,
  isActive,
  children,
  className,
}: WizardStepProps) {
  return (
    <div
      className={cn(
        'transition-all duration-300 ease-in-out',
        isActive ? 'opacity-100' : 'opacity-0 pointer-events-none absolute',
        className
      )}
    >
      {/* Encabezado del paso */}
      <div className="mb-6">
        <h2 className="text-2xl font-bold text-neutral-900">
          {stepNumber}. {title}
        </h2>
        {description && (
          <p className="mt-2 text-neutral-600">{description}</p>
        )}
      </div>

      {/* Contenido del paso */}
      <div className="space-y-4">
        {children}
      </div>
    </div>
  );
}
