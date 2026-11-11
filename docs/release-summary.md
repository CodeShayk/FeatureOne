# FeatureOne Library Release Summary

## Overview
This document provides a comprehensive, technical summary of all FeatureOne library releases, detailing architectural enhancements, new condition types, OpenFeature specification compliance, and security improvements across every version.

---

## Release History

### Release v5.0.1 (Base Release)
- Base version of FeatureOne establishing core feature toggle evaluation engine.
- Supports primitive Simple and Regex toggle conditions.
- Defines extensible `IStorageProvider`, `IToggle`, `ICondition`, and `ICache` abstractions.

---

### Release v5.1.0 (Security & Architecture Minor Release)
- **Release Date**: October 13, 2025
- **Security Protections**:
  - ReDoS (Regular Expression Denial of Service) protection in `RegexCondition` with execution timeout enforcement.
  - Safe dynamic type loading in `ConditionDeserializer` against malicious assembly injection.
- **New Features**:
  - `DateRangeCondition` for time-windowed feature releases.
  - Configuration validation engine (`ConfigurationValidator`) to validate feature names and condition properties.
  - Dependency Injection support with `AddFeatureOne()`, `AddFeatureOneWithSQLStorage()`, and `AddFeatureOneWithFileStorage()`.
- **Architectural Enhancements**:
  - Prefix matching in `IFeatureStore.FindStartsWith()`.
  - Factory pattern initialization via `Features.Initialize(() => ...)` logic.

---

### Release v5.2.0 (Relational Conditions & Framework Upgrade)
- **Release Date**: March 18, 2026
- **New Features**:
  - `RelationalCondition` supporting 5 relational comparison operators (`Equals`, `NotEquals`, `GreaterThan`, `GreaterThanOrEqual`, `LessThanOrEqual`) for claim value evaluation.
- **Framework & Dependencies**:
  - Added `.NET 10.0` target framework (`net10.0`).
  - Upgraded Microsoft package dependencies to version `10.0.5`.
- **Quality**:
  - Expanded unit test suite coverage to 98%+ line coverage.

---

### Release v6.0.0 (Major Release: OpenFeature Specification Compliance)
- **Release Date**: August 16, 2026
- **Summary**:
  Major milestone release introducing full **CNCF OpenFeature Specification (v1.x)** compliance. The provider ships **inside the core `FeatureOne` package** under the `FeatureOne.OpenFeature` namespace — there is no separate package. FeatureOne now offers two equally supported APIs over the same evaluation engine: the native `IFeatures` API and the OpenFeature provider.

- **Key Highlights**:
  1. **OpenFeature Provider (built into `FeatureOne`)**:
     - Introduces `FeatureOneProvider` inheriting from OpenFeature's `FeatureProvider`.
     - Full support for typed flag evaluation: `ResolveBooleanValueAsync`, `ResolveStringValueAsync`, `ResolveIntegerValueAsync`, `ResolveDoubleValueAsync`, and `ResolveStructureValueAsync`. The non-boolean resolvers are projections of the boolean toggle result, not multivariate flag values.
     - Returns standard `ResolutionDetails<T>` with accurate `Reason` (`TARGETING_MATCH`, `DISABLED`, `ERROR`), `Variant` (`"on"`, `"off"`), and `ErrorType` (`FlagNotFound`, `ProviderNotReady`, `General`).
  2. **EvaluationContext Claim Mapping**:
     - Extension method `ToClaims()` mapping OpenFeature `EvaluationContext` attributes — strings, booleans, integers, doubles, `DateTime`, lists and structures — to FeatureOne user claims, using the invariant culture for numbers.
     - Automatic mapping of `TargetingKey` to standard claims (`"targetingKey"`, `"sub"`, `"user_id"`).
  3. **Core Facade Interoperability**:
     - Exposed `FeatureStore` property on `Features` facade (`Features.Current.FeatureStore`).
     - The native API remains fully backward compatible; storage providers and custom condition implementations are unaffected.
  4. **Dependency Injection**:
     - Added `AddFeatureOneOpenFeature()` extension methods for ASP.NET Core `IServiceCollection`, plus an options overload for hook configuration. Global provider registration runs from an `IHostedService` at application start rather than as a side effect of the DI factory.
  5. **Provider Lifecycle**:
     - `InitializeAsync` validates store availability and reports `ProviderStatus.Error`; a missing store surfaces as `ProviderNotReady` rather than `FlagNotFound`.
  6. **Custom Condition Registration**:
     - `ConditionDeserializer.Register<T>("Name")` registers user-defined `ICondition` types, preserving the deny-by-default type allow list from v5.1.0.
  7. **Comprehensive Verification**:
     - `test/FeatureOne.OpenFeature.Tests` expanded to 49 tests covering resolution types, context mapping, error codes, provider lifecycle, hook concurrency and DI registration.
     - Total test suite count expanded to **260 tests** with 100% pass rate.

