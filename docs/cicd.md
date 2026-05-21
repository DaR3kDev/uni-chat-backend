---
layout: default
title: CI/CD
nav_order: 5
description: Pipeline de GitHub Actions e imagen Docker en GHCR.
---

# CI/CD

Workflow: [`.github/workflows/ci-cd-backend.yml`](https://github.com/DaR3kDev/uni-chat-backend/blob/main/.github/workflows/ci-cd-backend.yml)

## Resumen

| Evento | Job | Qué hace |
|--------|-----|----------|
| Push o PR (cualquier rama) | `ci` | `make ci` en `ubuntu-24.04` (restore, lint, build, test) |
| Push a `main` (si `ci` pasa) | `docker` | Build y push de imagen a **GHCR** |

El deploy a servidores de producción sigue siendo **manual**; los secrets para ese paso están documentados en [CI/CD y secrets]({{ site.baseurl }}/github-secrets.html).

## Job `ci`

- Directorio de trabajo: `uni-chat-backend/`
- .NET SDK 10.x
- Cache de paquetes NuGet
- Comando: `make ci` (equivalente a restore + lint + build + test)

Ejecución local antes de push:

```bash
cd uni-chat-backend/uni-chat-backend
make install-hooks   # una vez: pre-commit en cada commit
make ci
```

| Comando | Incluye |
|---------|---------|
| `make pre-commit` | lint + build + test |
| `make ci` | restore + lint + build + test |

## Job `docker` (solo `main`)

Tras CI exitoso en push a `main`:

```
ghcr.io/dar3kdev/uni-chat-backend:latest
ghcr.io/dar3kdev/uni-chat-backend:main
ghcr.io/dar3kdev/uni-chat-backend:<commit-sha>
```

Contexto de build: carpeta `./uni-chat-backend` (Dockerfile del proyecto .NET).

### Usar la imagen en un servidor

1. Generar `.env` de producción (secrets o `scripts/render-env.sh`).
2. Definir compose con `image: ghcr.io/dar3kdev/uni-chat-backend:latest` y variables `Mongo__*`, `Jwt__*`, etc.
3. Login en registry si el paquete es privado:

```bash
echo $GITHUB_TOKEN | docker login ghcr.io -u TU_USUARIO --password-stdin
```

Ejemplo de compose de producción: sección [Producción con imagen Docker]({{ site.baseurl }}/instalacion.html#producción-con-imagen-docker-ghcr) en la guía de instalación.

## Makefile relacionado

Desde `uni-chat-backend/uni-chat-backend/`:

| Comando | Uso |
|---------|-----|
| `make ci` | Mismo pipeline que GitHub Actions |
| `make pre-commit` | Hook local |
| `make install-hooks` | Activa pre-commit en `.git/hooks` |
| `make docker-release` | Build y push local de imagen (avanzado) |

## Documentación del sitio

Cada push a `main` también reconstruye **GitHub Pages** desde la carpeta `docs/`. Para editar el sitio, ver [Contribuir a la documentación]({{ site.baseurl }}/contribuir-docs.html).
