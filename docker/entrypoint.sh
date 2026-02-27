#!/bin/bash
set -e

echo "Waiting for postgres to be ready..."
until PGPASSWORD=$DB_PASSWORD psql -h postgres -U ledgerlink_user -d LedgerLink -c '\q' 2>&1; do
  >&2 echo "Postgres is unavail able - sleeping"
  sleep 1
done

echo "PostgreSQL is up - checking for migrations..."

# Note: EF Core migrations must be applied from development machine or CI/CD
# This container uses the runtime image without SDK, so migrations can't run here
# You need to run migrations before or after deployment:
# docker compose exec app sh -c "apt-get update && apt-get install -y dotnet-sdk-8.0 && dotnet ef database update"

echo "Starting application..."
exec dotnet LedgerLink.dll
