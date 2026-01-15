# 🚀 Deployment Guide - L.A.M.A. Mototurismo

Guía completa para desplegar a producción.

---

## 📋 Pre-Deployment Checklist

### Código & Build

```bash
# 1. Verificar que no hay errores
npm run build
# Expected: ✓ Compiled successfully, 15 static pages

# 2. Verificar linting
npm run lint
# Expected: Ningún error

# 3. Verificar tipos TypeScript
npm run type-check
# Expected: Ningún error
```

### Configuración

```bash
# 1. Revisar .env.local
cat .env.local
# Asegurar: NEXT_PUBLIC_API_BASE_URL=https://api.lama.com

# 2. Revisar next.config.js
# Verificar: headers, redirects, imágenes, etc.

# 3. Revisar package.json
# Verificar: dependencies pinned, scripts correctos
```

### Documentación

```bash
# 1. README.md actualizado
# 2. ARCHITECTURE.md presente
# 3. COMPONENTS.md presente
# 4. TESTING.md presente
# 5. CHANGELOG.md actualizado
```

---

## 🌐 Opciones de Deployment

### Opción 1: Vercel (Recomendado para Next.js)

**Ventajas**:
- Deployment automático en cada push
- CDN global incluido
- Edge functions
- Serverless functions
- Analytics incluida
- Free tier generoso

**Pasos**:

```bash
# 1. Instalar Vercel CLI
npm i -g vercel

# 2. Loguear en Vercel
vercel login

# 3. Deployar a staging (preview)
vercel

# 4. Deployar a producción
vercel --prod
```

**Configuración en Vercel Dashboard**:

1. Conectar repositorio GitHub
2. Importar proyecto
3. Variables de entorno:
   ```
   NEXT_PUBLIC_API_BASE_URL=https://api.lama.com
   NEXT_PUBLIC_ENVIRONMENT=production
   ```
4. Build: `npm run build`
5. Start: `npm start`

**URL Resultante**: `https://lama-web.vercel.app`

---

### Opción 2: Docker + Cloud Run (Google Cloud)

**Dockerfile**:

```dockerfile
FROM node:18-alpine

WORKDIR /app

# Copiar package files
COPY package*.json ./

# Instalar dependencias
RUN npm ci

# Copiar código
COPY . .

# Build
RUN npm run build

# Exponer puerto
EXPOSE 3000

# Start
CMD ["npm", "start"]
```

**Deploy a Cloud Run**:

```bash
# 1. Construir imagen
docker build -t lama-web:latest .

# 2. Taggear para Google Cloud Registry
docker tag lama-web:latest gcr.io/PROJECT_ID/lama-web:latest

# 3. Push a registry
docker push gcr.io/PROJECT_ID/lama-web:latest

# 4. Deploy a Cloud Run
gcloud run deploy lama-web \
  --image gcr.io/PROJECT_ID/lama-web:latest \
  --platform managed \
  --region us-central1 \
  --port 3000
```

---

### Opción 3: AWS Amplify

**Pasos**:

1. Conectar repositorio GitHub a AWS Amplify
2. Configurar build:
   ```yaml
   version: 1
   frontend:
     phases:
       preBuild:
         commands:
           - npm ci
       build:
         commands:
           - npm run build
     artifacts:
       baseDirectory: .next
       files:
         - '**/*'
     cache:
       paths:
         - node_modules/**/*
   ```
3. Variables de entorno en Amplify Console
4. Deploy automático

---

### Opción 4: Self-Hosted (VPS/Servidor Propio)

