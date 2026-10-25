# Quickstart: MVP RSS/Atom Subscription List

This guide validates the add-and-view flow after the API and UI projects have
been created according to [plan.md](./plan.md).

## Prerequisites

- .NET 10 SDK
- A modern web browser
- The repository checked out locally

## Build and test

From the repository root:

```powershell
dotnet test backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj
dotnet build frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
```

Both commands must complete successfully.

## Run locally

Start the API in one terminal:

```powershell
dotnet run --project backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj
```

Start the UI in another terminal:

```powershell
dotnet run --project frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
```

Use the configured local URLs. The frontend API base URL must point at the
running backend, and the backend must allow the frontend origin through CORS.
The stakeholder document gives default example ports of `5151` for the API and
`5213` for the UI; confirm these match the generated launch settings and
frontend configuration.
The local UI configuration uses `http://localhost:5151/api/` and the HTTP
launch profiles so browser validation does not depend on trusting a local
development certificate.

## Acceptance checks

1. Open the UI and confirm the empty subscription list and URL-entry control
   are visible.
2. Add `https://example.com/feed.xml`; confirm it appears immediately.
3. Add the same URL again; confirm the application identifies it as a
   duplicate and leaves exactly one entry in the list.
4. Add an HTTP URL; confirm it appears. Then try a URL with a non-HTTP(S)
   scheme; confirm it is rejected and the list is unchanged.
5. Try empty input, malformed input, and a value longer than 2048 characters;
   confirm each is rejected and the list is unchanged.
6. Restart the API; confirm the list is empty.
7. Inspect network activity; confirm the application makes no request to
   `example.com` and does not fetch feed content.

## Template routing check

If the UI project is generated from a template, remove its unused demo pages
and navigation links as described in
[TechStack.md](../../StakeholderDocuments/TechStack.md) before adding the
subscription page. Confirm that exactly one page handles the root route before
running the UI.
