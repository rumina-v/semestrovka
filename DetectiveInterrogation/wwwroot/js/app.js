const tokenKey = "detective.jwt";
const caseKey = "detective.case";

function getToken() {
    return localStorage.getItem(tokenKey);
}

function setToken(token) {
    localStorage.setItem(tokenKey, token);
}

function clearToken() {
    localStorage.removeItem(tokenKey);
    sessionStorage.removeItem(caseKey);
}

function parseJwt(token) {
    try {
        const payload = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
        return JSON.parse(decodeURIComponent(atob(payload).split("").map(c => `%${(`00${c.charCodeAt(0).toString(16)}`).slice(-2)}`).join("")));
    } catch {
        return {};
    }
}

function claims() {
    const token = getToken();
    return token ? parseJwt(token) : {};
}

function claim(name) {
    const data = claims();
    return data[name] || data[`http://schemas.xmlsoap.org/ws/2005/05/identity/claims/${name}`] || data[`http://schemas.microsoft.com/ws/2008/06/identity/claims/${name}`];
}

function getRole() {
    return claim("role");
}

async function api(path, options = {}) {
    const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
    const token = getToken();
    if (token) headers.Authorization = `Bearer ${token}`;

    const response = await fetch(path, { ...options, headers });
    const text = await response.text();
    const data = text ? JSON.parse(text) : null;

    if (!response.ok) {
        throw new Error(data?.message || data?.error || `HTTP ${response.status}`);
    }

    return data;
}

function html(value) {
    const element = document.createElement("div");
    element.textContent = value ?? "";
    return element.innerHTML;
}

function status(message, isError = false) {
    const element = document.querySelector("[data-status]");
    if (!element) return;
    element.textContent = message || "";
    element.classList.toggle("error", isError);
}

function isAuthenticated() {
    const token = getToken();
    if (!token) return false;

    const data = parseJwt(token);
    if (!data.exp) return true;

    const isExpired = Date.now() >= data.exp * 1000;
    if (isExpired) {
        clearToken();
        return false;
    }

    return true;
}

function requireAuth() {
    if (isAuthenticated()) return true;
    window.location.href = "/auth/login";
    return false;
}

function updateHomeNav() {
    const signedIn = isAuthenticated();
    document.querySelectorAll("[data-guest-only]").forEach(item => item.hidden = signedIn);
    document.querySelectorAll("[data-auth-only]").forEach(item => item.hidden = !signedIn);
}

async function loadHome() {
    if (!document.querySelector("[data-home-page]")) return;

    updateHomeNav();
}

function initAuth() {
    const loginForm = document.querySelector("[data-login-form]");
    if (loginForm) {
        loginForm.addEventListener("submit", async event => {
            event.preventDefault();
            const form = new FormData(loginForm);
            try {
                const result = await api("/api/Auth/login", {
                    method: "POST",
                    body: JSON.stringify({
                        username: form.get("username"),
                        password: form.get("password")
                    })
                });
                setToken(result.token);
                window.location.href = "/";
            } catch (error) {
                status(error.message, true);
            }
        });
    }

    const registerForm = document.querySelector("[data-register-form]");
    if (registerForm) {
        registerForm.addEventListener("submit", async event => {
            event.preventDefault();
            const form = new FormData(registerForm);
            try {
                const result = await api("/api/Auth/register", {
                    method: "POST",
                    body: JSON.stringify({
                        username: form.get("username"),
                        email: form.get("email"),
                        password: form.get("password")
                    })
                });
                setToken(result.token);
                window.location.href = "/";
            } catch (error) {
                status(error.message, true);
            }
        });
    }
}

async function getMainCase() {
    const cases = await api("/api/Case");
    const item = cases[0];
    if (!item) throw new Error("Дело не найдено.");
    return await api(`/api/Case/${item.id}`);
}

async function loadDesk() {
    if (!document.querySelector("[data-desk-page]")) return;
    if (!requireAuth()) return;

    try {
        const item = await getMainCase();
        sessionStorage.setItem(caseKey, JSON.stringify(item));

        document.querySelector("[data-desk-title]").textContent = item.title || "Убийство судьи";
        document.querySelector("[data-desk-description]").textContent = item.fullDescription || item.shortDescription || "Материалы дела пока не заполнены.";

        const suspects = item.suspects || [];
        renderSuspect("[data-suspect-one]", suspects[0], "Подозреваемый №1 не найден.");
        renderSuspect("[data-suspect-two]", suspects[1], "Подозреваемый №2 не найден.");

        const evidence = item.evidence || [];
        document.querySelector("[data-desk-evidence]").innerHTML = evidence.length
            ? evidence.map(e => `<div class="evidence-item"><strong>${html(e.title)}</strong><br>${html(e.shortText || e.fullText || "")}</div>`).join("")
            : `<div class="evidence-item">Улики не добавлены.</div>`;
    } catch (error) {
        status(error.message, true);
    }
}

