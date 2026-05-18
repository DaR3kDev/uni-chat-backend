# uni-chat-backend

API de chat en tiempo real con **ASP.NET Core 10**, autenticación JWT, mensajería asíncrona (RabbitMQ), caché (Redis), persistencia (MongoDB), tiempo real (SignalR) y almacenamiento de archivos (Cloudinary).

Documentación extendida (endpoints, arquitectura): carpeta [`docs/`](docs/) — ver [`docs/docs/backend.mdx`](docs/docs/backend.mdx).

---

## Estructura del repositorio

```
uni-chat-backend/              ← raíz del repo (este README)
├── uni-chat-backend/          ← proyecto .NET, Makefile, Docker Compose
│   ├── Makefile
│   ├── .env.example
│   ├── appsettings.json
│   ├── docker-compose.yml
│   └── uni-chat-backend.csproj
├── docs/                      ← documentación Docusaurus
└── uni-chat-backend.slnx
```

> **Importante:** los comandos `make` y `docker compose` se ejecutan siempre desde la carpeta del proyecto:
>
> ```bash
> cd uni-chat-backend
> ```

---

## Requisitos previos

| Herramienta | Linux | macOS | Windows |
|-------------|-------|-------|---------|
| **.NET SDK 10** | [Instalador / script oficial](https://dotnet.microsoft.com/download) | `brew install dotnet` | [Instalador](https://dotnet.microsoft.com/download) o `winget install Microsoft.DotNet.SDK.10` |
| **Docker + Compose v2** | Docker Engine + plugin `compose` | [Docker Desktop](https://www.docker.com/products/docker-desktop/) | [Docker Desktop](https://www.docker.com/products/docker-desktop/) (**WSL2** recomendado) |
| **GNU Make** | `sudo apt install build-essential` (Debian/Ubuntu) o equivalente | Xcode Command Line Tools o `brew install make` | Ver nota Windows abajo |
| **Bun** (opcional, solo docs) | [bun.sh](https://bun.sh) | `brew install oven-sh/bun/bun` | Instalador Bun o usar WSL |

### Windows: cómo usar el Makefile

El [`Makefile`](uni-chat-backend/Makefile) usa **bash** (`read`, `case`, colores). Opciones recomendadas:

1. **WSL2 (Ubuntu)** — entorno recomendado: clona el repo dentro de WSL y usa `make` con normalidad.
2. **Git Bash** + GNU Make — por ejemplo con [Chocolatey](https://chocolatey.org/): `choco install make`.
3. **PowerShell / CMD sin bash** — el Makefile **no** funcionará; usa la sección [Sin Makefile](#sin-makefile-windows--alternativa) con comandos equivalentes.

---

## Clonar el proyecto

```bash
git clone <url-del-repositorio>
cd uni-chat-backend/uni-chat-backend
```

Comprueba que las herramientas están instaladas:

```bash
dotnet --version      # debe ser 10.x
docker compose version
make --version        # GNU Make (opcional si usas comandos manuales)
```

---

## Configuración

Hay **dos archivos** de configuración según cómo ejecutes la API:

| Archivo | Uso |
|---------|-----|
| **`.env`** | Variables para **Docker Compose** (MongoDB, Redis, RabbitMQ, JWT en contenedor, paneles admin). |
| **`appsettings.json`** | Configuración cuando ejecutas la API en el **host** con `make run` / `dotnet run`. |

### 1. Variables Docker (`.env`)

Desde `uni-chat-backend/`:

```bash
make setup
# o, con preguntas si ya existe .env:
make setup-interactive
```

Equivalente manual: copia `.env.example` a `.env` y rellena los valores.

#### Variables principales

| Variable | Descripción | Ejemplo (desarrollo local) |
|----------|-------------|----------------------------|
| `MONGO_ROOT_USERNAME` | Usuario admin de MongoDB en Docker | `admin` |
| `MONGO_ROOT_PASSWORD` | Contraseña admin MongoDB | `admin123` |
| `MONGO_CONNECTION_STRING` | Cadena para la **API en Docker** (`make docker-up`) | `mongodb://admin:admin123@mongodb:27017/?authSource=admin` |
| `MONGO_DATABASE` | Nombre de la base de datos | `unichat` |
| `REDIS_CONNECTION_STRING` | Redis para la API en Docker | `redis:6379` |
| `RABBITMQ_USER` / `RABBITMQ_PASS` | Credenciales RabbitMQ | `guest` / `guest` |
| `RABBITMQ_CONNECTION_STRING` | AMQP para la API en Docker | `amqp://guest:guest@rabbitmq:5672` |
| `JWT_KEY` | Clave secreta JWT (mín. 32 caracteres en producción) | cadena larga aleatoria |
| `JWT_ISSUER` | Emisor del token | `uni-chat` |
| `JWT_AUDIENCE` | Audiencia del token | `uni-chat-users` |
| `JWT_EXPIRE_MINUTES` | Expiración access token | `15` |
| `REFRESH_EXPIRE_DAYS` | Expiración refresh token | `7` |
| `CLOUDINARY_CLOUD_NAME` | Cloudinary (subida de archivos) | tu cloud name |
| `CLOUDINARY_API_KEY` | API key Cloudinary | — |
| `CLOUDINARY_API_SECRET` | API secret Cloudinary | — |
| `MONGO_EXPRESS_USER` / `MONGO_EXPRESS_PASS` | Login del panel mongo-express | opcional |
| `SEQ_ADMIN` / `SEQ_ADMIN_PASSWORD` | Admin de Seq (logs) | opcional |

> **No subas** `.env`, claves JWT ni credenciales Cloudinary al repositorio.

### 2. API en el host (`appsettings.json`)

Cuando usas el flujo habitual **`make up`** (solo infra en Docker) + **`make run`** (API en tu máquina), apunta los servicios a **`localhost`**:

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
    "Key": "tu-clave-secreta-muy-larga",
    "Issuer": "uni-chat",
    "Audience": "uni-chat-users",
    "ExpireMinutes": "15"
  },
  "RefreshToken": {
    "ExpireDays": "7"
  },
  "Cloudinary": {
    "CloudName": "tu-cloud-name",
    "ApiKey": "tu-api-key",
    "ApiSecret": "tu-api-secret"
  }
}
```

Los valores de MongoDB deben coincidir con `MONGO_ROOT_*` y `MONGO_DATABASE` de tu `.env`.

### Modos de ejecución

| Modo | Comandos | API | Infra |
|------|----------|-----|-------|
| **Desarrollo habitual** | `make up` + `make run` | Host → `http://localhost:5012` | Docker |
| **Todo en Docker** | `make docker-up` | Contenedor → `http://localhost:8080` | Docker |
| **Atajo dev** | `make dev` | Igual que habitual (setup + up + run) | Docker |
| **Dev con hot reload** | `make dev-watch` | `dotnet watch` | Docker |

---

## Inicio rápido

### Opción 1: Instalación completa recomendada

```bash
cd uni-chat-backend

make install  # Verifica dependencias, configura .env, restaura paquetes y compila
# Edita .env y appsettings.json con tus credenciales

make up       # MongoDB, Redis, RabbitMQ y paneles
make run      # API en http://localhost:5012
```

### Opción 2: Instalación manual

```bash
cd uni-chat-backend

make setup
# Edita .env y appsettings.json con tus credenciales

make up       # MongoDB, Redis, RabbitMQ y paneles
make run      # API en http://localhost:5012
```

### Opción 3: Atajo de desarrollo

```bash
cd uni-chat-backend

# Configura .env y appsettings.json primero
make dev      # setup + up + run en un solo comando
```

### Ver puertos de servicios

Para ver todos los puertos disponibles:

```bash
make ports
```

Esto mostrará una tabla visual con los puertos de MongoDB, Redis, RabbitMQ, mongo-express, RedisInsight, Seq, Dozzle, Portainer y la API.

**URLs locales (API en host):**

| URL | Uso |
|-----|-----|
| `http://localhost:5012` | HTTP (perfil por defecto) |
| `https://localhost:7155` | HTTPS (perfil `https` en launchSettings) |
| OpenAPI / Scalar | Disponible en entorno `Development` |

**SignalR:** hub en `/messages/chat` (requiere JWT).

---

## Instalación desde Imagen Docker (GitHub Container Registry)

El proyecto publica automáticamente una imagen Docker en **GitHub Container Registry (GHCR)** en cada push a la rama `main`. Esta opción es ideal para producción o cuando no necesitas modificar el código.

### Ventajas de usar la imagen pre-construida

- ✅ No requiere .NET SDK instalado
- ✅ Build consistente en todos los entornos
- ✅ Despliegue más rápido
- ✅ Ideal para producción y staging
- ✅ Imágenes versionadas con tags `latest` y SHA del commit

### Paso 1: Autenticación en GHCR (opcional para imágenes públicas)

Si la imagen es pública, puedes descargarla sin autenticación. Si es privada, necesitas autenticarte:

```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u $GITHUB_ACTOR --password-stdin
```

Para uso personal, puedes usar tu GitHub Personal Access Token.

### Paso 2: Descargar la imagen

```bash
ghcr.io/dar3kdev/uni-chat-backend:main
# O una versión específica:
ghcr.io/dar3kdev/uni-chat-backend:<commit-sha>
```

### Paso 3: Configurar variables de entorno

Crea un archivo `.env` con las variables necesarias (igual que en desarrollo local):

```bash
MONGO_ROOT_USERNAME=admin
MONGO_ROOT_PASSWORD=admin123
MONGO_CONNECTION_STRING=mongodb://admin:admin123@mongodb:27017/?authSource=admin
MONGO_DATABASE=unichat
REDIS_CONNECTION_STRING=redis:6379
RABBITMQ_USER=guest
RABBITMQ_PASS=guest
RABBITMQ_CONNECTION_STRING=amqp://guest:guest@rabbitmq:5672
JWT_KEY=tu-clave-secreta-muy-larga-minimo-32-caracteres
JWT_ISSUER=uni-chat
JWT_AUDIENCE=uni-chat-users
JWT_EXPIRE_MINUTES=15
REFRESH_EXPIRE_DAYS=7
CLOUDINARY_CLOUD_NAME=tu-cloud-name
CLOUDINARY_API_KEY=tu-api-key
CLOUDINARY_API_SECRET=tu-api-secret
```

### Paso 4: Ejecutar con docker-compose

Crea un `docker-compose.yml` para producción:

```yaml
version: '3.8'

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
    image: redis:latest
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
    image: ghcr.io/DaR3kDev/uni-chat-backend:latest
    container_name: uni-chat-api
    restart: unless-stopped
    environment:
      - MONGO_CONNECTION_STRING=${MONGO_CONNECTION_STRING}
      - MONGO_DATABASE=${MONGO_DATABASE}
      - REDIS_CONNECTION_STRING=${REDIS_CONNECTION_STRING}
      - RABBITMQ_CONNECTION_STRING=${RABBITMQ_CONNECTION_STRING}
      - JWT_KEY=${JWT_KEY}
      - JWT_ISSUER=${JWT_ISSUER}
      - JWT_AUDIENCE=${JWT_AUDIENCE}
      - JWT_EXPIRE_MINUTES=${JWT_EXPIRE_MINUTES}
      - REFRESH_EXPIRE_DAYS=${REFRESH_EXPIRE_DAYS}
      - CLOUDINARY_CLOUD_NAME=${CLOUDINARY_CLOUD_NAME}
      - CLOUDINARY_API_KEY=${CLOUDINARY_API_KEY}
      - CLOUDINARY_API_SECRET=${CLOUDINARY_API_SECRET}
    ports:
      - "8080:8080"
    depends_on:
      - mongodb
      - redis
      - rabbitmq

volumes:
  mongodb_data:
```

Ejecuta el stack:

```bash
docker compose up -d
```

La API estará disponible en `http://localhost:8080`.

### Verificar la imagen en GHCR

1. Ve al repositorio en GitHub
2. Haz clic en la pestaña "Packages"
3. Busca el paquete `uni-chat-backend`
4. Verás las versiones disponibles (latest y SHA)

---

## Makefile

Los nombres de los comandos están en **inglés**; las descripciones en **`make help`** están en **español**.

Ejecuta desde `uni-chat-backend/`:

```bash
make          # igual que make help
make help
```

### Configuración

| Comando | Descripción |
|---------|-------------|
| `help` | Muestra todos los comandos y sus descripciones en español |
| `install` | Instalación completa con mensajes interactivos, emojis y registro de pasos |
| `setup` | Crea `.env` desde `.env.example` si no existe |
| `setup-interactive` | Si `.env` existe: omitir, respaldar y recrear, o salir |

### .NET

| Comando | Descripción |
|---------|-------------|
| `restore` | Restaura paquetes NuGet |
| `build` | Compila el proyecto |
| `run` | Ejecuta la API (`http://localhost:5012`) |
| `watch` | Ejecuta con recarga automática al guardar |
| `clean` | Limpia artefactos de compilación (sin confirmación) |
| `clean-confirm` | Pide confirmación en español antes de limpiar |

### Docker

| Comando | Descripción |
|---------|-------------|
| `up` | Levanta MongoDB, Redis, RabbitMQ y paneles en segundo plano |
| `down` | Detiene y elimina contenedores (sin confirmación) |
| `down-confirm` | Pregunta antes de detener el stack |
| `restart` | `down` + `up` |
| `logs` | Logs en tiempo real de todos los servicios |
| `ps` | Estado de los contenedores |
| `ports` | Muestra los puertos de los servicios Docker con tabla visual |
| `docker-build` | Construye la imagen Docker de la API |
| `docker-up` | Construye y levanta el stack completo (API en puerto 8080) |

### Documentación (Docusaurus)

| Comando | Descripción |
|---------|-------------|
| `docs-install` | Instala dependencias con Bun en `../docs` |
| `docs` | Servidor de desarrollo de la documentación |
| `docs-build` | Genera el build estático de la documentación |

### Atajos e interactivo

| Comando | Descripción |
|---------|-------------|
| `dev` | `setup` + `up` + `run` |
| `dev-watch` | `setup` + `up` + `watch` |
| `menu` | Menú numerado en español (acciones frecuentes) |

> Con `CI=true`, los targets `*-confirm` y `menu` no ejecutan acciones destructivas ni abren menús interactivos (pensado para pipelines).

### Puertos tras `make up`

| Servicio | Puerto |
|----------|--------|
| MongoDB | 27017 |
| Redis | 6379 |
| RabbitMQ | 5672 (UI de gestión: 15672) |
| mongo-express | 8081 |
| RedisInsight | 5540 |
| Seq (logs) | 5341 |
| Dozzle | 8083 |
| Portainer | 9000 / 9443 |
| API (solo con `docker-up`) | 8080 |

---

## CI/CD con GitHub Actions

El proyecto incluye un workflow automatizado de **CI/CD** que se ejecuta en GitHub Actions. Este workflow garantiza la calidad del código y automatiza el despliegue de la imagen Docker.

### ¿Qué hace el workflow?

El workflow se divide en dos jobs principales:

#### 1. Job CI (Continuous Integration)
- **Checkout**: Obtiene el código del repositorio
- **Setup .NET**: Configura .NET SDK 10.0
- **Restore**: Descarga las dependencias NuGet
- **Build**: Compila el proyecto en configuración Release
- **Test**: Ejecuta los tests unitarios (si existen)

#### 2. Job Docker Build & Push
- **Setup Docker Buildx**: Configura Docker para builds multi-plataforma
- **Login a GHCR**: Se autentica en GitHub Container Registry usando `GITHUB_TOKEN`
- **Build y Push**: Construye la imagen Docker y la publica en GHCR
- **Tags**: Genera tags `latest` y el SHA del commit para trazabilidad
- **Cache**: Usa cache de capas Docker para acelerar builds futuros

### ¿Cuándo se ejecuta?

El workflow se ejecuta automáticamente en los siguientes eventos:
- **Push a rama `main`**: Ejecuta CI y publica la imagen en GHCR
- **Pull requests a `main`**: Ejecuta solo CI (sin publicar imagen)

### ¿Cómo ayuda al desarrollo?

El workflow CI/CD proporciona múltiples beneficios:

✅ **Detección temprana de errores**: Los errores de compilación y tests se detectan antes del merge

✅ **Builds consistentes**: La imagen Docker se construye siempre en el mismo entorno, eliminando diferencias entre máquinas

✅ **Automatización**: No necesitas construir manualmente la imagen Docker para producción

✅ **Versionado**: Cada commit genera una imagen con su SHA, permitiendo rollback a versiones específicas

✅ **Integración continua**: Garantiza que el código siempre compila y pasa tests antes de llegar a producción

### Ver la ejecución del workflow

1. Ve al repositorio en GitHub
2. Haz clic en la pestaña "Actions"
3. Verás el historial de ejecuciones del workflow
4. Haz clic en una ejecución para ver los detalles, logs y artefactos

### Usar la imagen generada en GHCR

Después de cada push exitoso a `main`, la imagen está disponible en:

```
ghcr.io/DaR3kDev/uni-chat-backend:latest
ghcr.io/DaR3kDev/uni-chat-backend:<commit-sha>
```

Para usarla, sigue la guía en la sección [Instalación desde Imagen Docker (GHCR)](#instalación-desde-imagen-docker-github-container-registry).

### Configuración del workflow

El archivo de configuración está en `.github/workflows/ci-cd-backend.yml`. El workflow usa:
- `GITHUB_TOKEN`: Proporcionado automáticamente por GitHub (no requiere configuración manual)
- Permisos: `contents: read` y `packages: write` para publicar en GHCR

---

## Sin Makefile (Windows / alternativa)

Desde la carpeta `uni-chat-backend/`:

| Make | Linux / macOS / WSL / Git Bash | Windows CMD | Windows PowerShell |
|------|--------------------------------|-------------|-------------------|
| `setup` | `cp .env.example .env` | `copy .env.example .env` | `Copy-Item .env.example .env` |
| `up` | `docker compose up -d` | igual | igual |
| `down` | `docker compose down` | igual | igual |
| `run` | `dotnet run --project uni-chat-backend.csproj` | igual | igual |
| `build` | `dotnet build uni-chat-backend.csproj` | igual | igual |
| `restore` | `dotnet restore uni-chat-backend.csproj` | igual | igual |
| `watch` | `dotnet watch run --project uni-chat-backend.csproj` | igual | igual |
| `logs` | `docker compose logs -f` | igual | igual |
| `ps` | `docker compose ps` | igual | igual |
| `docker-up` | `docker compose up -d --build` | igual | igual |

---

## Solución de problemas

| Problema | Qué hacer |
|----------|-----------|
| `dotnet: command not found` | Instala [.NET SDK 10](https://dotnet.microsoft.com/download) y reinicia la terminal. |
| `make: command not found` (Windows) | Usa WSL2, Git Bash + GNU Make, o los [comandos manuales](#sin-makefile-windows--alternativa). |
| Docker no arranca o puerto en uso | `make ps` o `docker compose ps`; libera el puerto o `make down`. |
| La API no conecta a MongoDB/Redis | Con `make run`, usa `localhost` en `appsettings.json`. Con `docker-up`, usa hosts `mongodb`, `redis`, `rabbitmq` en `.env`. |
| `menu` o `*-confirm` no preguntan | Comprueba si `CI=true` está definido en el entorno. |
| `make install` falla | Verifica que .NET SDK 10 y Docker estén instalados. Ejecuta `dotnet --version` y `docker --version`. |
| Error al pull de imagen GHCR | Si la imagen es privada, autentícate: `echo $GITHUB_TOKEN \| docker login ghcr.io -u $GITHUB_ACTOR --password-stdin`. |
| Workflow CI/CD falla | Ve a la pestaña "Actions" en GitHub para ver los logs del workflow. Verifica que el código compile localmente. |
| Imagen no aparece en GHCR | Verifica que el workflow se haya ejecutado exitosamente en "Actions". Solo se publica en push a `main`. |
| Error de autenticación en GHCR | Asegúrate de que el repositorio tenga los permisos correctos: `contents: read` y `packages: write`. |

---

## Seguridad

- No commitees `.env`, claves JWT ni credenciales de Cloudinary.
- Usa secretos distintos en desarrollo y producción.
- Para detalle de endpoints y flujos E2E, consulta [`docs/docs/backend.mdx`](docs/docs/backend.mdx) y [`docs/docs/chat-privado.md`](docs/docs/chat-privado.md).
