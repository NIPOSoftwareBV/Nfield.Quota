[![Build status](https://niposoftware.visualstudio.com/_apis/public/build/definitions/15ce0e91-931d-4fbf-9169-8c3dde412b54/269/badge)](https://niposoftware.visualstudio.com/Nfield/_build/index?definitionId=269) [![NuGet version](https://badge.fury.io/nu/Nfield.Quota.svg)](https://badge.fury.io/nu/Nfield.Quota)

# Nfield.Quota

## Introduction
Using this project developers can create quota structures for Nfield Online surveys. For more information about Nfield and/or NIPO Software please visit the [NIPO](http://www.nipo.com) website.

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
The number in this file should be increased in a sensible way every time the package is changed,
using [semantic versioning](https://semver.org/).

The suffixes that will be appended to the nuget package versions will be the following: 
- no suffix for release versions
- `-beta` for builds from master
- `-alpha` for builds on other branches
