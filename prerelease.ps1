if ($env:APPVEYOR_REPO_BRANCH -eq "master" -and (-not $env:APPVEYOR_PULL_REQUEST_NUMBER))  {
  Write-Host "Package will be published to NuGet"
  Set-AppveyorBuildVariable -Name "assembly_informational_version" -Value "$version"
} else {
  Write-Host "Package will be published to NuGet as prerelease"
  Set-AppveyorBuildVariable -Name "assembly_informational_version" -Value "$version-prerelease"
}
