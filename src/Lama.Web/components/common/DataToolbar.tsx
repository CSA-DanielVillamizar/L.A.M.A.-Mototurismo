'use client';

/**
 * DataToolbar Component
 * Barra de herramientas para buscar, filtrar y acciones en listas
 */

import React from 'react';
import { Search, RotateCcw } from 'lucide-react';

interface DataToolbarProps {
  searchPlaceholder?: string;
  onSearchChange?: (value: string) => void;
  onRefresh?: () => void;
  children?: React.ReactNode; // Para filtros custom
}

export function DataToolbar({
  searchPlaceholder = 'Buscar...',
  onSearchChange,
  onRefresh,
  children,
}: DataToolbarProps) {
  return (
    <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
      <div className="flex-1">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            placeholder={searchPlaceholder}
            onChange={(e) => onSearchChange?.(e.target.value)}
            className="w-full rounded-lg border border-gray-300 bg-white py-2 pl-10 pr-4 text-sm text-gray-900 placeholder:text-gray-500 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            aria-label={searchPlaceholder}
          />
        </div>
      </div>

      <div className="flex gap-2">
        {children}
        {onRefresh && (
          <button
            onClick={onRefresh}
            className="flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            aria-label="Refrescar"
          >
            <RotateCcw className="h-4 w-4" />
            <span className="hidden sm:inline">Refrescar</span>
          </button>
        )}
      </div>
    </div>
  );
}
