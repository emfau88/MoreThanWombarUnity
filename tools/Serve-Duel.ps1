param([int]$Port = 8765)
$ErrorActionPreference = 'Stop'
$duelDirectory = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\Builds\B3c\WebGL'))
if (-not (Test-Path -LiteralPath (Join-Path $duelDirectory 'index.html') -PathType Leaf)) {
    throw 'Create the WebGL duel build first: Unity menu Wombat Lab / B3c Build WebGL.'
}
Write-Host "Duell: http://127.0.0.1:$Port — stop with Ctrl+C"
& python -m http.server $Port --bind 127.0.0.1 --directory $duelDirectory
