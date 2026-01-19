'use client';

/**
 * StatusPill Component
 * Badge para mostrar estados (confirmed, pending, rejected, etc.)
 * Con colores semánticos
 */

import React from 'react';

type StatusType =
  | 'confirmed'
  | 'pending'
  | 'rejected'
  | 'processing'
  | 'completed'
  | 'draft';

interface StatusPillProps {
  status: StatusType;
  label?: string;
}

const statusConfig: Record<
  StatusType,
  { bg: string; text: string; dot: string }
> = {
  confirmed: {
    bg: 'bg-green-50',
    text: 'text-green-700',
    dot: 'bg-green-600',
  },
  pending: {
    bg: 'bg-yellow-50',
    text: 'text-yellow-700',
    dot: 'bg-yellow-600',
  },
  rejected: {
    bg: 'bg-red-50',
    text: 'text-red-700',
    dot: 'bg-red-600',
  },
  processing: {
    bg: 'bg-blue-50',
    text: 'text-blue-700',
    dot: 'bg-blue-600',
  },
  completed: {
    bg: 'bg-green-50',
    text: 'text-green-700',
    dot: 'bg-green-600',
  },
  draft: {
    bg: 'bg-gray-50',
    text: 'text-gray-700',
    dot: 'bg-gray-400',
  },
};

const statusLabels: Record<StatusType, string> = {
  confirmed: 'Confirmado',
  pending: 'Pendiente',
  rejected: 'Rechazado',
  processing: 'Procesando',
  completed: 'Completado',
  draft: 'Borrador',
};

export function StatusPill({ status, label }: StatusPillProps) {
  const config = statusConfig[status];
  const displayLabel = label || statusLabels[status];

  return (
    <span
      className={`inline-flex items-center gap-2 rounded-full px-3 py-1 text-sm font-medium ${config.bg} ${config.text}`}
    >
      <span className={`h-2 w-2 rounded-full ${config.dot}`} aria-hidden="true" />
      {displayLabel}
    </span>
  );
}
