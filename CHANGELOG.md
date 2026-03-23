# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

---

## [1.1.0] - 2026-03-23

### Added
- `Critical()`, `Debug()`, `Trace()` log methods to cover all 6 standard `ILogger` levels
- `IsEnabled(LogLevel)` guard clause in `Log()` — avoids serialization and enricher invocation when the level is disabled
- Explicit null checks on all constructor parameters
- `sealed record LogEntry` to replace anonymous type for log entry serialization
- XML documentation on `BeginScope` to document the `IDisposable` contract
- Unit test suite (`StructLog.Tests`) using xUnit and Moq

### Changed
- `JsonSerializerOptions` moved to a `static readonly` field — previously a new instance was allocated on every log call
- `_logger.Log()` state parameter now passes the structured `LogEntry` instead of `null`, enabling structured log providers (Serilog, OpenTelemetry, Application Insights) to index fields correctly
- `BeginScope` return type corrected to `IDisposable?` (nullable), matching the official `ILogger` signature

### Removed
- Unused `record LogJsonState(string Json)` declaration

### Fixed
- Typo "Exemple" → "Example" in README
- `_logger.info()` → `_logger.Info()` in README code example (lowercase `i` does not compile)
- Missing language hints on README code blocks (added `csharp`, `json`, `bash`)

---

## [1.0.0] - 2026-02-27

### Added
- Initial release
- `IStructLog<T>` interface with `Info()`, `Warning()`, `Error()` methods
- `ILogEnricher` interface for custom context enrichers
- `IEventCode` interface for structured event identification
- `EventCode` built-in implementation
- `StructLogOptions` with `EnableEnrichers` flag
- `AddStructLog()` and `AddStructLog(IConfiguration)` extension methods for DI registration
- JSON output via `System.Text.Json`
- `BeginScope()` support for contextual logging
- GitHub Actions workflow for automatic NuGet publish on tag push
- MIT license

---

[1.1.0]: https://github.com/simoneM93/StructLog/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/simoneM93/StructLog/releases/tag/v1.0.0
