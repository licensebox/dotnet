#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
exec dotnet tests/VisualBasic/bin/Release/net10.0/VisualBasic.dll
