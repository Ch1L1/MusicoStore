# MusicoStore

Managing music equipment inventory with products, categories, and manufacturers.

## Quick Start

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022

### Run the Application 

1. **Clone the repository**
2. **Apply database migrations**
3. **Run the application**

## Project Structure
MusicoStore/ <br />
├── src/ <br />
│	├── MusicoStore.WebApi/          # API Controllers, Middleware <br />
│   ├── MusicoStore.Application/     # Business logic, DTOs, Interfaces <br />
│   ├── MusicoStore.Domain/          # Domain entities <br />
│   └── MusicoStore.Infrastructure/  # Data access, Repositories <br />
└── docs/                            # Documentation and diagrams<br />
│   ├── diagrams/              <br />      


## Key Features

- Product management with CRUD operations
- Product categorization system
- Filtering by name, description, price, category, and manufacturer

## Documentation

- **[Technical Overview](docs/README.md)** - Everything you need to know
- **[Use Case Diagram](docs/diagrams/use-case-diagram.png)** - System use cases
- **[ERD Diagram](docs/diagrams/erd-diagram.png)** - Database schema

## Technology Stack

- **Framework:** ASP.NET Core 9.0
- **Database:** 
- **ORM:** Entity Framework Core
- **API Docs:** Swagger/OpenAPI

## Team

- **[Team Lead Name]** - Richard Harman
- **[Member 1 Name]** - Jan Nouza
- **[Member 2 Name]** - Tomas Homola