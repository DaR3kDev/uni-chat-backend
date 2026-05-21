#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

PROJECT_DIR="$(cd "$SCRIPT_DIR/../uni-chat-backend" 2>/dev/null && pwd || true)"

if [ -z "$PROJECT_DIR" ] || [ ! -f "$PROJECT_DIR/.env" ]; then
  PROJECT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
fi

ENV_FILE="$PROJECT_DIR/.env"
OUTPUT_FILE="$PROJECT_DIR/appsettings.Development.json"

echo "DEBUG SCRIPT_DIR=$SCRIPT_DIR"
echo "DEBUG PROJECT_DIR=$PROJECT_DIR"
echo "DEBUG ENV_FILE=$ENV_FILE"

if [ ! -f "$ENV_FILE" ]; then
  echo "❌ No existe .env en: $ENV_FILE"
  exit 1
fi

export $(grep -v '^#' "$ENV_FILE" | xargs)

cat > "$OUTPUT_FILE" <<EOF
{
  "Mongo": {
    "ConnectionString": "$MONGO_CONNECTION_STRING",
    "Database": "$MONGO_DATABASE"
  },
  "RabbitMQ": {
    "ConnectionString": "$RABBITMQ_CONNECTION_STRING"
  },
  "Redis": {
    "ConnectionString": "$REDIS_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "$JWT_KEY",
    "Issuer": "$JWT_ISSUER",
    "Audience": "$JWT_AUDIENCE",
    "ExpireMinutes": $JWT_EXPIRE_MINUTES
  },
  "RefreshToken": {
    "ExpireDays": $REFRESH_EXPIRE_DAYS
  },
  "Cloudinary": {
    "CloudName": "$CLOUDINARY_CLOUD_NAME",
    "ApiKey": "$CLOUDINARY_API_KEY",
    "ApiSecret": "$CLOUDINARY_API_SECRET"
  }
}
EOF

echo "✅ OK: appsettings generado en $OUTPUT_FILE"