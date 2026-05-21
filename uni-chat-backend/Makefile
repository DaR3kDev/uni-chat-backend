# Uni Chat Backend — Makefile
# Comandos en inglés; descripciones en español (make help).
# En CI (CI=true), los targets *-confirm no ejecutan acciones destructivas sin confirmación humana.

.DEFAULT_GOAL := help

CSPROJ     := uni-chat-backend.csproj
SLN        := ../uni-chat-backend.slnx
TEST_PROJ  := uni-chat-backend.Tests/uni-chat-backend.Tests.csproj
CONFIG     := Release
COMPOSE    := docker compose
DOCS_DIR      := ../docs
BUN_VERSION_FILE := $(DOCS_DIR)/.bun-version
BUN_VERSION   := $(shell cat $(BUN_VERSION_FILE) 2>/dev/null || echo "1.2.23")
BUN           := $(shell command -v bun 2>/dev/null || echo "$(HOME)/.bun/bin/bun")
# dotnet en PATH, o instalación local en ~/.dotnet (script oficial / CI del agente)
DOTNET        := $(shell command -v dotnet 2>/dev/null || echo "$(HOME)/.dotnet/dotnet")
IMAGE_NAME ?= ghcr.io/dar3kdev/uni-chat-backend
IMAGE_TAG  ?= main

# --- Funciones auxiliares para mensajes con emojis ---

