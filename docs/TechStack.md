# Technology Stack Document
## Tic-Tac-Toe Web Application

---

## 1. Document Information

| Field | Details |
|---|---|
| **Document Title** | Technology Stack Document |
| **Project Name** | Tic-Tac-Toe Web Application |
| **Project Type** | Full-Stack Web Application (Course Assignment) |
| **Version** | 1.1 |
| **Date** | November 4, 2026 |
| **Document Status** | Draft – Submitted for Instructor Review |
| **File Location** | `docs/TechStack.md` |
| **Related Documents** | Product Requirements Document (PRD), Architecture Document (`Architecture.md`), Software Requirements Specification (SRS), Database Design, UI/UX Design |

### 1.1 Revision History

| Version | Date | Change |
|---|---|---|
| 1.0 | November 4, 2026 | Initial version |
| 1.1 | November 4, 2026 | Updated for Simplified Clean Architecture with four projects (Domain, Application, Infrastructure, Web). Replaced the single-project layout and the diagram that connected MVC directly to Entity Framework Core. Resolves items C-02 and C-04 of `Architecture.md`. |

---

## 2. Introduction

### 2.1 Purpose

This document describes the technologies selected to build the Tic-Tac-Toe Web Application. It explains what each technology is, why it was chosen, and how all technologies work together. It guides the design and development stages that follow.

### 2.2 Audience

| Audience | Use of This Document |
|---|---|
| Course instructor | Reviews the technical choices and their justification |
| Developer(s) | Uses the document as a reference during design and implementation |
| Future maintainers | Understands how the application is structured |

### 2.3 Scope

This document covers the technologies, tools, architecture, and integration approach. It does not contain source code, detailed database tables, or user interface designs. Those are covered in separate documents. The detailed architecture (layers, dependency rules, and components) is described in `Architecture.md`; this document summarizes it from the technology point of view.

---

## 3. Project Overview

| Item | Description |
|---|---|
| **Project Name** | Tic-Tac-Toe Web Application |
| **Project Type** | Full-Stack Web Application |
| **Description** | A web-based Tic-Tac-Toe game where two players play against each other on the same device |
| **Main Features** | Home page, player name input, interactive 3×3 board, automatic winner and draw detection, result display, restart, scoreboard, and game history |
| **Users** | Two players sharing one device and one browser |
| **Data Stored** | Player names, game results, and date/time of completed games |

### 3.1 Technical Design Principles

| Principle | Meaning for This Project |
|---|---|
| **Simplicity** | Use the fewest technologies and layers needed to deliver the features |
| **Beginner-friendly** | Choose well-documented tools that are commonly taught in courses |
| **Single application** | Build one deployable web application that serves pages and handles data; no separate frontend project |
| **Clean separation** | Organize the code into four projects (Simplified Clean Architecture) so business rules stay independent of the web and database technologies |
| **Consistency** | Keep every technology choice aligned with the project's PRD scope |
| **Maintainability** | Keep responsibilities clearly separated so each part is easy to understand and change |

---

## 4. Technology Stack Overview

### 4.1 Selected Stack Summary

| Layer | Technology | Role in the Project |
|---|---|---|
| **Frontend** | HTML5 | Page structure and content |
| | CSS3 | Custom styling and visual appearance |
| | Bootstrap 5 | Responsive layout and ready-made interface components |
| | Vanilla JavaScript | Interactive game behavior in the browser |
| **Backend** | ASP.NET Core MVC | Receives requests, calls the Application layer, and returns HTML pages |
| | C# | Programming language of the backend |
| | Simplified Clean Architecture | Organizes the backend into four projects: Domain, Application, Infrastructure, and Web |
| **Database** | Microsoft SQL Server | Permanent storage of game records |
| **ORM** | Entity Framework Core | Connects C# code to the database; used only in the Infrastructure project |
| **Tools** | Visual Studio 2022, Visual Studio Code, SSMS, Git, GitHub | Development, database management, and version control |

### 4.2 Technologies Intentionally Not Used

