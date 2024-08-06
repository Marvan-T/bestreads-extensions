#!/bin/bash

# Ensure the script fails on any error
set -e

# Navigate to the project directory
cd BestReads

# User secrets if not already done
dotnet user-secrets init

echo "Clearing existing secrets..."
dotnet user-secrets clear

echo "Setting new secrets"
dotnet user-secrets set "NYTimes:ApiKey" "$NYTIMES_API_KEY"
dotnet user-secrets set "Open_AI_Key" "$Open_AI_Key"
dotnet user-secrets set "AZURE_SEARCH_SERVICE_ENDPOINT" "$AZURE_SEARCH_SERVICE_ENDPOINT"
dotnet user-secrets set "AZURE_SEARCH_INDEX_NAME" "$AZURE_SEARCH_INDEX_NAME"
dotnet user-secrets set "AZURE_SEARCH_ADMIN_KEY" "$AZURE_SEARCH_ADMIN_KEY"
dotnet user-secrets set "DEFAULT_THUMBNAIL_URL" "$DEFAULT_THUMBNAIL_URL"

echo "Secrets have been set up successfully."