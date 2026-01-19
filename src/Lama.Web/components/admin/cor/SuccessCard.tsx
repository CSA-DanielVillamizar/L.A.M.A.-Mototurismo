'use client';

/**
 * SuccessCard Component
 * Pantalla final: Confirmación de validación exitosa con resumen
 */

import React from 'react';
import { CorValidationResponse } from './types';
import { CheckCircle2, ArrowRight } from 'lucide-react';
import { Button } from '@/components/ui/button';
import Link from 'next/link';

interface SuccessCardProps {
  data: CorValidationResponse;
  onValidateAnother: () => void;
}

export function SuccessCard({ data, onValidateAnother }: SuccessCardProps) {
  const formattedDate = new Date(data.timestamp).toLocaleDateString('es-CO', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });

  return (
    <div className="space-y-6">
      {/* Success Icon & Message */}
      <div className="flex flex-col items-center gap-4 rounded-lg bg-green-50 border border-green-200 p-8 text-center">
        <CheckCircle2 className="h-12 w-12 text-green-600" />
        <div>
          <h2 className="text-2xl font-bold text-gray-900">
            Asistencia Confirmada
          </h2>
          <p className="mt-2 text-gray-600">
            La evidencia ha sido validada y registrada correctamente.
          </p>
        </div>
      </div>

      {/* Summary Card */}
      <div className="rounded-lg border border-gray-200 bg-white p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">
          Resumen de Validación
        </h3>

        <div className="space-y-3">
          <div className="flex items-center justify-between py-3 border-b border-gray-100">
            <span className="text-sm text-gray-600">ID de Asistencia</span>
            <span className="font-mono text-sm font-medium text-gray-900">
              {data.attendanceId}
            </span>
          </div>

          <div className="flex items-center justify-between py-3 border-b border-gray-100">
            <span className="text-sm text-gray-600">Puntos Otorgados</span>
            <span className="text-2xl font-bold text-indigo-600">
              +{data.pointsAwarded}
            </span>
          </div>

          {data.pointsBreakdown && (
            <div className="py-3 border-b border-gray-100">
              <p className="text-sm text-gray-600 mb-2">Desglose de Puntos</p>
              <div className="bg-gray-50 rounded p-3 space-y-1 text-sm">
                <div className="flex justify-between">
                  <span>Puntos Base:</span>
                  <span className="font-medium">{data.pointsBreakdown.basePoints}</span>
                </div>
                <div className="flex justify-between">
                  <span>Bonificación:</span>
                  <span className="font-medium text-green-600">
                    +{data.pointsBreakdown.bonusPoints}
                  </span>
                </div>
                <div className="text-xs text-gray-600 mt-2">
                  {data.pointsBreakdown.description}
                </div>
              </div>
            </div>
          )}

          <div className="flex items-center justify-between py-3">
            <span className="text-sm text-gray-600">Registrado en</span>
            <span className="text-sm text-gray-900">{formattedDate}</span>
          </div>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="flex flex-col gap-3 sm:flex-row">
        <Button
          onClick={onValidateAnother}
          className="flex-1"
        >
          Validar Otro Evento
        </Button>

        <Link href="/admin/queue" className="flex-1">
          <Button variant="outline" className="w-full">
            <ArrowRight className="h-4 w-4 mr-2" />
            Ir a Cola de Espera
          </Button>
        </Link>
      </div>

      {/* Additional Info */}
      <div className="rounded-lg border border-blue-200 bg-blue-50 p-4">
        <p className="text-sm text-blue-900">
          La información ha sido guardada en el sistema. Puedes consultar el historial
          de validaciones en el panel de control.
        </p>
      </div>
    </div>
  );
}