# Colores ANSI
COLOR_RESET   := \033[0m
COLOR_BOLD    := \033[1m
COLOR_GREEN   := \033[32m
COLOR_YELLOW  := \033[33m
COLOR_BLUE    := \033[34m
COLOR_MAGENTA := \033[35m
COLOR_CYAN    := \033[36m
COLOR_RED     := \033[31m

# Función para imprimir paso con emoji
define print_step
@echo "$(COLOR_CYAN)$(1)$(COLOR_RESET)$(2)"
endef

# Función para imprimir éxito
define print_success
@echo "$(COLOR_GREEN)✅ $(1)$(COLOR_RESET)"
endef

# Función para imprimir error
define print_error
@echo "$(COLOR_RED)❌ $(1)$(COLOR_RESET)"
endef

# Función para imprimir banner
define print_header
@echo ""
@echo "$(COLOR_BOLD)$(COLOR_MAGENTA)$(1)$(COLOR_RESET)"
@echo "$(COLOR_MAGENTA)$(2)$(COLOR_RESET)"
@echo ""
endef

.PHONY: help setup setup-interactive install check-dotnet check-bun restore build run watch clean clean-confirm \
        lint lint-fix test ci pre-commit install-hooks env-from-ci \
        up down down-confirm restart logs ps ports docker-build docker-up \
        docker-image docker-push docker-release \
        docs-install docs docs-build dev dev-watch menu

# --- Ayuda ---

help: ## Muestra todos los comandos y sus descripciones en español
	@$(call print_header,"📖 Uni Chat Backend — Comandos Disponibles","═════════════════════════════════════════")
	@grep -E '^[a-zA-Z0-9_-]+:.*##' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*## "}; {printf "  $(COLOR_CYAN)%-22s$(COLOR_RESET) %s\n", $$1, $$2}'
	@echo ""
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) Usa $(COLOR_CYAN)make ci$(COLOR_RESET) antes de push (restore + lint + build + test)"
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) Usa $(COLOR_CYAN)make install-hooks$(COLOR_RESET) para activar pre-commit local"
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) Usa $(COLOR_CYAN)make install$(COLOR_RESET) para una instalación completa"
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) Usa $(COLOR_CYAN)make ports$(COLOR_RESET) para ver todos los puertos"
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) Usa $(COLOR_CYAN)make docker-release$(COLOR_RESET) para construir y publicar la imagen"
	@echo ""

# --- Configuración ---

setup: ## Crea .env y .env.dotnet desde sus archivos example si no existen (mensajes en español)
	@if [ -f .env ]; then \
		echo "\033[33mYa existe .env — no se ha modificado nada.\033[0m"; \
	else \
		cp .env.example .env && echo "\033[32mArchivo .env creado. Edítalo con tus credenciales antes de \`make up\`.\033[0m"; \
	fi

	@if [ -f .env.dotnet ]; then \
		echo "\033[33mYa existe .env.dotnet — no se ha modificado nada.\033[0m"; \
	else \
		cp .env.dotnet.example .env.dotnet && echo "\033[32mArchivo .env.dotnet creado para IConfiguration Binding.\033[0m"; \
	fi

setup-interactive: ## Pregunta en español si .env o .env.dotnet existen: omitir, respaldar o sobrescribir
	@if [ -z "$$CI" ] || [ "$$CI" != "true" ]; then \

		if [ ! -f .env.example ]; then \
			echo "\033[31mNo se encontró .env.example.\033[0m"; exit 1; \
		fi; \

		if [ ! -f .env.dotnet.example ]; then \
			echo "\033[31mNo se encontró .env.dotnet.example.\033[0m"; exit 1; \
		fi; \

		# =========================
		# .env
		# =========================
		if [ ! -f .env ]; then \
			cp .env.example .env && echo "\033[32mArchivo .env creado desde .env.example.\033[0m"; \
		else \
			echo "\033[33mYa existe un archivo .env.\033[0m"; \
			read -r -p "¿Qué deseas hacer con .env? [1=omitir / 2=respaldar .env y crear nuevo / 3=salir]: " choice; \
			case "$$choice" in \
				1) echo "Sin cambios en .env." ;; \
				2) cp .env ".env.backup.$$(date +%Y%m%d%H%M%S)" && \
				   cp .env.example .env && \
				   echo "\033[32mRespaldo creado y nuevo .env generado desde .env.example.\033[0m" ;; \
				3|*) echo "Operación finalizada."; exit 0 ;; \
			esac; \
		fi; \

		# =========================
		# .env.dotnet
		# =========================
		if [ ! -f .env.dotnet ]; then \
			cp .env.dotnet.example .env.dotnet && \
			echo "\033[32mArchivo .env.dotnet creado desde .env.dotnet.example.\033[0m"; \
		else \
			echo "\033[33mYa existe un archivo .env.dotnet.\033[0m"; \
			read -r -p "¿Qué deseas hacer con .env.dotnet? [1=omitir / 2=respaldar .env.dotnet y crear nuevo / 3=salir]: " choice; \
			case "$$choice" in \
				1) echo "Sin cambios en .env.dotnet." ;; \
				2) cp .env.dotnet ".env.dotnet.backup.$$(date +%Y%m%d%H%M%S)" && \
				   cp .env.dotnet.example .env.dotnet && \
				   echo "\033[32mRespaldo creado y nuevo .env.dotnet generado desde .env.dotnet.example.\033[0m" ;; \
				3|*) echo "Operación finalizada."; exit 0 ;; \
			esac; \
		fi; \

	else \
		echo "\033[33mCI=true: se omite setup-interactive (no hay terminal interactiva).\033[0m"; \
	fi

