# 🏨 Hotel Reservation Management System

A production-style **Hotel Reservation Management System** built with **ASP.NET Core 8**,
**Razor Pages**, **Entity Framework Core**, **MySQL** (via Pomelo), **ASP.NET Identity**,
**Bootstrap 5**, **Chart.js** and **Swagger**.

The application is a complete admin dashboard: manage hotels, room types, rooms, customers,
employees, reservations, payments and services; view a rich dashboard with charts; and export
reports to PDF / CSV / Excel.

---

## 🧱 Architecture

The solution follows a **clean / layered architecture** with four projects. Dependencies
point toward the application/domain core: Web composes the app, Application owns use-case
interfaces and business logic, and Infrastructure provides the EF Core adapter.

```
┌─────────────────────────────────────────────────────────────┐
│  HotelReservation.Web        (Presentation / UI + API)        │
│  Razor Pages · Controllers/Api · Middleware · ViewModels   │
└───────────────┬───────────────────────────────────────────┘
                 │ uses
┌───────────────▼───────────────────────────────────────────┐
│  HotelReservation.Application  (Business Logic)              │
│  Interfaces (I*Service, DbContext seam) · Services · DTOs   │
└───────────────┬───────────────────────────────────────────┘
                 │ uses Domain models
┌───────────────▼───────────────────────────────────────────┐
│  HotelReservation.Domain       (Entities & Enums)            │
│  Models (Hotel, Room, Reservation, …) · Enums            │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│  HotelReservation.Infrastructure (Data Access)               │
│  HotelReservationDbContext adapter · EF Core Migrations     │
└─────────────────────────────────────────────────────────────┘
```

| Project | Responsibility |
|----------|----------------|
| **Domain** | Plain entity models (`Hotel`, `Room`, `RoomType`, `Customer`, `Employee`, `Reservation`, `Payment`, `Service`, `ReservationService`, `Department`) and enums (`ReservationStatus`, `RoomStatus`, `PaymentStatus`, `PaymentMethod`). No external dependencies. |
| **Infrastructure** | `HotelReservationDbContext` (EF Core) and Migrations. Owns the database mapping and relationships, and adapts the Application data seam. |
| **Application** | Service interfaces + implementations and DTOs. Encapsulates all business rules (e.g. room-availability check, price calculation). Depends on Domain plus EF Core abstractions, not the Infrastructure project. |
| **Web** | Razor Pages (UI), REST API controllers, middleware, view models, design system (`admin.css`), and DI wiring in `Program.cs`. |

**Design highlights**
- **Repository-free service layer:** UI talks to `I*Service` interfaces; the Web project wires the concrete implementations via Dependency Injection.
- **DTOs separate wire/view models from persistence entities** — validation lives on the DTOs, not the domain.
- **Single DbContext** shared by Identity and the domain (`IdentityDbContext<IdentityUser>`).
- **Global exception middleware** returns friendly error pages for the UI and JSON for the API.

---

## 🗂️ Project Structure

```
HotelReservation.sln
├── docker-compose.yml          # MySQL 8 container (port 3307)
├── sql/
│   ├── 01_create_database.sql
│   ├── 02_create_tables.sql
│   ├── 03_sample_data.sql     # realistic seed data (auto-loaded when DB empty)
│   ├── 04_…08_*.sql         # example queries, views, stored procedures
│   └── mysql/                 # MySQL-flavoured equivalents
└── src/
    ├── HotelReservation.Domain/
    ├── HotelReservation.Infrastructure/
    ├── HotelReservation.Application/
    └── HotelReservation.Web/
        ├── Pages/              # Razor Pages (dashboard, CRUD, reports)
        ├── Controllers/Api/    # REST API (9 entities)
        ├── Middleware/         # GlobalException + RequestLogging
        ├── Helpers/           # PaginationHelper, StatusUi (badge colours)
        └── wwwroot/          # css/admin.css, js/admin.js
```

---

## ⚙️ Technology Stack

