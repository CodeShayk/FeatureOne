# Changelog

## [6.0.0] - 2026-08-16

### Added
- **New Package**: `FeatureOne.OpenFeature` implementing official OpenFeature Specification (v1.x) `FeatureProvider`.
- **OpenFeature Provider**: `FeatureOneProvider` with full support for Boolean, String, Integer, Double, and Structure flag resolution.
- **Context Mapping**: `EvaluationContextExtensions` mapping OpenFeature `TargetingKey` and attributes to FeatureOne user claims.
- **Dependency Injection**: `AddFeatureOneOpenFeature` extension methods for ASP.NET Core `IServiceCollection` and `OpenFeature.Api.Instance`.
- **Core Interoperability**: Exposed `FeatureStore` property on `Features` class for open provider integration.

### Quality & Testing
- **Test Suite**: Added `FeatureOne.OpenFeature.Tests` with 100% pass rate across OpenFeature resolution types, context mapping, error codes, and global client integration.

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
