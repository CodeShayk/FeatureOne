# <img src="https://github.com/CodeShayk/FeatureOne/blob/master/images/feature-flag.png" alt="feature-flag" style="width:60px;"/> FeatureOne v6.0.0

[![GitHub Release](https://img.shields.io/github/v/release/CodeShayk/FeatureOne?logo=github&sort=semver)](https://github.com/CodeShayk/FeatureOne/releases/latest)
[![OpenFeature](https://img.shields.io/badge/OpenFeature-Compliant-brightgreen)](https://openfeature.dev/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CodeShayk/FeatureOne/blob/master/License.md)
[![build-master](https://github.com/CodeShayk/FeatureOne/actions/workflows/Build-Master.yml/badge.svg)](https://github.com/CodeShayk/FeatureOne/actions/workflows/Build-Master.yml)
[![CodeQL](https://github.com/CodeShayk/FeatureOne/actions/workflows/codeql.yml/badge.svg)](https://github.com/CodeShayk/FeatureOne/actions/workflows/codeql.yml)
[![.Net](https://img.shields.io/badge/.Net_Standard-2.1-green)](https://dotnet.microsoft.com/en-us/download/netstandard/2.1)
[![.Net](https://img.shields.io/badge/.Net-9.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
[![.Net](https://img.shields.io/badge/.Net-10.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

> **FeatureOne** is a high-performance, lightweight feature flagging library for .NET. Use it through its
> native API, or through any **CNCF OpenFeature Specification (v1.x)** client — both are first-class,
> fully supported ways to consume the same evaluation engine.

---

## Two Ways to Use FeatureOne

FeatureOne ships one evaluation engine — storage providers, condition strategies, caching and claims-based
targeting — with two equally supported front doors. Pick whichever fits your codebase; you can also mix them
in the same application, since both read the same `IFeatureStore`.

| | **Native API** | **OpenFeature Provider** |
|---|---|---|
| Entry point | `IFeatures` / `Features.Current` | `OpenFeature.Api.Instance.GetClient()` |
| Call style | Synchronous `IsEnabled(...)` | Asynchronous `GetBooleanValueAsync(...)` |
| Targeting input | `ClaimsPrincipal`, `IEnumerable<Claim>`, or a claims dictionary | `EvaluationContext` (mapped to claims automatically) |
| Extra dependency | None | OpenFeature SDK (already included in the `FeatureOne` package) |
| Choose it when | You want the smallest possible surface area, synchronous call sites, or direct use of ASP.NET Core `ClaimsPrincipal` | You want vendor-neutral flag APIs, portability across flag backends, or OpenFeature hooks and telemetry |

Neither is a wrapper around the other in a way that costs you functionality: conditions, operators, storage
providers and caching behave identically through both.

### A. Native API

```csharp
using FeatureOne;
using FeatureOne.Core.Stores;
using FeatureOne.File;

// 1. Build the store over your chosen storage provider
var fileConfig = new FileConfiguration { FilePath = @"C:\Config\Features.json" };
var featureStore = new FeatureStore(new FileStorageProvider(fileConfig));

// 2. Initialize the global facade (or inject IFeatures - see below)
Features.Initialize(() => new Features(featureStore));

// 3. Evaluate
if (Features.Current.IsEnabled("dashboard_widget", User))   // User is a ClaimsPrincipal
{
    ShowDashboardWidget();
}
```

With dependency injection:

```csharp
services.AddFeatureOneWithFileStorage(new FileConfiguration { FilePath = "Features.json" });

// then inject IFeatures anywhere
public class DashboardController(IFeatures features)
{
    public IActionResult Index()
        => features.IsEnabled("dashboard_widget", User) ? View("Widget") : View("Default");
}
```

### B. OpenFeature Provider

```csharp
using FeatureOne.Core.Stores;
using FeatureOne.File;
using FeatureOne.OpenFeature;
using OpenFeature;
using OpenFeature.Model;

// 1. Build the store over your chosen storage provider
var fileConfig = new FileConfiguration { FilePath = @"C:\Config\Features.json" };
var featureStore = new FeatureStore(new FileStorageProvider(fileConfig));

// 2. Register FeatureOne as the OpenFeature provider
await Api.Instance.SetProviderAsync(new FeatureOneProvider(featureStore));

// 3. Evaluate through a standard OpenFeature client
var client = Api.Instance.GetClient();
var context = EvaluationContext.Builder()
    .SetTargetingKey("usr_123")
    .Set("tier", "gold")
    .Build();

bool showWidget = await client.GetBooleanValueAsync("dashboard_widget", false, context);
```

With dependency injection:

```csharp
services.AddFeatureOneWithFileStorage(new FileConfiguration { FilePath = "Features.json" });
services.AddFeatureOneOpenFeature();   // registers the provider and sets it globally on startup

// then inject IFeatureClient anywhere, or resolve Api.Instance.GetClient()
```

The `FeatureOneProvider` lives in the `FeatureOne.OpenFeature` namespace inside the core **FeatureOne**
package — there is no separate package to install.

> **Note on flag types.** FeatureOne is a boolean toggle engine. The provider implements all five
> OpenFeature resolvers, but the non-boolean ones are projections of the same boolean result
> (`String` → `"true"`/`"false"`, `Integer` → `1`/`0`, `Double` → `1.0`/`0.0`, `Structure` → a wrapped
> boolean). They are not multivariate flag support. Prefer `GetBooleanValueAsync` unless a typed call site
> makes one of the projections convenient.

---

#### NuGet Packages
| Package | Latest | Details |
|---|---|---|
| **FeatureOne** | [![NuGet version](https://badge.fury.io/nu/FeatureOne.svg)](https://badge.fury.io/nu/FeatureOne) | Core evaluation engine, native `IFeatures` API, and the built-in **CNCF OpenFeature Specification (v1.x)** provider (`FeatureOneProvider`, `FeatureOne.OpenFeature` namespace). |
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

## Capabilities

Available identically through both the native API and the OpenFeature provider:

- **Condition strategies**: `SimpleCondition`, `RegexCondition` (with ReDoS timeout protection), `RelationalCondition`, `DateRangeCondition`, plus your own via `ICondition`.
- **Operators**: combine conditions with `Operator.Any` (OR) or `Operator.All` (AND).
- **Storage providers**: SQL, JSON file, or your own `IStorageProvider`.
- **Caching**: pluggable `ICache` with configurable expiry.
- **Custom conditions**: register your own condition types with `ConditionDeserializer.Register<T>("Name")`.
- **Logging**: pluggable `IFeatureLogger`; on the OpenFeature side, `FeatureOneLoggingHook` bridges the hook pipeline to the same logger.

---

## Getting Started

### i. Installation
Install the latest NuGet package as appropriate for your project:

`FeatureOne` - Core library, native API, and built-in OpenFeature provider.
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
- **[GitHub Wiki](docs/wiki.md)**: Complete guide and API reference, covering both the native API and OpenFeature integration.

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
| **v6.0.0** | Aug 16, 2026 | Major | **OpenFeature Specification Compliance** (`FeatureOneProvider` in the core `FeatureOne` package under the `FeatureOne.OpenFeature` namespace, `EvaluationContext` claims mapping, typed flag evaluation, DI extensions), **custom condition registration** (`ConditionDeserializer.Register<T>`) | High - backward compatible for the native API; condition deserialization now throws `FeatureOneConfigurationException` instead of `Exception` |

---

## Credits
Thank you for exploring FeatureOne. Please fork, contribute, report issues, and star the repo! Happy Coding !! :)
