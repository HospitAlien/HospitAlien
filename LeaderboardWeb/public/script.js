import { firebaseConfig } from "./apikey.js";

firebase.initializeApp(firebaseConfig);
const db = firebase.firestore();
console.log("Firebase initialized.");

const REFRESH_INTERVAL_MS = 60000;
const UPDATE_TIMER_INTERVAL_MS = 1000;

const urlParams = new URLSearchParams(window.location.search);
const maxPlayers = parseInt(urlParams.get("max")) || 10;

const leaderboardTitle = document.getElementById("leaderboard-title");
leaderboardTitle.textContent = `Top ${maxPlayers} Doctors in HospitAlien`;

// DOM Element References
const leaderboardBody = document.getElementById("leaderboard-body");
const lastUpdatedElement = document.getElementById("last-updated");
const updateTimerElement = document.getElementById("update-timer");

let updateIntervalId = null;
let countdownIntervalId = null;
let timeUntilNextUpdate = REFRESH_INTERVAL_MS / 1000;

// Fetch Leaderboard Data from Firebase
async function fetchLeaderboardData() {
  console.log("Fetching leaderboard data from Firebase...");
  try {
    const querySnapshot = await db
      .collection("leaderboard")
      .orderBy("score", "desc")
      .limit(maxPlayers + 5)
      .get();

    let rawData = [];
    querySnapshot.forEach((doc) => {
      const data = doc.data();
      if (data && typeof data.score === "number" && data.name && data.time) {
        rawData.push({
          id: doc.id,
          name: data.name,
          score: data.score,
          timestamp: data.time,
        });
      } else {
        console.warn(
          "Skipping document with missing/invalid data:",
          doc.id,
          data
        );
      }
    });

    rawData.sort((a, b) => {
      if (b.score !== a.score) {
        return 0;
      }
      return a.timestamp.seconds - b.timestamp.seconds;
    });

    let processedData = [];
    let currentRank = 0;
    let lastScore = -Infinity;

    for (let i = 0; i < rawData.length; i++) {
      const player = rawData[i];

      if (player.score !== lastScore) {
        currentRank = i + 1;
      }

      processedData.push({
        rank: currentRank,
        name: player.name,
        score: player.score,
      });

      lastScore = player.score;
      if (processedData.length >= maxPlayers) {
        break;
      }
    }

    return processedData;
  } catch (error) {
    console.error("Error fetching or processing leaderboard data:", error);
    throw error;
  }
}

function renderLeaderboard(data) {
  leaderboardBody.innerHTML = ""; // Clear existing content

  if (!data || data.length === 0) {
    leaderboardBody.innerHTML =
      '<tr><td colspan="4" class="error">Leaderboard fly away~ We are catching</td></tr>';
    return;
  }

  data.forEach((player) => {
    const row = document.createElement("tr");
    if (player.rank >= 1 && player.rank <= 3) {
      row.classList.add(`rank-${player.rank}`);
    }
    row.innerHTML = `
            <td class="rank">${player.rank}</td>
            <td class="name">${escapeHtml(player.name)}</td>
            <td class="score">${player.score.toLocaleString()}</td>
        `;
    leaderboardBody.appendChild(row);
  });
}

function escapeHtml(unsafe) {
  if (!unsafe) return "";
  return unsafe
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
}

async function UpdateLeaderboard() {
  console.log("Updating Leaderboard");
  stopCountdown(); // Stop previous countdown

  try {
    const data = await fetchLeaderboardData();
    renderLeaderboard(data);
    const now = new Date();
    lastUpdatedElement.textContent = `Last updated: ${now.toLocaleTimeString(
      "en-GB"
    )}`;
  } catch (error) {
    console.error("Leaderboard update failed:", error);
    leaderboardBody.innerHTML = `<tr><td colspan="4" class="error">Leaderboard fly away~ We are catching</td></tr>`;
    lastUpdatedElement.textContent = `Update failed`;
  } finally {
    resetAndStartCountdown(); // Always restart countdown
  }
}

function updateCountdownDisplay() {
  if (timeUntilNextUpdate > 0) {
    updateTimerElement.textContent = `Next update in ${timeUntilNextUpdate} second${
      timeUntilNextUpdate === 1 ? "" : "s"
    }`;
    timeUntilNextUpdate--;
  } else {
    updateTimerElement.textContent = `Updating now...`;
    stopCountdown();
  }
}

function stopCountdown() {
  if (countdownIntervalId) {
    clearInterval(countdownIntervalId);
    countdownIntervalId = null;
  }
}

function resetAndStartCountdown() {
  stopCountdown();
  timeUntilNextUpdate = REFRESH_INTERVAL_MS / 1000;
  updateCountdownDisplay();
  countdownIntervalId = setInterval(
    updateCountdownDisplay,
    UPDATE_TIMER_INTERVAL_MS
  );
}

// Init
document.addEventListener("DOMContentLoaded", () => {
  console.log("DOM loaded, initializing leaderboard...");
  UpdateLeaderboard();

  // Set up auto-refresh interval
  if (updateIntervalId) clearInterval(updateIntervalId);
  updateIntervalId = setInterval(UpdateLeaderboard, REFRESH_INTERVAL_MS);
  console.log(`Auto-refresh set every ${REFRESH_INTERVAL_MS / 1000} seconds`);
});
