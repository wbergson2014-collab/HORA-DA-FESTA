param(
    [string]$UnityEditor = 'C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Unity.exe',
    [string]$ProjectPath = (Join-Path $PSScriptRoot '../unity-game/HORA-DA-FESTA-UNITY'),
    [string]$Destination = (Join-Path $PSScriptRoot '../dist/HORA-DA-FESTA-PRONTO/Jogo')
)
$ErrorActionPreference = 'Stop'
if (!(Test-Path -LiteralPath $UnityEditor)) { throw 'Unity Editor nao encontrado. Informe -UnityEditor com o caminho instalado.' }
$project = (Resolve-Path -LiteralPath $ProjectPath).Path
$output = [IO.Path]::GetFullPath($Destination)
New-Item -ItemType Directory -Force -Path $output | Out-Null
$log = Join-Path $output 'build.log'
$previousOutput = $env:HDF_BUILD_OUTPUT
try {
    $env:HDF_BUILD_OUTPUT = Join-Path $output 'HoraDaFesta.exe'
    $arguments = @('-batchmode', '-force-d3d11', '-quit', '-projectPath', ('"' + $project + '"'), '-executeMethod', 'HoraDaFesta.Editor.FestaBuild.Windows', '-logFile', ('"' + $log + '"'))
    $process = Start-Process -FilePath $UnityEditor -ArgumentList $arguments -WindowStyle Hidden -Wait -PassThru
    if ($process.ExitCode -ne 0) { throw "Falha no build. Consulte $log" }
    if (!(Test-Path -LiteralPath $env:HDF_BUILD_OUTPUT)) { throw 'O build nao gerou o executavel esperado.' }
    Write-Output "Jogo Windows pronto em $output"
} finally { $env:HDF_BUILD_OUTPUT = $previousOutput }