| Technology / Approach | Reason Not Used |
|---|---|
| React, Angular, Vue | A separate frontend framework adds complexity that the project does not need |
| Node.js | The backend is fully built with C# and ASP.NET Core |
| Separate frontend application | The project is a single application; MVC views provide the pages |
| Microservices | A small two-player game does not need multiple services |
| Online multiplayer tools | Out of scope; both players use the same device |
| AI components | The application has no AI features; AI is used only as a development assistant |

---

## 5. Frontend Technologies

### 5.1 HTML5

| Aspect | Description |
|---|---|
| **What it is** | The standard language for structuring web page content |
| **Use in project** | Defines the Home page, player setup form, game board, scoreboard table, and history table |
| **Why selected** | It is the foundation of every web page, is supported by all browsers, and is easy to learn |

### 5.2 CSS3

| Aspect | Description |
|---|---|
| **What it is** | The standard language for styling web pages |
| **Use in project** | Styles the game board, X and O symbols, winning-cell highlight, and overall look |
| **Why selected** | It allows the interface to be clear and attractive without extra tools |

### 5.3 Bootstrap 5

| Aspect | Description |
|---|---|
| **What it is** | A popular CSS framework with a grid system and prebuilt components |
| **Use in project** | Responsive layout, navigation bar, buttons, forms, and tables |
| **Why selected** | It speeds up development, ensures a consistent look, and makes pages work on desktop, tablet, and phone |

### 5.4 Vanilla JavaScript

| Aspect | Description |
|---|---|
| **What it is** | Standard JavaScript used directly, without any framework or library |
| **Use in project** | Handles cell clicks, turn switching, winner and draw detection, result display, and restart behavior in the browser |
| **Why selected** | It is built into every browser, needs no installation, and helps beginners understand core programming concepts |

### 5.5 Frontend Responsibilities

| Responsibility | Handled By |
|---|---|
| Page structure | HTML5 |
| Visual design | CSS3 and Bootstrap 5 |
| Responsive layout | Bootstrap 5 |
| Game interaction and rules during play | Vanilla JavaScript |
| Form input fields (player names) | HTML5 with Bootstrap 5 styling |

---

## 6. Backend Technologies

### 6.1 ASP.NET Core MVC

| Aspect | Description |
|---|---|
| **What it is** | A Microsoft framework for building web applications using the Model-View-Controller pattern |
| **Use in project** | Forms the presentation layer (`TicTacToe.Web` project): receives browser requests, passes them to the Application layer, and returns HTML pages |
| **Why selected** | It provides a clear structure, includes built-in page rendering, and lets one web application serve pages and receive game results without a separate frontend |

### 6.2 C#

| Aspect | Description |
|---|---|
| **What it is** | A modern, object-oriented programming language by Microsoft |
| **Use in project** | Used to write the Domain rules, Application services, Infrastructure data access, and Web Controllers |
| **Why selected** | It is the main language of ASP.NET Core, is readable and strongly typed, and is widely taught in courses |

### 6.3 The MVC Pattern

| Component | Responsibility | Example in This Project |
|---|---|---|
| **Model** | Represents data. In this solution the role is split: business data in the Domain, data carriers (DTOs) in the Application layer, and page data (ViewModels) in the Web project | The `Game` entity (Domain); a scoreboard entry DTO (Application); a history page ViewModel (Web) |
| **View** | Produces the HTML page shown to the user | Home page, game page, scoreboard page, history page |
| **Controller** | Receives a request, coordinates the work, and chooses the response | Controllers for the home page, the game, the scoreboard, and the history |

> **Note:** MVC is used only in the `TicTacToe.Web` project (presentation layer). Business rules and database access are in other projects. See Section 10.

### 6.3.1 How MVC Handles Incoming HTTP Requests

