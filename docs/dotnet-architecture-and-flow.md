# .NET Architecture and Flow

This document describes the ASP.NET Core side of the Hotel Reservation Management System: the project layout, dependency direction, module interfaces, request flow, and the main reservation/payment workflows.

## Runtime Shape

The app is an ASP.NET Core 8 modular monolith. The Web project hosts both Razor Pages for the admin UI and API controllers for JSON clients. Identity, domain data, migrations, startup seeding, middleware, Swagger, and static assets are composed from `Program.cs`.

```mermaid
flowchart TB
    Browser["Admin browser"] -->|"HTML forms, cookies"| Razor["HotelReservation.Web<br/>Razor Pages"]
    Client["API client"] -->|"JSON + auth cookie"| Api["HotelReservation.Web<br/>API controllers"]

    Razor -->|"I*Service interfaces"| App["HotelReservation.Application<br/>use cases + DTOs"]
    Api -->|"I*Service interfaces"| App

    App -->|"Domain models, enums, events"| Domain["HotelReservation.Domain"]
    App -->|"IHotelReservationDbContext seam"| DbSeam["Application data seam"]
    Infra["HotelReservation.Infrastructure<br/>EF Core adapter"] -->|"implements"| DbSeam
    Infra -->|"maps"| MySql[("MySQL 8<br/>HotelReservationDB")]
    WebComposition["Program.cs<br/>composition root"] -->|"registers adapter + services"| Infra
    WebComposition -->|"registers services"| App

    style App fill:#eef2ff,stroke:#4f46e5
    style Domain fill:#f0fdf4,stroke:#16a34a
    style Infra fill:#fff7ed,stroke:#ea580c
    style MySql fill:#f8fafc,stroke:#475569
```

## Project Responsibilities

| Project | Responsibility |
| --- | --- |
| `HotelReservation.Domain` | Plain domain entities, enums, and domain event contracts. It has no persistence or web dependency. |
| `HotelReservation.Application` | Use-case services, service interfaces, DTOs, business rules, status constants, pricing calculation, lookup queries, reporting queries, and the `IHotelReservationDbContext` data seam. |
| `HotelReservation.Infrastructure` | EF Core `HotelReservationDbContext`, MySQL mappings, Identity storage, and migrations. It adapts the Application data seam. |
| `HotelReservation.Web` | Razor Pages, API controllers, middleware, layout/assets, authentication setup, Swagger setup, DI composition, startup role/admin/sample-data seeding. |

## Dependency Direction

The important direction is toward the application/domain core:

```mermaid
flowchart LR
    Web["Web"] -->|"calls interfaces"| Application["Application"]
    Web -->|"composition root references"| Infrastructure["Infrastructure"]
    Infrastructure -->|"implements data seam"| Application
    Application -->|"uses models/events"| Domain["Domain"]
    Infrastructure -->|"maps entities"| Domain
```

`Application` does not reference the Infrastructure project directly. Instead, services depend on `IHotelReservationDbContext`. The concrete EF Core adapter is registered in Web:

- `AddDbContext<HotelReservationDbContext>(...)`
- `AddScoped<IHotelReservationDbContext>(sp => sp.GetRequiredService<HotelReservationDbContext>())`
- `AddScoped<IReservationService, ReservationService>()`, and the other application services.

This keeps business workflows testable through service interfaces while still allowing EF Core queries where the app currently needs them.

## Core Modules

| Module | Interface | Main role |
| --- | --- | --- |
| `ReservationService` | `IReservationService` | Validates reservation dates, checks room availability, calculates base room total, persists reservations, and publishes reservation events. |
| `ReservationServiceEntityService` | `IReservationServiceEntityService` | Attaches paid services to reservations, enforces the composite key, snapshots service price, and recomputes totals. |
| `PaymentService` | `IPaymentService` | Enforces one payment per reservation, validates payment method/status, and manages payment records. |
| `RoomService` | `IRoomService` | Manages rooms and availability filtering. |
| `ReportService` | `IReportService` | Builds reservation summary, monthly revenue, and popular service projections. |
| `LookupService` | `ILookupService` | Supplies UI dropdown/search data without Razor Pages querying DbContext directly. |
| `ReservationPricingCalculator` | static module | Calculates room total and full reservation total from room type price, stay dates, and service total. |
| `DomainValues` | static module | Centralizes reservation, room, payment status, and payment method values. |

## Request Flow

```mermaid
sequenceDiagram
    autonumber
    participant U as User
    participant RP as Razor Page / API Controller
    participant App as Application Service
    participant DB as IHotelReservationDbContext
    participant EF as EF Core MySQL Adapter
    participant MySQL as MySQL

    U->>RP: Submit form or call API
    RP->>App: Call I*Service with DTO
    App->>App: Validate status, dates, pricing, invariants
    App->>DB: Query/update aggregate data
    DB->>EF: Adapter dispatch
    EF->>MySQL: SQL
    MySQL-->>EF: Rows/result
    EF-->>DB: Entities/projection
    DB-->>App: Result
    App-->>RP: DTO / success / domain error
    RP-->>U: HTML redirect/page or JSON response
```

## Reservation Flow

```mermaid
sequenceDiagram
    autonumber
    participant UI as Reservation page/API
    participant S as ReservationService
    participant DB as Data seam
    participant Price as ReservationPricingCalculator
    participant Events as DomainEventPublisher

    UI->>S: CreateReservationDto
    S->>S: Require CheckOutDate > CheckInDate
    S->>DB: Find overlapping non-cancelled reservations
    alt room unavailable
        S-->>UI: InvalidOperationException
    else room available
        S->>DB: Load room + room type
        S->>Price: CalculateRoomTotal(room, dates)
        Price-->>S: TotalAmount
        S->>DB: Add reservation + SaveChanges
        S->>Events: Publish ReservationCreatedEvent
        S-->>UI: ReservationDto
    end
```

Availability uses overlap logic:

- same `RoomID`
- status is not `Cancelled`
- existing `CheckInDate < requested CheckOutDate`
- existing `CheckOutDate > requested CheckInDate`

## Reservation Status Flow

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Confirmed: update status
    Pending --> Cancelled: update status
    Confirmed --> Completed: checkout / manual update
    Confirmed --> Cancelled: cancellation
    Completed --> [*]
    Cancelled --> [*]
```

When a reservation transitions to `Confirmed`, `ReservationConfirmedEventHandler` reconciles room status from `Available` to `Occupied` if it is still safe to do so. When it transitions to `Cancelled`, a cancellation event is published for future side effects.

## Payment Flow

```mermaid
sequenceDiagram
    autonumber
    participant UI as Payment page/API
    participant S as PaymentService
    participant DB as Data seam

    UI->>S: CreatePaymentDto
    S->>DB: Check reservation exists
    alt reservation missing
        S-->>UI: KeyNotFoundException
    else reservation exists
        S->>DB: Check no payment exists for reservation
        alt duplicate payment
            S-->>UI: InvalidOperationException
        else first payment
            S->>S: Validate method/status via DomainValues
            S->>DB: Add payment + SaveChanges
            S-->>UI: PaymentDto
        end
    end
```

## Startup Flow

```mermaid
flowchart TD
    Start["Program.cs starts"] --> Db["Configure MySQL DbContext"]
    Db --> Identity["Configure ASP.NET Identity + roles"]
    Identity --> Services["Register application services, lookup service, event handlers"]
    Services --> Pipeline["Configure middleware, static files, auth, Razor Pages, controllers"]
    Pipeline --> SeedRoles["Create Admin / Receptionist / Manager roles"]
    SeedRoles --> AdminConfig{"AdminSeed configured?"}
    AdminConfig -->|"yes"| AdminUser["Create initial admin if missing"]
    AdminConfig -->|"no"| SkipAdmin["Skip admin seed"]
    AdminUser --> SampleData{"Hotels empty?"}
    SkipAdmin --> SampleData
    SampleData -->|"yes"| LoadSql["Load sql/03_sample_data.sql"]
    SampleData -->|"no"| Run["Run Kestrel"]
    LoadSql --> Run
```

The initial admin is no longer hard-coded. Configure:

- `AdminSeed:Email`
- `AdminSeed:Password`

For environment variables use:

- `AdminSeed__Email`
- `AdminSeed__Password`

## UI Assets

The admin UI uses Bootstrap 5, Bootstrap Icons, Chart.js, and project-specific `admin.css`/`admin.js`. Vendor assets are stored locally under `wwwroot/lib`, so the UI does not depend on external CDN availability at runtime.