**Requisitos**:
- VPS con Node.js 18+ (Ubuntu 22.04 recomendado)
- PM2 para process management
- Nginx como reverse proxy
- SSL certificate (Let's Encrypt)

**Instalación**:

```bash
# 1. SSH en servidor
ssh root@your-server.com

# 2. Instalar Node.js
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# 3. Instalar PM2
sudo npm install -g pm2

# 4. Clonar repositorio
git clone https://github.com/.../lama-web.git
cd lama-web

# 5. Instalar dependencias
npm ci

# 6. Build
npm run build

# 7. Configurar PM2
pm2 start npm --name "lama-web" -- start
pm2 save
pm2 startup

# 8. Nginx reverse proxy
sudo apt-get install nginx
# Configurar /etc/nginx/sites-available/default
sudo systemctl restart nginx
```

**Nginx Config** (`/etc/nginx/sites-available/default`):

```nginx
server {
    listen 80;
    server_name lama-mototurismo.com www.lama-mototurismo.com;
    
    # Redirect HTTP a HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name lama-mototurismo.com www.lama-mototurismo.com;
    
    # SSL Certificates (Let's Encrypt)
    ssl_certificate /etc/letsencrypt/live/lama-mototurismo.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/lama-mototurismo.com/privkey.pem;
    
    # Security headers
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    
    # Proxy to Node.js
    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

**SSL con Let's Encrypt**:

```bash
sudo apt-get install certbot python3-certbot-nginx
sudo certbot certonly --nginx -d lama-mototurismo.com -d www.lama-mototurismo.com
sudo certbot renew --dry-run
```

---

## 📊 Monitoreo Post-Deploy

### Health Checks

```typescript
// app/api/health/route.ts
export async function GET() {
  return Response.json(
    {
      status: 'ok',
      timestamp: new Date().toISOString(),
      version: process.env.NEXT_PUBLIC_APP_VERSION,
    },
    { status: 200 }
  );
}
```

URL: `https://lama-mototurismo.com/api/health`

### Logs

**Vercel**: Dashboard → Deployments → Runtime Logs
**Docker**: `docker logs container-name`
**PM2**: `pm2 logs`

### Alertas

Configurar en:
- Uptime monitoring (UptimeRobot, Pingdom)
- Error tracking (Sentry, Rollbar)
- Performance monitoring (New Relic, DataDog)

---

## 🔄 CI/CD Pipeline

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Deploy

on:
  push:
    branches: [master]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'
      
      - name: Install dependencies
        run: npm ci
      
      - name: Lint
        run: npm run lint
      
      - name: Build
        run: npm run build
      
      - name: Deploy to Vercel
        uses: vercel/action@master
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID }}
          production: true
```

---

## 🔐 Seguridad en Producción

### Headers de Seguridad

```javascript
// next.config.js
async headers() {
  return [
    {
      source: '/:path*',
      headers: [
        {
          key: 'Strict-Transport-Security',
          value: 'max-age=31536000; includeSubDomains'
        },
        {
          key: 'X-Frame-Options',
          value: 'SAMEORIGIN'
        },
        {
          key: 'X-Content-Type-Options',
          value: 'nosniff'
        },
        {
          key: 'X-XSS-Protection',
          value: '1; mode=block'
        },
        {
          key: 'Referrer-Policy',
          value: 'strict-origin-when-cross-origin'
        },
      ],
    },
  ];
}
```

### Environment Variables

```bash
# Producción
NEXT_PUBLIC_API_BASE_URL=https://api.lama.com
NEXT_PUBLIC_ENVIRONMENT=production
# NUNCA exponer secrets en NEXT_PUBLIC_*
```

### HTTPS Obligatorio

- [ ] Certificado SSL válido
- [ ] HTTP → HTTPS redirect
- [ ] HSTS headers configurados
- [ ] Mixed content warnings revisados

---

## 📈 Performance Optimization para Producción

### Caching Headers

```javascript
// next.config.js
headers: [
  {
    source: '/fonts/:path*',
    headers: [
      {
        key: 'Cache-Control',
        value: 'public, max-age=31536000, immutable'
      }
    ]
  },
  {
    source: '/_next/static/:path*',
    headers: [
      {
        key: 'Cache-Control',
        value: 'public, max-age=31536000, immutable'
      }
    ]
  }
]
```

### Image Optimization

```typescript
// Already configured in next.config.js
images: {
  formats: ['image/avif', 'image/webp'],
  minimumCacheTTL: 60 * 60 * 24 * 365,
}
```

### Compression

Next.js automáticamente comprime con:
- Gzip para navegadores legacy
- Brotli para navegadores modernos

---

## 🔄 Rollback Plan

**Si deployment falla**:

```bash
# Vercel
vercel rollback

# Docker
docker pull gcr.io/PROJECT/lama-web:previous
docker stop lama-web
docker run -d --name lama-web gcr.io/PROJECT/lama-web:previous

# GitHub Actions
Revert commit que causó problema
git revert <commit-hash>
git push origin master
# Auto-redeploy en push
```

---

## 📝 Post-Deploy Checklist

- [ ] HTTPS funciona
- [ ] Dominio apunta correctamente
- [ ] API conecta a backend
- [ ] Mails de notificación llegan
- [ ] Analytics trackea pageviews
- [ ] Error logging funciona
- [ ] Performance scores ≥ 90
- [ ] Responsive en mobile
- [ ] SEO metadata visible en browser
- [ ] Cross-browser testing (Chrome, Firefox, Safari)
- [ ] Load testing bajo carga (¿10, 100, 1000 usuarios simultáneos?)

---

## 📞 Contacts & Escalation

**Equipo Técnico**:
- DevOps Lead: [nombre]
- Backend Lead: [nombre]
- Frontend Lead: [nombre]

**Incidentes**:
- Slack: #incidents
- PagerDuty: [link]
- Status Page: [link]

**Runbook**:
1. Detectar incidente
2. Notificar equipo
3. Accionar rollback si necesario
4. Investigar causa raíz
5. Post-mortem y lecciones aprendidas

---

**Última actualización**: Enero 15, 2026
**Versión**: 2.0.0
**Status**: ✅ Ready for Production