1. The user opens a page or submits a form in the browser, which sends an HTTP request to the server.
2. ASP.NET Core receives the request and uses **routing** to match the web address to a specific Controller and action.
3. The selected **Controller action** runs and processes the request.
4. The Controller calls an **Application service**. The service applies the business rules (defined in the Domain) and, when data is needed, uses a repository interface. The Infrastructure project implements this interface with Entity Framework Core.
5. The service returns a result to the Controller, which prepares a **ViewModel** with the data to display.
6. The Controller passes the ViewModel to a **View**, which creates the final HTML page.
7. The server sends the HTML (or a small data response) back to the browser.

### 6.3.2 How Controllers Process User Requests

| Step | Controller Activity |
|---|---|
| 1 | Receive the request and any submitted data (for example, player names or a game result) |
| 2 | Check that the submitted data has the expected format; the Application and Domain layers apply the business rules (for example, names must not be empty, must be different, and must be at most 20 characters) |
| 3 | Call an Application service, which performs the work and reaches the database only through an interface |
| 4 | Build the ViewModel needed by the page from the service result |
| 5 | Return a View, a redirect, or a small response to the browser |

### 6.3.3 How Models Represent Application Data

Data is represented by C# classes placed in the project where they belong:

| Kind of Class | Project | Example | Purpose |
|---|---|---|---|
| **Entity** | Domain | `Game` | One completed match (players, result, winner, date and time). It carries the business rules, such as the required names, the 20-character limit, and a result that matches the winner. It is the only class saved in the database |
| **DTO** (data transfer object) | Application | Scoreboard entry, history item, save-game request | Carries data between the Web project and the Application layer without exposing entities |
| **ViewModel** | Web | History page model, scoreboard page model | Holds exactly the data a View needs to display |
| **Input model** | Web | Data posted by the browser | Receives submitted data before it is passed to the Application layer |

### 6.3.4 How Views Render HTML Pages

Views are page templates that combine fixed HTML with data supplied by the Controller. For example, the History View receives a list of games and creates one table row for each game. Views use Bootstrap 5 classes for layout and styling, and a shared layout keeps the navigation bar and page frame the same on every page.

---

## 7. Database Technology

### 7.1 Microsoft SQL Server

| Aspect | Description |
|---|---|
| **What it is** | A relational database management system from Microsoft |
| **Use in project** | Permanently stores completed games so that the scoreboard and history remain available after the application is closed |
| **Why selected** | It integrates well with ASP.NET Core and Entity Framework Core, is reliable, and is commonly used in courses |

### 7.2 Data Stored

| Data Item | Purpose |
|---|---|
| Game identifier | Uniquely identifies each saved game |
| Player X name and Player O name | Shows who played |
| Result and winner name | Supports the scoreboard and history |
| Date and time played | Supports ordering of the history |

### 7.3 Database Principles

| Principle | Description |
|---|---|
| Only completed games are stored | Unfinished or restarted games are not saved |
| Scoreboard is calculated from saved games | Win totals are never edited manually |
| Minimal personal data | Only player names, results, and timestamps are kept |

Detailed table and column design is provided in the separate Database Design document.

---

## 8. ORM Technology

### 8.1 Entity Framework Core

| Aspect | Description |
|---|---|
| **What it is** | An Object-Relational Mapper (ORM) that lets C# code work with database data as objects |
| **Use in project** | Saves completed games, retrieves the history, and calculates win totals for the scoreboard |
| **Why selected** | It removes the need to write most database commands by hand, reduces errors, and supports safe data access |
| **Where it is used** | Only in the `TicTacToe.Infrastructure` project. No other project references it |

### 8.2 How Entity Framework Core Communicates with SQL Server

