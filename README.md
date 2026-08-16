# <img src="https://github.com/CodeShayk/FeatureOne/blob/master/images/feature-flag.png" alt="feature-flag" style="width:60px;"/> FeatureOne v6.0.0

[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/FeatureOne?logo=github&sort=semver)](https://github.com/CodeShayk/FeatureOne/releases/latest)
[![OpenFeature](https://img.shields.io/badge/OpenFeature-Compliant-brightgreen)](https://openfeature.dev/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/FeatureOne/blob/master/License.md)
[![build-master](https://github.com/CodeShayk/FeatureOne/actions/workflows/Build-Master.yml/badge.svg)](https://github.com/CodeShayk/FeatureOne/actions/workflows/Build-Master.yml)
[![CodeQL](https://github.com/CodeShayk/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/CodeShayk/FeatureOne/actions/workflows/codeql.yml)
[![.Net](https://img.shields.io/badge/.Net_Standard-2.1-green)](https://dotnet.microsoft.com/en-us/download/netstandard/2.1)
[![.Net](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
[![.Net](https://img.shields.io/badge/.Net-10.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

> **FeatureOne** is a high-performance, lightweight, and fully **CNCF OpenFeature Specification (v1.x)** compliant feature flagging library for .NET applications.

---

## 🚀 OpenFeature Specification & Compliance

FeatureOne features a native **OpenFeature Specification Provider** (`FeatureOneProvider` under the `FeatureOne.OpenFeature` namespace) built directly into the core library. This allows you to evaluate feature toggles using vendor-neutral OpenFeature SDK clients while leveraging FeatureOne's powerful condition strategies, custom storage providers, and caching mechanisms.

### Key OpenFeature Highlights
- **Standardized Provider (`FeatureOneProvider`)**: Implements `OpenFeature.FeatureProvider` for vendor-agnostic feature flagging.
- **Typed Flag Evaluation**: Supports `Boolean`, `String`, `Integer`, `Double`, and `Structure` flag resolutions.
- **Evaluation Context Claims Mapping**: Converts OpenFeature `TargetingKey` and `EvaluationContext` attributes seamlessly to FeatureOne user claims.
- **Full Hook Lifecycle Adaptability**: Participates in OpenFeature's 5-stage Hook pipeline (`BeforeAsync` $\rightarrow$ `Resolve` $\rightarrow$ `AfterAsync` / `ErrorAsync` $\rightarrow$ `FinallyAsync`) with native logger hooks (`FeatureOneLoggingHook`).
- **Provider Status & Events**: Fully manages provider lifecycle states (`ProviderStatus.NotReady`, `Ready`, `Error`) and event propagation.
- **ASP.NET Core DI Integration**: Easily registered via `services.AddFeatureOneOpenFeature()`.

```csharp
using FeatureOne.OpenFeature;
using OpenFeature;
using OpenFeature.Model;

// Register FeatureOne as the global OpenFeature provider
await Api.Instance.SetProviderAsync(new FeatureOneProvider());

// Evaluate flags using standard OpenFeature Client
var client = Api.Instance.GetClient();
var context = EvaluationContext.Builder().SetTargetingKey("usr_123").Set("tier", "gold").Build();

bool showWidget = await client.GetBooleanValueAsync("dashboard_widget", false, context);
```

---

#### NuGet Packages
| Package | Latest | Details |
|---|---|---|
| **FeatureOne** | [![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) | Core feature evaluation engine and built-in **CNCF OpenFeature Specification (v1.x)** provider (`FeatureOneProvider` under `FeatureOne.OpenFeature` namespace). **v6.0.0**: OpenFeature provider built directly into core library. |
| **FeatureOne.SQL** | [![NuGet version](https://badge.fury.io/nu/FeatureOne.SQL.svg)](https://badge.fury.io/nu/FeatureOne.SQL) | SQL storage provider for implementing feature toggles using relational database backends (MSSQL, SQLite, PostgreSQL, MySQL). |
| **FeatureOne.File** | [![NuGet version](https://badge.fury.io/nu/FeatureOne.File.svg)](https://badge.fury.io/nu/FeatureOne.File) | File storage provider for implementing feature toggles using JSON configuration files. |

---

## Concept

### What is a Feature Toggle?
Feature toggle is a mechanism that allows code to be turned “on” or “off” remotely without requiring a deployment. Feature toggles are commonly used in applications to gradually roll out new features, test changes on a small subset of users, or instantly disable features during emergencies.

### How Feature Toggles Work
Feature toggle is typically a logical check wrapped around application code to execute or skip functionality based on evaluated status at runtime.

### Benefits of Feature Toggles
- **Risk Mitigation**: Instant rollbacks without redeploying code.
- **Continuous Delivery**: Merge incomplete code safely behind toggles.
- **Targeted Rollouts**: Release features based on user claims, roles, tiers, date ranges, or regular expressions.
- **Vendor-Neutral Standardization**: Standardize feature flagging across your organization via OpenFeature SDKs.

---

## Getting Started

### i. Installation
Install the latest NuGet package as appropriate for your project:

`FeatureOne` - Core library with built-in OpenFeature provider support.
```bash
NuGet\Install-Package FeatureOne
```

`FeatureOne.SQL` - SQL storage provider for database backends.
```bash
NuGet\Install-Package FeatureOne.SQL
```

`FeatureOne.File` - File system storage provider for JSON configurations.
```bash
NuGet\Install-Package FeatureOne.File
```

### ii. Developer Guide & Documentation

- **[Developer Guide](docs/DeveloperGuide.md)**: In-depth setup, custom condition creation, storage provider implementations, and ASP.NET Core DI extensions.
- **[GitHub Wiki](docs/wiki.md)**: Complete guide and API reference for FeatureOne & OpenFeature integration.

---

## Support

If you encounter issues or have questions, please [raise a new issue](https://github.com/CodeShayk/FeatureOne/issues/new/choose).

## License

This project is licensed under the [MIT License](LICENSE).

---

## Version History
The following previous versions are available:

| Version | Release Notes |
|---|---|
| [`v6.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v6.0.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v6.0.0) |
| [`v5.2.0`](https://github.com/CodeShayk/FeatureOne/tree/v5.2.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v5.2.0) |
| [`v5.1.0`](https://github.com/CodeShayk/FeatureOne/tree/v5.1.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v5.1.0) |
| [`v5.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v5.0.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v5.0.0) |
| [`v4.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v4.0.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v4.0.0) |
| [`v3.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v3.0.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v3.0.0) |
| [`v2.0.0`](https://github.com/CodeShayk/FeatureOne/tree/v2.0.0) | [Notes](https://github.com/CodeShayk/FeatureOne/releases/tag/v2.0.0) |

---

## Recent Releases

| Version | Release Date | Type | Key Changes | Backward Compatibility |
|---|---|---|---|---|
| **v5.0.0** | Previous | Initial | Core feature toggle functionality | N/A (Initial release) |
| **v5.1.0** | Nov 03, 2025 | Minor | **Security fixes** (ReDoS protection, secure type loading), **architectural improvements** (prefix matching, dependency injection), **new features** (DateRangeCondition, configuration validation), **DI integration** | High - maintains all existing functionality with minor security-related behavioral changes |
| **v5.2.0** | Mar 18, 2026 | Minor | **New condition** (RelationalCondition with 5 relational operators), **target framework** (added net10.0, removed netstandard2.0 and net8.0), **package upgrades** (all MS packages to 10.0.5), **expanded test coverage** (98%+ line coverage) | High - fully backward compatible, additive changes only |
| **v6.0.0** | Aug 16, 2026 | Major | **OpenFeature Specification Compliance** (official `FeatureOneProvider` implementation in core `FeatureOne` package under `FeatureOne.OpenFeature` namespace, `EvaluationContext` claims mapping, typed flag evaluation, DI extensions) | High - 100% backward compatible, additive features only |

---

## Credits
Thank you for exploring FeatureOne. Please fork, contribute, report issues, and star the repo! Happy Coding !! :)
