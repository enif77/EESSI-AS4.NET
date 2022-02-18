# Run this to setup dotnet dev certs.
# dotnet dev-certs https --trust

# For VersionAssemblies.ps1
$env:BUILD_BUILDNUMBER = '4.0.1.1000'


# Python 2.7 is required for installing node-saas from source via 'npm install'.
# If a binary package is used, this is not needed.
# $env:PYTHON = 'C:\Devel\bin\Python27'
# $env:Path = "C:\Devel\bin\Python27;C:\Devel\bin\Python27\Scripts;" + $env:Path

Set-Location "./source/Eu.EDelivery.AS4.FE/ui"
npm install node-sass
npm install
npm run build:aot:prod
npm run copytooutput    
Set-Location "..\..\..\"

.\tools\NuGet\nuget.exe restore .\source\AS4.sln

& './scripts/VersionAssemblies.ps1'

#$msbuild = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe'
$msbuild = 'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\msbuild.exe'
& $msbuild '.\source\AS4.sln' /t:Rebuild /p:Configuration=Release /nologo /nr:false /verbosity:minimal

# This part was used to build the MSI package. It works with old VS2017 only, so it is disabled now.
# #$devEnvPath = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe'
# $solutionPath = "./source/AS4.sln"
# $projectPath = "./source/Eu.EDelivery.AS4.WindowsService.Installer/Eu.EDelivery.AS4.WindowsService.Installer.vdproj"
# $parameters = "/Rebuild Release " + $solutionPath + " /Project " + $projectPath + " /ProjectConfig Release /Out errors.txt"
# "Process to start [$devEnvPath $parameters]"
# $process = [System.Diagnostics.Process]::Start($devEnvPath, $parameters)
# $process.WaitForExit()
# if (Test-Path errors.txt) {
#     Get-Content errors.txt
# }

Set-Location output
& '../scripts/add-probing.ps1'
& '../scripts/stagingscript.ps1'
Set-Location ..

Set-Location "./scripts/"
& './GenerateXsd.ps1'
Set-Location ".."
