@echo off
echo Build Inferno.Runtime
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"  /build release "Inferno.Runtime/src/Inferno.Runtime.sln"
echo Build Inferno.UI
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"  /build release "Inferno.UI/src/Inferno.UI.sln"
echo Build Inferno.Social
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"  /build release "Inferno.Social/src/Inferno.Social.sln"

echo DONE!
pause