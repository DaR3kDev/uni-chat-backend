---
layout: default
title: Componentes
parent: Chat privado / E2E
nav_order: 2
---

# Componentes del sistema

La arquitectura integra varios componentes que trabajan en conjunto para comunicación segura, escalable y en tiempo real.

## Mapa de componentes

| Componente | Rol | Tecnología |
|------------|-----|------------|
| Web API | Punto de entrada HTTP | ASP.NET Core (.NET 10) |
| ChatHub | Tiempo real | SignalR (`/messages/chat`) |
| MediatR | Comandos/queries síncronos | Handlers por feature |
| Wolverine | Eventos y colas | RabbitMQ |
| Base de datos | Persistencia | MongoDB |
| Caché | Estado y invalidación rápida | Redis (StackExchange.Redis) |
| Mensajería asíncrona | Broker | RabbitMQ |
| Archivos multimedia | Media en la nube | Cloudinary |
| Cifrado | Contenido de texto | AES-256 (`E2EEncryptionService`) |

## Flujo de un mensaje

```
┌────────────────────────────────────────────────────────────┐
│                    Flujo de un Mensaje                     │
└────────────────────────────────────────────────────────────┘

  01 Autenticación     JWT (HTTP o conexión SignalR).
         │
         ▼
  02 Conexión RT       SignalR ChatHub.
         │
         ▼
  03 Suscripción       JoinConversation (grupo por conversación).
         │
         ▼
  04 Enviar mensaje    HTTP POST /api/messages/send o hub SendMessage.
         │
         ▼
  05 Procesamiento     MediatR → SendMessageHandler:
                       cifrado AES-256 → MongoDB → invalidar caché Redis
                       → Wolverine PublishAsync (evento).
         │
         ▼
  06 Distribución      Clientes en el grupo SignalR (ReceiveMessage).
```

## Interacción entre componentes

```
Cliente A                    Servidor                    Cliente B
   │                            │                            │
   │── JWT Auth ──────────────► │                            │
   │◄─ Token ────────────────── │                            │
   │── SignalR Connect ────────► │                            │
   │── JoinConversation ───────► │                            │
   │── SendMessage ────────────► │                            │
   │                    MediatR + E2E encrypt                 │
   │                    MongoDB + Redis                       │
   │                    Wolverine → RabbitMQ (evento)         │
   │                            │── SignalR Push ───────────► │
```

Detalle del cifrado en [E2EE]({{ site.baseurl }}/chat-privado/e2ee.html).

---

## MongoDB

Base de datos NoSQL de documentos (**BSON**), con esquemas flexibles por colección.

### Colecciones en este proyecto

Base de datos configurada en `appsettings.json` como **`chat_db`** (ajusta el nombre en tu entorno):

```
chat_db
├── users
├── conversations    (incluye EncryptionKey por conversación)
├── messages         (content cifrado para texto)
├── contacts
└── refreshTokens
```

### Ejemplo de documento de mensaje

```json
{
  "_id": "guid",
  "conversationId": "guid",
  "senderId": "guid",
  "content": "<base64 cifrado AES-256>",
  "type": "TEXT",
  "fileUrl": "https://res.cloudinary.com/...",
  "fileName": "archivo.pdf",
  "createdAt": "2026-06-01T10:30:00Z"
}
```

{: .note }
> **Info:** El campo `content` para mensajes de tipo texto se almacena **cifrado**. Ver [E2EE]({{ site.baseurl }}/chat-privado/e2ee.html).

### Por qué MongoDB aquí

- Mensajes con formas distintas (texto, archivos, metadatos)
- Alta concurrencia de escrituras
- Complemento con Redis para estado y caché de listados

---

## Redis

Capa en memoria **clave-valor**; no sustituye a MongoDB.

```
  Request ──► Redis (caché) ── HIT ──► respuesta rápida
                  │
                  MISS
                  ▼
              MongoDB ──► respuesta + actualizar caché
```

En `SendMessageHandler`, tras crear un mensaje se invalidan claves como `messages:{conversationId}` y `conversations:{userId}`, y se incrementan contadores de no leídos.

| Uso | Descripción |
|-----|-------------|
| Caché de listados | Conversaciones y mensajes recientes |
| Contadores | Mensajes no leídos por usuario/conversación |
| Estado online | Gestionado también vía repositorio (`SetUserOnlineAsync` en el hub) |

{: .warning }
> **Aviso:** Redis guarda datos **transitorios**. La fuente de verdad de mensajes es MongoDB.

---

## RabbitMQ

Broker para desacoplar productores y consumidores. Si el receptor no está conectado, el mensaje puede permanecer en cola hasta su consumo.

| | Sin cola (síncrono) | Con RabbitMQ |
|---|---------------------|--------------|
| Dependencia | Receptor disponible al instante | Puede estar offline |
| Pérdida | Posible ante fallos | Persistencia en cola |
| Escala | Limitada | Workers adicionales |

Wolverine se configura en `AddWolverineConfiguration` (`Program.cs`) con transporte RabbitMQ para eventos como el publicado tras enviar un mensaje.

{: .note }
> **Info:** La API HTTP y SignalR usan **MediatR** de forma síncrona; **Wolverine** encola trabajo/eventos hacia RabbitMQ cuando aplica.

---

## Wolverine

Framework de mensajería para .NET orientado a handlers y publicación de mensajes.

```
  HTTP / SignalR
       │
       ▼
  MediatR → SendMessageHandler
       │         ├── MongoDB
       │         ├── Redis
       │         └── IMessageBus.PublishAsync(SendMessageEvent)
       ▼
  Wolverine + RabbitMQ (consumidores / handlers de evento)
```

Los comandos de lectura (`GetMessagesQuery`, etc.) y la mayoría de escrituras HTTP siguen el patrón **MediatR + FluentValidation** en `Features/`.

Beneficios: handlers por caso de uso, pruebas unitarias sencillas y posibilidad de escalar consumidores de cola.
