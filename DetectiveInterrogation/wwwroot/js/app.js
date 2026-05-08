document.addEventListener("click", event => {
    if (event.target.closest("[data-logout]")) {
        api("/api/Auth/logout", { method: "POST" })
            .catch(() => {})
            .finally(() => {
                clearAuth();
                window.location.href = "/";
            });
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

document.addEventListener("DOMContentLoaded", async () => {
    await refreshCurrentUser();
    initAuth();
    loadDesk();
    initInterrogationRoom();
    loadEnding();
    loadProfile();
    loadAdmin();
});
