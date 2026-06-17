@pushd %~dp0
@dotnet run ".\tools\Build.cs" -- %*
@popd
