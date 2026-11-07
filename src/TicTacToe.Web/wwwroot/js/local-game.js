(function () {
  "use strict";

  const game = document.querySelector("[data-local-game]");
  if (!game) {
    return;
  }

  const board = game.querySelector("[data-local-board]");
  const cells = Array.from(game.querySelectorAll("[data-cell-index]"));
  const statusChip = game.querySelector("[data-local-status]");
  const gameNote = game.querySelector("[data-local-note]");
  const moveError = game.querySelector("[data-local-error]");
  const roundBox = game.querySelector("[data-local-round-box]");
  const newRoundButton = game.querySelector("[data-local-new-round]");
  const scorePlayed = game.querySelector("[data-local-score-played]");
  const scoreDraws = game.querySelector("[data-local-score-draws]");
  const xWins = game.querySelector("[data-local-x-wins]");
  const oWins = game.querySelector("[data-local-o-wins]");
  const xWinsLabel = game.querySelector("[data-local-x-wins-label]");
  const oWinsLabel = game.querySelector("[data-local-o-wins-label]");
  const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
  const moveUrl = game.dataset.moveUrl;
  const newRoundUrl = game.dataset.newRoundUrl;
  const token = tokenInput ? tokenInput.value : "";

  const initialStatus = game.dataset.status || "InProgress";
  let state = {
    board: game.dataset.board || "---------",
    status: initialStatus,
    currentTurn: game.dataset.currentTurn || null,
    winnerSymbol: game.dataset.winnerSymbol || null,
    matchesPlayed: Number(scorePlayed ? scorePlayed.textContent : 0),
    draws: Number(scoreDraws ? scoreDraws.textContent : 0),
    xWins: Number(xWins ? xWins.textContent : 0),
    oWins: Number(oWins ? oWins.textContent : 0),
    statusText: statusChip ? statusChip.textContent : "",
    statusClass: statusChip ? statusChip.classList.contains("is-finished") ? "is-finished" : "is-active" : "is-active",
    gameNote: gameNote ? gameNote.textContent : "",
    isGameOver: initialStatus === "Won" || initialStatus === "Draw"
  };
  let pending = false;

  function showError(message) {
    if (!moveError) {
      return;
    }

    moveError.textContent = message || "That move could not be played.";
    moveError.hidden = false;
  }

  function clearError() {
    if (!moveError) {
      return;
    }

    moveError.textContent = "";
    moveError.hidden = true;
  }

  function updateBoard() {
    cells.forEach((cell) => {
      const index = Number(cell.dataset.cellIndex);
      const mark = state.board[index];
      const value = mark === "-" ? "" : mark;
      cell.textContent = value;
      cell.classList.toggle("mark-x", mark === "X");
      cell.classList.toggle("mark-o", mark === "O");
      cell.setAttribute("aria-label", "Cell " + (index + 1) + ": " + (value || "empty"));
    });
  }

  function updateStatus() {
    if (!statusChip) {
      return;
    }

    statusChip.textContent = state.statusText;
    statusChip.className = "status-chip " + (state.statusClass || "is-active");
  }

  function updatePlayers() {
    const inProgress = state.status === "InProgress";

    game.querySelectorAll("[data-local-player]").forEach((player) => {
      const symbol = player.dataset.localPlayer;
      const isUp = inProgress && state.currentTurn === symbol;
      player.classList.toggle("is-turn", isUp);

      const note = player.querySelector("[data-local-player-note]");
      if (note) {
        note.textContent = inProgress ? (isUp ? "up now" : "waiting") : "round over";
      }
    });

    if (gameNote) {
      gameNote.textContent = state.gameNote;
    }
  }

  function updateWins(element, label, count) {
    if (element) {
      element.textContent = count;
    }

    if (label) {
      label.textContent = count === 1 ? "win" : "wins";
    }
  }

  function updateScorecard() {
    if (scorePlayed) {
      scorePlayed.textContent = state.matchesPlayed;
    }

    if (scoreDraws) {
      scoreDraws.textContent = state.draws;
    }

    updateWins(xWins, xWinsLabel, state.xWins);
    updateWins(oWins, oWinsLabel, state.oWins);
  }

  function updateBoardInteractivity() {
    const canPlay = state.status === "InProgress" && !pending;

    cells.forEach((cell) => {
      const index = Number(cell.dataset.cellIndex);
      const open = canPlay && state.board[index] === "-";
      cell.disabled = !open;
      cell.classList.toggle("is-open", open);
    });
  }

  function updateRoundControl() {
    if (roundBox) {
      roundBox.hidden = !state.isGameOver;
    }

    if (newRoundButton) {
      newRoundButton.disabled = pending;
    }
  }

  function applyState(next) {
    state = next;
    pending = false;
    clearError();
    updateBoard();
    updateStatus();
    updatePlayers();
    updateScorecard();
    updateBoardInteractivity();
    updateRoundControl();
  }

  async function post(url, values) {
    if (!url || !token) {
      throw new Error("This local game is unavailable. Refresh the page.");
    }

    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
        "RequestVerificationToken": token
      },
      body: new URLSearchParams(values)
    });

    let payload = null;
    try {
      payload = await response.json();
    } catch (error) {
      payload = null;
    }

    if (!response.ok) {
      throw new Error(payload && payload.error ? payload.error : "Could not reach the local game.");
    }

    if (!payload) {
      throw new Error("The local game returned an empty response.");
    }

    return payload;
  }

  board.addEventListener("click", async (event) => {
    const cell = event.target.closest(".board-cell");
    if (!cell || cell.disabled || pending || state.status !== "InProgress") {
      return;
    }

    const cellIndex = Number(cell.dataset.cellIndex);
    if (!Number.isInteger(cellIndex)) {
      return;
    }

    pending = true;
    clearError();
    updateBoardInteractivity();
    updateRoundControl();

    try {
      applyState(await post(moveUrl, { cellIndex: String(cellIndex) }));
    } catch (error) {
      pending = false;
      showError(error.message || "Could not play that move.");
      updateBoardInteractivity();
      updateRoundControl();
    }
  });

  if (newRoundButton) {
    newRoundButton.addEventListener("click", async () => {
      if (pending || !state.isGameOver) {
        return;
      }

      pending = true;
      clearError();
      updateBoardInteractivity();
      updateRoundControl();

      try {
        applyState(await post(newRoundUrl, {}));
        const firstOpenCell = cells.find((cell) => !cell.disabled);
        if (firstOpenCell) {
          firstOpenCell.focus();
        }
      } catch (error) {
        pending = false;
        showError(error.message || "Could not start a new round.");
        updateBoardInteractivity();
        updateRoundControl();
      }
    });
  }

  updateBoardInteractivity();
  updateRoundControl();
})();
