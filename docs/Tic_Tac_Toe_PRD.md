# Product Requirements Document (PRD)
## Tic-Tac-Toe Web Application

*Prepared by: Business Analyst / Product Manager*

---

## 1. Document Information

| Field | Details |
|---|---|
| **Document Title** | Product Requirements Document (PRD) |
| **Project Name** | Tic-Tac-Toe Web Application |
| **Version** | 1.0 |
| **Date** | November 4, 2026 |
| **Document Status** | Draft – Submitted for Instructor Review |
| **Project Type** | Full-Stack Web Application (Course Assignment) |
| **Next Documents** | Software Requirements Specification (SRS), Use Case Diagram, Database Design, UI/UX Design |

---

## 2. Executive Summary

The Tic-Tac-Toe Web Application is a simple full-stack web application that allows two people to play Tic-Tac-Toe against each other on the same device. Players enter their names, take turns marking a 3×3 board, and the system automatically detects a win or a draw and shows the result.

Completed games are verified by the server and then saved in a Microsoft SQL Server database by a single ASP.NET Core MVC application, which also serves the pages the players see. The saved data powers a **Scoreboard** (total wins per player) and a **Game History** page (list of previous matches).

The project is intentionally small and beginner-friendly. It demonstrates core full-stack skills: a responsive user interface, a single ASP.NET Core MVC application, server-side validation, database persistence, and clear requirements documentation. It does not include online multiplayer, chat, payments, complex authentication, or computer-controlled opponents.

---

## 3. Project Overview

| Item | Description |
|---|---|
| **Project Name** | Tic-Tac-Toe Web Application |
| **Project Type** | Full-Stack Web Application |
| **Purpose** | Provide a web-based Tic-Tac-Toe game for two players on the same device |
| **Frontend (Views)** | Razor Views inside the MVC application, using HTML5, CSS3, Bootstrap 5, and JavaScript |
| **Backend** | ASP.NET Core MVC (single application; no separate frontend project) |
| **Database** | Microsoft SQL Server |
| **ORM** | Entity Framework Core |

### 3.1 Core Features Summary

| # | Feature |
|---|---|
| 1 | Home Page |
| 2 | Two-player game (Player X vs Player O) |
| 3 | Player name input |
| 4 | Interactive 3×3 game board |
| 5 | Automatic winner detection |
| 6 | Draw detection |
| 7 | Display of game results |
| 8 | Restart game functionality |
| 9 | Scoreboard tracking each player's wins |
| 10 | Game history of previous matches |

---

## 4. Problem Statement

Two people who want to play Tic-Tac-Toe on a single device currently rely on paper or on generic apps that do not remember who won. Players must check the board manually for a winner or a draw, and there is no record of past matches or running win counts.

This project solves these problems by providing a simple web application that:

- enforces the game rules and turn order automatically;
- detects wins and draws without manual checking;
- records results permanently; and
- displays win totals and past matches on demand.

---

## 5. Project Goals and Objectives

### 5.1 Goal

Deliver a working, easy-to-use, two-player Tic-Tac-Toe web application with persistent score and history tracking, suitable as a full-stack course assignment.

### 5.2 Objectives

| ID | Objective | Measure |
|---|---|---|
| OBJ-001 | Allow two players to play a complete game on one device | A game can be played from start to finish without errors |
| OBJ-002 | Automatically determine the game outcome | 100% of wins and draws are detected correctly |
| OBJ-003 | Store every completed game in the database | Each finished game appears in Game History |
| OBJ-004 | Show cumulative wins per player | Scoreboard totals match the saved game records |
| OBJ-005 | Demonstrate the full-stack architecture | Views, Controllers, Models, EF Core, and SQL Server work together in one ASP.NET Core MVC application |
| OBJ-006 | Produce documentation that guides later project stages | PRD is sufficient to begin the SRS, use cases, database design, and UI/UX design |
| OBJ-007 | Save only valid, non-duplicated results | The server rejects every result that does not match the final board state, and one game never creates two records |

