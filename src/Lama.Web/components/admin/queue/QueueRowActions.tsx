'use client';

import React, { useState } from 'react';
import { QueueAttendee } from './types';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { ClipboardCheck, Eye, Loader2 } from 'lucide-react';

interface QueueRowActionsProps {
  attendee: QueueAttendee;
  isProcessing?: boolean;
  onValidate: (attendee: QueueAttendee) => void;
  onViewDetail: (attendee: QueueAttendee) => void;
}

export function QueueRowActions({
  attendee,
  isProcessing = false,
  onValidate,
  onViewDetail,
}: QueueRowActionsProps) {
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  const handleViewDetail = () => {
    setIsDetailOpen(true);
    onViewDetail(attendee);
  };

  return (
    <>
      <div className="flex items-center justify-center gap-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => onValidate(attendee)}
          disabled={attendee.status === 'confirmed' || isProcessing}
          aria-label="Validar asistencia"
        >
          {isProcessing ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <ClipboardCheck className="h-4 w-4" />
          )}
        </Button>

        <Button
          variant="ghost"
          size="sm"
          onClick={handleViewDetail}
          aria-label="Ver detalle"
        >
          <Eye className="h-4 w-4" />
        </Button>
      </div>

      {/* Detail Dialog */}
      <Dialog open={isDetailOpen} onOpenChange={setIsDetailOpen}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Detalle de Asistencia</DialogTitle>
            <DialogDescription>
              Información completa del registro
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4">
            <div className="rounded-lg border border-gray-200 bg-gray-50 p-4 space-y-3">
              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Miembro</span>
                <span className="text-sm font-medium text-gray-900">
                  {attendee.memberName}
                </span>
              </div>

              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Email</span>
                <span className="text-sm text-gray-900">
                  {attendee.memberEmail}
                </span>
              </div>

              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Evento</span>
                <span className="text-sm font-medium text-gray-900">
                  {attendee.eventName}
                </span>
              </div>

              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Vehículo</span>
                <span className="text-sm text-gray-900">
                  {attendee.vehicleMake} {attendee.vehicleModel} ({attendee.vehiclePlate})
                </span>
              </div>

              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Odómetro</span>
                <span className="text-sm font-mono text-gray-900">
                  {attendee.odometer.toLocaleString()} {attendee.odometerUnit}
                </span>
              </div>

              <div className="flex justify-between">
                <span className="text-sm text-gray-600">Estado</span>
                <span className="text-sm capitalize text-gray-900">
                  {attendee.status}
                </span>
              </div>

              {attendee.pointsAwarded && (
                <div className="flex justify-between">
                  <span className="text-sm text-gray-600">Puntos</span>
                  <span className="text-sm font-bold text-indigo-600">
                    +{attendee.pointsAwarded}
                  </span>
                </div>
              )}

              {attendee.notes && (
                <div>
                  <span className="text-sm text-gray-600 block mb-1">Notas</span>
                  <p className="text-sm text-gray-900">{attendee.notes}</p>
                </div>
              )}
            </div>

            <Button
              onClick={() => setIsDetailOpen(false)}
              className="w-full"
            >
              Cerrar
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
}
