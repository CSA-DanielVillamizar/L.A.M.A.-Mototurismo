# Authentication Implementation - ETAPA 8 Phase 1

## 📋 Overview

Implementación completa del sistema de autenticación (OAuth/JWT) para L.A.M.A. Mototurismo.

**Files Created:**
- `lib/auth.ts` (350+ lines) - Capa de servicio de autenticación
- `lib/auth-context.tsx` (250+ lines) - React Context para estado global
- `app/login/page.tsx` (280+ lines) - Página de login con OAuth
- `app/signup/page.tsx` (320+ lines) - Página de registro
- `middleware.ts` (45+ lines) - Protección de rutas
- `components/layout/UserMenu.tsx` (180+ lines) - Menu de usuario autenticado

**Total Lines of Code:** 1,425+ lines

---

## 🏗️ Architecture

### 1. **Authentication Service Layer** (`lib/auth.ts`)

Proporciona todas las funciones de autenticación:

```typescript
// Token Management
function storeToken(token: string, expiresIn: number): void
export function getToken(): string | null
export function clearAuth(): void

// Login & Signup
export async function loginWithEmail(credentials: LoginCredentials): Promise<AuthToken>
export async function signupWithEmail(data: SignupData): Promise<AuthToken>

// OAuth Providers
export async function loginWithGoogle(idToken: string): Promise<AuthToken>
export async function loginWithGitHub(code: string): Promise<AuthToken>

// Password Management
export async function requestPasswordReset(email: string): Promise<void>
export async function confirmPasswordReset(token: string, newPassword: string): Promise<void>

// Verification
export async function verifyEmail(token: string): Promise<void>
export async function refreshToken(refreshToken: string): Promise<AuthToken>

// Utilities
export function getAuthUser(): any
export function isAuthenticated(): boolean
```

**Key Features:**
- ✅ Token storage in localStorage with expiration tracking
- ✅ Automatic token expiration validation
- ✅ OAuth support (Google, GitHub)
- ✅ Password reset flow
- ✅ Email verification
- ✅ Token refresh mechanism

---

### 2. **Authentication Context** (`lib/auth-context.tsx`)

Provider global que mantiene estado de autenticación:

```typescript
interface AuthContextType {
  isAuthenticated: boolean
  user: any | null
  token: string | null
  isLoading: boolean
  error: string | null
  
  loginEmail: (credentials: LoginCredentials) => Promise<void>
  loginGoogle: (idToken: string) => Promise<void>
  loginGitHub: (code: string) => Promise<void>
  signup: (data: SignupData) => Promise<void>
  logout: () => Promise<void>
  clearError: () => void
}
```

**Custom Hooks:**
- `useAuth()` - Acceso completo al contexto
- `useIsAuthenticated()` - Solo estado de autenticación
- `useUser()` - Solo datos del usuario
- `useToken()` - Solo token

**Usage Example:**
```typescript
'use client';
import { useAuth } from '@/lib/auth-context';

export function MyComponent() {
  const { user, logout, isLoading } = useAuth();
  
  return (
    <div>
      {user && <p>Welcome, {user.name}</p>}
      <button onClick={logout} disabled={isLoading}>
        Logout
      </button>
    </div>
  );
}
```

---

### 3. **Login Page** (`app/login/page.tsx`)

Página completa de login con:

- Email/Password form
- OAuth buttons (Google, GitHub)
- "Remember me" checkbox
- Password recovery link
- Sign up link
- Error handling
- Loading states

**Styling:**
- Gradient background (slate-900 to purple-900)
- Card-based layout con glassmorphism
- Responsive design (mobile-first)
- Dark theme

---

### 4. **Signup Page** (`app/signup/page.tsx`)

Formulario de registro con:

- Full name, email, phone, password fields
- Password confirmation
- Terms & conditions acceptance
- Form validation
- Error handling
- Login link for existing users

---

### 5. **Route Protection** (`middleware.ts`)

Middleware que protege rutas:

```typescript
// Public routes (sin protección)
/login, /signup, /forgot-password, /verify-email, /reset-password, /, /sponsors

// Protected routes (requieren autenticación)
/member/*, /admin/*

// Redirects unauthenticated users to /login
```

---

### 6. **User Menu** (`components/layout/UserMenu.tsx`)

Componente de dropdown menu que muestra:

- Avatar del usuario (iniciales)
- Nombre y email
- Clase (si aplica)
- Link a perfil
- Link a panel de control
- Link a admin (si es admin)
- Botón de logout

---

## 🔄 Integration with Existing Code

### Layout Update (`app/layout.tsx`)

El layout raíz ahora incluye `AuthProvider`:

```typescript
import { AuthProvider } from "@/lib/auth-context";

export default function RootLayout({ children }) {
  return (
    <html>
      <body>
        <AuthProvider>
          <LayoutWrapper>
            {children}
          </LayoutWrapper>
        </AuthProvider>
      </body>
    </html>
  );
}
```

---

## 🔐 Backend Integration

### API Endpoints Required

El sistema espera estos endpoints en el backend:

```
POST /api/auth/login
  Body: { email, password }
  Response: { accessToken, refreshToken?, expiresIn, user }

POST /api/auth/signup
  Body: { name, email, phone, password }
  Response: { accessToken, refreshToken?, expiresIn, user }

POST /api/auth/google
  Body: { idToken }
  Response: { accessToken, refreshToken?, expiresIn, user }

POST /api/auth/github
  Body: { code }
  Response: { accessToken, refreshToken?, expiresIn, user }

POST /api/auth/logout
  Headers: Authorization: Bearer {token}
  Response: { success: true }

POST /api/auth/refresh
  Body: { refreshToken }
  Response: { accessToken, refreshToken?, expiresIn }

POST /api/auth/password-reset/request
  Body: { email }
  Response: { success: true }

POST /api/auth/password-reset/confirm
  Body: { token, newPassword }
  Response: { success: true }

POST /api/auth/verify-email
  Body: { token }
  Response: { success: true }
```