install: ## Instalación completa con mensajes interactivos, emojis y registro de pasos
	@$(call print_header,"🚀 Instalación de Uni Chat Backend","═════════════════════════════════════════")
	@$(call print_step,"🔍","Verificando dependencias...")
	@command -v dotnet >/dev/null 2>&1 || { $(call print_error,".NET no está instalado. Por favor instala .NET SDK 10."); exit 1; }
	@command -v docker >/dev/null 2>&1 || { $(call print_error,"Docker no está instalado. Por favor instala Docker."); exit 1; }
	@$(call print_success,"Dependencias verificadas")
	@echo ""
	@$(call print_step,"⚙️","Configurando archivo .env...")
	@$(MAKE) setup-interactive
	@$(call print_success,"Configuración completada")
	@echo ""
	@$(call print_step,"📦","Restaurando paquetes NuGet...")
	@$(MAKE) restore
	@$(call print_success,"Paquetes restaurados")
	@echo ""
	@$(call print_step,"🏗️","Compilando proyecto...")
	@$(MAKE) build
	@$(call print_success,"Proyecto compilado")
	@echo ""
	@$(call print_header,"✅ Instalación completada","═════════════════════════════════════════")
	@echo "🎉 ¡Todo listo para comenzar!"
	@echo ""
	@echo "Próximos pasos:"
	@echo "  • Ejecuta $(COLOR_CYAN)make up$(COLOR_RESET) para levantar los servicios Docker"
	@echo "  • Ejecuta $(COLOR_CYAN)make run$(COLOR_RESET) para iniciar la API"
	@echo "  • O usa $(COLOR_CYAN)make dev$(COLOR_RESET) para hacer todo junto"
	@echo ""

# --- .NET ---

check-dotnet:
	@if [ ! -x "$(DOTNET)" ] && ! command -v dotnet >/dev/null 2>&1; then \
		echo "$(COLOR_RED)❌ .NET SDK no encontrado.$(COLOR_RESET)"; \
		echo "Instala .NET 10: https://dotnet.microsoft.com/download"; \
		echo "O en WSL: curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0"; \
		echo "Luego añade a ~/.bashrc: export PATH=\"\$$HOME/.dotnet:\$$PATH\""; \
		exit 1; \
	fi

restore: check-dotnet ## Descarga y restaura los paquetes NuGet (solution: API + tests)
	@$(call print_step,"📦","Restaurando paquetes NuGet...")
	$(DOTNET) restore $(SLN)
	@$(call print_success,"Paquetes restaurados")

build: ## Compila la solution (API + tests) sin ejecutar
	@$(call print_step,"🏗️","Compilando solution...")
	$(DOTNET) build $(SLN) -c $(CONFIG)
	@$(call print_success,"Proyecto compilado")

lint: ## Verifica espacios en blanco y finales de línea (dotnet format whitespace)
	@$(call print_step,"🔍","Verificando formato de código...")
	$(DOTNET) format whitespace $(SLN) --verify-no-changes
	@$(call print_success,"Formato correcto")

lint-fix: ## Aplica correcciones de espacios en blanco y finales de línea
	@$(call print_step,"✏️","Aplicando formato de código...")
	$(DOTNET) format whitespace $(SLN)
	@$(call print_success,"Formato aplicado")

test: ## Ejecuta tests unitarios (Release)
	@$(call print_step,"🧪","Ejecutando tests...")
	$(DOTNET) test $(TEST_PROJ) -c $(CONFIG) --no-build --verbosity normal
	@$(call print_success,"Tests completados")

ci: restore lint build test ## Pipeline CI: restore, lint, build y test (mismo flujo que GitHub Actions)
	@$(call print_header,"✅ CI completado","═════════════════════════════════════════")

pre-commit: check-dotnet lint build test ## Hook local: lint + build + test (sin restore explícito)
	@$(call print_success,"Pre-commit checks OK")

install-hooks: ## Configura git hooks en .githooks (ejecutar una vez por clon)
	@$(call print_step,"🪝","Instalando hooks de Git...")
	@../scripts/install-git-hooks.sh
	@$(call print_success,"Hooks instalados — pre-commit ejecutará make pre-commit")

env-from-ci: ## Genera .env desde variables de entorno (GitHub Secrets / deploy)
	@$(call print_step,"🔐","Generando .env desde variables de entorno...")
	@../scripts/render-env.sh
	@$(call print_success,"Archivo .env generado")

run: ## Ejecuta la API (http://localhost:5012 en perfil http)
	@$(call print_step,"🚀","Iniciando API en http://localhost:5012...")
	$(DOTNET) run --project $(CSPROJ)

