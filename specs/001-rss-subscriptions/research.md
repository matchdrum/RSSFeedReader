# Research: MVP RSS/Atom Subscription List

## Decisions

### Application framework and target

- **Decision**: Use the documented ASP.NET Core Web API and Blazor WebAssembly
  split, targeting .NET 10 (`net10.0`).
- **Rationale**: The stakeholder technology document names this architecture;
  the environment has .NET SDK 10.0.113 installed. No existing project or
  `global.json` constrains the target.
- **Alternatives considered**: Choosing a different application architecture
  would contradict the stakeholder decision. Targeting another .NET version
  has no support in the current repository files.

### Subscription storage and lifecycle

- **Decision**: Keep URL entries in an in-memory collection owned by the API
  for the lifetime of the running application process. Preserve insertion
  order and reject a submission if its trimmed URL string exactly matches an
  existing entry.
- **Rationale**: This satisfies the session-only MVP and the requirement not
  to accept duplicate URLs without introducing a database or identity system.
  Exact comparison after trimming is deterministic and avoids imposing
  broader URL canonicalization rules.
- **Alternatives considered**: Persistent storage is explicitly deferred.
  Case-insensitive or canonicalized URL comparison would treat distinct
  strings as duplicates and is outside the clarified requirement.

### URL input validation

- **Decision**: Reject missing, null, empty, or whitespace-only values; trim
  surrounding whitespace; accept only absolute URLs with `http` or `https`
  schemes with a non-empty host; reject values longer than 2048 characters.
  Do not make a network request to verify reachability or feed content.
- **Rationale**: The constitution requires API input validation, URL-format
  checks, null checks, and length limits. The feature clarification settles
  the permitted schemes and explicitly excludes reachability/feed checks.
  .NET provides model-validation mechanisms for required values and maximum
  lengths, but does not prescribe a universal URL-length limit. The plan
  selects 2048 characters as an explicit bounded-input policy for this MVP,
  not as a framework or protocol default.
- **Alternatives considered**: Accepting arbitrary input conflicts with
  constitution and feature requirements. Restricting to HTTPS only conflicts
  with the accepted clarification permitting HTTP and HTTPS.
- **Validation approach**: Parse as an absolute URI, then check the scheme
  against HTTP/HTTPS and require a non-empty host. Do not rely on a generic
  URL annotation alone to enforce those application-specific conditions.

### API contract

- **Decision**: Expose a collection read operation and an add operation under
  `/api/subscriptions`; use JSON, return the entries in insertion order, and
  return a client error for invalid input.
- **Rationale**: The stakeholder document requires the backend to add and
  return subscriptions and the frontend to communicate with it. A minimal
  resource-oriented HTTP contract supports that flow without adding feed
  operations.
- **Alternatives considered**: An in-process-only frontend list would not
  follow the documented backend responsibilities. Feed endpoints are out of
  MVP scope.

### Verification

- **Decision**: Use xUnit for backend unit and integration tests and exercise
  the full add-and-view path through the locally running browser UI.
- **Rationale**: The stakeholder technology document names xUnit and calls for
  checking the frontend/backend connection, configured API URL, and CORS.
- **Alternatives considered**: Manual-only API checks would not adequately
  cover validation and duplicate rejection behavior.
- **Integration-test approach**: Use `Microsoft.AspNetCore.Mvc.Testing` and
  `WebApplicationFactory<Program>` to exercise HTTP status codes and response
  bodies, in addition to focused unit tests for validation and collection
  behavior.

### Browser/API integration configuration

- **Decision**: Load the API base URL from the Blazor WebAssembly client
  configuration, allow only the configured frontend origin in API CORS, and
  keep secrets out of client-visible configuration.
- **Rationale**: The stakeholder document requires matching API URL, local
  ports, and CORS. Client-side configuration is visible to users, so it is
  appropriate for service URLs but not secrets.
- **Alternatives considered**: Assuming frontend and API share an origin is
  inconsistent with the documented separate local ports. Allowing arbitrary
  origins is broader than needed for the local MVP.

## Repository Starting Point

At planning time the repository contains stakeholder/specification documents
but no `.sln`, `.csproj`, `global.json`, or application source projects.
Project creation and initial frontend-template cleanup are therefore
foundational implementation work, not changes to an existing application.

## References

- [ASP.NET Core model validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0)
- [StringLengthAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute?view=net-10.0)
- [Uri.TryCreate](https://learn.microsoft.com/en-us/dotnet/api/system.uri.trycreate?view=net-10.0) and [Uri.Scheme](https://learn.microsoft.com/en-us/dotnet/api/system.uri.scheme?view=net-10.0)
- [Blazor configuration](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/configuration?view=aspnetcore-10.0) and [calling web APIs from Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-10.0)
- [ASP.NET Core CORS](https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-10.0)
- [ASP.NET Core integration tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0)