---

## 6. Target Users and User Personas

### 6.1 Target Users

| User Type | Description |
|---|---|
| **Casual Player** | Anyone who wants a quick two-player game on one device |
| **Course Instructor / Evaluator** | Reviews the application against this PRD for grading |

There is only one functional role in the system: the **Player**. No login, administrator, or other roles exist.

### 6.2 User Personas

| | **Persona 1: Sara (Casual Player)** | **Persona 2: Omar (Friendly Competitor)** |
|---|---|---|
| **Age / Occupation** | 20, university student | 24, office employee |
| **Technical Skill** | Basic | Moderate |
| **Goal** | Play a quick game with a friend during a break | Keep track of who wins more often against a colleague |
| **Needs** | Simple screens, clear turn indicator, no setup | Persistent scoreboard and a list of past games |
| **Frustrations** | Confusing menus, unclear whose turn it is | Losing track of results between sessions |

---

## 7. Project Scope

### 7.1 In Scope

| ID | Item |
|---|---|
| SC-001 | Home page with navigation to the game, scoreboard, and history |
| SC-002 | Two-player game on the same device (Player X vs Player O) |
| SC-003 | Player name entry before each game |
| SC-004 | Interactive 3×3 game board |
| SC-005 | Automatic winner and draw detection |
| SC-006 | Display of the game result |
| SC-007 | Restart game functionality |
| SC-008 | Scoreboard showing wins per player |
| SC-009 | Game history showing previous matches |
| SC-010 | Saving completed games in SQL Server through an ASP.NET Core MVC controller using Entity Framework Core, after the server has re-verified the submitted result against the final board state |
| SC-011 | Responsive layout using Bootstrap 5 |
| SC-012 | Prevention of duplicate saves (one save request per completed game from the browser) |

### 7.2 Out of Scope

| ID | Item |
|---|---|
| OS-001 | Online or remote multiplayer |
| OS-002 | Chat or messaging between players |
| OS-003 | Payment or subscription features |
| OS-004 | User registration, login, or complex authentication |
| OS-005 | Computer-controlled (AI) opponents |
| OS-006 | Single-player mode |
| OS-007 | Board sizes other than 3×3 |
| OS-008 | Administrator features or user management |
| OS-009 | Native mobile apps |
| OS-010 | Deleting or editing saved game records |

---

## 8. Functional Requirements

### 8.1 Home Page

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-001 | Home Page display | The system shall display a Home Page as the first page of the application, showing the application name and a short description of the game. |
| FR-002 | Home Page navigation | The Home Page shall provide clearly labeled links or buttons to **Start Game**, **Scoreboard**, and **Game History**. Selecting each one opens the corresponding page. |
| FR-003 | Site-wide navigation | Every page shall include a navigation bar with links to Home, Scoreboard, and Game History. |

### 8.2 Player Setup

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-004 | Player name input | Before a game starts, the system shall provide one input field for the **Player X** name and one for the **Player O** name. |
| FR-005 | Name validation – required | The system shall not start a game if either name is empty or contains only spaces. It shall display a clear error message next to the field. |
| FR-006 | Name validation – length | The system shall limit each player name to a maximum of 20 characters. |
| FR-007 | Name validation – distinct names | The system shall not start a game if both players enter the same name (comparison is not case-sensitive). It shall display an error message. |
| FR-008 | Start game | When both names are valid, the system shall start a new game, display the board, and show the two player names with their symbols (X and O). |

### 8.3 Game Board and Gameplay

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-009 | Game board display | The system shall display an interactive 3×3 board of nine cells, all empty at the start of a game. |
| FR-010 | Player X starts | Player X shall always take the first turn of every game. |
| FR-011 | Placing a mark | When a player selects an empty cell during their turn, the system shall place that player's symbol (X or O) in the cell. |
| FR-012 | Turn alternation | After a valid move, the system shall switch the turn to the other player. |
| FR-013 | Current turn indicator | The system shall display the name and symbol of the player whose turn it is. |
| FR-014 | Occupied cell protection | If a player selects a cell that already contains a symbol, the system shall ignore the selection and shall not change the turn. |
| FR-015 | Input lock after game end | After a win or a draw, the system shall prevent any further moves on that board. |

### 8.4 Result Detection and Display

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-016 | Automatic winner detection | After every move, the system shall check whether the player who just moved has three matching symbols in a row, column, or diagonal. If so, the game ends with that player as the winner. |
| FR-017 | Winning line highlight | When a game is won, the system shall visually highlight the three winning cells. |
| FR-018 | Automatic draw detection | If all nine cells are filled and no player has won, the system shall end the game as a draw. |
| FR-019 | Result display | When a game ends, the system shall display the result: either "[Player Name] wins!" or "It's a draw!". |

### 8.5 Restart Game

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-020 | Restart button | The game page shall provide a **Restart Game** button, available during a game and after a game has ended. |
| FR-021 | Restart behavior | When Restart is selected, the system shall clear the board, set the turn to Player X, hide any displayed result, and keep the same player names. |
| FR-022 | Unfinished game on restart | A game that is restarted before it ends (no winner and no draw) shall not be saved to the database and shall not affect the scoreboard. |
| FR-023 | Change players | The system shall allow players to return to the player setup screen to enter different names (for example, by navigating to the Start Game page again). |

### 8.6 Saving Game Results (Backend)

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-024 | Save completed game | When a game ends in a win or a draw, the system shall submit the result and the final board state to an ASP.NET Core MVC controller action, which shall verify it (FR-040) and then store it in the SQL Server database through Entity Framework Core. |
| FR-025 | Saved game data | Each saved game shall record: a unique game ID, the Player X name, the Player O name, the result (X win, O win, or draw), the winner's name (empty for a draw), and the date and time the game ended. |
| FR-026 | Save failure message | If saving fails, the system shall show an error message to the players. The game result shall still be displayed on screen. |

### 8.7 Scoreboard

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-027 | Scoreboard page | The system shall provide a Scoreboard page listing each player who has won at least one game together with their total number of wins. |
| FR-028 | Scoreboard calculation | Wins shall be counted per player name from the saved games. Draws shall not count as wins. |
| FR-029 | Scoreboard ordering | The scoreboard shall list players from the highest number of wins to the lowest. |
| FR-030 | Scoreboard update | The scoreboard shall include a game's result as soon as that game has been saved. |
| FR-031 | Empty scoreboard | If no wins have been recorded, the Scoreboard page shall display a message such as "No games have been won yet." |

### 8.8 Game History

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-032 | Game History page | The system shall provide a Game History page listing all previously saved matches. |
| FR-033 | History columns | Each history entry shall show: Player X name, Player O name, result (winner name or "Draw"), and the date and time played. |
| FR-034 | History ordering | The history shall list the most recent game first. |
| FR-035 | Empty history | If no games have been saved, the page shall display a message such as "No games have been played yet." |

### 8.9 Controller Actions and Server-Side Validation

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-036 | Save-game action | The MVC application shall provide a controller action that accepts a completed game's data (player names, claimed result, and the final board state) and, once it has passed the checks in FR-039 and FR-040, stores it. The final board state is used only for verification and is not stored (see FR-025). |
| FR-037 | Get-history action | The MVC application shall provide a controller action that retrieves all saved games, most recent first, and displays them in the Game History view. |
| FR-038 | Get-scoreboard action | The MVC application shall provide a controller action that calculates each winning player's name and total wins and displays them in the Scoreboard view. |
| FR-039 | Input validation on server | The server shall reject game data that is incomplete or invalid (for example, a missing player name, a name longer than 20 characters, identical player names (not case-sensitive), a board that does not contain exactly nine cells, or an unrecognized result) and return an error response. The server shall not rely on the browser having performed the same checks. |
| FR-040 | Server-side result re-verification | Before saving, the server shall independently re-verify the submitted result against the submitted final board state, following business rules BR-021 to BR-024. If the result does not match the board, the server shall not save the game and shall return an error response, which the browser shall show as a save-failure message (FR-026). |

### 8.10 Duplicate Save Prevention

| ID | Requirement | Expected System Behavior |
|---|---|---|
| FR-041 | Single save per game | The browser shall send the save request only once for each completed game. As soon as the request is sent, the browser shall mark that game as "submitted", disable any save-related control (including a retry control, if one is shown), and ignore any further attempt to save the same game, such as a double-click. A new save request is allowed only for a new game that has been started and completed. |

---

## 9. Non-Functional Requirements

### 9.1 Performance

| ID | Requirement |
|---|---|
| NFR-001 | Placing a mark and updating the board shall appear instantaneous to the user (under 1 second). |
| NFR-002 | The Scoreboard and Game History pages shall load within 3 seconds under normal conditions on a local or course-demo environment. |
| NFR-003 | Saving a completed game shall take no more than 3 seconds under normal conditions. |

### 9.2 Usability

| ID | Requirement |
|---|---|
| NFR-004 | A first-time user shall be able to start and finish a game without instructions. |
| NFR-005 | The interface shall use clear labels, readable text, and consistent layout across all pages. |
| NFR-006 | The layout shall be responsive and usable on desktop, tablet, and phone screens using Bootstrap 5. |
| NFR-007 | Error and validation messages shall be written in plain language and appear next to the related input. |
| NFR-008 | The X and O symbols and the winning cells shall be easily distinguishable (for example, through different colors). |

### 9.3 Reliability

| ID | Requirement |
|---|---|
| NFR-009 | Game rules (turns, win, draw) shall behave identically every time with no incorrect outcomes. |
| NFR-010 | A completed game shall be saved exactly once; one game shall not create duplicate records, including when the player double-clicks or repeats an action (see FR-041). |
| NFR-011 | If the backend or database is unavailable, the application shall display an error message and shall not crash or freeze. |
| NFR-012 | Saved data shall remain in the database after the application or browser is closed. |

### 9.4 Security

| ID | Requirement |
|---|---|
| NFR-013 | The server shall validate all incoming data on the server side, including re-verification of each submitted result against the final board state (FR-039, FR-040). |
| NFR-014 | Database access shall use Entity Framework Core (parameterized queries) to prevent SQL injection. |
| NFR-015 | Player names displayed on pages shall be output safely so that entered text cannot run as script (protection against cross-site scripting). |
| NFR-016 | The database connection details shall not be visible in client-side code (Views and JavaScript). |
| NFR-017 | The application shall collect only player names, game results, and timestamps. No other personal data shall be collected. |

### 9.5 Maintainability

| ID | Requirement |
|---|---|
| NFR-018 | The Model, View, and Controller layers of the MVC application, and the database layer, shall be kept separate. |
| NFR-019 | Code shall use meaningful names and short explanatory comments. |
| NFR-020 | Database structure shall be created and updated through Entity Framework Core. |
| NFR-021 | The project shall be stored in a version control repository with a README describing how to set up and run it. |

### 9.6 Browser Compatibility

| ID | Requirement |
|---|---|
| NFR-022 | The application shall work correctly in the latest versions of Google Chrome, Microsoft Edge, Mozilla Firefox, and Safari. |
| NFR-023 | The application shall require JavaScript to be enabled in the browser. |

---

## 10. User Stories

| ID | User Story | Related Requirements |
|---|---|---|
| US-001 | As a player, I want to see a home page, so that I understand the application and can easily navigate to what I need. | FR-001 to FR-003 |
| US-002 | As a player, I want to enter my name and my opponent's name, so that the game and records show who is playing. | FR-004 to FR-008 |
| US-003 | As a player, I want to see clear errors if I enter invalid names, so that I can correct them quickly. | FR-005 to FR-007 |
| US-004 | As a player, I want to click a cell on a 3×3 board to place my symbol, so that I can play my turn easily. | FR-009, FR-011 |
| US-005 | As a player, I want to see whose turn it is, so that I never play out of turn. | FR-010, FR-012, FR-013 |
| US-006 | As a player, I want the game to ignore clicks on filled cells, so that moves cannot be overwritten by mistake. | FR-014 |
| US-007 | As a player, I want the game to detect a winner automatically, so that I do not need to check the board myself. | FR-016, FR-017 |
| US-008 | As a player, I want the game to detect a draw automatically, so that I know when no one can win. | FR-018 |
| US-009 | As a player, I want to see the result clearly at the end of a game, so that we both know the outcome. | FR-019 |
| US-010 | As a player, I want to restart the game, so that we can play again quickly. | FR-020 to FR-022 |
| US-011 | As a player, I want to play with different names, so that other people can use the same device. | FR-023 |
| US-012 | As a player, I want my completed games saved once and only when the result is valid, so that results are not lost, duplicated, or wrong. | FR-024 to FR-026, FR-039 to FR-041 |
| US-013 | As a player, I want to view a scoreboard of wins, so that we can see who has won the most games. | FR-027 to FR-031 |
| US-014 | As a player, I want to view a history of previous matches, so that I can review past results. | FR-032 to FR-035 |

---

## 11. User Journey / Application Workflow

### 11.1 Main User Journey

| Step | User Action | System Response |
|---|---|---|
| 1 | Opens the application | Home Page is displayed with navigation options |
| 2 | Selects **Start Game** | Player setup screen with two name fields is displayed |
| 3 | Enters both names and confirms | System validates names; if valid, an empty board is shown with Player X's turn |
| 4 | Player X selects a cell | X is placed; turn switches to Player O |
| 5 | Players continue alternating | After each move, system checks for a win or a draw |
| 6 | A line of three is completed, or the board fills | Result is shown (winner or draw); the board is locked |
| 7 | — | Browser submits the result and final board once; the server re-verifies it and saves the game through the MVC controller |
| 8 | Selects **Restart Game** | Board is cleared; Player X starts again with the same names |
| 9 | Selects **Scoreboard** | Wins per player are displayed |
| 10 | Selects **Game History** | Previous matches are displayed, newest first |

### 11.2 Game Flow (Text Workflow)

1. Start → Home Page
2. Start Game → Enter player names
3. Names valid? **No** → show error, return to step 2. **Yes** → show empty board
4. Current player selects a cell
5. Cell empty? **No** → ignore and wait. **Yes** → place symbol
6. Win found? **Yes** → show winner, save game, lock board
7. Board full? **Yes** → show draw, save game, lock board
8. Otherwise → switch turn and return to step 4
9. After the game ends → Restart Game, change players, or view Scoreboard / Game History

### 11.3 Alternative and Exception Flows

| Scenario | System Behavior |
|---|---|
| Invalid or duplicate names | Show an error message and do not start the game |
| Click on an occupied cell | Ignore the click; turn does not change |
| Restart during an unfinished game | Clear the board; do not save the game |
| Save to database fails | Show an error message; the result stays visible on screen |
| Server rejects a result that does not match the board | Do not save the game; show a save-failure message; the result stays visible on screen |
| Player double-clicks or repeats the save action | Only one request is sent; save-related controls are disabled after the first request; only one record is stored |
| No saved games yet | Scoreboard and History pages show an empty-state message |

---

## 12. Business Rules

### 12.1 Game Rules

| ID | Rule |
|---|---|
| BR-001 | The game is played by exactly two players on the same device. |
| BR-002 | One player uses the symbol **X** and the other uses the symbol **O**. |
| BR-003 | The board is a 3×3 grid with nine cells. |
| BR-004 | A cell can hold only one symbol, and a placed symbol cannot be changed or removed during the game. |
| BR-005 | Both players must have a valid, different name before a game starts. |

### 12.2 Winning Conditions

| ID | Rule |
|---|---|
| BR-006 | A player wins by placing three of their symbols in one complete line. |
| BR-007 | Valid lines are: 3 horizontal rows, 3 vertical columns, and 2 diagonals (8 lines in total). |
| BR-008 | The game ends immediately when a player completes a winning line. |

### 12.3 Draw Conditions

| ID | Rule |
|---|---|
| BR-009 | A game is a draw when all nine cells are filled and neither player has a winning line. |
| BR-010 | If the last (ninth) move completes a winning line, the game is a win, not a draw. |

### 12.4 Turn Management

| ID | Rule |
|---|---|
| BR-011 | Player X always plays first in every game, including after a restart. |
| BR-012 | Players alternate turns; each turn consists of exactly one valid move. |
| BR-013 | An invalid move (occupied cell) does not use up the player's turn. |
| BR-014 | No moves are allowed after the game has ended. |

### 12.5 Score Management

| ID | Rule |
|---|---|
| BR-015 | Only completed games (win or draw) are saved. |
| BR-016 | A win adds one point to the winning player's total wins. |
| BR-017 | A draw adds no points to either player. |
| BR-018 | Scores are calculated per player name, not case-sensitive (for example, "Sara" and "sara" are treated as the same player). |
| BR-019 | Scores are calculated from saved game records and are never edited manually. |
| BR-020 | Restarting a game does not change existing scores. |

### 12.6 Result Verification and Saving

| ID | Rule |
|---|---|
| BR-021 | The server treats the submitted final board state as the source of truth and works out the outcome (X win, O win, or draw) from it itself; it does not trust the outcome claimed by the browser. |
| BR-022 | A board is valid only if it has nine cells, each empty, X, or O, and the number of X marks equals the number of O marks or is exactly one more (X always plays first). |
| BR-023 | A submitted result is accepted only if it matches the board: an X win requires a winning line for X (and none for O) with X having one more mark than O; an O win requires a winning line for O (and none for X) with equal numbers of marks; a draw requires all nine cells filled and no winning line. A board that is unfinished is rejected. |
| BR-024 | A game that fails verification is not saved and does not affect the scoreboard or history. The winner's name that is saved comes from the verified outcome (the name of Player X or Player O), not from a separate value supplied by the browser. |
| BR-025 | The browser sends one save request per completed game (see FR-041). |

---

## 13. Assumptions and Constraints

### 13.1 Assumptions

| ID | Assumption |
|---|---|
| AS-001 | Both players share one device and one browser window. |
| AS-002 | Players have a modern web browser with JavaScript enabled. |
| AS-003 | The ASP.NET Core MVC application and the SQL Server database are running whenever results are saved or viewed. |
| AS-004 | Players are identified only by the names they type; there are no accounts. |
| AS-005 | The scoreboard and history are shared by everyone who uses the same installation of the application. |
| AS-006 | The application is deployed locally or on a simple host for demonstration, with a small number of users. |

### 13.2 Constraints

| ID | Constraint |
|---|---|
| CN-001 | The technology stack is fixed: HTML5, CSS3, Bootstrap 5, JavaScript, ASP.NET Core MVC (a single application with no separate frontend), SQL Server, and Entity Framework Core. |
| CN-002 | The project must be completed within the course schedule by a student or a small team. |
| CN-003 | The application is limited to two players on the same device. |
| CN-004 | The application contains no AI functionality; AI is used only as a development assistant. |
| CN-005 | No paid services or third-party paid tools are required. |
| CN-006 | In this version, no unique game token exists on the server. Duplicate-save prevention relies on the browser behavior in FR-041. |

---

## 14. Dependencies

