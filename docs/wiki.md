# FeatureOne - Complete Guide & Documentation

Welcome to the official **FeatureOne Wiki**. FeatureOne is a high-performance, lightweight .NET feature flagging library supporting **.NET Standard 2.1**, **.NET 9.0**, and **.NET 10.0**, as well as full CNCF **OpenFeature Specification (v1.x)** compliance.

---

## Table of Contents

1. [Introduction](#introduction)
2. [What are Feature Toggles?](#what-are-feature-toggles)
3. [Benefits of Feature Toggles](#benefits-of-feature-toggles)
4. [Getting Started](#getting-started)
5. [Core Concepts](#core-concepts)
6. [Architecture Overview](#architecture-overview)
7. [Installation](#installation)
8. [Basic Usage](#basic-usage)
9. [OpenFeature Specification Provider](#openfeature-specification-provider) ⭐ NEW v6.0.0
10. [Dependency Injection Integration](#dependency-injection-integration)
11. [Storage Providers](#storage-providers)
12. [Condition Types](#condition-types)
13. [Advanced Configuration & Validation](#advanced-configuration--validation)
14. [Extending FeatureOne](#extending-featureone)
15. [Best Practices](#best-practices)
16. [Troubleshooting](#troubleshooting)
17. [API Reference](#api-reference)

---

## Introduction

**FeatureOne** is a feature toggle library for .NET applications. With FeatureOne, developers can wrap new functionality under conditional flag evaluations at runtime, permitting instant rollbacks, target user rollouts, time-based features, relational evaluations, and standardized OpenFeature integration without requiring code redeployments.

- **Target Frameworks**: `.NETStandard 2.1`, `.NET 9.0`, `.NET 10.0`
- **Supported Storage Backends**: Memory, SQL (MSSQL, SQLite, PostgreSQL, MySQL, ODBC, OleDb), File System (JSON), Custom `IStorageProvider`
- **OpenFeature Compliant**: Official `FeatureOneProvider` implementation in `FeatureOne.OpenFeature` package.

---

## What are Feature Toggles?

A **feature toggle** (or feature flag) is a software technique allowing developers to toggle application features "on" or "off" remotely.

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

Flag status is dynamically evaluated based on:
- **Storage Provider**: Fetches feature toggle definitions from storage medium.
- **Conditions**: Evaluates toggle rules against user claims (e.g. email, role, tier, date/time).
- **Operators**: Evaluates multiple conditions using logical `Operator.Any` (OR) or `Operator.All` (AND).

---

## Installation

Install NuGet packages according to your requirements:

### 1. Core Package (Custom Storage Provider)
```bash
dotnet add package FeatureOne --version 6.0.0
```

### 2. OpenFeature Specification Provider
```bash
dotnet add package FeatureOne.OpenFeature --version 6.0.0
```

### 3. SQL Storage Provider
```bash
dotnet add package FeatureOne.SQL --version 6.0.0
```

### 4. File System Storage Provider
```bash
dotnet add package FeatureOne.File --version 6.0.0
```

---

## OpenFeature Specification Provider (⭐ v6.0.0)

FeatureOne provides an official provider implementation (`FeatureOneProvider`) compliant with the CNCF **OpenFeature Specification (v1.x)**.

### Quick Start with OpenFeature SDK

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

// 2. Set FeatureOneProvider as global provider
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

### ASP.NET Core Dependency Injection Setup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IFeatureStore>(sp => new FeatureStore(storageProvider));
    services.AddFeatureOneOpenFeature(); // Automatically registers & sets as global provider
}
```

---

## Condition Types

FeatureOne provides four out-of-the-box condition strategies:

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
Evaluates a Regular Expression pattern against a user claim:
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

## API Reference & Project Structure

- **Core Library**: [FeatureOne](docs/DeveloperGuide.md)
- **OpenFeature Specification**: [FeatureOne.OpenFeature](docs/DeveloperGuide.md#featureoneopenfeature---openfeature-specification-provider)
- **Release Summary**: [Release Summary](docs/release-summary.md)
- **Release Table**: [Release Table](docs/release-table.md)
- **Changelog**: [CHANGELOG](docs/CHANGELOG.md)
