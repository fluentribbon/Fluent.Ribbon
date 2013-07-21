@ECHO OFF

SET target=Build
SET prerelease=

:: Check if NuGet package should be built
IF NOT '%1'=='' (
    SET target=MakeNuGetPackage

    :: Build a prerelease package
    IF NOT '%1'=='stable' (
        SET prerelease=%1
    )
)

SET MSBuildUseNoSolutionCache=1
SET msbuildexe="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

%msbuildexe% Fluent.Ribbon.msbuild /target:%target% /property:Configuration=Release /property:Prerelease=%prerelease% /v:m /nologo

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause
