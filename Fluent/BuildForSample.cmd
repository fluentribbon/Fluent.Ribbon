@ECHO OFF

SET target=Build
SET prerelease=

SET MSBuildUseNoSolutionCache=1
SET msbuildexe="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

%msbuildexe% Fluent.Ribbon.msbuild /target:%target% /property:Configuration=Release /property:Prerelease=%prerelease% /v:m /nologo
%msbuildexe% Fluent.Ribbon.msbuild /target:%target% /property:Configuration=Debug /property:Prerelease=%prerelease% /v:m /nologo

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause
