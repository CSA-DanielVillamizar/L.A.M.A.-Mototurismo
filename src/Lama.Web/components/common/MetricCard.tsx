'use client';

/**
 * MetricCard Component
 * Tarjeta para mostrar KPIs y métricas
 */

import React from 'react';
import { LucideIcon } from 'lucide-react';

interface MetricCardProps {
  icon: LucideIcon;
  label: string;
  value: string | number;
  trend?: {
    value: number;
    isPositive: boolean;
  };
  className?: string;
}

export function MetricCard({
  icon: Icon,
  label,
  value,
  trend,
  className = '',
}: MetricCardProps) {
  return (
    <div className={`rounded-lg border border-gray-200 bg-white p-6 ${className}`}>
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-600">{label}</p>
          <p className="mt-2 text-3xl font-bold text-gray-900">{value}</p>
          {trend && (
            <p
              className={`mt-2 text-sm font-medium ${
                trend.isPositive ? 'text-green-600' : 'text-red-600'
              }`}
            >
              {trend.isPositive ? '+' : ''}{trend.value}%
            </p>
          )}
        </div>
        <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-indigo-50">
          <Icon className="h-6 w-6 text-indigo-600" aria-hidden="true" />
        </div>
      </div>
    </div>
  );
}
