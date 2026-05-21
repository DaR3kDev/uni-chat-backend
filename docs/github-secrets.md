# GitHub Secrets y generación de `.env`

> **Primera instalación en tu máquina:** empieza por [GUIA-INSTALACION.md](GUIA-INSTALACION.md) (clonar, Docker, `.env` local y arrancar la API). Este documento es para **producción y GitHub Actions**.

Los secretos **nunca** se guardan en el repositorio. Se configuran en GitHub y se inyectan como variables de entorno en runners o servidores de deploy.

## Dónde crear los secrets

1. Repositorio en GitHub → **Settings** → **Secrets and variables** → **Actions**
2. **New repository secret** por cada fila de la tabla (mismo nombre que la variable)
3. Para entornos distintos (staging/producción), usar **Environments** con secrets por entorno

## Tabla: Secret de GitHub → variable `.env`

| Secret (GitHub) | Variable en `.env` | Obligatorio | Uso |
|-----------------|-------------------|-------------|-----|
| `MONGO_ROOT_USERNAME` | `MONGO_ROOT_USERNAME` | Sí | MongoDB en Docker |
| `MONGO_ROOT_PASSWORD` | `MONGO_ROOT_PASSWORD` | Sí | MongoDB en Docker |
| `MONGO_CONNECTION_STRING` | `MONGO_CONNECTION_STRING` | Sí | Cadena para la API en contenedor |
| `MONGO_DATABASE` | `MONGO_DATABASE` | Sí | Nombre de base de datos |
| `REDIS_CONNECTION_STRING` | `REDIS_CONNECTION_STRING` | Sí | Redis |
| `RABBITMQ_USER` | `RABBITMQ_USER` | Sí | RabbitMQ |
| `RABBITMQ_PASS` | `RABBITMQ_PASS` | Sí | RabbitMQ |
| `RABBITMQ_CONNECTION_STRING` | `RABBITMQ_CONNECTION_STRING` | Sí | AMQP para la API |
| `JWT_KEY` | `JWT_KEY` | Sí | Clave JWT (mín. 32 caracteres en producción) |
| `JWT_ISSUER` | `JWT_ISSUER` | Sí | Emisor del token |
| `JWT_AUDIENCE` | `JWT_AUDIENCE` | Sí | Audiencia del token |
| `JWT_EXPIRE_MINUTES` | `JWT_EXPIRE_MINUTES` | Sí | Expiración access token |
| `REFRESH_EXPIRE_DAYS` | `REFRESH_EXPIRE_DAYS` | Sí | Expiración refresh token |
| `CLOUDINARY_CLOUD_NAME` | `CLOUDINARY_CLOUD_NAME` | Sí | Cloudinary |
| `CLOUDINARY_API_KEY` | `CLOUDINARY_API_KEY` | Sí | Cloudinary |
| `CLOUDINARY_API_SECRET` | `CLOUDINARY_API_SECRET` | Sí | Cloudinary |
| `ENABLE_DOCS` | `ENABLE_DOCS` | No | Swagger/OpenAPI en contenedor |
| `MONGO_EXPRESS_USER` | `MONGO_EXPRESS_USER` | No | Panel mongo-express |
| `MONGO_EXPRESS_PASS` | `MONGO_EXPRESS_PASS` | No | Panel mongo-express |
| `SEQ_ADMIN` | `SEQ_ADMIN` | No | Seq (logs) |
| `SEQ_ADMIN_PASSWORD` | `SEQ_ADMIN_PASSWORD` | No | Seq (logs) |

Los nombres del secret y de la variable `.env` coinciden a propósito para simplificar el deploy.

## Generar `.env` en un runner o servidor

Script: [`scripts/render-env.sh`](../scripts/render-env.sh)

Desde la raíz del repo, con variables ya exportadas (p. ej. inyectadas por GitHub Actions):

```yaml
# Ejemplo futuro — job deploy
env:
  JWT_KEY: ${{ secrets.JWT_KEY }}
  MONGO_CONNECTION_STRING: ${{ secrets.MONGO_CONNECTION_STRING }}
  # ... resto de secrets con el mismo nombre
steps:
  - run: ./scripts/render-env.sh
  - run: cd uni-chat-backend && docker compose up -d
```

Solo validar que existen todas las obligatorias (sin escribir archivo):

```bash
./scripts/render-env.sh --check
```

Desde `uni-chat-backend/`:

```bash
make env-from-ci
```

## Desarrollo local sin GitHub

- Plantilla vacía: [`.env.example`](../uni-chat-backend/.env.example) → `make setup`
- Valores dummy: [`.env.ci.example`](../uni-chat-backend/.env.ci.example) → `cp .env.ci.example .env` (no usar en producción)

## CI actual (backend)

El workflow [`.github/workflows/ci-cd-backend.yml`](../.github/workflows/ci-cd-backend.yml) **no requiere** estos secrets: compila y ejecuta tests sin levantar Mongo/Redis/Rabbit.

## Pre-commit

Tras `make install-hooks`, un `git commit` rechaza archivos `.env` en staging y ejecuta `make pre-commit` (lint + build + test).
