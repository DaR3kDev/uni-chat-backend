---
layout: default
title: Chat privado / E2E
nav_order: 3
---

# Sistema de Chat Privado

**Autores:** Julián Andrés Caracas Sánchez · Kevin Villegas · Santiago Méndez · José David Zuluaga

**Institución:** Corporación de Estudios Tecnológicos del Norte del Valle — Unidad de Ingeniería, Arquitectura, Urbanismo y Afines · Cartago Valle · Junio 2026

---

## Guía rápida del documento

Este documento es la referencia principal del proyecto. Resume el problema, el alcance, la arquitectura, los componentes y las decisiones de seguridad del sistema.

### Contenido

- Introducción
- Justificación
- Planteamiento del Problema
- Propósito
- Alcance
- Cambios Respecto a la Versión Anterior
- Arquitectura Vertical Slice
- Componentes del Sistema
- MongoDB - Base de Datos NoSQL
- Redis - Caché en Memoria
- RabbitMQ - Mensajería Asíncrona
- Wolverine - Bus de Mensajes Interno
- Cifrado de Extremo a Extremo (E2EE)

## Introducción

El desarrollo de muchas aplicaciones se basa en utilizar la información de las personas con fines comerciales. Como consecuencia, quien las usa pierde el control sobre sus datos y contactos. Por este motivo, el presente sistema nace inspirado en la filosofía de Brave, enfocado puramente en proteger la privacidad.

La herramienta pide solo lo estrictamente necesario para funcionar de forma segura. De esta manera, se garantiza un espacio digital sin rastreadores ni venta de información a terceros, donde la protección de quien la utiliza es el único objetivo.

## Justificación

Muchas aplicaciones funcionan intercambiando el uso de la herramienta por datos personales para publicidad. Este sistema surge como una alternativa donde la información de la persona no es una moneda de cambio.

El desarrollo se basa en el sentido común: pedir únicamente los datos estrictamente necesarios para funcionar. No existen rastreadores ocultos ni intereses comerciales en vender perfiles a terceros. El control de lo que se comparte se queda siempre en manos de quien usa el sistema.

## Planteamiento del Problema

Es común que las herramientas digitales exijan información personal que termina en perfiles comerciales o compartida con terceros, lo que provoca que el usuario pierda el control de sus datos. Gran parte de estos sistemas rastrean la actividad en segundo plano, aunque no sea necesario para su funcionamiento.

El respeto a la intimidad ha quedado relegado, generando una falta de opciones seguras. Por esta razón, surge la necesidad de crear un sistema que no vea a la persona como una simple fuente de datos, ofreciendo una alternativa clara que proteja la información y garantice un intercambio de mensajes tranquilo, sin intereses de venta ni seguimientos ocultos.

## Propósito

Este documento especifica los requerimientos funcionales y no funcionales del Sistema de Chat Privado, desarrollado con **ASP.NET Web API (.NET 10)** y **MongoDB**. Sirve como contrato técnico entre los actores del sistema, estableciendo qué debe hacer el sistema, bajo qué condiciones y con qué restricciones, siguiendo el estándar **IEEE 830**.

## Alcance

El sistema permite:

- Comunicación en tiempo real entre usuarios mediante **SignalR**
- Cifrado de extremo a extremo (**E2EE**) implementado con **AES-256** por conversación
- Autenticación por **JWT + Refresh Token**
- Almacenamiento en **MongoDB** con identificadores **GUID**
- Arquitectura **Vertical Slice** con organización por feature

**Fuera del alcance:**
- Notificaciones push nativas
- Videollamadas
- Integración OAuth externa

---

<a id="cambios-respecto-a-la-versión-anterior"></a>

# Cambios Respecto a la Versión Anterior

En comparación con la primera versión presentada en el primer corte, el sistema actual ha presentado cambios significativos tanto en su estructura interna como en sus funcionalidades.

## Resumen de Cambios

| Área | Cambio |
|---|---|
| **Base de datos** | Migración para mejorar organización y acceso |
| **Arquitectura** | De capas a Vertical Slice |
| **Mensajería** | Soporte multimedia (documentos, imágenes, videos, audios) |
| **Seguridad** | Cookies seguras, refresh tokens, E2EE con AES-256 |

