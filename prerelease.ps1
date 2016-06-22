if ($env:APPVEYOR_REPO_BRANCH -eq "master" -and (-not $env:APPVEYOR_PULL_REQUEST_NUMBER))  {
  Set-AppveyorBuildVariable -Name "assembly_informational_version" -Value "$version"
} else {
  Set-AppveyorBuildVariable -Name "assembly_informational_version" -Value "$version-prerelease"
}
