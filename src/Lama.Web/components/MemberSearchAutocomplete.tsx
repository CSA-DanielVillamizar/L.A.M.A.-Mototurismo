'use client';

import { useState, useCallback, useRef, useEffect } from 'react';
import type { MemberSearchResult } from '@/types/api';
import { apiClient } from '@/lib/api-client';

interface MemberSearchAutocompleteProps {
  /**
   * Miembro seleccionado actualmente
   */
  selectedMember: MemberSearchResult | null;

  /**
   * Callback cuando se selecciona un miembro
   */
  onMemberSelect: (member: MemberSearchResult) => void;

  /**
   * Placeholder del input
   */
  placeholder?: string;

  /**
   * Debounce delay en ms
   */
  debounceMs?: number;

  /**
   * ID de atributo para testing
   */
  testId?: string;
}

/**
 * Componente de autocomplete para búsqueda de miembros
 * Busca por nombre, orden o placa
 */
export function MemberSearchAutocomplete({
  selectedMember,
  onMemberSelect,
  placeholder = 'Buscar por nombre, orden o placa...',
  debounceMs = 300,
  testId,
}: MemberSearchAutocompleteProps) {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<MemberSearchResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const debounceTimer = useRef<NodeJS.Timeout | null>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  // Cerrar dropdown cuando se clickea afuera
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(event.target as Node)
      ) {
        setIsOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Búsqueda con debounce
  const search = useCallback(
    async (searchTerm: string) => {
      if (!searchTerm || searchTerm.length < 1) {
        setResults([]);
        setIsOpen(false);
        setError(null);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const searchResults = await apiClient.searchMembers(searchTerm);
        setResults(searchResults);
        setIsOpen(true);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error en búsqueda');
        setResults([]);
        setIsOpen(false);
      } finally {
        setLoading(false);
      }
    },
    []
  );

  const handleInputChange = useCallback(
    (value: string) => {
      setQuery(value);

      // Limpiar timer anterior
      if (debounceTimer.current) {
        clearTimeout(debounceTimer.current);
      }

      // Configura nuevo timer
      debounceTimer.current = setTimeout(() => {
        search(value);
      }, debounceMs);
    },
    [search, debounceMs]
  );

  const handleSelect = useCallback(
    (member: MemberSearchResult) => {
      onMemberSelect(member);
      setQuery(`${member.fullName} (#${member.memberId})`);
      setIsOpen(false);
      setResults([]);
    },
    [onMemberSelect]
  );

  return (
    <div ref={containerRef} className="relative space-y-2" data-testid={testId}>
      <label htmlFor="member-search" className="block text-sm font-medium text-gray-700">
        Buscar Miembro
      </label>

      <div className="relative">
        <input
          id="member-search"
          type="text"
          value={query}
          onChange={(e) => handleInputChange(e.target.value)}
          placeholder={placeholder}
          autoComplete="off"
          className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
        />

        {loading && (
          <div className="absolute right-3 top-3">
            <div className="h-5 w-5 animate-spin rounded-full border-2 border-gray-300 border-t-blue-500" />
          </div>
        )}
      </div>

      {error && (
        <p className="text-sm text-red-600">{error}</p>
      )}

      {/* Dropdown de resultados */}
      {isOpen && results.length > 0 && (
        <ul className="absolute left-0 right-0 top-[4.5rem] z-10 max-h-60 overflow-y-auto rounded-lg border border-gray-300 bg-white shadow-lg">
          {results.map((member) => (
            <li key={member.memberId}>
              <button
                type="button"
                onClick={() => handleSelect(member)}
                className="w-full px-4 py-3 text-left hover:bg-blue-50 focus:bg-blue-50 focus:outline-none"
              >
                <div className="font-medium text-gray-900">
                  {member.fullName} #{member.memberId}
                </div>
                <div className="text-sm text-gray-500">
                  Estado: {member.status} • Capítulo: {member.chapterId}
                </div>
              </button>
            </li>
          ))}
        </ul>
      )}

      {isOpen && results.length === 0 && query && !loading && (
        <div className="absolute left-0 right-0 top-[4.5rem] z-10 rounded-lg border border-gray-300 bg-white p-4 text-center text-gray-500 shadow-lg">
          No se encontraron miembros
        </div>
      )}

      {selectedMember && (
        <p className="text-sm text-green-600 font-medium">
          ✓ Seleccionado: {selectedMember.fullName}
        </p>
      )}
    </div>
  );
}
