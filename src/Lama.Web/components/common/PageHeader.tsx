'use client';

/**
 * PageHeader Component
 * Encabezado premium para páginas con título, subtítulo, icono y acciones
 * Stripe-like design
 */

import React from 'react';
import { LucideIcon } from 'lucide-react';

interface PageHeaderProps {
  icon?: LucideIcon;
  title: string;
  subtitle?: string;
  actions?: React.ReactNode;
}

export function PageHeader({
  icon: Icon,
  title,
  subtitle,
  actions,
}: PageHeaderProps) {
  return (
    <div className="flex flex-col gap-6 md:flex-row md:items-center md:justify-between">
      <div className="flex items-start gap-4">
        {Icon && (
          <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-indigo-50">
            <Icon className="h-6 w-6 text-indigo-600" aria-hidden="true" />
          </div>
        )}
        <div className="flex-1">
          <h1 className="text-3xl font-bold text-gray-900">{title}</h1>
          {subtitle && (
            <p className="mt-1 text-base text-gray-600">{subtitle}</p>
          )}
        </div>
      </div>
      {actions && <div className="flex gap-2">{actions}</div>}
    </div>
  );
}