---

## Release Timeline

| Version | Release Date | Type | Focus Area | Primary Goal |
|---|---|---|---|---|
| **v5.0.1** | Initial | Major | Core Feature Toggles | Base feature toggle engine and interfaces |
| **v5.1.0** | Oct 13, 2025 | Minor | Security & Architecture | ReDoS protection, safe type loading, `DateRangeCondition`, DI support |
| **v5.2.0** | Mar 18, 2026 | Minor | Relational Engine & .NET 10 | `RelationalCondition`, net10.0 framework support, MS 10.0.5 updates |
| **v6.0.0** | Aug 16, 2026 | Major | OpenFeature Compliance | Official `FeatureOneProvider`, OpenFeature spec compliance, claims mapping, DI helpers |

---

## Nuget Package Matrix (v6.0.0)

| Package | Latest Version | Description | Target Frameworks |
|---|---|---|---|
| **FeatureOne** | `6.0.0` | Core evaluation engine, condition strategies, native `IFeatures` API, and the built-in CNCF OpenFeature Specification provider (`FeatureOneProvider`, `FeatureOne.OpenFeature` namespace) | `netstandard2.1`, `net9.0`, `net10.0` |
| **FeatureOne.SQL** | `6.0.0` | SQL storage provider (MSSQL, SQLite, PostgreSQL, MySQL, ODBC, OleDb) | `netstandard2.1`, `net9.0`, `net10.0` |
| **FeatureOne.File** | `6.0.0` | File system storage provider with JSON toggle configuration | `netstandard2.1`, `net9.0`, `net10.0` |

---

## Upgrade & Migration Path

FeatureOne maintains a strict **zero-breaking-change** contract for existing public APIs:

1. **Upgrading from v5.x to v6.0.0**:
   - Update NuGet package references to `6.0.0`.
   - Existing code using `Features.Current.IsEnabled(...)` or custom `IStorageProvider` will continue working with zero modifications.
   - To integrate with the OpenFeature SDK, add `using FeatureOne.OpenFeature;` and call `builder.Services.AddFeatureOneOpenFeature()` — the provider is already in the `FeatureOne` package, so no extra install is needed.
   - Condition deserialization failures now throw `FeatureOneConfigurationException` (derived from `Exception`) instead of a bare `Exception`; update any code asserting on the exact exception type.

---

## Quality & Security Metrics

| Version | Test Count | Test Pass Rate | Line Coverage | Security Hardening |
|---|---|---|---|---|
| **v5.0.1** | ~20 | 100% | ~60% | Base |
| **v5.1.0** | 170 | 100% | 90%+ | ReDoS protection, type loading sandbox, input validation |
| **v5.2.0** | 198 | 100% | 98%+ | Relational comparison boundary validation |
| **v6.0.0** | 216 | 100% | 98%+ | Standardized OpenFeature spec error handling (`FlagNotFound`, `General`) |
