set username=%1
set pwd=%2
set ip=%3
PsExec.exe -u %username% -p %pwd% -i 1 -d -w "C:\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter" \\%ip% cmd /c "C:\Program Files (x86)\MergeHealthcare\MergePort\Fusion\utilities\HL7\transmitter\ReadSend.bat"


