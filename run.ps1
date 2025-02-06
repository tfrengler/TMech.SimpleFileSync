$Config = "Debug"
Write-Host -f Yellow "Building/compiling ($Config)..."
$BuildOutput = dotnet build -c $Config

if ($BuildOutput -match "Build FAILED")
{
    Write-Host -f Red "Build failed! Aborting test run"
    exit
}

Write-Host -f Green "Build done, no errors"
Write-Host "Running program"

& "./Build/bin/$Config/net8.0/TMech.SimpleFileSync.exe" $args
Write-Host -f Yellow "--DONE--"