param([switch]$CheckOnly, [switch]$Stop, [switch]$NoBrowser, [switch]$NoGame, [switch]$NoLive)
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$package = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$logs = Join-Path $package 'logs'
$sessionPath = Join-Path $logs 'session.json'
$node = Join-Path $package 'runtime/node.exe'
$game = Join-Path $package 'Jogo/HoraDaFesta.exe'
$server = Join-Path $package 'tiktok-integration/server.js'
$panel = Join-Path $package 'painel-admin/index.html'
$state = @{}
$launchMutex = $null
$mutexHeld = $false
function Show-Problem([string]$message) {
    if ($CheckOnly) { throw $message }
    Add-Type -AssemblyName System.Windows.Forms
    [Windows.Forms.MessageBox]::Show($message, 'HORA DA FESTA', 'OK', 'Error') | Out-Null
}
function Get-BridgeStatus {
    $client = [Net.WebSockets.ClientWebSocket]::new()
    $timeout = [Threading.CancellationTokenSource]::new(1500)
    try {
        $client.ConnectAsync([Uri]'ws://127.0.0.1:8765', $timeout.Token).GetAwaiter().GetResult() | Out-Null
        $buffer = New-Object byte[] 8192
        $result = $client.ReceiveAsync([ArraySegment[byte]]::new($buffer), $timeout.Token).GetAwaiter().GetResult()
        if ($result.MessageType -ne [Net.WebSockets.WebSocketMessageType]::Text) { return $null }
        return ([Text.Encoding]::UTF8.GetString($buffer, 0, $result.Count) | ConvertFrom-Json)
    } catch { return $null } finally { $client.Abort(); $client.Dispose(); $timeout.Dispose() }
}
function Connect-ExistingLive {
    try {
        $source = Invoke-RestMethod -Uri 'http://127.0.0.1:8781/roblox/events' -TimeoutSec 2
        $username = ([string]$source.username).TrimStart('@')
        if (!$source.live -or $username -notmatch '^[a-zA-Z0-9._]{2,30}$') { return }
        $client = [Net.WebSockets.ClientWebSocket]::new()
        $timeout = [Threading.CancellationTokenSource]::new(2500)
        try {
            $client.ConnectAsync([Uri]'ws://127.0.0.1:8765', $timeout.Token).GetAwaiter().GetResult() | Out-Null
            $buffer = New-Object byte[] 8192
            $client.ReceiveAsync([ArraySegment[byte]]::new($buffer), $timeout.Token).GetAwaiter().GetResult() | Out-Null
            $json = @{ action = 'tiktok-connect'; username = $username } | ConvertTo-Json -Compress
            $bytes = [Text.Encoding]::UTF8.GetBytes($json)
            $client.SendAsync([ArraySegment[byte]]::new($bytes), [Net.WebSockets.WebSocketMessageType]::Text, $true, $timeout.Token).GetAwaiter().GetResult() | Out-Null
            $client.ReceiveAsync([ArraySegment[byte]]::new($buffer), $timeout.Token).GetAwaiter().GetResult() | Out-Null
        } finally { $client.Abort(); $client.Dispose(); $timeout.Dispose() }
    } catch { Write-Output 'Simulacao pronta. A LIVE pode ser conectada quando o coletor 8781 estiver online.' }
}
function Test-Port {
    $client = [Net.Sockets.TcpClient]::new()
    try { return $client.ConnectAsync('127.0.0.1', 8765).Wait(500) -and $client.Connected }
    catch { return $false } finally { $client.Dispose() }
}
function Record-Process($process) {
    $process.Refresh()
    return @{ id = $process.Id; started = $process.StartTime.ToUniversalTime().Ticks.ToString(); path = $process.Path }
}
function Stop-Owned($record) {
    if (!$record) { return }
    $process = Get-Process -Id $record.id -ErrorAction SilentlyContinue
    if (!$process) { return }
    if ($process.Path -ne $record.path -or $process.StartTime.ToUniversalTime().Ticks.ToString() -ne $record.started) { return }
    if ($process.MainWindowHandle -ne 0) {
        $process.CloseMainWindow() | Out-Null
        if ($process.WaitForExit(5000)) { return }
    }
    if (!$process.HasExited) { Stop-Process -InputObject $process }
}
try {
    $hash = [Security.Cryptography.SHA256]::Create()
    $key = [BitConverter]::ToString($hash.ComputeHash([Text.Encoding]::UTF8.GetBytes($package.ToLowerInvariant()))).Replace('-', '').Substring(0, 16)
    $hash.Dispose()
    $launchMutex = [Threading.Mutex]::new($false, ('Local\HoraDaFesta-' + $key))
    $mutexHeld = $launchMutex.WaitOne(15000)
    if (!$mutexHeld) { throw 'O iniciador ainda esta ocupado. Aguarde alguns segundos e clique novamente.' }
    if (Test-Path -LiteralPath $sessionPath) {
        $saved = Get-Content -LiteralPath $sessionPath -Raw | ConvertFrom-Json
        foreach ($entry in $saved.PSObject.Properties) { $state[$entry.Name] = $entry.Value }
    }
    if ($Stop) {
        Stop-Owned $state.game
        Stop-Owned $state.bridge
        if (Test-Path -LiteralPath $sessionPath) { Remove-Item -LiteralPath $sessionPath }
        Write-Output 'Jogo e servidor deste iniciador encerrados. O coletor LIVE 8781 foi preservado.'
        return
    }
    if (!(Test-Path -LiteralPath $node)) {
        $installed = Get-Command node.exe -ErrorAction SilentlyContinue
        if ($installed) { $node = $installed.Source }
    }
    if (!(Test-Path -LiteralPath $game)) { $game = Join-Path $package 'unity-game/HORA-DA-FESTA-UNITY/Builds/Windows/HoraDaFesta.exe' }
    foreach ($required in @($node, $server, $panel, (Join-Path $package 'tiktok-integration/node_modules/ws/package.json'))) {
        if (!(Test-Path -LiteralPath $required)) { throw "Arquivo necessario ausente: $required. Use o pacote HORA-DA-FESTA-PRONTO completo." }
    }
    if (!$NoGame -and !(Test-Path -LiteralPath $game)) { throw 'O jogo Windows nao foi encontrado. Use o pacote PRONTO ou gere o jogo pelo menu HORA DA FESTA > Criar jogo Windows no Unity.' }
    if ($CheckOnly) { Write-Output 'Tudo pronto: jogo, servidor, painel e Node conferidos.'; return }
    New-Item -ItemType Directory -Force -Path $logs | Out-Null
    $status = Get-BridgeStatus
    if (!$status) {
        if (Test-Port) { throw 'A porta 8765 esta ocupada por outro programa. Feche o servidor antigo de HORA DA FESTA e clique novamente. Nao feche o coletor LIVE 8781.' }
        $state.bridge = Record-Process (Start-Process -FilePath $node -ArgumentList @('"' + $server + '"') -WorkingDirectory (Split-Path $server) -WindowStyle Hidden -RedirectStandardOutput (Join-Path $logs 'servidor.log') -RedirectStandardError (Join-Path $logs 'servidor-erros.log') -PassThru)
        $state | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $sessionPath -Encoding UTF8
        for ($attempt = 0; $attempt -lt 20 -and !$status; $attempt++) { Start-Sleep -Milliseconds 250; $status = Get-BridgeStatus }
    }
    if (!$status -or $status.bridge -ne 'hora-da-festa' -or $status.protocolVersion -ne 1) { throw 'O servidor na porta 8765 nao e a versao compativel. Feche o servidor antigo e clique novamente. O coletor 8781 deve permanecer aberto.' }
    if (!$NoLive -and $status.tiktok -ne 'connected') { Connect-ExistingLive }
    $gameAlreadyRunning = $false
    if (!$NoGame) {
        $existing = Get-Process -Name HoraDaFesta -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $game } | Select-Object -First 1
        $gameAlreadyRunning = [bool]$existing
        if (!$existing) {
            $gameArgs = @('-screen-fullscreen', '0', '-screen-width', '1280', '-screen-height', '720', '-logFile', ('"' + (Join-Path $logs 'jogo.log') + '"'))
            $existing = Start-Process -FilePath $game -ArgumentList $gameArgs -WorkingDirectory (Split-Path $game) -WindowStyle Normal -RedirectStandardOutput (Join-Path $logs 'jogo-console.log') -RedirectStandardError (Join-Path $logs 'jogo-erros.log') -PassThru
            Start-Sleep -Milliseconds 500
            if ($existing.HasExited) { throw 'O jogo fechou ao iniciar. Consulte logs/jogo.log e logs/jogo-erros.log no pacote.' }
        }
        $state.game = Record-Process $existing
        $state | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $sessionPath -Encoding UTF8
    }
    if (!$NoBrowser -and (!$gameAlreadyRunning -or !$state.panelOpened)) {
        Start-Process -FilePath $panel
        $state.panelOpened = $true
        $state | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $sessionPath -Encoding UTF8
    }
    Write-Output 'HORA DA FESTA iniciado. Use o painel para adicionar espectadores e presentes.'
} catch {
    if (Test-Path -LiteralPath $logs) { $_.Exception.Message | Add-Content -LiteralPath (Join-Path $logs 'iniciador-erros.log') }
    Show-Problem $_.Exception.Message
    exit 1
} finally {
    if ($mutexHeld) { $launchMutex.ReleaseMutex() }
    if ($launchMutex) { $launchMutex.Dispose() }
}
