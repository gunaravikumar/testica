@ECHO OFF
set arg1=%1
ECHO #### Batch file for deleting IRWSDB and MU2 databases on another server ####
ECHO.
ECHO.
sqlcmd -S %arg1% -U sa -P welcome@123 -Q "ALTER DATABASE IRWSDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
sqlcmd -S %arg1% -U sa -P welcome@123 -Q "DROP DATABASE IRWSDB"
ECHO IRWSDB deleted
sqlcmd -S %arg1% -U sa -P welcome@123 -Q "ALTER DATABASE MU2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE"
sqlcmd -S %arg1% -U sa -P welcome@123 -Q "DROP DATABASE MU2"
ECHO MU2 deleted
ECHO.
#:: ECHO Press any key to quit
#TIMEOUT 10