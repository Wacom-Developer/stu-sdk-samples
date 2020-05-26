@setlocal
@set APP=DemoButtons
@set APP_BIN=bin\%APP%.class
@set INSTALL_PATH="\Wacom STU SDK"
@if not exist bin md bin
@if exist %APP_BIN% del %APP_BIN%
@
@echo Compiling %APP% ...
@javac -classpath %INSTALL_PATH%\Java\jar\x64\wgssSTU.jar -d bin %APP%.java
@echo.
@if not exist %APP_BIN% goto END
@echo Run %APP% ...
@java -classpath %INSTALL_PATH%\Java\jar\x64\wgssSTU.jar;.\bin -Djava.library.path=%INSTALL_PATH%\Java\jar\x64 %APP%
@goto END

:END
