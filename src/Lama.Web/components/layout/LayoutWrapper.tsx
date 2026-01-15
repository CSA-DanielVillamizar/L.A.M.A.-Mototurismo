'use client';

import { ReactNode } from 'react';
import { Sidebar, Topbar, Breadcrumbs } from './AppShell';

interface LayoutWrapperProps {
  children: ReactNode;
  breadcrumbs?: { label: string; href?: string }[];
  title?: string;
}

/**
 * Wrapper de layout para la aplicación principal
 * Incluye sidebar, topbar y estructura responsive
 */
export function LayoutWrapper({ children, breadcrumbs, title }: LayoutWrapperProps) {
  return (
    <div className="flex h-screen bg-neutral-50">
      {/* Sidebar */}
      <Sidebar />

      {/* Main Content */}
      <div className="flex flex-1 flex-col overflow-hidden">
        {/* Topbar */}
        <Topbar />

        {/* Breadcrumbs */}
        {breadcrumbs && <Breadcrumbs items={breadcrumbs} />}

        {/* Page Title */}
        {title && (
          <div className="border-b border-neutral-200 bg-white px-6 py-4 ml-0 md:ml-64">
            <h1 className="text-2xl font-bold text-primary-900">{title}</h1>
          </div>
        )}

        {/* Content Area */}
        <main className="flex-1 overflow-auto">
          <div className="p-6 md:p-8 ml-0 md:ml-64">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}