function renderSuspect(selector, suspect, fallback) {
    const root = document.querySelector(selector);
    if (!root) return;
    if (!suspect) {
        root.innerHTML = `<p>${fallback}</p>`;
        return;
    }

    root.innerHTML = `
        <div class="suspect-card">
            <strong>${html(suspect.name)}</strong>
            <p>${html(suspect.description || "Описание отсутствует.")}</p>
        </div>
    `;
}

function showDeskSpread(index) {
    const file = document.querySelector("[data-case-file]");
    if (!file) return;
    file.dataset.spread = String(index);
    const desk = document.querySelector("[data-desk-page]");
    if (desk) {
        desk.dataset.deskSpread = String(index);
    }
    document.querySelectorAll("[data-spread-panel]").forEach(panel => {
        panel.hidden = Number(panel.dataset.spreadPanel) !== index;
    });
}

function getSavedCase() {
    const raw = sessionStorage.getItem(caseKey);
    return raw ? JSON.parse(raw) : null;
}

async function ensureCaseForRoom() {
    const saved = getSavedCase();
    if (saved) return saved;
    return await getMainCase();
}

const interrogation = {
    case: null,
    suspects: [],
    index: 0,
    sessionId: null,
    active: false
};

const interrogationGreetings = [
    {
        detective: "Здравствуйте, Мадам Маккэри. Вчера ночью была убита ваша соседка, Мари Робинсон. У следствия есть к вам несколько вопросов. Пожалуйста, помните что ложь и увиливание на допросе будут восприняты как препятствование расследованию. Расскажите что случилось в тот день.",
        suspect: "Добрый день, да.. уже видела новость об этой ужасной трагедии. В тот день я приходила к Мари, чтобы поговорить о старом деле. Дома была странная атмосфера, она была на нервах, все вокруг было в каких то клочках бумаги. Ну, в общем, как и раньше, не пришли ни к какому согласию. Она не хотела меня слушать, а я уже слишком устала от этих разговоров. После этого вернулась домой. Больше я к ней в тот день не возвращалась."
    },
    {
        detective: "Здравствуйте, Мистер Робинсон. Вчера ночью была убита ваша жена, Мари Робинсон. У следствия есть к вам несколько вопросов. Пожалуйста, помните что ложь и увиливание на допросе будут восприняты как препятствование расследованию. Расскажите что случилось в тот день.",
        suspect: "Здравствуйте. В тот день утром мы с Мари поссорились. Это пустяки, просто недопонимания. И к обеду я уехал на работу. Потом чтобы не продолжать ссору, ночью не вернулся, а утром мне позвонили из полиции и сказали приехать для дачи показаний. Узнал все я от них, потом прочел газету и знаете... До сих пор не могу осознать, что это правда, моей Мари больше нет."
    }
];

function suspectSortValue(suspect) {
    const name = (suspect?.name || "").toLowerCase();
    if (name.includes("лилит")) return 0;
    if (name.includes("джефф")) return 1;
    return 2;
}

function getGreetingForSuspect(suspect, fallbackIndex) {
    const name = (suspect?.name || "").toLowerCase();
    if (name.includes("джефф")) return interrogationGreetings[1];
    if (name.includes("лилит")) return interrogationGreetings[0];
    return interrogationGreetings[fallbackIndex] || interrogationGreetings[0];
}

async function initInterrogationRoom() {
    if (!document.querySelector("[data-interrogation-page]")) return;
    if (!requireAuth()) return;

    history.replaceState({ interrogationLocked: true }, "", window.location.href);
    history.pushState({ interrogationLocked: true }, "", window.location.href);
    window.addEventListener("popstate", () => {
        history.pushState({ interrogationLocked: true }, "", window.location.href);
        status("Во время допроса нельзя вернуться к материалам дела.", true);
    });

    try {
        interrogation.case = await ensureCaseForRoom();
        interrogation.suspects = [...(interrogation.case.suspects || [])]
            .sort((left, right) => suspectSortValue(left) - suspectSortValue(right));
        if (interrogation.suspects.length === 0) throw new Error("В деле нет подозреваемых.");
        setCurrentSuspect(0);
    } catch (error) {
        status(error.message, true);
    }
}

function setCurrentSuspect(index) {
    interrogation.index = index;
    interrogation.sessionId = null;
    interrogation.active = false;

    const suspect = interrogation.suspects[index];
    const room = document.querySelector("[data-interrogation-page]");
    if (room) {
        room.dataset.suspectIndex = String(index);
        room.dataset.phase = "intro";
    }

    document.querySelector("[data-suspect-name]").textContent = suspect.name;
    document.querySelector("[data-suspect-counter]").textContent = `Подозреваемый ${index + 1} из ${interrogation.suspects.length}`;
    document.querySelector("[data-suspect-speech]").textContent = "Подозреваемый молча ждёт начала разговора.";
    document.querySelector("[data-detective-speech]").hidden = true;
    document.querySelector("[data-notebook]").hidden = true;
    document.querySelector("[data-state-panel]").hidden = true;
    document.querySelector("[data-start-greeting]").hidden = false;
    document.querySelector("[data-start-greeting]").textContent = "Начать приветствие";
    status("");
}

async function startCurrentSuspect() {
    const suspect = interrogation.suspects[interrogation.index];
    if (!suspect) {
        status("Данные подозреваемого еще не загрузились. Обновите страницу или вернитесь к столу детектива.", true);
        return;
    }

    if (interrogation.active) return;

    const greeting = getGreetingForSuspect(suspect, interrogation.index);
    const greetingButton = document.querySelector("[data-start-greeting]");
    const detectiveSpeech = document.querySelector("[data-detective-speech]");
    const suspectSpeech = document.querySelector("[data-suspect-speech]");
    const notebook = document.querySelector("[data-notebook]");
    const statePanel = document.querySelector("[data-state-panel]");

    suspectSpeech.textContent = greeting.suspect;
    detectiveSpeech.hidden = false;
    detectiveSpeech.textContent = greeting.detective;
    greetingButton.hidden = true;
    status("Загружаю материалы для вопросов...");

    try {
        const session = await api("/api/Interrogation/start", {
            method: "POST",
            body: JSON.stringify({
                caseId: interrogation.case.id,
                suspectId: suspect.id
            })
        });

        interrogation.sessionId = session.id;
        interrogation.active = true;

        const room = document.querySelector("[data-interrogation-page]");
        if (room) {
            room.dataset.phase = "active";
        }

        notebook.hidden = false;
        statePanel.hidden = false;
        renderState(session);
        await loadPhrases(session.id);
        status("");
    } catch (error) {
        greetingButton.hidden = false;
        status(error.message, true);
    }
}

function renderState(state) {
    document.querySelector("[data-trust]").textContent = state.currentTrust ?? 0;
    document.querySelector("[data-aggression]").textContent = state.currentAggression ?? 0;
    document.querySelector("[data-session-status]").textContent = state.status || "InProgress";
}

async function loadPhrases(sessionId) {
    const phrases = await api(`/api/Interrogation/phrases/${sessionId}`);
    const root = document.querySelector("[data-phrases-list]");

    if (!phrases.length) {
        root.innerHTML = `<button class="phrase-button" type="button" data-finish-suspect>Завершить допрос</button>`;
        return;
    }

    const groups = new Map();
    phrases.forEach(phrase => {
        const key = phrase.evidenceId;
        if (!groups.has(key)) groups.set(key, []);
        groups.get(key).push(phrase);
    });

    root.innerHTML = Array.from(groups.values()).map(group => {
        const evidenceTitle = group[0].evidenceTitle || "Улика";
        const phraseButtons = group.map(phrase => `
            <button class="phrase-button" type="button" data-phrase-id="${phrase.id}">
                ${html(phrase.text)}
            </button>
        `).join("");
        return `<div class="notebook-group"><strong>${html(evidenceTitle)}</strong>${phraseButtons}</div>`;
    }).join("");
}

async function processPhrase(phraseId) {
    if (!interrogation.sessionId) return;

    try {
        const result = await api(`/api/Interrogation/phrase/${interrogation.sessionId}`, {
            method: "POST",
            body: JSON.stringify({ phraseId })
        });

        if (result.error) {
            status(result.error, true);
            return;
        }

        document.querySelector("[data-detective-speech]").hidden = false;
        document.querySelector("[data-detective-speech]").textContent = "В блокноте отмечена выбранная улика.";
        document.querySelector("[data-suspect-speech]").textContent = result.replyText || "Подозреваемый не отвечает.";
        renderState(result);

        if (result.status && result.status !== "InProgress") {
            await finishSuspect();
            return;
        }

        await loadPhrases(interrogation.sessionId);
        status("");
    } catch (error) {
        status(error.message, true);
    }
}

async function finishSuspect() {
    if (interrogation.sessionId) {
        try {
            await api(`/api/Interrogation/end/${interrogation.sessionId}`, { method: "POST" });
        } catch {
        }
    }

    if (interrogation.index < interrogation.suspects.length - 1) {
        setCurrentSuspect(interrogation.index + 1);
        document.querySelector("[data-suspect-speech]").textContent = "Следующий подозреваемый входит в комнату.";
    } else {
        window.location.href = "/ending";
    }
}

