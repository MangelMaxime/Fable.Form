@echo off

dotnet tool restore

call npm i

node build.js %*
