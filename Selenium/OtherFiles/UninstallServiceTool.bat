@echo off
echo Uninstalling Service Tool
START /WAIT wmic product where "Name like '%%iConnect Access Service Tool'" call uninstall /nointeractive
echo Uninstallation Completed