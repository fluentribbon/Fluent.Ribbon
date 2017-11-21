Add-Type -TypeDefinition @"
    public enum Platform
    {
       Current,
       x86,
       x64
    }
"@

function Using-Object
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [AllowEmptyCollection()]
        [AllowNull()]
        [Object]
        $InputObject,
 
        [Parameter(Mandatory = $true)]
        [scriptblock]
        $ScriptBlock
    )
 
    try
    {
        . $ScriptBlock
    }
    finally
    {
        if ($null -ne $InputObject -and $InputObject -is [System.IDisposable])
        {
            $InputObject.Dispose()
        }
    }
}

function Get-MSBuildVersion()
{
    [CmdletBinding()]
    Param(
       [Parameter(Mandatory=$False)]
       [Version]$Version = $null,
       [Parameter(Mandatory=$False)]
       [switch]$All = $false
    )

    $versions = Get-ChildItem HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\ | ForEach-Object { New-Object System.Version ((Split-Path $_.Name -Leaf)) } | Sort-Object -Descending   

    if ($All)
    {
        return $versions
    }

    if ($null -eq $Version)
    {
        return $versions[0]
    }

    foreach ($currentVersion in $versions)
    {
        if ($currentVersion -eq $Version)
        {
            return $currentVersion
        }
    }

    foreach ($currentVersion in $versions)
    {
        if ($currentVersion.Major -eq $Version)
        {
            return $currentVersion
        }
    }

    Write-Error "MSBuild version $Version could not be found"

    return $null
}

function Get-MSBuildPathLegacy()
{
    [CmdletBinding()]
    Param(
       [Parameter(Mandatory=$False)]
       [Version]$Version = $null, 
       [Parameter(Mandatory=$False)]
       [Platform]$Platform = [Platform]::Current
    )

    $foundVersion = Get-MSBuildVersion $Version -ErrorAction SilentlyContinue

    if ($null -eq $foundVersion)
    {
        Write-Error "MSBuild version $Version could not be found"
        return
    }

    switch ($Platform)
    {
        Current { $registryView = [Microsoft.Win32.RegistryView]::Default }
        x86 { $registryView = [Microsoft.Win32.RegistryView]::Registry32 }
        x64 { $registryView = [Microsoft.Win32.RegistryView]::Registry64 }
    }

    $path = "MSBuildToolsPath"

    Using-Object ($key = [Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine, $registryView)) {
        Using-Object ($subKey =  $key.OpenSubKey("SOFTWARE\Microsoft\MSBuild\ToolsVersions\$foundVersion")) {
            $resolvedPath = $subKey.GetValue($path)

            if ($resolvedPath -eq $null)
            {
                Write-Error "Could not resolve path for version '$foundVersion' and '$path'"
                return $null
            }

            Write-Verbose "$path : $resolvedPath"

            return $resolvedPath
        }
    }    
}

function Get-MSBuildPath()
{
    [CmdletBinding()]
    Param(
       [Parameter(Mandatory=$False)]
       [String]$VersionString = $null, 
       [Parameter(Mandatory=$False)]
       [Platform]$Platform = [Platform]::Current
    )

    if ($VersionString) {
        if (($VersionString -like "*.*") -eq $False) {
            $VersionString += ".0"
        }

        $version = [Version]$VersionString
    }

	if ((Get-Command vswhere) -or $version -ge [Version]"15.0") {
        if ($version -eq $null) {
		    $installationPath = vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
        }
        else {
            $versionConstraint = "[$($version.Major).$($version.Minor), $($version.Major + 1).$($version.Minor))"
            $installationPath = vswhere -version $versionConstraint -products * -requires Microsoft.Component.MSBuild -property installationPath
        }

		if ($installationPath) {
            $need64Bit = $False
            switch ($Platform)
            {
                Current { $need64Bit = [System.Environment]::Is64BitProcess }
                x64 { $need64Bit = $true }
                x86 { $need64Bit = $false }
            }
            
            $likePattern = if ($need64Bit) { '*bin\amd64\msbuild.exe' } else { '*bin\msbuild.exe' } 
            $msbuild = (Get-ChildItem -Path $installationPath -Filter "MSBuild.exe" -Recurse) | Where-Object { $_.FullName -like $likePattern } 

			if (Test-Path $msbuild.DirectoryName) {
			    return $msbuild.DirectoryName
			}
		}
	}
	
    # If none of the upper branches returned a version we try the legacy path
	return Get-MSBuildPathLegacy -Version $version -Platform $Platform
}

function Get-MSBuild()
{
    [CmdletBinding()]
    Param(
       [Parameter(Mandatory=$False,Position=1)]
       [String]$VersionString = $null, 
       [Parameter(Mandatory=$False)]
       [Platform]$Platform = [Platform]::Current
    )

    $msbuildPath = Get-MSBuildPath -VersionString $VersionString -Platform $Platform

    if ($null -eq $msbuildPath)
    {
        Write-Error "MSBuild could not be found"
        return
    }

	$msbuild = Join-Path $msbuildPath "msbuild.exe"

	Write-Host "Using msbuild from '$msbuild'"

    return $msbuild
}

#Get-MSBuildVersion -All
#Get-MSBuildPath MSBuildToolsPath -Verbose
#Get-MSBuildPath MSBuildToolsRoot -Verbose

#Get-MSBuildPath MSBuildToolsPath -Platform x86 -Verbose
#Get-MSBuildPath MSBuildToolsPath -Platform x64 -Verbose
#Get-MSBuild
#Get-MSBuild 1 -ErrorAction Continue
#Get-MSBuild 2
#Get-MSBuild 3
#Get-MSBuild 3.5
#Get-MSBuild 12
#Get-MSBuild 14
#Get-MSBuild "14.0" -Platform x86 -Verbose
#Get-MSBuild -Version 15.0