## Migración de Base de Datos

Se mejoró la organización, almacenamiento y acceso a la información mediante una nueva base de datos, optimizando el rendimiento general del sistema.

## Nueva Arquitectura: Vertical Slice

Se migró de una arquitectura en capas a una **arquitectura Vertical Slice**, la cual permite una mejor separación por funcionalidades, facilitando la escalabilidad y el mantenimiento del sistema.

:::info ¿Qué es Vertical Slice?
Cada funcionalidad agrupa su propia capa de presentación, lógica y acceso a datos en una sola carpeta independiente. Para más detalles, revisa la sección **Arquitectura Vertical Slice** de este mismo documento.
:::

## Mensajería Multimedia

Se añadieron nuevas opciones de mensajería multimedia, permitiendo el envío de:

- 📄 Documentos
- 🖼️ Imágenes
- 🎥 Videos
- 🎵 Audios

Esto amplía significativamente la interacción entre los usuarios más allá del texto plano.

## Seguridad Avanzada

El cambio más importante se dio en el ámbito de la seguridad. Se implementaron:

- **Cookies seguras** para la gestión de sesiones
- **Refresh tokens** para renovación de autenticación
- **Cabeceras de protección** HTTP
- **Bloqueo** ante múltiples peticiones a la API (rate limiting)
- **Cifrado E2EE** con AES-256 por conversación

### Cifrado de Extremo a Extremo (E2EE)

El E2EE garantiza que únicamente el emisor y el receptor puedan acceder al contenido de la conversación. Este modelo utiliza claves criptográficas únicas almacenadas en los dispositivos de los usuarios, ofreciendo:

- Mayor privacidad
- Protección contra accesos no autorizados
- Comunicación mucho más segura

