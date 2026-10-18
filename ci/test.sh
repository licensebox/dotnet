#!/usr/bin/env bash
set -euo pipefail
dotnet build tests/Adapter/Adapter.csproj -c Release
dotnet run --project tests/BindingTest/BindingTest.csproj -c Release
dotnet pack src/Licensebox/Licensebox.csproj -c Release -o "$PWD/packages"
dotnet restore tests/PackageConsumer/PackageConsumer.csproj --source "$PWD/packages"
dotnet run --no-restore --project tests/PackageConsumer/PackageConsumer.csproj -c Release
dotnet build tests/FSharp/FSharp.fsproj -c Release
python3 ../protocol/conformance/run.py --adapter bash ci/fsharp-adapter.sh
python3 ../protocol/conformance/heartbeat.py --adapter bash ci/fsharp-adapter.sh
dotnet build tests/VisualBasic/VisualBasic.vbproj -c Release
python3 ../protocol/conformance/run.py --adapter bash ci/visualbasic-adapter.sh
python3 ../protocol/conformance/heartbeat.py --adapter bash ci/visualbasic-adapter.sh
