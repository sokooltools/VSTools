@echo off

if "%5" NEQ "" (goto:SKIP)

title Pre-Build: [SokoolTools.VSTools]

@echo ===============================================================================================================
@echo = (c) 2012-2019 SokoolTools.
@echo =
@echo = This batch file is used for pre-build configuration and should be called directly from the Visual Studio 
@echo = project's [Pre-Build] event command-line using the following syntax:
@echo =
@echo = "$(ProjectDir)Prebuild.bat" "$(ConfigurationName)" "$(SolutionDir)" "$(ProjectName)" "$(TargetDir)" "$(TargetName)"
@echo =
@echo = It is used for incrementing the version number in 'Release' builds or optionally resetting the experimental 
@echo = instance in 'Debug' builds.
@echo ===============================================================================================================
@echo.
@echo Press any key to continue . . . &pause>nul
goto:EOF

:SKIP

verify other 2>nul
setlocal EnableExtensions
if errorlevel 1 echo Unable to enable extensions
setlocal EnableDelayedExpansion
if errorlevel 1 echo Unable to enable delayed expansion

set Configuration=%~1
set SolutionDir=%~2
set ProjectDir=%~dp0
set ProjectName=%~3
set TargetDir=%~4
set TargetName=%~5
set TargetPath=%~4%~5

echo.
echo.
echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo BEGIN [%ProjectName%] Pre-Build
echo - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

echo.
echo ======================================================================================
echo = List of local variables.
echo ======================================================================================
echo.
echo Configuration =	%Configuration%
echo SolutionDir   =	%SolutionDir%
echo ProjectDir    =	%ProjectDir%
echo ProjectName   =	%ProjectName%
echo TargetDir     =	%TargetDir%
echo TargetName    =	%TargetName%
echo TargetPath    =	%TargetPath%

echo.
echo ======================================================================================
echo = Increment the version number.
echo ======================================================================================
echo.

:: Get the resolved path which also removes the last backslash from it.
call:GETRESPATH "%SolutionDir%" SolutionFolder

set ivn=%SolutionFolder%\incrementversionnumber\bin\incrementversionnumber

if %Configuration% EQU Debug (
	echo Note: The version number is only incremented in a 'Release' build.
	call:DO_DEBUG_SPECIFIC_TASKS
	goto:END
)

"%ivn%.exe" "%SolutionFolder%"
if errorlevel 1 (echo The version number increment failed!&goto:END)

for /F "USEBACKQ TOKENS=*" %%A in ("%ivn%.dat") do (set versn=%%~A)
echo The version number has been set to '%versn%'

:END
echo.
echo - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
echo END [%ProjectName%] Pre-Build
echo +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo.
echo.

goto:EOF

:: +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

:DO_DEBUG_SPECIFIC_TASKS
echo.
::call "%USERPROFILE%\Documents\Visual Studio 2019\ResetExpInstance.cmd"
goto:EOF

:: +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
:GETRESPATH
set "vf=%~1"
if not defined vf goto :eof
if not exist "%vf%" mkdir "%vf%"
pushd %vf%
set %2=%CD%
popd
goto:EOF

:: +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
:GETVERSION
set "vf=%~1"
if not defined vf goto :eof
if not exist "%vf%" goto :eof
set "vers="
for /F "tokens=2 delims==" %%a in ('
	wmic datafile where name^="%vf:\=\\%" get Version /value 
') do set "vers=%%a"
set %2=%vers%
goto:EOF
