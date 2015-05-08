import-module psake
invoke-psake -buildFile ".\Build-HandbrakeEncoder.PSAKE.ps1" -properties @{ config = 'Debug' }