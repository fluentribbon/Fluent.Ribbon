Param(
    [Parameter(Mandatory=$False)]
    [string]$Configuration = "Release",
    [Parameter(Mandatory=$False)]
    [switch]$Publish,
    [Parameter(Mandatory=$False)]
    [string]$PreRelease
)

$ErrorActionPreference = "Stop"

. $PSScriptRoot\MSBuildHelper.ps1

$target = "Build"
if ($Publish) { $target = "PublishVersion" }

Write-Output Building
$msbuild = Get-MSBuild
&$msbuild Fluent.Ribbon.msbuild /target:$target /property:Configuration=$Configuration /property:Prerelease=$PreRelease /v:m /nologo

if ($LASTEXITCODE -ne 0) { exit 1 }