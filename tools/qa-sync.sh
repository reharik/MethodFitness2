#!/usr/bin/env bash
# Build here, put the result on the QA site, in well under a minute.
#
#   tools/qa-sync.sh            build, then send the DLLs and any view/script/css
#                               file changed since the last sync
#   tools/qa-sync.sh --all      send every view, script and stylesheet too
#   tools/qa-sync.sh --dry-run  build and show what would be sent; send nothing
#
# For trying things while you work. It is not a deploy:
#   * QA only. The site path is fixed below; PROD is only ever deployed from a
#     build GitHub made.
#   * Whatever it puts on QA is replaced by the next pipeline deploy (any push).
#   * Don't run it while the pipeline's browser tests are running against QA.
#
# Needs: dotnet SDK, python3, the aws CLI signed in to the account, and
# tools/qa-sync.env (see qa-sync.env.example).
set -euo pipefail

root="$(cd "$(dirname "$0")/.." && pwd)"
web="$root/src/MF.Web"
stamp="$root/.git/qa-sync-stamp"
site='C:\MethodFitness\WebSites\QA\csharpMethodFit'
mode="${1:-}"

if [ "$mode" != "--dry-run" ]; then
  [ -f "$root/tools/qa-sync.env" ] || { echo "tools/qa-sync.env is missing; copy tools/qa-sync.env.example and fill it in"; exit 1; }
  # shellcheck disable=SC1091
  . "$root/tools/qa-sync.env"
  : "${MF_INSTANCE_ID:?set MF_INSTANCE_ID in tools/qa-sync.env}"
  : "${MF_SYNC_BUCKET:?set MF_SYNC_BUCKET in tools/qa-sync.env}"
  region="${MF_REGION:-us-east-2}"
fi

echo "building..."
python3 "$root/tools/linux-restore.py" > /dev/null
dotnet build "$root/src/MethodFitness.sln" -nologo -v:quiet -clp:NoSummary

stage="$(mktemp -d)"; trap 'rm -rf "$stage"' EXIT
mkdir -p "$stage/bin"
for name in MF.Web MF.Core CC.Core; do
  cp "$web/bin/$name.dll" "$stage/bin/"
  [ -f "$web/bin/$name.pdb" ] && cp "$web/bin/$name.pdb" "$stage/bin/"
done

newer=()
if [ "$mode" != "--all" ] && [ -f "$stamp" ]; then newer=(-newer "$stamp"); fi
# views, scripts and styles only: never a .config file, so the server's settings stay its own
( cd "$web" && find Views Areas Content -type f \
    \( -name '*.cshtml' -o -name '*.js' -o -name '*.css' -o -name '*.html' -o -name '*.htm' \) \
    "${newer[@]}" -print0 | xargs -0 -r cp --parents -t "$stage" )

count=$(find "$stage" -type f | wc -l)
echo "sending $count files:"
( cd "$stage" && find . -type f | sed 's|^\./|  |' | sort | head -40 )
[ "$count" -gt 40 ] && echo "  ... and $((count - 40)) more"

zip="$stage/../qa-sync-$$.zip"
( cd "$stage" && python3 -c "
import os, sys, zipfile
with zipfile.ZipFile(sys.argv[1], 'w', zipfile.ZIP_DEFLATED) as z:
    for d, _, files in os.walk('.'):
        for f in files:
            p = os.path.join(d, f); z.write(p, os.path.relpath(p, '.'))
" "$zip" )
trap 'rm -rf "$stage" "$zip"' EXIT

if [ "$mode" = "--dry-run" ]; then echo "dry run: nothing sent ($(du -h "$zip" | cut -f1) zipped)"; exit 0; fi

# The server has no access to the bucket. It gets a link that works for five minutes.
key="qa-sync/$(date -u +%Y%m%dT%H%M%S)-$$.zip"
aws s3 cp --quiet --region "$region" "$zip" "s3://$MF_SYNC_BUCKET/$key"
url=$(aws s3 presign --region "$region" --expires-in 300 "s3://$MF_SYNC_BUCKET/$key")

params=$(python3 - "$url" "$site" <<'PY'
import json, sys
url, site = sys.argv[1], sys.argv[2]
print(json.dumps({"commands": [
    "$ErrorActionPreference = 'Stop'",
    "[Net.ServicePointManager]::SecurityProtocol = 'Tls12'",
    "$zip = Join-Path $env:TEMP 'qa-sync.zip'",
    f"Invoke-WebRequest -UseBasicParsing -Uri '{url}' -OutFile $zip",
    f"tar.exe -xf $zip -C '{site}'",
    "if ($LASTEXITCODE) { throw 'could not unpack onto the QA site' }",
    "Remove-Item $zip",
    "'on QA at ' + (Get-Date -Format 'HH:mm:ss')",
]}))
PY
)
id=$(aws ssm send-command --region "$region" --document-name AWS-RunPowerShellScript \
       --instance-ids "$MF_INSTANCE_ID" --comment "qa-sync from $(hostname)" \
       --parameters "$params" --query Command.CommandId --output text)
status=Pending
for _ in $(seq 1 60); do
  sleep 1
  status=$(aws ssm get-command-invocation --region "$region" --command-id "$id" --instance-id "$MF_INSTANCE_ID" --query Status --output text 2>/dev/null || echo Pending)
  case "$status" in Pending|InProgress|Delayed) ;; *) break ;; esac
done
aws ssm get-command-invocation --region "$region" --command-id "$id" --instance-id "$MF_INSTANCE_ID" --query StandardOutputContent --output text | tail -n 3
aws s3 rm --quiet --region "$region" "s3://$MF_SYNC_BUCKET/$key" || true
if [ "$status" != "Success" ]; then
  aws ssm get-command-invocation --region "$region" --command-id "$id" --instance-id "$MF_INSTANCE_ID" --query StandardErrorContent --output text | tail -n 15
  echo "sync failed: $status"; exit 1
fi
touch "$stamp"
