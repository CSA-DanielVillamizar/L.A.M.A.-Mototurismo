'use client';

import { useState, useCallback } from 'react';
import type { EvidenceUploadResponse } from '@/types/api';
import { apiClient } from '@/lib/api-client';

interface EvidenceUploaderProps {
  /**
   * ID del evento
   */
  eventId: number;

  /**
   * ID del miembro
   */
  memberId: number;

  /**
   * ID del vehículo
   */
  vehicleId: number;

  /**
   * Callback cuando la subida es exitosa
   */
  onSuccess: (response: EvidenceUploadResponse) => void;

  /**
   * Callback cuando hay error
   */
  onError: (error: string) => void;

  /**
   * Deshabilitar el formulario
   */
  disabled?: boolean;

  /**
   * ID de atributo para testing
   */
  testId?: string;
}

/**
 * Componente para subir evidencia fotográfica
 * Maneja 2 fotos, lectura del odómetro, unidad y tipo de evidencia
 */
export function EvidenceUploader({
  eventId,
  memberId,
  vehicleId,
  onSuccess,
  onError,
  disabled = false,
  testId,
}: EvidenceUploaderProps) {
  const [loading, setLoading] = useState(false);
  const [evidenceType, setEvidenceType] = useState<'START_YEAR' | 'CUTOFF'>('START_YEAR');
  const [odometerReading, setOdometerReading] = useState('');
  const [unit, setUnit] = useState<'Miles' | 'Kilometers'>('Miles');
  const [pilotWithBikePhoto, setPilotWithBikePhoto] = useState<File | null>(null);
  const [odometerCloseupPhoto, setOdometerCloseupPhoto] = useState<File | null>(null);
  const [notes, setNotes] = useState('');

  const handlePhotoSelect = useCallback(
    (
      event: React.ChangeEvent<HTMLInputElement>,
      setPhoto: (file: File | null) => void
    ) => {
      const file = event.target.files?.[0];
      if (file && file.type.startsWith('image/')) {
        setPhoto(file);
      } else {
        onError('Por favor selecciona una imagen válida');
      }
    },
    [onError]
  );

  const validateForm = useCallback(() => {
    if (!eventId || !memberId || !vehicleId) {
      onError('Faltan datos requeridos del formulario');
      return false;
    }

    if (!pilotWithBikePhoto) {
      onError('Selecciona la foto de piloto con moto');
      return false;
    }

    if (!odometerCloseupPhoto) {
      onError('Selecciona la foto de close-up del odómetro');
      return false;
    }

    const reading = parseFloat(odometerReading);
    if (isNaN(reading) || reading <= 0) {
      onError('Ingresa una lectura de odómetro válida (mayor a 0)');
      return false;
    }

    return true;
  }, [eventId, memberId, vehicleId, pilotWithBikePhoto, odometerCloseupPhoto, odometerReading, onError]);

  const handleSubmit = useCallback(
    async (e: React.FormEvent) => {
      e.preventDefault();

      if (!validateForm()) {
        return;
      }

      try {
        setLoading(true);

        const response = await apiClient.uploadEvidence({
          eventId,
          vehicleId,
          evidenceType,
          pilotWithBikePhoto: pilotWithBikePhoto!,
          odometerCloseupPhoto: odometerCloseupPhoto!,
          odometerReading: parseFloat(odometerReading),
          unit,
          notes: notes || undefined,
          licPlate: '',
          motorcycleData: '',
          trike: false,
        });

        onSuccess(response);

        // Limpiar formulario
        setEvidenceType('START_YEAR');
        setOdometerReading('');
        setUnit('Miles');
        setPilotWithBikePhoto(null);
        setOdometerCloseupPhoto(null);
        setNotes('');
      } catch (error) {
        onError(error instanceof Error ? error.message : 'Error al subir evidencia');
      } finally {
        setLoading(false);
      }
    },
    [
      validateForm,
      eventId,
      memberId,
      vehicleId,
      evidenceType,
      pilotWithBikePhoto,
      odometerCloseupPhoto,
      odometerReading,
      unit,
      notes,
      onSuccess,
      onError,
    ]
  );

  const isFormValid =
    eventId && memberId && vehicleId && pilotWithBikePhoto && odometerCloseupPhoto && odometerReading;

  return (
    <form onSubmit={handleSubmit} className="space-y-6" data-testid={testId}>
      {/* Tipo de Evidencia */}
      <div>
        <label htmlFor="evidence-type" className="block text-sm font-medium text-gray-700">
          Tipo de Evidencia
        </label>
        <select
          id="evidence-type"
          value={evidenceType}
          onChange={(e) => setEvidenceType(e.target.value as 'START_YEAR' | 'CUTOFF')}
          disabled={disabled || loading}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm disabled:bg-gray-100 disabled:text-gray-500 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        >
          <option value="START_YEAR">Inicio de Año</option>
          <option value="CUTOFF">Corte de Año</option>
        </select>
      </div>

      {/* Lectura de Odómetro */}
      <div className="grid grid-cols-3 gap-4">
        <div className="col-span-2">
          <label htmlFor="odometer-reading" className="block text-sm font-medium text-gray-700">
            Lectura del Odómetro *
          </label>
          <input
            id="odometer-reading"
            type="number"
            step="0.1"
            min="0"
            value={odometerReading}
            onChange={(e) => setOdometerReading(e.target.value)}
            placeholder="Ej: 50250.5"
            disabled={disabled || loading}
            required
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm disabled:bg-gray-100 disabled:text-gray-500 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          />
        </div>

        <div>
          <label htmlFor="unit" className="block text-sm font-medium text-gray-700">
            Unidad
          </label>
          <select
            id="unit"
            value={unit}
            onChange={(e) => setUnit(e.target.value as 'Miles' | 'Kilometers')}
            disabled={disabled || loading}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm disabled:bg-gray-100 disabled:text-gray-500 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          >
            <option value="Miles">Millas</option>
            <option value="Kilometers">Kilómetros</option>
          </select>
        </div>
      </div>

      {/* Foto 1: Piloto con Moto */}
      <div>
        <label htmlFor="pilot-photo" className="block text-sm font-medium text-gray-700">
          Foto: Piloto con Moto * {pilotWithBikePhoto && <span className="text-green-600">✓</span>}
        </label>
        <div className="mt-2 flex justify-center rounded-lg border-2 border-dashed border-gray-300 px-6 py-10">
          <div className="text-center">
            {pilotWithBikePhoto ? (
              <div className="space-y-2">
                <p className="text-sm font-medium text-gray-900">{pilotWithBikePhoto.name}</p>
                <p className="text-xs text-gray-500">
                  {(pilotWithBikePhoto.size / 1024 / 1024).toFixed(2)} MB
                </p>
                <button
                  type="button"
                  onClick={() => setPilotWithBikePhoto(null)}
                  className="text-sm text-red-600 hover:text-red-700"
                >
                  Cambiar imagen
                </button>
              </div>
            ) : (
              <>
                <p className="text-sm text-gray-600">
                  Arrastra una imagen aquí o haz clic para seleccionar
                </p>
                <p className="text-xs text-gray-500">PNG, JPG, GIF up to 10MB</p>
              </>
            )}
          </div>
          <input
            id="pilot-photo"
            type="file"
            accept="image/*"
            onChange={(e) => handlePhotoSelect(e, setPilotWithBikePhoto)}
            disabled={disabled || loading}
            required
            className="hidden"
          />
        </div>
        <label htmlFor="pilot-photo" className="mt-2 block cursor-pointer">
          <div className="rounded-md bg-blue-50 px-4 py-2 text-center text-sm font-medium text-blue-700 hover:bg-blue-100">
            Seleccionar imagen
          </div>
        </label>
      </div>

      {/* Foto 2: Close-up Odómetro */}
      <div>
        <label htmlFor="odometer-photo" className="block text-sm font-medium text-gray-700">
          Foto: Close-up Odómetro * {odometerCloseupPhoto && <span className="text-green-600">✓</span>}
        </label>
        <div className="mt-2 flex justify-center rounded-lg border-2 border-dashed border-gray-300 px-6 py-10">
          <div className="text-center">
            {odometerCloseupPhoto ? (
              <div className="space-y-2">
                <p className="text-sm font-medium text-gray-900">{odometerCloseupPhoto.name}</p>
                <p className="text-xs text-gray-500">
                  {(odometerCloseupPhoto.size / 1024 / 1024).toFixed(2)} MB
                </p>
                <button
                  type="button"
                  onClick={() => setOdometerCloseupPhoto(null)}
                  className="text-sm text-red-600 hover:text-red-700"
                >
                  Cambiar imagen
                </button>
              </div>
            ) : (
              <>
                <p className="text-sm text-gray-600">
                  Arrastra una imagen aquí o haz clic para seleccionar
                </p>
                <p className="text-xs text-gray-500">PNG, JPG, GIF up to 10MB</p>
              </>
            )}
          </div>
          <input
            id="odometer-photo"
            type="file"
            accept="image/*"
            onChange={(e) => handlePhotoSelect(e, setOdometerCloseupPhoto)}
            disabled={disabled || loading}
            required
            className="hidden"
          />
        </div>
        <label htmlFor="odometer-photo" className="mt-2 block cursor-pointer">
          <div className="rounded-md bg-blue-50 px-4 py-2 text-center text-sm font-medium text-blue-700 hover:bg-blue-100">
            Seleccionar imagen
          </div>
        </label>
      </div>

      {/* Notas Opcionales */}
      <div>
        <label htmlFor="notes" className="block text-sm font-medium text-gray-700">
          Notas (Opcional)
        </label>
        <textarea
          id="notes"
          value={notes}
          onChange={(e) => setNotes(e.target.value)}
          placeholder="Observaciones adicionales..."
          disabled={disabled || loading}
          rows={3}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm disabled:bg-gray-100 disabled:text-gray-500 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        />
      </div>

      {/* Botón Submit */}
      <button
        type="submit"
        disabled={disabled || loading || !isFormValid}
        className="w-full rounded-lg bg-blue-600 px-4 py-3 font-semibold text-white hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
      >
        {loading ? 'Subiendo evidencia...' : 'Confirmar Asistencia'}
      </button>
    </form>
  );
}
