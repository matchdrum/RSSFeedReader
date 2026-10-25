---

description: "Implementation tasks for the MVP RSS/Atom subscription list"

---

# Tasks: MVP RSS/Atom Subscription List

**Input**: Design documents from `specs/001-rss-subscriptions/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md,
contracts/subscriptions-api.md, quickstart.md

**Tests**: Tests are included because the project constitution requires
behavior and boundary test coverage. Write tests before the implementation
they verify.

**Organization**: Tasks are grouped by user story for independently testable
delivery. This feature has one P1 user story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Task can run in parallel with other tasks in the phase because it
  touches different files and has no dependency on unfinished work.
- **[Story]**: Identifies the user story served by the task.
- Every task includes the file path or project path it creates or changes.

## Phase 1: Setup

**Purpose**: Create the .NET solution and the planned API, UI, and test projects.

- [ ] T001 Create the .NET 10 solution in `RSSFeedReader.sln` using the `.sln` solution format.
- [ ] T002 [P] Create the ASP.NET Core Web API project targeting `net10.0` in `backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj`.
- [ ] T003 [P] Create the Blazor WebAssembly project targeting `net10.0` in `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj`.
- [ ] T004 [P] Create the xUnit test project targeting `net10.0` in `backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj`.
- [ ] T005 Add the API, UI, and test projects to `RSSFeedReader.sln`, add the API project reference, and add `Microsoft.AspNetCore.Mvc.Testing` to `backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj`.

## Phase 2: Foundational

**Purpose**: Configure shared API/UI communication and remove template routing conflicts before story work.

**Checkpoint**: Complete this phase before implementing the subscription flow.

- [ ] T006 Configure the API in `backend/RSSFeedReader.Api/Program.cs` to allow CORS only from the configured local frontend origin.
- [ ] T007 Configure the UI API base URL in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` to match the API launch URL.
- [ ] T008 Remove unused template pages `frontend/RSSFeedReader.UI/Pages/Home.razor`, `frontend/RSSFeedReader.UI/Pages/Counter.razor`, and `frontend/RSSFeedReader.UI/Pages/Weather.razor`, and remove their links from `frontend/RSSFeedReader.UI/Layout/NavMenu.razor`.
- [ ] T009 Build `RSSFeedReader.sln` and verify that exactly one page in `frontend/RSSFeedReader.UI/Pages/` declares the root route `@page "/"`.

## Phase 3: User Story 1 - Add and View Subscriptions (Priority: P1)

**Goal**: Let the local user add a unique HTTP or HTTPS URL and immediately
view accepted subscriptions for the current API process.

**Independent Test**: Start with an empty list, add a valid URL, and verify it
appears immediately. Submit the same trimmed URL again and verify it is
identified as a duplicate without changing the list. Verify invalid URLs are
rejected, and a new API process starts with an empty list.

### Tests for User Story 1

- [ ] T010 [P] [US1] Add `SubscriptionStore` unit tests in `backend/RSSFeedReader.Api.Tests/SubscriptionStoreTests.cs` for initial empty state, insertion order, successful unique adds, exact trimmed-string duplicate rejection, and unchanged state after duplicates.
- [ ] T011 [P] [US1] Add HTTP integration tests in `backend/RSSFeedReader.Api.Tests/SubscriptionsApiTests.cs` for GET and POST responses, duplicate status `409 Conflict`, and invalid input status `400 Bad Request` according to `specs/001-rss-subscriptions/contracts/subscriptions-api.md`.
- [ ] T012 [P] [US1] Add URL validation boundary tests in `backend/RSSFeedReader.Api.Tests/SubscriptionUrlValidationTests.cs` for null, empty, whitespace-only, malformed, relative, hostless, non-HTTP(S), and over-2048-character values, plus valid HTTP and HTTPS URLs.

### Implementation for User Story 1

