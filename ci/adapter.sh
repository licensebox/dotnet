#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
exec dotnet tests/Adapter/bin/Release/net10.0/Adapter.dll
