'use client';

/**
 * EventStep Component
 * Paso 1: Seleccionar evento
 */

import React, { useState, useEffect } from 'react';
import { Event } from './types';
import { EmptyState } from '@/components/common';
import { Calendar, Loader2 } from 'lucide-react';

interface EventStepProps {
  selectedEvent: Event | null;
  onSelectEvent: (event: Event) => void;
  loading: boolean;
}

export function EventStep({
  selectedEvent,
  onSelectEvent,
  loading,
}: EventStepProps) {
  const [events, setEvents] = useState<Event[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchEvents();
  }, []);

  const fetchEvents = async () => {
    setIsLoading(true);
    setError(null);

    try {
      // TODO: Reemplazar con api-client.get('/events')
      // Simulación: eventos del año actual
      const mockEvents: Event[] = [
        {
          id: '1',
          name: 'Maratón Quito 2025',
          year: 2025,
          date: '2025-03-15',
          location: 'Quito, Ecuador',
        },
        {
          id: '2',
          name: 'Media Maratón Cuenca',
          year: 2025,
          date: '2025-04-20',
          location: 'Cuenca, Ecuador',
        },
        {
          id: '3',
          name: 'Carrera Nocturna Guayaquil',
          year: 2025,
          date: '2025-05-10',
          location: 'Guayaquil, Ecuador',
        },
      ];
      setEvents(mockEvents);
    } catch (err) {
      setError('Error al cargar eventos');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader2 className="h-6 w-6 animate-spin text-indigo-600" />
        <span className="ml-2 text-gray-600">Cargando eventos...</span>
      </div>
    );
  }

  if (events.length === 0) {
    return (
      <EmptyState
        icon={Calendar}
        title="No hay eventos disponibles"
        description="Intenta refrescar o contacta al administrador"
      />
    );
  }

  return (
    <div className="space-y-4">
      <div className="grid gap-3 max-h-96 overflow-y-auto">
        {events.map((event) => (
          <button
            key={event.id}
            onClick={() => onSelectEvent(event)}
            className={`rounded-lg border-2 p-4 text-left transition-all ${
              selectedEvent?.id === event.id
                ? 'border-indigo-600 bg-indigo-50'
                : 'border-gray-200 bg-white hover:border-indigo-300'
            }`}
          >
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <h3 className="font-semibold text-gray-900">{event.name}</h3>
                <div className="mt-2 flex flex-col gap-1 text-sm text-gray-600">
                  <p>
                    <span className="font-medium">Fecha:</span> {formatDate(event.date)}
                  </p>
                  <p>
                    <span className="font-medium">Lugar:</span> {event.location}
                  </p>
                </div>
              </div>
              <div className="ml-4 flex h-6 w-6 items-center justify-center rounded-full border-2 border-gray-300">
                {selectedEvent?.id === event.id && (
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

function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('es-CO', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
}
