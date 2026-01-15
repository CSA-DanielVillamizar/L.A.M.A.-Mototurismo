/**
 * Formatea un número como puntos (ej: 1500 -> "1,500")
 */
export function formatPoints(points: number): string {
  return new Intl.NumberFormat('es-CO').format(points);
}

/**
 * Formatea una fecha en formato ISO a formato local
 */
export function formatDate(dateString: string): string {
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('es-CO', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  }).format(date);
}

/**
 * Formatea el tamaño de archivo (bytes a KB/MB)
 */
export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

/**
 * Valida que un archivo sea una imagen
 */
export function isImageFile(file: File): boolean {
  return file.type.startsWith('image/');
}

/**
 * Valida el tamaño máximo de un archivo (en MB)
 */
export function validateFileSize(file: File, maxSizeMB: number): boolean {
  const maxBytes = maxSizeMB * 1024 * 1024;
  return file.size <= maxBytes;
}
