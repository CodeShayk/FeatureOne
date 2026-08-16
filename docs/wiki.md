# FeatureOne - Complete Guide & Wiki Documentation

Welcome to the official **FeatureOne Wiki**. FeatureOne is a high-performance, lightweight, and fully **CNCF OpenFeature Specification (v1.x)** compliant feature flagging library for .NET supporting **.NET Standard 2.1**, **.NET 9.0**, and **.NET 10.0**.

---

## Table of Contents

1. [Introduction](#introduction)
2. [OpenFeature Specification & Compliance](#openfeature-specification--compliance) ⭐ NEW v6.0.0
3. [What are Feature Toggles?](#what-are-feature-toggles)
4. [Benefits of Feature Toggles](#benefits-of-feature-toggles)
5. [Installation](#installation)
6. [OpenFeature Quick Start Guide](#openfeature-quick-start-guide)
7. [Dependency Injection Integration](#dependency-injection-integration)
8. [Condition Strategies](#condition-strategies)
9. [Storage Providers](#storage-providers)
10. [OpenFeature Hooks & Lifecycle Adaptability](#openfeature-hooks--lifecycle-adaptability)
11. [Advanced Configuration & Validation](#advanced-configuration--validation)
12. [Extending FeatureOne](#extending-featureone)
13. [Best Practices](#best-practices)
14. [Troubleshooting](#troubleshooting)
15. [API Reference & Project Structure](#api-reference--project-structure)

---

## Introduction

**FeatureOne** is a vendor-neutral feature toggle library for enterprise .NET applications. FeatureOne enables developers to control program features dynamically at runtime using **CNCF OpenFeature Specification (v1.x)** standard clients (`FeatureOneProvider`), while leveraging native condition strategies, custom storage providers, and caching mechanisms.

- **Target Frameworks**: `.NETStandard 2.1`, `.NET 9.0`, `.NET 10.0`
- **Specification Compliance**: CNCF OpenFeature Specification (v1.x)
- **Supported Storage Backends**: In-Memory, SQL (MSSQL, SQLite, PostgreSQL, MySQL, ODBC, OleDb), File System (JSON), and Custom `IStorageProvider`

---

## OpenFeature Specification & Compliance (⭐ v6.0.0)

FeatureOne includes an official **OpenFeature Specification Provider** (`FeatureOneProvider` under the `FeatureOne.OpenFeature` namespace) built directly into the core `FeatureOne` library.

### Compliance Matrix

| OpenFeature Feature | Spec Compliance | FeatureOne Implementation |
|---|---|---|
| **FeatureProvider** | Spec 2.1 | `FeatureOneProvider` implementing `OpenFeature.FeatureProvider` |
| **Typed Flag Evaluation** | Spec 2.2 | Full support for `ResolveBooleanValueAsync`, `ResolveStringValueAsync`, `ResolveIntegerValueAsync`, `ResolveDoubleValueAsync`, and `ResolveStructureValueAsync` |
| **Resolution Details** | Spec 2.3 | Returns `ResolutionDetails<T>` with accurate `Reason` (`TARGETING_MATCH`, `DISABLED`, `ERROR`), `Variant` (`"on"`, `"off"`), and `ErrorType` (`FlagNotFound`, `ProviderNotReady`, `General`) |
| **Provider Lifecycle** | Spec 2.4 | Implements `InitializeAsync`, `ShutdownAsync`, and status management (`ProviderStatus.NotReady`, `Ready`, `Error`) |
| **Events System** | Spec 2.5 | Emits standard lifecycle events (`ProviderReady`, `ProviderError`, `ProviderConfigurationChanged`, `ProviderStale`) |
| **Context Claims Mapping** | Spec 3.1 | Maps `TargetingKey` and attributes to FeatureOne user claims via `EvaluationContextExtensions.ToClaims()` |
| **Hooks Architecture** | Spec 4.1 | Extends `Hook` with `FeatureOneLoggingHook` (participates in `BeforeAsync`, `AfterAsync`, `ErrorAsync`, `FinallyAsync`) |

---

## What are Feature Toggles?

A **feature toggle** (or feature flag) is a software engineering technique allowing developers to enable or disable application features remotely without code redeployments.

```csharp
var featureName = "dashboard_widget";
if (Features.Current.IsEnabled(featureName)) 
{
    ShowDashboardWidget();
}
else 
{
    ShowDefaultDashboard();
}
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

### 1. Core Package (with built-in OpenFeature support)
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

---

## OpenFeature Quick Start Guide

### 1. Basic OpenFeature Client Evaluation

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
    .Build();

// 5. Evaluate Flag
bool isEnabled = await client.GetBooleanValueAsync("dashboard_widget", false, context);
```

---

## Dependency Injection Integration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register FeatureStore
    services.AddSingleton<IFeatureStore>(sp => new FeatureStore(storageProvider));
    
    // Registers FeatureOneProvider and sets as global OpenFeature provider
    services.AddFeatureOneOpenFeature(); 
}
```

---

## Condition Strategies

FeatureOne provides four built-in condition strategies out-of-the-box:

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
Evaluates user claim values against fixed targets using relational operators (`Equals`, `NotEquals`, `GreaterThan`, `GreaterThanOrEqual`, `LessThanOrEqual`):
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

---

## Storage Providers

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
Features.Initialize(() => new Features(new FeatureStore(storageProvider)));
```

### 2. File System Storage (`FeatureOne.File`)
Loads feature toggles from a JSON configuration file on disk.

```csharp
var fileConfig = new FileConfiguration { FilePath = @"C:\Work\Features.json" };
var storageProvider = new FileStorageProvider(fileConfig);
Features.Initialize(() => new Features(new FeatureStore(storageProvider)));
```

---

## OpenFeature Hooks & Lifecycle Adaptability

FeatureOne adapts to OpenFeature's Hook pipeline (`BeforeAsync`, `AfterAsync`, `ErrorAsync`, `FinallyAsync`):

```csharp
var provider = new FeatureOneProvider(featureStore, logger);

// Add native logger hook adapter to provider
provider.AddHook(new FeatureOneLoggingHook(logger));

await Api.Instance.SetProviderAsync(provider);
```

---

## API Reference & Project Structure

- **[Developer Guide](docs/DeveloperGuide.md)**: Full implementation guide and C# code examples.
- **[Release Summary](docs/release-summary.md)**: Technical summary of releases through v6.0.0.
- **[Release Table](docs/release-table.md)**: Version matrix and backward compatibility guidelines.
- **[CHANGELOG](docs/CHANGELOG.md)**: Detailed changelog by version.
