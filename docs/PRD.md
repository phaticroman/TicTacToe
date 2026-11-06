# Product Requirements Document (PRD)

# TicTacToe Online

---

## 1. Document Information

| Field | Details |
|---|---|
| Project Name | TicTacToe Online |
| Project Type | Web-based Multiplayer Game |
| Document Version | 1.0 |
| Document Status | Draft |
| Date | November 4, 2026 |
| Prepared By | Business Analyst / Product Manager |
| Intended Audience | Developers, project supervisors, testers |

---

## 2. Project Overview

TicTacToe Online is a simple browser-based multiplayer Tic Tac Toe game. Two players can play against each other online without registration or login.

One player creates a game room and receives a unique room code. The second player joins the room using that code. Both players then play in real time, and every move appears instantly on both screens.

---

## 3. Problem Statement

Many online games require sign-up, downloads, or complex setup before a player can start. Friends who only want a quick game of Tic Tac Toe often have no simple way to play together remotely.

TicTacToe Online solves this by offering a no-account, no-install game that starts with a single room code.

---

## 4. Project Goals and Objectives

| ID | Goal | Objective |
|---|---|---|
| G-01 | Online two-player gameplay | Allow two players to play a complete Tic Tac Toe game over the internet or a local network. |
| G-02 | Simple room handling | Let a player create or join a room in just a few steps. |
| G-03 | Real-time experience | Show each move on both players' screens without refreshing the page. |
| G-04 | Fair play | Make sure all moves follow the game rules, checked by the server. |
| G-05 | Easy to use | Provide a clean, simple interface that works on desktop and mobile. |

---

## 5. Target Users

| User Group | Description | Key Need |
|---|---|---|
| Casual players | People who want a quick online game. | Start playing in seconds. |
| Friends | People who want to play together using a shared code. | Simple way to meet in the same game. |
| No-account users | People who do not want to register or log in. | Play without giving personal information. |

---

## 6. Functional Requirements

| ID | Requirement | Description | Priority |
|---|---|---|---|
| FR-01 | Guest Player | Players can enter a display name without creating an account. | High |
| FR-02 | Create Room | A player can create a new game room. | High |
| FR-03 | Room Code Generation | The system generates a unique room code when a room is created. The code must be short and easy to share. | High |
| FR-04 | Join Room | A second player can join an existing room using its room code. | High |
| FR-05 | Two-Player Limit | Each room supports a maximum of two active players. | High |
| FR-06 | Symbol Assignment | The system automatically assigns X and O to the two players. X always moves first. | High |
| FR-07 | Turn Management | Players must take turns making moves. | High |
| FR-08 | Real-time Synchronization | Moves are synchronized between both players using SignalR, without a page refresh. | High |
| FR-09 | Move Validation | The server validates each move and rejects invalid moves, occupied cells, and out-of-turn moves. | High |
| FR-10 | Winner Detection | The system detects when a player wins (three matching symbols in a row, column, or diagonal). | High |
| FR-11 | Draw Detection | The system detects when all cells are occupied and there is no winner. | High |
| FR-12 | Restart Game | After a game finishes, either player can request a rematch. The new game starts when both players agree. | Medium |
| FR-13 | Room Match History | The room shows each finished match with its round number, winner or draw, and finish time. History is kept across rematches. | Medium |
| FR-14 | Room Scorecard | The room shows how many matches have been played, how many ended in a draw, and how many wins each player has. The scorecard updates after every finished match and is kept across rematches. | Medium |

---

## 7. Non-Functional Requirements

| ID | Category | Requirement |
|---|---|---|
| NFR-01 | Performance | Game updates are delivered to both players in near real time. |
| NFR-02 | Security | All game operations are validated on the server. The browser is never trusted to enforce the rules. |
| NFR-03 | Reliability | If a player's connection is interrupted, the game state is kept, the opponent is notified, and the player can reconnect to the same room. |
| NFR-04 | Usability | The interface is simple and intuitive. A new player can start a game without instructions. |
| NFR-05 | Responsiveness | The application works on modern desktop and mobile browsers. |
| NFR-06 | Maintainability | The solution follows Clean Architecture, and business rules stay in the Domain and Application layers. |
| NFR-07 | Compatibility | The application runs in local development and in Docker deployment. |

---

## 8. User Flow

