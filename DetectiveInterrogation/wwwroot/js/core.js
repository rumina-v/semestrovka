const userKey = "detective.user";
const caseKey = "detective.case";

function getCurrentUser() {
    try {
        const user = sessionStorage.getItem(userKey);
        return user ? JSON.parse(user) : null;
    } catch {
        return null;
    }
}

function setCurrentUser(user) {
    if (user) {
        sessionStorage.setItem(userKey, JSON.stringify(user));
    } else {
        sessionStorage.removeItem(userKey);
    }
}

function clearAuth() {
    sessionStorage.removeItem(userKey);
    sessionStorage.removeItem(caseKey);
}

function claims() {
    return getCurrentUser() || {};
}

function claim(name) {
    const data = claims();
    if (name === "name") return data.username;
    if (name === "emailaddress") return data.email;
    return data[name];
}

function getRole() {
    return claim("role");
}

async function api(path, options = {}) {
    const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
    const response = await fetch(path, { ...options, headers, credentials: "same-origin" });
    const text = await response.text();
    const data = text ? JSON.parse(text) : null;

    if (!response.ok) {
        if (response.status === 401) clearAuth();
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
    return Boolean(getCurrentUser());
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

async function refreshCurrentUser() {
    try {
        const response = await fetch("/api/Auth/me", { credentials: "same-origin" });
        if (!response.ok) {
            clearAuth();
            return null;
        }

        const data = await response.json();
        setCurrentUser(data.user);
        return data.user;
    } catch {
        clearAuth();
        return null;
    } finally {
        updateHomeNav();
    }
}