async function loadEnding() {
    if (!document.querySelector("[data-ending-page]")) return;
    if (!requireAuth()) return;

    try {
        const item = await ensureCaseForRoom();
        const ending = await api(`/api/Interrogation/ending/${item.id}`);
        const title = document.querySelector("[data-ending-title]");
        const text = document.querySelector("[data-ending-text]");

        if (title) {
            title.textContent = ending.resultTitle || "Судебное решение";
        }

        if (text) {
            const achievements = (ending.achievements || []).length
                ? `<p><strong>Достижения:</strong> ${ending.achievements.map(html).join(", ")}</p>`
                : "";

            text.innerHTML = `
                <p>${html(ending.resultText || "Расследование завершено. Материалы переданы в суд.").replace(/\n/g, "<br>")}</p>
                ${achievements}
            `;
        }

        status("");
    } catch (error) {
        status(error.message, true);
    }
}

async function loadProfile() {
    if (!document.querySelector("[data-profile-page]")) return;
    if (!requireAuth()) return;

    document.querySelector("[data-profile-name]").textContent = claim("name") || "Детектив";
    document.querySelector("[data-profile-email]").textContent = claim("emailaddress") || claim("email") || "email не указан";

    try {
        const achievements = await api("/api/Achievement/user");
        document.querySelector("[data-achievements-list]").innerHTML = achievements.length
            ? achievements.map(item => `<div class="achievement-item"><strong>${html(item.title)}</strong><br>${html(item.description || "")}</div>`).join("")
            : `<div class="achievement-item">Достижения пока не получены.</div>`;
    } catch (error) {
        status(error.message, true);
    }
}

async function loadAdmin() {
    if (!document.querySelector("[data-admin-stats]")) return;
    if (!requireAuth()) return;

    if (getRole() !== "Admin") {
        status("Доступ только для администратора.", true);
        return;
    }

    try {
        const [stats, users, achievements] = await Promise.all([
            api("/api/Admin/statistics"),
            api("/api/Admin/users"),
            api("/api/Achievement/all")
        ]);
        document.querySelector("[data-admin-stats]").innerHTML = stats.map(item => `<div class="stat-row"><strong>${html(item.value)}</strong> ${html(item.label)}</div>`).join("");
        document.querySelector("[data-admin-users]").innerHTML = users.map(user => `
            <div class="admin-row">
                <div><strong>${html(user.username)}</strong><br>${html(user.email)} · ${html(user.role)}</div>
                <button class="danger-action" type="button" data-delete-user="${user.id}">Удалить</button>
            </div>
        `).join("");
        document.querySelector("[data-admin-achievements]").innerHTML = achievements.map(item => `
            <div class="admin-row">
                <div><strong>${html(item.title)}</strong><br>${html(item.description || "")}</div>
                <button class="secondary-action" type="button" data-edit-achievement="${item.id}">Редактировать</button>
            </div>
        `).join("");
        status("");
    } catch (error) {
        status(error.message, true);
    }
}

document.addEventListener("click", event => {
    if (event.target.closest("[data-logout]")) {
        clearToken();
        window.location.href = "/";
    }

    if (event.target.closest("[data-investigate]")) {
        if (!isAuthenticated()) {
            status("Пользователь не авторизован.", true);
            return;
        }
        window.location.href = "/desk";
    }

    if (event.target.closest("[data-next-page]")) {
        const current = Number(document.querySelector("[data-case-file]").dataset.spread || 0);
        showDeskSpread(Math.min(current + 1, 2));
    }

    if (event.target.closest("[data-prev-page]")) {
        const current = Number(document.querySelector("[data-case-file]").dataset.spread || 0);
        showDeskSpread(Math.max(current - 1, 0));
    }

    if (event.target.closest("[data-enter-interrogation]")) {
        window.location.href = "/interrogation/room";
    }

    if (event.target.closest("[data-start-greeting]")) {
        startCurrentSuspect();
    }

    const phrase = event.target.closest("[data-phrase-id]");
    if (phrase) {
        processPhrase(Number(phrase.dataset.phraseId));
    }

    if (event.target.closest("[data-finish-suspect]")) {
        finishSuspect();
    }

    if (event.target.closest("[data-refresh-admin]")) {
        loadAdmin();
    }

    const deleteUser = event.target.closest("[data-delete-user]");
    if (deleteUser) {
        const userId = Number(deleteUser.dataset.deleteUser);
        if (!userId) return;
        api(`/api/Admin/users/${userId}`, { method: "DELETE" })
            .then(() => {
                status("Пользователь удалён.");
                loadAdmin();
            })
            .catch(error => status(error.message, true));
    }

    if (event.target.closest("[data-edit-achievement]")) {
        status("Редактирование достижений будет открыто в следующем шаге.", false);
    }
});

document.addEventListener("DOMContentLoaded", () => {
    updateHomeNav();
    loadHome();
    initAuth();
    loadDesk();
    initInterrogationRoom();
    loadEnding();
    loadProfile();
    loadAdmin();
});
