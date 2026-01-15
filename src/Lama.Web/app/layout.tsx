import type { Metadata } from "next";
import { LayoutWrapper } from "@/components/layout";
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
      <body className="bg-neutral-50">
        <LayoutWrapper>
          {children}
        </LayoutWrapper>
      </body>
    </html>
  );
}
