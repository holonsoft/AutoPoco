<#
.SYNOPSIS
  Runs Stryker.NET mutation testing against the core library or the Faker package.

.DESCRIPTION
  The settings live in stryker-config.json of each test project. This script adds what a config file
  cannot hold:

  MinVerSkip=true for the duration of the run. Stryker compiles the mutated assembly from a design time
  build, where MinVer does not run, so the mutant would get version 1.0.0.0 while the test assembly
  references 6.0.0.0; the test host then fails to load it and every mutant ends as "Error". Skipping
  MinVer gives both builds 1.0.0.0. Upstream issue: https://github.com/stryker-mutator/stryker-net/issues/3675
  Remove the variable once that is fixed.

  The report is written to <test project>/StrykerOutput/<timestamp>/reports/mutation-report.html.

.PARAMETER Package
  Core (holonsoft.AutoPoco, default) or Faker (holonsoft.AutoPoco.Faker).

.PARAMETER Mutate
  Optional glob(s) that limit the mutated files, e.g. "**/Util/CheckDigits.cs". Without it every file is
  mutated.

.PARAMETER Concurrency
  Optional number of parallel test hosts. 1 is slow but avoids the under-reported kills of
  https://github.com/stryker-mutator/stryker-net/issues/3832.

.PARAMETER Open
  Opens the HTML report when the run is done.

.EXAMPLE
  ./eng/mutation-test.ps1
  ./eng/mutation-test.ps1 -Mutate "**/Util/CheckDigits.cs" -Open
  ./eng/mutation-test.ps1 -Package Faker -Concurrency 1
#>
[CmdletBinding()]
param(
   [ValidateSet('Core', 'Faker')]
   [string] $Package = 'Core',

   [string[]] $Mutate = @(),

   [ValidateRange(1, 64)]
   [int] $Concurrency,

   [switch] $Open
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$testProject = if ($Package -eq 'Core') { 'holonsoft.AutoPoco.Tests' } else { 'holonsoft.AutoPoco.Faker.Tests' }
$testProjectPath = Join-Path $repositoryRoot $testProject

$strykerArguments = @()
foreach ($pattern in $Mutate) {
   $strykerArguments += @('--mutate', $pattern)
}
if ($PSBoundParameters.ContainsKey('Concurrency')) {
   $strykerArguments += @('--concurrency', $Concurrency)
}
if ($Open) {
   $strykerArguments += @('--open-report')
}

$previousMinVerSkip = $env:MinVerSkip
Push-Location $testProjectPath
try {
   $env:MinVerSkip = 'true'

   dotnet tool restore
   if ($LASTEXITCODE -ne 0) { throw "dotnet tool restore failed with exit code $LASTEXITCODE." }

   dotnet tool run dotnet-stryker @strykerArguments
   $strykerExitCode = $LASTEXITCODE
}
finally {
   $env:MinVerSkip = $previousMinVerSkip
   Pop-Location
}

exit $strykerExitCode
