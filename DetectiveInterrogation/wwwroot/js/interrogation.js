const interrogation = {
    case: null,
    suspects: [],
    index: 0,
    sessionId: null,
    active: false,
    state: null
};

const interrogationGreetings = {
    jeffrey: {
        detective: "Здравствуйте, Мистер Робинсон. Вчера ночью была убита ваша жена, Мари Робинсон. У следствия есть к вам несколько вопросов. Пожалуйста, помните что ложь и увиливание на допросе будут восприняты как препятствование расследованию. Расскажите что случилось в тот день.",
        suspect: "Здравствуйте. В тот день утром мы с Мари поссорились. Это пустяки, просто недопонимания. И к обеду я уехал на работу. Потом чтобы не продолжать ссору, ночью не вернулся, а утром мне позвонили из полиции и сказали приехать для дачи показаний. Узнал все я от них, потом прочел газету и знаете... До сих пор не могу осознать, что это правда, моей Мари больше нет."
    },
    lilith: {
        detective: "Здравствуйте, Мадам Маккэри. Вчера ночью была убита ваша соседка, Мари Робинсон. У следствия есть к вам несколько вопросов. Пожалуйста, помните что ложь и увиливание на допросе будут восприняты как препятствование расследованию. Расскажите что случилось в тот день.",
        suspect: "Добрый день, да.. уже видела новость об этой ужасной трагедии. В тот день я приходила к Мари, чтобы поговорить о старом деле. Дома была странная атмосфера, она была на нервах, все вокруг было в каких то клочках бумаги. Ну, в общем, как и раньше, не пришли ни к какому согласию. Она не хотела меня слушать, а я уже слишком устала от этих разговоров. После этого вернулась домой. Больше я к ней в тот день не возвращалась."
    }
};

function suspectSortValue(suspect) {
    if (!suspect) return 2;
    return suspect.isGuilty ? 1 : 0;
}

function getGreetingForSuspect(suspect) {
    const name = (suspect?.name || "").toLowerCase();
    if (name.includes("джеффри") || name.includes("робинсон")) return interrogationGreetings.jeffrey;
    if (name.includes("лилит") || name.includes("маккэри")) return interrogationGreetings.lilith;
    return interrogation.index === 0 ? interrogationGreetings.jeffrey : interrogationGreetings.lilith;
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
            .sort((left, right) => suspectSortValue(left) - suspectSortValue(right) || left.id - right.id);
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
    const suspectCounter = document.querySelector("[data-suspect-counter]");
    if (suspectCounter) {
        suspectCounter.hidden = true;
        suspectCounter.textContent = "";
    }

    document.querySelector("[data-suspect-speech]").textContent = "Подозреваемый молча ждёт начала разговора.";
    document.querySelector("[data-detective-speech]").hidden = true;
    document.querySelector("[data-notebook]").hidden = true;
    const greetingButton = document.querySelector("[data-start-greeting]");
    greetingButton.hidden = false;
    greetingButton.textContent = "Начать приветствие";
    status("");
}

async function startCurrentSuspect() {
    const suspect = interrogation.suspects[interrogation.index];
    if (!suspect || interrogation.active) return;

    const greeting = getGreetingForSuspect(suspect);
    const greetingButton = document.querySelector("[data-start-greeting]");
    const detectiveSpeech = document.querySelector("[data-detective-speech]");
    const suspectSpeech = document.querySelector("[data-suspect-speech]");
    const notebook = document.querySelector("[data-notebook]");

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
        if (room) room.dataset.phase = "active";

        notebook.hidden = false;
        renderState(session);
        await loadPhrases(session.id);
        status("");
    } catch (error) {
        greetingButton.hidden = false;
        status(error.message, true);
    }
}

function renderState(state) {
    interrogation.state = {
        currentTrust: state.currentTrust ?? 0,
        currentPressure: state.currentPressure ?? 0,
        status: state.status || "InProgress"
    };
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
        const phraseButtons = group.map(phrase => `
            <button class="phrase-button" type="button" data-phrase-id="${phrase.id}">
                ${html(phrase.text)}
            </button>
        `).join("");
        return `<div class="notebook-group">${phraseButtons}</div>`;
    }).join("");
}

async function processPhrase(phraseId) {
    if (!interrogation.sessionId) return;

    try {
        const selectedPhrase = document.querySelector(`[data-phrase-id="${phraseId}"]`)?.textContent?.trim() || "";
        const result = await api(`/api/Interrogation/phrase/${interrogation.sessionId}`, {
            method: "POST",
            body: JSON.stringify({ phraseId })
        });

        if (result.error) {
            status(result.error, true);
            return;
        }

        document.querySelector("[data-detective-speech]").hidden = false;
        document.querySelector("[data-detective-speech]").textContent = result.phraseText || selectedPhrase;
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
