const state = {
    resources: [],
    apus: [],
    concepts: [],
    budgets: []
};

const money = value => new Intl.NumberFormat("es-ES", {
    style: "currency",
    currency: "EUR"
}).format(value);

function message(id, text, kind = "") {
    const element = document.querySelector(`#${id}`);
    element.textContent = text;
    element.className = `message ${kind}`.trim();
}

async function problem(response) {
    try {
        const data = await response.json();
        return data.title ?? "La operación no pudo completarse.";
    } catch {
        return "La operación no pudo completarse.";
    }
}

async function request(url, options) {
    const response = await fetch(url, options);

    if (!response.ok) {
        throw new Error(await problem(response));
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}

function fillSelect(id, items, label) {
    const select = document.querySelector(`#${id}`);
    select.replaceChildren();

    for (const item of items) {
        const option = document.createElement("option");
        option.value = item.id;
        option.textContent = label(item);
        select.appendChild(option);
    }
}

async function loadResources() {
    state.resources = await request("/resources");

    const body = document.querySelector("#resource-body");
    body.replaceChildren();

    for (const item of state.resources) {
        const row = document.createElement("tr");

        for (const value of [
            item.key,
            item.name,
            item.type,
            item.unit,
            money(item.price)
        ]) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.appendChild(cell);
        }

        const actions = document.createElement("td");
        actions.className = "actions";

        const editButton = document.createElement("button");
        editButton.className = "secondary compact";
        editButton.textContent = "Editar";
        editButton.addEventListener("click", () => editResource(item));

        actions.appendChild(editButton);
        row.appendChild(actions);
        body.appendChild(row);
    }

    fillSelect("component-resource", state.resources, item => `${item.key} — ${item.name}`);
}

async function editResource(resource) {
    const name = prompt("Nombre del recurso:", resource.name);
    if (name === null) return;

    const priceInput = prompt("Precio:", resource.price);
    if (priceInput === null) return;

    const active = confirm("Aceptar para dejar el recurso activo. Cancelar para inactivarlo.");

    await request(`/resources/${resource.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            name: name.trim(),
            price: Number(priceInput),
            isActive: active
        })
    });

    await loadResources();
}

async function loadApus() {
    state.apus = await request("/apus");

    const list = document.querySelector("#apu-list");
    list.replaceChildren();

    for (const apu of state.apus) {
        const card = document.createElement("article");
        card.className = "card";

        const title = document.createElement("h3");
        title.textContent = `${apu.key} — ${apu.name}`;
        card.appendChild(title);

        const summary = document.createElement("p");
        summary.innerHTML = `Unidad: <strong>${apu.unit}</strong> · Costo directo: <strong>${money(apu.directCost)}</strong>`;
        card.appendChild(summary);

        const items = document.createElement("div");
        items.className = "item-list";

        for (const component of apu.components ?? []) {
            const row = document.createElement("div");
            row.className = "item-row";

            const description = document.createElement("span");
            description.textContent =
                `${component.resourceKey} — ${component.resourceName} · ${component.quantity} × ${money(component.unitPrice)}`;

            const remove = document.createElement("button");
            remove.className = "danger compact";
            remove.textContent = "Eliminar";
            remove.addEventListener("click", async () => {
                if (!confirm("¿Eliminar este componente?")) return;

                await request(`/apus/${apu.id}/components/${component.id}`, {
                    method: "DELETE"
                });

                await loadApus();
            });

            row.appendChild(description);
            row.appendChild(remove);
            items.appendChild(row);
        }

        card.appendChild(items);
        list.appendChild(card);
    }

    fillSelect("component-apu", state.apus, item => `${item.key} — ${item.name}`);
    fillSelect("concept-apu", state.apus, item => `${item.key} — ${item.name}`);
}

async function loadConcepts() {
    state.concepts = await request("/concepts");

    const body = document.querySelector("#concept-body");
    body.replaceChildren();

    for (const item of state.concepts) {
        const row = document.createElement("tr");

        for (const value of [
            item.key,
            item.name,
            money(item.directCost),
            money(item.unitPrice)
        ]) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.appendChild(cell);
        }

        body.appendChild(row);
    }

    fillSelect("percentages-concept", state.concepts, item => `${item.key} — ${item.name}`);
    fillSelect("item-concept", state.concepts, item => `${item.key} — ${item.name}`);
}

async function loadBudgets() {
    state.budgets = await request("/budgets");

    const list = document.querySelector("#budget-list");
    list.replaceChildren();

    for (const budget of state.budgets) {
        const card = document.createElement("article");
        card.className = "card";

        const title = document.createElement("h3");
        title.textContent = `${budget.key} — ${budget.name}`;
        card.appendChild(title);

        const summary = document.createElement("p");
        summary.innerHTML = `Partidas: <strong>${budget.items?.length ?? 0}</strong> · Total: <strong>${money(budget.total)}</strong>`;
        card.appendChild(summary);

        const items = document.createElement("div");
        items.className = "item-list";

        for (const item of budget.items ?? []) {
            const row = document.createElement("div");
            row.className = "item-row";

            const description = document.createElement("span");
            description.textContent =
                `${item.conceptKey} — ${item.conceptName} · ${item.quantity} × ${money(item.unitPrice)} = ${money(item.total)}`;

            const quantity = document.createElement("button");
            quantity.className = "secondary compact";
            quantity.textContent = "Cantidad";
            quantity.addEventListener("click", async () => {
                const value = prompt("Nueva cantidad:", item.quantity);
                if (value === null) return;

                await request(`/budgets/${budget.id}/items/${item.id}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ quantity: Number(value) })
                });

                await loadBudgets();
            });

            const remove = document.createElement("button");
            remove.className = "danger compact";
            remove.textContent = "Eliminar";
            remove.addEventListener("click", async () => {
                if (!confirm("¿Eliminar esta partida?")) return;

                await request(`/budgets/${budget.id}/items/${item.id}`, {
                    method: "DELETE"
                });

                await loadBudgets();
            });

            row.appendChild(description);
            row.appendChild(quantity);
            row.appendChild(remove);
            items.appendChild(row);
        }

        card.appendChild(items);
        list.appendChild(card);
    }

    fillSelect("item-budget", state.budgets, item => `${item.key} — ${item.name}`);
}

