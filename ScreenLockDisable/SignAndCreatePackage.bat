@echo off

set "cert=%CodesignCertPath%"
set "timestamp=http://timestamp.digicert.com"

set "src=.\bin\publish"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\ScreenLockDisable.exe"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\ScreenLockDisable.exe"

signtool.exe sign /fd sha256 /a /f "%cert%" "%src%\ScreenLockDisable.dll"
signtool.exe timestamp /tr "%timestamp%" /td sha256 "%src%\ScreenLockDisable.dll"

del /S /Q ".\bin\ScreenLockDisable.zip" >nul 2>&1
7za.exe a -mx0 -tzip ".\bin\ScreenLockDisable.zip" "%src%\*" -xr!*.pdb
7za.exe h -scrcSHA256 ".\bin\ScreenLockDisable.zip" > ".\bin\sha256.txt"

pause