:::tip
Consulta la sección [Cifrado de Extremo a Extremo (E2EE)](#cifrado-de-extremo-a-extremo-e2ee) para ver el detalle técnico del cifrado.
:::

---

<a id="arquitectura-vertical-slice"></a>

# Arquitectura Vertical Slice

La **Arquitectura Vertical Slice** es un enfoque de diseño de software que organiza el código por funcionalidades (_features_) o casos de uso, en lugar de por capas técnicas (presentación, negocio, datos). Agrupa todo lo necesario para una funcionalidad en una sola carpeta, reduciendo dependencias y acoplamiento.

## Concepto

En una arquitectura tradicional por capas, un cambio funcional implica modificar múltiples carpetas: el controlador, el servicio y el repositorio. Con Vertical Slice, **todo lo que corresponde a una funcionalidad vive junto**:

```
📁 Features/
├── 📁 SendMessage/
│   ├── SendMessageCommand.cs
│   ├── SendMessageHandler.cs
│   └── SendMessageEndpoint.cs
├── 📁 GetConversation/
│   ├── GetConversationQuery.cs
│   ├── GetConversationHandler.cs
│   └── GetConversationEndpoint.cs
└── 📁 Auth/
    ├── LoginCommand.cs
    ├── LoginHandler.cs
    └── LoginEndpoint.cs
```

## Comparación con Arquitectura por Capas

| | Arquitectura por Capas | Vertical Slice |
|---|---|---|
| **Organización** | Por tipo técnico (Controllers, Services, Repos) | Por funcionalidad (feature/caso de uso) |
| **Cohesión** | Baja (piezas de una función dispersas) | Alta (todo junto en un slice) |
| **Acoplamiento** | Alto entre capas | Bajo entre slices |
| **Impacto de cambios** | Atraviesa múltiples carpetas | Localizado en un solo slice |
| **Trabajo en equipo** | Conflictos frecuentes | Cada equipo trabaja en su slice |

### Diagrama comparativo

```
Arquitectura por Capas        Vertical Slice
─────────────────────         ──────────────────────────────
  Presentación                  Slice 1 │ Slice 2 │ Slice 3
  ──────────────                ───────   ───────   ───────
  Lógica de negocio             UI      │ UI      │ UI
  ──────────────                Lógica  │ Lógica  │ Lógica
  Acceso a datos                Datos   │ Datos   │ Datos
```

## Ventajas

### Alta Cohesión
Las piezas que cambian juntas permanecen juntas. Navegar el código de una funcionalidad no requiere saltar entre múltiples carpetas; todo está en un mismo slice.

### Bajo Acoplamiento
Las slices se comunican de forma explícita y evitan dependencias innecesarias entre funcionalidades independientes. Modificar una slice no rompe otras.

### Fácil Refactorización
Reordenar o evolucionar una funcionalidad requiere menos movimiento de archivos. Incluso eliminar una característica completa es tan simple como borrar su carpeta.

### Desarrollo Ágil
Los equipos pueden trabajar por dominio o caso de uso, entregando cambios más pequeños y fáciles de validar sin invadir el trabajo de otros.

## Desventajas y Mitigación

### Posible Duplicidad de Código

Al priorizar la independencia de cada funcionalidad, pueden generarse implementaciones similares en distintos slices si no se gestionan adecuadamente los componentes compartidos.

**Cómo mitigarlo:**

- Extraer solo los componentes _realmente_ transversales a una carpeta `Shared/` o `Common/`
- Compartir contratos e interfaces, no lógica de negocio acoplada
- Revisar duplicaciones durante el diseño de nuevas slices
- Mantener convenciones claras para evitar reutilización excesiva

## Implementación en Este Proyecto

En este sistema, cada slice de funcionalidad contiene:

1. **Comando o Query** — Define la intención (qué se quiere hacer)
2. **Handler** — Ejecuta la lógica mediante **Wolverine/MediatR**
3. **Endpoint** — Expone la funcionalidad vía ASP.NET Web API

Este enfoque se complementa directamente con el patrón **CQRS** (Command Query Responsibility Segregation), donde los comandos modifican estado y las queries solo lo leen.

:::info Relación con CQRS
Cada slice implementa ya sea un _Command_ (escritura) o una _Query_ (lectura), manteniendo la separación de responsabilidades dentro de su propio contexto. Consulta la sección **Wolverine - Bus de Mensajes Interno** para ver cómo se despachan estos handlers.
:::

---

<a id="componentes-del-sistema"></a>

# Componentes del Sistema

La arquitectura integra ocho componentes principales que trabajan en conjunto para garantizar comunicación segura, escalable y en tiempo real.

## Mapa de Componentes

| Componente | Rol | Tecnología |
|---|---|---|
| **Web API** | Punto de entrada HTTP | ASP.NET (.NET 10) |
| **ChatHub** | Comunicación en tiempo real | SignalR |
| **Bus de mensajes** | Despacho de comandos/queries | Wolverine (CQRS) |
| **Base de datos** | Persistencia de documentos | MongoDB |
| **Caché y estado** | Datos en memoria de alta velocidad | Redis |
| **Mensajería asíncrona** | Cola de mensajes entre usuarios | RabbitMQ |
| **Archivos multimedia** | Almacenamiento de media | Cloudinary |
| **Cifrado** | Protección de mensajes E2E | AES-256 (E2EE) |

## Flujo del Sistema

El flujo cubre desde la autenticación hasta la entrega cifrada del mensaje en tiempo real, pasando por seis etapas clave:

```
┌────────────────────────────────────────────────────────────┐
│                    Flujo de un Mensaje                     │
└────────────────────────────────────────────────────────────┘

  01 Autenticación     El usuario se autentica con JWT.
         │
         ▼
  02 Conexión RT       Se establece la conexión con SignalR ChatHub.
         │
         ▼
  03 Suscripción       El cliente se une a la conversación.
         │
         ▼
  04 Enviar Mensaje    El usuario redacta y envía el mensaje.
         │
         ▼
  05 Procesamiento     Wolverine despacha el handler:
                       cifrado AES-256 → persistencia en MongoDB
                       → publicación en RabbitMQ.
         │
         ▼
  06 Distribución      SignalR entrega el mensaje cifrado
                       al receptor suscrito en tiempo real.
```

## Interacción entre Componentes

```
Cliente A                    Servidor                    Cliente B
   │                            │                            │
   │── JWT Auth ──────────────► │                            │
   │◄─ Token ───────────────── │                            │
   │                            │                            │
   │── SignalR Connect ────────► │                            │
   │── JoinConversation ───────► │                            │
   │                            │                            │
   │── SendMessage ────────────► │                            │
   │                       Wolverine Handler                  │
   │                       AES-256 Encrypt                    │
   │                       MongoDB Save                       │
   │                       RabbitMQ Publish                   │
   │                            │ ──► SignalR Push ──────────► │
   │                            │                    Decrypt  │
```

## Secciones de Componentes

En esta misma página se documentan en detalle los componentes clave:

- MongoDB — Base de datos NoSQL de documentos
- Redis — Caché en memoria de alta velocidad
- RabbitMQ — Mensajería asíncrona
- Wolverine — Bus de mensajes interno y CQRS
- E2EE — Cifrado de extremo a extremo con AES-256

---

<a id="mongodb---base-de-datos-nosql"></a>

# MongoDB - Base de Datos NoSQL

MongoDB es una base de datos NoSQL de documentos, de código abierto y alto rendimiento, diseñada para la escalabilidad, flexibilidad y el desarrollo de aplicaciones modernas. En lugar de tablas y filas tradicionales, utiliza un modelo orientado a documentos en formato **BSON** (similar a JSON) con esquemas dinámicos.

## Características Clave

| Característica | Descripción |
|---|---|
| **Modelo de documentos** | Almacena datos en documentos flexibles; diferentes registros pueden tener campos distintos |
| **NoSQL / No Relacional** | Ideal para datos no estructurados o semiestructurados |
| **Alta escalabilidad** | Maneja grandes volúmenes de datos mediante _sharding_ y replicación |
| **Desarrollo ágil** | Integración de nuevos tipos de datos sin redefinir el esquema |

## Colecciones en Este Proyecto

El sistema gestiona las siguientes colecciones en MongoDB:

```
📦 chat-privado (database)
├── 📄 users            — Información de usuarios registrados
├── 📄 conversations    — Conversaciones y metadatos
├── 📄 messages         — Mensajes cifrados (contenido AES-256)
├── 📄 contacts         — Relaciones entre usuarios
└── 📄 refreshTokens    — Tokens de renovación de sesión
```

## Estructura de un Documento de Mensaje

```json
{
  "_id": "guid-unico",
  "conversationId": "guid-conversacion",
  "senderId": "guid-emisor",
  "content": "<contenido cifrado AES-256>",
  "type": "text | image | document | video | audio",
  "mediaUrl": "https://cloudinary.com/...",
  "createdAt": "2026-06-01T10:30:00Z",
  "read": false
}
```

:::info Cifrado en reposo
El campo `content` siempre se almacena **cifrado**. MongoDB nunca persiste el texto plano del mensaje. Consulta [Cifrado de Extremo a Extremo (E2EE)](#cifrado-de-extremo-a-extremo-e2ee) para el detalle del proceso de cifrado.
:::

## Por qué MongoDB en Este Sistema

- **Mensajes variables:** Los mensajes pueden ser texto, imágenes, videos o documentos — esquemas flexibles los manejan de forma natural.
- **Alta concurrencia:** Múltiples usuarios enviando mensajes simultáneamente sin cuellos de botella.
- **Escalabilidad horizontal:** El _sharding_ permite distribuir la carga a medida que crece la base de usuarios.
- **Integración con Redis:** MongoDB actúa como almacenamiento persistente, mientras Redis gestiona los datos en memoria de alta velocidad (estado de conexión, colas recientes).

---

<a id="redis---caché-en-memoria"></a>

# Redis - Caché en Memoria

Redis actúa como almacén de datos en memoria de alta velocidad, operando bajo un modelo **clave-valor** que facilita el acceso inmediato a información transitoria requerida por el sistema.

## Rol en el Sistema

Redis no reemplaza a MongoDB. Actúa como una capa de caché que **reduce la dependencia directa** de la base de datos principal, minimizando la latencia en operaciones frecuentes y críticas para la experiencia en tiempo real.

```
                    ┌─────────────┐
  Request ─────────► Redis (caché) ├── HIT  ──► Respuesta rápida
                    └──────┬──────┘
                           │ MISS
                           ▼
                    ┌─────────────┐
                    │   MongoDB   ├──────────► Respuesta + actualiza caché
                    └─────────────┘
```

## Datos Gestionados por Redis

| Tipo de dato | Descripción |
|---|---|
| **Estado de usuarios** | Persiste qué usuarios están conectados en tiempo real |
| **Tokens de sesión** | Almacena temporalmente los tokens activos para validación rápida |
| **Colas de mensajes recientes** | Acelera la entrega de los últimos mensajes de una conversación |
| **Notificaciones** | Gestiona notificaciones temporales pendientes de entrega |
| **Variables de comunicación** | Datos frecuentes del sistema de mensajería |

## Beneficios en Este Proyecto

### Baja Latencia
Al acceder a datos directamente desde memoria, las consultas frecuentes (como verificar si un usuario está en línea) se resuelven en microsegundos en lugar de milisegundos.

### Entrega Rápida de Mensajes
Las colas de mensajes recientes en Redis permiten que SignalR entregue el contenido sin esperar una consulta completa a MongoDB.

### Gestión de Usuarios en Línea
El estado `online/offline` de cada usuario se persiste en Redis, permitiendo que el sistema sepa en tiempo real quién puede recibir mensajes por SignalR.

### Soporte para Tiempo Real
Redis soporta los procesos de comunicación en tiempo real con mayor eficiencia y concurrencia, complementando directamente el ChatHub de SignalR.

## Configuración Típica

```csharp
// Registro en Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "ChatPrivado:";
});
```

:::caution Datos transitorios
Redis almacena información **transitoria**. Los mensajes y datos permanentes siempre se persisten en MongoDB. Redis solo acelera el acceso a datos de vida corta.
:::

---

<a id="rabbitmq---mensajería-asíncrona"></a>

# RabbitMQ - Mensajería Asíncrona

RabbitMQ actúa como **broker intermediario** para la gestión del envío y recepción de mensajes entre usuarios, eliminando la dependencia directa e inmediata entre emisor y receptor.

## Flujo de Mensajería

```
  Usuario 1                RabbitMQ               Usuario 2
     │                        │                       │
     │── Envía mensaje ──────► │                       │
     │                   Cola de mensajes              │
     │                   (persiste el msg)             │
     │                        │                       │
     │                        │── Entrega mensaje ───► │
     │                        │                       │
     │                        │◄── Acuse de recibo ── │
```

Cuando el **Usuario 1** envía un mensaje, este es almacenado temporalmente en una cola creada por el broker. La información queda en espera hasta que el **Usuario 2** la consume. El receptor puede generar una respuesta sin que exista una dependencia directa e inmediata entre ambos.

## Beneficios

### Desacoplamiento
No existe dependencia directa entre emisor y receptor. Si el Usuario 2 está temporalmente desconectado, el mensaje permanece en la cola hasta que pueda consumirlo.

### Confiabilidad
Los mensajes no se pierden. RabbitMQ persiste los mensajes en la cola, garantizando la entrega incluso ante fallos temporales en alguno de los servicios.

### Escalabilidad
La comunicación asíncrona permite que múltiples usuarios envíen y reciban mensajes de forma concurrente sin saturar el sistema. Se pueden agregar más consumidores (workers) sin modificar el productor.

## Comparación: Síncrono vs Asíncrono

| | Sin RabbitMQ (síncrono) | Con RabbitMQ (asíncrono) |
|---|---|---|
| **Dependencia** | Receptor debe estar disponible | Receptor puede estar offline |
| **Pérdida de mensajes** | Posible si el receptor falla | No — persiste en cola |
| **Escalabilidad** | Limitada | Horizontal mediante workers |
| **Latencia** | Bloqueante | No bloqueante |

## Configuración en el Proyecto

```csharp
// Registro de RabbitMQ con Wolverine
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.Host("rabbitmq://localhost");
    })
    .AutoProvisionQueues();
    
    opts.PublishMessage<SendMessageCommand>()
        .ToRabbitQueue("messages-queue");
});
```

:::tip Integración con Wolverine
En este sistema, RabbitMQ se integra con **Wolverine** como transporte de mensajes. Los comandos publicados por Wolverine son encolados en RabbitMQ y consumidos por los handlers correspondientes. Consulta la sección **Wolverine - Bus de Mensajes Interno** para el detalle.
:::

---

<a id="wolverine---bus-de-mensajes-interno"></a>

# Wolverine - Bus de Mensajes Interno

En este proyecto se utilizó **WolverineFx** como herramienta de mediación y mensajería interna para gestionar el flujo de comandos y consultas dentro de la aplicación.

## ¿Qué es Wolverine?

Wolverine es un framework de mensajería y mediación para .NET que implementa el patrón **CQRS** (Command Query Responsibility Segregation). Permite desacoplar la lógica de negocio de los controladores, ya que cada solicitud es procesada por **handlers especializados** encargados de ejecutar una funcionalidad específica.

## Rol en la Arquitectura

```
  HTTP Request
       │
       ▼
  ASP.NET Controller / Endpoint
       │
       │  IMessageBus.SendAsync(command)
       ▼
  ┌──────────────────────────────────┐
  │          Wolverine Pipeline      │
  │  ┌────────────┐                  │
  │  │ Validation │ FluentValidation  │
  │  └─────┬──────┘                  │
  │        ▼                         │
  │  ┌────────────┐                  │
  │  │  Handler   │ Lógica de negocio │
  │  └─────┬──────┘                  │
  │        ▼                         │
  │  ┌────────────┐                  │
  │  │  MongoDB   │ Persistencia      │
  │  └────────────┘                  │
  └──────────────────────────────────┘
```

## Patrón CQRS con Wolverine

Wolverine distingue entre dos tipos de mensajes:

### Commands (Escritura)
Modifican el estado del sistema. No retornan datos directamente.

```csharp
// Comando
public record SendMessageCommand(
    Guid ConversationId,
    Guid SenderId,
    string Content,
    MessageType Type
);

// Handler
public class SendMessageHandler
{
    public async Task Handle(
        SendMessageCommand cmd,
        IMongoCollection<Message> messages,
        IEncryptionService encryption)
    {
        var encrypted = encryption.Encrypt(cmd.Content, cmd.ConversationId);
        await messages.InsertOneAsync(new Message { ... });
    }
}
```

### Queries (Lectura)
Obtienen datos sin modificar el estado.

```csharp
// Query
public record GetConversationQuery(Guid ConversationId, int Page);

// Handler
public class GetConversationHandler
{
    public async Task<ConversationDto> Handle(
        GetConversationQuery query,
        IMongoCollection<Message> messages)
    {
        // Solo lectura, sin efectos secundarios
        return await messages.Find(...).ToListAsync();
    }
}
```

## Procesos Coordinados por Wolverine

Wolverine coordina de forma eficiente los siguientes procesos internos:

| Proceso | Descripción |
|---|---|
| **Autenticación** | Login, generación de JWT y refresh tokens |
| **Cifrado** | Invocación del `E2EEncryptionService` antes de persistir |
| **Gestión de mensajes** | Validación, encolado en RabbitMQ y notificación por SignalR |
| **Almacenamiento** | Escritura y lectura en MongoDB |

## Beneficios en Este Proyecto

- **Organización del código:** Cada caso de uso tiene su propio handler, ubicado en su slice correspondiente
- **Testabilidad:** Los handlers son clases simples, fáciles de probar unitariamente
- **Escalabilidad:** Wolverine puede procesar mensajes en paralelo y distribuirlos vía RabbitMQ
- **Integración con Vertical Slice:** El patrón handler-por-feature se alinea perfectamente con la arquitectura adoptada

:::info FluentValidation
Wolverine integra **FluentValidation** en su pipeline. Cada comando puede tener un `Validator<TCommand>` que se ejecuta automáticamente antes del handler, rechazando solicitudes inválidas sin llegar a la lógica de negocio.
:::

---

# Cifrado de Extremo a Extremo (E2EE)

El cifrado de extremo a extremo (E2EE) es un mecanismo de seguridad que garantiza que la información intercambiada entre usuarios **solo pueda ser leída por el emisor y el receptor**, evitando el acceso por parte de terceros, incluidos los servidores intermedios.

## Algoritmo

El sistema utiliza **AES-256** (Advanced Encryption Standard con clave de 256 bits), implementado a través del servicio `E2EEncryptionService`. Cada conversación tiene su propia clave criptográfica, garantizando aislamiento entre chats.

## Flujo de Cifrado

```
  Emisor                     Backend                    Receptor
    │                           │                          │
    │  1. Redacta mensaje        │                          │
    │──────────────────────────► │                          │
    │                           │                          │
    │            2. E2EEncryptionService                    │
    │               AES-256(mensaje, claveConversación)     │
    │                           │                          │
    │            3. Mensaje cifrado → MongoDB               │
    │                           │                          │
    │            4. SignalR push (cifrado)                  │
    │                           │─────────────────────────► │
    │                           │                          │
    │                           │    5. Descifrado cliente  │
    │                           │       Visualiza texto     │
```

### Etapas Detalladas

1. **Emisor escribe** — El usuario redacta el mensaje en el cliente.
2. **Cifrado AES-256** — El backend cifra el mensaje con la clave única de la conversación antes de persistirlo.
3. **Transmisión segura** — El mensaje viaja cifrado por SignalR/HTTPS y se almacena cifrado en MongoDB.
4. **Descifrado en cliente** — Solo el receptor autorizado descifra y visualiza el contenido.

## Implementación

```csharp
public interface IE2EEncryptionService
{
    string Encrypt(string plainText, Guid conversationId);
    string Decrypt(string cipherText, Guid conversationId);
}

public class E2EEncryptionService : IE2EEncryptionService
{
    private readonly IKeyStore _keyStore;

    public string Encrypt(string plainText, Guid conversationId)
    {
        var key = _keyStore.GetKey(conversationId); // Clave única por conversación
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        
        // Cifrar y retornar IV + CipherText en Base64
        var encrypted = aes.EncryptCbc(
            Encoding.UTF8.GetBytes(plainText), aes.IV);
        
        return Convert.ToBase64String(aes.IV.Concat(encrypted).ToArray());
    }

    public string Decrypt(string cipherText, Guid conversationId)
    {
        var key = _keyStore.GetKey(conversationId);
        var data = Convert.FromBase64String(cipherText);
        
        var iv = data[..16];
        var cipher = data[16..];
        
        using var aes = Aes.Create();
        aes.Key = key;
        
        return Encoding.UTF8.GetString(aes.DecryptCbc(cipher, iv));
    }
}
```

## Garantías de Seguridad

| Escenario | Protección |
|---|---|
| **Intercepción en tránsito** | El mensaje viaja cifrado por HTTPS + AES-256 |
| **Acceso a la base de datos** | MongoDB almacena solo texto cifrado, nunca en claro |
| **Servidor comprometido** | Sin las claves del cliente, el contenido es ilegible |
| **Múltiples conversaciones** | Claves independientes por conversación |

## Complemento: Otras Medidas de Seguridad

El E2EE es parte de una estrategia de seguridad más amplia:

| Mecanismo | Función |
|---|---|
| **JWT + Refresh Token** | Autenticación y renovación segura de sesiones |
| **Cookies seguras** | Almacenamiento seguro del token en el cliente |
| **Cabeceras HTTP** | Protección contra XSS, Clickjacking y otros ataques |
| **Rate limiting** | Bloqueo ante múltiples peticiones sospechosas a la API |

:::danger Nunca texto plano
El campo `content` de los documentos en MongoDB **siempre** contiene el texto cifrado. Ningún proceso del backend debe persistir o loguear mensajes en texto plano.
:::