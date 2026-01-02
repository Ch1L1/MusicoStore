#!/usr/bin/env bash
set -e

echo "Removing existing migrations..."

MIGRATIONS_PATH="src/MusicoStore.DataAccessLayer/Migrations"

if [ -d "$MIGRATIONS_PATH" ]; then
  find "$MIGRATIONS_PATH" -type f -name "*.cs" ! -name "AppDbContextModelSnapshot.cs" -delete
  echo "Migration files removed (snapshot preserved)."
else
  echo "Migrations folder not found."
fi

echo "Adding migration: Initial..."

dotnet ef migrations add Initial \
  --project src/MusicoStore.DataAccessLayer \
  --startup-project src/MusicoStore.WebApi

echo "Dropping database..."

dotnet ef database drop \
  --project src/MusicoStore.DataAccessLayer \
  --startup-project src/MusicoStore.WebApi \
  --force

echo "Updating database..."

dotnet ef database update \
  --project src/MusicoStore.DataAccessLayer \
  --startup-project src/MusicoStore.WebApi

echo "Migration reset complete."
