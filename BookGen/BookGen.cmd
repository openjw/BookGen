@echo off
rem -----------------------------------------------------------------------------
rem  (c) 2019 Ruzsinszki Gábor
rem  This code is licensed under MIT license (see LICENSE for details)
rem -----------------------------------------------------------------------------

WHERE foo 2> nul
IF %ERRORLEVEL% NEQ 0 goto notfound

dotnet BookGen.dll %*
goto exit

:notfound
ECHO This program Requires .NET Core Runtime, but it's not installed. Exiting

:exit