---

## 🔗 Integration with API Service

El `lib/api.ts` ya fue actualizado en ETAPA 8 Phase 0 para incluir:

```typescript
// Auto-inject token from localStorage
function getHeaders() {
  const token = getToken();
  return {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` }),
  };
}

// All API calls automatically include the token
export async function fetchMembers(): Promise<Member[]> {
  return fetchAPI<Member[]>('/api/members');
  // Header Authorization es automáticamente incluido
}
```

---

## 📝 Usage Examples

### Example 1: Protected Component

```typescript
'use client';
import { useAuth } from '@/lib/auth-context';
import { useRouter } from 'next/navigation';

export function AdminPanel() {
  const { user, isAuthenticated } = useAuth();
  const router = useRouter();

  if (!isAuthenticated) {
    return <p>Access denied. Redirecting to login...</p>;
  }

  if (user.role !== 'admin') {
    router.push('/member/dashboard');
    return null;
  }

  return <div>Admin Panel</div>;
}
```

### Example 2: Component with Auth Actions

```typescript
'use client';
import { useAuth } from '@/lib/auth-context';

export function UserActions() {
  const { user, logout, isLoading } = useAuth();

  return (
    <div>
      {user ? (
        <>
          <p>Hello, {user.name}!</p>
          <button onClick={logout} disabled={isLoading}>
            {isLoading ? 'Logging out...' : 'Logout'}
          </button>
        </>
      ) : (
        <p>Not authenticated</p>
      )}
    </div>
  );
}
```

### Example 3: Using Token for API Calls

```typescript
'use client';
import { useToken } from '@/lib/auth-context';
import { fetchMembers } from '@/lib/api';

export function MemberList() {
  const token = useToken();
  const [members, setMembers] = useState([]);

  useEffect(() => {
    if (token) {
      // Token is automatically injected in fetchAPI
      fetchMembers().then(setMembers);
    }
  }, [token]);

  return <div>{members.map(m => <p>{m.name}</p>)}</div>;
}
```

---

## 🧪 Testing the Authentication Flow

### 1. Test Login Form

1. Navigate to `http://localhost:3002/login`
2. Enter credentials (backend required)
3. Click "Iniciar sesión"
4. Should redirect to `/member/dashboard`
5. Token stored in localStorage

### 2. Test Protected Routes

1. Clear localStorage (dev tools)
2. Try to access `/member/dashboard`
3. Should redirect to `/login`

### 3. Test UserMenu

1. Login successfully
2. User avatar should appear in top-right
3. Click to open dropdown
4. Should show user info and logout button
5. Click logout → redirect to login

### 4. Test Signup

1. Navigate to `http://localhost:3002/signup`
2. Fill form with valid data
3. Accept terms
4. Click "Crear cuenta"
5. Should redirect to dashboard with token stored

---

## 🔧 Configuration

### Environment Variables

Add to `.env.local`:

```env
# Backend API
NEXT_PUBLIC_API_BASE_URL=http://localhost:5000

# OAuth Providers (optional for now)
NEXT_PUBLIC_GOOGLE_CLIENT_ID=your_google_client_id
NEXT_PUBLIC_GITHUB_CLIENT_ID=your_github_client_id
```

---

## 📊 State Flow Diagram

```
1. User navigates to /login
   ↓
2. User submits credentials
   ↓
3. loginEmail() calls API endpoint
   ↓
4. Backend validates and returns token + user
   ↓
5. storeToken() saves token to localStorage
   ↓
6. Context updates with user & token
   ↓
7. Page redirects to /member/dashboard
   ↓
8. Protected routes now accessible
```

---

## 🎯 Next Steps (ETAPA 8 Phase 2)

1. **Backend Integration:**
   - Implement auth endpoints in Lama.API
   - Integrate with Entity Framework DbContext
   - Implement JWT token generation

2. **OAuth Setup:**
   - Configure Google OAuth credentials
   - Configure GitHub OAuth credentials
   - Implement OAuth callback handlers

3. **Security:**
   - Add HTTPS/SSL in production
   - Implement rate limiting for login attempts
   - Add CSRF protection
   - Implement secure password hashing

4. **Testing:**
   - Unit tests for auth service
   - Integration tests with backend
   - E2E tests for login flow

---

## 📚 Files Summary

| File | Lines | Purpose |
|------|-------|---------|
| lib/auth.ts | 350+ | Authentication service layer |
| lib/auth-context.tsx | 250+ | React context provider |
| app/login/page.tsx | 280+ | Login UI page |
| app/signup/page.tsx | 320+ | Signup UI page |
| middleware.ts | 45+ | Route protection |
| components/layout/UserMenu.tsx | 180+ | User dropdown menu |
| **TOTAL** | **1,425+** | **Complete auth system** |

---

## ✅ Completion Status

- ✅ Auth service layer (lib/auth.ts)
- ✅ React Context (lib/auth-context.tsx)
- ✅ Login page with email/OAuth
- ✅ Signup page with validation
- ✅ Route middleware protection
- ✅ User menu component
- ✅ Integration with API layer
- ⏳ Backend endpoint implementation
- ⏳ OAuth provider setup
- ⏳ Security hardening

---

**Time Invested:** ~2 hours
**Status:** Frontend authentication infrastructure complete
**Next:** Backend implementation + OAuth setup
