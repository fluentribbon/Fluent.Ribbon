@ECHO OFF

SET MSBuildUseNoSolutionCache=1
SET msbuildexe="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

%msbuildexe% Fluent.Ribbon.msbuild /target:Build /property:Configuration=Debug /v:m /nologo
%msbuildexe% Fluent.Ribbon.msbuild /target:Build /property:Configuration=Release /v:m /nologo

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause