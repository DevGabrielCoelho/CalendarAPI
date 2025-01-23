#!/bin/bash

if [ -f /src/.env ]; then
    while IFS= read -r line || [[ -n "$line" ]]; do
        [[ "$line" =~ ^#.*$ ]] && continue
        [[ -z "$line" ]] && continue
        if [[ "$line" == *=* ]]; then
            key=$(echo "$line" | cut -d '=' -f 1)
            value=$(echo "$line" | cut -d '=' -f 2-)
            export "$key"="$value"
        fi
    done < /src/.env
fi

FLAG_FILE="/app/flag_file/migrations_done.flag"

echo "Waiting for PostgreSQL to start..."

until pg_isready -h postgres -p 5432 -U "$POSTGRES_DOCKER_USER" > /dev/null 2>&1; do
    echo "Waiting for PostgreSQL... Retrying in 5 seconds."
    sleep 5
done

echo "PostgreSQL started."

if [ ! -f "$FLAG_FILE" ]; then
    echo "Running migrations..."
    sleep 5
    cd /src

    dotnet ef database update --connection "$ConnectionStrings__Npgsql"

    touch "$FLAG_FILE"
    
    echo "Migrations completed at $(date)" >> "$FLAG_FILE"

    echo "Migrations completed!"
else
    echo "Migrations have already been performed."
    echo "Migration attempt at $(date) (already done)" >> "$FLAG_FILE"
fi

echo "Starting the .NET application..."

dotnet /app/publish/CalendarAPI.dll
