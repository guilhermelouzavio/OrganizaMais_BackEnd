# Simple Financial Control API

This project implements a robust and scalable Financial Control API using .NET 8 (or higher), following a Clean Architecture approach with CQRS (Command Query Responsibility Segregation) and leveraging Dapper for database interactions. PostgreSQL is used as the primary database.

## Architecture Overview

This API is structured around **Clean Architecture** principles, aiming for a strong separation of concerns, high maintainability, and testability.

### Core Layers:

1.  **`SimpleBank.Domain`**:
    * **Heart of the application.** This layer contains the core business logic, entities, value objects, domain events, and interfaces for repositories.
    * It is completely independent of external frameworks or databases.
    * **Key Components**:
        * `Entities`: `User`, `FinancialAccount`, `Category`, `Transaction`.
        * `Interfaces`: `IUnitOfWork`, `IRepository<T>`, `IUserRepository`, etc. (defines contracts for persistence).
        * `Enums`: Business-specific enumerations like `TransactionType`.
        * `Specifications`: Defines reusable query criteria (e.g., `UserByEmailSpecification`).

2.  **`SimpleBank.Application`**:
    * **Application-specific business rules.** This layer orchestrates the domain entities to perform application use cases.
    * It depends on `SimpleBank.Domain`.
    * **Key Components**:
        * **CQRS (Command Query Responsibility Segregation)**:
            * `Commands`: Objects representing intentions to *change* the system's state (e.g., `CreateUserCommand`).
            * `Queries`: Objects representing intentions to *read* the system's state (e.g., `GetUserByIdQuery`).
            * `Handlers`: Classes that process `Commands` and `Queries`, interacting with domain entities and repositories.
        * **MediatR**: Used as an in-process mediator to dispatch commands and queries to their respective handlers, promoting loose coupling.
        * `DTOs (Data Transfer Objects)`: Objects used to transfer data between the application and external layers (e.g., API consumers).
        * **Chain of Responsibility (via MediatR Behaviors)**:
            * `Behaviors`: Middleware in the MediatR pipeline. We implement a `ValidationBehavior` that uses `FluentValidation`.
            * `Validators`: `FluentValidation` classes that define validation rules for `Commands` before they reach their handlers.
        * **Strategy Pattern**:
            * `Interfaces`: `ITransactionFeeStrategy` defines the contract for different transaction fee calculation methods.
            * `Strategies`: Concrete implementations (e.g., `CreditCardFeeStrategy`, `PixFeeStrategy`) encapsulate varying business logic based on transaction types.

3.  **`SimpleBank.Infrastructure`**:
    * **External concerns and data persistence.** This layer handles database interactions, external services, file system operations, etc.
    * It depends on `SimpleBank.Domain` and is agnostic of `SimpleBank.Application` (though `Application` depends on `Domain`'s repository interfaces implemented here).
    * **Key Components**:
        * `Repositories`: Implementations of the repository interfaces defined in `SimpleBank.Domain`, using **Dapper** for efficient SQL queries.
        * `UnitOfWork`: Implements `IUnitOfWork` from `Domain`, coordinating persistence operations. (Note: With Dapper, transactions spanning multiple operations need explicit connection/transaction management within the UoW).
        * `Npgsql`: The ADO.NET data provider for PostgreSQL.
        * `Specifications`: `SpecificationEvaluator` helps convert domain specifications into SQL WHERE clauses for Dapper (simplified for this project).

4.  **`SimpleBank.Api`**:
    * **Entry point of the application.** This layer exposes the API endpoints to external clients.
    * It depends on `SimpleBank.Application` (to dispatch Commands/Queries) and `SimpleBank.Infrastructure` (for dependency injection setup).
    * **Key Components**:
        * `Controllers`: ASP.NET Core MVC Controllers that receive HTTP requests, dispatch `Commands` and `Queries` via `IMediator`, and return HTTP responses.
        * `Program.cs`: Configures the ASP.NET Core host, registers all services (MediatR, FluentValidation, repositories, strategies), and sets up the HTTP request pipeline (Swagger, Authorization, etc.).
        * `appsettings.json`: Configuration file, primarily for the database connection string.

### How Data Flows (CQRS in Action):

* **Write Operations (Commands)**:
    1.  Client sends an HTTP `POST`/`PUT`/`DELETE` request to `SimpleBank.Api/Controllers`.
    2.  The Controller creates and dispatches a `Command` object to `IMediator.Send()`.
    3.  `MediatR`'s `ValidationBehavior` (Chain of Responsibility) intercepts the `Command`, runs `FluentValidation` rules, and potentially throws a `ValidationException`.
    4.  If valid, `MediatR` dispatches the `Command` to its specific `CommandHandler` in `SimpleBank.Application`.
    5.  The `CommandHandler` orchestrates business logic:
        * Uses `SimpleBank.Domain` entities for state management.
        * Interacts with `SimpleBank.Domain/Repositories` interfaces.
        * Uses `SimpleBank.Application/Strategies` for variable logic (e.g., fee calculation).
        * Commits changes via `SimpleBank.Infrastructure/UnitOfWork` (which uses `Dapper` to talk to `PostgreSQL`).
    6.  The result (e.g., a DTO) is returned through the pipeline back to the Controller.
    7.  The Controller returns an appropriate HTTP response.

* **Read Operations (Queries)**:
    1.  Client sends an HTTP `GET` request to `SimpleBank.Api/Controllers`.
    2.  The Controller creates and dispatches a `Query` object to `IMediator.Send()`.
    3.  `MediatR` dispatches the `Query` to its specific `QueryHandler` in `SimpleBank.Application`.
    4.  The `QueryHandler` retrieves data:
        * Interacts with `SimpleBank.Domain/Repositories` interfaces.
        * Uses `SimpleBank.Infrastructure/Repositories` (Dapper) to query `PostgreSQL`.
    5.  The data is mapped to a `DTO` and returned through the pipeline back to the Controller.
    6.  The Controller returns an appropriate HTTP response.