- [ ] T013 [US1] Define subscription request and response DTOs in `backend/RSSFeedReader.Api/Models/SubscriptionDto.cs`, including the required URL string and its 1–2048 character bound.
- [ ] T014 [US1] Implement absolute URL validation in `backend/RSSFeedReader.Api/Models/SubscriptionUrlValidator.cs`, trimming surrounding whitespace and accepting only URLs with a non-empty host and `http` or `https` scheme.
- [ ] T015 [US1] Implement the process-lifetime, insertion-ordered subscription collection in `backend/RSSFeedReader.Api/Services/SubscriptionStore.cs`; atomically reject a duplicate when the trimmed URL string exactly matches an existing value and leave state unchanged.
- [ ] T016 [US1] Implement `GET /api/subscriptions` and `POST /api/subscriptions` in `backend/RSSFeedReader.Api/Controllers/SubscriptionsController.cs`, returning accepted entries, `409 Conflict` for duplicates, and `400 Bad Request` for invalid URLs without making network requests.
- [ ] T017 [US1] Register the subscription store, controller routes, and automatic request validation in `backend/RSSFeedReader.Api/Program.cs`.
- [ ] T018 [US1] Implement the configured API client and methods to list and add subscriptions in `frontend/RSSFeedReader.UI/Services/SubscriptionsApiClient.cs`.
- [ ] T019 [US1] Implement the root subscription page in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` with URL entry, add action, immediate list refresh after success, duplicate feedback, and invalid-input feedback.
- [ ] T020 [US1] Update the navigation label for the root subscription page in `frontend/RSSFeedReader.UI/Layout/NavMenu.razor`.

**Checkpoint**: User Story 1 is independently usable; unique valid URLs appear
immediately, duplicates and invalid URLs leave the list unchanged, and feed
content is never fetched.

## Phase 4: Polish & Cross-Cutting Validation

**Purpose**: Verify the full local integration and the documented acceptance flow.

- [ ] T021 Run the xUnit suite with `dotnet test backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj` and fix failures in the affected files under `backend/RSSFeedReader.Api.Tests/`.
- [ ] T022 Build the API and UI projects with `dotnet build backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj` and `dotnet build frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj`.
- [ ] T023 Run the API/UI acceptance scenarios in `specs/001-rss-subscriptions/quickstart.md`, verify the configured API URL and CORS origin, confirm duplicates are rejected and session data is lost after API restart, and confirm no feed URL is fetched.

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; T002–T004 can run in parallel after
  the solution path is established. T005 depends on the solution and projects.
- **Foundational (Phase 2)**: Depends on the projects in Phase 1; blocks all
  user-story implementation. T006–T008 touch distinct files and can be done
  in parallel; T009 follows their configuration and cleanup.
- **User Story 1 (Phase 3)**: Depends on the foundational checkpoint. Test
  tasks T010–T012 can be authored in parallel and should fail before
  implementation. Implement DTOs and validation before the store and
  controller; implement the UI client before the page.
- **Polish (Phase 4)**: Depends on the completed story; T021–T023 verify the
  integrated implementation.

### User Story Dependencies

- **US1 (P1)**: Depends only on Setup and Foundational completion. No other
  user stories are defined.

### Parallel Opportunities

- **Setup**: T002, T003, and T004 create independent projects.
- **Foundational**: T006, T007, and T008 change separate files after project
  generation.
- **US1 tests**: T010, T011, and T012 are separate test files and can be
  prepared concurrently before production implementation.
- **US1 implementation**: After DTO/validation contracts are agreed, T015
  (store) and UI client work can proceed in separate files; the page follows
  the client, while controller integration follows the store and validator.

## Parallel Example: User Story 1

```text
Task: T010 SubscriptionStore unit tests in backend/RSSFeedReader.Api.Tests/SubscriptionStoreTests.cs
Task: T011 Subscription API HTTP tests in backend/RSSFeedReader.Api.Tests/SubscriptionsApiTests.cs
Task: T012 URL validation tests in backend/RSSFeedReader.Api.Tests/SubscriptionUrlValidationTests.cs
```

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 setup.
2. Complete Phase 2 foundational configuration and verify routing.
3. Write and run the User Story 1 tests; confirm they fail before implementation.
4. Implement the API validation, in-memory store, and HTTP contract.
5. Implement the UI client and subscription page.
6. Complete the acceptance checks in `specs/001-rss-subscriptions/quickstart.md`.

### Incremental Delivery

The specification defines a single P1 story. Deliver that story as the MVP;
feed fetching, item display, persistence, and background refresh remain out of
scope.

## Notes

- `[P]` marks tasks that work on separate files with no unmet dependencies.
- `[US1]` maps tasks to the add-and-view story in `specs/001-rss-subscriptions/spec.md`.
- The repository currently has no solution or application projects; project
  creation is therefore explicit setup work.
- Preserve the distinction between URL syntax validation and feed/reachability
  checking; no task may issue a request to a submitted feed URL.
