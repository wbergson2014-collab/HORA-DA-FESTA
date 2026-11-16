param(
 [string]$Destination = 'D:\PROJETOS GPT\HORA DA FESTA\HORA-DA-FESTA-UNITY',
 [switch]$Apply
)
$ErrorActionPreference = 'Stop'
$source = Join-Path $PSScriptRoot '../unity-game/HORA-DA-FESTA-UNITY/Assets/HoraDaFesta'
$destinationRoot = (Resolve-Path -LiteralPath $Destination).Path
$versionPath = Join-Path $destinationRoot 'ProjectSettings/ProjectVersion.txt'
if (!(Test-Path -LiteralPath $versionPath)) { throw 'Destino nao e um projeto Unity.' }
$manifest = Get-Content -LiteralPath (Join-Path $destinationRoot 'Packages/manifest.json') -Raw | ConvertFrom-Json
if (!$manifest.dependencies.'com.unity.render-pipelines.universal') { throw 'Instale o URP antes de sincronizar.' }
$target = Join-Path $destinationRoot 'Assets/HoraDaFesta'
if (Test-Path -LiteralPath $target) { throw 'Assets/HoraDaFesta ja existe. Nenhum arquivo foi sobrescrito. Compare/mescle manualmente em uma copia do projeto.' }
Write-Output "Origem: $source"
Write-Output "Destino novo: $target"
Write-Output 'Somente Runtime e Editor serao copiados. Cenas, Packages e ProjectSettings locais serao preservados.'
if (!$Apply) { Write-Output 'Previa concluida. Use -Apply para copiar apos fazer backup e fechar o Unity.'; return }
New-Item -ItemType Directory -Path $target | Out-Null
foreach ($folder in @('Runtime','Editor')) {
 Copy-Item -LiteralPath (Join-Path $source $folder) -Destination $target -Recurse
 $meta = Join-Path $source ($folder + '.meta')
 if (Test-Path -LiteralPath $meta) { Copy-Item -LiteralPath $meta -Destination $target }
}
Write-Output 'Concluido. Abra o projeto e use HORA DA FESTA > Gerar cena Arraial.'
