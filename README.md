# Transactions API

A robust RESTful API for managing financial transactions, built with ASP.NET Core 9 and PostgreSQL.

## Features

- **CRUD Operations**: Full support for creating, reading, updating, and deleting transactions.
- **Database Integration**: Powered by PostgreSQL using Entity Framework Core.
- **Architecture**: Implements the Repository Pattern for clean data access and testability.
- **Docker Support**: Containerized for easy deployment with Docker and Docker Compose.
- **API Documentation**: Integrated Swagger UI for interactive testing.

## Tech Stack

- **Framework**: .NET 9 (webapi)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Environment Management**: DotNetEnv
- **API UI**: Swagger / Swashbuckle

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run with Docker Compose

The easiest way to get the API and the Database running:

```bash
docker-compose up --build
```

The API will be available at `http://localhost:8080` (or `http://localhost:5253` if running locally).
Swagger documentation can be accessed at `/swagger`.

### Local Development

1.  Restore dependencies:
    ```bash
    dotnet restore
    ```
2.  Update your `appsettings.json` or `.env` with the PostgreSQL connection string.
3.  Run the application:
    ```bash
    dotnet run
    ```

## Project Structure

- `Controllers/` - REST API Endpoints
- `Models/` - Database entities and DB Context
- `Repositories/` - Data access logic abstractions
- `db/` - Database initialization and migrations

## License

MIT
