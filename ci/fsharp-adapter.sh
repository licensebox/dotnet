#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
exec dotnet tests/FSharp/bin/Release/net10.0/FSharp.dll
