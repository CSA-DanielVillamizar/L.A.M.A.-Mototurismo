'use client';

/**
 * QueueFilters Component
 * Toolbar de filtros (búsqueda, evento, estado)
 */

import React from 'react';
import { QueueFiltersState } from './types';
import { DataToolbar } from '@/components/common';
import { Button } from '@/components/ui/button';
import { X } from 'lucide-react';

interface QueueFiltersProps {
  filters: QueueFiltersState;
  events: Array<{ id: string; name: string }>;
  onFiltersChange: (filters: QueueFiltersState) => void;
  onRefresh: () => void;
}

export function QueueFilters({
  filters,
  events,
  onFiltersChange,
  onRefresh,
}: QueueFiltersProps) {
  const hasActiveFilters =
    filters.search || filters.eventId || filters.status !== 'all';

  const handleClearFilters = () => {
    onFiltersChange({
      search: '',
      eventId: null,
      status: 'all',
    });
  };

  return (
    <div className="space-y-4">
      <DataToolbar
        searchPlaceholder="Buscar por miembro o placa..."
        onSearchChange={(value) =>
          onFiltersChange({ ...filters, search: value })
        }
        onRefresh={onRefresh}
      >
        {/* Event Filter */}
        <select
          value={filters.eventId || ''}
          onChange={(e) =>
            onFiltersChange({
              ...filters,
              eventId: e.target.value || null,
            })
          }
          className="rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm text-gray-900 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
          aria-label="Filtrar por evento"
        >
          <option value="">Todos los eventos</option>
          {events.map((event) => (
            <option key={event.id} value={event.id}>
              {event.name}
            </option>
          ))}
        </select>

        {/* Status Filter */}
        <select
          value={filters.status || 'all'}
          onChange={(e) =>
            onFiltersChange({
              ...filters,
              status: (e.target.value === 'all' ? null : e.target.value) as QueueFiltersState['status'],
            })
          }
          className="rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm text-gray-900 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
          aria-label="Filtrar por estado"
        >
          <option value="all">Todos los estados</option>
          <option value="pending">Pendiente</option>
          <option value="confirmed">Confirmado</option>
          <option value="rejected">Rechazado</option>
        </select>

        {/* Clear Filters */}
        {hasActiveFilters && (
          <Button
            variant="ghost"
            size="sm"
            onClick={handleClearFilters}
            aria-label="Limpiar filtros"
          >
            <X className="h-4 w-4 mr-1" />
            <span className="hidden sm:inline">Limpiar</span>
          </Button>
        )}
      </DataToolbar>
    </div>
  );
}
