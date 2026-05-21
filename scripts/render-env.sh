#!/usr/bin/env sh
# Genera uni-chat-backend/.env desde variables de entorno (sin valores en el repo).
# Uso: export JWT_KEY=... MONGO_CONNECTION_STRING=... && ./scripts/render-env.sh
#      ./scripts/render-env.sh --check   # solo valida que existan las obligatorias
set -eu

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
ENV_FILE="$ROOT/uni-chat-backend/.env"
CHECK_ONLY=false

if [ "${1:-}" = "--check" ]; then
  CHECK_ONLY=true
fi

# Variables obligatorias para Docker Compose + API
REQUIRED_VARS="
MONGO_ROOT_USERNAME
MONGO_ROOT_PASSWORD
MONGO_CONNECTION_STRING
MONGO_DATABASE
REDIS_CONNECTION_STRING
RABBITMQ_USER
RABBITMQ_PASS
RABBITMQ_CONNECTION_STRING
JWT_KEY
JWT_ISSUER
JWT_AUDIENCE
JWT_EXPIRE_MINUTES
REFRESH_EXPIRE_DAYS
CLOUDINARY_CLOUD_NAME
CLOUDINARY_API_KEY
CLOUDINARY_API_SECRET
"

missing=""
for var in $REQUIRED_VARS; do
  eval "val=\${$var:-}"
  if [ -z "$val" ]; then
    missing="$missing $var"
  fi
done

if [ -n "$missing" ]; then
  echo "Error: faltan variables de entorno obligatorias:$missing" >&2
  echo "Defínelas o configúralas como GitHub Secrets antes de deploy." >&2
  echo "Ver docs/github-secrets.md" >&2
  exit 1
fi

if [ "$CHECK_ONLY" = true ]; then
  echo "OK: todas las variables obligatorias están definidas."
  exit 0
fi

# Opcionales con default vacío
ENABLE_DOCS="${ENABLE_DOCS:-}"
MONGO_EXPRESS_USER="${MONGO_EXPRESS_USER:-}"
MONGO_EXPRESS_PASS="${MONGO_EXPRESS_PASS:-}"
SEQ_ADMIN="${SEQ_ADMIN:-}"
SEQ_ADMIN_PASSWORD="${SEQ_ADMIN_PASSWORD:-}"

cat > "$ENV_FILE" << EOF
# Generado por scripts/render-env.sh — no editar en CI; regenerar con secrets.
# $(date -u +"%Y-%m-%dT%H:%M:%SZ")

MONGO_ROOT_USERNAME=${MONGO_ROOT_USERNAME}
MONGO_ROOT_PASSWORD=${MONGO_ROOT_PASSWORD}
MONGO_CONNECTION_STRING=${MONGO_CONNECTION_STRING}
MONGO_DATABASE=${MONGO_DATABASE}

REDIS_CONNECTION_STRING=${REDIS_CONNECTION_STRING}

RABBITMQ_USER=${RABBITMQ_USER}
RABBITMQ_PASS=${RABBITMQ_PASS}
RABBITMQ_CONNECTION_STRING=${RABBITMQ_CONNECTION_STRING}

JWT_KEY=${JWT_KEY}
JWT_ISSUER=${JWT_ISSUER}
JWT_AUDIENCE=${JWT_AUDIENCE}
JWT_EXPIRE_MINUTES=${JWT_EXPIRE_MINUTES}

REFRESH_EXPIRE_DAYS=${REFRESH_EXPIRE_DAYS}

CLOUDINARY_CLOUD_NAME=${CLOUDINARY_CLOUD_NAME}
CLOUDINARY_API_KEY=${CLOUDINARY_API_KEY}
CLOUDINARY_API_SECRET=${CLOUDINARY_API_SECRET}

ENABLE_DOCS=${ENABLE_DOCS}

MONGO_EXPRESS_USER=${MONGO_EXPRESS_USER}
MONGO_EXPRESS_PASS=${MONGO_EXPRESS_PASS}

SEQ_ADMIN=${SEQ_ADMIN}
SEQ_ADMIN_PASSWORD=${SEQ_ADMIN_PASSWORD}
EOF

chmod 600 "$ENV_FILE" 2>/dev/null || true
echo "Archivo generado: $ENV_FILE"
