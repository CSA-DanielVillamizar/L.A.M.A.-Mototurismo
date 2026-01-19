'use client';

/**
 * SectionCard Component
 * Card para agrupar contenido relacionado con header, body y footer
 */

import React from 'react';

interface SectionCardProps {
  title?: string;
  subtitle?: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
  className?: string;
}

export function SectionCard({
  title,
  subtitle,
  children,
  footer,
  className = '',
}: SectionCardProps) {
  return (
    <div className={`rounded-lg border border-gray-200 bg-white ${className}`}>
      {(title || subtitle) && (
        <div className="border-b border-gray-100 px-6 py-4">
          {title && <h2 className="text-lg font-semibold text-gray-900">{title}</h2>}
          {subtitle && <p className="mt-1 text-sm text-gray-600">{subtitle}</p>}
        </div>
      )}
      
      <div className="px-6 py-4">{children}</div>
      
      {footer && (
        <div className="border-t border-gray-100 px-6 py-4">{footer}</div>
      )}
    </div>
  );
}
