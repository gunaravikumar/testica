cd C:
cd C:\Program Files (x86)\Wireshark
C:
set arg1=%1
tshark -Y http.host==%arg1% -O WebSocket -Y websocket.payload -E occurrence=a -T fields -e text -a duration:60 > captures.txt
exit ERRORLEVEL


