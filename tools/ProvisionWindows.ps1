## Setup Windows machine for development.
## By: Christian Gunderman

# Visual Studio 2019 P1 is assumed to be installed already, as it comes on the Azure DevOps build machines.

# Install Go tools
$fileGuid = [Guid]::NewGuid()
$goFileName = (Join-Path $env:TEMP "$fileGuid.msi")
echo "Downloading Go for Windows to $goFileName..."
(New-Object System.Net.WebClient).DownloadFile("https://dl.google.com/go/go1.12.7.windows-amd64.msi", $goFileName)
echo "Installing Go for Windows..."
$goLogFileName = "$env:TEMP\GoInstall.log"
Start-Process msiexec.exe -Wait -ArgumentList "/i $goFileName /QN /L*v $goLogFile"

# .DownloadString('https://dl.google.com/go/go1.12.7.windows-amd64.msi')

# # Install Chocolatey Package manager:
# Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://dl.google.com/go/go1.12.7.windows-amd64.msi'))

# # Install MinGW (GCC for Windows for c-Go compilation).
# choco install mingw --version 4.8.5 --yes --force

# # Update environment variables.
# refreshenv

# https://osdn.net/projects/mingw/downloads/68260/mingw-get-setup.exe/