| ID | Dependency | Purpose |
|---|---|---|
| DP-001 | Bootstrap 5 | Responsive layout and interface components |
| DP-002 | ASP.NET Core (.NET) runtime and SDK | Building and running the MVC application |
| DP-003 | Entity Framework Core | Database access and database creation through the ORM |
| DP-004 | Microsoft SQL Server | Storage of game records |
| DP-005 | Modern web browser | Running the frontend |
| DP-006 | Development tools (code editor/IDE, SQL Server management tool, version control) | Development, testing, and submission |

---

## 15. Success Criteria and Acceptance Criteria

### 15.1 Success Criteria

| ID | Criterion |
|---|---|
| SUC-001 | All ten core features are implemented and working. |
| SUC-002 | Two players can complete a game with a correct win or draw outcome every time. |
| SUC-003 | Every completed game appears in Game History and is reflected in the Scoreboard. |
| SUC-004 | Data remains available after closing and reopening the application. |
| SUC-005 | The application works on the supported browsers and on desktop and phone screen sizes. |
| SUC-006 | The PRD is accepted by the instructor as a basis for the SRS and design stages. |

### 15.2 Acceptance Criteria

| ID | Feature | Acceptance Criteria | Related FR |
|---|---|---|---|
| AC-001 | Home Page | Home Page loads first and provides working links to Start Game, Scoreboard, and Game History. | FR-001 to FR-003 |
| AC-002 | Player names | Game does not start with empty, over-length, or identical names; valid names start the game. | FR-004 to FR-008 |
| AC-003 | Game board | A 3×3 empty board appears; clicking an empty cell places the correct symbol. | FR-009, FR-011 |
| AC-004 | Turns | X plays first; turns alternate; the current player is always displayed; clicking a filled cell changes nothing. | FR-010, FR-012 to FR-014 |
| AC-005 | Winner detection | All 8 winning lines are detected for both X and O; the winner is displayed and cells are highlighted; the board locks. | FR-015 to FR-017, FR-019 |
| AC-006 | Draw detection | A full board with no winning line shows a draw; a win on the ninth move is shown as a win. | FR-018, FR-019 |
| AC-007 | Restart | Restart clears the board, keeps names, gives X the first turn, and does not save an unfinished game. | FR-020 to FR-022 |
| AC-008 | Saving | Each completed game is stored exactly once with all required fields, even if the player double-clicks or repeats the action quickly. | FR-024, FR-025, FR-036, FR-041 |
| AC-009 | Scoreboard | Scoreboard shows correct win totals in descending order; draws do not add points; an empty-state message shows when there are no wins. | FR-027 to FR-031 |
| AC-010 | Game History | History shows all saved games with required columns, newest first; an empty-state message shows when empty. | FR-032 to FR-035 |
| AC-011 | Error handling | Failed saves and server errors produce a clear message without crashing the application. | FR-026, FR-039, NFR-011 |
| AC-012 | Responsiveness and compatibility | All pages are usable on desktop and phone screens in Chrome, Edge, Firefox, and Safari. | NFR-006, NFR-022 |
| AC-013 | Server-side verification | The server saves a submitted result only if it matches the final board state; a result with a mismatched board, an impossible board, or invalid names is rejected and not saved, and an error message is shown. | FR-039, FR-040, NFR-013 |

---

## 16. Future Enhancements

The following ideas are **not part of the current scope** and are listed only for possible later versions:

| ID | Enhancement |
|---|---|
| FE-001 | Best-of-three or best-of-five match mode |
| FE-002 | Display of draw counts on the scoreboard |
| FE-003 | Filtering or searching game history by player name |
| FE-004 | Option to clear the scoreboard and history |
| FE-005 | Light and dark display themes |
| FE-006 | Optional sound effects |
| FE-007 | Optional move timer per turn |
| FE-008 | Unique game token issued and checked by the server to prevent duplicate records at the server level |

---

## 17. Glossary

