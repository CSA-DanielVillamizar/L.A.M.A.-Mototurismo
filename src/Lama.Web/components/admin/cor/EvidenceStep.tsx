'use client';

/**
 * EvidenceStep Component
 * Paso 4: Subir evidencia (fotos, odómetro, notas)
 */

import React, { useState, useRef } from 'react';
import { EvidenceForm } from './types';
import { Upload, X, AlertCircle } from 'lucide-react';

interface EvidenceStepProps {
  evidence: EvidenceForm | null;
  onChangeEvidence: (evidence: EvidenceForm) => void;
  loading: boolean;
}

const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/webp'];

export function EvidenceStep({
  evidence,
  onChangeEvidence,
  loading,
}: EvidenceStepProps) {
  const [dragActive, setDragActive] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const currentEvidence = evidence || {
    odometer: { value: 0, unit: 'km' },
    photos: [],
    notes: '',
  };

  // Validar archivo
  const validateFile = (file: File): string | null => {
    if (file.size > MAX_FILE_SIZE) {
      return `El archivo ${file.name} excede 5MB`;
    }
    if (!ALLOWED_TYPES.includes(file.type)) {
      return `El archivo ${file.name} debe ser JPG, PNG o WebP`;
    }
    return null;
  };

  // Manejar archivos droppeados o seleccionados
  const handleFiles = (files: FileList | null) => {
    if (!files) return;

    setUploadError(null);
    const newFiles: File[] = [];
    const errors: string[] = [];

    Array.from(files).forEach((file) => {
      const error = validateFile(file);
      if (error) {
        errors.push(error);
      } else {
        newFiles.push(file);
      }
    });

    if (errors.length > 0) {
      setUploadError(errors.join('; '));
      return;
    }

    onChangeEvidence({
      ...currentEvidence,
      photos: [...currentEvidence.photos, ...newFiles],
    });
  };

  // Drag and drop handlers
  const handleDrag = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === 'dragenter' || e.type === 'dragover') {
      setDragActive(true);
    } else if (e.type === 'dragleave') {
      setDragActive(false);
    }
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    handleFiles(e.dataTransfer.files);
  };

  // Remover foto
  const removePhoto = (index: number) => {
    onChangeEvidence({
      ...currentEvidence,
      photos: currentEvidence.photos.filter((_, i) => i !== index),
    });
  };

  return (
    <div className="space-y-6">
      {/* Odómetro */}
      <div className="grid gap-4 sm:grid-cols-2">
        <div>
          <label htmlFor="odometer" className="block text-sm font-medium text-gray-900 mb-2">
            Lectura de odómetro
          </label>
          <input
            id="odometer"
            type="number"
            min="0"
            value={currentEvidence.odometer.value}
            onChange={(e) =>
              onChangeEvidence({
                ...currentEvidence,
                odometer: {
                  ...currentEvidence.odometer,
                  value: parseFloat(e.target.value) || 0,
                },
              })
            }
            className="w-full rounded-lg border border-gray-300 bg-white py-2 px-4 text-gray-900 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            placeholder="0"
          />
          <p className="mt-1 text-xs text-gray-600">Valor en números</p>
        </div>

        <div>
          <label htmlFor="unit" className="block text-sm font-medium text-gray-900 mb-2">
            Unidad
          </label>
          <select
            id="unit"
            value={currentEvidence.odometer.unit}
            onChange={(e) =>
              onChangeEvidence({
                ...currentEvidence,
                odometer: {
                  ...currentEvidence.odometer,
                  unit: e.target.value as 'km' | 'mi',
                },
              })
            }
            className="w-full rounded-lg border border-gray-300 bg-white py-2 px-4 text-gray-900 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
          >
            <option value="km">Kilómetros (km)</option>
            <option value="mi">Millas (mi)</option>
          </select>
        </div>
      </div>

      {/* Notas */}
      <div>
        <label htmlFor="notes" className="block text-sm font-medium text-gray-900 mb-2">
          Notas adicionales
        </label>
        <textarea
          id="notes"
          value={currentEvidence.notes || ''}
          onChange={(e) =>
            onChangeEvidence({
              ...currentEvidence,
              notes: e.target.value,
            })
          }
          placeholder="Agregar notas sobre el evento, condiciones, etc."
          className="w-full rounded-lg border border-gray-300 bg-white py-2 px-4 text-gray-900 placeholder:text-gray-500 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
          rows={3}
        />
        <p className="mt-1 text-xs text-gray-600">Máximo 500 caracteres</p>
      </div>

      {/* Dropzone */}
      <div>
        <label className="block text-sm font-medium text-gray-900 mb-2">
          Fotos de evidencia
        </label>

        <div
          onDragEnter={handleDrag}
          onDragLeave={handleDrag}
          onDragOver={handleDrag}
          onDrop={handleDrop}
          className={`rounded-lg border-2 border-dashed p-8 text-center transition-colors ${
            dragActive
              ? 'border-indigo-600 bg-indigo-50'
              : 'border-gray-300 bg-white hover:border-indigo-300'
          }`}
        >
          <input
            ref={fileInputRef}
            type="file"
            multiple
            accept="image/jpeg,image/png,image/webp"
            onChange={(e) => handleFiles(e.target.files)}
            className="hidden"
            aria-label="Seleccionar archivos de imagen"
          />

          <Upload className="mx-auto h-8 w-8 text-gray-400" />
          <p className="mt-2 font-medium text-gray-900">
            Arrastra fotos aquí o{' '}
            <button
              onClick={() => fileInputRef.current?.click()}
              className="text-indigo-600 hover:underline"
            >
              haz clic para seleccionar
            </button>
          </p>
          <p className="mt-1 text-xs text-gray-600">
            JPG, PNG, WebP. Máximo 5MB por archivo.
          </p>
        </div>

        {uploadError && (
          <div className="mt-2 flex items-start gap-2 rounded-lg border border-red-200 bg-red-50 p-3">
            <AlertCircle className="h-4 w-4 flex-shrink-0 text-red-600 mt-0.5" />
            <p className="text-sm text-red-700">{uploadError}</p>
          </div>
        )}
      </div>

      {/* Foto Previews */}
      {currentEvidence.photos.length > 0 && (
        <div>
          <p className="block text-sm font-medium text-gray-900 mb-3">
            {currentEvidence.photos.length} archivo(s) seleccionado(s)
          </p>
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
            {currentEvidence.photos.map((file, idx) => (
              <div key={idx} className="group relative rounded-lg overflow-hidden bg-gray-100">
                {/* Mostrar preview si es posible */}
                <img
                  src={URL.createObjectURL(file)}
                  alt={`Preview ${idx + 1}`}
                  className="h-32 w-full object-cover"
                  onLoad={(e) => {
                    // Limpiar object URL después de cargar
                    URL.revokeObjectURL((e.target as HTMLImageElement).src);
                  }}
                />

                {/* Remove Button */}
                <button
                  onClick={() => removePhoto(idx)}
                  className="absolute top-1 right-1 rounded-full bg-red-600 p-1 text-white opacity-0 transition-opacity group-hover:opacity-100"
                  aria-label={`Eliminar ${file.name}`}
                >
                  <X className="h-4 w-4" />
                </button>

                {/* Filename */}
                <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-2 text-xs text-white truncate">
                  {file.name}
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
