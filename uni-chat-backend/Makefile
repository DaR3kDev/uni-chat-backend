# Uni Chat Backend — Makefile
# Comandos en inglés; descripciones en español (make help).
# En CI (CI=true), los targets *-confirm no ejecutan acciones destructivas sin confirmación humana.

.DEFAULT_GOAL := help

CSPROJ   := uni-chat-backend.csproj
COMPOSE  := docker compose
DOCS_DIR := ../docs
DOTNET   := dotnet

.PHONY: help setup setup-interactive restore build run watch clean clean-confirm \
        up down down-confirm restart logs ps docker-build docker-up \
        docs-install docs docs-build dev dev-watch menu

# --- Ayuda ---

help: ## Muestra todos los comandos y sus descripciones en español
	@echo ""
	@echo "  Uni Chat Backend — comandos disponibles"
	@echo ""
	@grep -E '^[a-zA-Z0-9_-]+:.*##' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*## "}; {printf "  \033[36m%-22s\033[0m %s\n", $$1, $$2}'
	@echo ""
	@echo "  Puertos típicos tras \`make up\`:"
	@echo "    MongoDB 27017 | Redis 6379 | RabbitMQ 5672 (UI 15672)"
	@echo "    mongo-express 8081 | RedisInsight 5540 | Seq 5341"
	@echo "    Dozzle 8083 | Portainer 9000/9443 | API 8080 (solo \`make docker-up\`)"
	@echo ""

# --- Configuración ---

setup: ## Crea .env desde .env.example si no existe (mensajes en español)
	@if [ -f .env ]; then \
		echo "\033[33mYa existe .env — no se ha modificado nada.\033[0m"; \
	else \
		cp .env.example .env && echo "\033[32mArchivo .env creado. Edítalo con tus credenciales antes de \`make up\`.\033[0m"; \
	fi

setup-interactive: ## Pregunta en español si .env existe: omitir, respaldar o sobrescribir
	@if [ -z "$$CI" ] || [ "$$CI" != "true" ]; then \
		if [ ! -f .env.example ]; then \
			echo "\033[31mNo se encontró .env.example.\033[0m"; exit 1; \
		fi; \
		if [ ! -f .env ]; then \
			cp .env.example .env && echo "\033[32mArchivo .env creado desde .env.example.\033[0m"; \
		else \
			echo "\033[33mYa existe un archivo .env.\033[0m"; \
			read -r -p "¿Qué deseas hacer? [1=omitir / 2=respaldar .env y crear nuevo / 3=salir]: " choice; \
			case "$$choice" in \
				1) echo "Sin cambios." ;; \
				2) cp .env ".env.backup.$$(date +%Y%m%d%H%M%S)" && cp .env.example .env && echo "\033[32mRespaldo creado y nuevo .env generado desde .env.example.\033[0m" ;; \
				3|*) echo "Operación finalizada." ;; \
			esac; \
		fi; \
	else \
		echo "\033[33mCI=true: se omite setup-interactive (no hay terminal interactiva).\033[0m"; \
	fi

# --- .NET ---

restore: ## Descarga y restaura los paquetes NuGet del proyecto
	$(DOTNET) restore $(CSPROJ)

build: ## Compila el proyecto sin ejecutarlo
	$(DOTNET) build $(CSPROJ)

run: ## Ejecuta la API (http://localhost:5012 en perfil http)
	$(DOTNET) run --project $(CSPROJ)

watch: ## Ejecuta la API con recarga automática al guardar cambios
	$(DOTNET) watch run --project $(CSPROJ)

clean: ## Elimina artefactos de compilación (sin confirmación; apto para CI)
	$(DOTNET) clean $(CSPROJ)

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
	$(COMPOSE) up -d

down: ## Detiene y elimina los contenedores del stack (sin confirmación)
	$(COMPOSE) down

down-confirm: ## Detiene contenedores tras confirmación (pregunta en español)
	@if [ "$$CI" = "true" ]; then \
		echo "\033[33mCI=true: no se ejecuta \`docker compose down\` en down-confirm. Usa \`make down\` en el pipeline si lo necesitas.\033[0m"; \
	else \
		read -r -p "¿Detener y eliminar los contenedores del stack? [s/N]: " a; \
		case "$$a" in s|S|si|Si|sí|Sí) $(COMPOSE) down; echo "\033[32mContenedores detenidos.\033[0m" ;; \
			*) echo "Operación cancelada." ;; esac; \
	fi

restart: down up ## Reinicia todos los contenedores (down + up)

logs: ## Muestra los logs en tiempo real de todos los servicios
	$(COMPOSE) logs -f

ps: ## Lista el estado actual de los contenedores
	$(COMPOSE) ps

docker-build: ## Construye la imagen Docker de la API
	$(COMPOSE) build api

docker-up: ## Construye y levanta el stack completo, incluyendo la API (puerto 8080)
	$(COMPOSE) up -d --build

# --- Documentación (Docusaurus) ---

docs-install: ## Instala las dependencias de la documentación con Bun
	cd $(DOCS_DIR) && bun install

docs: ## Inicia el servidor de desarrollo de la documentación
	cd $(DOCS_DIR) && bun run start

docs-build: ## Genera el build estático de la documentación
	cd $(DOCS_DIR) && bun run build

# --- Atajos ---

dev: setup up run ## Configura .env, levanta Docker y ejecuta la API (bloqueante)

dev-watch: setup up watch ## Configura .env, levanta Docker y ejecuta la API con recarga automática

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
		echo "  0) Salir"; \
		read -r -p "  Elige una opción [0-7]: " opt; \
		case "$$opt" in \
			1) $(MAKE) help; ;; \
			2) $(MAKE) up; ;; \
			3) $(MAKE) ps; ;; \
			4) echo "\033[33mAbriendo logs (Ctrl+C para volver al menú)...\033[0m"; $(MAKE) logs; ;; \
			5) echo "\033[33mLa API es bloqueante. Recomendado: otra terminal con \`make run\`.\033[0m"; read -r -p "¿Ejecutar aquí igualmente? [s/N]: " c; case "$$c" in s|S|si|Si|sí|Sí) $(MAKE) run ;; *) : ;; esac; ;; \
			6) $(MAKE) setup; ;; \
			7) $(MAKE) down-confirm; ;; \
			0) echo "Hasta luego."; break ;; \
			*) echo "Opción no válida." ;; \
		esac; \
	done
