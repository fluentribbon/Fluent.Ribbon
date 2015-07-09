@ECHO OFF

SET prerelease=

IF NOT '%1'=='' (
    :: Build a prerelease package
    IF NOT '%1'=='stable' (
        SET prerelease=%1
    )
)

SET MSBuildUseNoSolutionCache=1
SET msbuildexe="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

:: Build for Debug and Release to have a last visible point for potential compile warnings/errors
%msbuildexe% Fluent.Ribbon.msbuild /target:Build /property:Configuration=Debug /property:Prerelease=%prerelease% /v:m /nologo
%msbuildexe% Fluent.Ribbon.msbuild /target:PublishVersion /property:Configuration=Release /property:Prerelease=%prerelease% /v:m /nologo

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause