param([int]$Port = 8765, [switch]$Lan, [ValidateSet('B3c','B8')][string]$Build = 'B3c')
$ErrorActionPreference = 'Stop'
$duelDirectory = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\Builds\$Build\WebGL"))
if (-not (Test-Path -LiteralPath (Join-Path $duelDirectory 'index.html') -PathType Leaf)) {
    throw 'Create the WebGL duel build first: Unity menu Wombat Lab / B3c Build WebGL.'
}
Write-Host "Duell: http://127.0.0.1:$Port — stop with Ctrl+C"
$duelBindAddress = if ($Lan) { '0.0.0.0' } else { '127.0.0.1' }
if ($Lan) { Write-Host "Handy im selben WLAN: http://<IPv4-Adresse dieses PCs>:$Port" }
& python -m http.server $Port --bind $duelBindAddress --directory $duelDirectory
