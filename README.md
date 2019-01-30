[![Build status](https://niposoftware.visualstudio.com/_apis/public/build/definitions/15ce0e91-931d-4fbf-9169-8c3dde412b54/269/badge)](https://niposoftware.visualstudio.com/Nfield/_build/index?definitionId=269) [![NuGet version](https://badge.fury.io/nu/Nfield.Quota.svg)](https://badge.fury.io/nu/Nfield.Quota)

# Nfield.Quota

## Introduction
Using this project developers can create quota structures for Nfield Online surveys. For more information about Nfield and/or NIPO Software please visit the [NIPO] website.

## Documentation
See our [Read the Docs](http://nfieldquota.readthedocs.io/en/latest/getting_started/) page for documentation and examples to get started.

## Installation
The recommended way to consume this project is to reference the NuGet package. You can install it by executing the following command in the Package Manager Console.

```
PM> Install-Package Nfield.Quota
```

## Versioning
There is a file `version.txt` in the root of the repository containing the `major.minor` version.
The build server will append an incrementing third digit.
The number in this file should be increased in a sensible way every time the SDK is changed,
using [semantic versioning](https://semver.org/).

The suffixes that will be appended to the nuget package versions will be the following: 
- no suffix for release versions
- `-beta` for builds from master
- `-alpha` for builds on other branches

## Release procedure
This project uses [AppVeyor] for continuous integration. Commits to the development branch result in a prerelease package on [NuGet]. Commits into the master branch will publish a new release on [NuGet] and also a [draft release] on [GitHub]. This release can then be amended with information on what's new before being published.

[NIPO]: http://www.nipo.com
[AppVeyor]: http://www.appveyor.com
[NuGet]: http://nuget.org
[GitHub]: https://github.com
[draft release]: https://github.com/NIPOSoftware/Nfield.Quota/releases
