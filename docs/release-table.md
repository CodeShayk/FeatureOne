| Version | Release Date | Type | Key Changes | Backward Compatibility |
|--------|-------------|------|-------------|---------------------|
| v5.0.1 | Previous | Initial | Core feature toggle functionality | N/A (Initial release) |
| v5.1.0 | Oct 13, 2025 | Minor | Security fixes (ReDoS protection, secure type loading), architectural improvements (prefix matching, dependency injection), new features (DateRangeCondition, configuration validation) | High - maintains all existing functionality with minor security-related behavioral changes |
| v5.2.0 | Mar 18, 2026 | Minor | RelationalCondition (5 operators), net10.0 target framework, Microsoft 10.0.5 package upgrades | Full - fully backward compatible |
| v6.0.0 | Aug 16, 2026 | Major | OpenFeature Specification (v1.x) Provider compliance (`FeatureOneProvider`, `FeatureOne.OpenFeature` package, context mapping, DI extensions) | Full - 100% backward compatible, additive changes only |