async function loadAll() {
    try {
        await loadResources();
        await loadApus();
        await loadConcepts();
        await loadBudgets();
        document.querySelector("#status").textContent = "API disponible";
    } catch (error) {
        document.querySelector("#status").textContent = "API no disponible";
        console.error(error);
    }
}

document.querySelectorAll(".tab").forEach(button => {
    button.addEventListener("click", () => {
        document.querySelectorAll(".tab").forEach(tab => tab.classList.remove("active"));
        document.querySelectorAll(".view").forEach(view => view.classList.remove("active"));

        button.classList.add("active");
        document.querySelector(`#${button.dataset.view}`).classList.add("active");
    });
});

document.querySelectorAll(".reload").forEach(button => {
    button.addEventListener("click", async () => {
        button.disabled = true;

        try {
            await ({
                resources: loadResources,
                apus: loadApus,
                concepts: loadConcepts,
                budgets: loadBudgets
            })[button.dataset.load]();
        } finally {
            button.disabled = false;
        }
    });
});

document.querySelector("#resource-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        await request("/resources", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                key: document.querySelector("#resource-key").value.trim(),
                name: document.querySelector("#resource-name").value.trim(),
                type: Number(document.querySelector("#resource-type").value),
                unitCode: document.querySelector("#resource-unit-code").value.trim(),
                unitSymbol: document.querySelector("#resource-unit-symbol").value.trim(),
                unitName: document.querySelector("#resource-unit-name").value.trim(),
                price: Number(document.querySelector("#resource-price").value)
            })
        });

        event.target.reset();
        message("resource-message", "Recurso guardado.", "success");
        await loadResources();
    } catch (error) {
        message("resource-message", error.message, "error");
    }
});

