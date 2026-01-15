import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "LAMA COR - Sistema de Evidencias",
  description: "Sistema de subida de evidencias para LAMA Mototurismo",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es">
      <body className="bg-gray-50">
        <nav className="bg-primary-700 text-white shadow-lg">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex items-center justify-between h-16">
              <div className="flex items-center">
                <h1 className="text-xl font-bold">LAMA COR</h1>
              </div>
              <div className="flex space-x-4">
                <a
                  href="/evidence/upload"
                  className="px-3 py-2 rounded-md text-sm font-medium hover:bg-primary-600 transition-colors"
                >
                  Subir Evidencia
                </a>
              </div>
            </div>
          </div>
        </nav>

        <main className="min-h-screen py-8">{children}</main>

        <footer className="bg-gray-800 text-white py-6 mt-12">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
            <p className="text-sm">
              © {new Date().getFullYear()} LAMA Mototurismo. Todos los derechos reservados.
            </p>
          </div>
        </footer>
      </body>
    </html>
  );
}
