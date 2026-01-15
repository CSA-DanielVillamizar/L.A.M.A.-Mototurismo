'use client';

import { usePWAInstall } from '@/hooks/usePWA';
import { Button } from '@/components/ui/button';
import { IconUpload } from '@/components/icons';

/**
 * Banner para promover instalación de PWA
 */
export function PWAInstallBanner() {
  const { isInstallable, isInstalled, promptInstall } = usePWAInstall();

  if (!isInstallable || isInstalled) {
    return null;
  }

  const handleInstall = async () => {
    const accepted = await promptInstall();
    if (accepted) {
      console.log('PWA installed successfully');
    }
  };

  return (
    <div className="fixed bottom-4 left-4 right-4 md:left-auto md:right-4 md:w-96 bg-purple-900 border border-purple-700 rounded-lg shadow-xl p-4 z-50 animate-slide-up">
      <div className="flex items-start gap-3">
        <div className="flex-shrink-0">
          <div className="w-10 h-10 bg-purple-700 rounded-lg flex items-center justify-center">
            <IconUpload size={20} className="text-white" />
          </div>
        </div>
        
        <div className="flex-1">
          <h3 className="text-white font-semibold text-sm mb-1">
            Instalar L.A.M.A. COR
          </h3>
          <p className="text-purple-200 text-xs mb-3">
            Accede más rápido con nuestra app instalada
          </p>
          
          <div className="flex gap-2">
            <Button
              onClick={handleInstall}
              className="bg-white text-purple-900 hover:bg-purple-100 text-xs h-8"
            >
              Instalar ahora
            </Button>
            <Button
              onClick={() => {
                const banner = document.querySelector('[data-pwa-banner]');
                if (banner) {
                  (banner as HTMLElement).style.display = 'none';
                }
              }}
              variant="ghost"
              className="text-purple-200 hover:text-white text-xs h-8"
            >
              Más tarde
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
