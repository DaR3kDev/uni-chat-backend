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

```bash
cd uni-chat-backend

make setup
# Edita .env y appsettings.json con tus credenciales

make up      # MongoDB, Redis, RabbitMQ y paneles
make run     # API en http://localhost:5012
```

O en un solo paso (después de configurar `.env` y `appsettings.json`):

```bash
make dev
```

**URLs locales (API en host):**

| URL | Uso |
|-----|-----|
| `http://localhost:5012` | HTTP (perfil por defecto) |
| `https://localhost:7155` | HTTPS (perfil `https` en launchSettings) |
| OpenAPI / Scalar | Disponible en entorno `Development` |

**SignalR:** hub en `/messages/chat` (requiere JWT).

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

---

## Seguridad

- No commitees `.env`, claves JWT ni credenciales de Cloudinary.
- Usa secretos distintos en desarrollo y producción.
- Para detalle de endpoints y flujos E2E, consulta [`docs/docs/backend.mdx`](docs/docs/backend.mdx) y [`docs/docs/chat-privado.md`](docs/docs/chat-privado.md).
