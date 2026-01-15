'use client';

import { useState, useEffect, FormEvent, ChangeEvent } from 'react';
import { apiClient } from '@/lib/api-client';
import type {
  Event,
  MemberSearchResult,
  Vehicle,
  EvidenceUploadResponse,
} from '@/types/api';

export default function EvidenceUploadForm() {
  // Estado para dropdowns dinámicos
  const [events, setEvents] = useState<Event[]>([]);
  const [memberSearchResults, setMemberSearchResults] = useState<MemberSearchResult[]>([]);
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);

  // Estado de carga
  const [loadingEvents, setLoadingEvents] = useState(true);
  const [searchingMembers, setSearchingMembers] = useState(false);
  const [loadingVehicles, setLoadingVehicles] = useState(false);

  // Estado del formulario
  const [formData, setFormData] = useState({
    eventId: '',
    memberId: '',
    memberSearchQuery: '',
    selectedMemberName: '',
    vehicleId: '',
    evidenceType: 'START_YEAR' as 'START_YEAR' | 'CUTOFF',
    odometerReading: '',
    unit: 'Kilometers' as 'Miles' | 'Kilometers',
    readingDate: '',
    notes: '',
  });

  const [pilotPhoto, setPilotPhoto] = useState<File | null>(null);
  const [odometerPhoto, setOdometerPhoto] = useState<File | null>(null);

  // Estado de submission
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitSuccess, setSubmitSuccess] = useState<EvidenceUploadResponse | null>(null);

  // Cargar eventos al montar
  useEffect(() => {
    loadEvents();
  }, []);

  // Buscar miembros cuando cambia el query (debounce)
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      if (formData.memberSearchQuery.length >= 2) {
        searchMembers(formData.memberSearchQuery);
      } else {
        setMemberSearchResults([]);
      }
    }, 300); // 300ms debounce

    return () => clearTimeout(timeoutId);
  }, [formData.memberSearchQuery]);

  // Cargar vehículos cuando se selecciona un miembro
  useEffect(() => {
    if (formData.memberId) {
      loadMemberVehicles(parseInt(formData.memberId));
    } else {
      setVehicles([]);
      setFormData((prev) => ({ ...prev, vehicleId: '' }));
    }
  }, [formData.memberId]);

  const loadEvents = async () => {
    try {
      setLoadingEvents(true);
      const eventList = await apiClient.getEvents();
      setEvents(eventList);
    } catch (error) {
      console.error('Error cargando eventos:', error);
    } finally {
      setLoadingEvents(false);
    }
  };

  const searchMembers = async (query: string) => {
    try {
      setSearchingMembers(true);
      const results = await apiClient.searchMembers(query);
      setMemberSearchResults(results);
    } catch (error) {
      console.error('Error buscando miembros:', error);
      setMemberSearchResults([]);
    } finally {
      setSearchingMembers(false);
    }
  };

  const loadMemberVehicles = async (memberId: number) => {
    try {
      setLoadingVehicles(true);
      const vehicleList = await apiClient.getMemberVehicles(memberId);
      setVehicles(vehicleList);
    } catch (error) {
      console.error('Error cargando vehículos:', error);
      setVehicles([]);
    } finally {
      setLoadingVehicles(false);
    }
  };

  const handleInputChange = (
    e: ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleMemberSelect = (member: MemberSearchResult) => {
    setFormData((prev) => ({
      ...prev,
      memberId: member.memberId.toString(),
      selectedMemberName: member.fullName,
      memberSearchQuery: member.fullName,
    }));
    setMemberSearchResults([]); // Cerrar dropdown
  };

  const handlePilotPhotoChange = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setPilotPhoto(e.target.files[0]);
    }
  };

  const handleOdometerPhotoChange = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setOdometerPhoto(e.target.files[0]);
    }
  };

  const validateForm = (): string | null => {
    if (!formData.eventId || parseInt(formData.eventId) <= 0) {
      return 'Debes seleccionar un evento';
    }
    if (!formData.memberId || parseInt(formData.memberId) <= 0) {
      return 'Debes seleccionar un miembro';
    }
    if (!formData.vehicleId || parseInt(formData.vehicleId) <= 0) {
      return 'Debes seleccionar un vehículo';
    }
    if (!pilotPhoto) {
      return 'Foto del piloto con moto es requerida';
    }
    if (!odometerPhoto) {
      return 'Foto del odómetro es requerida';
    }
    if (!formData.odometerReading || parseFloat(formData.odometerReading) <= 0) {
      return 'Lectura del odómetro es requerida y debe ser mayor a 0';
    }
    return null;
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    // Limpiar estados previos
    setSubmitError(null);
    setSubmitSuccess(null);

    // Validar
    const validationError = validateForm();
    if (validationError) {
      setSubmitError(validationError);
      return;
    }

    try {
      setSubmitting(true);

      const request = {
        eventId: parseInt(formData.eventId),
        memberId: parseInt(formData.memberId),
        vehicleId: parseInt(formData.vehicleId),
        evidenceType: formData.evidenceType,
        pilotWithBikePhoto: pilotPhoto!,
        odometerCloseupPhoto: odometerPhoto!,
        odometerReading: parseFloat(formData.odometerReading),
        unit: formData.unit,
        readingDate: formData.readingDate || undefined,
        notes: formData.notes || undefined,
      };

      const response = await apiClient.uploadEvidence(request);
      setSubmitSuccess(response);

      // Limpiar formulario después de 3 segundos
      setTimeout(() => {
        resetForm();
      }, 3000);
    } catch (error) {
      setSubmitError(
        error instanceof Error ? error.message : 'Error desconocido al subir evidencia'
      );
    } finally {
      setSubmitting(false);
    }
  };

  const resetForm = () => {
    setFormData({
      eventId: '',
      memberId: '',
      memberSearchQuery: '',
      selectedMemberName: '',
      vehicleId: '',
      evidenceType: 'START_YEAR',
      odometerReading: '',
      unit: 'Kilometers',
      readingDate: '',
      notes: '',
    });
    setPilotPhoto(null);
    setOdometerPhoto(null);
    setSubmitSuccess(null);
    setSubmitError(null);
    setMemberSearchResults([]);
    setVehicles([]);
  };

  return (
    <div className="max-w-4xl mx-auto p-6">
      <h1 className="text-3xl font-bold mb-2">Subir Evidencia de Asistencia</h1>
      <p className="text-gray-600 mb-6">
        Completa el formulario para registrar la asistencia de un miembro a un evento
      </p>

      {/* Success Message */}
      {submitSuccess && (
        <div className="mb-6 p-4 bg-green-50 border border-green-200 rounded-lg">
          <h3 className="text-green-800 font-semibold mb-2">✅ {submitSuccess.message}</h3>
          <div className="text-green-700 text-sm space-y-1">
            <p>
              <strong>Puntos otorgados:</strong> {submitSuccess.pointsAwarded} puntos totales
            </p>
            <p>
              <strong>Desglose:</strong> {submitSuccess.pointsPerEvent} puntos por evento +{' '}
              {submitSuccess.pointsPerDistance} puntos por distancia
            </p>
            <p>
              <strong>Clasificación:</strong> {submitSuccess.visitorClass}
            </p>
            <p>
              <strong>ID de asistencia:</strong> {submitSuccess.attendanceId}
            </p>
          </div>
        </div>
      )}

      {/* Error Message */}
      {submitError && (
        <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg">
          <p className="text-red-800 font-semibold">❌ Error</p>
          <p className="text-red-700 text-sm">{submitError}</p>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-6 bg-white p-8 rounded-lg shadow-md">
        {/* Evento */}
        <div>
          <label htmlFor="eventId" className="block text-sm font-medium text-gray-700 mb-2">
            Evento <span className="text-red-500">*</span>
          </label>
          <select
            id="eventId"
            name="eventId"
            value={formData.eventId}
            onChange={handleInputChange}
            disabled={loadingEvents}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            required
          >
            <option value="">
              {loadingEvents ? 'Cargando eventos...' : 'Selecciona un evento'}
            </option>
            {events.map((event) => (
              <option key={event.eventId} value={event.eventId}>
                {event.eventName} - {event.eventDate}
              </option>
            ))}
          </select>
          {!loadingEvents && events.length === 0 && (
            <p className="mt-1 text-sm text-gray-500">No hay eventos disponibles</p>
          )}
        </div>

        {/* Miembro (Autocomplete) */}
        <div className="relative">
          <label htmlFor="memberSearch" className="block text-sm font-medium text-gray-700 mb-2">
            Miembro <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            id="memberSearch"
            name="memberSearchQuery"
            value={formData.memberSearchQuery}
            onChange={handleInputChange}
            placeholder="Buscar por nombre o apellido (min. 2 caracteres)"
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            autoComplete="off"
          />
          {searchingMembers && (
            <div className="absolute right-3 top-11 text-gray-400">
              <svg
                className="animate-spin h-5 w-5"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                ></circle>
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                ></path>
              </svg>
            </div>
          )}

          {/* Dropdown de resultados */}
          {memberSearchResults.length > 0 && (
            <div className="absolute z-10 w-full mt-1 bg-white border border-gray-300 rounded-lg shadow-lg max-h-60 overflow-auto">
              {memberSearchResults.map((member) => (
                <button
                  key={member.memberId}
                  type="button"
                  onClick={() => handleMemberSelect(member)}
                  className="w-full text-left px-4 py-2 hover:bg-blue-50 focus:bg-blue-50 focus:outline-none"
                >
                  <div className="font-medium">{member.fullName}</div>
                  <div className="text-xs text-gray-500">{member.status}</div>
                </button>
              ))}
            </div>
          )}

          {formData.selectedMemberName && (
            <p className="mt-1 text-sm text-green-600">
              ✓ Seleccionado: {formData.selectedMemberName}
            </p>
          )}
        </div>

        {/* Vehículo */}
        <div>
          <label htmlFor="vehicleId" className="block text-sm font-medium text-gray-700 mb-2">
            Vehículo <span className="text-red-500">*</span>
          </label>
          <select
            id="vehicleId"
            name="vehicleId"
            value={formData.vehicleId}
            onChange={handleInputChange}
            disabled={!formData.memberId || loadingVehicles}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-100"
            required
          >
            <option value="">
              {!formData.memberId
                ? 'Primero selecciona un miembro'
                : loadingVehicles
                ? 'Cargando vehículos...'
                : 'Selecciona un vehículo'}
            </option>
            {vehicles.map((vehicle) => (
              <option key={vehicle.vehicleId} value={vehicle.vehicleId}>
                {vehicle.displayName}
              </option>
            ))}
          </select>
          {formData.memberId && !loadingVehicles && vehicles.length === 0 && (
            <p className="mt-1 text-sm text-red-500">
              Este miembro no tiene vehículos registrados
            </p>
          )}
        </div>

        {/* Tipo de Evidencia */}
        <div>
          <label htmlFor="evidenceType" className="block text-sm font-medium text-gray-700 mb-2">
            Tipo de Evidencia <span className="text-red-500">*</span>
          </label>
          <select
            id="evidenceType"
            name="evidenceType"
            value={formData.evidenceType}
            onChange={handleInputChange}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            required
          >
            <option value="START_YEAR">Inicio de Año (START_YEAR)</option>
            <option value="CUTOFF">Corte (CUTOFF)</option>
          </select>
        </div>

        {/* Foto: Piloto con Moto */}
        <div>
          <label htmlFor="pilotPhoto" className="block text-sm font-medium text-gray-700 mb-2">
            Foto: Piloto con Moto <span className="text-red-500">*</span>
          </label>
          <input
            type="file"
            id="pilotPhoto"
            accept="image/*"
            onChange={handlePilotPhotoChange}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            required
          />
          {pilotPhoto && (
            <p className="mt-1 text-sm text-green-600">✓ {pilotPhoto.name}</p>
          )}
        </div>

        {/* Foto: Odómetro */}
        <div>
          <label htmlFor="odometerPhoto" className="block text-sm font-medium text-gray-700 mb-2">
            Foto: Odómetro Close-up <span className="text-red-500">*</span>
          </label>
          <input
            type="file"
            id="odometerPhoto"
            accept="image/*"
            onChange={handleOdometerPhotoChange}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            required
          />
          {odometerPhoto && (
            <p className="mt-1 text-sm text-green-600">✓ {odometerPhoto.name}</p>
          )}
        </div>

        {/* Lectura del Odómetro */}
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label
              htmlFor="odometerReading"
              className="block text-sm font-medium text-gray-700 mb-2"
            >
              Lectura del Odómetro <span className="text-red-500">*</span>
            </label>
            <input
              type="number"
              id="odometerReading"
              name="odometerReading"
              value={formData.odometerReading}
              onChange={handleInputChange}
              step="0.1"
              min="0"
              placeholder="12345.5"
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              required
            />
          </div>

          <div>
            <label htmlFor="unit" className="block text-sm font-medium text-gray-700 mb-2">
              Unidad <span className="text-red-500">*</span>
            </label>
            <select
              id="unit"
              name="unit"
              value={formData.unit}
              onChange={handleInputChange}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              required
            >
              <option value="Kilometers">Kilómetros</option>
              <option value="Miles">Millas</option>
            </select>
          </div>
        </div>

        {/* Fecha de Lectura (opcional) */}
        <div>
          <label htmlFor="readingDate" className="block text-sm font-medium text-gray-700 mb-2">
            Fecha de Lectura (opcional)
          </label>
          <input
            type="date"
            id="readingDate"
            name="readingDate"
            value={formData.readingDate}
            onChange={handleInputChange}
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>

        {/* Notas (opcional) */}
        <div>
          <label htmlFor="notes" className="block text-sm font-medium text-gray-700 mb-2">
            Notas Adicionales (opcional)
          </label>
          <textarea
            id="notes"
            name="notes"
            value={formData.notes}
            onChange={handleInputChange}
            rows={3}
            placeholder="Comentarios adicionales sobre la evidencia..."
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>

        {/* Submit Button */}
        <div className="pt-4">
          <button
            type="submit"
            disabled={submitting}
            className="w-full bg-blue-600 text-white py-3 px-6 rounded-lg font-semibold hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:bg-gray-400 disabled:cursor-not-allowed transition-colors"
          >
            {submitting ? (
              <span className="flex items-center justify-center">
                <svg
                  className="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                >
                  <circle
                    className="opacity-25"
                    cx="12"
                    cy="12"
                    r="10"
                    stroke="currentColor"
                    strokeWidth="4"
                  ></circle>
                  <path
                    className="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  ></path>
                </svg>
                Subiendo evidencia...
              </span>
            ) : (
              'Subir Evidencia'
            )}
          </button>
        </div>
      </form>

      {/* Info sobre eventos disponibles */}
      {!loadingEvents && events.length > 0 && (
        <div className="mt-6 text-sm text-gray-500">
          <p>
            📅 {events.length} evento{events.length !== 1 ? 's' : ''} disponible
            {events.length !== 1 ? 's' : ''}
          </p>
        </div>
      )}
    </div>
  );
}
