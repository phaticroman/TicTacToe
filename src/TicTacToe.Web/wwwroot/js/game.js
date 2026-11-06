(function () {
  "use strict";

  const board = document.querySelector("[data-game-board]");
  if (!board) {
    return;
  }

  const roomCode = board.dataset.roomCode;
  const playerSymbol = board.dataset.playerSymbol;
  const cells = Array.from(board.querySelectorAll("[data-cell-index]"));
  const statusChip = document.querySelector("[data-status-chip]");
  const playerList = document.querySelector("[data-player-list]");
  const gameNote = document.querySelector("[data-game-note]");
  const connectionNote = document.querySelector("[data-connection-note]");
  const moveError = document.querySelector("[data-move-error]");
  const rematchBox = document.querySelector("[data-rematch-box]");
  const rematchButton = document.querySelector("[data-rematch-button]");
  const rematchNote = document.querySelector("[data-rematch-note]");
  const historyList = document.querySelector("[data-history-list]");
  const scorePlayed = document.querySelector("[data-score-played]");
  const scoreDraws = document.querySelector("[data-score-draws]");
  const scoreList = document.querySelector("[data-score-list]");

  let state = null;
  let joined = false;
  let pendingMove = false;
  let pendingRematch = false;

  function setConnectionNote(message, isError) {
    if (!connectionNote) {
      return;
    }

    connectionNote.textContent = message;
    connectionNote.classList.toggle("is-error", Boolean(isError));
  }

  function showMoveError(message) {
    if (!moveError) {
      return;
    }

    moveError.textContent = message || "That move could not be played.";
    moveError.hidden = false;
  }

  function clearMoveError() {
    if (!moveError) {
      return;
    }

    moveError.textContent = "";
    moveError.hidden = true;
  }

  function statusFor(room) {
    if (room.status === "WaitingForPlayer") {
      return { text: "Waiting for opponent", className: "is-waiting" };
    }

    if (room.status === "InProgress") {
      return room.currentTurn === playerSymbol
        ? { text: "Your turn", className: "is-active" }
        : { text: "Opponent's turn", className: "is-waiting" };
    }

    if (room.status === "Won") {
      return room.winnerSymbol === playerSymbol
        ? { text: "You win", className: "is-finished" }
        : { text: "You lose", className: "is-finished" };
    }

    return { text: "Draw", className: "is-finished" };
  }

  function opponentFor(room) {
    return room.players.find((player) => player.symbol !== playerSymbol) || null;
  }

  function noteFor(room) {
    if (room.status === "WaitingForPlayer") {
      return "Share the room code " + room.roomCode + " with a friend. The board unlocks when they join.";
    }

    if (room.status === "InProgress") {
      const opponent = opponentFor(room);
      if (opponent && !opponent.isConnected) {
        return opponent.displayName + " is offline. The board will sync when they return.";
      }

      return room.currentTurn === playerSymbol
        ? "You have the next move."
        : "Waiting for the other player.";
    }

    return "The match has ended.";
  }

  function rematchFor(room) {
    if (room.status !== "Won" && room.status !== "Draw") {
      return { visible: false, label: "Request rematch", disabled: true, note: "" };
    }

    const me = room.players.find((player) => player.symbol === playerSymbol);
    const opponent = opponentFor(room);
    const opponentName = opponent ? opponent.displayName : "The other player";

    if (me && me.wantsRematch) {
      return {
        visible: true,
        label: "Rematch requested",
        disabled: true,
        note: "Waiting for " + opponentName + " to accept."
      };
    }

    if (opponent && opponent.wantsRematch) {
      return {
        visible: true,
        label: "Accept rematch",
        disabled: false,
        note: opponentName + " wants a rematch."
      };
    }

    return {
      visible: true,
      label: "Request rematch",
      disabled: false,
      note: "Ready for another round?"
    };
  }

  function createPlayerItem(player) {
    const isYou = player.symbol === playerSymbol;
    const item = document.createElement("li");
    item.className = isYou ? "player is-you" : "player";

    const symbol = document.createElement("span");
    symbol.className = "player-symbol " + (player.symbol === "X" ? "mark-x" : "mark-o");
    symbol.setAttribute("aria-label", player.symbol);
    symbol.textContent = player.symbol;

    const name = document.createElement("span");
    name.className = "player-name";
    name.textContent = player.displayName;

    const meta = document.createElement("span");
    meta.className = "player-meta";

    const note = document.createElement("span");
    note.className = "player-note";
    note.textContent = isYou ? "you" : "opponent";

    const presence = document.createElement("span");
    presence.className = "player-presence " + (player.isConnected ? "is-online" : "is-offline");
    presence.textContent = player.isConnected ? "online" : "offline";

    meta.append(note, presence);
    item.append(symbol, name, meta);
    return item;
  }

  function formatHistoryTime(value) {
    const endedAt = new Date(value);
    if (Number.isNaN(endedAt.getTime())) {
      return "";
    }

    return endedAt.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  }

  function createHistoryItem(entry) {
    const item = document.createElement("li");
    item.className = "history-item";

    const round = document.createElement("span");
    round.className = "history-round";
    round.textContent = "Game " + entry.roundNumber;

    const result = document.createElement("span");
    result.className = "history-result";

    if (entry.winnerSymbol === null) {
      result.textContent = "Draw";
    } else {
      const mark = document.createElement("span");
      mark.className = "history-mark " + (entry.winnerSymbol === "X" ? "mark-x" : "mark-o");
      mark.setAttribute("aria-label", entry.winnerSymbol);
      mark.textContent = entry.winnerSymbol;

      const label = document.createElement("span");
      label.textContent = (entry.winnerName || entry.winnerSymbol) + " won";

      result.append(mark, label);
    }

    const time = document.createElement("time");
    time.className = "history-time";
    time.dateTime = entry.endedAt;
    time.textContent = formatHistoryTime(entry.endedAt);
    time.title = new Date(entry.endedAt).toLocaleString();

    item.append(round, result, time);
    return item;
  }

  function updateHistory(room) {
    if (!historyList) {
      return;
    }

    const entries = (room.history || [])
      .slice()
      .sort((left, right) => right.roundNumber - left.roundNumber);

    if (entries.length === 0) {
      const empty = document.createElement("li");
      empty.className = "history-empty";
      empty.textContent = "No finished matches yet.";
      historyList.replaceChildren(empty);
      return;
    }

    historyList.replaceChildren(...entries.map(createHistoryItem));
  }

  function createScoreRow(tally) {
    const isYou = tally.symbol === playerSymbol;
    const item = document.createElement("li");
    item.className = isYou ? "scorecard-row is-you" : "scorecard-row";

    const mark = document.createElement("span");
    mark.className = "scorecard-mark " + (tally.symbol === "X" ? "mark-x" : "mark-o");
    mark.setAttribute("aria-label", tally.symbol);
    mark.textContent = tally.symbol;

    const name = document.createElement("span");
    name.className = "scorecard-name";
    name.textContent = tally.displayName;

    if (isYou) {
      const you = document.createElement("span");
      you.className = "scorecard-you";
      you.textContent = "you";
      name.append(" ", you);
    }

    const wins = document.createElement("span");
    wins.className = "scorecard-wins";
    const count = document.createElement("strong");
    count.textContent = tally.wins;
    wins.append(count, " " + (tally.wins === 1 ? "win" : "wins"));

    item.append(mark, name, wins);
    return item;
  }

  function updateScorecard(room) {
    if (!room.stats) {
      return;
    }

    if (scorePlayed) {
      scorePlayed.textContent = room.stats.matchesPlayed;
    }

    if (scoreDraws) {
      scoreDraws.textContent = room.stats.draws;
    }

    if (scoreList) {
      scoreList.replaceChildren(...room.stats.players.map(createScoreRow));
    }
  }

  function updateBoardInteractivity() {
    const canPlay = Boolean(state) &&
      joined &&
      !pendingMove &&
      state.status === "InProgress" &&
      state.currentTurn === playerSymbol;

    cells.forEach((cell) => {
      const index = Number(cell.dataset.cellIndex);
      const isEmpty = state ? state.board[index] === "-" : false;
      const open = canPlay && isEmpty;
      cell.disabled = !open;
      cell.classList.toggle("is-open", open);
    });
  }

  function updateRematchControl() {
    if (!rematchBox || !rematchButton || !rematchNote) {
      return;
    }

    if (!state) {
      rematchBox.hidden = true;
      return;
    }

    const rematch = rematchFor(state);
    rematchBox.hidden = !rematch.visible;
    rematchButton.textContent = rematch.label;
    rematchButton.disabled = rematch.disabled ||
      pendingRematch ||
      !joined ||
      connection.state !== "Connected";
    rematchNote.textContent = rematch.note;
  }

  function applyState(room) {
    state = room;
    joined = true;
    pendingMove = false;
    pendingRematch = false;
    clearMoveError();

    cells.forEach((cell) => {
      const index = Number(cell.dataset.cellIndex);
      const mark = room.board[index];
      const value = mark === "-" ? "" : mark;
      cell.textContent = value;
      cell.classList.toggle("mark-x", mark === "X");
      cell.classList.toggle("mark-o", mark === "O");
      cell.setAttribute("aria-label", "Cell " + (index + 1) + ": " + (value || "empty"));
    });

    if (statusChip) {
      const status = statusFor(room);
      statusChip.textContent = status.text;
      statusChip.className = "status-chip " + status.className;
    }

    if (playerList) {
      playerList.replaceChildren(...room.players.map(createPlayerItem));
    }

    if (gameNote) {
      gameNote.textContent = noteFor(room);
    }

    updateHistory(room);
    updateScorecard(room);
    updateBoardInteractivity();
    updateRematchControl();
  }

  if (typeof signalR === "undefined") {
    setConnectionNote("Live updates are unavailable. Refresh to try again.", true);
    return;
  }

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gamehub")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  connection.on("RoomState", applyState);
  connection.on("MoveRejected", (message) => {
    pendingMove = false;
    showMoveError(message);
    updateBoardInteractivity();
  });
  connection.on("RematchRejected", (message) => {
    pendingRematch = false;
    showMoveError(message || "Could not request a rematch.");
    updateRematchControl();
  });

  connection.onreconnecting(() => {
    joined = false;
    pendingMove = false;
    pendingRematch = false;
    setConnectionNote("Connection lost. Reconnecting...", false);
    updateBoardInteractivity();
    updateRematchControl();
  });

  connection.onreconnected(() => {
    setConnectionNote("Reconnected. Syncing the board...", false);
    joinRoom();
  });

  connection.onclose(() => {
    joined = false;
    pendingMove = false;
    setConnectionNote("Connection closed. Refresh to rejoin the room.", true);
    updateBoardInteractivity();
  });

  async function joinRoom() {
    try {
      await connection.invoke("JoinRoom", roomCode);
      setConnectionNote("", false);
    } catch (error) {
      setConnectionNote(error.message || "Could not join the live room.", true);
    }
  }

  async function startConnection() {
    setConnectionNote("Connecting to the live board...", false);

    try {
      await connection.start();
    } catch (error) {
      setConnectionNote("Live updates are unavailable. Retrying...", true);
      window.setTimeout(startConnection, 3000);
      return;
    }

    await joinRoom();
  }

  if (rematchButton) {
    rematchButton.addEventListener("click", async () => {
      if (pendingRematch || !joined || connection.state !== "Connected") {
        return;
      }

      pendingRematch = true;
      clearMoveError();
      updateRematchControl();

      try {
        await connection.invoke("RequestRematch");
      } catch (error) {
        pendingRematch = false;
        showMoveError(error.message || "Could not request a rematch.");
        updateRematchControl();
      }
    });
  }

  board.addEventListener("click", async (event) => {
    const cell = event.target.closest(".board-cell");
    if (!cell || cell.disabled || pendingMove || !joined || connection.state !== "Connected") {
      return;
    }

    const cellIndex = Number(cell.dataset.cellIndex);
    if (!Number.isInteger(cellIndex)) {
      return;
    }

    pendingMove = true;
    clearMoveError();
    updateBoardInteractivity();

    try {
      await connection.invoke("MakeMove", cellIndex);
    } catch (error) {
      pendingMove = false;
      showMoveError(error.message || "Could not send that move.");
      updateBoardInteractivity();
    }
  });

  startConnection();
})();
