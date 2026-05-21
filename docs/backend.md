---
layout: default
title: Backend
nav_order: 4
description: Documentación técnica del backend de Uni Chat.
---

# Uni Chat Backend

API de chat en tiempo real con ASP.NET Core, autenticación JWT, mensajería asíncrona y persistencia distribuida.

**Stack:** .NET 10 · SignalR · MongoDB · Redis · RabbitMQ

## Responsabilidad y objetivo

- **Responsabilidad:** autenticación, conversaciones, mensajes y eventos en tiempo real.
- **Objetivo técnico:** escalar con baja latencia y mantener seguridad extremo a extremo.

## Descripción funcional

La API cubre:

- Autenticación de usuarios (registro, login, refresh, logout, perfil).
- Gestión de contactos.
- Conversaciones directas.
- Envío y consulta de mensajes (texto y archivos).
- Eventos realtime (typing, delivered, read).

## Stack tecnológico

| Área | Tecnología |
|------|------------|
| Runtime | .NET 10 (`net10.0`) |
| Framework | ASP.NET Core Minimal API + SignalR |
| Persistencia | MongoDB + Cloudinary |
| Infra | Redis + RabbitMQ |
| Arquitectura | MediatR + FluentValidation + Mapster |
| Documentación API | OpenAPI + Scalar |

## Estructura principal

Directorio `uni-chat-backend/`:

- `API/`: endpoints HTTP, hub y middlewares.
- `Application/`: comportamientos transversales.
- `Domain/`: entidades y enums.
- `Features/`: casos de uso por módulo.
- `Infrastructure/`: persistencia, seguridad y DI.
- `Program.cs`: composición de servicios.

## Requisitos previos

- .NET SDK 10
- Docker + Docker Compose

## Configuración local

1. Copiar variables de entorno:

```bash
cd uni-chat-backend
cp .env.example .env
```

2. Configurar `appsettings.json`:

- `Mongo.ConnectionString`
- `Mongo.Database`
- `Redis.ConnectionString`
- `Jwt.Key`, `Jwt.Issuer`, `Jwt.Audience`
- `RefreshToken.ExpireDays`
- `Cloudinary.CloudName`, `Cloudinary.ApiKey`, `Cloudinary.ApiSecret`

## Levantar dependencias

```bash
docker compose up -d
```

> **Info:** Servicios: MongoDB, Redis, RabbitMQ y paneles de administración asociados.

## Ejecutar API

```bash
dotnet restore
dotnet run
```

URLs locales: `http://localhost:5012` y `https://localhost:7155`.

## Endpoints principales

### Auth

| Método | Endpoint |
| --- | --- |
| `POST` | `/api/auth/register` |
| `POST` | `/api/auth/login` |
| `GET` | `/api/auth/me` |
| `POST` | `/api/auth/refresh` |
| `POST` | `/api/auth/logout` |

### Contacts

| Método | Endpoint |
| --- | --- |
| `POST` | `/api/contacts` |
| `GET` | `/api/contacts` |
| `DELETE` | `/api/contacts/{contactId:guid}` |

### Conversations

| Método | Endpoint |
| --- | --- |
| `POST` | `/api/conversations/direct` |
| `GET` | `/api/conversations` |
| `POST` | `/api/conversations/{conversationId:guid}/join` |

### Messages

| Método | Endpoint |
| --- | --- |
| `POST` | `/api/messages/send` |
| `POST` | `/api/messages/upload` |
| `GET` | `/api/messages/conversation/{conversationId:guid}` |
| `DELETE` | `/api/messages/{messageId:guid}` |

## Realtime con SignalR

- Hub: `/messages/chat`
- Requiere autenticación (`[Authorize]`)

Métodos clave: `JoinConversation`, `SendMessage`, `TypingStarted`, `TypingStopped`, `MessageDelivered`, `MessageRead`.

## Flujo recomendado de inicio

1. `docker compose up -d`
2. Configurar `appsettings.json`
3. `dotnet run`
4. Registrar o iniciar sesión
5. Crear conversación
6. Enviar mensajes por HTTP o SignalR

> **Nota:** No subas secretos reales (`.env`, JWT, credenciales Cloudinary) al repositorio.
