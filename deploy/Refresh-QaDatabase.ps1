<#
.SYNOPSIS
  Replaces the QA database with a fresh copy of production.

.DESCRIPTION
  Backs up the source database to a file (copy-only, so it does not disturb any
  other backup routine), then restores that file over the target database.
  The source is only read. The backup file is kept, so every run also leaves
  you with a dated backup of production.

.EXAMPLE
  .\Refresh-QaDatabase.ps1
#>
param(
    [string]$Server = 'localhost',
    [string]$Source = 'MethodFitness_PROD',
    [string]$Target = 'MethodFitness_QA',
    [string]$BackupDir
)

$ErrorActionPreference = 'Stop'

if ($Target -eq $Source) { throw 'Source and target are the same database.' }
if ($Target -match 'PROD') { throw "Refusing to overwrite a database with PROD in its name ($Target)." }
foreach ($n in $Source, $Target) {
    if ($n -notmatch '^[A-Za-z0-9_]+$') { throw "Unexpected database name: $n" }
}

$conn = New-Object System.Data.SqlClient.SqlConnection "Server=$Server;Database=master;Integrated Security=True"
$conn.Open()

function Invoke-Query([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 0
    $cmd.CommandText = $sql
    $table = New-Object System.Data.DataTable
    $reader = $cmd.ExecuteReader()
    $table.Load($reader)
    $reader.Close()
    return @($table.Rows)
}

function Invoke-Statement([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandTimeout = 0
    $cmd.CommandText = $sql
    [void]$cmd.ExecuteNonQuery()
}

try {
    $existing = Invoke-Query "select name from sys.databases where name in ('$Source', '$Target')" | ForEach-Object { $_.name }
    if ($existing -notcontains $Source) { throw "Source database $Source not found on $Server." }
    if ($existing -notcontains $Target) { throw "Target database $Target not found on $Server. Create it once (empty) and run this again." }

    if (-not $BackupDir) {
        $BackupDir = (Invoke-Query "select cast(serverproperty('InstanceDefaultBackupPath') as nvarchar(260)) as p" | Select-Object -First 1).p
    }
    if (-not $BackupDir -or $BackupDir -is [DBNull]) { throw 'Could not find the default backup folder; pass -BackupDir.' }
    $file = Join-Path $BackupDir ("{0}_{1}.bak" -f $Source, (Get-Date -Format 'yyyy-MM-dd_HHmmss'))

    Write-Host "Backing up $Source -> $file"
    Invoke-Statement "backup database [$Source] to disk = '$file' with copy_only, init, checksum"

    # Restore each file in the backup onto the matching file of the existing target,
    # so the copy lands where the target already lives.
    $inBackup = Invoke-Query "restore filelistonly from disk = '$file'"
    $onTarget = Invoke-Query "select type_desc, physical_name from sys.master_files where database_id = db_id('$Target') order by file_id"
    $moves = @()
    foreach ($kind in @(@('D', 'ROWS'), @('L', 'LOG'))) {
        $from = @($inBackup | Where-Object { $_.Type -eq $kind[0] })
        $to   = @($onTarget | Where-Object { $_.type_desc -eq $kind[1] })
        if ($from.Count -ne $to.Count) { throw "The backup has $($from.Count) $($kind[1]) file(s) but $Target has $($to.Count); restore by hand." }
        for ($i = 0; $i -lt $from.Count; $i++) {
            $moves += "move '$($from[$i].LogicalName)' to '$($to[$i].physical_name)'"
        }
    }

    Write-Host "Restoring over $Target"
    Invoke-Statement "alter database [$Target] set single_user with rollback immediate"
    try {
        Invoke-Statement ("restore database [$Target] from disk = '$file' with replace, " + ($moves -join ', '))
    }
    finally {
        Invoke-Statement "if databasepropertyex('$Target', 'Status') = 'ONLINE' alter database [$Target] set multi_user"
    }

    $state = Invoke-Query "select state_desc, user_access_desc from sys.databases where name = '$Target'" | Select-Object -First 1
    Write-Host ''
    Write-Host "$Target is now a copy of $Source ($($state.state_desc), $($state.user_access_desc))."
    Write-Host "Backup kept at: $file"
}
finally {
    $conn.Close()
}
