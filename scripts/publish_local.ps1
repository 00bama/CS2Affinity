# Publish local single-file EXE to `output` folder for manual upload
dotnet publish CS2Affinity/CS2Affinity.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o output
if ($LASTEXITCODE -ne 0) { Write-Error "Publish failed"; exit $LASTEXITCODE }
Write-Host "Published to output\CS2Affinity.exe"
