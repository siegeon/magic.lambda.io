
set version=%1
set key=%2

cd %~dp0
dotnet build magic.lambda.io/magic.lambda.io.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.lambda.io/bin/Release/magic.lambda.io.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json
