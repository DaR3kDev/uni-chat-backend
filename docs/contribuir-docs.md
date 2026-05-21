---
layout: default
title: Editar documentación
nav_order: 7
description: Cómo añadir páginas y previsualizar el sitio Jekyll localmente.
---

# Contribuir a la documentación

El sitio público usa **Jekyll** con el tema [just-the-docs](https://github.com/just-the-docs/just-the-docs), publicado desde la carpeta `docs/` en la rama `main`.

URL: [https://dar3kdev.github.io/uni-chat-backend/](https://dar3kdev.github.io/uni-chat-backend/)

## Configuración en GitHub (una vez)

1. Repositorio → **Settings** → **Pages**
2. **Build and deployment** → Deploy from a branch
3. **Branch:** `main` · **Folder:** `/docs`

Si antes usabas la rama `gh-pages`, cambia la fuente a `/docs` en `main`.

## Añadir o editar una página

Crea o modifica un archivo `.md` en `docs/` con front matter:

```yaml
---
layout: default
title: Título en el menú
nav_order: 8
---
```

### Menú anidado (como Chat privado)

Página padre:

```yaml
---
layout: default
title: Mi sección
nav_order: 3
has_children: true
permalink: /mi-seccion/
---
```

Página hija:

```yaml
---
layout: default
title: Subpágina
parent: Mi sección
nav_order: 1
---
```

El `parent` debe coincidir exactamente con el `title` del padre.

### Archivos excluidos del sitio

No se publican (ver `exclude` en [`_config.yml`](https://github.com/DaR3kDev/uni-chat-backend/blob/main/docs/_config.yml)):

- `frontend.md` — cliente web en otro repositorio
- `GUIA-INSTALACION.md` — redirección en el repo
- `PAGES.md` — stub para mantenedores

`baseurl` del sitio: `/uni-chat-backend`. Enlaces internos recomendados:

```liquid
[Instalación]({{ site.baseurl }}/instalacion.html)
```

## Callouts (notas y avisos)

Usa el formato de just-the-docs, no la sintaxis `:::info` de Docusaurus:

```markdown
{: .note }
> **Info:** Texto de la nota.

{: .warning }
> **Aviso:** Texto de advertencia.

{: .important }
> **Importante:** Texto crítico.
```

## Preview local

Desde `uni-chat-backend/uni-chat-backend/`:

```bash
make docs-install   # bundle install en docs/ (Ruby + Bundler)
make docs           # http://127.0.0.1:4000/uni-chat-backend/
make docs-build     # genera docs/_site/ (validación)
```

Requisitos: Ruby 3.x y Bundler (`gem install bundler`).

Sin Ruby local puedes editar Markdown y validar tras push a `main` en GitHub Pages.

## Checklist antes de merge

- [ ] Front matter con `layout`, `title` y `nav_order` (y `parent` si aplica)
- [ ] Enlaces con `{{ site.baseurl }}` o rutas relativas al sitio
- [ ] `make docs-build` termina sin error
- [ ] Comandos y rutas coinciden con el código (`uni-chat-backend/uni-chat-backend/` para `make`)
