'use client';

/**
 * MemberStep Component
 * Paso 2: Seleccionar miembro con autocomplete accessible
 * Soporta navegación con teclado (Arrow Up/Down, Enter, Escape)
 */

import React, { useState, useRef, useEffect } from 'react';
import { Member } from './types';
import { Loader2, Users, X } from 'lucide-react';

interface MemberStepProps {
  eventId?: string;
  selectedMember: Member | null;
  onSelectMember: (member: Member) => void;
  loading: boolean;
}

export function MemberStep({
  eventId,
  selectedMember,
  onSelectMember,
  loading,
}: MemberStepProps) {
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<Member[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [highlightedIndex, setHighlightedIndex] = useState(-1);
  const inputRef = useRef<HTMLInputElement>(null);
  const listRef = useRef<HTMLDivElement>(null);
  const searchTimeoutRef = useRef<NodeJS.Timeout>();

  // Búsqueda debounced
  useEffect(() => {
    if (searchTimeoutRef.current) {
      clearTimeout(searchTimeoutRef.current);
    }

    if (!searchQuery.trim()) {
      setSearchResults([]);
      setIsOpen(false);
      return;
    }

    setIsSearching(true);
    searchTimeoutRef.current = setTimeout(async () => {
      try {
        // TODO: Reemplazar con api-client.get(`/members/search?q=${searchQuery}&eventId=${eventId}`)
        // Simulación
        const mockResults: Member[] = [
          {
            id: '1',
            firstName: 'Juan',
            lastName: 'Pérez',
            email: 'juan@example.com',
            chapter: 'Quito',
          },
          {
            id: '2',
            firstName: 'Maria',
            lastName: 'García',
            email: 'maria@example.com',
            chapter: 'Cuenca',
          },
          {
            id: '3',
            firstName: 'Carlos',
            lastName: 'López',
            email: 'carlos@example.com',
            chapter: 'Guayaquil',
          },
        ].filter(
          (m) =>
            `${m.firstName} ${m.lastName}`
              .toLowerCase()
              .includes(searchQuery.toLowerCase()) ||
            m.email.toLowerCase().includes(searchQuery.toLowerCase())
        );

        setSearchResults(mockResults);
        setIsOpen(true);
        setHighlightedIndex(-1);
      } catch (error) {
        setSearchResults([]);
      } finally {
        setIsSearching(false);
      }
    }, 300); // Debounce 300ms
  }, [searchQuery, eventId]);

  // Manejo de teclado
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (!isOpen) return;

    switch (e.key) {
      case 'ArrowDown':
        e.preventDefault();
        setHighlightedIndex((prev) =>
          prev < searchResults.length - 1 ? prev + 1 : prev
        );
        break;

      case 'ArrowUp':
        e.preventDefault();
        setHighlightedIndex((prev) => (prev > 0 ? prev - 1 : -1));
        break;

      case 'Enter':
        e.preventDefault();
        if (highlightedIndex >= 0) {
          onSelectMember(searchResults[highlightedIndex]);
          setSearchQuery('');
          setIsOpen(false);
        }
        break;

      case 'Escape':
        e.preventDefault();
        setIsOpen(false);
        break;

      default:
        break;
    }
  };

  const handleSelectMember = (member: Member) => {
    onSelectMember(member);
    setSearchQuery('');
    setIsOpen(false);
    setHighlightedIndex(-1);
  };

  const handleClear = () => {
    setSearchQuery('');
    setSearchResults([]);
    setIsOpen(false);
    setHighlightedIndex(-1);
    inputRef.current?.focus();
  };

  return (
    <div className="space-y-4">
      {/* Input + Autocomplete */}
      <div className="relative">
        <label className="block text-sm font-medium text-gray-900 mb-2">
          Buscar miembro
        </label>

        <div className="relative">
          <input
            ref={inputRef}
            type="text"
            placeholder="Nombre, correo o capítulo..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            onFocus={() => searchQuery && setIsOpen(true)}
            onKeyDown={handleKeyDown}
            className="w-full rounded-lg border border-gray-300 bg-white py-2 pl-4 pr-10 text-gray-900 placeholder:text-gray-500 focus:border-indigo-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
            aria-label="Buscar miembro"
            aria-expanded={isOpen}
            aria-autocomplete="list"
            aria-activedescendant={
              highlightedIndex >= 0
                ? `member-option-${highlightedIndex}`
                : undefined
            }
          />

          {isSearching && (
            <div className="absolute right-3 top-1/2 -translate-y-1/2">
              <Loader2 className="h-4 w-4 animate-spin text-indigo-600" />
            </div>
          )}

          {searchQuery && !isSearching && (
            <button
              onClick={handleClear}
              className="absolute right-3 top-1/2 -translate-y-1/2 rounded hover:bg-gray-100 p-1"
              aria-label="Limpiar búsqueda"
            >
              <X className="h-4 w-4 text-gray-400" />
            </button>
          )}
        </div>

        {/* Dropdown de resultados */}
        {isOpen && searchResults.length > 0 && (
          <div
            ref={listRef}
            className="absolute top-full left-0 right-0 z-10 mt-2 rounded-lg border border-gray-200 bg-white shadow-lg"
            role="listbox"
          >
            {searchResults.map((member, idx) => (
              <button
                key={member.id}
                id={`member-option-${idx}`}
                onClick={() => handleSelectMember(member)}
                className={`w-full px-4 py-3 text-left text-sm transition-colors ${
                  idx === highlightedIndex
                    ? 'bg-indigo-50 text-indigo-600'
                    : 'text-gray-900 hover:bg-gray-50'
                }`}
                role="option"
                aria-selected={idx === highlightedIndex}
              >
                <div className="font-medium">
                  {member.firstName} {member.lastName}
                </div>
                <div className="text-xs text-gray-600">
                  {member.email} • {member.chapter}
                </div>
              </button>
            ))}
          </div>
        )}

        {isOpen && searchQuery && searchResults.length === 0 && !isSearching && (
          <div className="absolute top-full left-0 right-0 z-10 mt-2 rounded-lg border border-gray-200 bg-white p-4 text-center text-sm text-gray-600 shadow-lg">
            No se encontraron resultados
          </div>
        )}
      </div>

      {/* Selected member card */}
      {selectedMember && (
        <div className="rounded-lg border border-indigo-200 bg-indigo-50 p-4">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <h3 className="font-semibold text-gray-900">
                {selectedMember.firstName} {selectedMember.lastName}
              </h3>
              <div className="mt-2 flex flex-col gap-1 text-sm text-gray-600">
                <p>
                  <span className="font-medium">Email:</span> {selectedMember.email}
                </p>
                <p>
                  <span className="font-medium">Capítulo:</span> {selectedMember.chapter}
                </p>
              </div>
            </div>
            <div className="ml-4">
              <Users className="h-8 w-8 text-indigo-600" />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
