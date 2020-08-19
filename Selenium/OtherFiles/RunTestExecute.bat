REM Clears the screen
CLS
@ECHO OFF
SETLOCAL EnableDelayedExpansion

set projectPath=%1
set projectName=%2
set unitName=%3
set routineName=%4
set allarg=%*
Set TEpath=
set path32="C:\PROGRA~2\SmartBear\TestExecute 12\Bin"
set path64="C:\Program Files\SmartBear\TestExecute 12\x64\Bin"

set logfile=C:\VSCmdError.txt
set errorfile=C:\TCError_%DATE:~-4%-%DATE:~4,2%-%DATE:~7,2%_%TIME:~10,2%-%TIME:~3,2%.txt

ECHO ************ Executing %routineName% at %date% %time% ****** >> %logfile%

IF EXIST %path32% ( set TEpath=%path32% 
) ELSE ( 
	if exist %path64% ( set TEpath=%path64%
	) else (
		GOTO NoTestExecute
	)
)

CD /D %TEpath%

for /f "tokens=4,* delims= " %%a in ('echo %allarg%') do set Params=%%b


ECHO TestExecute12 Path: %CD% >> %logfile%
ECHO ProjectPath: %projectPath% >> %logfile%
ECHO Launches TestExecute >> %logfile%

REM executes the specified project
REM and closes TestExecute when the run is over
start "TestExecute" /min /wait TestExecute.exe %projectPath% /run /exit /SilentMode /ErrorLog:%errorfile% /DoNotShowLog /p:%projectName% /u:%unitName% /rt:%routineName% %Params%

ECHO Closing TestExecute >> %logfile%

IF ERRORLEVEL 1001 GOTO NotEnoughDiskSpace
IF ERRORLEVEL 1000 GOTO AnotherInstance
IF ERRORLEVEL 127 GOTO DamagedInstall
IF ERRORLEVEL 4 GOTO Timeout
IF ERRORLEVEL 3 GOTO CannotRun
IF ERRORLEVEL 2 GOTO Errors
IF ERRORLEVEL 1 GOTO Warnings
IF ERRORLEVEL 0 GOTO Success
IF ERRORLEVEL -1 GOTO LicenseFailed
 
:NotEnoughDiskSpace
ECHO There is not enough free disk space to run TestExecute >> %logfile%
GOTO End
 
:AnotherInstance
ECHO Another instance of TestExecute is already running >> %logfile%
GOTO End
 
:DamagedInstall
ECHO TestExecute installation is damaged or some files are missing >> %logfile%
GOTO End
 
:Timeout
ECHO Timeout elapsed  >> %logfile%
GOTO End
 
:CannotRun
ECHO The script cannot be run >> %logfile%
GOTO End
 
:Errors
ECHO There are errors >> %logfile%
GOTO End
 
:Warnings
ECHO There are warnings >> %logfile%
GOTO End
 
:Success
ECHO No errors >> %logfile%
GOTO End
 
:LicenseFailed
ECHO License check failed >> %logfile%
GOTO End

:NoTestExecute
ECHO No TestExecute Installed >> %logfile%
GOTO End

:End
ECHO *********** Execution Completed at %date% %time% ************
exit /b %ERRORLEVEL%