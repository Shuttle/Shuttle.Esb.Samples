@echo off

if not exist Shuttle.ProcessManagement.sln (
	echo Could not find solution file 'Shuttle.ProcessManagement.sln'.
	echo Please ensure that you are executing this file (host-on-windows.cmd^) in the '{base}\Shuttle.Esb.Samples\Shuttle.ProcessManagement\.' folder.
	GOTO:EOF
)

REM echo Checking if 'http-server' is installed...

REM npm list -g --depth 0 | findstr /L /I /C:"-- http-server@" > null

REM if %errorlevel% neq 0 (
	REM echo Could not find 'http-server'.  Please install it using 'npm install http-server -g'
	REM GOTO:EOF
REM )

if "%1%" == "no-build" goto host

echo Building...

msbuild Shuttle.ProcessManagement.sln /v:q

if %errorlevel% neq 0 (
	echo Could not build the solution.  
	echo Please ensure that 'msbuild.exe' is reachable (running from Visual Studio command prompt is the easiest^).
	GOTO:EOF
)

:host
echo Starting...

start dotnet .\Shuttle.EMailSender.Server\bin\Debug\netcoreapp2.1\Shuttle.EMailSender.Server.dll
start dotnet .\Shuttle.Invoicing.Server\bin\Debug\netcoreapp2.1\Shuttle.Invoicing.Server.dll
start dotnet .\Shuttle.Ordering.Server\bin\Debug\netcoreapp2.1\Shuttle.Ordering.Server.dll
start dotnet .\Shuttle.Process.Custom.Server\bin\Debug\netcoreapp2.1\Shuttle.Process.Custom.Server.dll
start dotnet .\Shuttle.Process.CustomES.Server\bin\Debug\netcoreapp2.1\Shuttle.Process.CustomES.Server.dll
start dotnet .\Shuttle.Process.ESModule.Server\bin\Debug\netcoreapp2.1\Shuttle.Process.ESModule.Server.dll
start dotnet .\Shuttle.Process.QueryServer\bin\Debug\netcoreapp2.1\Shuttle.Process.QueryServer.dll
start dotnet .\Shuttle.ProcessManagement.WebApi\bin\Debug\netcoreapp2.1\Shuttle.ProcessManagement.WebApi.dll --urls http://localhost:8656

