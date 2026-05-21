---
layout: default
title: Inicio
nav_order: 1
---

# Uni Chat Backend

API de chat en tiempo real con **ASP.NET Core 10**: JWT, MongoDB, Redis, RabbitMQ, SignalR y Cloudinary.

## Empieza aquí

| Guía | Descripción |
|------|-------------|
| [Instalación]({{ site.baseurl }}/instalacion.html) | Paso a paso para desarrolladores nuevos |
| [Backend]({{ site.baseurl }}/backend.html) | Arquitectura, endpoints y SignalR |
| [Chat privado / E2E]({{ site.baseurl }}/chat-privado.html) | Sistema de mensajería cifrada |
| [CI/CD y secrets]({{ site.baseurl }}/github-secrets.html) | GitHub Actions y variables de producción |

## Inicio rápido

Desde la carpeta del proyecto .NET (`uni-chat-backend/uni-chat-backend/`):

```bash
cd uni-chat-backend
cp .env.ci.example .env
make up && make run
```

API local: **http://localhost:5012**

Documentación interactiva de la API (con la API en ejecución): OpenAPI + Scalar en el mismo host.
