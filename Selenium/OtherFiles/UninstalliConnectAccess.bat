@echo off
echo Uninstalling Exam Importer
START /WAIT wmic product where "name like '%%EI%%'" call uninstall /nointeractive
echo Uninstalled Exam Importer

echo Uninstalling iConnect Application
START /WAIT wmic product where "Name like '%%iConnect Access'" call uninstall /nointeractive
echo Uninstallation Completed