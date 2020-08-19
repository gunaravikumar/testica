@ECHO OFF
ECHO #### Batch file for Running Z3D Powershell scripts####
:: ECHO Press any key to start
Powershell.exe Set-ExecutionPolicy Unrestricted -Force
ECHO Execution Policy is set
Powershell.exe c:\drs\sys\util\Z3D_InstallerTasks.ps1
ECHO Z3D Installer Task are triggered successfully
Powershell.exe c:\drs\sys\util\Z3D_UpdateConfigFromICA.ps1
ECHO Update Config from iCA are triggered successfully
:: ECHO Press any key to quit
TIMEOUT 10