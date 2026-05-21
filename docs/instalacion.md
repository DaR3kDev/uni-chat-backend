---
layout: default
title: Instalación
nav_order: 2
---

# Guía de instalación (paso a paso)

Esta guía está pensada para **quien nunca ha trabajado con el proyecto**. Si solo quieres un resumen técnico, ve al [README del repositorio](https://github.com/DaR3kDev/uni-chat-backend/blob/main/README.md).

Al terminar tendrás:

- MongoDB, Redis y RabbitMQ en Docker
- La API corriendo en `http://localhost:5012`
- (Opcional) Hooks de Git y tests locales configurados

---

## Qué necesitas instalar

| Herramienta | Obligatorio | Para qué |
|-------------|-------------|----------|
| [Git](https://git-scm.com/) | Sí | Clonar el repositorio |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) o Docker Engine + Compose | Sí | Base de datos, Redis, RabbitMQ |
| [.NET SDK 10](https://dotnet.microsoft.com/download) | Sí (si ejecutas la API con `make run`) | Compilar y ejecutar la API en tu PC |
| [GNU Make](https://www.gnu.org/software/make/) | Sí en Linux/WSL | Comandos `make up`, `make run`, etc. |

**Windows:** usa **WSL2 con Ubuntu**. El Makefile no funciona bien en PowerShell sin WSL.

---

## Checklist antes de empezar

Marca mentalmente cada punto:

- [ ] Tienes Git instalado
- [ ] Docker está **encendido** (`docker ps` no da error)
- [ ] Tienes .NET 10 (`dotnet --version` muestra `10.x`)
- [ ] Tienes Make (`make --version`)
- [ ] Los puertos 27017, 6379, 5672 y 5012 están libres (o sabes qué proceso los usa)

---

## Paso 1 — Clonar el repositorio

```bash
git clone <URL-de-tu-repositorio>
cd uni-chat-backend/uni-chat-backend
```

**¿Por qué `uni-chat-backend/uni-chat-backend`?**

- La **raíz del repo** contiene el README, `docs/`, `.github/` y scripts.
- La carpeta **interior** `uni-chat-backend/` es donde viven el código .NET, el `Makefile` y `docker-compose.yml`.
- **Todos los comandos `make` se ejecutan desde la carpeta interior.**

Comprueba que estás en el sitio correcto:

```bash
pwd
# Debe terminar en .../uni-chat-backend/uni-chat-backend
ls Makefile docker-compose.yml
```

---

## Paso 2 — Instalar herramientas (WSL / Ubuntu)

### .NET 10

Opción A — instalador oficial desde [dotnet.microsoft.com](https://dotnet.microsoft.com/download).

Opción B — script en WSL:

```bash
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0
echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
source ~/.bashrc
dotnet --version
```

### Docker

Instala [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) o Docker Engine en Linux. Verifica:

```bash
docker compose version
docker ps
```

### Make

```bash
sudo apt update
sudo apt install -y build-essential
make --version
```

---

## Paso 3 — Configurar variables locales

Hay **dos archivos** según cómo ejecutes la API:

| Archivo | Cuándo |
|---------|--------|
| `.env` | Docker Compose (`make up`, `make docker-up`) |
| `appsettings.json` | API en tu máquina (`make run`) |

### Opción A — Rápida para desarrollo (recomendada)

Usa valores de prueba ya preparados:

```bash
cp .env.ci.example .env
```

Edita `.env` solo si vas a usar **Cloudinary** de verdad (subida de archivos). Si no, puedes dejar los placeholders y probar el resto de la API.

Alinea **`appsettings.json`** para que la API en el host use `localhost` (servicios Docker):

```json
{
  "Mongo": {
    "ConnectionString": "mongodb://admin:admin123@localhost:27017/?authSource=admin",
    "Database": "unichat"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "RabbitMQ": {
    "ConnectionString": "amqp://guest:guest@localhost:5672"
  },
  "Jwt": {
    "Key": "ci-only-fake-jwt-key-minimum-32-characters-long",
    "Issuer": "uni-chat",
    "Audience": "uni-chat-users",
    "ExpireMinutes": 15
  },
  "RefreshToken": {
    "ExpireDays": 7
  },
  "Cloudinary": {
    "CloudName": "tu-cloud-name",
    "ApiKey": "tu-api-key",
    "ApiSecret": "tu-api-secret"
  }
}
```

Los valores de Mongo deben coincidir con `MONGO_ROOT_*` de tu `.env`.

### Opción B — Plantilla vacía

```bash
make setup
```

Abre `.env` y rellena cada variable siguiendo los comentarios de [`.env.example`](https://github.com/DaR3kDev/uni-chat-backend/blob/main/uni-chat-backend/.env.example).

### Archivos que NUNCA debes subir a Git

- `.env` (contiene contraseñas)
- Claves JWT o secrets de Cloudinary en commits

El hook `pre-commit` bloquea commitear `.env` si lo añades por error.

---

## Paso 4 — Levantar servicios (Docker)

Desde `uni-chat-backend/`:

```bash
make up
make ps
```

Si algo falla:

```bash
make logs
make down
make up
```

### URLs útiles (con `make up`)

| Servicio | URL / puerto |
|----------|----------------|
| MongoDB | `localhost:27017` |
| Redis | `localhost:6379` |
| RabbitMQ | `localhost:5672` |
| RabbitMQ UI | http://localhost:15672 |
| mongo-express | http://localhost:8081 |
| RedisInsight | http://localhost:5540 |
| Seq (logs) | http://localhost:5341 |
| Dozzle | http://localhost:8083 |
| Portainer | http://localhost:9000 |

Ver tabla visual:

```bash
make ports
```

---

## Paso 5 — Arrancar la API

### Modo habitual (API en tu PC, infra en Docker)

```bash
make run
```

Abre en el navegador: **http://localhost:5012**

- Documentación OpenAPI/Scalar: disponible en entorno `Development`
- SignalR (chat en tiempo real): hub `/messages/chat` (requiere JWT)

### Atajo todo-en-uno

Si aún no tienes `.env`:

```bash
make dev
```

Equivale a `setup` + `up` + `run` (la terminal queda ocupada con la API; Ctrl+C para parar).

### Modo alternativo — API también en Docker

```bash
make docker-up
```

API en **http://localhost:8080**. En este modo la API lee variables de `.env` con hosts `mongodb`, `redis`, `rabbitmq` (no `localhost`).

---

## Paso 6 — Hooks de Git y calidad de código

Una vez por clon del repo:

```bash
make install-hooks
```

Qué hace:

- En cada **`git commit`**: ejecuta `make pre-commit` (lint + build + test) y **rechaza** commits que incluyan `.env`.
- Antes de **`git push`**, conviene ejecutar el pipeline completo:

```bash
make ci
```

| Comando | Qué incluye |
|---------|-------------|
| `make pre-commit` | lint + build + test |
| `make ci` | restore + lint + build + test (igual que GitHub Actions) |

Si `make lint` falla:

```bash
make lint-fix
git add -A
git commit -m "tu mensaje"
```

---

## Paso 7 — Documentación del proyecto (opcional)

La documentación se publica en **GitHub Pages** (Jekyll): [https://dar3kdev.github.io/uni-chat-backend/](https://dar3kdev.github.io/uni-chat-backend/)

Para previsualizar en local (requiere Ruby y Bundler):

```bash
make docs-install
make docs
```

También puedes editar los archivos `.md` en la carpeta `docs/` del repositorio; GitHub reconstruye el sitio al hacer push a `main`.

---

## Producción con imagen Docker (GHCR)

En cada push a `main`, GitHub publica una imagen en **GitHub Container Registry**:

```
ghcr.io/dar3kdev/uni-chat-backend:latest
ghcr.io/dar3kdev/uni-chat-backend:main
ghcr.io/dar3kdev/uni-chat-backend:<sha-del-commit>
```

### Descargar y usar la imagen

1. Crea `.env` con variables de producción (o usa `scripts/render-env.sh` con secrets; ver [CI/CD y secrets](github-secrets.html)).

2. Ejemplo de `docker-compose.prod.yml` (en tu servidor):

```yaml
services:
  mongodb:
    image: mongo:latest
    container_name: mongodb
    restart: unless-stopped
    environment:
      MONGO_INITDB_ROOT_USERNAME: ${MONGO_ROOT_USERNAME}
      MONGO_INITDB_ROOT_PASSWORD: ${MONGO_ROOT_PASSWORD}
    ports:
      - "27017:27017"
    volumes:
      - mongodb_data:/data/db

  redis:
    image: redis:7-alpine
    container_name: redis
    restart: unless-stopped
    ports:
      - "6379:6379"

  rabbitmq:
    image: rabbitmq:3-management
    container_name: rabbitmq
    restart: unless-stopped
    environment:
      RABBITMQ_DEFAULT_USER: ${RABBITMQ_USER}
      RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASS}
    ports:
      - "5672:5672"
      - "15672:15672"

  api:
    image: ghcr.io/dar3kdev/uni-chat-backend:latest
    container_name: uni-chat-api
    restart: unless-stopped
    environment:
      Mongo__ConnectionString: ${MONGO_CONNECTION_STRING}
      Mongo__Database: ${MONGO_DATABASE}
      Redis__ConnectionString: ${REDIS_CONNECTION_STRING}
      RabbitMQ__ConnectionString: ${RABBITMQ_CONNECTION_STRING}
      Jwt__Key: ${JWT_KEY}
      Jwt__Issuer: ${JWT_ISSUER}
      Jwt__Audience: ${JWT_AUDIENCE}
      Jwt__ExpireMinutes: ${JWT_EXPIRE_MINUTES}
      RefreshToken__ExpireDays: ${REFRESH_EXPIRE_DAYS}
      Cloudinary__CloudName: ${CLOUDINARY_CLOUD_NAME}
      Cloudinary__ApiKey: ${CLOUDINARY_API_KEY}
      Cloudinary__ApiSecret: ${CLOUDINARY_API_SECRET}
    ports:
      - "8080:8080"
    depends_on:
      - mongodb
      - redis
      - rabbitmq

volumes:
  mongodb_data:
```

3. Ejecutar:

```bash
docker compose -f docker-compose.prod.yml up -d
```

Imagen privada en GHCR:

```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u TU_USUARIO --password-stdin
```

---

## Errores frecuentes

| Problema | Solución |
|----------|----------|
| `dotnet: command not found` | Instala .NET 10 y `export PATH="$HOME/.dotnet:$PATH"` en `~/.bashrc` |
| `make: command not found` | En Windows usa WSL; en Ubuntu: `sudo apt install build-essential` |
| `docker: Cannot connect to daemon` | Abre Docker Desktop o inicia el servicio `docker` |
| Puerto en uso | `make down` o `docker compose down`; revisa `make ps` |
| API no conecta a Mongo | Con `make run`, `appsettings.json` debe usar `localhost`. Con `docker-up`, `.env` debe usar `mongodb`, `redis`, `rabbitmq` |
| `make lint` falla en commit | `make lint-fix` y vuelve a commitear |
| `Syntax error: "(" unexpected` en make | Actualiza el repo; no uses paréntesis rotos en mensajes del Makefile |
| Commit rechazado por `.env` | Quita `.env` del staging: `git reset HEAD .env` |
| `make ci` lento | Normal la primera vez; usa cache de NuGet después |

Más detalle en el [README — Solución de problemas](https://github.com/DaR3kDev/uni-chat-backend/blob/main/README.md#solución-de-problemas).

---

## Siguiente lectura

| Tema | Enlace |
|------|--------|
| Resumen del repo y Makefile | [README](https://github.com/DaR3kDev/uni-chat-backend/blob/main/README.md) |
| GitHub Secrets y deploy | [github-secrets](github-secrets.html) |
| Endpoints y arquitectura API | [backend](backend.html) |
| Chat privado / E2E | [chat-privado](chat-privado.html) |
| CI en GitHub | [README — CI/CD](https://github.com/DaR3kDev/uni-chat-backend/blob/main/README.md#cicd-y-calidad) |
