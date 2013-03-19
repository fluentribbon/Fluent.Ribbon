@ECHO OFF

SET target=Build
SET prerelease=

:: Default to Release configuration if nothing is specified
IF '%1'=='' (SET configuration=Release) ELSE (SET configuration=%1)

:: Check if NuGet package should be built
IF NOT '%2'=='' (
    SET target=MakeNuGetPackage

    :: Build a prerelease package
    IF NOT '%2'=='stable' (
        SET prerelease=%2
    )
)

SET MSBuildUseNoSolutionCache=1
SET msbuildexe="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

%msbuildexe% Fluent.Ribbon.msbuild /target:%target% /property:Configuration=%configuration% /property:Prerelease=%prerelease%

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause
