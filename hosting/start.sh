#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="$SCRIPT_DIR/.env"

if [ ! -f "$ENV_FILE" ]; then
  read -rp "Domain name (e.g. garden.example.com or localhost): " APP_DOMAIN
  APP_DOMAIN="${APP_DOMAIN:-localhost}"

  cp "$SCRIPT_DIR/.env.prod.example" "$ENV_FILE"

  JWT_KEY=$(openssl rand -base64 48)
  POSTGRES_PASSWORD=$(openssl rand -base64 24)

  if [[ "$OSTYPE" == "darwin"* ]]; then
    sed -i '' "s|changeme-minimum-32-characters-long|$JWT_KEY|" "$ENV_FILE"
    sed -i '' "s|POSTGRES_PASSWORD=changeme|POSTGRES_PASSWORD=$POSTGRES_PASSWORD|" "$ENV_FILE"
    sed -i '' "s|APP_DOMAIN=garden.example.com|APP_DOMAIN=$APP_DOMAIN|" "$ENV_FILE"
  else
    sed -i "s|changeme-minimum-32-characters-long|$JWT_KEY|" "$ENV_FILE"
    sed -i "s|POSTGRES_PASSWORD=changeme|POSTGRES_PASSWORD=$POSTGRES_PASSWORD|" "$ENV_FILE"
    sed -i "s|APP_DOMAIN=garden.example.com|APP_DOMAIN=$APP_DOMAIN|" "$ENV_FILE"
  fi

  echo "Created $ENV_FILE with generated secrets."
fi

cd "$SCRIPT_DIR"

# shellcheck disable=SC1090
source "$ENV_FILE"

COMPOSE_CMD="podman compose -f docker-compose.yaml"
TLS=false

if [ -n "${ACME_EMAIL:-}" ] && [ "$ACME_EMAIL" != "admin@example.com" ]; then
  COMPOSE_CMD="$COMPOSE_CMD -f docker-compose.tls.yaml"
  TLS=true
  echo "TLS enabled (ACME_EMAIL=$ACME_EMAIL)."
else
  echo "Starting without TLS."
fi

$COMPOSE_CMD up -d --build

if [ "$TLS" = true ]; then
  echo ""
  echo "Application available at: https://${APP_DOMAIN}:8443"
else
  echo ""
  echo "Application available at: http://${APP_DOMAIN}:8080"
fi
