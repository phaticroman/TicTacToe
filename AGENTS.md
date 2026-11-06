# AGENTS.md

## Approved decisions vs docs

- Product and data rules: `docs/PRD.md`, `docs/DatabaseDesign.md`. Flow/technical intent: `docs/Architecture.md`.
- `docs/TechStack.md` and `docs/Architecture.md` describe a four-project Clean Architecture with Cortex.Mediator. The project owner approved a **single ASP.NET Core MVC project without a mediator**. Follow this file for structure; follow the docs for product and data rules.
- `TechnicalDesign.md` does not exist; `docs/Architecture.md` is the technical design.

## Repository state

- .NET 10 solution `TicTacToe.slnx` with `src/TicTacToe.Web` (MVC app) and `tests/TicTacToe.Tests` (xUnit, references the web project).
- `Data/AppDbContext.cs`, `Entities/`, and `Enums/` are schema setup. `Game/GameRules.cs` holds the pure move/win/draw rules. `Services/RoomService.cs` handles create/join/get-room persistence. `Services/GameService.cs` validates and persists moves.
- MVC pages support creating/joining a room and viewing it. `Hubs/GameHub.cs` provides `JoinRoom`, `MakeMove`, and `RequestRematch`; `wwwroot/js/game.js` applies live `RoomState` updates through the vendored SignalR browser client.
- Presence is tracked by `Services/PlayerConnectionTracker` (multi-connection per player), persisted through `GamePlayer.IsConnected`, and re-synced on join/reconnect. Rematch requests persist in `WantsRematch` until both players agree. The hub broadcasts board/status state and sends move/rematch rejections to the caller.
- Tests: `GameRulesTests` are pure; `RoomServiceTests`, `GameServiceTests`, `PlayerConnectionTrackerTests`, controller tests, and cookie tests use EF Core InMemory/`DefaultHttpContext` and do not need SQL Server.

## Commands (run from repo root)

- Build: `dotnet build TicTacToe.slnx`
- Test: `dotnet test TicTacToe.slnx`
- Run: `dotnet run --project src/TicTacToe.Web` (`http://localhost:5262`, HTTPS `https://localhost:7019`)
- Add migration: `dotnet ef migrations add <Name> --project src/TicTacToe.Web --startup-project src/TicTacToe.Web`
- Apply migration: `dotnet ef database update --project src/TicTacToe.Web --startup-project src/TicTacToe.Web`
- Local DB: SQL Server Express `.\SQLEXPRESS`, database `TicTacToe`, Windows auth. The `MSSQL$SQLEXPRESS` service must be running.

## Stack and layout

- ASP.NET Core MVC on .NET 10; no Web API, no Clean Architecture, no CQRS or mediator.
- SignalR at `/gamehub` for realtime; built-in DI; SQL Server + EF Core code-first; Bootstrap 5 + plain JS.
- Single project folders: `Controllers/`, `Data/`, `Entities/`, `Enums/`, `Game/` (pure rules), `Hubs/`, `Models/` (view models), `Services/` (add as needed), `Views/`, `wwwroot/`.
- Keep game logic out of controllers and hubs; use plain rule classes and a server-side service layer.
- Out of scope: auth/login, AI, chat, match history, leaderboards, Redis, microservices, extra dependencies.

## Web flow and guest identity

- Routes: `GET /` (home), `POST /Home/Create`, `POST /Home/Join`, `GET /Game?code=...`. Forms use anti-forgery tokens.
- Guest identity uses the HttpOnly cookie `tictactoe.guest` with value `{roomCode}:{playerId}` (12-hour expiry, SameSite Lax, Secure when HTTPS). Helpers: `Services/GuestIdentityExtensions.cs`.
- `GameController.Index` requires the cookie's room code to match `code` (when provided) and the player ID to belong to that room; otherwise it clears the cookie and redirects home.
- View models live in `Models/`; `GameViewModel` derives the status text and CSS class. The UI uses the graph-paper game-sheet style in `wwwroot/css/site.css`.

## Server authority and data invariants

- Clients send only the chosen cell, never board state. Validate identity, membership, turn, and cell server-side.
- Guest identity is `GamePlayers.Id`; do not persist SignalR connection IDs.
- `GameHub.JoinRoom` validates the guest cookie, stores room/player in `Context.Items`, then joins group `room:{CODE}`. `MakeMove` reuses that state; `RoomState` goes to the group and `MoveRejected` goes to the caller.
- Do not add `CancellationToken` parameters to hub methods: SignalR counts them as client arguments. Use `Context.ConnectionAborted` inside the method instead.
- SignalR serializes enums as strings (`JsonStringEnumConverter`); browser comparisons use `"X"`, `"O"`, `"InProgress"`, etc.
- `GameRooms.Board` is 9 characters, row-major (`-` empty, `X`/`O` occupied).
- `Status` ∈ `WaitingForPlayer`, `InProgress`, `Won`, `Draw`; X always moves first.
- Unique `RoomCode`; unique `(RoomId, Symbol)`; players cascade-delete with the room.
- `RowVersion` is for optimistic concurrency; `GameService` rejects conflicting updates with a retry message.
- SignalR browser asset is `wwwroot/lib/signalr/signalr.min.js` (npm `@microsoft/signalr` 10.0.11, the latest browser build); server packages use the installed .NET 10 shared framework.
- Entity `Id` values are EF-generated on insert. Do not assign Guids in entity constructors; doing so makes newly added players look `Modified` instead of `Added`.
- Rematch reuses the same room/players and only resets board, status, turn, winner, and `WantsRematch`.

## UI work

- Use the `frontend-design` skill at `.agents/skills/frontend-design/SKILL.md` before building or reshaping the Razor/JS UI.
