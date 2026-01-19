'use client';

/**
 * Página de Gestión de Miembros
 * Lista y administra los miembros del sistema
 */

import { Suspense, useEffect, useState } from 'react';
import { Breadcrumbs } from '@/components/layout/AppShell';
import { Loading } from '@/components/ui/loading';
import { Users } from 'lucide-react';

interface Member {
  id: string;
  name: string;
  email: string;
  status: string;
}

function MembersContent() {
  const [members, setMembers] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchMembers = async () => {
      try {
        setLoading(true);
        const response = await fetch('http://localhost:5000/api/v1/members/search?q=');
        if (!response.ok) throw new Error('Error al cargar miembros');
        const data = await response.json();
        setMembers(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error desconocido');
      } finally {
        setLoading(false);
      }
    };

    fetchMembers();
  }, []);

  return (
    <div className="space-y-6">
      <Breadcrumbs items={[{ label: 'Admin' }, { label: 'Miembros' }]} />

      <div className="px-6">
        <div className="flex items-center gap-3 mb-6">
          <Users className="text-primary-600" size={28} />
          <h1 className="text-3xl font-bold text-primary-900">Miembros</h1>
        </div>

        {loading && <Loading message="Cargando miembros..." />}

        {error && (
          <div className="rounded-lg bg-danger-50 border border-danger-200 p-4 text-danger-700">
            {error}
          </div>
        )}

        {!loading && members.length === 0 && (
          <div className="rounded-lg bg-neutral-50 border border-neutral-200 p-8 text-center text-neutral-600">
            <Users size={48} className="mx-auto mb-4 text-neutral-400" />
            <p>No hay miembros registrados</p>
          </div>
        )}

        {!loading && members.length > 0 && (
          <div className="grid gap-4">
            {members.map((member) => (
              <div
                key={member.id}
                className="rounded-lg border border-neutral-200 p-4 hover:shadow-md transition-shadow"
              >
                <h3 className="font-semibold text-primary-900">{member.name}</h3>
                <p className="text-sm text-neutral-600">{member.email}</p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default function MembersPage() {
  return (
    <Suspense fallback={<Loading message="Cargando página de miembros..." />}>
      <MembersContent />
    </Suspense>
  );
}
