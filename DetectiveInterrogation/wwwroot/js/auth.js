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
                setCurrentUser(result.user);
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
                setCurrentUser(result.user);
                window.location.href = "/";
            } catch (error) {
                status(error.message, true);
            }
        });
    }
}
