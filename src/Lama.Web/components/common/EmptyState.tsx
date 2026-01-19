'use client';

/**
 * EmptyState Component
 * Estado vacío con icono, título, descripción y acción
 * Profesional y minimalista
 */

import React from 'react';
import { LucideIcon } from 'lucide-react';

interface EmptyStateProps {
  icon: LucideIcon;
  title: string;
  description?: string;
  action?: React.ReactNode;
}

export function EmptyState({
  icon: Icon,
  title,
  description,
  action,
}: EmptyStateProps) {
  return (
    <div className="flex min-h-[300px] flex-col items-center justify-center gap-4 rounded-lg border border-dashed border-gray-200 bg-gray-50 py-12 px-6">
      <div className="flex h-16 w-16 items-center justify-center rounded-full bg-gray-100">
        <Icon className="h-8 w-8 text-gray-400" aria-hidden="true" />
      </div>
      
      <div className="text-center">
        <h3 className="text-lg font-semibold text-gray-900">{title}</h3>
        {description && (
          <p className="mt-1 text-sm text-gray-600">{description}</p>
        )}
      </div>
      
      {action && <div className="mt-4">{action}</div>}
    </div>
  );
}
