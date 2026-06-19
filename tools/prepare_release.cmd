@echo off
git submodule -q update --init
if %errorlevel% neq 0 exit /b %errorlevel%
dotnet run "%~dp0..\tools-shared\PrepareRelease.cs" -- FakeItEasy %*
