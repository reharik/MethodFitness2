#!/usr/bin/env bash
# Compiles the solution on Linux. Needs: the dotnet SDK, python3.
# Checks that the C# compiles; it does not compile the Razor views or run the site.
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
python3 "$root/tools/linux-restore.py"
refs="$root/src/packages/Microsoft.NETFramework.ReferenceAssemblies.net48.1.0.3/build/"
dotnet msbuild "$root/src/MethodFitness.sln" -nologo -v:minimal -m \
  -p:Configuration=Release \
  -p:TargetFrameworkRootPath="$refs" \
  "$@"
