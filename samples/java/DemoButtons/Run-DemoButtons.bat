@setlocal
@set APP=DemoButtons
@set APP_BIN=bin\%APP%.class
@set INSTALL_PATH="C:\Program Files (x86)\Wacom STU SDK"
@if not exist bin md bin
@if exist %APP_BIN% del %APP_BIN%
@
@echo Compiling %APP% ...
@javac -classpath %INSTALL_PATH%\Java\jar\Win32\wgssSTU.jar -d bin %APP%.java
@echo.
@if not exist %APP_BIN% goto END
@echo Run %APP% ...
@java -classpath %INSTALL_PATH%\Java\jar\Win32\wgssSTU.jar;.\bin -Djava.library.path=%INSTALL_PATH%\Java\jar\Win32 %APP%
@goto END

:END