| Area | Technology |
|------|-------------|
| Framework | ASP.NET Core 8 (Razor Pages + Minimal/Controller API) |
| ORM | Entity Framework Core 8 (Pomelo MySQL provider) |
| Database | **MySQL 8** (Docker container) |
| Auth | ASP.NET Core Identity (Cookie) + Roles: Admin / Manager / Receptionist |
| UI | Bootstrap 5.3, Bootstrap Icons, Chart.js 4 |
| Docs | Swagger / OpenAPI 3 |
| Patterns | Clean Architecture, DI, DTOs, Repository-less service layer |

> ⚠️ **Note on the database:** the brief mentions "SQL Server", but this project is
> implemented against **MySQL** (Pomelo EF Core) running in Docker on port **3307**.
> The connection string and `DbContext` are already configured for MySQL — no change needed.

---

## 🚀 How to Run

### 1. Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (the repo was built with the .NET 8 targeting pack; SDK 10 also works via `RollForward`).
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) for the MySQL container.

### 2. Start the database
```bash
# From the repository root
docker compose up -d
```
This launches MySQL 8 on `localhost:3307` with database `HotelReservationDB`
(root password `Hotel@2024`). The container initialises the schema and example SQL
on first start.

### 3. Run the application
```bash
dotnet run --project src/HotelReservation.Web/HotelReservation.Web.csproj
```
On first launch the app:
1. applies EF Core migrations / ensures the schema,
2. seeds the Identity **roles** and, when configured, an initial **admin user**,
3. loads `sql/03_sample_data.sql` **only if the database is empty**.

### 4. Open the app
| Resource | URL |
|----------|-----|
| Application (dashboard) | https://localhost:5001/  (HTTP → https redirect from http://localhost:5000) |
| Swagger API docs | https://localhost:5001/swagger |
| Login | https://localhost:5001/Identity/Account/Login |

> The dev TLS certificate is self-signed — browsers will warn. Click
> **Advanced → Proceed**, or trust it once with `dotnet dev-certs https --trust`.

### 5. Create the first admin
The app no longer ships a hard-coded administrator password. Configure these values
through user-secrets, environment variables, or your deployment secret store before
first launch:

```
AdminSeed:Email
AdminSeed:Password
```

For environment variables, use `AdminSeed__Email` and `AdminSeed__Password`.

---

## 🔑 Features

- **Dashboard** — stat cards (hotels, rooms, customers, revenue) + Chart.js graphs
  (revenue trend, reservation-status doughnut, room-occupancy bar), latest reservations,
  latest customers, popular services.
- **Full CRUD** for all 9 entities (Hotel, RoomType, Room, Customer, Employee,
  Reservation, Payment, Service, ReservationService) with foreign-key dropdowns.
- **Business workflow** — create customer → reserve room (with availability check) →
  record payment → confirm → report.
- **Reports** — Reservation Summary & Monthly Revenue with filters, **Print**,
  **Export PDF** (browser print), **CSV** and **Excel** export.
- **REST API** — `GET/POST/PUT/DELETE` for every entity, documented in Swagger.
- **Identity & roles**, **global exception handling**, friendly 404/403/500 pages,
  request-logging middleware.
- **Responsive admin UI** — sidebar, breadcrumbs, status badges, search / filter /
  sort / pagination on every list.

---

## 🧪 Seeding & Sample Data

`sql/03_sample_data.sql` contains realistic data:

| Table | Rows |
|-------|------|
| Hotels | 5 |
| RoomTypes | 5 |
| Departments | 10 |
| Rooms | 25 |
| Customers | 30 |
| Employees | 12 |
| Services | 10 |
| Reservations | 40 |
| Payments | 40 (1 per reservation) |
| ReservationServices | 50 |

To **re-seed** from scratch, drop the database in the container and restart the app,
or run the script manually against `HotelReservationDB`.

---

## 📦 Build & Verify

```bash
dotnet build src/HotelReservation.Web/HotelReservation.Web.csproj
# Build succeeds with 0 errors, 0 code warnings.
```

---

## 📝 Notes

- The application is **demo / university-submission ready**: every page is styled,
  every entity is manageable through the UI, and the workflow is end-to-end functional.
- Keep `docker-compose.yml` and `appsettings.json` connection strings in sync if you
  change the database credentials.
