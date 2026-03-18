#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$SCRIPT_DIR/.env"

if [ ! -f "$ENV_FILE" ]; then
  cp "$SCRIPT_DIR/.env.prod.example" "$ENV_FILE"

  JWT_KEY=$(openssl rand -base64 48)
  POSTGRES_PASSWORD=$(openssl rand -base64 24)

  if [[ "$OSTYPE" == "darwin"* ]]; then
    sed -i '' "s|changeme-minimum-32-characters-long|$JWT_KEY|" "$ENV_FILE"
    sed -i '' "s|POSTGRES_PASSWORD=changeme|POSTGRES_PASSWORD=$POSTGRES_PASSWORD|" "$ENV_FILE"
  else
    sed -i "s|changeme-minimum-32-characters-long|$JWT_KEY|" "$ENV_FILE"
    sed -i "s|POSTGRES_PASSWORD=changeme|POSTGRES_PASSWORD=$POSTGRES_PASSWORD|" "$ENV_FILE"
  fi

  echo "Created $ENV_FILE with generated secrets."
  echo "Edit APP_DOMAIN and ACME_EMAIL before starting with TLS."
fi

cd "$SCRIPT_DIR"

COMPOSE_CMD="podman compose -f docker-compose.yaml"

# shellcheck disable=SC1090
source "$ENV_FILE"
if [ -n "${ACME_EMAIL:-}" ] && [ "$ACME_EMAIL" != "admin@example.com" ]; then
  COMPOSE_CMD="$COMPOSE_CMD -f docker-compose.tls.yaml"
  echo "TLS enabled (ACME_EMAIL=$ACME_EMAIL)."
else
  echo "TLS disabled (ACME_EMAIL not set). Starting HTTP only."
fi

$COMPOSE_CMD up -d --build
