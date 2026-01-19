'use client';

/**
 * QueueTable Component
 * Tabla premium para mostrar registros de asistencia en cola
 */

import React from 'react';
import { QueueAttendee } from './types';
import { StatusPill, EmptyState } from '@/components/common';
import { QueueRowActions } from './QueueRowActions';
import { Loader2, Inbox } from 'lucide-react';

interface QueueTableProps {
  data: QueueAttendee[];
  loading: boolean;
  validatingRowId?: string | null;
  onValidate: (attendee: QueueAttendee) => void;
  onViewDetail: (attendee: QueueAttendee) => void;
}

export function QueueTable({
  data,
  loading,
  validatingRowId,
  onValidate,
  onViewDetail,
}: QueueTableProps) {
  if (loading) {
    return (
      <div className="rounded-lg border border-gray-200 bg-white">
        <div className="flex items-center justify-center py-12">
          <Loader2 className="h-6 w-6 animate-spin text-indigo-600" />
          <span className="ml-2 text-gray-600">Cargando registros...</span>
        </div>
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <EmptyState
        icon={Inbox}
        title="No hay registros en cola"
        description="Los registros aparecerán aquí cuando se envíen para validación"
      />
    );
  }

  return (
    <div className="rounded-lg border border-gray-200 bg-white overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-900">
                Miembro
              </th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-900">
                Estado
              </th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-900">
                Evento
              </th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-900">
                Fecha
              </th>
              <th className="px-6 py-3 text-left text-xs font-semibold text-gray-900">
                Vehículo
              </th>
              <th className="px-6 py-3 text-right text-xs font-semibold text-gray-900">
                Odómetro
              </th>
              <th className="px-6 py-3 text-right text-xs font-semibold text-gray-900">
                Puntos
              </th>
              <th className="px-6 py-3 text-center text-xs font-semibold text-gray-900">
                Acciones
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {data.map((attendee) => {
              const isProcessing = validatingRowId === attendee.id;
              return (
              <tr
                key={attendee.id}
                className={`hover:bg-gray-50 transition-colors ${isProcessing ? 'opacity-60' : ''}`}
              >
                <td className="px-6 py-4">
                  <div className="flex flex-col">
                    <span className="font-medium text-gray-900">
                      {attendee.memberName}
                    </span>
                    <span className="text-xs text-gray-600">
                      {attendee.memberEmail}
                    </span>
                  </div>
                </td>

                <td className="px-6 py-4">
                  <StatusPill status={attendee.status} />
                </td>

                <td className="px-6 py-4">
                  <div className="flex flex-col">
                    <span className="font-medium text-gray-900">
                      {attendee.eventName}
                    </span>
                    <span className="text-xs text-gray-600">
                      {formatDate(attendee.eventDate)}
                    </span>
                  </div>
                </td>

                <td className="px-6 py-4">
                  <span className="text-sm text-gray-600">
                    {formatDate(attendee.submittedAt)}
                  </span>
                </td>

                <td className="px-6 py-4">
                  <div className="flex flex-col">
                    <span className="font-medium text-gray-900">
                      {attendee.vehicleMake} {attendee.vehicleModel}
                    </span>
                    <span className="text-xs text-gray-600">
                      {attendee.vehiclePlate}
                    </span>
                  </div>
                </td>

                <td className="px-6 py-4 text-right">
                  <span className="font-mono text-sm text-gray-900">
                    {attendee.odometer.toLocaleString()}{' '}
                    <span className="text-gray-600">{attendee.odometerUnit}</span>
                  </span>
                </td>

                <td className="px-6 py-4 text-right">
                  {attendee.pointsAwarded ? (
                    <span className="font-bold text-indigo-600">
                      +{attendee.pointsAwarded}
                    </span>
                  ) : (
                    <span className="text-gray-400">—</span>
                  )}
                </td>

                <td className="px-6 py-4">
                  <QueueRowActions
                    attendee={attendee}
                    isProcessing={isProcessing}
                    onValidate={onValidate}
                    onViewDetail={onViewDetail}
                  />
                </td>
              </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
}

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('es-CO', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
  });
}
