Add-Type -TypeDefinition @"
    public enum MSBuildPath
    {
       MSBuildToolsPath,
       MSBuildToolsRoot
    }
"@

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
       $Version = $null,
       [Parameter(Mandatory=$False)]
       [switch]$All = $false
    )

    $versions = dir HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\ | %{ new-object System.Version ((Split-Path $_.Name -Leaf)) } | Sort-Object -Descending   

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

function Get-MSBuildPath()
{
    [CmdletBinding()]
    Param(
       [Parameter(Mandatory=$True)]
       [MSBuildPath]$Path,
       [Parameter(Mandatory=$False)]
       $Version = $null, 
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

    Using-Object ($key = [Microsoft.Win32.RegistryKey]::OpenBaseKey([Microsoft.Win32.RegistryHive]::LocalMachine, $registryView)) {
        Using-Object ($subKey =  $key.OpenSubKey("SOFTWARE\Microsoft\MSBuild\ToolsVersions\$foundVersion")) {
            $resolvedPath = $subKey.GetValue($Path)

            if ($resolvedPath -eq $null)
            {
                Write-Error "Could not resolve path for version '$foundVersion' and '$Path'"
                return $null
            }

            Write-Verbose "$Path : $resolvedPath"

            return $resolvedPath
        }
    }    
}

function Get-MSBuild()
{
    [CmdletBinding()]
    Param(
       [Parameter(Mandatory=$False)]
       $Version = $null, 
       [Parameter(Mandatory=$False)]
       [Platform]$Platform = [Platform]::Current
    )

    $toolsPath = Get-MSBuildPath MSBuildToolsPath -Version $Version -Platform $Platform

    if ($null -eq $toolsPath)
    {
        Write-Error "MSBuild could not be found"
        return
    }

    return Join-Path $toolsPath "msbuild.exe"
}

#Get-MSBuildVersion -All
#Get-MSBuildPath MSBuildToolsPath -Verbose
#Get-MSBuildPath MSBuildToolsRoot -Verbose

#Get-MSBuildPath MSBuildToolsPath -Platform x86 -Verbose
#Get-MSBuildPath MSBuildToolsPath -Platform x64 -Verbose
#Get-MSBuild 1 -ErrorAction Continue
#Get-MSBuild 2
#Get-MSBuild 3
#Get-MSBuild 3.5
#Get-MSBuild 12
#Get-MSBuild 14 -Platform x86 -Verbose