| Step | Description |
|---|---|
| 1 | A **database context** class represents the connection to the database and the collections of stored data |
| 2 | The repository class in the Infrastructure project asks the context to add, read, or query records using C#. It does this for the Application services, which know it only through an interface |
| 3 | Entity Framework Core translates these C# operations into database commands |
| 4 | SQL Server runs the commands and returns the results |
| 5 | Entity Framework Core converts the results back into Domain entity objects (C#) |

### 8.3 Database Creation and Changes

The database structure is created and updated through Entity Framework Core **migrations**. A migration is a recorded set of structure changes that can be applied to the database. This keeps the database consistent with the Domain entity and makes the setup repeatable for any developer. Migrations are stored in the Infrastructure project, and the Web project is used as the startup project.

---

## 9. Development Tools and Environment

| Tool | Purpose | Why Selected |
|---|---|---|
| **Visual Studio 2022** | Main environment for developing the ASP.NET Core MVC application in C# | Full support for ASP.NET Core, debugging, and Entity Framework Core |
| **Visual Studio Code** | Lightweight editor for HTML, CSS, JavaScript, and Markdown documents | Fast, free, and good for front-end files and documentation |
| **SQL Server Management Studio (SSMS)** | Viewing and checking the database and its stored data | Official tool for SQL Server; helps verify saved results |
| **Git** | Local version control | Tracks changes and allows safe experimentation |
| **GitHub** | Online repository hosting | Backup, submission, and collaboration |

### 9.1 Suggested Solution Structure

The solution contains four projects. The detailed folder structure and dependency rules are in `Architecture.md`.

| Project | Layer | Main Content | EF Core Used? |
|---|---|---|---|
| `TicTacToe.Domain` | Core | `Game` entity, `GameResult` enum, game rules, domain validation | No |
| `TicTacToe.Application` | Application logic | Service interfaces, `IGameRepository`, services, DTOs | No |
| `TicTacToe.Infrastructure` | Data access | `AppDbContext`, entity configuration, `GameRepository`, migrations | **Yes** |
| `TicTacToe.Web` | Presentation | Controllers, Views, ViewModels, `wwwroot` (CSS, JavaScript), `Program.cs`, configuration | No |

```text
TicTacToe/
├── TicTacToe.sln
├── docs/
└── src/
    ├── TicTacToe.Domain/           Entities, Enums, Common
    ├── TicTacToe.Application/      Interfaces, Services, DTOs
    ├── TicTacToe.Infrastructure/   Persistence (with Migrations), Configurations, Repositories
    └── TicTacToe.Web/              Controllers, Models, ViewModels, Views, wwwroot (css, js)
```

---

## 10. Application Architecture

### 10.1 Architecture Overview

The application is built as **Simplified Clean Architecture** using four projects in one solution. The browser displays pages and runs the game in JavaScript. The **Web** project (ASP.NET Core MVC) receives requests and calls the **Application** layer. The Application layer uses the business rules in the **Domain** and reaches the database through an interface. The **Infrastructure** project implements that interface using Entity Framework Core and Microsoft SQL Server.

Controllers never access Entity Framework Core directly. This keeps business rules independent of the web and database technologies.

### 10.2 Architecture Diagram

```mermaid
flowchart TD
    U["User - two players on one device"]
    F["Frontend - HTML5, CSS3, Bootstrap 5, JavaScript"]
    W["TicTacToe.Web - ASP.NET Core MVC: Controllers, Views, ViewModels"]
    A["TicTacToe.Application - services, interfaces, DTOs"]
    D["TicTacToe.Domain - entities, enums, business rules"]
    I["TicTacToe.Infrastructure - Entity Framework Core, DbContext, repository"]
    S[("Microsoft SQL Server")]

    U --> F
    F --> W
    W --> A
    A --> D
    A -->|"through IGameRepository"| I
    I --> D
    I --> S
```

*The diagram shows how a request flows through the system. The Application layer calls the database code only through the `IGameRepository` interface, which the Infrastructure project implements. For this reason, in the source code the Infrastructure project depends on the Application project, not the other way around (see Section 10.5).*

### 10.3 Layer Responsibilities

| Layer / Project | Responsibility |
|---|---|
| **User** | Interacts with the application through a web browser |
| **Frontend** (in `TicTacToe.Web` as Views and `wwwroot`) | Displays pages and runs the interactive game in the browser |
| **`TicTacToe.Web`** (ASP.NET Core MVC) | Receives requests, calls Application services, builds ViewModels, renders Views, and configures dependency injection |
| **`TicTacToe.Application`** | Validates names, processes a submitted game result, returns the history and the scoreboard; defines the interfaces it needs |
| **`TicTacToe.Domain`** | Defines the `Game` entity and the business rules (winning lines, draw, name rules, result and winner consistency) |
| **`TicTacToe.Infrastructure`** | Implements the repository interface using Entity Framework Core; holds the database context, configuration, and migrations |
| **Microsoft SQL Server** | Stores game records permanently |

### 10.4 How JavaScript Handles Interactive Game Functionality

| Function | JavaScript Behavior |
|---|---|
| Cell selection | Detects a click on a cell and places the current player's symbol if the cell is empty |
| Turn management | Starts with Player X, alternates after each valid move, and shows the current player |
| Winner detection | After each move, checks the eight possible winning lines |
| Draw detection | Declares a draw when all nine cells are filled with no winner |
| Result display | Shows the winner or draw message and highlights the winning cells |
| Board locking | Blocks further moves once the game has ended |
| Restart | Clears the board, resets the turn to Player X, and keeps the same player names |
| Saving results | When a game ends, sends the names, the result, and the final board to a Controller action so the server can check and store the result |

JavaScript works only inside the browser during play. The server is contacted only to open pages, to validate the player names, and to store or retrieve results.

### 10.4.1 Division of Responsibilities

| Concern | Browser (JavaScript) | Web (MVC) | Application / Domain | Infrastructure |
|---|---|---|---|---|
| Real-time game play | Yes | No | No | No |
| Page rendering | No | Yes | No | No |
| Business rules (authoritative) | Copy for user experience only | No | Yes (Domain) | No |
| Server-side validation of names and results | No | Passes data on | Yes | No |
| Scoreboard and history calculation | No | Displays the result | Yes | Provides the data |
| Database access | No | No | Through an interface only | Yes (Entity Framework Core) |

### 10.5 Project Dependency Rules

| Rule | Description |
|---|---|
| 1 | `TicTacToe.Domain` does not depend on any other project |
| 2 | `TicTacToe.Application` depends on `TicTacToe.Domain` only |
| 3 | `TicTacToe.Infrastructure` depends on `TicTacToe.Application` and `TicTacToe.Domain` |
| 4 | `TicTacToe.Web` depends on `TicTacToe.Application`, and also references `TicTacToe.Infrastructure` only in `Program.cs` to register services (composition root) |

```mermaid
flowchart LR
    Web["TicTacToe.Web"]
    Infra["TicTacToe.Infrastructure"]
    App["TicTacToe.Application"]
    Dom["TicTacToe.Domain"]

    Web --> App
    Web -. composition root only .-> Infra
    Infra --> App
    Infra --> Dom
    App --> Dom
```

Dependencies point inward, toward the Domain, because the business rules are the most stable part of the application and must not be affected by changes to the web pages or the database.

---

## 11. Frontend and Backend Integration

The frontend and backend live in the same ASP.NET Core MVC web application (the `TicTacToe.Web` project). There is no separate frontend application to build, host, or connect.

| Integration Point | Description |
|---|---|
| **Page delivery** | The server renders Views into HTML pages and sends them to the browser |
| **Static files** | CSS, JavaScript, and Bootstrap files are served from the Web project's `wwwroot` folder |
| **Form submission** | The player setup form is sent to a Controller, which passes the names to an Application service for validation and then displays the game page, or the form again with error messages |
| **Saving a result** | When a game ends, JavaScript sends the result and the final board to a Controller action using an asynchronous browser request. The Controller passes the data to the Application layer, which validates and stores it, and the Controller replies with a short success or error message |
| **Scoreboard and history** | These pages are requested like any other page; the Controller calls an Application service and the View displays the result |
| **Shared layout** | One layout provides the navigation bar and common page structure |

---

## 12. Database Integration

| Aspect | Description |
|---|---|
| **Connection** | The connection string is stored in the Web project's server configuration, not in browser files. It is passed to the Infrastructure project at startup |
| **Database context** | A single database context class in the Infrastructure project manages access to the stored games |
| **Abstraction** | The Application layer defines `IGameRepository`; the Infrastructure project implements it. No other project uses Entity Framework Core |
| **Saving** | A Controller passes a request DTO to the game service. The service validates it using Domain rules, creates a `Game` entity, and asks the repository to store it through Entity Framework Core |
| **History** | The game service asks the repository for all games, newest first, and returns them as DTOs. The Controller builds a ViewModel for the History View |
| **Scoreboard** | The scoreboard service asks the repository for the number of wins per player name and returns the totals as DTOs. The Controller builds a ViewModel for the Scoreboard View |
| **Schema changes** | Handled through Entity Framework Core migrations stored in the Infrastructure project |
| **Verification** | The developer uses SSMS to confirm that records are stored correctly |

---

## 13. Request and Response Workflow

### 13.1 Opening a Page (for example, Game History)

```mermaid
sequenceDiagram
    actor Player
    participant Browser
    participant Web as Web (MVC Controller)
    participant App as Application service
    participant Repo as GameRepository (Infrastructure)
    participant EF as Entity Framework Core
    participant DB as SQL Server

    Player->>Browser: Selects "Game History"
    Browser->>Web: HTTP request for the History page
    Web->>Web: Routing selects the Controller action
    Web->>App: Request the game history
    App->>Repo: Get all games, newest first (through IGameRepository)
    Repo->>EF: Query games
    EF->>DB: Run database query
    DB-->>EF: Return game records
    EF-->>Repo: Return Game entities
    Repo-->>App: Game entities
    App-->>Web: History DTOs
    Web->>Web: Build ViewModel and render View
    Web-->>Browser: HTML page
    Browser-->>Player: History table is displayed
```

### 13.2 Saving a Completed Game

```mermaid
sequenceDiagram
    actor Player
    participant JS as JavaScript (Browser)
    participant Web as Web (MVC Controller)
    participant App as Application service
    participant Repo as GameRepository (Infrastructure)
    participant EF as Entity Framework Core
    participant DB as SQL Server

    Player->>JS: Makes the final move
    JS->>JS: Detects win or draw and shows result
    JS->>Web: Sends names, result, and final board
    Web->>App: Save completed game (request DTO)
    App->>App: Validate using Domain rules
    alt Submission is invalid
        App-->>Web: Error response
        Web-->>JS: Short error message
        JS-->>Player: Shows save error, result stays visible
    else Submission is valid
        App->>Repo: Add game (through IGameRepository)
        Repo->>EF: Add the Game entity
        EF->>DB: Insert record
        DB-->>EF: Confirm
        EF-->>Repo: Confirm
        Repo-->>App: Done
        App-->>Web: Success response
        Web-->>JS: Short success message
    end
```

### 13.3 Workflow Summary

| Step | Action | Technology / Project |
|---|---|---|
| 1 | User opens a page or submits a form | Browser (HTML, Bootstrap) |
| 2 | Request is routed to a Controller action | ASP.NET Core MVC (Web) |
| 3 | Controller calls an Application service | C# (Web to Application) |
| 4 | Input is validated using business rules | C# (Application and Domain) |
| 5 | Data is read or written through the repository interface | C# (Application to Infrastructure) |
| 6 | Entity Framework Core runs the database commands | Entity Framework Core and Microsoft SQL Server (Infrastructure) |
| 7 | Result returns to the Controller as DTOs | C# (Application to Web) |
| 8 | Controller builds a ViewModel and View creates the HTML page | ASP.NET Core MVC Views (Web) |
| 9 | Page is displayed and game logic runs | Browser and JavaScript |

---

## 14. Technology Integration Workflow

This section shows how the technologies and projects are used together in the order a user experiences them.

| Stage | User Experience | Technologies and Projects Involved |
|---|---|---|
| 1. Home | The Home page opens with links to Start Game, Scoreboard, and Game History | Web (Controller and View), HTML5, CSS3, Bootstrap 5 |
| 2. Setup | Players enter their names; invalid names show errors | HTML5 form, Bootstrap 5, Web (Controller), Application and Domain (validation) |
| 3. Play | Players take turns on the 3×3 board | HTML5, CSS3, Vanilla JavaScript |
| 4. Result | The winner or draw is shown automatically | Vanilla JavaScript |
| 5. Save | The completed game is checked and stored | JavaScript, Web, Application, Domain, Infrastructure (Entity Framework Core), SQL Server |
| 6. Scoreboard | Win totals are displayed | Web, Application, Infrastructure (Entity Framework Core), SQL Server, Bootstrap 5 table |
| 7. History | Previous matches are listed, newest first | Web, Application, Infrastructure (Entity Framework Core), SQL Server, Bootstrap 5 table |
| 8. Restart | The board resets; names are kept | Vanilla JavaScript |

---

## 15. Security Considerations

The application has no login and handles very little data, so security measures are kept simple and focused.

| Area | Consideration | Approach |
|---|---|---|
| **Input validation** | Player names and game results must be valid | Validate on the server in the Domain and Application layers in addition to the browser; reject empty, over-length, or invalid data |
| **Layer boundaries** | Business rules must not be bypassed | Controllers call Application services only; database access exists only in the Infrastructure project |
| **SQL injection** | Malicious text must not alter database commands | Use Entity Framework Core, which sends data to the database safely |
| **Cross-site scripting (XSS)** | Entered names must not run as script | Rely on the automatic output encoding of MVC Views; do not insert raw user text into pages |
| **Cross-site request forgery (CSRF)** | Forms must not be submitted by other websites | Use the built-in anti-forgery protection of ASP.NET Core MVC for form submissions |
| **Connection details** | Database credentials must stay private | Store the connection string in server configuration, never in browser files or public repositories |
| **Data minimization** | Collect only what is needed | Store only player names, results, and timestamps |
| **Error handling** | Errors must not reveal internal details | Show friendly messages to users; keep technical details on the server |
| **Transport security** | Data should be protected in transit | Use HTTPS when the application is hosted |

---

## 16. Development Environment Requirements

### 16.1 Software Requirements

| Requirement | Details |
|---|---|
| Operating system | Windows 10 or 11 (recommended for Visual Studio 2022 and SSMS) |
| .NET SDK | A current supported version of the .NET SDK compatible with Visual Studio 2022 |
| IDE | Visual Studio 2022 with the **ASP.NET and web development** workload |
| Editor | Visual Studio Code |
| Database | Microsoft SQL Server (Express or Developer edition is sufficient) or SQL Server LocalDB |
| Database tool | SQL Server Management Studio (SSMS) |
| Version control | Git and a GitHub account |
| Browser | Latest version of Chrome, Edge, Firefox, or Safari |

### 16.2 Hardware Recommendations

| Component | Recommendation |
|---|---|
| Processor | Modern dual-core processor or better |
| Memory | At least 8 GB RAM |
| Storage | At least 20 GB of free disk space |

### 16.3 Setup Overview

| Step | Activity |
|---|---|
| 1 | Install Visual Studio 2022 with the web development workload |
| 2 | Install SQL Server and SSMS |
| 3 | Install Git and clone the GitHub repository |
| 4 | Configure the database connection string |
| 5 | Apply Entity Framework Core migrations to create the database |
| 6 | Run the application and test in a browser |

---

## 17. Future Technology Considerations

The following items are **not part of the current scope**. They are possible improvements for later versions and would be considered only if the project requirements change.

| ID | Consideration | Possible Benefit |
|---|---|---|
| FT-001 | Automated testing tools for C# and JavaScript | Higher confidence when making changes |
| FT-002 | Hosting on a cloud platform | Access to the application from a public web address |
| FT-003 | Logging of server errors | Easier troubleshooting |
| FT-004 | Continuous integration through GitHub | Automatic build and test on each change |
| FT-005 | Browser-side local storage for player name suggestions | Faster entry of repeated names |
| FT-006 | Light and dark display themes | Improved visual comfort |
| FT-007 | Unit test projects for the Domain and Application layers | Easier and safer changes to business rules |

Any change to the technology stack should be reviewed against the PRD scope to avoid adding unnecessary complexity.

---

*End of Document*
