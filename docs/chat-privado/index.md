---
layout: default
title: Chat privado / E2E
nav_order: 3
has_children: true
permalink: /chat-privado/
---

# Sistema de Chat Privado

**Autores:** Julián Andrés Caracas Sánchez · Kevin Villegas · Santiago Méndez · José David Zuluaga

**Institución:** Corporación de Estudios Tecnológicos del Norte del Valle — Unidad de Ingeniería, Arquitectura, Urbanismo y Afines · Cartago Valle · Junio 2026

---

## Guías en esta sección

| Página | Contenido |
|--------|-----------|
| [Arquitectura]({{ site.baseurl }}/chat-privado/arquitectura.html) | Vertical Slice, CQRS, MediatR y organización por features |
| [Componentes]({{ site.baseurl }}/chat-privado/componentes.html) | MongoDB, Redis, RabbitMQ, Wolverine, flujos del sistema |
| [E2EE]({{ site.baseurl }}/chat-privado/e2ee.html) | Cifrado AES-256 por conversación e implementación real |

Para instalar y ejecutar el backend, ve a [Instalación]({{ site.baseurl }}/instalacion.html). Para endpoints HTTP y SignalR, [Backend]({{ site.baseurl }}/backend.html).

---

## Introducción

El desarrollo de muchas aplicaciones se basa en utilizar la información de las personas con fines comerciales. Como consecuencia, quien las usa pierde el control sobre sus datos y contactos. Por este motivo, el presente sistema nace inspirado en la filosofía de Brave, enfocado en proteger la privacidad.

La herramienta pide solo lo estrictamente necesario para funcionar de forma segura: un espacio digital sin rastreadores ni venta de información a terceros.

## Justificación

Muchas aplicaciones intercambian el uso de la herramienta por datos personales para publicidad. Este sistema es una alternativa donde la información no es moneda de cambio: sin rastreadores ocultos ni perfiles vendidos a terceros.

## Planteamiento del problema

Las herramientas digitales suelen exigir datos personales que terminan en perfiles comerciales o se comparten con terceros. Surge la necesidad de un sistema que no trate a la persona como fuente de datos y que garantice un intercambio de mensajes seguro.

## Propósito

Este documento especifica requerimientos funcionales y no funcionales del Sistema de Chat Privado, desarrollado con **ASP.NET Core (.NET 10)** y **MongoDB**, como referencia técnica del proyecto (estándar IEEE 830).

## Alcance

El sistema permite:

- Comunicación en tiempo real mediante **SignalR**
- Cifrado por conversación (**E2EE**) con **AES-256**
- Autenticación **JWT + Refresh Token**
- Persistencia en **MongoDB** con identificadores **GUID**
- Arquitectura **Vertical Slice** por feature

**Fuera del alcance:**

- Notificaciones push nativas
- Videollamadas
- Integración OAuth externa

---

## Cambios respecto a la versión anterior

En comparación con la primera versión del primer corte, el sistema actual incorpora cambios en estructura y funcionalidades.

### Resumen de cambios

| Área | Cambio |
|------|--------|
| Base de datos | Mejor organización y acceso (MongoDB) |
| Arquitectura | De capas a Vertical Slice |
| Mensajería | Soporte multimedia (documentos, imágenes, videos, audios) |
| Seguridad | Refresh tokens, rate limiting, E2EE con AES-256 |

### Nueva arquitectura: Vertical Slice

Se migró de capas horizontales a **Vertical Slice** para separar por funcionalidad y facilitar mantenimiento. Detalle en [Arquitectura]({{ site.baseurl }}/chat-privado/arquitectura.html).

### Mensajería multimedia

Envío de documentos, imágenes, videos y audios además de texto plano (vía HTTP upload y SignalR).

### Seguridad avanzada

- Refresh tokens para renovación de sesión
- Cabeceras de protección HTTP
- Rate limiting ante abuso de la API
- **E2EE** con AES-256 por conversación — ver [E2EE]({{ site.baseurl }}/chat-privado/e2ee.html)