document.querySelector("#apu-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        await request("/apus", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                key: document.querySelector("#apu-key").value.trim(),
                name: document.querySelector("#apu-name").value.trim(),
                unitCode: document.querySelector("#apu-unit-code").value.trim(),
                unitSymbol: document.querySelector("#apu-unit-symbol").value.trim(),
                unitName: document.querySelector("#apu-unit-name").value.trim()
            })
        });

        event.target.reset();
        message("apu-message", "APU guardado.", "success");
        await loadApus();
    } catch (error) {
        message("apu-message", error.message, "error");
    }
});

document.querySelector("#component-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        const apuId = document.querySelector("#component-apu").value;

        await request(`/apus/${apuId}/components`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                resourceId: document.querySelector("#component-resource").value,
                quantity: Number(document.querySelector("#component-quantity").value)
            })
        });

        event.target.reset();
        message("component-message", "Componente agregado.", "success");
        await loadApus();
    } catch (error) {
        message("component-message", error.message, "error");
    }
});

document.querySelector("#concept-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        await request("/concepts", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                key: document.querySelector("#concept-key").value.trim(),
                name: document.querySelector("#concept-name").value.trim(),
                unitCode: document.querySelector("#concept-unit-code").value.trim(),
                unitSymbol: document.querySelector("#concept-unit-symbol").value.trim(),
                unitName: document.querySelector("#concept-unit-name").value.trim(),
                apuId: document.querySelector("#concept-apu").value
            })
        });

        event.target.reset();
        message("concept-message", "Concepto guardado.", "success");
        await loadConcepts();
    } catch (error) {
        message("concept-message", error.message, "error");
    }
});

document.querySelector("#percentages-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        const conceptId = document.querySelector("#percentages-concept").value;

        await request(`/concepts/${conceptId}/percentages`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                indirectCost: Number(document.querySelector("#indirect-cost").value),
                financing: Number(document.querySelector("#financing").value),
                profit: Number(document.querySelector("#profit").value),
                additionalCharges: Number(document.querySelector("#additional-charges").value)
            })
        });

        message("percentages-message", "Porcentajes actualizados.", "success");
        await loadConcepts();
    } catch (error) {
        message("percentages-message", error.message, "error");
    }
});

document.querySelector("#budget-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        await request("/budgets", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                key: document.querySelector("#budget-key").value.trim(),
                name: document.querySelector("#budget-name").value.trim()
            })
        });

        event.target.reset();
        message("budget-message", "Presupuesto guardado.", "success");
        await loadBudgets();
    } catch (error) {
        message("budget-message", error.message, "error");
    }
});

document.querySelector("#budget-item-form").addEventListener("submit", async event => {
    event.preventDefault();

    try {
        const budgetId = document.querySelector("#item-budget").value;

        await request(`/budgets/${budgetId}/items`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                conceptId: document.querySelector("#item-concept").value,
                quantity: Number(document.querySelector("#item-quantity").value)
            })
        });

        event.target.reset();
        message("budget-item-message", "Partida agregada.", "success");
        await loadBudgets();
    } catch (error) {
        message("budget-item-message", error.message, "error");
    }
});


document.querySelector("#resource-import-form").addEventListener("submit", async event => {
    event.preventDefault();

    const fileInput = document.querySelector("#resource-import-file");
    const file = fileInput.files[0];

    if (!file) {
        message("resource-import-message", "Selecciona un archivo CSV.", "error");
        return;
    }

    const formData = new FormData();
    formData.append("file", file);

    try {
        const result = await request("/imports/resources.csv", {
            method: "POST",
            body: formData
        });

        const detail = result.rejected > 0
            ? ` Importados: ${result.imported}. Rechazados: ${result.rejected}.`
            : ` Importados: ${result.imported}.`;

        message(
            "resource-import-message",
            `Importación completada.${detail}`,
            result.rejected > 0 ? "error" : "success");

        event.target.reset();
        await loadResources();
    } catch (error) {
        message("resource-import-message", error.message, "error");
    }
});
loadAll();

