@echo off

msbuild "xFunc (Libs for .Net 2.0).sln" /p:Configuration=Release
if errorlevel 0 pause
if errorlevel 1 goto err
msbuild "xFunc (Libs for .Net 3.0).sln" /p:Configuration=Release
if errorlevel 0 pause
if errorlevel 1 goto err
msbuild "xFunc (Libs for .Net 3.5).sln" /p:Configuration=Release
if errorlevel 0 pause
if errorlevel 1 goto err
msbuild "xFunc (Libs for .Net 4.0).sln" /p:Configuration=Release
if errorlevel 0 pause
if errorlevel 1 goto err
msbuild "xFunc (Libs for .Net 4.5).sln" /p:Configuration=Release
if errorlevel 0 pause
if errorlevel 1 goto err
msbuild "xFunc (Libs for Portable).sln" /p:Configuration=Release
if errorlevel 0 pause
if errorlevel 1 goto err

exit

:err
pause