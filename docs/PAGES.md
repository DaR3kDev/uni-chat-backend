# Publicar documentación en GitHub Pages

El sitio usa **Jekyll** con el tema [just-the-docs](https://github.com/just-the-docs/just-the-docs), construido por GitHub Pages desde la carpeta `docs/` del repositorio.

## Configuración en GitHub (una vez)

1. Repositorio → **Settings** → **Pages**
2. **Build and deployment** → Source: **Deploy from a branch**
3. **Branch:** `main` · **Folder:** `/docs`
4. Guardar. Tras unos minutos el sitio queda en:

   `https://dar3kdev.github.io/uni-chat-backend/`

Si antes publicabas desde la rama `gh-pages` (build de Docusaurus), cambia la fuente a `/docs` en `main` para ver el contenido nuevo.

## Editar contenido

Añade o modifica archivos `.md` en `docs/` con front matter de just-the-docs, por ejemplo:

```yaml
---
layout: default
title: Mi página
nav_order: 7
---
```

El orden del menú lateral lo define `nav_order`. La configuración global está en [`_config.yml`](_config.yml) (`baseurl: /uni-chat-backend`).

## Preview local (opcional)

Desde `uni-chat-backend/uni-chat-backend/`:

```bash
make docs-install   # bundle install (Ruby)
make docs           # http://127.0.0.1:4000
make docs-build     # validar build en docs/_site/
```

Requisitos: Ruby 3.x y Bundler (`gem install bundler`).

Sin Ruby local puedes limitarte a editar Markdown y comprobar el sitio tras cada push a `main`.