watch: ## Ejecuta la API con recarga automática al guardar cambios
	@$(call print_step,"👀","Iniciando API con hot reload...")
	$(DOTNET) watch run --project $(CSPROJ)

clean: ## Elimina artefactos de compilación (sin confirmación; apto para CI)
	@$(call print_step,"🧹","Limpiando artefactos de compilación...")
	$(DOTNET) clean $(SLN)
	@$(call print_success,"Limpieza completada")

clean-confirm: ## Pide confirmación en español antes de limpiar artefactos
	@if [ "$$CI" = "true" ]; then \
		echo "\033[33mCI=true: no se ejecuta \`dotnet clean\` en clean-confirm. Usa \`make clean\` en el pipeline.\033[0m"; \
	else \
		read -r -p "¿Eliminar artefactos de compilación (dotnet clean)? [s/N]: " a; \
		case "$$a" in s|S|si|Si|sí|Sí) $(DOTNET) clean $(CSPROJ); echo "\033[32mLimpieza completada.\033[0m" ;; \
			*) echo "Operación cancelada." ;; esac; \
	fi

# --- Docker ---

up: ## Inicia MongoDB, Redis, RabbitMQ y paneles de administración en segundo plano
	@$(call print_step,"🐳","Levantando contenedores Docker...")
	$(COMPOSE) up -d
	@$(call print_success,"Contenedores levantados")

down: ## Detiene y elimina los contenedores del stack (sin confirmación)
	@$(call print_step,"🛑","Deteniendo contenedores Docker...")
	$(COMPOSE) down
	@$(call print_success,"Contenedores detenidos")

down-confirm: ## Detiene contenedores tras confirmación (pregunta en español)
	@if [ "$$CI" = "true" ]; then \
		echo "\033[33mCI=true: no se ejecuta \`docker compose down\` en down-confirm. Usa \`make down\` en el pipeline si lo necesitas.\033[0m"; \
	else \
		read -r -p "¿Detener y eliminar los contenedores del stack? [s/N]: " a; \
		case "$$a" in s|S|si|Si|sí|Sí) $(COMPOSE) down; echo "\033[32mContenedores detenidos.\033[0m" ;; \
			*) echo "Operación cancelada." ;; esac; \
	fi

restart: down up ## Reinicia todos los contenedores (down + up)
	@$(call print_step,"🔄","Reiniciando contenedores...")

logs: ## Muestra los logs en tiempo real de todos los servicios
	@$(call print_step,"📋","Mostrando logs — Ctrl+C para salir...")
	$(COMPOSE) logs -f

ps: ## Lista el estado actual de los contenedores
	@$(call print_step,"📊","Estado de contenedores:")
	$(COMPOSE) ps

ports: ## Muestra los puertos de los servicios Docker
	@$(call print_header,"📊 Puertos de Servicios Docker","═════════════════════════════════════════")
	@echo "$(COLOR_BOLD)Servicio$(COLOR_RESET)           $(COLOR_BOLD)Puerto$(COLOR_RESET)    $(COLOR_BOLD)URL$(COLOR_RESET)"
	@echo "─────────────────────────────────────────────────────────────────"
	@echo "🐘 MongoDB             27017        mongodb://localhost:27017"
	@echo "🔴 Redis               6379         redis://localhost:6379"
	@echo "🐰 RabbitMQ            5672         amqp://localhost:5672"
	@echo "🐰 RabbitMQ UI         15672        http://localhost:15672"
	@echo "🗄️  mongo-express       8081         http://localhost:8081"
	@echo "🔍 RedisInsight        5540         http://localhost:5540"
	@echo "📋 Seq (logs)          5341         http://localhost:5341"
	@echo "📺 Dozzle (logs UI)    8083         http://localhost:8083"
	@echo "🚢 Portainer           9000         http://localhost:9000"
	@echo "🚢 Portainer (HTTPS)   9443         https://localhost:9443"
	@echo "🌐 API (docker-up)     8080         http://localhost:8080"
	@echo "─────────────────────────────────────────────────────────────────"
	@echo ""
	@echo "$(COLOR_YELLOW)Nota:$(COLOR_RESET) La API en modo desarrollo (make run) usa el puerto 5012"
	@echo ""

