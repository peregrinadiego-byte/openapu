const ux = {
    messageElement: document.querySelector("#global-message"),

    setMessage(text, kind = "") {
        this.messageElement.textContent = text;
        this.messageElement.className =
            `global-message ${kind}`.trim();
    },

    async refreshSummary() {
        try {
            const [resources, apus, concepts, budgets] =
                await Promise.all([
                    fetch("/resources").then(this.readJson),
                    fetch("/apus").then(this.readJson),
                    fetch("/concepts").then(this.readJson),
                    fetch("/budgets").then(this.readJson)
                ]);

            document.querySelector("#summary-resources").textContent =
                resources.length;

            document.querySelector("#summary-apus").textContent =
                apus.length;

            document.querySelector("#summary-concepts").textContent =
                concepts.length;

            document.querySelector("#summary-budgets").textContent =
                budgets.length;

            const total = budgets.reduce(
                (sum, budget) => sum + Number(budget.total ?? 0),
                0);

            document.querySelector("#summary-total").textContent =
                new Intl.NumberFormat("es-MX", {
                    style: "currency",
                    currency: "MXN"
                }).format(total);
        } catch {
            this.setMessage(
                "No fue posible actualizar el resumen general.",
                "error");
        }
    },

    async readJson(response) {
        if (!response.ok) {
            throw new Error("Request failed.");
        }

        return response.json();
    }
};

function persistNavigation() {
    const savedView = localStorage.getItem("openapu.activeView");
    const initialTab = savedView
        ? document.querySelector(`.tab[data-view="${savedView}"]`)
        : null;

    if (initialTab) {
        initialTab.click();
    }

    document.querySelectorAll(".tab").forEach(tab => {
        tab.addEventListener("click", () => {
            localStorage.setItem(
                "openapu.activeView",
                tab.dataset.view);
        });
    });
}

function clearValidation(form) {
    form.querySelectorAll(".invalid").forEach(element => {
        element.classList.remove("invalid");
    });

    form.querySelectorAll(".field-error").forEach(element => {
        element.remove();
    });
}

function validateForm(form) {
    clearValidation(form);

    let valid = true;

    for (const field of form.querySelectorAll(
        "input[required], select[required]")) {
        if (field.value.trim() !== "") {
            continue;
        }

        valid = false;
        field.classList.add("invalid");

        const error = document.createElement("span");
        error.className = "field-error";
        error.textContent = "Este campo es obligatorio.";

        field.insertAdjacentElement("afterend", error);
    }

    for (const field of form.querySelectorAll(
        'input[type="number"]')) {
        if (field.value === "") {
            continue;
        }

        const value = Number(field.value);
        const minimum = field.min === ""
            ? null
            : Number(field.min);

        if (!Number.isFinite(value) ||
            (minimum !== null && value < minimum)) {
            valid = false;
            field.classList.add("invalid");

            const error = document.createElement("span");
            error.className = "field-error";
            error.textContent = "Introduce un valor válido.";

            field.insertAdjacentElement("afterend", error);
        }
    }

    return valid;
}

function enhanceForms() {
    document.querySelectorAll("form").forEach(form => {
        form.setAttribute("novalidate", "novalidate");

        form.addEventListener(
            "submit",
            event => {
                if (!validateForm(form)) {
                    event.preventDefault();
                    event.stopImmediatePropagation();

                    ux.setMessage(
                        "Revisa los campos marcados antes de continuar.",
                        "error");
                }
            },
            true);

        form.addEventListener("submit", () => {
            const button = form.querySelector(
                'button[type="submit"]');

            if (!button) {
                return;
            }

            button.classList.add("busy");
            button.disabled = true;

            ux.setMessage(
                "Procesando operación…",
                "loading");

            window.setTimeout(() => {
                button.classList.remove("busy");
                button.disabled = false;
            }, 1500);
        });
    });
}

function observeUpdates() {
    const observer = new MutationObserver(() => {
        const success = document.querySelector(
            ".message.success:not(:empty)");

        const error = document.querySelector(
            ".message.error:not(:empty)");

        if (error) {
            ux.setMessage(error.textContent, "error");
            return;
        }

        if (success) {
            ux.setMessage(success.textContent, "success");
            ux.refreshSummary();
        }
    });

    document.querySelectorAll(".message").forEach(element => {
        observer.observe(element, {
            childList: true,
            characterData: true,
            subtree: true
        });
    });
}

function handleUnhandledErrors() {
    window.addEventListener("unhandledrejection", event => {
        ux.setMessage(
            event.reason?.message ??
                "La operación no pudo completarse.",
            "error");
    });
}

persistNavigation();
enhanceForms();
observeUpdates();
handleUnhandledErrors();
ux.refreshSummary();
