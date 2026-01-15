'use client';

import { useEffect, useState, useCallback } from 'react';
import type { Vehicle } from '@/types/api';
import { apiClient } from '@/lib/api-client';

interface VehicleSelectorProps {
  /**
   * ID del miembro cuya  motocicleta se selecciona
   */
  memberId: number | null;

  /**
   * Vehículo seleccionado actualmente
   */
  selectedVehicle: Vehicle | null;

  /**
   * Callback cuando cambia la selección
   */
  onVehicleSelect: (vehicle: Vehicle | null) => void;

  /**
   * ID de atributo para testing
   */
  testId?: string;
}

/**
 * Componente para seleccionar un vehículo de un miembro
 * Carga vehículos desde el backend cuando se proporciona un memberId
 */
export function VehicleSelector({
  memberId,
  selectedVehicle,
  onVehicleSelect,
  testId,
}: VehicleSelectorProps) {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Cargar vehículos cuando cambia el miembro
  useEffect(() => {
    if (!memberId) {
      setVehicles([]);
      onVehicleSelect(null);
      return;
    }

    const loadVehicles = async () => {
      try {
        setLoading(true);
        setError(null);
        const vehiclesData = await apiClient.adminGetMemberVehicles(memberId);
        setVehicles(vehiclesData);

        // Limpiar selección si hay cambio de miembro
        if (selectedVehicle?.memberId !== memberId) {
          onVehicleSelect(null);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error al cargar vehículos');
        setVehicles([]);
        onVehicleSelect(null);
      } finally {
        setLoading(false);
      }
    };

    loadVehicles();
  }, [memberId, selectedVehicle?.memberId, onVehicleSelect]);

  const handleSelect = useCallback(
    (vehicleId: string) => {
      if (!vehicleId) {
        onVehicleSelect(null);
        return;
      }

      const vehicle = vehicles.find((v) => v.vehicleId === parseInt(vehicleId));
      if (vehicle) {
        onVehicleSelect(vehicle);
      }
    },
    [vehicles, onVehicleSelect]
  );

  if (!memberId) {
    return (
      <div className="rounded-lg bg-gray-50 p-4" data-testid={testId}>
        <p className="text-sm text-gray-500">Selecciona un miembro primero para ver sus vehículos</p>
      </div>
    );
  }

  return (
    <div className="space-y-3" data-testid={testId}>
      <div>
        <label htmlFor="vehicle-select" className="block text-sm font-medium text-gray-700">
          Seleccionar Moto/Vehículo
        </label>
        <select
          id="vehicle-select"
          value={selectedVehicle?.vehicleId || ''}
          onChange={(e) => handleSelect(e.target.value)}
          disabled={loading || vehicles.length === 0}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm disabled:bg-gray-100 disabled:text-gray-500 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        >
          <option value="">
            {loading ? 'Cargando vehículos...' : 'Elige un vehículo'}
          </option>
          {vehicles.map((vehicle) => (
            <option key={vehicle.vehicleId} value={vehicle.vehicleId}>
              {vehicle.motorcycleData} - {vehicle.licPlate}{vehicle.trike ? ' (Trike)' : ''}
            </option>
          ))}
        </select>
      </div>

      {error && (
        <p className="text-sm text-red-600">Error: {error}</p>
      )}

      {vehicles.length === 0 && !loading && !error && (
        <p className="text-sm text-amber-600">Este miembro no tiene vehículos registrados</p>
      )}
    </div>
  );
}
