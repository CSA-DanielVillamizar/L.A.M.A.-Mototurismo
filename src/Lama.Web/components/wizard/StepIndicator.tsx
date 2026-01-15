'use client';

import React from 'react';
import { cn } from '@/lib/utils';
import {
  IconCheckmark,
  IconChevronRight,
} from '@/components/icons';

/**
 * Propiedades del indicador de pasos
 */
interface StepIndicatorProps {
  /** Total de pasos */
  totalSteps: number;
  /** Paso actual (0-based) */
  currentStep: number;
  /** Etiquetas de cada paso */
  labels: string[];
  /** Pasos completados (índices de pasos que ya pasaron) */
  completedSteps?: number[];
}

/**
 * Indicador visual del progreso del wizard
 * Muestra los pasos completados, actuales y pendientes
 */
export function StepIndicator({
  totalSteps,
  currentStep,
  labels,
  completedSteps = [],
}: StepIndicatorProps) {
  return (
    <div className="flex items-center gap-2">
      {Array.from({ length: totalSteps }).map((_, index) => {
        const isCompleted = completedSteps.includes(index) || index < currentStep;
        const isCurrent = index === currentStep;

        return (
          <React.Fragment key={index}>
            {/* Círculo del paso */}
            <div
              className={cn(
                'flex items-center justify-center w-10 h-10 rounded-full font-semibold text-sm transition-all',
                isCurrent
                  ? 'bg-primary-600 text-white'
                  : isCompleted
                    ? 'bg-success-600 text-white'
                    : 'bg-neutral-200 text-neutral-600'
              )}
            >
              {isCompleted ? (
                <IconCheckmark className="w-5 h-5" />
              ) : (
                index + 1
              )}
            </div>

            {/* Conector entre pasos */}
            {index < totalSteps - 1 && (
              <div
                className={cn(
                  'flex-1 h-1 transition-all',
                  index < currentStep
                    ? 'bg-success-600'
                    : 'bg-neutral-200'
                )}
              />
            )}
          </React.Fragment>
        );
      })}
    </div>
  );
}

/**
 * Variante compacta del indicador (solo etiquetas)
 */
interface StepIndicatorCompactProps {
  totalSteps: number;
  currentStep: number;
  labels: string[];
}

export function StepIndicatorCompact({
  totalSteps,
  currentStep,
  labels,
}: StepIndicatorCompactProps) {
  return (
    <div className="flex items-center gap-1 overflow-x-auto pb-2">
      {Array.from({ length: totalSteps }).map((_, index) => (
        <React.Fragment key={index}>
          <div
            className={cn(
              'px-3 py-1 rounded-full text-xs font-medium whitespace-nowrap transition-all',
              index === currentStep
                ? 'bg-primary-600 text-white'
                : index < currentStep
                  ? 'bg-success-100 text-success-800'
                  : 'bg-neutral-100 text-neutral-600'
            )}
          >
            {labels[index]}
          </div>

          {index < totalSteps - 1 && (
            <IconChevronRight className="w-4 h-4 text-neutral-300 flex-shrink-0" />
          )}
        </React.Fragment>
      ))}
    </div>
  );
}
