#!/bin/bash
set -e

# Włącz rozszerzenia w template1 - wszystkie nowe bazy będą je dziedziczyć
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname template1 <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS postgis;
    CREATE EXTENSION IF NOT EXISTS postgis_topology;
    CREATE EXTENSION IF NOT EXISTS h3;
EOSQL