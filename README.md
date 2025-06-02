# User Management System – ASP.NET Core 8

This project is a secure, modular User Management System built using ASP.NET Core 8. It provides a RESTful API for managing user accounts, including functionality for user registration, authentication using JSON Web Tokens (JWT), protected user CRUD operations, webhook notifications on user login, and structured logging via Serilog.

## Getting Started

To run this project, you will need the .NET 8 SDK installed on your machine along with access to a relational database such as SQL Server or SQLite. You may also find it helpful to install the EF Core CLI tools for managing migrations.

First, clone the repository to your local machine and navigate into the project directory. Once inside, open the `appsettings.json` file located in the `UserManagementSystem.API` folder. Here, you should update the database connection string, JWT configuration, and webhook URL to match your environment.

With your configuration in place, apply any pending migrations using the Entity Framework CLI. This ensures the necessary database schema is created. You can then launch the application using the `dotnet run` command, pointing to the `UserManagementSystem.API` project. Once the API is running, you can explore the available endpoints via Swagger UI at `https://localhost:{port}/swagger`.

## Authentication and Authorization

Authentication is implemented using JWTs, which are issued to users upon successful login. These tokens include user-specific claims such as their unique identifier and email address. JWTs are signed using a symmetric key defined in the configuration file and include a one-hour expiration by default.

To access protected endpoints, clients must include a valid token in the `Authorization` header of each request using the `Bearer` scheme. The token is then validated by middleware before the request reaches the controller.

## API Overview

The API exposes two public endpoints for authentication. The `/auth/register` endpoint allows clients to create new user accounts, while the `/auth/login` endpoint authenticates users and returns a JWT. All remaining endpoints are protected and require authentication.

Authenticated users can retrieve all registered users, get individual user records by ID, update user information, or delete user accounts. Each of these actions maps to a corresponding HTTP method and route under the `/users` path.

## Webhook Integration

On each successful login, the system sends a POST request to a configured webhook URL. This request contains a payload listing all users who have logged in within the past 30 minutes. The webhook payload includes the user’s ID, username, email address, and the timestamp of their most recent login. The target URL for the webhook is defined in the application's configuration.

If the webhook fails to deliver, the failure is logged via Serilog. However, the application does not attempt automatic retries. This behavior can be extended if required.

## Logging

Logging is provided by Serilog and is enabled by default to write to the console. You can easily configure Serilog to write logs to a file, structured data store, or external logging service.

The system logs all significant events, including user registration, successful and failed login attempts, webhook delivery results, and unhandled exceptions. Log levels are used appropriately to distinguish between informational messages, warnings, and errors, allowing for effective monitoring and debugging in production environments.

## Data Contracts

The API expects and returns JSON payloads for all requests and responses.

To register a new user, clients must provide a username, email address, password, and the user’s first and last names. The registration response contains a JWT. Similarly, the login endpoint accepts an email and password and returns a JWT if the credentials are valid.

For user-related operations, the API returns user objects containing a unique identifier, username, email address, and the time of the user's last login. Updating or deleting users requires a valid JWT and returns appropriate HTTP status codes indicating success or failure.

## Technologies Used

This project is built using ASP.NET Core 8 and Entity Framework Core for data access. JWT-based authentication is implemented for secure access to protected routes. Webhook integration is included to notify external systems of user login events. Serilog is used for application-level logging. The project follows clean architecture principles, using layered separation of concerns with services, repositories, DTOs, and models. Swagger is used for API exploration and testing during development.
