'use client';

import { useState, useEffect, useCallback } from 'react';
import Link from 'next/link';
import type { Event, Attendee } from '@/types/api';
import { apiClient } from '@/lib/api-client';
import { EventSelector } from '@/components/EventSelector';

/**
 * Página Admin Queue
 * Muestra lista de asistentes pendientes para validar evidencia
 *
 * URL: /admin/queue
 */
export default function QueuePage() {
  const [selectedEvent, setSelectedEvent] = useState<Event | null>(null);
  const [attendees, setAttendees] = useState<Attendee[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [filterStatus, setFilterStatus] = useState<'PENDING' | 'CONFIRMED' | ''>('PENDING');

  // Cargar asistentes cuando cambia el evento o filtro
  useEffect(() => {
    if (!selectedEvent) {
      setAttendees([]);
      return;
    }

    const loadAttendees = async () => {
      try {
        setLoading(true);
        setError(null);
        const status = filterStatus ? (filterStatus as 'PENDING' | 'CONFIRMED') : undefined;
        const attendeesData = await apiClient.getEventAttendees(selectedEvent.eventId, status);
        setAttendees(attendeesData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error al cargar asistentes');
        setAttendees([]);
      } finally {
        setLoading(false);
      }
    };

    loadAttendees();
  }, [selectedEvent, filterStatus]);

  const handleValidateClick = useCallback(
    (memberId: number, vehicleId: number) => {
      if (!selectedEvent) return;
      // Navegar a /admin/cor con parámetros precompletados
      const queryString = `?eventId=${selectedEvent.eventId}&memberId=${memberId}&vehicleId=${vehicleId}`;
      window.location.href = `/admin/cor${queryString}`;
    },
    [selectedEvent]
  );

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'PENDING':
        return <span className="inline-flex items-center rounded-md bg-yellow-50 px-2 py-1 text-xs font-medium text-yellow-800">Pendiente</span>;
      case 'CONFIRMED':
        return <span className="inline-flex items-center rounded-md bg-green-50 px-2 py-1 text-xs font-medium text-green-800">Confirmado</span>;
      case 'REJECTED':
        return <span className="inline-flex items-center rounded-md bg-red-50 px-2 py-1 text-xs font-medium text-red-800">Rechazado</span>;
      default:
        return <span className="inline-flex items-center rounded-md bg-gray-50 px-2 py-1 text-xs font-medium text-gray-800">{status}</span>;
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-6xl">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">
                Cola de Validación (Queue)
              </h1>
              <p className="mt-2 text-gray-600">
                Revisa los asistentes pendientes y valida su evidencia
              </p>
            </div>
            <Link
              href="/admin/cor"
              className="inline-flex items-center rounded-lg bg-blue-600 px-4 py-2 font-semibold text-white hover:bg-blue-700"
            >
              Ir a COR
            </Link>
          </div>
        </div>

        {/* Tarjeta Principal */}
        <div className="rounded-lg bg-white shadow">
          <div className="px-6 py-8">
            {/* Selector de Evento */}
            <div className="mb-8">
              <EventSelector
                selectedEvent={selectedEvent}
                onEventSelect={setSelectedEvent}
                allowYearSelection={true}
                testId="queue-event-selector"
              />
            </div>

            {selectedEvent && (
              <>
                {/* Filtro de Estado */}
                <div className="mb-6">
                  <label htmlFor="status-filter" className="block text-sm font-medium text-gray-700">
                    Filtrar por estado
                  </label>
                  <select
                    id="status-filter"
                    value={filterStatus}
                    onChange={(e) => setFilterStatus(e.target.value as any)}
                    className="mt-1 block w-full max-w-xs rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                  >
                    <option value="">Todos los estados</option>
                    <option value="PENDING">Pendiente</option>
                    <option value="CONFIRMED">Confirmado</option>
                  </select>
                </div>

                {/* Lista de Asistentes */}
                {loading ? (
                  <div className="flex justify-center py-12">
                    <div className="h-8 w-8 animate-spin rounded-full border-4 border-gray-300 border-t-blue-600" />
                  </div>
                ) : error ? (
                  <div className="rounded-lg bg-red-50 p-4">
                    <p className="text-sm text-red-700">{error}</p>
                  </div>
                ) : attendees.length === 0 ? (
                  <div className="rounded-lg bg-gray-50 p-8 text-center">
                    <p className="text-gray-600">
                      {filterStatus === 'PENDING'
                        ? 'No hay asistentes pendientes'
                        : 'No hay asistentes con ese estado'}
                    </p>
                  </div>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead className="border-b border-gray-200 bg-gray-50">
                        <tr>
                          <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700">
                            Miembro
                          </th>
                          <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700">
                            Vehículo
                          </th>
                          <th className="px-6 py-3 text-left text-xs font-semibold text-gray-700">
                            Estado
                          </th>
                          <th className="px-6 py-3 text-right text-xs font-semibold text-gray-700">
                            Acción
                          </th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-200">
                        {attendees.map((attendee) => (
                          <tr key={attendee.attendanceId} className="hover:bg-gray-50">
                            <td className="px-6 py-4">
                              <div>
                                <p className="font-semibold text-gray-900">
                                  {attendee.completeNames}
                                </p>
                                <p className="text-sm text-gray-500">
                                  #{attendee.order} • ID: {attendee.memberId}
                                </p>
                              </div>
                            </td>
                            <td className="px-6 py-4">
                              <div>
                                <p className="font-medium text-gray-900">
                                  {attendee.motorcycleData}
                                </p>
                                <p className="text-sm text-gray-500">{attendee.licPlate}</p>
                              </div>
                            </td>
                            <td className="px-6 py-4">
                              {getStatusBadge(attendee.status)}
                            </td>
                            <td className="px-6 py-4 text-right">
                              {attendee.status === 'PENDING' && (
                                <button
                                  onClick={() => handleValidateClick(attendee.memberId, attendee.vehicleId)}
                                  className="inline-flex items-center rounded-md bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700"
                                >
                                  Validar
                                </button>
                              )}
                              {attendee.status === 'CONFIRMED' && (
                                <span className="text-xs text-gray-600">
                                  ✓ {attendee.confirmedAt && new Date(attendee.confirmedAt).toLocaleDateString('es-MX')}
                                </span>
                              )}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                    <div className="border-t border-gray-200 bg-gray-50 px-6 py-4">
                      <p className="text-sm font-medium text-gray-700">
                        Total: <span className="text-gray-900">{attendees.length}</span> asistentes
                      </p>
                    </div>
                  </div>
                )}
              </>
            )}
          </div>
        </div>

        {/* Información */}
        <div className="mt-8 rounded-lg bg-blue-50 p-6">
          <h3 className="text-sm font-semibold text-blue-900">Cómo funciona la cola</h3>
          <ol className="mt-3 space-y-2 list-decimal list-inside text-sm text-blue-700">
            <li>Selecciona un evento para ver sus asistentes pendientes</li>
            <li>Haz clic en "Validar" para abrir el formulario COR precompletado</li>
            <li>Sube las fotos y confirma la asistencia</li>
            <li>El estado cambiará a "Confirmado" automáticamente</li>
          </ol>
        </div>
      </div>
    </div>
  );
}