docker-build: ## Construye la imagen Docker de la API
	@$(call print_step,"🔨","Construyendo imagen Docker...")
	$(COMPOSE) build api
	@$(call print_success,"Imagen Docker construida")

docker-up: ## Construye y levanta el stack completo, incluyendo la API (puerto 8080)
	@$(call print_step,"🚀","Levantando stack completo con API...")
	$(COMPOSE) up -d --build
	@$(call print_success,"Stack levantado en http://localhost:8080")

docker-image: ## Construye la imagen Docker con IMAGE_NAME e IMAGE_TAG configurables
	@$(call print_header,"🐳 Construcción de Imagen Docker","═════════════════════════════════════════")
	@$(call print_step,"🔍 ","Imagen destino: $(COLOR_BOLD)$(IMAGE_NAME):$(IMAGE_TAG)$(COLOR_RESET)")
	@echo ""
	@$(call print_step,"🔨","Construyendo imagen...")
	docker build -t $(IMAGE_NAME):$(IMAGE_TAG) .
	@echo ""
	@$(call print_success,"Imagen construida: $(IMAGE_NAME):$(IMAGE_TAG)")
	@echo ""
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) Usa $(COLOR_CYAN)make docker-push$(COLOR_RESET) para publicarla en el registro"
	@echo "$(COLOR_YELLOW)💡 Tip:$(COLOR_RESET) O usa $(COLOR_CYAN)make docker-release$(COLOR_RESET) para build + push en un solo paso"
	@echo ""

docker-push: ## Publica la imagen Docker en el registro (IMAGE_NAME e IMAGE_TAG configurables)
	@$(call print_header,"📤 Publicación de Imagen Docker","═════════════════════════════════════════")
	@$(call print_step,"🔍 ","Imagen a publicar: $(COLOR_BOLD)$(IMAGE_NAME):$(IMAGE_TAG)$(COLOR_RESET)")
	@echo ""
	@$(call print_step,"📤","Publicando imagen en el registro remoto...")
	docker push $(IMAGE_NAME):$(IMAGE_TAG)
	@echo ""
	@$(call print_success,"Imagen publicada: $(IMAGE_NAME):$(IMAGE_TAG)")
	@echo ""

docker-release: docker-image docker-push ## Construye y publica la imagen en un solo paso (build + push)
	@$(call print_header,"🚀 Release completado","═════════════════════════════════════════")
	@echo "🎉 Imagen disponible en el registro:"
	@echo "   $(COLOR_CYAN)$(IMAGE_NAME):$(IMAGE_TAG)$(COLOR_RESET)"
	@echo ""
	@echo "Para usarla:"
	@echo "  docker pull $(IMAGE_NAME):$(IMAGE_TAG)"
	@echo ""

# --- Documentación (Docusaurus) ---

check-bun:
	@if [ ! -x "$(BUN)" ] && ! command -v bun >/dev/null 2>&1; then \
		echo "$(COLOR_RED)❌ Bun no encontrado (versión requerida: $(BUN_VERSION)).$(COLOR_RESET)"; \
		echo "Instala: curl -fsSL https://bun.sh/install | bash -s \"bun-v$(BUN_VERSION)\""; \
		echo "Versión del proyecto: $(BUN_VERSION_FILE)"; \
		exit 1; \
	fi
	@installed="$$($(BUN) --version 2>/dev/null | tr -d '\n')"; \
	if [ -n "$$installed" ] && [ "$$installed" != "$(BUN_VERSION)" ]; then \
		echo "$(COLOR_YELLOW)⚠️  Bun $$installed detectado; el proyecto usa $(BUN_VERSION).$(COLOR_RESET)"; \
		echo "Actualiza: $(BUN) upgrade --version $(BUN_VERSION)"; \
	fi

