cd ./src/WindowsDriver
msbuild WindowsDriver.sln /t:Clean;Restore;Build /property:Configuration=Release

cd ../UIADriver
msbuild UIADriver.sln /t:Clean;Restore;Build /property:Configuration=Release

cd ../../
rmdir build /s /q
xcopy "./src/WindowsDriver/bin/x64/Release/net8.0-windows" "./build" /s /y /i
xcopy "./src/UIADriver/bin/x64/Release/net8.0-windows" "./build/UIADriver" /s /y /i