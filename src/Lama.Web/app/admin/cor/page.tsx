'use client';

import { Suspense } from 'react';
import { CORPageContent } from './page-content';

/**
 * Página COR con Suspense para useSearchParams
 */
export default function CORPage() {
  return (
    <Suspense fallback={<div className="min-h-screen flex items-center justify-center">Cargando...</div>}>
      <CORPageContent />
    </Suspense>
  );
}
