# Implementation Plan: MVP RSS/Atom Subscription List

**Branch**: `001-rss-subscriptions` | **Date**: 2026-10-25 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/001-rss-subscriptions/spec.md`

## Summary

Deliver the single-user MVP flow for adding a subscription URL and immediately
viewing the in-memory list. Use the documented ASP.NET Core Web API backend and
Blazor WebAssembly frontend. The API validates non-empty, syntactically valid
HTTP/HTTPS URLs, rejects duplicate submissions that match after trimming
surrounding whitespace, and performs no network access to feed URLs. The
selected target is .NET 10, matching the SDK available in the repository
environment.

## Technical Context

**Language/Version**: C# / .NET 10 (`net10.0`)

**Primary Dependencies**: ASP.NET Core Web API; Blazor WebAssembly; xUnit and
`Microsoft.AspNetCore.Mvc.Testing` for backend tests

**Storage**: In-memory backend subscription collection; no persistence

**Testing**: xUnit unit and API integration tests; browser-based end-to-end
acceptance checks

**Target Platform**: Locally run application on Windows, macOS, and Linux;
browser UI communicating with local API

**Project Type**: Web application with separate backend API and frontend UI

**Performance Goals**: Adding a subscription and displaying the updated list
within one second under local single-user use

**Constraints**: Single-user proof of concept; only HTTP and HTTPS URL schemes;
no feed fetching, parsing, persistence, or automatic refresh; application
subscriptions are lost when the backend process stops. Limit submitted URLs to
2048 characters as an explicit MVP bound, reject null/empty/whitespace input,
and trim surrounding whitespace before validation. The API allows the exact
configured frontend origin through CORS.

**Scale/Scope**: One local user and a small in-memory list for the current
backend process; no production availability or multi-user guarantees

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **User value and scope**: PASS — the plan covers only adding and viewing
  subscriptions; fetching, persistence, and other deferred features are
  excluded.
- **Security and privacy**: PASS — validate input before accepting it; do not
  fetch user-supplied URLs or expose sensitive diagnostics. URL validation is
  in the feature requirements, resolving the constitution's MVP constraint
  against adding URL validation without an approved requirement. Limit CORS to
  the configured frontend origin and keep secrets out of client configuration.
- **Maintainability**: PASS — keep the API, in-memory application behavior, and
  UI responsibilities separate; add no speculative dependencies.
- **Code quality and verification**: PASS — test URL validation and list
  behavior at unit and HTTP integration levels, verify the API/UI integration,
  and run the relevant builds and tests.
- **Incremental simplicity**: PASS — use transient in-memory data and only the
  single MVP user flow.

## Project Structure

### Documentation (this feature)

```text
specs/001-rss-subscriptions/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── subscriptions-api.md
└── tasks.md
```

### Source Code (repository root)

```text
backend/
├── RSSFeedReader.Api/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
└── RSSFeedReader.Api.Tests/

frontend/
└── RSSFeedReader.UI/
    ├── Pages/
    ├── Layout/
    ├── Services/
    └── wwwroot/
```

The source projects do not exist yet. The paths above follow
[TechStack.md](../../StakeholderDocuments/TechStack.md) and name the planned
projects; implementation must verify the generated project layout before
creating feature files.

**Structure Decision**: Use the documented split between
`backend/RSSFeedReader.Api` and `frontend/RSSFeedReader.UI`, with an xUnit
backend test project. Keep subscription state and input validation in the
backend; the frontend submits URLs and displays the returned list.

## Complexity Tracking

No constitution violations or additional architectural complexity require
justification.