| Step | Actor | Action |
|---|---|---|
| 1 | Player 1 | Opens the website. |
| 2 | Player 1 | Enters a display name. |
| 3 | Player 1 | Selects **Create Room**. |
| 4 | System | Generates a unique room code. |
| 5 | Player 1 | Shares the room code with a friend. |
| 6 | Player 2 | Opens the website, enters a display name, and enters the room code. |
| 7 | System | Validates the room and allows Player 2 to join. |
| 8 | Both players | Enter the game screen. |
| 9 | System | Assigns X and O. |
| 10 | Both players | Take turns making moves. |
| 11 | System | Synchronizes game updates between both browsers using SignalR. |
| 12 | System | Detects a winner or a draw. |
| 13 | Both players | Can request a rematch. |

---

## 9. Project Scope

### In Scope (Version 1.0)

| Feature |
|---|
| Guest player mode |
| Create game room |
| Join game room using room code |
| Two-player multiplayer |
| Automatic X/O assignment |
| Turn management |
| Real-time game synchronization |
| Winner and draw detection |
| Restart / rematch functionality |
| Match history within a room |
| Room scorecard with matches played, draws, and wins per player |
| Basic responsive UI |

### Out of Scope

| Feature |
|---|
| User registration and login |
| AI opponent |
| Chat system |
| Leaderboard |
| Move-by-move replay |
| Spectator mode |
| Tournament system |
| Public matchmaking |
| Payment system |
| Player ranking system |

---

## 10. Acceptance Criteria

| ID | Related Requirement | Scenario | Expected Result |
|---|---|---|---|
| AC-01 | FR-01 | A player enters a display name and continues. | The player proceeds without any account or login. |
| AC-02 | FR-02 | A player selects Create Room. | A new room is created and the player waits in it. |
| AC-03 | FR-03 | Two rooms are created. | Each room has a different room code. |
| AC-04 | FR-04 | A second player enters a correct room code. | The player joins the room and both players enter the game screen. |
| AC-05 | FR-04 | A player enters an invalid or unknown room code. | The join is rejected and a clear error message is shown. |
| AC-06 | FR-05 | A third player tries to join a room that already has two players. | The join is rejected and a "room is full" message is shown. |
| AC-07 | FR-06 | Both players are in the game. | Each player receives a different symbol (X or O). |
| AC-08 | FR-07, FR-09 | A player tries to move when it is not their turn. | The move is rejected and the board does not change. |
| AC-09 | FR-09 | A player selects a cell that is already occupied. | The move is rejected and the existing symbol is not changed. |
| AC-10 | FR-08 | One player makes a valid move. | The move appears on both browsers without a page refresh, and both boards match. |
| AC-11 | FR-10 | A player completes three matching symbols in a row, column, or diagonal. | The server declares that player the winner and both players see the result. |
| AC-12 | FR-11 | All nine cells are filled and no player has won. | The server declares a draw and both players see the result. |
| AC-13 | FR-10, FR-11 | A player tries to move after the game has finished. | The move is rejected and the board does not change. |
| AC-14 | FR-12 | After a game ends, a player requests a rematch and the opponent agrees. | The board is cleared and a new game starts in the same room. |
| AC-15 | NFR-03 | A player's connection drops and returns. | The player is returned to the same room with the current board state, and the opponent was notified of the interruption. |
| AC-16 | FR-13 | A game ends and then a rematch is played. | The room shows the finished round with its winner or draw, and keeps it after the rematch starts. |
| AC-17 | FR-14 | Games are played and some end in a draw. | The scorecard shows the total matches played, the number of draws, and the win count for each player, including across rematches. |

---

## 11. Assumptions and Constraints

### Assumptions

| ID | Assumption |
|---|---|
| A-01 | Only two players can play in one room. |
| A-02 | No user registration or authentication is required. |
| A-03 | Players identify themselves using guest display names. |
| A-04 | Version 1.0 supports one game type only: standard 3x3 Tic Tac Toe. |
| A-05 | The application can run locally using localhost. |
| A-06 | Two computers on the same local network can access the application when it is correctly configured. |
| A-07 | Internet multiplayer is possible after deployment or appropriate network configuration. |
| A-08 | The server is responsible for validating game moves. |
| A-09 | In a rematch, players keep their symbols and X moves first. |

### Constraints

| Category | Technology / Decision |
|---|---|
| Language and Framework | C#, ASP.NET Core MVC (.NET 10) |
| Architecture | Clean Architecture with four projects: Domain, Application, Infrastructure, Web |
| Application Logic | Cortex.Mediator for Commands, Queries, and Handlers |
| Real-time Communication | ASP.NET Core SignalR |
| Frontend | HTML5, CSS3, JavaScript, Bootstrap 5 |
| Data | Microsoft SQL Server with Entity Framework Core |
| Dependency Injection | Built-in .NET DI |
| Deployment | Docker and Docker Compose |
| Other | No separate Web API project |
