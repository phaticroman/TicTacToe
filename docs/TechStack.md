# Technology Stack Document

# TicTacToe Online

---

## 1. Document Information

| Field | Details |
|---|---|
| Project Name | TicTacToe Online |
| Project Type | Online Multiplayer Web Application |
| Document Version | 1.0 |
| Document Status | Draft |
| Date | November 4, 2026 |
| Prepared By | Senior .NET Solution Architect |
| Intended Audience | Developers, project supervisors |

---

## 2. Project Overview

TicTacToe Online is a simple browser-based multiplayer Tic Tac Toe game. Two players can play together online without registration or login.

One player creates a game room and receives a unique room code. Another player joins using that code. Both players then play in real time.

This document lists the technologies used to build the project and explains why each one was chosen.

---

## 3. Technology Stack Summary

| Category | Technology | Why It Is Used |
|---|---|---|
| Programming Language | C# | Modern, strongly typed language that fits the .NET ecosystem. |
| Framework | ASP.NET Core MVC (.NET 10) | Builds server-rendered web pages and handles HTTP requests. |
| Architecture | Clean Architecture | Keeps the code organized and easy to maintain. |
| Mediator Library | Cortex.Mediator | Handles Commands, Queries, and Handlers in a simple way. |
| Real-time Communication | ASP.NET Core SignalR | Sends game updates to both players instantly. |
| Frontend | HTML5, CSS3, JavaScript | Standard technologies for web pages and interactivity. |
| CSS Framework | Bootstrap 5 | Provides ready-made responsive components. |
| Database | Microsoft SQL Server | Reliable storage for game room and player data. |
| ORM | Entity Framework Core | Lets the application work with the database using C# objects. |
| Database Migration | EF Core Migrations | Tracks and applies database changes in a controlled way. |
| Dependency Injection | Built-in .NET DI | Manages dependencies without extra packages. |
| Containerization | Docker | Packages the application so it runs the same everywhere. |
| Container Orchestration | Docker Compose | Starts the application and SQL Server together. |
| IDE | Visual Studio 2022 | Full-featured development environment for .NET. |
| Version Control | Git and GitHub | Tracks code changes and supports teamwork. |

---

## 4. Backend Technologies

| Technology | Responsibility | Why It Is Used |
|---|---|---|
| C# | Main programming language for all backend code. | One language across all layers keeps the project simple. |
| ASP.NET Core MVC | Handles HTTP requests, controllers, Razor Views, and page rendering. | Suits a web application with pages and no need for a separate API. |
| Cortex.Mediator | Handles application requests using Commands, Queries, and Handlers. | Keeps each use case in its own small, easy-to-find handler. |
| SignalR | Enables real-time communication between the two players. | Delivers moves instantly without page refresh. |
| Built-in Dependency Injection | Creates and supplies the objects that classes depend on. | Included in .NET, so no extra package is needed. |

---

## 5. Frontend Technologies

| Technology | Responsibility | Why It Is Used |
|---|---|---|
| HTML5 | Defines page structure. | Standard foundation of every web page. |
| CSS3 | Handles styling and layout. | Controls the look of the game board and pages. |
| JavaScript | Runs the interactive game board and SignalR client communication. | Needed to update the board in the browser in real time. |
| Bootstrap 5 | Provides responsive UI components. | Makes the game work on desktop and mobile with little custom CSS. |

---

## 6. Database Technologies

| Technology | Responsibility | Why It Is Used |
|---|---|---|
| Microsoft SQL Server | Stores game room and player information. | Reliable, widely used, and works well with .NET. |
| Entity Framework Core | Handles database operations from C# code. | Reduces manual database code and keeps data access consistent. |
| EF Core Migrations | Manages database changes over time. | Lets the team update the database safely as the project grows. |

---

## 7. Architecture Decisions

The application follows Clean Architecture with four projects. Each project has a clear responsibility.

| Project | Responsibility |
|---|---|
| TicTacToe.Domain | Entities, enums, and core game rules. |
| TicTacToe.Application | Services, interfaces, DTOs, Commands, Queries, and Handlers. Cortex.Mediator belongs here. |
| TicTacToe.Infrastructure | EF Core, DbContext, database configurations, and persistence implementations. |
| TicTacToe.Web | MVC Controllers, Razor Views, SignalR Hubs, ViewModels, and static files. |

**Rules:**

- Cortex.Mediator belongs to the Application layer.
- SignalR Hubs belong to the Web layer.
- Business logic must not be implemented inside Controllers or SignalR Hubs. They only receive requests and pass them to the Application layer.

---

## 8. Development Tools

| Tool | Purpose |
|---|---|
| Visual Studio 2022 | Writing, running, and debugging the C# application. |
| SQL Server Management Studio (SSMS) | Viewing and managing the SQL Server database. |
| Docker Desktop | Running Docker containers on a developer's computer. |
| Git | Tracking changes to the source code. |
| GitHub | Hosting the repository and sharing code with the team and supervisors. |

---

## 9. Technical Constraints

| Constraint | Description |
|---|---|
| MVC only | Use ASP.NET Core MVC, not Web API. No separate Web API project. |
| Mediator library | Use Cortex.Mediator, not MediatR. |
| Real-time | Use SignalR for multiplayer communication. |
| Dependency injection | Use built-in .NET dependency injection. |
| No authentication | No login or registration system. |
| No extra abstractions | No CQRS frameworks or abstractions beyond the Command/Query handling required by Cortex.Mediator. |
| No Redis | Redis is not used. |
| No microservices | The application is a single deployable solution. |
| No cloud services | Version 1.0 does not require any cloud services. |
| Minimal packages | Avoid unnecessary third-party packages. |
| Local testing | Support multiplayer testing on localhost. |
| Local network | Support two computers on the same local Wi-Fi network. |
