param(
    [Parameter(Mandatory = $true)][ValidatePattern('^[a-zA-Z0-9][a-zA-Z0-9._-]+$')][string]$Tag
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$buildRoot = Join-Path $projectRoot 'Builds/B3c'
$webRoot = Join-Path $buildRoot 'WebGL'
$report = Get-Content (Join-Path $buildRoot 'WebGLBuildStatus.json') -Raw | ConvertFrom-Json
if ($report.status -ne 'Succeeded') { throw 'Create a successful WebGL build before publishing.' }
if (!(Test-Path (Join-Path $webRoot 'index.html') -PathType Leaf)) { throw 'WebGL index.html missing.' }
if ((Get-Item (Join-Path $webRoot 'index.html')).LastWriteTimeUtc -lt [DateTime]::Parse($report.utc).ToUniversalTime()) { throw 'WebGL page is older than the build report.' }
$pending = & git -C $projectRoot status --porcelain
if ($LASTEXITCODE -ne 0 -or $pending) { throw 'Commit the game and publishing files before publishing.' }
$revision = & git -C $projectRoot rev-parse HEAD
@{ commit = $revision; buildUtc = $report.utc; release = $Tag } | ConvertTo-Json | Set-Content (Join-Path $webRoot 'version.json') -Encoding utf8
$archive = Join-Path $buildRoot 'webgl-duel.zip'
Compress-Archive -Path (Join-Path $webRoot '*') -DestinationPath $archive -Force
$archiveBytes = [IO.File]::ReadAllBytes($archive)
$partSize = 5MB
$partPaths = @()
for ($offset = 0; $offset -lt $archiveBytes.Length; $offset += $partSize) {
    $partLength = [Math]::Min($partSize, $archiveBytes.Length - $offset)
    $partBytes = New-Object byte[] $partLength
    [Array]::Copy($archiveBytes, $offset, $partBytes, 0, $partLength)
    $partPath = $archive + ('.part-{0:d3}' -f [int]($offset / $partSize))
    [IO.File]::WriteAllBytes($partPath, $partBytes)
    $partPaths += $partPath
}
$repo = 'emfau88/MoreThanWombarUnity'
& gh release create $Tag @partPaths --repo $repo --target $revision --draft --title "Schrotthof-Duell / $Tag" --notes 'Spielbarer WebGL-Prototyp mit Touch-Steuerung, Tastatur und Gamepad. Android im Querformat. Veröffentlichung über GitHub Pages.'
if ($LASTEXITCODE -ne 0) { throw 'Release upload failed; inspect the draft before retrying.' }
& gh release edit $Tag --repo $repo --draft=false
if ($LASTEXITCODE -ne 0) { throw 'Publishing the uploaded draft failed.' }
& gh workflow run play.yml --repo $repo --ref main -f "release_tag=$Tag"
if ($LASTEXITCODE -ne 0) { throw 'Release uploaded; start the Pages workflow manually with this release tag.' }
Write-Output 'Release published and Pages workflow started.'
