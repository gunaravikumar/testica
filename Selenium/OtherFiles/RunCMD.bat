set ip=%1
set username=%2
set pwd=%3
set cmd=%4
set iparam=%5
if %iparam%==true PsExec.exe \\%ip% -u %username% -p %pwd% -accepteula -i cmd /c %cmd%
if %iparam%==false PsExec.exe \\%ip% -u %username% -p %pwd% -accepteula cmd /c %cmd%