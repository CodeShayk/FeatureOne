# Changelog

## [6.0.0] - 2026-08-16

### Added
- **OpenFeature Provider**: `FeatureOneProvider` implementing the OpenFeature Specification (v1.x) `FeatureProvider`, shipped **inside the core `FeatureOne` package** under the `FeatureOne.OpenFeature` namespace. Full support for Boolean, String, Integer, Double, and Structure flag resolution (the non-boolean resolvers are projections of the boolean result, not multivariate values).
- **Context Mapping**: `EvaluationContextExtensions` mapping OpenFeature `TargetingKey` and attributes to FeatureOne user claims.
- **Dependency Injection**: `AddFeatureOneOpenFeature` extension methods for ASP.NET Core `IServiceCollection`, including an options overload for hook configuration. Global provider registration runs from an `IHostedService` at startup rather than from the DI factory.
- **Custom Condition Registration**: `ConditionDeserializer.Register<T>("Name")` for user-defined `ICondition` types, preserving the deny-by-default type allow list introduced in v5.1.0.
- **Core Interoperability**: Exposed `FeatureStore` property on `Features` class for open provider integration.

### Fixed
- **Non-string `EvaluationContext` attributes were silently dropped** during claims mapping — integer, double, list and structure attributes never reached conditions, which then evaluated as disabled with no error reported.
- **`InitializeAsync` could never report failure**, leaving every provider error-state guard unreachable.
- **A missing feature store reported `FlagNotFound`**, indistinguishable from a genuinely absent flag; it now reports `ProviderNotReady`.
- **Global OpenFeature provider registration was a side effect of the DI factory**, so it never ran unless something resolved `FeatureOneProvider`, and it blocked on an async call inside the factory.
- **`ConditionDeserializer` re-parsed the toggle JSON once per writable property** during hydration.
- **`RelationalOperator.LessThan` was never implemented** — it fell through to the switch default and always returned `false`.
- **`RelationalCondition` compared numeric claims lexicographically**, ranking `"9"` above `"18"`.
- **`RelationalCondition` string comparison was culture-sensitive**, making results machine-dependent.

### Changed
- Condition deserialization failures now throw `FeatureOneConfigurationException` (derived from `Exception`) instead of a bare `Exception`.
- `RelationalCondition` now compares numerically when both the claim value and the configured value parse as numbers, and ordinally otherwise. Toggles comparing numeric claims with `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, or `LessThanOrEqual` will evaluate differently — correctly — than in v5.2.0. `Equals` also matches equivalent numeric forms, so `"5.0"` now equals `"5"`.

### Quality & Testing
- **Test Suite**: 260 tests passing across all four test projects, including 49 in `FeatureOne.OpenFeature.Tests` covering resolution types, context mapping, error codes, provider lifecycle, hook concurrency, DI registration, and global client integration.

## [5.2.0] - 2026-03-18

### Added
- **New Feature**: `RelationalCondition` with 5 relational operators (`Equals`, `NotEquals`, `GreaterThan`, `GreaterThanOrEqual`, `LessThanOrEqual`).
- **Framework Support**: Added `net10.0` target framework.

## [5.1.0] - 2025-10-13

### Security
- **Critical**: Fixed RegexCondition ReDoS (Regular Expression Denial of Service) vulnerability by adding timeout validation
- **Critical**: Secured dynamic type loading in ConditionDeserializer to prevent unsafe type loading
- **Enhanced**: Added comprehensive input validation to prevent injection attacks
- **Enhanced**: Implemented timeout protections to prevent resource exhaustion

### Added
- **New Feature**: DateRangeCondition for time-based feature toggles with flexible date range support
- **New Feature**: Configuration validation system for feature names and condition parameters
- **New Feature**: Dependency Injection integration with extension methods for Microsoft.Extensions.DependencyInjection
- **New Feature**: Factory function support for dynamic configuration
- **New Feature**: Comprehensive service registration for all FeatureOne packages (Core, File, SQL)

### Changed
- **Architecture**: Updated FindStartsWith implementation to properly support actual prefix matching instead of exact matching
- **Architecture**: Implemented proper dependency injection patterns with new constructors that accept dependencies explicitly
- **Architecture**: Improved testability throughout the library with better DI patterns
- **Performance**: Enhanced caching strategies and memory management
- **API**: Added new constructors with explicit dependency injection while maintaining backward compatibility

### Fixed
- **Critical**: RegexCondition timeout protection to prevent hanging applications with malicious patterns
- **Security**: Type loading security issue in ConditionDeserializer
- **Functionality**: Prefix matching functionality in FindStartsWith method
- **Quality**: Configuration validation to detect errors early

### Breaking Changes (Minimal)
- **Security**: External condition types from other assemblies may no longer be loadable (security enhancement)
- **Behavioral**: RegexCondition may timeout complex patterns that previously worked
- **Behavioral**: FindStartsWith now returns proper prefix matches instead of exact matches

### Quality Improvements
- **Testing**: Achieved 90%+ code coverage across all critical components
- **Documentation**: Updated all documentation with new DI integration examples
- **Validation**: Added comprehensive configuration validation with clear error messages
