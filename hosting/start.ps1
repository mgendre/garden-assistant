$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$EnvFile = Join-Path $ScriptDir ".env"

if (-not (Test-Path $EnvFile)) {
    $AppDomain = Read-Host "Domain name (e.g. garden.example.com or localhost)"
    if (-not $AppDomain) { $AppDomain = "localhost" }

    Copy-Item (Join-Path $ScriptDir ".env.prod.example") $EnvFile

    $JwtKey = [Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }) -as [byte[]])
    $PostgresPassword = [Convert]::ToBase64String((1..24 | ForEach-Object { Get-Random -Maximum 256 }) -as [byte[]])

    (Get-Content $EnvFile) `
        -replace 'changeme-minimum-32-characters-long', $JwtKey `
        -replace 'POSTGRES_PASSWORD=changeme', "POSTGRES_PASSWORD=$PostgresPassword" `
        -replace 'APP_DOMAIN=garden.example.com', "APP_DOMAIN=$AppDomain" |
        Set-Content $EnvFile

    Write-Host "Created $EnvFile with generated secrets."
}

Set-Location $ScriptDir

$EnvVars = @{}
Get-Content $EnvFile | ForEach-Object {
    if ($_ -match '^\s*([^#][^=]+)=(.*)$') {
        $EnvVars[$Matches[1].Trim()] = $Matches[2].Trim()
    }
}

$ComposeFiles = @("-f", "docker-compose.yaml")
$Tls = $false

if ($EnvVars["ACME_EMAIL"] -and $EnvVars["ACME_EMAIL"] -ne "admin@example.com") {
    $ComposeFiles += @("-f", "docker-compose.tls.yaml")
    $Tls = $true
    Write-Host "TLS enabled (ACME_EMAIL=$($EnvVars["ACME_EMAIL"]))."
} else {
    Write-Host "Starting without TLS."
}

podman compose @ComposeFiles up -d --build

$Domain = $EnvVars["APP_DOMAIN"]
if ($Tls) {
    Write-Host "`nApplication available at: https://${Domain}:8443"
} else {
    Write-Host "`nApplication available at: http://${Domain}:8080"
}
