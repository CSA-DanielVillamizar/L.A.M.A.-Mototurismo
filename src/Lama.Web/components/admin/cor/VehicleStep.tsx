'use client';

/**
 * VehicleStep Component
 * Paso 3: Seleccionar vehículo
 */

import React, { useState, useEffect } from 'react';
import { Vehicle } from './types';
import { EmptyState } from '@/components/common';
import { Car, Loader2 } from 'lucide-react';

interface VehicleStepProps {
  memberId?: string;
  selectedVehicle: Vehicle | null;
  onSelectVehicle: (vehicle: Vehicle) => void;
  loading: boolean;
}

export function VehicleStep({
  memberId,
  selectedVehicle,
  onSelectVehicle,
  loading,
}: VehicleStepProps) {
  const [vehicles, setVehicles] = useState<Vehicle[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (memberId) {
      fetchVehicles();
    }
  }, [memberId]);

  const fetchVehicles = async () => {
    if (!memberId) return;

    setIsLoading(true);
    setError(null);

    try {
      // TODO: Reemplazar con api-client.get(`/members/${memberId}/vehicles`)
      // Simulación
      const mockVehicles: Vehicle[] = [
        {
          id: '1',
          licensePlate: 'ABC-1234',
          make: 'Toyota',
          model: 'Corolla',
          year: 2022,
        },
        {
          id: '2',
          licensePlate: 'XYZ-5678',
          make: 'Honda',
          model: 'Civic',
          year: 2021,
        },
      ];
      setVehicles(mockVehicles);
    } catch (err) {
      setError('Error al cargar vehículos');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-6 w-6 animate-spin text-indigo-600" />
        <span className="ml-2 text-gray-600">Cargando vehículos...</span>
      </div>
    );
  }

  if (vehicles.length === 0) {
    return (
      <EmptyState
        icon={Car}
        title="No hay vehículos registrados"
        description="Este miembro no tiene vehículos. Contacta al administrador."
      />
    );
  }

  return (
    <div className="space-y-4">
      <div className="grid gap-3">
        {vehicles.map((vehicle) => (
          <button
            key={vehicle.id}
            onClick={() => onSelectVehicle(vehicle)}
            className={`rounded-lg border-2 p-4 text-left transition-all ${
              selectedVehicle?.id === vehicle.id
                ? 'border-indigo-600 bg-indigo-50'
                : 'border-gray-200 bg-white hover:border-indigo-300'
            }`}
          >
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <h3 className="font-semibold text-gray-900">
                  {vehicle.make} {vehicle.model}
                </h3>
                <div className="mt-2 flex flex-col gap-1 text-sm text-gray-600">
                  <p>
                    <span className="font-medium">Placa:</span> {vehicle.licensePlate}
                  </p>
                  <p>
                    <span className="font-medium">Año:</span> {vehicle.year}
                  </p>
                </div>
              </div>
              <div className="ml-4 flex h-6 w-6 items-center justify-center rounded-full border-2 border-gray-300">
                {selectedVehicle?.id === vehicle.id && (
                  <div className="h-4 w-4 rounded-full bg-indigo-600" />
                )}
              </div>
            </div>
          </button>
        ))}
      </div>

      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">
          {error}
        </div>
      )}
    </div>
  );
}
