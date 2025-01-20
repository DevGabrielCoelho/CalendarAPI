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

echo "Aguardando PostgreSQL iniciar..."

until pg_isready -h postgres -p 5432 -U "$POSTGRES_DOCKER_USER" > /dev/null 2>&1; do
    echo "Aguardando PostgreSQL... Tentando novamente em 5 segundos."
    sleep 5
done

echo "PostgreSQL iniciado."

if [ ! -f "$FLAG_FILE" ]; then
    echo "Executando as migrações..."
    sleep 5
    cd /src

    dotnet ef database update --connection "$ConnectionStrings__Npgsql"

    touch "$FLAG_FILE"
    
    echo "Migrações realizadas em $(date)" >> "$FLAG_FILE"

    echo "Migrações concluídas!"
else
    echo "As migrações já foram realizadas anteriormente."
    echo "Tentativa de migração em $(date) (já realizado)" >> "$FLAG_FILE"
fi

echo "Iniciando o aplicativo .NET..."

dotnet /app/publish/CalendarAPI.dll
