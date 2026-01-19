'use client';

import React, { useState, useEffect, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { PageHeader } from '@/components/common';
import { QueueTable } from './QueueTable';
import { QueueFilters } from './QueueFilters';
import { QueueAttendee, QueueFiltersState } from './types';
import { ListChecks, RefreshCw, AlertCircle } from 'lucide-react';
import { apiClient } from '@/lib/api-client';

export function QueuePageShell() {
  const router = useRouter();
  const [data, setData] = useState<QueueAttendee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState<QueueFiltersState>({
    search: '',
    eventId: null,
    status: null,
  });
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [sort, setSort] = useState<string>('');
  const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  
  // UI states (NOT domain states)
  const [isValidating, setIsValidating] = useState(false);
  const [validatingRowId, setValidatingRowId] = useState<string | null>(null);

  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(searchQuery), 300);
    return () => clearTimeout(timer);
  }, [searchQuery]);

  const loadQueue = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Validar status: solo aceptar pending|confirmed|rejected
      const validStatuses = ['pending', 'confirmed', 'rejected'];
      const normalizedStatus = filters.status && validStatuses.includes(filters.status)
        ? (filters.status.toUpperCase() as 'PENDING' | 'CONFIRMED' | 'REJECTED')
        : undefined;
      
      if (filters.status && !normalizedStatus) {
        console.warn(`Invalid status: ${filters.status}. Normalizing to undefined.`);
      }
      
      const result = await apiClient.getAdminQueue({
        eventId: filters.eventId ? parseInt(filters.eventId, 10) : undefined,
        status: normalizedStatus,
        q: debouncedSearch || undefined,
        page,
        pageSize,
        sort: sort || undefined,
      });
      const mappedData: QueueAttendee[] = result.items.map((item: any) => ({
        id: item.attendanceId.toString(),
        memberName: item.memberName,
        memberEmail: item.memberEmail,
        eventName: item.eventName,
        eventDate: item.eventDate,
        vehicleMake: item.vehicleMotorcycleData.split(' ')[0] || 'Unknown',
        vehicleModel: item.vehicleMotorcycleData.split(' ')[1] || '',
        vehiclePlate: item.vehicleLicPlate,
        odometer: 0,
        odometerUnit: 'km' as const,
        pointsAwarded: item.pointsAwardedPerMember,
        status: item.status.toLowerCase() as any,
        submittedAt: item.createdAt,
        memberId: item.memberId.toString(),
        eventId: item.eventId.toString(),
        vehicleId: item.vehicleId.toString(),
      }));
      setData(mappedData);
      setTotal(result.total);
      setTotalPages(result.totalPages);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Error al cargar cola';
      setError(message);
      console.error('Error loading queue:', err);
    } finally {
      setLoading(false);
    }
  }, [filters, debouncedSearch, page, pageSize, sort]);

  useEffect(() => {
    setPage(1);
  }, [filters, debouncedSearch]);

  useEffect(() => {
    loadQueue();
  }, [loadQueue]);

  const handleFiltersChange = (newFilters: QueueFiltersState) => {
    setFilters(newFilters);
  };

  const handleRefresh = () => {
    loadQueue();
  };

  const handleValidate = (attendee: QueueAttendee) => {
    // Set UI state to "processing" while navigating
    setIsValidating(true);
    setValidatingRowId(attendee.id);
    router.push(
      `/admin/cor?eventId=${attendee.eventId}&memberId=${attendee.memberId}&vehicleId=${attendee.vehicleId}`
    );
  };

  const handleViewDetail = (attendee: QueueAttendee) => {
    console.log('View detail:', attendee);
  };

  return (
    <div className="space-y-6">
      <PageHeader
        title="Cola de Validación"
        subtitle="Registros de asistencia pendientes de validación"
        icon={ListChecks}
        actions={
          <button
            onClick={handleRefresh}
            className="flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            <RefreshCw className="h-4 w-4" />
            Actualizar
          </button>
        }
      />

      {error && (
        <div
          className="rounded-lg border border-red-200 bg-red-50 p-4 text-red-700"
          role="alert"
          aria-live="assertive"
        >
          <div className="flex gap-3">
            <AlertCircle className="h-5 w-5 flex-shrink-0" />
            <div>
              <h3 className="font-semibold">Error al cargar cola</h3>
              <p className="text-sm">{error}</p>
            </div>
          </div>
        </div>
      )}

      {loading && (
        <div className="sr-only" role="status" aria-live="polite" aria-busy="true">
          Cargando cola de asistencias...
        </div>
      )}

      <QueueFilters
        filters={filters}
        events={[]}
        onFiltersChange={handleFiltersChange}
        onRefresh={handleRefresh}
      />

      {!error && (
        <QueueTable
          data={data}
          loading={loading || isValidating}
          validatingRowId={validatingRowId}
          onValidate={handleValidate}
          onViewDetail={handleViewDetail}
        />
      )}

      {!error && !loading && data.length > 0 && (
        <div className="flex items-center justify-between rounded-lg border border-gray-200 bg-white p-4">
          <div className="text-sm text-gray-600">
            Página <span className="font-semibold">{page}</span> de{' '}
            <span className="font-semibold">{totalPages}</span> ({total} total)
          </div>
          <div className="flex gap-2">
            <button
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-3 py-2 text-sm font-medium text-gray-700 disabled:text-gray-400"
            >
              Anterior
            </button>
            <button
              onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-3 py-2 text-sm font-medium text-gray-700 disabled:text-gray-400"
            >
              Siguiente
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
