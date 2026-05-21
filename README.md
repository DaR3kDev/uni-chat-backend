# uni-chat-backend

API de chat en tiempo real con **ASP.NET Core 10**: JWT, MongoDB, Redis, RabbitMQ, SignalR y Cloudinary.

---

## Empieza aquí

**¿Primera vez en el proyecto?** Sigue la guía paso a paso (instalación, configuración, errores comunes):

- **Sitio web:** [dar3kdev.github.io/uni-chat-backend](https://dar3kdev.github.io/uni-chat-backend/)
- **En el repo:** [docs/instalacion.md](docs/instalacion.md) · [docs/GUIA-INSTALACION.md](docs/GUIA-INSTALACION.md) (índice)

---

## Índice

- [Inicio rápido](#inicio-rápido)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Requisitos](#requisitos)
- [Configuración](#configuración)
- [Makefile](#makefile)
- [CI/CD y calidad](#cicd-y-calidad)
- [Imagen Docker (GHCR)](#imagen-docker-ghcr)
- [Windows sin Make](#windows-sin-make)
- [Solución de problemas](#solución-de-problemas)
- [Seguridad](#seguridad)
- [Documentación](#documentación)

---

## Inicio rápido

Desde la carpeta del proyecto .NET (`uni-chat-backend/uni-chat-backend/`):

```bash
cd uni-chat-backend
cp .env.ci.example .env
# Edita appsettings.json (localhost) y Cloudinary en .env si lo necesitas
make up && make run
```

API: **http://localhost:5012**

Pasos detallados, checklist y troubleshooting: **[instalacion.md](docs/instalacion.md)** o el [sitio de documentación](https://dar3kdev.github.io/uni-chat-backend/instalacion.html).

---

## Estructura del repositorio

```
uni-chat-backend/                 ← raíz del repo (este README)
├── .githooks/                    ← pre-commit (make install-hooks)
├── .github/workflows/            ← CI/CD en GitHub Actions
├── scripts/                      ← render-env.sh, install-git-hooks.sh
├── docs/                         ← Jekyll (GitHub Pages)
│   ├── _config.yml
│   ├── instalacion.md
│   ├── backend.md
│   ├── github-secrets.md
│   └── ...
├── uni-chat-backend/             ← código .NET + Makefile + Docker
│   ├── Makefile
│   ├── .env.example
│   ├── .env.ci.example           ← valores dummy para dev
│   ├── docker-compose.yml
│   ├── uni-chat-backend.csproj
│   └── uni-chat-backend.Tests/
└── uni-chat-backend.slnx
```

> Todos los comandos `make` y `docker compose` se ejecutan desde **`uni-chat-backend/uni-chat-backend/`**.

---

## Requisitos

| Herramienta | Linux / WSL | macOS | Windows |
|-------------|---------------|-------|---------|
| .NET SDK 10 | [Descarga](https://dotnet.microsoft.com/download) | `brew install dotnet` | Instalador o WSL2 |
| Docker + Compose v2 | Docker Engine | Docker Desktop | Docker Desktop (WSL2) |
| GNU Make | `apt install build-essential` | Xcode CLI / `brew install make` | WSL2 recomendado |
| Ruby + Bundler (opcional) | Solo preview local de docs | `brew install ruby` | WSL2 |

En Windows, el Makefile requiere **bash** (WSL2 o Git Bash). Ver [Windows sin Make](#windows-sin-make).

---

## Configuración

| Archivo | Uso |
|---------|-----|
| `.env` | Variables para **Docker Compose** |
| `appsettings.json` | API en el **host** con `make run` |
| `.env.ci.example` | Valores de prueba listos para copiar |
| `.env.example` | Plantilla vacía (`make setup`) |

Guía completa de variables y ejemplos: **[Instalación — Paso 3](https://dar3kdev.github.io/uni-chat-backend/instalacion.html#paso-3--configurar-variables-locales)**.

Secrets en producción / GitHub: **[github-secrets.md](docs/github-secrets.md)**.

---

## Makefile

Ejecuta `make help` desde `uni-chat-backend/` (descripciones en español).

### Los más usados

| Comando | Descripción |
|---------|-------------|
| `help` | Lista de comandos |
| `up` | Levanta MongoDB, Redis, RabbitMQ y paneles |
| `down` | Detiene contenedores |
| `run` | API en http://localhost:5012 |
| `dev` | setup + up + run |
| `ps` / `ports` / `logs` | Estado, puertos y logs Docker |
| `docker-up` | Stack completo con API en puerto 8080 |

### Calidad y CI

| Comando | Descripción |
|---------|-------------|
| `ci` | restore + lint + build + test (como GitHub Actions) |
| `pre-commit` | lint + build + test (hook local) |
| `install-hooks` | Activa pre-commit en Git (una vez) |
| `lint` / `lint-fix` | Formato whitespace |
| `test` | Tests unitarios |
| `env-from-ci` | Genera `.env` desde variables de entorno |

### Documentación (Jekyll)

| Comando | Descripción |
|---------|-------------|
| `docs-install` | `bundle install` en `docs/` |
| `docs` | Servidor Jekyll local |
| `docs-build` | Build estático (validación) |

Publicación: push a `main` → GitHub Pages construye `docs/` automáticamente. Ver [docs/PAGES.md](docs/PAGES.md).

---

## CI/CD y calidad

Workflow: [`.github/workflows/ci-cd-backend.yml`](.github/workflows/ci-cd-backend.yml)

| Evento | Qué ocurre |
|--------|------------|
| Push / PR (cualquier rama) | Job `ci` en **ubuntu-24.04**: `make ci` |
| Push a `main` (si CI pasa) | Build y push imagen a GHCR |

Local:

```bash
make install-hooks   # una vez: pre-commit en cada commit
make ci              # antes de push
```

| Comando | Incluye |
|---------|---------|
| `make pre-commit` | lint + build + test |
| `make ci` | restore + lint + build + test |

Deploy a servidores: aún manual; ver [github-secrets.md](docs/github-secrets.md).

---

## Imagen Docker (GHCR)

Tras cada push exitoso a `main`:

```
ghcr.io/dar3kdev/uni-chat-backend:latest
ghcr.io/dar3kdev/uni-chat-backend:main
ghcr.io/dar3kdev/uni-chat-backend:<commit-sha>
```

Uso en servidor, compose de producción y login en registry: **[Instalación — Producción GHCR](https://dar3kdev.github.io/uni-chat-backend/instalacion.html#producción-con-imagen-docker-ghcr)**.

---

## Windows sin Make

Desde `uni-chat-backend/`:

| Make | Comando equivalente |
|------|---------------------|
| `setup` | `cp .env.example .env` |
| `up` | `docker compose up -d` |
| `down` | `docker compose down` |
| `run` | `dotnet run --project uni-chat-backend.csproj` |
| `build` | `dotnet build ../uni-chat-backend.slnx -c Release` |
| `test` | `dotnet test uni-chat-backend.Tests/uni-chat-backend.Tests.csproj -c Release` |
| `ci` | Usar WSL y `make ci` |

---

## Solución de problemas

| Problema | Qué hacer |
|----------|-----------|
| `dotnet: command not found` | .NET 10 + `export PATH="$HOME/.dotnet:$PATH"` en `~/.bashrc` |
| `make: command not found` | WSL2 o `apt install build-essential` |
| Docker / puerto en uso | `make down`, `make ps` |
| API no conecta a Mongo/Redis | `localhost` en `appsettings.json` con `make run`; hosts Docker en `.env` con `docker-up` |
| CI o commit falla en lint | `make lint-fix` |

**Más casos y soluciones paso a paso:** [Instalación — Errores frecuentes](https://dar3kdev.github.io/uni-chat-backend/instalacion.html#errores-frecuentes).

---

## Seguridad

- No commitees `.env` ni claves reales (el pre-commit lo bloquea en staging).
- Desarrollo y producción: secretos distintos.
- GitHub Actions / deploy: [github-secrets.md](docs/github-secrets.md) y `make env-from-ci`.

---

## Documentación

| Recurso | Enlace |
|---------|--------|
| Sitio (GitHub Pages) | [dar3kdev.github.io/uni-chat-backend](https://dar3kdev.github.io/uni-chat-backend/) |
| Instalación | [instalacion.md](docs/instalacion.md) |
| Backend / API | [backend.md](docs/backend.md) |
| Chat privado / E2E | [chat-privado.md](docs/chat-privado.md) |
| CI/CD y secrets | [github-secrets.md](docs/github-secrets.md) |
| Configurar Pages | [docs/PAGES.md](docs/PAGES.md) |
| Preview local | `make docs` desde `uni-chat-backend/` |
| OpenAPI / Scalar | Con la API en ejecución (`Development`) |