docs-install: check-bun ## Instala dependencias de docs con Bun $(BUN_VERSION)
	@$(call print_step,"📚","Instalando dependencias de documentación — Bun $(BUN_VERSION)...")
	cd $(DOCS_DIR) && $(BUN) install
	@$(call print_success,"Dependencias instaladas")

docs: check-bun ## Inicia el servidor de desarrollo de la documentación
	@$(call print_step,"📖","Iniciando servidor de documentación...")
	cd $(DOCS_DIR) && $(BUN) run start

docs-build: check-bun ## Genera el build estático de la documentación
	@$(call print_step,"📦","Generando build de documentación...")
	cd $(DOCS_DIR) && $(BUN) run build
	@$(call print_success,"Build generado")

# --- Atajos ---

dev: setup up run ## Configura .env, levanta Docker y ejecuta la API (bloqueante)
	@$(call print_step,"🚀","Modo desarrollo completo...")

dev-watch: setup up watch ## Configura .env, levanta Docker y ejecuta la API con recarga automática
	@$(call print_step,"👀","Modo desarrollo con hot reload...")

# --- Menú interactivo ---

menu: ## Menú numerado en español para acciones frecuentes
	@if [ "$$CI" = "true" ]; then \
		echo "\033[33mCI=true: el menú interactivo no está disponible. Usa \`make help\`.\033[0m"; exit 0; \
	fi; \
	while true; do \
		echo ""; \
		echo "  \033[1mMenú — Uni Chat Backend\033[0m"; \
		echo "  1) Ayuda (lista de comandos)"; \
		echo "  2) Levantar Docker (up)"; \
		echo "  3) Estado de contenedores (ps)"; \
		echo "  4) Ver logs (logs — seguirá en primer plano; Ctrl+C para volver al menú)"; \
		echo "  5) Ejecutar API (run — bloqueante; mejor salir del menú antes)"; \
		echo "  6) Crear .env si falta (setup)"; \
		echo "  7) Detener stack con confirmación (down-confirm)"; \
		echo "  8) Construir imagen Docker (docker-image)"; \
		echo "  9) Build + Push imagen Docker (docker-release)"; \
		echo "  0) Salir"; \
		read -r -p "  Elige una opción [0-9]: " opt; \
		case "$$opt" in \
			1) $(MAKE) help; ;; \
			2) $(MAKE) up; ;; \
			3) $(MAKE) ps; ;; \
			4) echo "\033[33mAbriendo logs (Ctrl+C para volver al menú)...\033[0m"; $(MAKE) logs; ;; \
			5) echo "\033[33mLa API es bloqueante. Recomendado: otra terminal con \`make run\`.\033[0m"; read -r -p "¿Ejecutar aquí igualmente? [s/N]: " c; case "$$c" in s|S|si|Si|sí|Sí) $(MAKE) run ;; *) : ;; esac; ;; \
			6) $(MAKE) setup; ;; \
			7) $(MAKE) down-confirm; ;; \
			8) read -r -p "  IMAGE_TAG (Enter para usar '$(IMAGE_TAG)'): " t; \
			   if [ -n "$$t" ]; then $(MAKE) docker-image IMAGE_TAG="$$t"; else $(MAKE) docker-image; fi; ;; \
			9) read -r -p "  IMAGE_TAG (Enter para usar '$(IMAGE_TAG)'): " t; \
			   if [ -n "$$t" ]; then $(MAKE) docker-release IMAGE_TAG="$$t"; else $(MAKE) docker-release; fi; ;; \
			0) echo "Hasta luego."; break ;; \
			*) echo "Opción no válida." ;; \
		esac; \
	done