| Term | Definition |
|---|---|
| **ASP.NET Core MVC** | A Microsoft framework for building web applications using the Model-View-Controller pattern. In this project, one MVC application provides both the pages players see and the server logic that verifies, saves, and retrieves game data. |
| **Board** | The 3×3 grid on which the game is played. |
| **Bootstrap 5** | A CSS framework used to create responsive page layouts and components. |
| **Cell** | One of the nine squares on the board. |
| **Controller** | The part of the MVC application that receives requests from the browser, validates them, works with the data, and chooses the View to display. |
| **Draw** | A game that ends with a full board and no winner. |
| **Entity Framework Core (EF Core)** | An Object-Relational Mapper that lets the backend work with database tables using .NET objects. |
| **Game History** | A list of previously completed matches. |
| **Move** | The placement of one symbol in one empty cell. |
| **ORM** | Object-Relational Mapper; a tool that connects application objects to database tables. |
| **Player X / Player O** | The two players, identified by their symbols; Player X always starts. |
| **PRD** | Product Requirements Document. |
| **Restart** | Clearing the board to begin a new game with the same players. |
| **Scoreboard** | A list showing the total number of wins for each player. |
| **Server-Side Validation** | Checking of submitted data by the server itself, including re-verifying a game result against the final board state, before anything is saved. |
| **SQL Server** | Microsoft's relational database system used to store game data. |
| **SRS** | Software Requirements Specification, the next document in the project. |
| **Turn** | The period in which one player makes one move. |
| **View** | A page template (Razor View) in the MVC application that is displayed in the browser. |
| **Winning Line** | Three identical symbols in a row, column, or diagonal. |

---

## Appendix A: Document Quality Review

Before finalizing, the PRD was checked for completeness, consistency, and scope.

| Check | Result |
|---|---|
| Every core feature (1–10) has at least one functional requirement | Yes – Home Page (FR-001–003), two-player game (FR-008–015), names (FR-004–007), board (FR-009–011), winner (FR-016–017), draw (FR-018), results (FR-019), restart (FR-020–023), scoreboard (FR-027–031), history (FR-032–035) |
| Every requirement has a unique ID | Yes |
| Functional and non-functional requirements are clearly separated | Yes – Sections 8 and 9 |
| Requirements are consistent with business rules | Yes – turn order, win, draw, and score rules match FR-010 to FR-021 and FR-028 |
| Out-of-scope features are absent from the requirements | Yes – no online play, chat, payments, authentication, AI opponent, or single-player mode |
| Contradictions found | None |
| Unnecessary features | None; enhancements are listed separately as future work |

### Decisions Made to Fill Gaps in the Brief

The project description did not specify the following points. They were defined minimally so that the requirements can be implemented and tested. The instructor or team may adjust them in the SRS.

| # | Decision | Reason |
|---|---|---|
| 1 | Player names must be non-empty, up to 20 characters, and different from each other (FR-005 to FR-007). | Needed for reliable scoreboard counting by name. |
| 2 | Scores are tracked by player name, not case-sensitive (BR-018). | The system has no accounts, so names are the only identifier. |
| 3 | Only completed games are saved (FR-022, BR-015). | Prevents incomplete records in history. |
| 4 | Restart keeps the same names; changing players is done from the setup screen (FR-021, FR-023). | Gives a clear, simple meaning to "restart." |
| 5 | Draws are saved in history but not counted on the scoreboard (BR-017). | The scoreboard is defined as wins only. |
| 6 | Gameplay (turns, win, and draw detection) runs in the browser (JavaScript) for instant feedback; the server re-verifies every submitted result against the final board state before saving it (FR-040). | Keeps the game responsive while ensuring the database receives only results that are consistent with a valid board. |
| 7 | The application is a single ASP.NET Core MVC application with no separate frontend (replacing the earlier Web API design). | Required by the architecture document. |
| 8 | Duplicate saves are prevented by the browser sending one request per game and disabling save-related controls after the first request (FR-041); no server-side game token is used. | The architecture document provides no unique game token in this version. |

*End of Document*
