
# <img src="https://github.com/NinjaRocks/FeatureOne/blob/master/ninja-icon-16.png" alt="ninja" style="width:30px;"/> FeatureOne v5.1.0
[![GitHub Release](https://img.shields.io/github/v/release/ninjarocks/FeatureOne?logo=github&sort=semver)](https://github.com/ninjarocks/FeatureOne/releases/latest)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/NinjaRocks/FeatureOne/blob/master/License.md) [![build-master](https://github.com/NinjaRocks/FeatureOne/actions/workflows/Build-Master.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/Build-Master.yml)
[![CodeQL](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/NinjaRocks/FeatureOne/actions/workflows/codeql.yml)
[![.Net](https://img.shields.io/badge/.Net_Framework-4.6.2-blue)](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net46)
[![.Net](https://img.shields.io/badge/.Net_Standard-2.1-blue)](https://dotnet.microsoft.com/en-us/download/netstandard/2.1)
[![.Net](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

.Net Library to implement feature toggles.
--
#### Nuget Packages
| Package  | Latest | Details | 
| --------| --------| --------|
|FeatureOne |[![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) | Provides core functionality to implement feature toggles with `no` backend storage provider. Needs package consumer to provide `IStorageProvider` implementation. Ideal for use case that requires custom storage backend. **v5.1.0**: Security fixes, DI integration, DateRangeCondition. |
|FeatureOne.SQL| [![NuGet version](https://badge.fury.io/nu/FeatureOne.SQL.svg)](https://badge.fury.io/nu/FeatureOne.SQL) | Provides SQL storage provider for implementing feature toggles using `SQL` backend. **v5.1.0**: Security fixes, DI integration, enhanced configuration. |
|FeatureOne.File |[![NuGet version](https://badge.fury.io/nu/FeatureOne.File.svg)](https://badge.fury.io/nu/FeatureOne.File) | Provides File storage provider for implementing feature toggles using `File System` backend. **v5.1.0**: Security fixes, DI integration, enhanced configuration. |

## Concept
### What is a feature toggle?
Feature toggle is a mechanism that allows code to be turned “on” or “off” remotely without the need for a deploy. Feature toggles are commonly used in applications to gradually roll out new features, allowing teams to test changes on a small subset of users before releasing them to everyone.

### How feature toggles work
Feature toggle is typically a logical check added to codebase to execute or ignore certain functionality in context based on evaluated status of the toggle at runitme.

In code, the functionality to be released is wrapped so that it can be controlled by the status of a feature toggle. If the status of the feature toggle is “on”, then the wrapped functionality is executed. If the status of the feature toggle is “off”, then the wrapped functionality is skipped.  The statuses of each feature is provided by a store provider external to the application.

### The benefits of feature toggles
The primary benefit of feature flagging is that it mitigates the risks associated with releasing changes to an application. Whether it be a new feature release or a small refactor, there is always the inherent risk of releasing new regressions. To mitigate this, changes to an application can be placed behind feature toggles, allowing them to be turned “on” or “off” in the event of an emergency.

## Getting Started?
### i. Installation
Install the latest nuget package as appropriate. 

`FeatureOne` - for installing FeatureOne for custom `IStorageProvider` implementation.
```
NuGet\Install-Package FeatureOne
```
`FeatureOne.SQL` - for installing FeatureOne with SQL storage provider.
```
NuGet\Install-Package FeatureOne.SQL
```
`FeatureOne.File` - for installing FeatureOne with File system storage provider.
```
NuGet\Install-Package FeatureOne.File
```

### ii. Developer Guide

Please see [Developer Guide](/DeveloperGuide.md) for details on how to implement FeatureOne in your project.

## Support

If you are having problems, please let me know by [raising a new issue](https://github.com/CodeShayk/FeatureOne/issues/new/choose).

## License

This project is licensed with the [MIT license](LICENSE).

## Version History
The following previous versions are available:

| Version                                                         | Release Notes                                                         |
| ----------------------------------------------------------------| ----------------------------------------------------------------------|
| [`v5.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v5.0.0) |  [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v5.0.0) |
| [`v4.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v4.0.0) |  [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v4.0.0) |
| [`v3.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v3.0.0) |  [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v3.0.0) |
| [`v2.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v2.0.0) |  [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v2.0.0) |

## Recent Releases

| Version | Release Date | Type | Key Changes | Backward Compatibility |
|--------|-------------|------|-------------|---------------------|
| v5.0.0 | Previous | Initial | Core feature toggle functionality | N/A (Initial release) |
| v5.1.0 | Nov 03, 2025 | Minor | **Security fixes** (ReDoS protection, secure type loading), **architectural improvements** (prefix matching, dependency injection), **new features** (DateRangeCondition, configuration validation), **DI integration** | High - maintains all existing functionality with minor security-related behavioral changes |

## Credits
Thank you for reading. Please fork, explore, contribute and report. Happy Coding !! :)


