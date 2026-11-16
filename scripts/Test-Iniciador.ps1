param([Parameter(Mandatory = $true)][string]$PackagePath)
$ErrorActionPreference = 'Stop'
$package = (Resolve-Path -LiteralPath $PackagePath).Path
$launcher = Join-Path $package 'scripts/Iniciar-HoraDaFesta.ps1'
$session = Join-Path $package 'logs/session.json'
& $launcher -CheckOnly
& $launcher -Stop
try {
    & $launcher -NoBrowser -NoGame -NoLive
    $first = Get-Content -LiteralPath $session -Raw | ConvertFrom-Json
    if (!$first.bridge.id) { throw 'O iniciador nao registrou o servidor.' }
    & $launcher -NoBrowser -NoGame -NoLive
    $second = Get-Content -LiteralPath $session -Raw | ConvertFrom-Json
    if ($first.bridge.id -ne $second.bridge.id) { throw 'A segunda abertura duplicou o servidor.' }
    if (!(Get-Process -Id $first.bridge.id -ErrorAction SilentlyContinue)) { throw 'O servidor encerrou inesperadamente.' }
} finally { & $launcher -Stop }
if (Test-Path -LiteralPath $session) { throw 'A sessao de processos nao foi limpa ao encerrar.' }
Write-Output 'TESTE APROVADO: verificacao, inicio, segunda abertura sem duplicacao e encerramento.'
