'use client';

/**
 * Página COR Premium - Flujo de validación de 4 pasos
 * Usa CorStepper para gestionar el estado y navegación del flujo
 * Soporta query params: eventId, memberId, vehicleId
 */

import { Suspense } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import { CorStepper } from '@/components/admin/cor';
import { CorValidationResponse } from '@/components/admin/cor/types';
import { Button } from '@/components/ui/button';
import { ArrowLeft } from 'lucide-react';
import Link from 'next/link';

function CORPageContent() {
  const searchParams = useSearchParams();
  const router = useRouter();

  // Deep-link params desde /admin/queue
  const eventId = searchParams.get('eventId');
  const memberId = searchParams.get('memberId');
  const vehicleId = searchParams.get('vehicleId');

  const handleValidationComplete = (data: CorValidationResponse) => {
    console.log('Validación completada:', data);
    // Podría redirigir automáticamente a /admin/queue después de 3 segundos
  };

  const handleCancel = () => {
    router.push('/admin/queue');
  };

  return (
    <div className="space-y-4">
      {/* Botón volver a queue */}
      <div>
        <Link href="/admin/queue">
          <Button variant="ghost" size="sm">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Volver a Cola
          </Button>
        </Link>
      </div>

      <CorStepper
        onComplete={handleValidationComplete}
        initialEventId={eventId || undefined}
        initialMemberId={memberId || undefined}
        initialVehicleId={vehicleId || undefined}
      />
    </div>
  );
}

export default function CORPage() {
  return (
    <Suspense
      fallback={
        <div className="flex items-center justify-center py-12">
          Cargando...
        </div>
      }
    >
      <CORPageContent />
    </Suspense>
  );
}
