'use client';

import React, { useState, useCallback } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { apiClient } from '@/lib/api-client';
import type { UploadEvidenceRequest } from '@/types/api';
import {
  WizardContainer,
  WizardStep,
  StepIndicator,
  FormCard,
  UploadZone,
  SuccessModal,
  ErrorModal,
} from '@/components/wizard';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import { Card } from '@/components/ui/card';
import { EventSelector } from '@/components/EventSelector';
import { MemberSearchAutocomplete } from '@/components/MemberSearchAutocomplete';
import { VehicleSelector } from '@/components/VehicleSelector';

const WIZARD_STEPS = [
  {
    title: 'Seleccionar Evento',
    description: 'Elige el evento en el que participaste',
  },
  {
    title: 'Buscar Miembro',
    description: 'Encuentra tu perfil en el sistema',
  },
  {
    title: 'Seleccionar Vehículo',
    description: 'Elige el vehículo con el que participaste',
  },
  {
    title: 'Detalles de Evidencia',
    description: 'Completa la información de la prueba',
  },
  {
    title: 'Cargar Fotos',
    description: 'Sube las fotos de la evidencia',
  },
  {
    title: 'Revisión y Envío',
    description: 'Verifica toda la información antes de enviar',
  },
];

/**
 * Tipos de evidencia soportados
 */
type EvidenceType = 'START_YEAR' | 'CUTOFF';

/**
 * Estructura de datos del wizard
 */
interface WizardData {
  // Paso 1: Evento
  eventId: number | null;

  // Paso 2: Miembro
  memberId: number | null;

  // Paso 3: Vehículo
  vehicleId: number | null;
  licPlate: string;
  motorcycleData: string;

  // Paso 4: Detalles
  evidenceType: EvidenceType;
  readingDate: string;
  odometerReading: number | null;
  unit: 'Kilometers' | 'Miles';
  notes: string;
  trike: boolean;

  // Paso 5: Fotos
  pilotWithBikePhoto: File | null;
  odometerCloseupPhoto: File | null;
}

/**
 * Componente principal del wizard COR
 */
