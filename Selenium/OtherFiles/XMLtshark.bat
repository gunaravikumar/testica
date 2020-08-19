cd C:
cd C:\Program Files\Wireshark
C:
set arg1=%1
set arg2=%2
tshark -Y "ip.addr==%arg1% || ip.addr==%arg2%"  -P -V -x -a duration:30 > captures.txt
exit ERRORLEVEL