async function loadEnding() {
    if (!document.querySelector("[data-ending-page]")) return;
    if (!requireAuth()) return;

    try {
        const item = await ensureCaseForRoom();
        const ending = await api(`/api/Interrogation/ending/${item.id}`);
        const title = document.querySelector("[data-ending-title]");
        const text = document.querySelector("[data-ending-text]");

        if (title) title.textContent = ending.resultTitle || "Судебное решение";
        if (text) {
            const fallback = "Расследование завершено. Материалы переданы в суд.";
            text.innerHTML = `<p>${html(ending.resultText || fallback).replace(/\n/g, "<br>")}</p>`;
        }

        const courtImage = document.querySelector("[data-court-image]");
        const prisonImage = document.querySelector("[data-prison-image]");
        if (courtImage && ending.courtImagePath) {
            courtImage.style.backgroundImage = `linear-gradient(rgba(33, 27, 22, 0.08), rgba(33, 27, 22, 0.22)), url("${ending.courtImagePath}")`;
        }
        if (prisonImage && ending.prisonImagePath) {
            prisonImage.style.backgroundImage = `linear-gradient(rgba(33, 27, 22, 0.08), rgba(33, 27, 22, 0.22)), url("${ending.prisonImagePath}")`;
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
