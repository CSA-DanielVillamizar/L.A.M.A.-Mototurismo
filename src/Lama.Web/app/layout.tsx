import type { Metadata, Viewport } from "next";
import { Inter } from "next/font/google";
import { LayoutWrapper } from "@/components/layout";
import { AuthProvider } from "@/lib/auth/AuthProvider";
import "./globals.css";

/**
 * Fuente principal: Inter (Google Fonts)
 * Stripe-like professional typography
 */
const inter = Inter({
  subsets: ["latin"],
  display: "swap",
  variable: "--font-inter",
});

export const metadata: Metadata = {
  title: "LAMA COR - Sistema de Evidencias",
  description: "Sistema de subida de evidencias para LAMA Mototurismo",
  manifest: "/manifest.json",
  appleWebApp: {
    capable: true,
    statusBarStyle: "black-translucent",
    title: "LAMA COR",
  },
};

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  maximumScale: 5,
  userScalable: true,
  themeColor: "#7c3aed",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es" className={inter.variable}>
      <head>
        <link rel="manifest" href="/manifest.json" />
        <meta name="theme-color" content="#4f46e5" />
        <link rel="apple-touch-icon" href="/icons/icon-192x192.png" />
      </head>
      <body className="bg-white text-gray-900 antialiased">
        <AuthProvider>
          <LayoutWrapper>
            {children}
          </LayoutWrapper>
        </AuthProvider>
      </body>
    </html>
  );
}
