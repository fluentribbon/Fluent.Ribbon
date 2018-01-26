Param(
    [Parameter(Mandatory=$False)]
    [string]$Target = "Build",    
    [Parameter(Mandatory=$False)]
    [string]$Configuration = "Release",
    [Parameter(Mandatory=$False)]
    [string]$PreRelease
)

$ErrorActionPreference = "Stop"

. $PSScriptRoot\MSBuildHelper.ps1

if ($PSBoundParameters['Verbose']) {
    $verbosity = "d"
} else {
    $verbosity = "m"
}

Write-Output "Building with verbosity '$verbosity'"

$msbuild = Get-MSBuild

$measure = Measure-Command {
	&$msbuild Fluent.Ribbon.msbuild /target:$Target /property:Configuration=$Configuration /property:Prerelease=$PreRelease /v:$verbosity /nologo | Out-Default
}

Write-Output "Time elapsed $($measure.ToString())"

if ($LASTEXITCODE -ne 0) { exit 1 }