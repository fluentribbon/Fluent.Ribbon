@ECHO OFF

SET target=Build
SET configuration=Release

IF NOT '%1'=='' (
	SET configuration=%1
)

SET MSBuildUseNoSolutionCache=1
SET msbuildexe="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

%msbuildexe% Fluent.Ribbon.msbuild /target:%target% /property:Configuration=%configuration% /v:m /nologo

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause
