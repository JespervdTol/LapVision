# Project Architecture

## 📁 Solution Structure

```
LapVision/
│
├── API/                         - ASP.NET Core Web API
│   ├── Controllers/             - HTTP endpoints (Auth, GPS, Session, etc.)
│   ├── Services/                - Application-level services (Auth, LapTime, Session)
│   ├── Helpers/                 - Mappers, default image, seeders
│   ├── API.http                 - API test requests
│   └── Program.cs               - API startup and configuration
│
├── App/                         - MAUI Blazor Hybrid Client
│   ├── wwwroot/                 - Static assets (CSS, JS, favicon)
│   ├── Components/              - Razor components (Layout, Pages, Routing)
│   ├── Platforms/               - Target platforms (Android, iOS, Windows, etc.)
│   ├── Resources/               - App icon, fonts, images, raw, splash
│   ├── App.xaml & MainPage.xaml - UI entry points
│   └── MauiProgram.cs           - DI and startup config
│
├── CoachWeb/                    - ASP.NET MVC Web App for Coach Dashboard
│   ├── wwwroot/                 - Static assets (CSS, JS, lib)
│   ├── Controllers/             - MVC Controllers (Account, Home, Report)
│   ├── Services/                - CoachWeb logic (AccountService, ReportService)
│   │   └── Interfaces/          - IService abstractions for testability
│   ├── Views/                   - Razor views (Account, Home, Report, Shared)
│   └── Program.cs               - MVC startup and DI configuration
│
├── Contracts/                   - Shared DTOs, Enums, ViewModels
│   ├── App/
│   │   ├── DTO/                 - Auth, Circuit, GPS, Heat, LapTime, Session DTOs
│   │   └── Enums/              - Shared enums (UserRole)
│   └── CoachWeb/
│       └── ViewModels/          - Account, Report, and Error ViewModels for MVC
│
├── Infrastructure/              - Data persistence and access
│   ├── App/
│   │   └── Persistence/         - EF Core DbContext and Migrations for API
│   └── CoachWeb/
│       ├── Interfaces/          - Repositories contracts (e.g., IAccountRepository)
│       └── Repositories/        - Raw SQL Repositories (e.g., ReportRepository)
│
├── Model/                       - Domain Layer
│   ├── Entities/                - Core domain models (LapTime, Session, Account, etc.)
│   │   └── CoachWeb/            - MVC-specific domain (Account.cs, Person.cs)
│   └── Enums/                   - Domain enums (e.g., UserRole)
│
├── Test/                        - Unit testing
│   └── AccountServiceTests.cs   - Example test class
│
├── Dockerfile                   - Docker setup for API
├── docker-compose.yml           - Multi-container orchestration
└── README.md                    - This file
```

---

## Layer Responsibilities

### App (MAUI Blazor)
- Cross-platform UI with Razor + .NET MAUI
- Uses `HttpClient` to call API
- Strictly uses DTOs from `Contracts.App.DTO`
- Does not access domain logic or DB directly

### API
- Exposes endpoints for mobile client
- Contains core application services
- Maps between domain entities and DTOs (via AutoMapper)
- Uses EF Core via `Infrastructure.App.Persistence`

### CoachWeb (MVC)
- Web dashboard for internal coach use
- Has own Controllers, Services, Interfaces, Views
- Uses ViewModels from `Contracts.CoachWeb`
- Directly queries MySQL via raw SQL repos from `Infrastructure.CoachWeb`

### Contracts
- Cross-layer DTOs, Enums, and ViewModels
- `App.DTO` for API ↔ App
- `CoachWeb.ViewModels` for UI rendering
- No external dependencies

### Infrastructure
- `App.Persistence` contains EF Core setup for API
- `CoachWeb` provides interfaces and SQL implementations for data access
- Decoupled from services using clean interfaces

### Model
- Pure domain entities and enums
- Shared by API, Infrastructure, and CoachWeb
- No framework dependencies

### Test
- Contains unit tests (e.g., for services)
- Uses mocks (e.g., Moq) to isolate dependencies

---

## Communication Flow

```
[App (MAUI)]
    ↓
HttpClient
    ↓
[API (Controllers)]
    ↓
[API Services]
    ↓
[Infrastructure.App.Persistence (EF Core)]
    ↓
[MySQL]

[CoachWeb (MVC)]
    ↔ [Infrastructure.CoachWeb.Repositories]
    ↔ [MySQL]
```

---

## Project Reference Rules

| Project       | Can Reference                       |
|---------------|-------------------------------------|
| App           | Contracts only                      |
| API           | Contracts, Model, Infrastructure    |
| CoachWeb      | Contracts, Model, Infrastructure    |
| Infrastructure| Model only                          |
| Model         | Nothing                             |
| Contracts     | Model                               |
| Test          | Everything (Except App)             |

---

## Design Trade-offs

### CoachWeb vs App

| Aspect              | CoachWeb (ASP.NET MVC)                                     | App (MAUI Blazor Hybrid)                             |
|---------------------|------------------------------------------------------------|------------------------------------------------------|
| **Access Pattern**   | Direct DB access via raw SQL repos                         | Communicates with API using DTOs                     |
| **Use Case**         | Internal dashboard for coaches                             | Public-facing cross-platform mobile app              |
| **Performance**      | High performance via low-level DB access                   | Slightly more overhead due to HTTP + serialization   |
| **Testability**      | Interfaces for services and repos                          | Injectable services & HttpClient wrappers            |
| **Security**         | Internal, but should still be guarded                      | API protected with role-based auth                   |
| **Flexibility**      | Full control over SQL queries                              | Backend encapsulates logic and schema                |
| **Maintainability**  | Needs schema tracking discipline                          | Centralized logic makes client updates simpler       |

---

## Keep Note

- Your architecture **clearly separates responsibilities** between UI, logic, data, and domain.
- Both `App` and `CoachWeb` serve different needs with minimal overlap.
- Shared contracts avoid duplication while keeping dependencies clean.

Thanks for reading!
