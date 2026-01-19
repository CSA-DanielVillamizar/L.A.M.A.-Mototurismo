'use client';

import React, { useState, useCallback, useEffect } from 'react';
import { AlertCircle, CheckCircle2, ChevronLeft } from 'lucide-react';
import { PageHeader, SectionCard } from '@/components/common';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { EventStep } from './EventStep';
import { MemberStep } from './MemberStep';
import { VehicleStep } from './VehicleStep';
import { EvidenceStep } from './EvidenceStep';
import { SuccessCard } from './SuccessCard';
import { apiClient } from '@/lib/api-client';
import {
  CorFlowState,
  CorStep,
  Event,
  Member,
  Vehicle,
  EvidenceForm,
  CorValidationResponse,
  CorStepperProps,
} from './types';

const STEPS: { id: CorStep; label: string; number: number }[] = [
  { id: 'event', label: 'Evento', number: 1 },
  { id: 'member', label: 'Miembro', number: 2 },
  { id: 'vehicle', label: 'Vehículo', number: 3 },
  { id: 'evidence', label: 'Evidencia', number: 4 },
];

export function CorStepper({ onComplete, initialEventId, initialMemberId, initialVehicleId }: CorStepperProps) {
  const [state, setState] = useState<CorFlowState>({
    currentStep: 'event',
    event: null,
    member: null,
    vehicle: null,
    evidence: null,
    loading: false,
    error: null,
  });

  // Cargar datos iniciales si vienen params (deep-link desde /admin/queue)
  useEffect(() => {
    const loadInitialData = async () => {
      if (!initialEventId && !initialMemberId && !initialVehicleId) return;

      setState((prev) => ({ ...prev, loading: true }));

      try {
        const loadedData: Partial<CorFlowState> = {};

        if (initialEventId) {
          const eventData = await apiClient.getEventById(parseInt(initialEventId, 10));
          loadedData.event = {
            id: eventData.eventId.toString(),
            name: eventData.eventName,
            year: new Date(eventData.eventDate).getFullYear(),
            date: eventData.eventDate,
            location: '', // Backend no proporciona location
          };
        }

        if (initialMemberId) {
          const member = await apiClient.getMemberById(parseInt(initialMemberId, 10));
          loadedData.member = {
            id: member.memberId.toString(),
            firstName: member.firstName,
            lastName: member.lastName,
            email: '', // Backend no proporciona email en MemberSearchResult
            chapter: member.chapterId.toString(),
          };
        }

        if (initialVehicleId) {
          // Asumir que se puede obtener el vehículo
          // Por ahora, crear un vehículo placeholder con el ID
          loadedData.vehicle = {
            id: initialVehicleId,
            licensePlate: '',
            make: '',
            model: '',
            year: new Date().getFullYear(),
          };
        }

        setState((prev) => ({
          ...prev,
          ...loadedData,
          currentStep: initialEventId && initialMemberId && initialVehicleId ? 'evidence' : prev.currentStep,
          loading: false,
        }));
      } catch (error) {
        setState((prev) => ({
          ...prev,
          error: error instanceof Error ? error.message : 'Error al cargar datos iniciales',
          loading: false,
        }));
      }
    };

    loadInitialData();
  }, [initialEventId, initialMemberId, initialVehicleId]);

  // Actualizar evento (reinicia member, vehicle, evidence)
  const handleSelectEvent = useCallback((event: Event) => {
    setState((prev) => ({
      ...prev,
      event,
      member: null,
      vehicle: null,
      evidence: null,
      error: null,
    }));
  }, []);

  // Avanzar a miembro después de evento
  const handleEventContinue = useCallback(() => {
    if (!state.event) {
      setState((prev) => ({
        ...prev,
        error: 'Selecciona un evento para continuar',
      }));
      return;
    }
    setState((prev) => ({
      ...prev,
      currentStep: 'member',
      error: null,
    }));
  }, [state.event]);

  // Actualizar miembro (reinicia vehicle, evidence)
  const handleSelectMember = useCallback((member: Member) => {
    setState((prev) => ({
      ...prev,
      member,
      vehicle: null,
      evidence: null,
      error: null,
    }));
  }, []);

  // Avanzar a vehículo después de miembro
  const handleMemberContinue = useCallback(() => {
    if (!state.member) {
      setState((prev) => ({
        ...prev,
        error: 'Selecciona un miembro para continuar',
      }));
      return;
    }
    setState((prev) => ({
      ...prev,
      currentStep: 'vehicle',
      error: null,
    }));
  }, [state.member]);

  // Actualizar vehículo (reinicia evidence)
  const handleSelectVehicle = useCallback((vehicle: Vehicle) => {
    setState((prev) => ({
      ...prev,
      vehicle,
      evidence: null,
      error: null,
    }));
  }, []);

  // Avanzar a evidencia después de vehículo
  const handleVehicleContinue = useCallback(() => {
    if (!state.vehicle) {
      setState((prev) => ({
        ...prev,
        error: 'Selecciona un vehículo para continuar',
      }));
      return;
    }
    setState((prev) => ({
      ...prev,
      currentStep: 'evidence',
      error: null,
    }));
  }, [state.vehicle]);

  // Actualizar evidencia
  const handleEvidenceChange = useCallback((evidence: EvidenceForm) => {
    setState((prev) => ({
      ...prev,
      evidence,
      error: null,
    }));
  }, []);

  // Enviar validación
  const handleSubmitEvidence = useCallback(async () => {
    if (!state.event || !state.member || !state.vehicle || !state.evidence) {
      setState((prev) => ({
        ...prev,
        error: 'Todos los campos son requeridos',
      }));
      return;
    }

    setState((prev) => ({ ...prev, loading: true, error: null }));

    try {
      // TODO: Llamar API con api-client.post('/cors/validate', payload)
      // Por ahora simular respuesta exitosa
      const response: CorValidationResponse = {
        attendanceId: `COR-${Date.now()}`,
        memberId: state.member.id,
        eventId: state.event.id,
        pointsAwarded: 50,
        pointsBreakdown: {
          basePoints: 40,
          bonusPoints: 10,
          description: 'Validación manual + bonificación de evento especial',
        },
        timestamp: new Date().toISOString(),
      };

      setState((prev) => ({
        ...prev,
        currentStep: 'success',
        successData: response,
        loading: false,
      }));

      onComplete?.(response);
    } catch (error) {
      setState((prev) => ({
        ...prev,
        error: error instanceof Error ? error.message : 'Error al enviar evidencia',
        loading: false,
      }));
    }
  }, [state.event, state.member, state.vehicle, state.evidence, onComplete]);

  // Reiniciar flujo
  const handleReset = useCallback(() => {
    setState({
      currentStep: 'event',
      event: null,
      member: null,
      vehicle: null,
      evidence: null,
      loading: false,
      error: null,
    });
  }, []);

  // Retroceder un paso
  const handleBack = useCallback(() => {
    const stepIndex = STEPS.findIndex((s) => s.id === state.currentStep);
    if (stepIndex > 0) {
      setState((prev) => ({
        ...prev,
        currentStep: STEPS[stepIndex - 1].id,
        error: null,
      }));
    }
  }, [state.currentStep]);

  // Mostrar página de éxito
  if (state.currentStep === 'success' && state.successData) {
    return (
      <SuccessCard
        data={state.successData}
        onValidateAnother={handleReset}
      />
    );
  }

  const currentStepObj = STEPS.find((s) => s.id === state.currentStep);
  const currentStepIndex = STEPS.findIndex((s) => s.id === state.currentStep);

  return (
    <div className="space-y-6">
      {/* Header */}
      <PageHeader
        icon={CheckCircle2}
        title="Validar COR"
        subtitle="Completa los pasos para registrar asistencia a evento"
        actions={
          <div className="flex gap-2">
            <Button variant="outline" size="sm">
              Ayuda
            </Button>
            <Button variant="ghost" size="sm">
              Refrescar
            </Button>
          </div>
        }
      />

      {/* Stepper (Visual Indicator) */}
      <div className="flex items-center justify-between gap-2">
        {STEPS.map((step, idx) => (
          <React.Fragment key={step.id}>
            <div className="flex flex-col items-center gap-2">
              <button
                onClick={() => {
                  if (idx < currentStepIndex) {
                    setState((prev) => ({
                      ...prev,
                      currentStep: step.id,
                      error: null,
                    }));
                  }
                }}
                className={`flex h-10 w-10 items-center justify-center rounded-full font-semibold transition-colors ${
                  idx <= currentStepIndex
                    ? 'bg-indigo-600 text-white'
                    : 'bg-gray-200 text-gray-600'
                }`}
                disabled={idx > currentStepIndex}
              >
                {idx < currentStepIndex ? (
                  <CheckCircle2 className="h-5 w-5" />
                ) : (
                  step.number
                )}
              </button>
              <span
                className={`text-xs font-medium ${
                  idx <= currentStepIndex ? 'text-indigo-600' : 'text-gray-500'
                }`}
              >
                {step.label}
              </span>
            </div>

            {idx < STEPS.length - 1 && (
              <div
                className={`mb-6 h-0.5 flex-1 ${
                  idx < currentStepIndex ? 'bg-indigo-600' : 'bg-gray-200'
                }`}
              />
            )}
          </React.Fragment>
        ))}
      </div>

      {/* Error Banner (con aria-live) */}
      {state.error && (
        <Alert variant="destructive" role="alert" aria-live="assertive">
          <AlertCircle className="h-4 w-4" />
          <AlertDescription>{state.error}</AlertDescription>
        </Alert>
      )}

      {/* Loading announcement (screen reader only) */}
      {state.loading && (
        <div className="sr-only" role="status" aria-live="polite">
          Procesando...
        </div>
      )}

      {/* Current Step Card */}
      <SectionCard
        title={currentStepObj?.label}
        subtitle={getStepSubtitle(state.currentStep)}
      >
        {state.currentStep === 'event' && (
          <EventStep
            selectedEvent={state.event}
            onSelectEvent={handleSelectEvent}
            loading={state.loading}
          />
        )}

        {state.currentStep === 'member' && (
          <MemberStep
            eventId={state.event?.id}
            selectedMember={state.member}
            onSelectMember={handleSelectMember}
            loading={state.loading}
          />
        )}

        {state.currentStep === 'vehicle' && (
          <VehicleStep
            memberId={state.member?.id}
            selectedVehicle={state.vehicle}
            onSelectVehicle={handleSelectVehicle}
            loading={state.loading}
          />
        )}

        {state.currentStep === 'evidence' && (
          <EvidenceStep
            evidence={state.evidence}
            onChangeEvidence={handleEvidenceChange}
            loading={state.loading}
          />
        )}
      </SectionCard>

      {/* Action Buttons */}
      <div className="flex justify-between gap-3">
        <Button
          variant="outline"
          onClick={handleBack}
          disabled={currentStepIndex === 0 || state.loading}
        >
          <ChevronLeft className="h-4 w-4 mr-1" />
          Atrás
        </Button>

        {state.currentStep === 'evidence' ? (
          <Button
            onClick={handleSubmitEvidence}
            disabled={!state.evidence || state.loading}
          >
            {state.loading ? 'Enviando...' : 'Confirmar Validación'}
          </Button>
        ) : (
          <Button
            onClick={() => {
              if (state.currentStep === 'event') handleEventContinue();
              else if (state.currentStep === 'member') handleMemberContinue();
              else if (state.currentStep === 'vehicle') handleVehicleContinue();
            }}
            disabled={state.loading}
          >
            Continuar
          </Button>
        )}
      </div>
    </div>
  );
}

function getStepSubtitle(step: CorStep): string {
  const subtitles: Record<CorStep, string> = {
    event: 'Selecciona el evento al que asistió',
    member: 'Busca y selecciona el miembro',
    vehicle: 'Selecciona el vehículo utilizado',
    evidence: 'Sube evidencia (fotos y datos del viaje)',
    success: 'Validación completada',
  };
  return subtitles[step];
}
