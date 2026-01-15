'use client';

import React, { useCallback, useState } from 'react';
import { cn } from '@/lib/utils';
import {
  IconUpload,
  IconFile,
  IconX,
  IconImage,
} from '@/components/icons';

/**
 * Propiedades de la zona de carga
 */
interface UploadZoneProps {
  /** Callback cuando archivos son seleccionados */
  onFilesSelected: (files: File[]) => void;
  /** Tipos MIME aceptados */
  acceptedTypes?: string[];
  /** Máximo de archivos permitidos */
  maxFiles?: number;
  /** Máximo tamaño por archivo en bytes */
  maxFileSize?: number;
  /** Texto personalizado para la zona vacía */
  label?: string;
  /** Descripciones adicionales */
  description?: string;
  /** Archivos actualmente seleccionados (para mostrar previsualizaciones) */
  selectedFiles?: File[];
  /** Callback cuando un archivo es removido */
  onRemoveFile?: (index: number) => void;
  /** Si true, es un área compacta sin previsualizaciones */
  compact?: boolean;
  /** Clases CSS adicionales */
  className?: string;
}

/**
 * Zona de carga de archivos con drag-drop
 * Soporta previsualizaciones de imágenes y validación
 */
export function UploadZone({
  onFilesSelected,
  acceptedTypes = ['image/jpeg', 'image/png', 'image/webp'],
  maxFiles = 2,
  maxFileSize = 5 * 1024 * 1024, // 5MB
  label = 'Arrastra fotos aquí o haz clic para seleccionar',
  description = 'PNG, JPG o WEBP. Máximo 5MB por archivo.',
  selectedFiles = [],
  onRemoveFile,
  compact = false,
  className,
}: UploadZoneProps) {
  const [isDragging, setIsDragging] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const validateFiles = useCallback(
    (files: File[]) => {
      const errors: string[] = [];

      // Validar cantidad de archivos
      if (selectedFiles.length + files.length > maxFiles) {
        errors.push(`Máximo ${maxFiles} archivos permitidos`);
      }

      // Validar cada archivo
      for (const file of files) {
        if (!acceptedTypes.includes(file.type)) {
          errors.push(`${file.name}: tipo no permitido`);
        }
        if (file.size > maxFileSize) {
          errors.push(`${file.name}: archivo muy grande`);
        }
      }

      if (errors.length > 0) {
        setError(errors.join(', '));
        return [];
      }

      setError(null);
      return files;
    },
    [selectedFiles.length, maxFiles, acceptedTypes, maxFileSize]
  );

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const handleDragLeave = () => {
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);

    const files = Array.from(e.dataTransfer.files);
    const validFiles = validateFiles(files);
    if (validFiles.length > 0) {
      onFilesSelected(validFiles);
    }
  };

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.currentTarget.files || []);
    const validFiles = validateFiles(files);
    if (validFiles.length > 0) {
      onFilesSelected(validFiles);
    }
  };

  if (compact) {
    return (
      <div className={className}>
        <label
          className={cn(
            'relative flex items-center justify-center gap-2 px-4 py-3 border-2 border-dashed rounded-lg cursor-pointer transition-colors',
            isDragging
              ? 'border-primary-500 bg-primary-50'
              : 'border-neutral-300 hover:border-primary-400'
          )}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
        >
          <IconUpload className="w-4 h-4 text-neutral-600" />
          <span className="text-sm text-neutral-700">
            {selectedFiles.length}/{maxFiles} fotos
          </span>
          <input
            type="file"
            multiple
            accept={acceptedTypes.join(',')}
            onChange={handleFileInputChange}
            disabled={selectedFiles.length >= maxFiles}
            className="hidden"
          />
        </label>
        {error && <p className="mt-2 text-xs text-danger-600">{error}</p>}
      </div>
    );
  }

  return (
    <div className={className}>
      <label
        className={cn(
          'relative flex flex-col items-center justify-center gap-3 px-6 py-12 border-2 border-dashed rounded-lg cursor-pointer transition-all',
          isDragging
            ? 'border-primary-500 bg-primary-50'
            : 'border-neutral-300 hover:border-primary-400 hover:bg-neutral-50'
        )}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      >
        <IconImage className="w-8 h-8 text-neutral-400" />
        <div className="text-center">
          <p className="font-medium text-neutral-900">{label}</p>
          <p className="text-sm text-neutral-600">{description}</p>
        </div>
        <input
          type="file"
          multiple
          accept={acceptedTypes.join(',')}
          onChange={handleFileInputChange}
          disabled={selectedFiles.length >= maxFiles}
          className="hidden"
        />
      </label>

      {error && (
        <p className="mt-3 text-sm text-danger-600 font-medium">{error}</p>
      )}

      {/* Previsualizaciones de archivos */}
      {selectedFiles.length > 0 && (
        <div className="mt-4 grid grid-cols-2 gap-3 md:grid-cols-3">
          {selectedFiles.map((file, index) => (
            <div
              key={index}
              className="relative group overflow-hidden rounded-lg bg-neutral-100"
            >
              {file.type.startsWith('image/') ? (
                // eslint-disable-next-line @next/next/no-img-element
                <img
                  src={URL.createObjectURL(file)}
                  alt={file.name}
                  className="w-full h-32 object-cover"
                />
              ) : (
                <div className="w-full h-32 flex items-center justify-center">
                  <IconFile className="w-8 h-8 text-neutral-400" />
                </div>
              )}

              {onRemoveFile && (
                <button
                  type="button"
                  onClick={() => onRemoveFile(index)}
                  className={cn(
                    'absolute inset-0 flex items-center justify-center bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity',
                    'text-white'
                  )}
                >
                  <IconX className="w-6 h-6" />
                </button>
              )}

              <div className="mt-1 text-xs text-neutral-600 truncate">
                {file.name}
              </div>
            </div>
          ))}
        </div>
      )}

      {selectedFiles.length > 0 && selectedFiles.length < maxFiles && (
        <p className="mt-3 text-xs text-neutral-600">
          {maxFiles - selectedFiles.length} más {maxFiles - selectedFiles.length === 1 ? 'archivo' : 'archivos'} permitido(s)
        </p>
      )}
    </div>
  );
}
