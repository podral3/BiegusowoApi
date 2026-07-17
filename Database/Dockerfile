FROM postgis/postgis:18-3.6

RUN apt-get update && apt-get install -y curl ca-certificates gnupg \
    && curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc | gpg --dearmor -o /usr/share/keyrings/pgdg.gpg \
    && echo "deb [signed-by=/usr/share/keyrings/pgdg.gpg] http://apt.postgresql.org/pub/repos/apt $(. /etc/os-release && echo $VERSION_CODENAME)-pgdg main" > /etc/apt/sources.list.d/pgdg.list \
    && apt-get update \
    && apt-get install -y postgresql-18-h3 \
    && apt-get purge -y curl gnupg \
    && rm -rf /var/lib/apt/lists/*
	
COPY 01-init-extensions.sh /docker-entrypoint-initdb.d/01-init-extensions.sh
RUN chmod +x /docker-entrypoint-initdb.d/01-init-extensions.sh