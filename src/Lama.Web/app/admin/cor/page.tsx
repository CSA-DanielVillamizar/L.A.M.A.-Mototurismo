'use client';

import { useState, useCallback } from 'react';
import { useSearchParams } from 'next/navigation';
import type { Event, MemberSearchResult, Vehicle, EvidenceUploadResponse } from '@/types/api';
import { EventSelector } from '@/components/EventSelector';
import { MemberSearchAutocomplete } from '@/components/MemberSearchAutocomplete';
import { VehicleSelector } from '@/components/VehicleSelector';
import { EvidenceUploader } from '@/components/EvidenceUploader';

/**
 * Página COR (Confirmation of Riding)
 * Permite a MTO validar evidencia de asistencia a eventos
 *
 * URL: /admin/cor
 * Parámetros opcionalesde query string:
 *  - ?eventId=123&memberId=456&vehicleId=789
 */
export default function CORPage() {
  const searchParams = useSearchParams();

  // Estado del formulario
  const [selectedEvent, setSelectedEvent] = useState<Event | null>(null);
  const [selectedMember, setSelectedMember] = useState<MemberSearchResult | null>(null);
  const [selectedVehicle, setSelectedVehicle] = useState<Vehicle | null>(null);
  const [uploadResult, setUploadResult] = useState<EvidenceUploadResponse | null>(null);
  const [uploadError, setUploadError] = useState<string | null>(null);

  // Cargar desde query string si está disponible
  const handleQueryStringPreload = useCallback(() => {
    const eventId = searchParams.get('eventId');
    const memberId = searchParams.get('memberId');
    const vehicleId = searchParams.get('vehicleId');

    if (eventId) {
      // Aquí simplemente notamos que se pasaron, el EventSelector maneja el resto
      // ya que necesita cargar los datos desde el backend
    }
  }, [searchParams]);

  const handleUploadSuccess = (response: EvidenceUploadResponse) => {
    setUploadResult(response);
    setUploadError(null);
    // Limpiar formulario después de 2 segundos
    setTimeout(() => {
      setSelectedMember(null);
      setSelectedVehicle(null);
    }, 2000);
  };

  const handleUploadError = (error: string) => {
    setUploadError(error);
    setUploadResult(null);
  };

  const formIsComplete =
    selectedEvent && selectedMember && selectedVehicle;

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-2xl">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Confirmación de Asistencia (COR)
          </h1>
          <p className="mt-2 text-gray-600">
            Registra la evidencia fotográfica para confirmar la asistencia de un miembro a un evento
          </p>
        </div>

        {/* Tarjeta Principal */}
        <div className="rounded-lg bg-white shadow">
          <div className="px-6 py-8">
            {/* Paso 1: Seleccionar Evento */}
            <div className="mb-8">
              <div className="mb-4 flex items-center">
                <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-600 text-white font-semibold">
                  1
                </div>
                <h2 className="ml-3 text-lg font-semibold text-gray-900">Evento</h2>
              </div>
              <EventSelector
                selectedEvent={selectedEvent}
                onEventSelect={setSelectedEvent}
                testId="event-selector"
              />
            </div>

            {/* Paso 2: Buscar Miembro */}
            {selectedEvent && (
              <div className="mb-8">
                <div className="mb-4 flex items-center">
                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-600 text-white font-semibold">
                    2
                  </div>
                  <h2 className="ml-3 text-lg font-semibold text-gray-900">Miembro</h2>
                </div>
                <MemberSearchAutocomplete
                  selectedMember={selectedMember}
                  onMemberSelect={setSelectedMember}
                  testId="member-search"
                />
              </div>
            )}

            {/* Paso 3: Seleccionar Vehículo */}
            {selectedMember && (
              <div className="mb-8">
                <div className="mb-4 flex items-center">
                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-600 text-white font-semibold">
                    3
                  </div>
                  <h2 className="ml-3 text-lg font-semibold text-gray-900">Vehículo</h2>
                </div>
                <VehicleSelector
                  memberId={selectedMember.memberId}
                  selectedVehicle={selectedVehicle}
                  onVehicleSelect={setSelectedVehicle}
                  testId="vehicle-selector"
                />
              </div>
            )}

            {/* Paso 4: Subir Evidencia */}
            {formIsComplete && (
              <div className="mb-8">
                <div className="mb-4 flex items-center">
                  <div className="flex h-8 w-8 items-center justify-center rounded-full bg-blue-600 text-white font-semibold">
                    4
                  </div>
                  <h2 className="ml-3 text-lg font-semibold text-gray-900">Evidencia</h2>
                </div>
                <EvidenceUploader
                  eventId={selectedEvent.eventId}
                  memberId={selectedMember.memberId}
                  vehicleId={selectedVehicle.vehicleId}
                  onSuccess={handleUploadSuccess}
                  onError={handleUploadError}
                  testId="evidence-uploader"
                />
              </div>
            )}
          </div>

          {/* Resultado de Carga */}
          {uploadResult && (
            <div className="border-t border-gray-200 bg-green-50 px-6 py-6">
              <div className="flex items-start">
                <div className="flex-shrink-0">
                  <svg className="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                </div>
                <div className="ml-3">
                  <h3 className="text-lg font-semibold text-green-800">
                    ¡Asistencia Confirmada!
                  </h3>
                  <div className="mt-3 space-y-2 text-sm text-green-700">
                    <p>
                      <strong>Puntos Otorgados:</strong> {uploadResult.pointsAwarded} pts
                    </p>
                    <p>
                      <strong>Desglose:</strong> {uploadResult.pointsPerEvent} por evento +{' '}
                      {uploadResult.pointsPerDistance} por distancia
                    </p>
                    <p>
                      <strong>Clase Visitante:</strong> {uploadResult.visitorClass}
                    </p>
                    <p>
                      <strong>ID Asistencia:</strong> #{uploadResult.attendanceId}
                    </p>
                  </div>
                  <p className="mt-4 text-xs text-gray-600">
                    El formulario se limpió automáticamente. Puedes registrar otro miembro.
                  </p>
                </div>
              </div>
            </div>
          )}

          {/* Error */}
          {uploadError && (
            <div className="border-t border-gray-200 bg-red-50 px-6 py-6">
              <div className="flex items-start">
                <div className="flex-shrink-0">
                  <svg
                    className="h-6 w-6 text-red-600"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
                <div className="ml-3">
                  <h3 className="text-sm font-semibold text-red-800">Error al registrar</h3>
                  <p className="mt-2 text-sm text-red-700">{uploadError}</p>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Info adicional */}
        <div className="mt-8 rounded-lg bg-blue-50 p-6">
          <h3 className="text-sm font-semibold text-blue-900">¿Necesitas ayuda?</h3>
          <ul className="mt-3 space-y-2 text-sm text-blue-700">
            <li>• Busca por nombre completo, número de orden o placa del vehículo</li>
            <li>• Las fotos deben ser claras y mostrar el piloto con la moto y el odómetro</li>
            <li>• El odómetro debe ser un número válido mayor a 0</li>
            <li>• Todos los campos marcados con * son obligatorios</li>
          </ul>
        </div>
      </div>
    </div>
  );
}
