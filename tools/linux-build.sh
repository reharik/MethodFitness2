#!/usr/bin/env bash
# Compiles the solution on Linux. Needs: the dotnet SDK, python3.
# Checks that the C# compiles; it does not compile the Razor views or run the site.
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
python3 "$root/tools/linux-restore.py"
# src/Directory.Build.targets tells the build where the reference assemblies are
dotnet build "$root/src/MethodFitness.sln" -nologo -v:minimal "$@"
