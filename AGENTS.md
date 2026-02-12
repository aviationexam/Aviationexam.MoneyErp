# AGENTS.md — Aviationexam.MoneyErp

## Project Overview

.NET client libraries for Money ERP (Czech/Slovak accounting software). Provides GraphQL and REST API clients published as NuGet packages. Multi-target: **net8.0, net9.0, net10.0**. C# **LangVersion 14**.

### Project Structure

```
src/
  Aviationexam.MoneyErp.Common/          # Core: auth, options, filters, HTTP handlers
  Aviationexam.MoneyErp.Common.Tests/    # Unit tests for Common
  Aviationexam.MoneyErp.Graphql/         # GraphQL client (ZeroQL-based, generated code)
  Aviationexam.MoneyErp.Graphql.Tests/   # Tests for GraphQL client
  Aviationexam.MoneyErp.RestApi/         # REST client (Kiota-generated, v1+v2)
  Aviationexam.MoneyErp.RestApi.Tests/   # Tests for REST client
  Aviationexam.MoneyErp.PreprocessOpenApi/ # OpenAPI spec preprocessor tool
```

**Generated code** (do NOT edit manually):
- `src/Aviationexam.MoneyErp.Graphql/GraphqlClient.g.cs` — ZeroQL-generated
- `src/Aviationexam.MoneyErp.RestApi/ClientV1/` — Kiota-generated REST v1
- `src/Aviationexam.MoneyErp.RestApi/ClientV2/` — Kiota-generated REST v2

## Build / Test / Lint Commands

### Build

```bash
dotnet restore --nologo
dotnet build --no-restore --nologo
# Release (CI): dotnet build --no-restore --nologo --configuration Release -p:ContinuousIntegrationBuild=true
```

### Test

Uses **Microsoft.Testing.Platform** as the test runner (configured in `global.json`).

```bash
# Run all tests
dotnet test --nologo

# Run a single test project
dotnet test --nologo src/Aviationexam.MoneyErp.Common.Tests/

# Run a single test by filter
dotnet test --nologo --filter "FullyQualifiedName~FilterForTests.StringEqualWorks"
dotnet test --nologo --filter "FullyQualifiedName~FilterForTests"

# Run tests with release config (as CI does)
dotnet test --nologo --no-build --configuration Release
```

**Note**: REST/GraphQL tests require env vars: `MONEYERP_CLIENT_ID`, `MONEYERP_CLIENT_SECRET`, `MONEYERP_ENDPOINT`. Common tests do not need these.

### Format / Lint

```bash
# Fix formatting (preferred)
just format

# Check formatting only (CI does this)
dotnet format --no-restore --verify-no-changes -v diag
```

### Package Restore

```bash
just commit-lock                       # Restore + commit lock files (preferred)
just restore-dotnet                    # Force re-evaluate packages only
```

### Tool Restore

```bash
dotnet tool restore    # Restores: kiota (OpenAPI client gen), zeroql (GraphQL client gen)
```

## Test Framework

- **xUnit v3** (`xunit.v3.mtp-v2`) with Microsoft.Testing.Platform runner
- **NSubstitute** for mocking (with `NSubstitute.Analyzers.CSharp`)
- **xunit `Assert`** for assertions (NOT FluentAssertions)
- `MartinCostello.Logging.XUnit.v3` for test output logging

### Test Patterns

- Test classes use **primary constructor** injection for `ITestOutputHelper`
- Tests use `[Fact]` for unit tests, `[Theory]` with `[ClassData]` for parameterized tests
- Integration tests build a `ServiceProvider` via factory pattern (see `ServiceProviderFactory.Create`)
- Use `TestContext.Current.CancellationToken` for async test cancellation
- Partial classes split test groups per operation (e.g., `FilterForTests.Equal.cs`, `FilterForTests.Or.cs`)

## Code Style

### Formatting (from .editorconfig)

- **Indentation**: 4 spaces for `.cs`, 2 spaces for `.csproj`/`.props`/`.targets`/`.cshtml`/`.resx`
- **Line endings**: CRLF
- **Max line length**: 200 characters
- **Final newline**: required
- **Trailing whitespace**: trimmed
- **Space after cast**: yes (`(int) value`, not `(int)value`)

### Namespace & Usings

- **File-scoped namespaces**: always (`namespace Foo.Bar;` not `namespace Foo.Bar { }`)
- **Using directives**: at file top, outside namespace
- **Sort system directives first**: disabled (`dotnet_sort_system_directives_first = false`)
- **No import grouping**: `dotnet_separate_import_directive_groups = false`

### Naming Conventions

- `PascalCase` for types, methods, properties, constants, public fields
- `camelCase` for local variables and parameters
- `_camelCase` for private fields (standard .NET convention)
- `E` prefix for enums (e.g., `EFilterOperator`)
- `I` prefix for interfaces (e.g., `IEndpointCertificateProvider`)

### Type Patterns

- **Nullable reference types**: enabled globally (`<Nullable>enable</Nullable>`)
- **Primary constructors**: used for DI in both classes and test classes
- **Partial classes**: used to split large classes across files by concern
- **Pattern matching**: preferred over `is`/`as` checks (`.editorconfig` suggestion)
- **Collection expressions**: `[item1, item2]` syntax for inline collections
- **`required` keyword**: used on mandatory properties without defaults

### Conditional Compilation

Multi-target code uses `#if` directives for framework-specific APIs:

```csharp
#if NET9_0_OR_GREATER
    // .NET 9+ specific code (e.g., Lock type, AddAsKeyed)
#else
    // Fallback for .NET 8
#endif
```

### Dependency Injection

- Builder pattern: `AddMoneyErp(...)` returns `MoneyErpBuilder`, methods chain from it
- Keyed services with string constants (e.g., `MoneyErpServiceKey = "MoneyErp"`)
- Options pattern: `IOptions<T>`, `IPostConfigureOptions<T>`, `IValidateOptions<T>`
- `TryAdd*` methods for default registrations (prevent overrides)

### JSON Serialization

- **System.Text.Json** (NOT Newtonsoft) with source generators for AOT compatibility
- `JsonSerializerContext` subclasses for serialization (e.g., `TokenResponseJsonContext`)
- `[JsonPropertyName]` for mapping to snake_case API properties

### Error Handling

- Throw `InvalidOperationException` for unexpected null/missing data
- `ArgumentOutOfRangeException` for invalid enum/switch values
- `response.EnsureSuccessStatusCode()` for HTTP calls (no manual status checks)
- No empty catch blocks

### AOT Compatibility

- `<IsAotCompatible>true</IsAotCompatible>` on library projects (false on test projects)
- `<InvariantGlobalization>true</InvariantGlobalization>` on library projects
- Source-generated JSON serializers for AOT support

## Analyzers

- **Meziantou.Analyzer**: included on all projects via `WarningConfiguration.targets`
- **NSubstitute.Analyzers.CSharp**: on test projects
- **Roslyn CA rules**: several promoted to `error` severity (see `.editorconfig`)
- **RMG rules** (RMG012, RMG020, RMG037, RMG038): unmapped member diagnostics are errors (pre-configured in `.editorconfig`)
- **Release builds**: `TreatWarningsAsErrors=true`
- **NuGet auditing**: enabled for all levels, warnings-as-errors in CI/Release

## Central Package Management

All NuGet versions in `src/Directory.Packages.props`. Use `<PackageReference Include="..." />` without version in `.csproj` files. Transitive pinning enabled.

Lock files (`packages.lock.json`) are committed. After changing packages: `just commit-lock`.

## Versioning

GitVersion in ContinuousDelivery mode. Main branch: Patch increment, `nightly` label.
