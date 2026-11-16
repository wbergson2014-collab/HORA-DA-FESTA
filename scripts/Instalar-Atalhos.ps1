param([string]$PackagePath = (Join-Path $PSScriptRoot '..'))
$ErrorActionPreference = 'Stop'
$package = (Resolve-Path -LiteralPath $PackagePath).Path
$launcher = Join-Path $package 'scripts/Iniciar-HoraDaFesta.ps1'
$game = Join-Path $package 'Jogo/HoraDaFesta.exe'
if (!(Test-Path -LiteralPath $launcher) -or !(Test-Path -LiteralPath $game)) { throw 'Use o pacote PRONTO completo para criar os atalhos.' }
$desktop = [Environment]::GetFolderPath('Desktop')
$shell = New-Object -ComObject WScript.Shell
foreach ($entry in @(
    @{ Name = 'HORA DA FESTA - Iniciar Unity.lnk'; Extra = ''; Description = 'Abre o jogo Unity, o painel e o servidor local.' },
    @{ Name = 'HORA DA FESTA - Encerrar Unity.lnk'; Extra = ' -Stop'; Description = 'Encerra o jogo e o servidor do iniciador. Preserva o coletor TikTok.' }
)) {
    $file = Join-Path $desktop $entry.Name
    $shortcut = $shell.CreateShortcut($file)
    if ((Test-Path -LiteralPath $file) -and $shortcut.Arguments -notlike ('*' + $launcher + '*')) { throw "Ja existe um atalho diferente com este nome: $file" }
    $shortcut.TargetPath = Join-Path ([Environment]::GetFolderPath('Windows')) 'System32/WindowsPowerShell/v1.0/powershell.exe'
    $shortcut.Arguments = '-NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File "' + $launcher + '"' + $entry.Extra
    $shortcut.WorkingDirectory = $package
    $shortcut.IconLocation = $game + ',0'
    $shortcut.Description = $entry.Description
    $shortcut.Save()
    Write-Output "Atalho pronto: $file"
}
