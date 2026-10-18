#!/usr/bin/env bash
set -euo pipefail
dotnet build tests/Adapter/Adapter.csproj -c Release
dotnet run --project tests/BindingTest/BindingTest.csproj -c Release
dotnet pack src/Licensebox/Licensebox.csproj -c Release -o "$PWD/packages"
dotnet restore tests/PackageConsumer/PackageConsumer.csproj --source "$PWD/packages"
dotnet run --no-restore --project tests/PackageConsumer/PackageConsumer.csproj -c Release
