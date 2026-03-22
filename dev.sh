#!/usr/bin/env bash
set -euo pipefail

# Start PostgreSQL
podman compose up -d db

# Wait for DB to be healthy
echo "Waiting for database..."
until podman compose exec db pg_isready -q 2>/dev/null; do
  sleep 1
done
echo "Database ready."

# Start Angular dev server in background
npm run start --prefix garden-assistant-app &
ANGULAR_PID=$!

# Start .NET backend
dotnet run --project garden-assistant-api/garden-assistant-api.csproj &
DOTNET_PID=$!

trap 'kill $ANGULAR_PID $DOTNET_PID 2>/dev/null; podman compose down' EXIT INT TERM

echo "Dev environment started."
wait
