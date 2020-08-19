REM Go to root directory
C:

cd Windows\System32

REM Set input parameters
set property=%1
set value=%2


REM Navigate to study path and run the batch file

call %property% %value%
