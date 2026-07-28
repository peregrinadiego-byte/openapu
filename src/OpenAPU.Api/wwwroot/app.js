const form = document.querySelector("#resource-form");
const formMessage = document.querySelector("#form-message");
const resourceBody = document.querySelector("#resource-body");
const emptyState = document.querySelector("#empty-state");
const statusElement = document.querySelector("#status");
const reloadButton = document.querySelector("#reload");

const typeValues = {
    Material: 0,
    Labor: 1,
    Equipment: 2,
    Auxiliary: 3
};

function setMessage(text, kind = "") {
    formMessage.textContent = text;
    formMessage.className = `message ${kind}`.trim();
}

function formatPrice(value) {
    return new Intl.NumberFormat("es-MX", {
        style: "currency",
        currency: "MXN"
    }).format(value);
}

async function readProblem(response) {
    try {
        const body = await response.json();
        return body.title ?? "La operación no pudo completarse.";
    } catch {
        return "La operación no pudo completarse.";
    }
}

async function loadResources() {
    reloadButton.disabled = true;

    try {
        const response = await fetch("/resources");

        if (!response.ok) {
            throw new Error(await readProblem(response));
        }

        const resources = await response.json();

        resourceBody.replaceChildren();
        emptyState.hidden = resources.length !== 0;

        for (const resource of resources) {
            const row = document.createElement("tr");

            const values = [
                resource.key,
                resource.name,
                resource.type,
                resource.unit,
                formatPrice(resource.price),
                resource.status
            ];

            for (const value of values) {
                const cell = document.createElement("td");
                cell.textContent = value;
                row.appendChild(cell);
            }

            resourceBody.appendChild(row);
        }

        statusElement.textContent = "API disponible";
    } catch (error) {
        statusElement.textContent = "API no disponible";
        setMessage(error.message, "error");
    } finally {
        reloadButton.disabled = false;
    }
}

form.addEventListener("submit", async event => {
    event.preventDefault();
    setMessage("");

    const submitButton = form.querySelector('button[type="submit"]');
    submitButton.disabled = true;

    const type = document.querySelector("#type").value;

    const payload = {
        key: document.querySelector("#key").value.trim(),
        name: document.querySelector("#name").value.trim(),
        type: typeValues[type],
        unitCode: document.querySelector("#unitCode").value.trim(),
        unitSymbol: document.querySelector("#unitSymbol").value.trim(),
        unitName: document.querySelector("#unitName").value.trim(),
        price: Number(document.querySelector("#price").value)
    };

    try {
        const response = await fetch("/resources", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(await readProblem(response));
        }

        form.reset();
        setMessage("Recurso guardado.", "success");
        await loadResources();
    } catch (error) {
        setMessage(error.message, "error");
    } finally {
        submitButton.disabled = false;
    }
});

reloadButton.addEventListener("click", loadResources);

loadResources();
