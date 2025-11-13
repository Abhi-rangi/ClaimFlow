#!/bin/bash

# Script to start the backend API
# Usage: ./start-backend.sh [inmemory|sqlserver]

MODE=${1:-inmemory}

echo "🚀 Starting Claims Management API Backend..."
echo "Mode: $MODE"

cd VerusClaims.API

if [ "$MODE" = "inmemory" ]; then
    echo "📦 Using In-Memory Database (no SQL Server required)"
    ASPNETCORE_ENVIRONMENT=Development dotnet run --no-build 2>/dev/null || dotnet run
elif [ "$MODE" = "sqlserver" ]; then
    echo "🗄️  Using SQL Server Database"
    echo "⚠️  Make sure SQL Server is running and connection string is configured!"
    dotnet run
else
    echo "❌ Invalid mode. Use 'inmemory' or 'sqlserver'"
    exit 1
fi

