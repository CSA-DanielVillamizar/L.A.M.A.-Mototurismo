import { type NextRequest, NextResponse } from 'next/server';

/**
 * Middleware para proteger rutas
 * Redirige usuarios no autenticados a /login
 */
export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Rutas públicas (sin protección)
  const publicRoutes = [
    '/login',
    '/signup',
    '/forgot-password',
    '/verify-email',
    '/reset-password',
    '/',
    '/sponsors', // Sponsors es público
  ];

  // Rutas protegidas (requieren autenticación)
  const protectedRoutes = [
    '/member',
    '/admin',
  ];

  // Verificar si ruta es pública
  const isPublicRoute = publicRoutes.some((route) =>
    pathname === route || pathname.startsWith(route + '/')
  );

  if (isPublicRoute) {
    return NextResponse.next();
  }

  // Verificar si ruta es protegida
  const isProtectedRoute = protectedRoutes.some((route) =>
    pathname.startsWith(route)
  );

  if (isProtectedRoute) {
    // Obtener token del header de cookies
    const token = request.cookies.get('auth_token')?.value;

    if (!token) {
      // Redirigir a login si no hay token
      return NextResponse.redirect(new URL('/login', request.url));
    }
  }

  return NextResponse.next();
}

/**
 * Configuración del middleware
 */
export const config = {
  matcher: [
    // Proteger todas las rutas excepto assets estáticos y API
    '/((?!_next/static|_next/image|favicon.ico|public).*)',
  ],
};
