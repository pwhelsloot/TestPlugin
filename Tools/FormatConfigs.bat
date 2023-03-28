@echo off

setlocal ENABLEDELAYEDEXPANSION

pushd "%~dp0"

ConfigFormatter.exe

pause

popd
