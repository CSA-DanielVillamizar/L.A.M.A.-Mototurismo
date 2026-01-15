# 🚀 Guía de Inicio Rápido - LAMA Web Frontend

## Instalación Express (5 minutos)

### 1. Prerequisitos
Asegúrate de tener instalado:
- Node.js 20+ ([descargar](https://nodejs.org/))
- Backend .NET 8 corriendo en puerto 5000

### 2. Instalación

```bash
# Navegar al directorio
cd src/Lama.Web

# Instalar dependencias (toma ~2 minutos)
npm install

# Configurar variables de entorno
echo NEXT_PUBLIC_API_BASE_URL=http://localhost:5000 > .env.local

# Iniciar servidor de desarrollo
npm run dev
```

### 3. Verificar

1. Abre tu navegador en: `http://localhost:3000`
2. Deberías ver la página de inicio del sistema COR
3. Navega a: `http://localhost:3000/evidence/upload`
4. El formulario debería cargar y mostrar "X tipos de estado cargados desde el backend"

## 📋 Checklist de Verificación

- [ ] Backend .NET corriendo en puerto 5000
- [ ] `npm install` completado sin errores
- [ ] `.env.local` creado con URL correcta
- [ ] `npm run dev` inicia sin errores
- [ ] Navegador muestra página de inicio
- [ ] Formulario de evidencia carga correctamente
- [ ] Mensaje "33 tipos de estado cargados" aparece

## 🐛 Solución de Problemas Comunes

### Error: "Cannot connect to API"
**Causa**: Backend no está corriendo
**Solución**: 
```bash
# En otra terminal, inicia el backend .NET
cd src/Lama.API
dotnet run
```

### Error: "Module not found"
**Causa**: Dependencias no instaladas
**Solución**:
```bash
rm -rf node_modules package-lock.json
npm install
```

### Error: "Port 3000 already in use"
**Causa**: Otro proceso usa el puerto 3000
**Solución**:
```bash
# Usar otro puerto
npm run dev -- -p 3001
```

### Error: "CORS policy"
**Causa**: Backend no permite requests desde localhost:3000
**Solución**: Agregar en `Program.cs` del backend:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

app.UseCors("AllowLocalhost");
```

## 📖 Primeros Pasos

### Probar el formulario de evidencia

1. Abre: `http://localhost:3000/evidence/upload`

2. Llena los campos:
   - Event ID: `1`
   - Member ID: `1`
   - Vehicle ID: `1`
   - Tipo de Evidencia: `START_YEAR`
   - Foto Piloto: Selecciona cualquier imagen
   - Foto Odómetro: Selecciona cualquier imagen
   - Lectura: `12345.5`
   - Unidad: `Kilometers`

3. Click en "Subir Evidencia"

4. Si todo está correcto, deberías ver:
   - ✅ Mensaje de éxito verde
   - Puntos totales otorgados
   - Detalles de la asistencia creada

## 🎯 Endpoints Disponibles

### Frontend
- `/` - Página de inicio
- `/evidence/upload` - Formulario de subida de evidencia

### Backend (API)
- `GET /api/MemberStatusTypes` - Lista de 33 tipos de estado
- `POST /api/admin/evidence/upload` - Subir evidencia

## 📞 Ayuda

Si encuentras problemas:

1. Revisa la consola del navegador (F12)
2. Revisa la terminal donde corre `npm run dev`
3. Verifica que el backend esté respondiendo:
   ```bash
   curl http://localhost:5000/api/MemberStatusTypes
   ```

## ✅ ¡Listo!

Ahora puedes empezar a usar el sistema COR para subir evidencias de asistencia.
