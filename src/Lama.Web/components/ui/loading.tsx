import { Loader2 } from 'lucide-react';

interface LoadingProps {
  message?: string;
}

export function Loading({ message = 'Cargando...' }: LoadingProps) {
  return (
    <div className="flex flex-col items-center justify-center py-12">
      <Loader2 className="animate-spin text-primary-600 mb-4" size={32} />
      <p className="text-neutral-600">{message}</p>
    </div>
  );
}
