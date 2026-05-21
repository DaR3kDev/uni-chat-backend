---
layout: default
title: Arquitectura
parent: Chat privado / E2E
nav_order: 1
---

# Arquitectura Vertical Slice

La **Arquitectura Vertical Slice** organiza el código por funcionalidades (_features_) o casos de uso, en lugar de por capas técnicas (presentación, negocio, datos). Todo lo necesario para una funcionalidad vive en la misma carpeta.

## Concepto

En arquitectura por capas, un cambio funcional implica tocar controlador, servicio y repositorio en carpetas distintas. Con Vertical Slice, **cada feature agrupa comando/query, handler y endpoint**:

```
Features/
├── Messages/
│   └── SendMessage/
│       ├── SendMessageCommand.cs
│       ├── SendMessageHandler.cs
│       └── (endpoint en API/Endpoints/Messages/)
├── Auth/
│   └── Login/
│       ├── LoginCommand.cs
│       ├── LoginHandler.cs
│       └── LoginEndpoint.cs
└── Conversations/
    └── GetOrCreateDirect/
        ├── GetOrCreateConversationCommand.cs
        └── GetOrCreateConversationHandler.cs
```

En este repositorio los endpoints HTTP viven bajo `API/Endpoints/` y los handlers bajo `Features/`, siguiendo el mismo criterio por dominio.

## Comparación con arquitectura por capas

| | Por capas | Vertical Slice |
|---|-----------|------------------|
| Organización | Por tipo técnico | Por funcionalidad |
| Cohesión | Baja (piezas dispersas) | Alta (todo en un slice) |
| Acoplamiento | Alto entre capas | Bajo entre slices |
| Impacto de cambios | Varias carpetas | Un slice |
| Trabajo en equipo | Más conflictos en merge | Equipos por feature |

```
Arquitectura por capas        Vertical Slice
─────────────────────         ──────────────────────────────
  Presentación                  Slice 1 │ Slice 2 │ Slice 3
  Lógica de negocio             UI      │ UI      │ UI
  Acceso a datos                Datos   │ Datos   │ Datos
```

## Ventajas

**Alta cohesión:** las piezas que cambian juntas permanecen juntas.

**Bajo acoplamiento:** las slices se comunican de forma explícita (MediatR, eventos Wolverine).

**Refactorización localizada:** eliminar una feature puede reducirse a borrar su carpeta.

**Desarrollo ágil:** entregas pequeñas por caso de uso sin invadir otras áreas.

## Desventajas y mitigación

**Posible duplicidad de código** si no se gestionan componentes compartidos:

- Extraer solo lo transversal a `Infrastructure/` o contratos en `Domain/`
- Compartir interfaces, no lógica de negocio acoplada
- Revisar duplicaciones al diseñar nuevas slices

## Implementación en este proyecto

Cada slice suele incluir:

1. **Command o Query** — intención (escritura o lectura)
2. **Handler** — lógica de negocio (`IRequestHandler` de **MediatR**)
3. **Endpoint** — exposición HTTP en `API/Endpoints/`

### MediatR vs Wolverine

| Herramienta | Uso en Uni Chat |
|-------------|-----------------|
| **MediatR** | Despacho de comandos/queries desde endpoints HTTP y desde `ChatHub` (SignalR) |
| **Wolverine** | Publicación de eventos asíncronos (p. ej. `SendMessageEvent` tras guardar un mensaje) y transporte RabbitMQ |

Ejemplo: `SendMessageHandler` implementa `IRequestHandler<SendMessageCommand, …>` (MediatR) y publica `SendMessageEvent` con `IMessageBus` (Wolverine) para procesamiento en cola.

{: .note }
> **Info:** CQRS separa comandos (escritura) y queries (lectura). En este proyecto ambos pasan por MediatR; la validación transversal usa **FluentValidation** registrada en el pipeline de la API.

Más detalle de infraestructura en [Componentes]({{ site.baseurl }}/chat-privado/componentes.html).
