'use client';

import { useMemo, useEffect, useState } from 'react';
import type { Event } from '@/types/api';
import { apiClient } from '@/lib/api-client';

interface EventSelectorProps {
  /**
   * Evento seleccionado actualmente
   */
  selectedEvent: Event | null;

  /**
   * Callback cuando cambia la selección
   */
  onEventSelect: (event: Event) => void;

  /**
   * Año para filtrar eventos (por defecto el año actual)
   */
  year?: number;

  /**
   * Permitir selección manual de año
   */
  allowYearSelection?: boolean;

  /**
   * ID de atributo para testing
   */
  testId?: string;
}

/**
 * Componente para seleccionar un evento
 * Carga eventos del backend filtrados por año
 */
export function EventSelector({
  selectedEvent,
  onEventSelect,
  year = new Date().getFullYear(),
  allowYearSelection = false,
  testId,
}: EventSelectorProps) {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedYear, setSelectedYear] = useState(year);

  // Cargar eventos cuando cambia el año
  useEffect(() => {
    const loadEvents = async () => {
      try {
        setLoading(true);
        setError(null);
        const eventsData = await apiClient.getEventsByYear(selectedYear);
        setEvents(eventsData);

        // Limpiar selección si el evento anterior no está en la nueva lista
        if (selectedEvent && !eventsData.find((e) => e.eventId === selectedEvent.eventId)) {
          onEventSelect(null as any);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error al cargar eventos');
        setEvents([]);
      } finally {
        setLoading(false);
      }
    };

    loadEvents();
  }, [selectedYear, selectedEvent, onEventSelect]);

  // Formatea la fecha para mostrar
  const formatDate = useMemo(() => {
    return (dateStr: string) => {
      try {
        const date = new Date(dateStr);
        return date.toLocaleDateString('es-MX', {
          year: 'numeric',
          month: 'short',
          day: 'numeric',
        });
      } catch {
        return dateStr;
      }
    };
  }, []);

  if (error && !allowYearSelection) {
    return (
      <div className="rounded-lg bg-red-50 p-4 text-red-700" data-testid={testId}>
        <p className="font-semibold">Error al cargar eventos</p>
        <p className="text-sm">{error}</p>
      </div>
    );
  }

  return (
    <div className="space-y-3" data-testid={testId}>
      {allowYearSelection && (
        <div>
          <label htmlFor="year" className="block text-sm font-medium text-gray-700">
            Año
          </label>
          <select
            id="year"
            value={selectedYear}
            onChange={(e) => setSelectedYear(parseInt(e.target.value))}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
          >
            {[2024, 2025, 2026].map((y) => (
              <option key={y} value={y}>
                {y}
              </option>
            ))}
          </select>
        </div>
      )}

      <div>
        <label htmlFor="event-select" className="block text-sm font-medium text-gray-700">
          Seleccionar Evento
        </label>
        <select
          id="event-select"
          value={selectedEvent?.eventId || ''}
          onChange={(e) => {
            const event = events.find((ev) => ev.eventId === parseInt(e.target.value));
            if (event) onEventSelect(event);
          }}
          disabled={loading}
          className="mt-1 block w-full rounded-md border-gray-300 shadow-sm disabled:bg-gray-100 disabled:text-gray-500 focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        >
          <option value="">
            {loading ? 'Cargando eventos...' : 'Elige un evento'}
          </option>
          {events.map((event) => (
            <option key={event.eventId} value={event.eventId}>
              {event.eventName} - {formatDate(event.eventDate)} (Clase {event.eventType})
            </option>
          ))}
        </select>
      </div>

      {events.length === 0 && !loading && (
        <p className="text-sm text-amber-600">No hay eventos disponibles para {selectedYear}</p>
      )}
    </div>
  );
}
