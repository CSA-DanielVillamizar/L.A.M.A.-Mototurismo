'use client';

import React from 'react';
import { CORWizard } from '@/components/CORWizard';
import { LayoutWrapper } from '@/components/layout';

/**
 * Contenido de la página COR (Confirmation of Riding)
 * Wizard de 6 pasos para validación de evidencia
 */
export function CORPageContent() {
  return (
    <LayoutWrapper
      title="Registro de Evidencia - COR"
      breadcrumbs={[
        { label: 'Admin', href: '/admin' },
        { label: 'COR', href: '/admin/cor' },
      ]}
    >
      <div className="max-w-4xl mx-auto">
        <CORWizard />
      </div>
    </LayoutWrapper>
  );
}
