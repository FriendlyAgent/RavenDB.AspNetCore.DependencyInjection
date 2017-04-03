@echo off

for %%i in ("%~dp0..") do set "rootProject=%%~fi"
set nugetFolder = %rootProject%\nuget\
set projectFolder=%rootProject%\src\RavenDB.AspNetCore.DependencyInjection\
set projectTestFolder=%rootProject%\tests\RavenDB.AspNetCore.DependencyInjection.test\

set config=%1
if "%config%" == "" (
   set config=Release
)

SETLOCAL
cd %projectFolder%
call dotnet restore
if not "%errorlevel%"=="0" goto failure

call dotnet build --configuration %config%
if not "%errorlevel%"=="0" goto failure
ENDLOCAL

SETLOCAL
cd %projectTestFolder%
call dotnet test --configuration %config%
if not "%errorlevel%"=="0" goto failure
ENDLOCAL

SETLOCAL
cd %nugetFolder%
call dotnet pack %projectFolder% --configuration %config% --output packages
if not "%errorlevel%"=="0" goto failure
ENDLOCAL

:success
exit 0

:failure
pause
exit -1