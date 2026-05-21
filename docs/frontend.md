---
layout: default
title: Frontend
nav_order: 5
description: Documentación técnica del frontend de Uni Chat.
---

# Uni Chat Frontend

Aplicación web de chat en tiempo real construida con React, TypeScript y Vite.

**Stack:** React 19 · TypeScript · Vite 7 · SignalR · TanStack

## Stack principal

- React 19 + TypeScript
- Vite 7
- TanStack Router + Query
- Zustand + Axios
- SignalR + Tailwind CSS 4

## Capacidades clave

- Autenticación y rutas protegidas
- Chat en tiempo real con hub SignalR
- Estado global y sesiones persistentes
- Interfaz modular con componentes reutilizables

## Arquitectura del frontend

Estructura base dentro de `src/`:

- `app/`: configuración global, layouts, providers y rutas.
- `entities/`: dominio y acceso a datos por entidad.
- `features/`: casos de uso de UI.
- `pages/`: composición de pantallas.
- `widgets/`: bloques reutilizables grandes.
- `shared/`: utilidades transversales.

## Flujo de autenticación

1. Usuario inicia sesión o se registra.
2. Se guarda token en `localStorage` (`authStorage`).
3. `AuthHydrator` valida sesión en `auth/me`.
4. Si no hay sesión, se limpia estado y redirige a `/login`.
5. Si hay sesión, `useAuthStore` marca autenticación válida.

## API REST y SignalR

### Cliente HTTP

El cliente Axios en `src/shared/api/http.ts`:

- Usa `VITE_API_URL` como `baseURL`.
- Configura `withCredentials: true`.
- Agrega `Authorization: Bearer <token>`.
- Ante `401`, intenta refresh (`POST /auth/refresh`).

### Tiempo real

La conexión SignalR en `src/features/chat/signalr/chat-hub-connection.ts`:

- Toma URL de `VITE_SIGNALR_URL`.
- Conecta a `/messages/chat`.
- Aplica reconexión automática.

## Rutas principales

| Ruta | Descripción |
| --- | --- |
| `/` | Home pública |
| `/(auth)/login` | Login |
| `/(auth)/register` | Registro |
| `/_protected/chat` | Chat protegido |

## Variables de entorno

```env
VITE_API_URL=http://localhost:3000
VITE_SIGNALR_URL=http://localhost:3000
```

## Instalación y ejecución local

```bash
bun install
bun run dev
```

## Scripts disponibles

| Script | Uso |
|--------|-----|
| `bun run dev` | Entorno de desarrollo |
| `bun run build` | Build de producción |
| `bun run lint` | Validación de calidad |
| `bun run preview` | Previsualización local |

> **Tip:** Verifica primero `VITE_API_URL` y `VITE_SIGNALR_URL` cuando haya errores de conexión o autenticación.
