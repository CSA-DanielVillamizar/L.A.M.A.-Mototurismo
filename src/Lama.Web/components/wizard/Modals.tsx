'use client';

import React, { ReactNode } from 'react';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import {
  IconCheckmark,
  IconAlert,
  IconX,
} from '@/components/icons';
import { Button } from '@/components/ui/button';

/**
 * Modal de éxito
 */
interface SuccessModalProps {
  /** Si true, el modal está abierto */
  isOpen: boolean;
  /** Título del modal */
  title?: string;
  /** Descripción o mensaje */
  message?: string;
  /** Detalles adicionales o puntos ganados */
  details?: ReactNode;
  /** Botón secundario opcional */
  secondaryAction?: {
    label: string;
    onClick: () => void;
  };
  /** Callback cuando se cierra el modal */
  onClose: () => void;
}

export function SuccessModal({
  isOpen,
  title = '¡Éxito!',
  message = 'Tu evidencia ha sido registrada correctamente.',
  details,
  secondaryAction,
  onClose,
}: SuccessModalProps) {
  return (
    <AlertDialog open={isOpen} onOpenChange={onClose}>
      <AlertDialogContent className="bg-white">
        {/* Ícono de éxito */}
        <div className="flex justify-center -mt-8 mb-4">
          <div className="flex items-center justify-center w-16 h-16 rounded-full bg-success-100">
            <IconCheckmark className="w-8 h-8 text-success-600" />
          </div>
        </div>

        <AlertDialogHeader>
          <AlertDialogTitle className="text-center text-2xl">
            {title}
          </AlertDialogTitle>
          <AlertDialogDescription className="text-center mt-2">
            {message}
          </AlertDialogDescription>
        </AlertDialogHeader>

        {details && (
          <div className="my-4 p-4 rounded-lg bg-neutral-50 border border-neutral-200">
            {details}
          </div>
        )}

        <AlertDialogFooter className="flex gap-3">
          {secondaryAction && (
            <Button
              type="button"
              variant="ghost"
              onClick={secondaryAction.onClick}
            >
              {secondaryAction.label}
            </Button>
          )}
          <AlertDialogAction onClick={onClose}>
            {secondaryAction ? 'Cerrar' : 'Aceptar'}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

/**
 * Modal de error
 */
interface ErrorModalProps {
  /** Si true, el modal está abierto */
  isOpen: boolean;
  /** Título del modal */
  title?: string;
  /** Descripción o mensaje de error */
  message?: string;
  /** Detalles técnicos opcionales */
  errorDetails?: string;
  /** Callback cuando se presiona "Reintentar" */
  onRetry?: () => void;
  /** Callback cuando se presiona "Cancelar" */
  onCancel: () => void;
}

export function ErrorModal({
  isOpen,
  title = 'Error',
  message = 'Ocurrió un error al procesar tu solicitud.',
  errorDetails,
  onRetry,
  onCancel,
}: ErrorModalProps) {
  return (
    <AlertDialog open={isOpen} onOpenChange={onCancel}>
      <AlertDialogContent className="bg-white">
        {/* Ícono de error */}
        <div className="flex justify-center -mt-8 mb-4">
          <div className="flex items-center justify-center w-16 h-16 rounded-full bg-danger-100">
            <IconX className="w-8 h-8 text-danger-600" />
          </div>
        </div>

        <AlertDialogHeader>
          <AlertDialogTitle className="text-center text-xl">
            {title}
          </AlertDialogTitle>
          <AlertDialogDescription className="text-center mt-2">
            {message}
          </AlertDialogDescription>
        </AlertDialogHeader>

        {errorDetails && (
          <div className="my-4 p-3 rounded-lg bg-danger-50 border border-danger-200">
            <p className="text-xs text-danger-700 font-mono">
              {errorDetails}
            </p>
          </div>
        )}

        <AlertDialogFooter className="flex gap-3">
          <AlertDialogCancel onClick={onCancel}>
            Cancelar
          </AlertDialogCancel>
          {onRetry && (
            <Button
              onClick={onRetry}
              variant="destructive"
            >
              Reintentar
            </Button>
          )}
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}

/**
 * Modal informativo genérico
 */
interface InfoModalProps {
  /** Si true, el modal está abierto */
  isOpen: boolean;
  /** Ícono a mostrar */
  icon?: React.ReactNode;
  /** Título del modal */
  title: string;
  /** Descripción o mensaje */
  message: string;
  /** Contenido adicional */
  children?: ReactNode;
  /** Acciones personalizadas */
  actions?: Array<{
    label: string;
    onClick: () => void;
    variant?: 'primary' | 'secondary' | 'danger';
  }>;
  /** Callback cuando se cierra el modal */
  onClose: () => void;
}

export function InfoModal({
  isOpen,
  icon,
  title,
  message,
  children,
  actions,
  onClose,
}: InfoModalProps) {
  return (
    <AlertDialog open={isOpen} onOpenChange={onClose}>
      <AlertDialogContent className="bg-white">
        {icon && (
          <div className="flex justify-center -mt-8 mb-4">
            <div className="flex items-center justify-center w-16 h-16 rounded-full bg-primary-100">
              {icon}
            </div>
          </div>
        )}

        <AlertDialogHeader>
          <AlertDialogTitle className="text-center">
            {title}
          </AlertDialogTitle>
          <AlertDialogDescription className="text-center mt-2">
            {message}
          </AlertDialogDescription>
        </AlertDialogHeader>

        {children && (
          <div className="my-4">
            {children}
          </div>
        )}

        <AlertDialogFooter className="flex gap-3">
          {actions ? (
            actions.map((action) => (
              <Button
                key={action.label}
                onClick={action.onClick}
                variant={action.variant as any}
              >
                {action.label}
              </Button>
            ))
          ) : (
            <AlertDialogAction onClick={onClose}>
              Aceptar
            </AlertDialogAction>
          )}
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
