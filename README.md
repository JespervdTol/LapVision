Project Structure Overview – Clean Architecture (LapVision)
📁 Solution Structure

LapVision/
│
├── API/                       - Web API (Controllers, Application Services)
│   ├── Controllers/           - Entry point for HTTP requests
│   ├── Services/              - Application logic (AuthService, LapTimeService, etc.)
│   └── Program.cs             - API configuration and DI setup
│
├── App/                       - MAUI Blazor Hybrid Client (Mobile App)
│   ├── Pages/                 - Razor UI (HeatPage, SessionList, etc.)
│   ├── Resources/             - Fonts, images, splash, etc.
│   └── App.xaml.cs            - App entry point and navigation
│
├── Contracts/                 - Shared data contracts (DTOs and enums)
│   ├── DTO/                   - Request/Response objects
│   └── Enums/                 - Serializable enums (e.g., UserRole)
│
├── Model/                     - Domain Layer (Entities and Domain Services)
│   ├── Entities/              - Core models (e.g., LapTime, Session, Account)
│   ├── Services/              - Pure business logic (no EF, no config)
│   └── Enums/                 - Domain-level enums
│
├── Infrastructure/            - Infrastructure Layer (Persistence, EF Core)
│   └── Persistence/
│       ├── DataContext.cs     - EF Core DbContext
│       └── Migrations/        - EF Core migration files
│
├── Dockerfile                 - Docker container setup for API
├── docker-compose.yml         - Multi-service setup (e.g., API + MySQL)
└── README.md                  - Project documentation (this file)

🧠 Layer Responsibilities
App (MAUI Blazor)
- UI rendering and navigation
- Talks to API via HTTP and DTOs (from Contracts)
- No access to domain logic or EF

API
- Handles HTTP requests
- Uses application services to orchestrate logic
- Maps between DTOs and domain entities
- Depends on Model, Contracts, and Infrastructure

Model
- Contains core domain logic (entities and rules)
- Does not depend on EF Core or config
- Only referenced by API and Infrastructure

Infrastructure
- Contains EF Core DataContext and migrations
- Handles persistence and external integrations
- Depends on Model for entity definitions

Contracts
- Contains DTOs and shared enums
- Used by both App and API
- No dependencies on any other project

🔄 Communication Flow
[MAUI App]
    ↓
HttpClient (DTOs from Contracts)
    ↓
[API Controllers]
    ↓
[API Services] (e.g. AuthService)
    ↓
[DataContext (EF Core)]
    ↓
[MySQL or other DB]
App calls API via HTTP, sends and receives DTOs. API maps to domain models and accesses the database.

🔁 Project Reference Rules
Project	Can Reference
App	Contracts only
API	Model, Infrastructure, Contracts
Infrastructure	Model only
Model	Nothing
Contracts	Nothing

✅ Benefits
- Clear separation of concerns
- App is decoupled from internal logic and data access
- Scales easily to web, desktop, or additional APIs
- Easy to test and mock each layer
- Safe to evolve backend independently of frontend

⚠️ Common Pitfalls to Avoid
❌ App referencing Model
❌ Contracts referencing Model or entities
❌ API exposing domain entities directly
❌ Domain services accessing EF Core or configuration

💡 Dev Tip
Use AutoMapper in the API to handle:
- DTO ↔ Entity mappings
- Enum mappings (e.g., Contracts.Enums.UserRole ↔ Model.Enums.UserRole)

Use HttpClient in App to call API like:
await Http.PostAsJsonAsync("api/auth/login", new LoginRequest { ... });

Thank u!
