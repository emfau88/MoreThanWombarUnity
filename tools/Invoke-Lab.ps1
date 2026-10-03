param(
  [ValidateSet('Probe', 'Tests', 'PlayTests', 'TestStatus', 'Play', 'Stop', 'Open')]
  [string]$Action = 'Probe'
)

$ErrorActionPreference = 'Stop'
$labProjectPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\UnityProject'))
if (-not (Test-Path -LiteralPath (Join-Path $labProjectPath 'ProjectSettings\ProjectVersion.txt'))) {
  throw "Unity test project missing: $labProjectPath"
}

function Invoke-LabCommand([string]$Command, [string[]]$Arguments = @()) {
  $labOutput = & unity command $Command @Arguments --project-path $labProjectPath --timeout 45 --json
  $labExitCode = $LASTEXITCODE
  $labResult = ($labOutput -join "`n") | ConvertFrom-Json
  if ($labExitCode -ne 0 -or -not $labResult.success) {
    throw ($labResult.errors | ConvertTo-Json -Depth 6 -Compress)
  }
  if (($labResult.data.result.PSObject.Properties.Name -contains 'success') -and
      $labResult.data.result.success -eq $false) {
    throw ($labResult.data.result | ConvertTo-Json -Depth 8 -Compress)
  }
  if ($labResult.data.result.summary.failed -gt 0) {
    throw ($labResult.data.result | ConvertTo-Json -Depth 12 -Compress)
  }
  return $labResult
}

# Only readiness probes may be retried. Never blindly repeat mutations after
# a disconnect: a domain reload may have completed the requested action.
function Wait-LabReady {
  for ($labAttempt = 0; $labAttempt -lt 4; $labAttempt++) {
    try {
      $labProbe = Invoke-LabCommand 'eval' @('return new { project = UnityEngine.Application.dataPath, compiling = UnityEditor.EditorApplication.isCompiling, updating = UnityEditor.EditorApplication.isUpdating, playing = UnityEditor.EditorApplication.isPlaying, compilationFailed = UnityEditor.EditorUtility.scriptCompilationFailed };')
      if ($labProbe.data.result.result.compilationFailed) { throw 'Latest script compilation failed; do not trust tests from stale assemblies.' }
      if (-not $labProbe.data.result.result.compiling -and -not $labProbe.data.result.result.updating) {
        return $labProbe
      }
    } catch {
      if ($labAttempt -eq 3) { throw }
    }
    Start-Sleep -Milliseconds 500
  }
  throw 'Editor is not ready. Check its window for a blocking dialog.'
}

switch ($Action) {
  'Open' { & unity open $labProjectPath --json; if ($LASTEXITCODE -ne 0) { throw 'Open failed' } }
  'Probe' { Wait-LabReady | ConvertTo-Json -Depth 10 }
  'Tests' {
    $null = Wait-LabReady
    Invoke-LabCommand 'run_tests' @('--mode', 'editor', '--filter', 'WombatLab') | ConvertTo-Json -Depth 12
  }
  'PlayTests' {
    $labReady = Wait-LabReady
    if ($labReady.data.result.result.playing) { throw 'Stop Play before starting the test runner.' }
    Invoke-LabCommand 'run_tests' @('--mode', 'playmode', '--filter', 'WombatLab', '--async_tests', 'true') | ConvertTo-Json -Depth 12
  }
  'TestStatus' { Invoke-LabCommand 'test_status' | ConvertTo-Json -Depth 12 }
  'Play' {
    $labReady = Wait-LabReady
    if (-not $labReady.data.result.result.playing) { $null = Invoke-LabCommand 'editor_play' }
    Wait-LabReady | ConvertTo-Json -Depth 10
  }
  'Stop' {
    $labReady = Wait-LabReady
    if ($labReady.data.result.result.playing) { $null = Invoke-LabCommand 'editor_stop' }
    Wait-LabReady | ConvertTo-Json -Depth 10
  }
}