export function CORWizard() {
  const router = useRouter();
  const searchParams = useSearchParams();

  // Estado del wizard
  const [currentStep, setCurrentStep] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  // Datos del formulario
  const [wizardData, setWizardData] = useState<WizardData>({
    eventId: null,
    memberId: null,
    vehicleId: null,
    licPlate: '',
    motorcycleData: '',
    evidenceType: 'START_YEAR',
    readingDate: new Date().toISOString().split('T')[0],
    odometerReading: null,
    unit: 'Kilometers',
    notes: '',
    trike: false,
    pilotWithBikePhoto: null,
    odometerCloseupPhoto: null,
  });

  // Modales
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const [showErrorModal, setShowErrorModal] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  /**
   * Validaciones por paso
   */
  const validateStep = useCallback((step: number): boolean => {
    switch (step) {
      case 0:
        return wizardData.eventId !== null;
      case 1:
        return wizardData.memberId !== null;
      case 2:
        return wizardData.vehicleId !== null;
      case 3:
        return (
          wizardData.readingDate !== '' &&
          wizardData.odometerReading !== null &&
          wizardData.odometerReading > 0
        );
      case 4:
        return (
          wizardData.pilotWithBikePhoto !== null &&
          wizardData.odometerCloseupPhoto !== null
        );
      default:
        return true;
    }
  }, [wizardData]);

  /**
   * Navegación al siguiente paso
   */
  const handleNext = useCallback(async () => {
    if (!validateStep(currentStep)) {
      return;
    }
    setCurrentStep((prev) => Math.min(prev + 1, WIZARD_STEPS.length - 1));
  }, [currentStep, validateStep]);

  /**
   * Navegación al paso anterior
   */
  const handlePrevious = useCallback(() => {
    setCurrentStep((prev) => Math.max(prev - 1, 0));
  }, []);

  /**
   * Envío del formulario
   */
  const handleFinish = useCallback(async () => {
    setIsLoading(true);

    try {
      const request: UploadEvidenceRequest = {
        eventId: wizardData.eventId!,
        vehicleId: wizardData.vehicleId!,
        evidenceType: wizardData.evidenceType,
        pilotWithBikePhoto: wizardData.pilotWithBikePhoto!,
        odometerCloseupPhoto: wizardData.odometerCloseupPhoto!,
        odometerReading: wizardData.odometerReading!,
        unit: wizardData.unit,
        readingDate: wizardData.readingDate,
        notes: wizardData.notes,
        licPlate: wizardData.licPlate,
        motorcycleData: wizardData.motorcycleData,
        trike: wizardData.trike,
      };

      await apiClient.uploadEvidence(request);
      setShowSuccessModal(true);

      // Resetear formulario después de 2 segundos
      setTimeout(() => {
        setWizardData({
          eventId: null,
          memberId: null,
          vehicleId: null,
          licPlate: '',
          motorcycleData: '',
          evidenceType: 'START_YEAR',
          readingDate: new Date().toISOString().split('T')[0],
          odometerReading: null,
          unit: 'Kilometers',
          notes: '',
          trike: false,
          pilotWithBikePhoto: null,
          odometerCloseupPhoto: null,
        });
        setCurrentStep(0);
      }, 2000);
    } catch (error) {
      const message =
        error instanceof Error ? error.message : 'Error desconocido';
      setErrorMessage(message);
      setShowErrorModal(true);
    } finally {
      setIsLoading(false);
    }
  }, [wizardData]);

  /**
   * Actualizar datos del wizard
   */
  const updateWizardData = useCallback(
    (updates: Partial<WizardData>) => {
      setWizardData((prev) => ({ ...prev, ...updates }));
    },
    []
  );

  return (
    <>
      <WizardContainer
        totalSteps={WIZARD_STEPS.length}
        currentStep={currentStep}
        onNext={handleNext}
        onPrevious={handlePrevious}
        onFinish={handleFinish}
        isNextDisabled={!validateStep(currentStep)}
        isLoading={isLoading}
        stepIndicator={
          <StepIndicator
            totalSteps={WIZARD_STEPS.length}
            currentStep={currentStep}
            labels={WIZARD_STEPS.map((s) => s.title)}
          />
        }
      >
        {/* PASO 1: Seleccionar Evento */}
        <WizardStep
          stepNumber={1}
          title={WIZARD_STEPS[0].title}
          description={WIZARD_STEPS[0].description}
          isActive={currentStep === 0}
        >
          <FormCard
            title="Evento"
            description="Selecciona el evento en el que registrarás tu evidencia"
          >
            <EventSelector
              selectedEvent={wizardData.eventId ? { eventId: wizardData.eventId } as any : null}
              onEventSelect={(event) => updateWizardData({ eventId: event.eventId })}
            />
          </FormCard>
        </WizardStep>

        {/* PASO 2: Buscar Miembro */}
        <WizardStep
          stepNumber={2}
          title={WIZARD_STEPS[1].title}
          description={WIZARD_STEPS[1].description}
          isActive={currentStep === 1}
        >
          <FormCard
            title="Participante"
            description="Busca tu perfil en el sistema"
          >
            <MemberSearchAutocomplete
              selectedMember={wizardData.memberId ? { memberId: wizardData.memberId } as any : null}
              onMemberSelect={(member) => updateWizardData({ memberId: member.memberId })}
            />
          </FormCard>
        </WizardStep>

        {/* PASO 3: Seleccionar Vehículo */}
        <WizardStep
          stepNumber={3}
          title={WIZARD_STEPS[2].title}
          description={WIZARD_STEPS[2].description}
          isActive={currentStep === 2}
        >
          <FormCard
            title="Vehículo"
            description="Selecciona el vehículo con el que participaste"
          >
            {wizardData.memberId ? (
              <VehicleSelector
                memberId={wizardData.memberId}
                selectedVehicle={wizardData.vehicleId ? { vehicleId: wizardData.vehicleId, licPlate: wizardData.licPlate, displayName: wizardData.motorcycleData } as any : null}
                onVehicleSelect={(vehicle) => {
                  if (vehicle) {
                    updateWizardData({ 
                      vehicleId: vehicle.vehicleId,
                      licPlate: vehicle.licensePlate,
                      motorcycleData: vehicle.displayName
                    });
                  }
                }}
              />
            ) : (
              <div className="p-4 rounded-lg bg-warning-50 border border-warning-200">
                <p className="text-sm text-warning-800">
                  Primero debes seleccionar un miembro
                </p>
              </div>
            )}
          </FormCard>
        </WizardStep>

        {/* PASO 4: Detalles de Evidencia */}
        <WizardStep
          stepNumber={4}
          title={WIZARD_STEPS[3].title}
          description={WIZARD_STEPS[3].description}
          isActive={currentStep === 3}
        >
          <FormCard
            title="Información de la Evidencia"
            description="Completa los detalles de tu registro"
          >
            <div className="space-y-4">
              {/* Tipo de evidencia */}
              <div>
                <label className="block text-sm font-medium text-neutral-900 mb-2">
                  Tipo de Evidencia
                </label>
                <Select
                  value={wizardData.evidenceType}
                  onValueChange={(value) =>
                    updateWizardData({
                      evidenceType: value as EvidenceType,
                    })
                  }
                >
                  <option value="START_YEAR">Año de Inicio (START_YEAR)</option>
                  <option value="CUTOFF">Corte de Temporada (CUTOFF)</option>
                </Select>
              </div>

              {/* Fecha */}
              <div>
                <label className="block text-sm font-medium text-neutral-900 mb-2">
                  Fecha de Registro
                </label>
                <Input
                  type="date"
                  value={wizardData.readingDate}
                  onChange={(e) =>
                    updateWizardData({ readingDate: e.target.value })
                  }
                />
              </div>

              {/* Lectura de odómetro */}
              <div className="grid grid-cols-3 gap-3">
                <div className="col-span-2">
                  <label className="block text-sm font-medium text-neutral-900 mb-2">
                    Lectura del Odómetro
                  </label>
                  <Input
                    type="number"
                    value={wizardData.odometerReading || ''}
                    onChange={(e) =>
                      updateWizardData({
                        odometerReading: parseInt(e.target.value) || 0,
                      })
                    }
                    placeholder="Ej: 15000"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-neutral-900 mb-2">
                    Unidad
                  </label>
                  <Select
                    value={wizardData.unit}
                    onValueChange={(value) =>
                      updateWizardData({ unit: value as 'Kilometers' | 'Miles' })
                    }
                  >
                    <option value="Kilometers">km</option>
                    <option value="Miles">millas</option>
                  </Select>
                </div>
              </div>

              {/* ¿Es Triciclo? */}
              <div>
                <label className="block text-sm font-medium text-neutral-900 mb-2">
                  ¿Es Triciclo?
                </label>
                <Select
                  value={wizardData.trike ? 'yes' : 'no'}
                  onValueChange={(value) =>
                    updateWizardData({ trike: value === 'yes' })
                  }
                >
                  <option value="no">No - Motocicleta Normal</option>
                  <option value="yes">Sí - Triciclo</option>
                </Select>
              </div>

              {/* Notas */}
              <div>
                <label className="block text-sm font-medium text-neutral-900 mb-2">
                  Notas Adicionales
                </label>
                <textarea
                  value={wizardData.notes}
                  onChange={(e) => updateWizardData({ notes: e.target.value })}
                  placeholder="Añade cualquier comentario..."
                  className="flex min-h-24 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                />
              </div>
            </div>
          </FormCard>
        </WizardStep>

        {/* PASO 5: Cargar Fotos */}
        <WizardStep
          stepNumber={5}
          title={WIZARD_STEPS[4].title}
          description={WIZARD_STEPS[4].description}
          isActive={currentStep === 4}
        >
          <FormCard
            title="Cargar Fotos"
            description="Sube fotos claras de la evidencia"
          >
            <div className="space-y-6">
              {/* Foto con piloto y moto */}
              <div>
                <label className="block text-sm font-medium text-neutral-900 mb-3">
                  Foto: Piloto con Moto
                </label>
                <UploadZone
                  label="Arrastra la foto aquí o haz clic"
                  description="Se debe ver el piloto y la moto en la foto"
                  selectedFiles={
                    wizardData.pilotWithBikePhoto
                      ? [wizardData.pilotWithBikePhoto]
                      : []
                  }
                  onFilesSelected={(files) => {
                    if (files.length > 0) {
                      updateWizardData({ pilotWithBikePhoto: files[0] });
                    }
                  }}
                  onRemoveFile={() =>
                    updateWizardData({ pilotWithBikePhoto: null })
                  }
                  maxFiles={1}
                />
              </div>

              {/* Foto del odómetro */}
              <div>
                <label className="block text-sm font-medium text-neutral-900 mb-3">
                  Foto: Odómetro en Primer Plano
                </label>
                <UploadZone
                  label="Arrastra la foto aquí o haz clic"
                  description="Debe verse claramente la lectura del odómetro"
                  selectedFiles={
                    wizardData.odometerCloseupPhoto
                      ? [wizardData.odometerCloseupPhoto]
                      : []
                  }
                  onFilesSelected={(files) => {
                    if (files.length > 0) {
                      updateWizardData({ odometerCloseupPhoto: files[0] });
                    }
                  }}
                  onRemoveFile={() =>
                    updateWizardData({ odometerCloseupPhoto: null })
                  }
                  maxFiles={1}
                />
              </div>
            </div>
          </FormCard>
        </WizardStep>

        {/* PASO 6: Revisión y Envío */}
        <WizardStep
          stepNumber={6}
          title={WIZARD_STEPS[5].title}
          description={WIZARD_STEPS[5].description}
          isActive={currentStep === 5}
        >
          <FormCard
            title="Resumen de tu Solicitud"
            description="Verifica que toda la información sea correcta"
            variant="default"
          >
            <div className="space-y-4 text-sm">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-neutral-600">Evento</p>
                  <p className="font-medium text-neutral-900">
                    {wizardData.eventId ? `ID: ${wizardData.eventId}` : '-'}
                  </p>
                </div>
                <div>
                  <p className="text-neutral-600">Participante</p>
                  <p className="font-medium text-neutral-900">
                    {wizardData.memberId ? `ID: ${wizardData.memberId}` : '-'}
                  </p>
                </div>
                <div>
                  <p className="text-neutral-600">Vehículo</p>
                  <p className="font-medium text-neutral-900">
                    {wizardData.vehicleId ? `ID: ${wizardData.vehicleId}` : '-'}
                  </p>
                </div>
                <div>
                  <p className="text-neutral-600">Tipo de Evidencia</p>
                  <p className="font-medium text-neutral-900">
                    {wizardData.evidenceType}
                  </p>
                </div>
                <div>
                  <p className="text-neutral-600">Odómetro</p>
                  <p className="font-medium text-neutral-900">
                    {wizardData.odometerReading} {wizardData.unit === 'Kilometers' ? 'km' : 'mi'}
                  </p>
                </div>
                <div>
                  <p className="text-neutral-600">¿Triciclo?</p>
                  <p className="font-medium text-neutral-900">
                    {wizardData.trike ? 'Sí' : 'No'}
                  </p>
                </div>
              </div>

              <hr className="border-neutral-200" />

              <div>
                <p className="text-neutral-600">Fotos Cargadas</p>
                <p className="font-medium text-neutral-900">
                  {wizardData.pilotWithBikePhoto &&
                  wizardData.odometerCloseupPhoto
                    ? '2 fotos cargadas ✓'
                    : 'Falta cargar fotos'}
                </p>
              </div>
            </div>
          </FormCard>
        </WizardStep>
      </WizardContainer>

      {/* Modal de éxito */}
      <SuccessModal
        isOpen={showSuccessModal}
        title="¡Evidencia Registrada!"
        message="Tu solicitud ha sido enviada exitosamente y está siendo revisada."
        details={
          <div className="space-y-2 text-sm">
            <p className="font-medium text-neutral-900">Puntos potenciales: +10</p>
            <p className="text-neutral-600">
              Tu evidencia ha sido registrada. El administrador la revisará en breve.
            </p>
          </div>
        }
        secondaryAction={{
          label: 'Ver mis evidencias',
          onClick: () => router.push('/member/evidences'),
        }}
        onClose={() => {
          setShowSuccessModal(false);
        }}
      />

      {/* Modal de error */}
      <ErrorModal
        isOpen={showErrorModal}
        title="Error al Registrar"
        message="No pudimos procesar tu solicitud"
        errorDetails={errorMessage}
        onRetry={handleFinish}
        onCancel={() => setShowErrorModal(false)}
      />
    </>
  );
}
