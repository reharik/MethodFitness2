<#
.SYNOPSIS
  Deploys the latest GitHub build of MF.Web to the QA or PROD site on this server.

.DESCRIPTION
  Downloads the build published by .github/workflows/build.yml (the rolling
  "qa-latest" prerelease), backs up the current site folder, takes the site
  offline for the few seconds the copy needs, and copies the build over it.

  The site's own Web.config is never touched: the build does not contain one.
  Files that exist on the server but not in the build are left alone, so
  anything uploaded or hand-edited on the server survives a deploy.

.EXAMPLE
  .\Deploy-MethodFit.ps1 -Target QA
  .\Deploy-MethodFit.ps1 -Target QA -Build qa-feature-session-completion   # a branch's build
  .\Deploy-MethodFit.ps1 -Target PROD -Compare     # show differences, change nothing
  .\Deploy-MethodFit.ps1 -Target PROD
#>
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('QA', 'PROD')]
    [string]$Target,

    # Which published build to deploy. "qa-latest" is the production branch's newest
    # build; a branch build is published as "qa-<branch>", e.g. qa-feature-session-completion.
    [string]$Build = 'qa-latest',

    # Overrides -Build with a full download address.
    [string]$Url,

    # List what differs between the live site and the build, then stop.
    [switch]$Compare,

    [int]$KeepBackups = 5
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

if (-not $Url) {
    if ($Build -notmatch '^[A-Za-z0-9._-]+$') { throw "Unexpected build name: $Build" }
    $Url = "https://github.com/reharik/MethodFitness2/releases/download/$Build/MF.Web.zip"
}
if ($Target -eq 'PROD' -and $Build -ne 'qa-latest' -and -not $Compare) {
    Write-Warning "Deploying a branch build ($Build) to PROD, not the production branch's build."
}

$root    = 'C:\MethodFitness'
$site    = Join-Path $root "WebSites\$Target\csharpMethodFit"
$work    = Join-Path $root 'Deploy'
$backups = Join-Path $root 'Backups'
$zip     = Join-Path $work 'MF.Web.zip'
$new     = Join-Path $work 'MF.Web'

if (-not (Test-Path (Join-Path $site 'Web.config'))) {
    throw "No Web.config in $site - is this the right site folder?"
}

# --- fetch and unpack the build -------------------------------------------
New-Item -ItemType Directory -Force $work | Out-Null
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Write-Host "Downloading $Url"
Invoke-WebRequest $Url -OutFile $zip -UseBasicParsing
if (Test-Path $new) { Remove-Item $new -Recurse -Force }
Expand-Archive $zip -DestinationPath $new
if (-not (Test-Path (Join-Path $new 'bin\MF.Web.dll'))) { throw 'The download does not look like an MF.Web build (no bin\MF.Web.dll).' }
if (Test-Path (Join-Path $new 'Web.config')) { throw 'The build contains a root Web.config; refusing to overwrite the server''s.' }
$count = (Get-ChildItem $new -Recurse -File).Count
Write-Host "Build unpacked: $count files"

# --- compare only ----------------------------------------------------------
if ($Compare) {
    $textExt = '.cshtml', '.js', '.css', '.htm', '.html', '.config', '.asax', '.aspx', '.xml', '.txt', '.json', '.rdlc', '.xsd'
    function Get-Signature([string]$path) {
        if ($textExt -contains [IO.Path]::GetExtension($path).ToLower()) {
            # ignore line-ending differences between the old and new build machines
            $text = [IO.File]::ReadAllText($path).Replace("`r", '')
            $bytes = [Text.Encoding]::UTF8.GetBytes($text)
            $sha = [Security.Cryptography.SHA256]::Create()
            return [BitConverter]::ToString($sha.ComputeHash($bytes))
        }
        return (Get-FileHash $path -Algorithm SHA256).Hash
    }
    function Get-RelativeFiles([string]$base) {
        Get-ChildItem $base -Recurse -File | ForEach-Object { $_.FullName.Substring($base.Length + 1) }
    }
    $buildFiles = Get-RelativeFiles $new
    $siteFiles  = Get-RelativeFiles $site | Where-Object { $_ -ne 'Web.config' -and $_ -ne 'app_offline.htm' }
    $rows = @()
    foreach ($f in $buildFiles) {
        if ($f -like 'bin\*') { continue }   # binaries always differ between builds
        $live = Join-Path $site $f
        if (-not (Test-Path $live)) { $rows += [pscustomobject]@{ Status = 'new in build'; File = $f } }
        elseif ((Get-Signature $live) -ne (Get-Signature (Join-Path $new $f))) { $rows += [pscustomobject]@{ Status = 'DIFFERENT'; File = $f } }
    }
    foreach ($f in $siteFiles) {
        if ($f -like 'bin\*') { continue }
        if ($buildFiles -notcontains $f) { $rows += [pscustomobject]@{ Status = 'only on server'; File = $f } }
    }
    if ($rows.Count -eq 0) { Write-Host "No differences outside bin\ between $Target and the build." }
    else {
        $rows | Sort-Object Status, File | Format-Table -AutoSize
        $rows | Group-Object Status | ForEach-Object { '{0,5}  {1}' -f $_.Count, $_.Name }
    }
    Write-Host 'Compare only - nothing was changed.'
    return
}

# --- back up the current site ---------------------------------------------
$stamp  = Get-Date -Format 'yyyy-MM-dd_HHmmss'
$backup = Join-Path $backups "${Target}_$stamp"
Write-Host "Backing up $site -> $backup"
robocopy $site $backup /E /NFL /NDL /NJH /NJS /NP /R:2 /W:2 | Out-Null
if ($LASTEXITCODE -ge 8) { throw "Backup failed (robocopy exit $LASTEXITCODE). Nothing was deployed." }

# --- deploy ----------------------------------------------------------------
$offline = Join-Path $site 'app_offline.htm'
try {
    # IIS unloads the app while this file exists, which releases the DLLs in bin.
    Set-Content $offline '<html><body style="font-family:sans-serif">Updating - back in a minute.</body></html>'
    Start-Sleep -Seconds 4
    Write-Host "Copying build -> $site"
    robocopy $new $site /E /NFL /NDL /NJH /NP /R:5 /W:2
    if ($LASTEXITCODE -ge 8) { throw "Copy failed (robocopy exit $LASTEXITCODE). Restore with: robocopy `"$backup`" `"$site`" /E" }
}
finally {
    Remove-Item $offline -Force -ErrorAction SilentlyContinue
}

# --- prune old backups ------------------------------------------------------
Get-ChildItem $backups -Directory -Filter "${Target}_*" | Sort-Object Name -Descending |
    Select-Object -Skip $KeepBackups | ForEach-Object { Remove-Item $_.FullName -Recurse -Force }

Write-Host ''
Write-Host "Deployed to $Target."
Write-Host "To roll back:  robocopy `"$backup`" `"$site`" /E"
$global:LASTEXITCODE = 0
