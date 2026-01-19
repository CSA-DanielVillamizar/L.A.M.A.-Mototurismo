import { Configuration, LogLevel } from '@azure/msal-browser';

/**
 * Configuración de MSAL para Microsoft Entra External ID (B2C)
 * Docs: https://learn.microsoft.com/azure/active-directory-b2c/tutorial-single-page-app
 */

// Configuración desde variables de entorno
const ENTRA_AUTHORITY =
  process.env.NEXT_PUBLIC_ENTRA_AUTHORITY ||
  'https://lama-moto.b2clogin.com/lama-moto.onmicrosoft.com/B2C_1_signin';

const ENTRA_CLIENT_ID = process.env.NEXT_PUBLIC_ENTRA_CLIENT_ID || '12345678-1234-1234-1234-123456789012';

const ENTRA_REDIRECT_URI = process.env.NEXT_PUBLIC_ENTRA_REDIRECT_URI || 'http://localhost:3002/auth/callback';

const ENTRA_KNOWN_AUTHORITIES = (process.env.NEXT_PUBLIC_ENTRA_KNOWN_AUTHORITIES || 'lama-moto.b2clogin.com').split(
  ','
);

export const msalConfig: Configuration = {
  auth: {
    clientId: ENTRA_CLIENT_ID,
    authority: ENTRA_AUTHORITY,
    knownAuthorities: ENTRA_KNOWN_AUTHORITIES,
    redirectUri: ENTRA_REDIRECT_URI,
    postLogoutRedirectUri: '/',
    navigateToLoginRequestUrl: false,
  },
  cache: {
    cacheLocation: 'sessionStorage', // Para tokens de Entra (NO app tokens)
    storeAuthStateInCookie: false,
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            break;
          case LogLevel.Info:
            console.info(message);
            break;
          case LogLevel.Verbose:
            console.debug(message);
            break;
          case LogLevel.Warning:
            console.warn(message);
            break;
        }
      },
      logLevel: process.env.NODE_ENV === 'production' ? LogLevel.Warning : LogLevel.Info,
    },
    // Configuración adicional de sistema
  },
};

/**
 * Scopes solicitados a Entra ID
 * Solo necesitamos openid, profile, email para obtener id_token
 * En modo desarrollo sin Entra real, estos scopes pueden no estar disponibles
 */
export const loginRequest = {
  scopes: ['openid', 'profile', 'email'],
};
