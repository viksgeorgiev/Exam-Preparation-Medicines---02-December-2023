@echo off
setlocal

:: Deletes all "obj" and "bin" folders in the current directory and subdirectories
for /d /r . %%d in (obj bin) do (
    echo Deleting folder: "%%d"
    rd /s /q "%%d"
)

echo Deletion complete.
pause
