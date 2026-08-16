# FeatureOne - Complete Guide & Wiki Documentation

Welcome to the official **FeatureOne Wiki**. FeatureOne is a high-performance, lightweight feature flagging
library for .NET supporting **.NET Standard 2.1**, **.NET 9.0**, and **.NET 10.0**. It can be consumed
through its **native API** or through any **CNCF OpenFeature Specification (v1.x)** client — both are
first-class, fully supported, and back onto the same evaluation engine.

---

## Table of Contents

1. [Introduction](#introduction)
2. [Choosing Your API](#choosing-your-api)
3. [What are Feature Toggles?](#what-are-feature-toggles)
4. [Benefits of Feature Toggles](#benefits-of-feature-toggles)
5. [Installation](#installation)
6. [Native API Quick Start](#native-api-quick-start)
7. [OpenFeature Quick Start](#openfeature-quick-start)
8. [Dependency Injection Integration](#dependency-injection-integration)
9. [Condition Strategies](#condition-strategies)
10. [Storage Providers](#storage-providers)
11. [OpenFeature Compliance Matrix](#openfeature-compliance-matrix)
12. [OpenFeature Hooks & Lifecycle](#openfeature-hooks--lifecycle)
13. [Logging](#logging)
14. [Extending FeatureOne](#extending-featureone)
15. [Best Practices](#best-practices)
16. [Troubleshooting](#troubleshooting)
17. [API Reference & Project Structure](#api-reference--project-structure)

---

## Introduction

**FeatureOne** is a feature toggle library for enterprise .NET applications. It controls program features
dynamically at runtime using claims-based targeting, pluggable storage, and composable condition strategies.

- **Target Frameworks**: `.NETStandard 2.1`, `.NET 9.0`, `.NET 10.0`
- **Consumption models**: native `IFeatures` API, and CNCF OpenFeature Specification (v1.x) clients via `FeatureOneProvider`
- **Supported Storage Backends**: In-Memory, SQL (MSSQL, SQLite, PostgreSQL, MySQL, ODBC, OleDb), File System (JSON), and custom `IStorageProvider`

---

## Choosing Your API

Both APIs are supported equally and neither is deprecated. They share storage providers, conditions,
operators, caching, and logging — the difference is the call surface, not the capability.

| | **Native API** | **OpenFeature Provider** |
|---|---|---|
| Entry point | `IFeatures` / `Features.Current` | `OpenFeature.Api.Instance.GetClient()` |
| Namespace | `FeatureOne` | `FeatureOne.OpenFeature` |
| Call style | Synchronous `bool IsEnabled(...)` | Asynchronous `Task<bool> GetBooleanValueAsync(...)` |
| Targeting input | `ClaimsPrincipal`, `IEnumerable<Claim>`, `IDictionary<string,string>` | `EvaluationContext`, mapped to claims by `ToClaims()` |
| Missing flag | Returns `false`, logs a warning | Returns your default with `ErrorType.FlagNotFound` |
| Evaluation failure | Returns `false`, logs the exception | Returns your default with `ErrorType.General` and an error message |
| Observability | `IFeatureLogger` | `IFeatureLogger` plus the OpenFeature hook pipeline |
| Portability | FeatureOne-specific call sites | Vendor-neutral call sites, portable across flag backends |
| DI registration | `services.AddFeatureOneWith*Storage(...)` | The same, plus `services.AddFeatureOneOpenFeature()` |

**Choose the native API when** you want the smallest surface area, synchronous call sites (e.g. inside a
Razor view or a tight loop), or direct `ClaimsPrincipal` targeting from ASP.NET Core.

**Choose the OpenFeature provider when** you want vendor-neutral flag APIs, the option to swap flag backends
without touching call sites, or the OpenFeature hook pipeline for telemetry and auditing.

**Both at once** is fine — register your storage once and the two front doors read the same `IFeatureStore`.

---

## What are Feature Toggles?

A **feature toggle** (or feature flag) is a software engineering technique allowing developers to enable or disable application features remotely without code redeployments.

Native API:
```csharp
if (Features.Current.IsEnabled("dashboard_widget"))
    ShowDashboardWidget();
else
    ShowDefaultDashboard();
```

OpenFeature:
```csharp
if (await client.GetBooleanValueAsync("dashboard_widget", false))
    ShowDashboardWidget();
else
    ShowDefaultDashboard();
```

Flag evaluation combines:
- **Storage Provider**: Retrieves toggle definitions from your chosen storage backend.
- **Conditions**: Evaluates rules against user claims (e.g. role, email, user tier, time range).
- **Operators**: Evaluates constituent conditions using logical `Operator.Any` (OR) or `Operator.All` (AND).

---

## Benefits of Feature Toggles

1. **Risk Mitigation**: Instant rollbacks without redeploying code.
2. **Continuous Integration**: Safely merge incomplete features behind flags.
3. **Targeted Rollouts**: Target features based on user claims, roles, tiers, or date windows.
4. **Standardization**: Vendor-neutral standardization across teams via OpenFeature SDKs.

---

## Installation

Install NuGet packages according to your requirements:

### 1. Core Package (native API + built-in OpenFeature provider)
```bash
dotnet add package FeatureOne --version 6.0.0
```

### 2. SQL Storage Provider
```bash
dotnet add package FeatureOne.SQL --version 6.0.0
```

### 3. File System Storage Provider
```bash
dotnet add package FeatureOne.File --version 6.0.0
```

There is no separate OpenFeature package — `FeatureOneProvider` ships inside **FeatureOne** under the
`FeatureOne.OpenFeature` namespace.

---

## Native API Quick Start

```csharp
using FeatureOne;
using FeatureOne.Core.Stores;
using FeatureOne.File;

// 1. Initialize FeatureStore
var fileConfig = new FileConfiguration { FilePath = @"C:\Config\Features.json" };
var storageProvider = new FileStorageProvider(fileConfig);
var featureStore = new FeatureStore(storageProvider);

// 2. Initialize the global facade
Features.Initialize(() => new Features(featureStore));

// 3. Evaluate - with no targeting
bool isEnabled = Features.Current.IsEnabled("dashboard_widget");

// 3b. Evaluate - targeting an ASP.NET Core ClaimsPrincipal
bool isEnabledForUser = Features.Current.IsEnabled("dashboard_widget", User);

// 3c. Evaluate - targeting an explicit claims dictionary
bool isEnabledForClaims = Features.Current.IsEnabled("dashboard_widget", new Dictionary<string, string>
{
    ["email"] = "john@gbk.com",
    ["tier"] = "gold"
});
```

`IsEnabled` never throws: an unknown flag, an invalid feature name, or a storage failure returns `false` and
logs the reason through `IFeatureLogger`. Prefer injecting `IFeatures` over the `Features.Current` static
where you can — see [Dependency Injection Integration](#dependency-injection-integration).

---

## OpenFeature Quick Start

```csharp
using FeatureOne.Core.Stores;
using FeatureOne.File;
using FeatureOne.OpenFeature;
using OpenFeature;
using OpenFeature.Model;

// 1. Initialize FeatureStore
var fileConfig = new FileConfiguration { FilePath = @"C:\Config\Features.json" };
var storageProvider = new FileStorageProvider(fileConfig);
var featureStore = new FeatureStore(storageProvider);

// 2. Set FeatureOneProvider as global OpenFeature provider
await Api.Instance.SetProviderAsync(new FeatureOneProvider(featureStore));

// 3. Get OpenFeature Client
var client = Api.Instance.GetClient();

// 4. Create Evaluation Context with user attributes
var context = EvaluationContext.Builder()
    .SetTargetingKey("usr_98765")
    .Set("email", "john@gbk.com")
    .Set("tier", "gold")
    .Set("seats", 5)          // numbers, booleans and dates map to claims too
    .Build();

// 5. Evaluate Flag
bool isEnabled = await client.GetBooleanValueAsync("dashboard_widget", false, context);
```

### Context to claims mapping

`EvaluationContextExtensions.ToClaims()` converts an `EvaluationContext` into the claims dictionary the
conditions evaluate against:

| `EvaluationContext` input | Resulting claim value |
|---|---|
| `TargetingKey` | Set as `targetingKey`, `sub`, and `user_id` |
| String attribute | Used verbatim |
| Boolean attribute | `"true"` / `"false"` |
| Integer attribute | Invariant-culture digits, e.g. `95` |
| Double attribute | Invariant-culture round-trip form, e.g. `1.5` |
| `DateTime` attribute | ISO-8601 round-trip (`"o"`) format |
| List / structure attribute | JSON serialized |
| Null attribute | Omitted |

Claim keys are matched case-insensitively. Numbers always use the invariant culture so that
`RelationalCondition` parses them identically regardless of the ambient culture.

### A note on flag types

FeatureOne is a boolean toggle engine. The provider implements all five OpenFeature resolvers, but the
non-boolean ones are projections of the same boolean result:

| Resolver | Enabled | Disabled |
|---|---|---|
| `ResolveBooleanValueAsync` | `true` | `false` |
| `ResolveStringValueAsync` | `"true"` | `"false"` |
| `ResolveIntegerValueAsync` | `1` | `0` |
| `ResolveDoubleValueAsync` | `1.0` | `0.0` |
| `ResolveStructureValueAsync` | `Value(true)` | `Value(false)` |

These are **not** multivariate flag values — `GetStringValueAsync("theme", "dark")` returns `"true"` or
`"false"`, never a theme name. Prefer `GetBooleanValueAsync` unless a typed call site makes one of the
projections convenient.

---

## Dependency Injection Integration

### Native API

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Registers IStorageProvider, IFeatureLogger, IFeatureStore and IFeatures
    services.AddFeatureOneWithFileStorage(new FileConfiguration { FilePath = "Features.json" });
}

// Inject IFeatures
public class DashboardController(IFeatures features) : Controller
{
    public IActionResult Index()
        => features.IsEnabled("dashboard_widget", User) ? View("Widget") : View("Default");
}
```

### OpenFeature

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddFeatureOneWithFileStorage(new FileConfiguration { FilePath = "Features.json" });

    // Registers FeatureOneProvider and sets it as the global OpenFeature provider on application start
    services.AddFeatureOneOpenFeature();
}
```

Global provider registration runs from an `IHostedService` at application start, not from the DI factory, so
it happens whether or not anything in your application resolves `FeatureOneProvider`.

To configure hooks or opt out of global registration:

```csharp
services.AddFeatureOneOpenFeature(options =>
{
    options.SetAsGlobalProvider = true;
    options.EnableLoggingHook = true;      // bridges the hook pipeline to IFeatureLogger
    options.AddHook(new MyTelemetryHook());
});
```

Both registrations can coexist — `AddFeatureOneOpenFeature()` reuses the `IFeatureStore` registered by
`AddFeatureOneWith*Storage(...)`, so native and OpenFeature call sites evaluate against the same data.

---

## Condition Strategies

Condition strategies apply identically to both APIs — they are a property of the toggle definition, not of
the call surface. FeatureOne provides four built-in strategies out of the box:

### 1. `SimpleCondition`
Enables or disables a feature unconditionally:
```json
{
  "simple_flag": {
    "toggle": {
      "conditions": [{ "type": "Simple", "isEnabled": true }]
    }
  }
}
```

### 2. `RegexCondition`
Evaluates a Regular Expression pattern against a user claim (with built-in ReDoS timeout protection):
```json
{
  "admin_feature": {
    "toggle": {
      "conditions": [{
        "type": "Regex",
        "claim": "role",
        "expression": "^administrator$"
      }]
    }
  }
}
```

### 3. `RelationalCondition`
Evaluates user claim values against fixed targets using relational operators (`Equals`, `NotEquals`, `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`):
```json
{
  "tier_feature": {
    "toggle": {
      "conditions": [{
        "type": "Relational",
        "claim": "tier",
        "operator": "GreaterThanOrEqual",
        "value": "gold"
      }]
    }
  }
}
```

> **How values are compared.** When both the claim value and the configured value parse as numbers they are
> compared **numerically**; otherwise they are compared as **ordinal strings**. So `age > 18` behaves
> arithmetically, while `tier >= "gold"` still orders lexically. Numeric parsing uses the invariant culture
> and string comparison is ordinal, so results never vary with the ambient culture. Numeric comparison also
> applies to `Equals`, so a claim of `"5.0"` equals a configured value of `"5"`.

### 4. `DateRangeCondition`
Enables features strictly within a specified UTC start and end date/time window:
```json
{
  "holiday_sale": {
    "toggle": {
      "conditions": [{
        "type": "DateRange",
        "startDate": "2026-12-01T00:00:00Z",
        "endDate": "2026-12-25T23:59:59Z"
      }]
    }
  }
}
```

### Combining conditions
```json
{
  "beta_dashboard": {
    "toggle": {
      "operator": "All",
      "conditions": [
        { "type": "Regex", "claim": "email", "expression": ".*@gbk\\.com" },
        { "type": "DateRange", "startDate": "2026-09-01T00:00:00Z", "endDate": "2026-10-01T00:00:00Z" }
      ]
    }
  }
}
```
`Any` (the default) requires one condition to pass; `All` requires every condition to pass.

---

## Storage Providers

Storage is shared by both APIs.

### 1. SQL Storage (`FeatureOne.SQL`)
Supports MS SQL, SQLite, PostgreSQL, MySQL, ODBC, and OleDb databases.

```csharp
var sqlConfig = new SQLConfiguration
{
    ConnectionSettings = new ConnectionSettings
    {
        Providername = "System.Data.SqlClient",
        ConnectionString = "Server=localhost;Database=Features;Integrated Security=SSPI;"
    },
    FeatureTable = new FeatureTable
    {
        TableName = "[dbo].[TFeatures]",
        NameColumn = "[Name]",
        ToggleColumn = "[Toggle]",
        ArchivedColumn = "[Archived]"
    }
};

var storageProvider = new SQLStorageProvider(sqlConfig);

// Native API
Features.Initialize(() => new Features(new FeatureStore(storageProvider)));

// OpenFeature
await Api.Instance.SetProviderAsync(new FeatureOneProvider(new FeatureStore(storageProvider)));
```

### 2. File System Storage (`FeatureOne.File`)
Loads feature toggles from a JSON configuration file on disk.

```csharp
var fileConfig = new FileConfiguration { FilePath = @"C:\Work\Features.json" };
var storageProvider = new FileStorageProvider(fileConfig);

// Native API
Features.Initialize(() => new Features(new FeatureStore(storageProvider)));

// OpenFeature
await Api.Instance.SetProviderAsync(new FeatureOneProvider(new FeatureStore(storageProvider)));
```

---

## OpenFeature Compliance Matrix

| OpenFeature Feature | Spec | FeatureOne Implementation |
|---|---|---|
| **FeatureProvider** | 2.1 | `FeatureOneProvider` implementing `OpenFeature.FeatureProvider` |
| **Typed Flag Evaluation** | 2.2 | `ResolveBooleanValueAsync`, `ResolveStringValueAsync`, `ResolveIntegerValueAsync`, `ResolveDoubleValueAsync`, `ResolveStructureValueAsync` (see [flag types](#a-note-on-flag-types)) |
| **Resolution Details** | 2.3 | Returns `ResolutionDetails<T>` with `Reason` (`TARGETING_MATCH`, `DISABLED`, `ERROR`), `Variant` (`"on"`, `"off"`), and `ErrorType` (`FlagNotFound`, `ProviderNotReady`, `General`) |
| **Provider Lifecycle** | 2.4 | `InitializeAsync` / `ShutdownAsync` with status management (`NotReady`, `Ready`, `Error`). `InitializeAsync` reports `Error` when no `IFeatureStore` is available |
| **Events System** | 2.5 | Lifecycle events emitted by the OpenFeature SDK from the provider status transitions above |
| **Context Claims Mapping** | 3.1 | `EvaluationContextExtensions.ToClaims()` — see [mapping table](#context-to-claims-mapping) |
| **Hooks Architecture** | 4.1 | `Hook` support via `AddHook`, with `FeatureOneLoggingHook` bridging to `IFeatureLogger` |

### Error semantics

| Situation | `ErrorType` | Returned value |
|---|---|---|
| Flag absent from the store | `FlagNotFound` | Your default |
| No `IFeatureStore` available, or provider initialization failed | `ProviderNotReady` | Your default |
| Condition evaluation threw | `General` | Your default |

A missing store reports `ProviderNotReady`, deliberately distinct from `FlagNotFound`, so a misconfigured
application is not mistaken for a typo'd flag key.

---

## OpenFeature Hooks & Lifecycle

FeatureOne participates in OpenFeature's Hook pipeline (`BeforeAsync`, `AfterAsync`, `ErrorAsync`, `FinallyAsync`):

```csharp
var provider = new FeatureOneProvider(featureStore, logger);

// Bridge the hook pipeline to FeatureOne's native logger
provider.AddHook(new FeatureOneLoggingHook(logger));

await Api.Instance.SetProviderAsync(provider);
```

Or declaratively through DI:

```csharp
services.AddFeatureOneOpenFeature(options => options.EnableLoggingHook = true);
```

`AddHook` is safe to call while evaluations are in flight, though hooks are normally added at configuration
time.

---

## Logging

Both APIs log through the same `IFeatureLogger` abstraction. The default implementation bridges to
`Microsoft.Extensions.Logging`:

```csharp
public class MyLogger : IFeatureLogger
{
    public void Debug(string message) { }
    public void Info(string message) { }
    public void Warn(string message) { }
    public void Error(string message, Exception ex = null) { }
}
```

- **Native API**: pass it to the `Features` constructor, or register `IFeatureLogger` in DI.
- **OpenFeature**: pass it to the `FeatureOneProvider` constructor, or register `IFeatureLogger` in DI and set `options.EnableLoggingHook = true` to also capture the hook lifecycle.

---

## Extending FeatureOne

### Custom conditions

Implement `ICondition`, then register it so the deserializer can construct it from JSON. Condition types are
resolved from an explicit allow list rather than by loading arbitrary type names from configuration, so
registration is required:

```csharp
public class PercentageCondition : ICondition
{
    public int Threshold { get; set; }
    public string Claim { get; set; }

    public bool Evaluate(IDictionary<string, string> claims)
        => claims != null
           && claims.TryGetValue(Claim, out var raw)
           && int.TryParse(raw, out var value)
           && value <= Threshold;
}
```

```csharp
var conditions = new ConditionDeserializer()
    .Register<PercentageCondition>("Percentage");

services.AddFeatureOneWithFileStorage(
    new FileConfiguration { FilePath = "Features.json" },
    deserializer: new ToggleDeserializer(conditions));
```

```json
{
  "gradual_rollout": {
    "toggle": {
      "conditions": [{ "type": "Percentage", "claim": "bucket", "threshold": 25 }]
    }
  }
}
```

Registration requirements — violations throw `FeatureOneConfigurationException` at registration time:
- the type must implement `ICondition`
- the type must be concrete
- the type must expose a parameterless constructor (it may be non-public)

Names are normalised, so `"Percentage"` and `"PercentageCondition"` register and resolve identically.
Public writable properties are hydrated from matching JSON keys, matched case-insensitively.

Custom conditions work identically through both the native API and the OpenFeature provider.

### Custom storage providers

Implement `IStorageProvider` and register it:

```csharp
public class RedisStorageProvider : IStorageProvider
{
    public IEnumerable<IFeature> GetByName(string name) { /* ... */ }
}

services.AddFeatureOne(sp => new RedisStorageProvider(/* ... */));
```

### Custom caching

Implement `ICache` and pass it to the storage provider extension:

```csharp
services.AddFeatureOneWithSQLStorage(sqlConfig, cache: new MyDistributedCache());
```

---

## Best Practices

1. **Prefer injecting `IFeatures`** over the `Features.Current` static — it keeps evaluation testable and avoids global state.
2. **Always pass a sensible default** to OpenFeature resolvers. The default is what your users get when the store is unreachable, so it should be the safe path.
3. **Prefer `GetBooleanValueAsync`** on the OpenFeature side; the typed resolvers are boolean projections, not multivariate values.
4. **Keep flag keys stable and lowercase-insensitive.** Lookups are case-insensitive but prefix-based, so avoid flag names that are prefixes of other flag names where you can.
5. **Register custom conditions once at startup** and reuse the `ConditionDeserializer` instance — registrations are per-instance, not global.
6. **Set a cache expiry** appropriate to how quickly you need toggle changes to take effect; the default `FeatureCache` is process-local.
7. **Don't use toggles as authorization.** Conditions evaluate claims, but a toggle is a rollout mechanism, not a security boundary.
8. **Clean up stale toggles** once a feature is fully rolled out.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| Native `IsEnabled` always returns `false` | Flag missing from the store, invalid feature name, or a storage exception being swallowed | Enable an `IFeatureLogger` and check the warning/error output |
| OpenFeature returns `ProviderNotReady` | No `IFeatureStore` was available when the provider was constructed or initialized | Register `IFeatureStore` in DI, pass one to the constructor, or call `Features.Initialize` first |
| OpenFeature returns `FlagNotFound` for a flag that exists | Flag key mismatch, or the toggle has no conditions — `FeatureStore` filters out toggles with an empty condition list | Check the key and ensure the toggle defines at least one condition |
| A numeric `EvaluationContext` attribute never matches | Fixed in v6.0.0 — earlier builds dropped non-string attributes | Upgrade to v6.0.0 |
| `FeatureOneConfigurationException: Could not find a condition type` | The toggle JSON references a condition type that has not been registered | Register it with `ConditionDeserializer.Register<T>("Name")` |
| Global OpenFeature provider is never set | `setAsGlobalProvider` was disabled, or the host never starts hosted services | Use `AddFeatureOneOpenFeature()` on a host that runs `IHostedService`, or call `Api.Instance.SetProviderAsync` yourself |
| Toggle changes are not picked up | Cached toggle values have not expired | Lower the cache expiry in `CacheSettings`, or supply a custom `ICache` |

---

## API Reference & Project Structure

- **[Developer Guide](DeveloperGuide.md)**: Full implementation guide and C# code examples.
- **[Release Summary](release-summary.md)**: Technical summary of releases through v6.0.0.
- **[Release Table](release-table.md)**: Version matrix and backward compatibility guidelines.
- **[CHANGELOG](CHANGELOG.md)**: Detailed changelog by version.
