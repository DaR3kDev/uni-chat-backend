---
layout: default
title: Backend
nav_order: 4
description: Documentación técnica del backend de Uni Chat.
---

# Uni Chat Backend

API de chat en tiempo real con ASP.NET Core, autenticación JWT, mensajería asíncrona y persistencia en MongoDB.

**Stack:** .NET 10 · SignalR · MongoDB · Redis · RabbitMQ · Cloudinary

## Responsabilidad

- Autenticación (registro, login, refresh, logout, perfil).
- Contactos y conversaciones directas.
- Mensajes (texto cifrado y archivos vía Cloudinary).
- Eventos en tiempo real (typing, delivered, read).

## Stack tecnológico

| Área | Tecnología |
|------|------------|
| Runtime | .NET 10 (`net10.0`) |
| API | ASP.NET Core Minimal API + SignalR |
| Casos de uso | **MediatR** (commands/queries) + **Wolverine** (eventos/colas) |
| Validación / mapeo | FluentValidation, Mapster |
| Persistencia | MongoDB |
| Caché | Redis (StackExchange.Redis) |
| Colas | RabbitMQ (vía WolverineFx) |
| Logs | Serilog + Seq (Docker) |
| Documentación API | OpenAPI + Scalar (`/scalar/v1`) |

## Estructura del proyecto

Directorio `uni-chat-backend/` (proyecto .NET):

| Carpeta | Contenido |
|---------|-----------|
| `API/` | Endpoints HTTP, configuración de middleware, hub SignalR |
| `Application/` | Comportamientos transversales (pipeline MediatR) |
| `Domain/` | Entidades y enums |
| `Features/` | Vertical slices: Auth, Contacts, Conversations, Messages |
| `Infrastructure/` | Repositorios, seguridad (`E2EEncryptionService`), DI, SignalR |
| `Program.cs` | Composición de servicios y arranque |

Todos los comandos `make` y `docker compose` se ejecutan desde **`uni-chat-backend/uni-chat-backend/`**. Guía completa: [Instalación]({{ site.baseurl }}/instalacion.html).

## Configuración local

```bash
cd uni-chat-backend/uni-chat-backend
cp .env.ci.example .env
# Ajustar appsettings.json (localhost) y Cloudinary en .env si hace falta
make up
make run
```

Claves en `appsettings.json` (API en el host): `Mongo`, `Redis`, `RabbitMQ`, `Jwt`, `RefreshToken`, `Cloudinary`.

## Endpoints HTTP

### Auth

| Método | Ruta |
|--------|------|
| `POST` | `/api/auth/register` |
| `POST` | `/api/auth/login` |
| `GET` | `/api/auth/me` |
| `POST` | `/api/auth/refresh` |
| `POST` | `/api/auth/logout` |

### Contacts

| Método | Ruta |
|--------|------|
| `POST` | `/api/contacts` |
| `GET` | `/api/contacts` |
| `DELETE` | `/api/contacts/{contactId:guid}` |

### Conversations

| Método | Ruta |
|--------|------|
| `POST` | `/api/conversations/direct` |
| `GET` | `/api/conversations` |
| `POST` | `/api/conversations/{conversationId:guid}/join` |

### Messages

| Método | Ruta |
|--------|------|
| `POST` | `/api/messages/send` |
| `POST` | `/api/messages/upload` |
| `GET` | `/api/messages/conversation/{conversationId:guid}` |
| `DELETE` | `/api/messages/{messageId:guid}` |

Requiere cabecera `Authorization: Bearer <token>` salvo registro/login.

## SignalR

- **Hub:** `/messages/chat`
- **Autenticación:** JWT (`[Authorize]`)

Métodos del hub (`ChatHub`):

| Método | Descripción |
|--------|-------------|
| `JoinConversation` | Unirse al grupo de la conversación |
| `LeaveConversation` | Salir del grupo |
| `SendMessage` | Enviar mensaje (delega en `SendMessageCommand` / MediatR) |
| `TypingStarted` / `TypingStopped` | Indicador de escritura |
| `MessageDelivered` / `MessageRead` | Estados de entrega y lectura |

Eventos hacia el cliente incluyen `ReceiveMessage`, `UserTyping`, `MessageDelivered`, `MessageRead`, `JoinedConversation`.

Conexión: misma base URL que la API; negociar token en la conexión SignalR según tu cliente.

## Cifrado (E2EE)

Los mensajes de texto se cifran con **AES-256** y clave por conversación antes de guardarse en MongoDB. Detalle: [E2EE]({{ site.baseurl }}/chat-privado/e2ee.html).

## Flujo recomendado

1. `make up` — infra en Docker
2. Configurar `appsettings.json` y `.env`
3. `make run` — API en http://localhost:5012
4. Abrir Scalar: http://localhost:5012/scalar/v1
5. Registrar usuario, crear conversación, enviar mensajes por HTTP o hub

{: .important }
> **Importante:** No subas `.env`, claves JWT ni credenciales Cloudinary al repositorio. Producción: [CI/CD y secrets]({{ site.baseurl }}/github-secrets.html).
