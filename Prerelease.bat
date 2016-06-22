ECHO OFF
SET PRERELEASE="-prerelease"
IF "%APPVEYOR_REPO_BRANCH%" == "master" (
  IF NOT DEFINED APPVEYOR_PULL_REQUEST_NUMBER (
    SET PRERELEASE=""
  )
)

IF %PRERELEASE% == "" (
  ECHO "Package will be publish to NuGet"
) ELSE (
  ECHO "Package will be publish to NuGet as prerelease"
)