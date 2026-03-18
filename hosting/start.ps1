$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$EnvFile = Join-Path $ScriptDir ".env"

if (-not (Test-Path $EnvFile)) {
    Copy-Item (Join-Path $ScriptDir ".env.prod.example") $EnvFile

    $JwtKey = [Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }) -as [byte[]])
    $PostgresPassword = [Convert]::ToBase64String((1..24 | ForEach-Object { Get-Random -Maximum 256 }) -as [byte[]])

    (Get-Content $EnvFile) `
        -replace 'changeme-minimum-32-characters-long', $JwtKey `
        -replace 'POSTGRES_PASSWORD=changeme', "POSTGRES_PASSWORD=$PostgresPassword" |
        Set-Content $EnvFile

    Write-Host "Created $EnvFile with generated secrets."
    Write-Host "Edit APP_DOMAIN and ACME_EMAIL before starting with TLS."
}

Set-Location $ScriptDir

$ComposeFiles = @("-f", "docker-compose.yaml")

$EnvVars = @{}
Get-Content $EnvFile | ForEach-Object {
    if ($_ -match '^\s*([^#][^=]+)=(.*)$') {
        $EnvVars[$Matches[1].Trim()] = $Matches[2].Trim()
    }
}

if ($EnvVars["ACME_EMAIL"] -and $EnvVars["ACME_EMAIL"] -ne "admin@example.com") {
    $ComposeFiles += @("-f", "docker-compose.tls.yaml")
    Write-Host "TLS enabled (ACME_EMAIL=$($EnvVars["ACME_EMAIL"]))."
} else {
    Write-Host "TLS disabled (ACME_EMAIL not set). Starting HTTP only."
}

podman compose @ComposeFiles up -d --build
