# MusicoStore

Managing music equipment inventory with products, categories, and manufacturers.

## Team

- **[Team Lead Name]** - Richard Harman
- **[Member 1 Name]** - Jan Nouza
- **[Member 2 Name]** - Tomas Homola

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022
- Docker

### Run the Application 

1. **Clone the repository**
2. **Apply database migrations**
3. **Run the application**

### Run (Windows – LocalDB default)
```bash
# at repo root
dotnet run --project src/MusicoStore.WebApi
```
- API → http://localhost:5210/swagger  
- Minimal UI → http://localhost:5210/  
- Auth header required (see **Auth** below)

### Run (Docker – SQL Server)
1) Create a `.env`, and set your password and Database Port:
```env
SA_PASSWORD=???
DB_PORT=1433
```
2) For macOS (Apple Silicon), synchronise the passwords from `.env` in `appsetings.json` in `LocalMacOSConnection` string:
```
 "LocalMacOSConnection": "Server=localhost,1433;Database=MusicoStoreDb;User Id=sa;Password=???;TrustServerCertificate=True"
```
3) Start the Database container with docker:  
   - Windows/Linux:
     ```bash
     docker compose up -d
     ```
   - macOS (Apple Silicon):
     ```bash
     docker compose -f docker-compose-macos.yml up -d
     ```
4) Run the API:
```bash
dotnet run --project src/MusicoStore.WebApi
```

> On first run the app **creates and seeds** the DB (see *Seeding & Reset*).

## Project Structure
MusicoStore/ <br />
├─ src/ <br />
│  ├─ MusicoStore.WebApi/            # Controllers, Middleware, wwwroot (mini UI) <br />
│  ├─ MusicoStore.WebMVC/            # MVC UI (registration/login, basic front-end) <br /> 
│  ├─ MusicoStore.DataAccessLayer/   # EF Core DbContext, entities, seed data <br />
│  ├─ MusicoStore.BusinessLayer/     # services / business logic <br />
│  ├─ MusicoStore.Domain/            # entities, DTOs, mapping profiles, interfaces <br />
│  └─ MusicoStore.Mongo/             # MongoDB logging helpers <br /> 
├─ docs/ <br />
│  └─ diagrams/                      # ER & Use-case images & README<br />
├─ tests/ <br />
│  └─ MusicoStore.Tests/             # xUnit tests <br />
├─ docker-compose.yml                # SQL Server (Windows/Linux) <br />
├─ docker-compose-macos.yml          # SQL Server (Apple Silicon) <br />
├─ .editorconfig                     # C# style <br />
└─ MusicoStore.sln <br />


## Key Features

- Product management with CRUD operations
- Product categorization system
- Filtering by name, description, price, category, and manufacturer

## Documentation

- **[Technical Overview](docs/README.md)** - Everything you need to know
- **[Use Case Diagram](docs/diagrams/use_case_diagram.png)** - System use cases
- **[ERD Diagram](docs/diagrams/ER_diagram.png)** - Database schema

## Technology Stack

- **Framework:** ASP.NET Core 9
- **ORM:** EF Core 9
- **Database:** SQL Server 2022 (Docker) or LocalDB (Windows)
- **API Docs:** Swagger/OpenAPI

## Technical Overview

### Architecture (n-layer)
```
Web API (Controllers, Swagger, Static UI)
        ↓
Infrastructure (Repositories, DI)
        ↓
Data Access Layer (EF Core DbContext, Entities, Seed)
        ↓
SQL Server
```

### API Endpoints (v1)
- **Products** `/api/v1/products`  
  - `GET` (list or query via optional `name`, `desc`, `priceMax`, `category`, `manufacturer`)  
  - `GET /{id}`  
  - `POST`  
  - `PUT /{id}`  
  - `DELETE /{id}`
- **Categories** `/api/v1/productcategory` – CRUD
- **Manufacturers** `/api/v1/manufacturer` – CRUD
- **Addresses** `/api/v1/address` – CRUD
- **Customers** `/api/v1/customers` – CRUD
- **Customer Addresses** `/api/customers/{customerId}/addresses` – list/add/remove addresses for a customer
- **Storages** `/api/v1/storage` – CRUD
- **Orders** `/api/v1/orders` – CRUD

### Seeding & Reset of Database

- `SeedData.EnsureSeededAsync` currently contains:
  - `EnsureDeletedAsync()` **(temporary for testing)**  
  - `EnsureCreatedAsync()`  
  - Inserts demo data using Bogus, randomly generated on each startup for the development version.

**Reset DB (Docker):**
```bash
docker compose down -v          # remove container + volume
docker compose up -d            # rebuild
```

**Reset DB (LocalDB):**
- Stop API, then delete `MusicoStoreDb` via SQL Server Object Explorer and also MongoDB or
- Temporarily keep `EnsureDeletedAsync()` in seed (already present for M1).
