::Params for exe
set CORE_URL=http://localhost:26519/
set PLUGIN_URL=http://localhost:16932/
set TENANT_GUID=4C0FFDF1-CC0A-4C1F-A023-7B2C0354AE98
set PASS_PHRASE=supersecret!supersecret!
::To get absolute path to metadata.json we:
:: 1) Temporarily pushd using relative path
:: 2) Get absolute using CD
:: 3) popd back
set METADATA_REL_PATH=..\..\API
set METADATA_ABS_PATH=
pushd %METADATA_REL_PATH%
set METADATA_ABS_PATH=%CD%\metadata.json
popd

if exist "temp\" rmdir /s /q "temp"
mkdir "temp"
tar -xf "RegisterPlugin_0.0.1.174.zip" -C "temp"
start /wait /b .\temp/RegisterPlugin.exe --core-service-root "%CORE_URL%" --app-service-root "%PLUGIN_URL%" --metadata-file "%METADATA_ABS_PATH%" --tenant-id "%TENANT_GUID%" --pass-phrase "%PASS_PHRASE%"
rmdir /s /q "temp"