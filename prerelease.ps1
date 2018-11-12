if ($env:APPVEYOR_REPO_BRANCH -eq "master" -and (-not $env:APPVEYOR_PULL_REQUEST_NUMBER))  {
  Write-Host "Package will be published to NuGet"
  $env:informational_version = $env:APPVEYOR_BUILD_VERSION
} else {
  Write-Host "Package will be published to NuGet as prerelease"
  $env:informational_version =  $env:APPVEYOR_BUILD_VERSION + "-prerelease" 
}
