/**
 * Hook de React para búsqueda de miembros con debounce + cancellation
 * Optimizado para 4,000+ miembros con Redis cache y AbortController
 */

import { useState, useEffect, useRef } from 'react';

interface MemberSearchResult {
  memberId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  status: string;
  chapterId: number;
}

interface UseMemberSearchOptions {
  debounceMs?: number; // Delay antes de ejecutar búsqueda (default 300ms)
  minChars?: number;   // Mínimo de caracteres para buscar (default 2)
}

export function useMemberSearch(options: UseMemberSearchOptions = {}) {
  const { debounceMs = 300, minChars = 2 } = options;
  
  const [searchTerm, setSearchTerm] = useState('');
  const [results, setResults] = useState<MemberSearchResult[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  // Ref para almacenar el AbortController activo
  const abortControllerRef = useRef<AbortController | null>(null);
  
  // Ref para el timeout del debounce
  const debounceTimeoutRef = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    // Limpiar timeout anterior
    if (debounceTimeoutRef.current) {
      clearTimeout(debounceTimeoutRef.current);
    }

    // Cancelar request anterior si existe
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }

    // Si el término es muy corto, limpiar resultados
    if (searchTerm.length < minChars) {
      setResults([]);
      setError(null);
      setIsLoading(false);
      return;
    }

    // Iniciar loading
    setIsLoading(true);
    setError(null);

    // Debounce: esperar antes de ejecutar búsqueda
    debounceTimeoutRef.current = setTimeout(async () => {
      // Crear nuevo AbortController para este request
      const abortController = new AbortController();
      abortControllerRef.current = abortController;

      try {
        const response = await fetch(
          `/api/members/search?q=${encodeURIComponent(searchTerm)}`,
          {
            signal: abortController.signal,
            headers: {
              'Content-Type': 'application/json',
            },
          }
        );

        // Si el request fue cancelado, no procesar respuesta
        if (abortController.signal.aborted) {
          return;
        }

        if (!response.ok) {
          if (response.status === 429) {
            throw new Error('Rate limit exceeded. Please wait a moment.');
          }
          throw new Error(`Search failed: ${response.statusText}`);
        }

        const data: MemberSearchResult[] = await response.json();
        setResults(data);
        setError(null);
      } catch (err: any) {
        // Ignorar errores de cancelación (AbortError)
        if (err.name === 'AbortError') {
          console.log('Search request cancelled');
          return;
        }

        console.error('Member search error:', err);
        setError(err.message || 'Search failed');
        setResults([]);
      } finally {
        // Solo actualizar loading si no fue cancelado
        if (!abortController.signal.aborted) {
          setIsLoading(false);
        }
      }
    }, debounceMs);

    // Cleanup: cancelar request y timeout al desmontar o cambiar searchTerm
    return () => {
      if (debounceTimeoutRef.current) {
        clearTimeout(debounceTimeoutRef.current);
      }
      if (abortControllerRef.current) {
        abortControllerRef.current.abort();
      }
    };
  }, [searchTerm, debounceMs, minChars]);

  return {
    searchTerm,
    setSearchTerm,
    results,
    isLoading,
    error,
  };
}

/**
 * Ejemplo de uso en componente:
 * 
 * function MemberSearchInput() {
 *   const { searchTerm, setSearchTerm, results, isLoading, error } = useMemberSearch({
 *     debounceMs: 300,
 *     minChars: 2
 *   });
 * 
 *   return (
 *     <div>
 *       <input
 *         type="text"
 *         value={searchTerm}
 *         onChange={(e) => setSearchTerm(e.target.value)}
 *         placeholder="Buscar miembro..."
 *       />
 *       {isLoading && <span>Buscando...</span>}
 *       {error && <span className="error">{error}</span>}
 *       <ul>
 *         {results.map(member => (
 *           <li key={member.memberId}>
 *             {member.fullName} ({member.status})
 *           </li>
 *         ))}
 *       </ul>
 *     </div>
 *   );
 * }
 */
