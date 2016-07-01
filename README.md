[![Build status](https://ci.appveyor.com/api/projects/status/4008w686pk2b5ghm/branch/master?svg=true)](https://ci.appveyor.com/project/NIPOSoftware/nfield-quota/branch/master) [![NuGet version](https://badge.fury.io/nu/Nfield.Quota.svg)](https://badge.fury.io/nu/Nfield.Quota)
# Nfield.Quota

##Introduction
Using this project developers can create quota structures for Nfield Online surveys. For more information about Nfield and/or NIPO Software please visit the [NIPO Software] website.

##Installation
The recommended way to consume this project is to reference the NuGet package. You can install it by executing the following command in the Package Manager Console.

```
PM> Install-Package Nfield.Quota
```

##Release procedure
This project uses [AppVeyor] for continuous integration. Commits to the development branch result in a prerelease package on [NuGet]. Commits into the master branch will publish a new release on [NuGet] and also a [draft release] on [GitHub]. This release can then be amended with information on what's new before being published.

[NIPO Software]: http://www.niposoftware.com
[AppVeyor]: http://www.appveyor.com
[NuGet]: http://nuget.org
[GitHub]: https://github.com
[draft release]: https://github.com/NIPOSoftware/Nfield.Quota/releases