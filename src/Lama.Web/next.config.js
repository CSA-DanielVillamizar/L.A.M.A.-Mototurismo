/**
 * Configuración de Next.js para L.A.M.A. Mototurismo
 * Optimizaciones para producción: compresión, caching, imágenes, etc.
 */

/** @type {import('next').NextConfig} */
const nextConfig = {
  // Modo estricto de React
  reactStrictMode: true,

  // Compresión y optimizaciones
  compress: true,
  poweredByHeader: false,

  // Optimización de imágenes
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'images.unsplash.com',
      },
      {
        protocol: 'https',
        hostname: '*.githubusercontent.com',
      },
    ],
    // Cache de imágenes: 60 días en navegador, 365 días en CDN
    minimumCacheTTL: 60 * 60 * 24 * 365,
    // Generar múltiples tamaños para responsive
    deviceSizes: [640, 750, 828, 1080, 1200, 1920, 2048, 3840],
    imageSizes: [16, 32, 48, 64, 96, 128, 256, 384],
    formats: ['image/avif', 'image/webp'],
  },

  // Headers para optimización
  async headers() {
    return [
      {
        source: '/fonts/:path*',
        headers: [
          {
            key: 'Cache-Control',
            value: 'public, max-age=31536000, immutable',
          },
        ],
      },
      {
        source: '/_next/static/:path*',
        headers: [
          {
            key: 'Cache-Control',
            value: 'public, max-age=31536000, immutable',
          },
        ],
      },
      {
        source: '/images/:path*',
        headers: [
          {
            key: 'Cache-Control',
            value: 'public, max-age=31536000, immutable',
          },
        ],
      },
    ];
  },

  // Redirecciones
  async redirects() {
    return [
      {
        source: '/mi-portal',
        destination: '/member/dashboard',
        permanent: true,
      },
      {
        source: '/admin',
        destination: '/admin/cor',
        permanent: true,
      },
    ];
  },

  // Variables de entorno
  env: {
    NEXT_PUBLIC_API_BASE_URL: process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5000',
    NEXT_PUBLIC_APP_NAME: 'L.A.M.A. Mototurismo',
    NEXT_PUBLIC_APP_VERSION: '2.0.0',
  },
};

module.exports = nextConfig;
