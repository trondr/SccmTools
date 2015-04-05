@ECHO OFF
Set ProductName=SccmTools

IF EXIST "%VSDEVCMD%" goto Build
IF EXIST "%MSBUILDPATH%" goto Build

:VSEnv
Set VSDEVCMD=%VS120COMNTOOLS%VsDevCmd.bat
Echo Checking to see if Visual Studio 2013 is installed ("%VS120COMNTOOLS%")
IF NOT EXIST "%VSDEVCMD%" set BuildMessage="Visual Studio 2013 do not seem to be installed, trying MSBuild instead..." & goto MSBuildEnv
Echo Preparing build environment...
call "%VSDEVCMD%"

:MSBuildEnv
Set MSBUILDPATH=%ProgramFiles(x86)%\MSBuild\12.0\Bin
Echo Checking to see if MSBuild is installed ("%MSBUILDPATH%")
IF NOT EXIST "%MSBUILDPATH%" set BuildMessage="Neither Visual Studio 2013 or MSBuild  seem to be installed. Terminating." & goto end
Set Path=%Path%;%MSBUILDPATH%

:Build
Echo Building %ProductName%...
msbuild.exe %ProductName%.build %1 %2 %3 %4 %5 %6 %7 %8 %9
Set BuildErrorLevel=%ERRORLEVEL%
IF %BuildErrorLevel%==0 Set BuildMessage=Sucessfully build %ProductName%
IF NOT %BuildErrorLevel% == 0 Set BuildMessage=Failed to build %ProductName%

:End
Echo %BuildMessage%
