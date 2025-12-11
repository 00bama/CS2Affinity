# Generates a SHA256 checksum file for the published EXE
param(
    [string]$ExePath = "output\CS2Affinity.exe",
    [string]$OutPath = "output\CS2Affinity.exe.sha256"
)

if (-not (Test-Path $ExePath)) { Write-Error "File not found: $ExePath"; exit 1 }
$hash = Get-FileHash -Path $ExePath -Algorithm SHA256
"$($hash.Hash)  $(Split-Path -Leaf $ExePath)" | Out-File -FilePath $OutPath -Encoding ASCII
Write-Host "Wrote SHA256 to $OutPath"
