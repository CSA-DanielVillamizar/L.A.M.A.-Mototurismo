'use client';

/**
 * Página de Gestión de Eventos
 * Lista y administra los eventos del sistema
 */

import { Suspense, useEffect, useState } from 'react';
import { Breadcrumbs } from '@/components/layout/AppShell';
import { Loading } from '@/components/ui/loading';
import { Calendar } from 'lucide-react';

interface Event {
  id: string;
  name: string;
  date: string;
  location: string;
  status: string;
}

function EventsContent() {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        setLoading(true);
        const response = await fetch('http://localhost:5000/api/v1/events');
        if (!response.ok) throw new Error('Error al cargar eventos');
        const data = await response.json();
        setEvents(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, []);

  return (
    <div className="space-y-6">
      <Breadcrumbs items={[{ label: 'Admin' }, { label: 'Eventos' }]} />

      <div className="px-6">
        <div className="flex items-center gap-3 mb-6">
          <Calendar className="text-primary-600" size={28} />
          <h1 className="text-3xl font-bold text-primary-900">Eventos</h1>
        </div>

        {loading && <Loading message="Cargando eventos..." />}

        {error && (
          <div className="rounded-lg bg-danger-50 border border-danger-200 p-4 text-danger-700">
            {error}
          </div>
        )}

        {!loading && events.length === 0 && (
          <div className="rounded-lg bg-neutral-50 border border-neutral-200 p-8 text-center text-neutral-600">
            <Calendar size={48} className="mx-auto mb-4 text-neutral-400" />
            <p>No hay eventos registrados</p>
          </div>
        )}

        {!loading && events.length > 0 && (
          <div className="grid gap-4">
            {events.map((event) => (
              <div
                key={event.id}
                className="rounded-lg border border-neutral-200 p-4 hover:shadow-md transition-shadow"
              >
                <h3 className="font-semibold text-primary-900">{event.name}</h3>
                <p className="text-sm text-neutral-600">{event.date}</p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default function EventsPage() {
  return (
    <Suspense fallback={<Loading message="Cargando página de eventos..." />}>
      <EventsContent />
    </Suspense>
  );
}
