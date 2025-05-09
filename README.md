# Project Architecture

## 📁 Solution Structure

```
LapVision/
│
├── API/                         - Web API (Controllers, Application Services)
│   ├── Controllers/             - Entry point for HTTP requests
│   ├── Services/                - Application logic (AuthService, LapTimeService, etc.)
│   ├── Helpers/
│   │   └── Mappers/             - Static mapping logic (DTO ↔ Entity, Enums)
│   ├── Interfaces/              - IService interfaces for testability
│   └── Program.cs               - API configuration and DI setup
│
├── App/                         - MAUI Blazor Hybrid Client (Mobile App)
│   ├── Pages/                   - Razor UI (HeatPage, SessionList, etc.)
│   ├── Resources/               - Fonts, images, splash, etc.
│   └── App.xaml.cs              - App entry point and navigation
│
├── CoachWeb/                    - ASP.NET MVC Web App for Coach Dashboard
│   ├── Controllers/             - MVC controllers (e.g., AccountController)
│   ├── Services/                - Domain-facing logic (e.g., AccountService)
│   ├── Repositories/            - Manual SQL access (e.g., AccountRepository)
│   ├── Interfaces/              - IService / IRepository interfaces
│   ├── Views/                   - Razor Views (e.g., Login.cshtml)
│   └── Program.cs               - MVC app configuration and DI setup
│
├── Contracts/                   - Shared data contracts (DTOs and enums)
│   ├── DTO/                     - Request/Response objects
│   └── Enums/                   - Serializable enums (e.g., UserRole)
│
├── Model/                       - Domain Layer (Entities and Domain Services)
│   ├── Entities/                - Core models (e.g., LapTime, Session, Account)
│   ├── Services/                - Pure business logic (no EF, no config)
│   └── Enums/                   - Domain-level enums
│
├── Infrastructure/              - Infrastructure Layer (EF Core persistence for API only)
│   └── Persistence/
│       ├── DataContext.cs       - EF Core DbContext
│       └── Migrations/          - EF Core migration files
│
├── Tests/                       - Unit and integration tests
│   ├── API.Tests/               - API layer tests with mocked services
│   ├── CoachWeb.Tests/          - MVC tests using mocked repositories
│   └── TestUtilities/           - Mocks, fakes, test builders, etc.
│
├── Dockerfile                   - Docker container setup for API
├── docker-compose.yml           - Multi-service setup (e.g., API + MySQL)
└── README.md                    - Project documentation (this file)
```

---

## Layer Responsibilities

### App (MAUI Blazor)
- UI rendering and navigation
- Talks to API via HTTP and DTOs (from `Contracts`)
- No access to domain logic or EF

### API
- Handles HTTP requests
- Uses application services to orchestrate logic
- Maps between DTOs and domain entities
- Depends on `Model`, `Contracts`, and `Infrastructure`

### CoachWeb (ASP.NET MVC)
- Web dashboard for coaches
- Uses repositories and services (without EF Core)
- Manually interacts with database (e.g., using MySqlConnector)
- Depends on `Model` for domain entities
- Does not call API and does not use DTOs

### Model
- Contains core domain logic (entities and rules)
- Does not depend on EF Core or config
- Referenced by API, CoachWeb, and Infrastructure

### Infrastructure
- Contains EF Core `DataContext` and migrations (used by API only)
- Depends on `Model` for entity definitions

### Contracts
- Contains DTOs and shared enums
- Used by App and API only (not CoachWeb)
- No dependencies on any other project

---

## Communication Flow

```
[MAUI App]
    ↓
HttpClient (DTOs from Contracts)
    ↓
[API Controllers]
    ↓
[API Services]
    ↓
[EF Core DataContext (Infrastructure)]
    ↓
[MySQL]

[CoachWeb MVC]
    ↔ [Repositories] ↔ [MySQL]
```

App communicates with API using DTOs. CoachWeb connects to DB directly via custom repositories.

---

## Project Reference Rules

| Project       | Can Reference                       |
|---------------|-------------------------------------|
| App           | Contracts only                      |
| API           | Contracts, Model, Infrastructure    |
| CoachWeb      | Model                               |
| Infrastructure| Model only                          |
| Model         | Nothing                             |
| Contracts     | Nothing                             |

---

## Benefits

- Clear separation of concerns
- MVC site can evolve independently from API
- CoachWeb is testable and follows SOLID (with interfaces, repositories)
- Entities are reused without duplication
- MAUI App uses API as intended, keeping frontends loosely coupled

---

## Common Pitfalls to Avoid

- CoachWeb referencing Contracts (DTOs are API-only)
- CoachWeb using EF Core (uses raw SQL or MySqlConnector instead)
- App referencing Model directly
- Logic in Controllers (should be in services)
- API exposing entities instead of mapping to DTOs

---

## Design Trade-offs

### Dual Frontends: CoachWeb vs App

| Aspect              | CoachWeb (ASP.NET MVC)                                     | App (MAUI Blazor Hybrid)                             |
|---------------------|------------------------------------------------------------|------------------------------------------------------|
| **Access Pattern**   | Direct DB access using custom repositories (raw SQL)       | Communicates via API using DTOs                      |
| **Use Case**         | Internal web dashboard for coaches                         | Cross-platform client app for users                  |
| **Performance**      | Low-latency direct DB access                               | Slight overhead due to HTTP calls                    |
| **Testability**      | Easily testable with mockable services & repositories      | Uses mocked HTTP handlers or DI for services         |
| **Coupling**         | Tightly coupled to DB schema                               | Loosely coupled via API contract (DTOs)              |
| **Flexibility**      | High control over queries and performance tuning           | Abstracted from DB structure                         |
| **Security Layer**   | Internal-only; typically on intranet or behind auth        | Exposed API surface with role-based access control   |
| **Maintenance**      | Manual sync with DB schema (risk of drift from API logic) | Central logic in API; clients stay thinner           |


### What to Watch For

- **Duplicate logic**: Ensure core validation/business rules are placed in the `Model` layer and reused, so both frontends behave consistently.
- **Schema changes**: Keep CoachWeb in sync with DB schema changes that might otherwise be abstracted by the API.
- **Security alignment**: Make sure CoachWeb has appropriate access controls, even if internal, and does not bypass logic enforced in the API.

Thanks for reading!
