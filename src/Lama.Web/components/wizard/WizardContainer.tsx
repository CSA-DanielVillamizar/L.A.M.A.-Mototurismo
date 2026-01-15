'use client';

import React, { ReactNode, useState } from 'react';
import { Button } from '@/components/ui/button';
import { IconChevronLeft, IconChevronRight } from '@/components/icons';
import { cn } from '@/lib/utils';

/**
 * Propiedades de contenedor del wizard
 */
interface WizardContainerProps {
  /** Total de pasos en el wizard */
  totalSteps: number;
  /** Índice del paso actual (0-based) */
  currentStep: number;
  /** Callback cuando el usuario presiona "Siguiente" */
  onNext: () => void | Promise<void>;
  /** Callback cuando el usuario presiona "Anterior" */
  onPrevious: () => void;
  /** Callback cuando el usuario presiona "Finalizar" */
  onFinish: () => void | Promise<void>;
  /** Si true, el botón "Siguiente" está deshabilitado */
  isNextDisabled?: boolean;
  /** Si true, el botón "Anterior" está deshabilitado */
  isPreviousDisabled?: boolean;
  /** Si true, está en estado de carga (en onNext) */
  isLoading?: boolean;
  /** Contenido del paso actual */
  children: ReactNode;
  /** Indicador de progreso personalizado */
  stepIndicator?: ReactNode;
}

/**
 * Contenedor principal del wizard
 * Maneja la navegación entre pasos y el estado general
 */
export function WizardContainer({
  totalSteps,
  currentStep,
  onNext,
  onPrevious,
  onFinish,
  isNextDisabled = false,
  isPreviousDisabled = false,
  isLoading = false,
  children,
  stepIndicator,
}: WizardContainerProps) {
  const isLastStep = currentStep === totalSteps - 1;
  const isFirstStep = currentStep === 0;

  return (
    <div className="flex flex-col h-full gap-6">
      {/* Indicador de progreso */}
      {stepIndicator && <div>{stepIndicator}</div>}

      {/* Contenido del paso */}
      <div className="flex-1 overflow-auto">
        {children}
      </div>

      {/* Botones de navegación */}
      <div className="flex items-center justify-between pt-6 border-t border-neutral-200">
        <Button
          variant="ghost"
          onClick={onPrevious}
          disabled={isFirstStep || isPreviousDisabled || isLoading}
          className="gap-2"
        >
          <IconChevronLeft className="w-4 h-4" />
          Anterior
        </Button>

        <div className="text-sm text-neutral-600">
          Paso {currentStep + 1} de {totalSteps}
        </div>

        {!isLastStep ? (
          <Button
            onClick={onNext}
            disabled={isNextDisabled || isLoading}
            isLoading={isLoading}
            className="gap-2"
          >
            Siguiente
            <IconChevronRight className="w-4 h-4" />
          </Button>
        ) : (
          <Button
            onClick={onFinish}
            disabled={isLoading}
            isLoading={isLoading}
            variant="default"
          >
            Finalizar
          </Button>
        )}
      </div>
    </div>
  );
}
