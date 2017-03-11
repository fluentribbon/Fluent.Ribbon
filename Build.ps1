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

Write-Output Building
$msbuild = Get-MSBuild
&$msbuild Fluent.Ribbon.msbuild /target:$Target /property:Configuration=$Configuration /property:Prerelease=$PreRelease /v:m /nologo

if ($LASTEXITCODE -ne 0) { exit 1 }