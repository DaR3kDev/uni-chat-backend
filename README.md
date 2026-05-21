# uni-chat-backend

API de chat en tiempo real con **ASP.NET Core 10**: JWT, MongoDB, Redis, RabbitMQ, SignalR y Cloudinary.

---

## Empieza aquí

**¿Primera vez en el proyecto?** Sigue la guía paso a paso (instalación, configuración, errores comunes):

**[docs/GUIA-INSTALACION.md](docs/GUIA-INSTALACION.md)**

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
- [Documentación API](#documentación-api)

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

Pasos detallados, checklist y troubleshooting: **[GUIA-INSTALACION.md](docs/GUIA-INSTALACION.md)**.

---

## Estructura del repositorio

```
uni-chat-backend/                 ← raíz del repo (este README)
├── .githooks/                    ← pre-commit (make install-hooks)
├── .github/workflows/            ← CI/CD en GitHub Actions
├── scripts/                      ← render-env.sh, install-git-hooks.sh
├── docs/
│   ├── GUIA-INSTALACION.md       ← guía para principiantes
│   ├── github-secrets.md         ← secrets y deploy
│   └── docs/                     ← Docusaurus (API, arquitectura)
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
| Bun 1.2.23 (solo docs) | Ver [`.bun-version`](docs/.bun-version) | Igual | WSL2 |

En Windows, el Makefile requiere **bash** (WSL2 o Git Bash). Ver [Windows sin Make](#windows-sin-make).

---

## Configuración

| Archivo | Uso |
|---------|-----|
| `.env` | Variables para **Docker Compose** |
| `appsettings.json` | API en el **host** con `make run` |
| `.env.ci.example` | Valores de prueba listos para copiar |
| `.env.example` | Plantilla vacía (`make setup`) |

Guía completa de variables y ejemplos: **[GUIA-INSTALACION — Paso 3](docs/GUIA-INSTALACION.md#paso-3--configurar-variables-locales)**.

Secrets en producción / GitHub: **[docs/github-secrets.md](docs/github-secrets.md)**.

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

### Documentación

| Comando | Descripción |
|---------|-------------|
| `docs-install` | `bun install` en `docs/` |
| `docs` | Servidor Docusaurus |
| `docs-build` | Build estático |

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

Uso en servidor, compose de producción y login en registry: **[GUIA-INSTALACION — Producción GHCR](docs/GUIA-INSTALACION.md#producción-con-imagen-docker-ghcr)**.

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
| Bun incorrecto | `bun upgrade --version 1.2.23` |

**Más casos y soluciones paso a paso:** [GUIA-INSTALACION — Errores frecuentes](docs/GUIA-INSTALACION.md#errores-frecuentes).

---

## Seguridad

- No commitees `.env` ni claves reales (el pre-commit lo bloquea en staging).
- Desarrollo y producción: secretos distintos.
- GitHub Actions / deploy: [github-secrets.md](docs/github-secrets.md) y `make env-from-ci`.

---

## Documentación API

- Guía de instalación: [docs/GUIA-INSTALACION.md](docs/GUIA-INSTALACION.md)
- Endpoints y arquitectura: [docs/docs/backend.mdx](docs/docs/backend.mdx)
- Chat privado / E2E: [docs/docs/chat-privado.md](docs/docs/chat-privado.md)
- Sitio Docusaurus (local): `make docs` desde `uni-chat-backend/`
