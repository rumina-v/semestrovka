async function getMainCase() {
    const cases = await api("/api/Case");
    const item = cases[0];
    if (!item) throw new Error("Дело не найдено.");
    return await api(`/api/Case/${item.id}`);
}

function formatText(value) {
    return (value || "")
        .split(/\n{2,}/)
        .map(part => part.trim())
        .filter(Boolean)
        .map(part => `<p>${html(part)}</p>`)
        .join("");
}

function renderSuspect(selector, titleSelector, suspect, fallback) {
    const root = document.querySelector(selector);
    const title = document.querySelector(titleSelector);
    if (!root) return;

    if (!suspect) {
        if (title) title.textContent = "Подозреваемый";
        root.innerHTML = `<p>${html(fallback)}</p>`;
        return;
    }

    const description = suspect.description || "Описание отсутствует.";
    const firstSentenceMatch = description.match(/^(.+?\.)\s*/s);
    const heading = firstSentenceMatch ? firstSentenceMatch[1].replace(/\.$/, "") : suspect.name;
    const body = firstSentenceMatch ? description.slice(firstSentenceMatch[0].length).trim() : description;

    if (title) title.textContent = heading;
    root.innerHTML = `<div class="suspect-text">${formatText(body || description)}</div>`;
}

async function loadDesk() {
    if (!document.querySelector("[data-desk-page]")) return;
    if (!requireAuth()) return;

    try {
        const item = await getMainCase();
        sessionStorage.setItem(caseKey, JSON.stringify(item));

        document.querySelector("[data-desk-title]").textContent = item.title || "Убийство судьи";
        document.querySelector("[data-desk-description]").textContent = item.shortDescription || "Материалы дела пока не заполнены.";

        const suspects = item.suspects || [];
        renderSuspect("[data-suspect-one]", "[data-suspect-one-title]", suspects[0], "Подозреваемый №1 не найден.");
        renderSuspect("[data-suspect-two]", "[data-suspect-two-title]", suspects[1], "Подозреваемый №2 не найден.");

        const evidence = item.evidence || [];
        document.querySelector("[data-desk-evidence]").innerHTML = evidence.length
            ? evidence.map(e => `<div class="evidence-item"><strong>${html(e.title)}</strong><br>${html(e.description || "")}</div>`).join("")
            : `<div class="evidence-item">Улики не добавлены.</div>`;
    } catch (error) {
        status(error.message, true);
    }
}

function showDeskSpread(index) {
    const file = document.querySelector("[data-case-file]");
    if (!file) return;
    file.dataset.spread = String(index);
    const desk = document.querySelector("[data-desk-page]");
    if (desk) desk.dataset.deskSpread = String(index);
    document.querySelectorAll("[data-spread-panel]").forEach(panel => {
        panel.hidden = Number(panel.dataset.spreadPanel) !== index;
    });
}

function getSavedCase() {
    const raw = sessionStorage.getItem(caseKey);
    return raw ? JSON.parse(raw) : null;
}

async function ensureCaseForRoom() {
    return getSavedCase() || await getMainCase();
}
