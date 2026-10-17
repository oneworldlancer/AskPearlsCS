
    (function() {
        "use strict";

    // Prevent duplicate initialization if this partial is loaded again by AJAX.
    if (window.askPearlsRecommendationItemsInitialized) {
            if (typeof window.askPearlsRecommendationItemsRefresh === "function") {
        window.askPearlsRecommendationItemsRefresh();
            }
    return;
        }

    window.askPearlsRecommendationItemsInitialized = true;

    // Temporary UI-only data. Replace with API data after all portal pages are complete.
    let recommendationItems = [
    {
        id: 1,
    name: "Oslo Sofa",
    key: "oslo_sofa",
    description: "A compact, practical sofa for everyday comfort.",
    imageUrl: "",
    active: true
            },
    {
        id: 2,
    name: "Luna Sofa",
    key: "luna_sofa",
    description: "A contemporary sofa with a style-focused design.",
    imageUrl: "",
    active: true
            },
    {
        id: 3,
    name: "Haven Sofa",
    key: "haven_sofa",
    description: "A spacious option for larger living areas.",
    imageUrl: "",
    active: true
            }
    ];

    let editingId = null;

        const $ = id => document.getElementById(id);

    const elements = {
        body: $("riTableBody"),
    search: $("riSearch"),
    filter: $("riStatusFilter"),
    total: $("riTotalCount"),
    active: $("riActiveCount"),
    results: $("riResultsCount"),
    empty: $("riEmptyState"),
    backdrop: $("riModalBackdrop"),
    form: $("riForm"),
    title: $("riModalTitle"),
    name: $("riName"),
    key: $("riKey"),
    description: $("riDescription"),
    imageUrl: $("riImageUrl"),
    activeInput: $("riActive"),
    error: $("riFormError"),
    save: $("riSaveButton")
        };

    function escapeHtml(value) {
            return String(value ?? "").replace(/[&<>"']/g, character => ({
        "&": "&amp;",
        "<": "&lt;",
                ">": "&gt;",
        '"': "&quot;",
        "'": "&#39;"
            })[character]);
        }

        function render() {
            const searchValue = elements.search.value.trim().toLowerCase();
        const statusValue = elements.filter.value;

            const filteredItems = recommendationItems.filter(item => {
                const matchesSearch =
        (item.name + " " + item.key + " " + item.description)
        .toLowerCase().includes(searchValue);

        const matchesStatus =
        statusValue === "all" ||
        (statusValue === "active" && item.active) ||
        (statusValue === "inactive" && !item.active);

        return matchesSearch && matchesStatus;
            });

        elements.total.textContent = recommendationItems.length;
        elements.active.textContent =
                recommendationItems.filter(item => item.active).length;

        elements.results.textContent =
        `Showing ${ filteredItems.length } of ${ recommendationItems.length } items`;

        elements.empty.classList.toggle("d-none", filteredItems.length !== 0);

            elements.body.innerHTML = filteredItems.map(item => `
        <tr>
            <td data-label="Recommendation Item">
                <div class="ri-item-name">
                    <div class="ri-item-icon">
                        <i class="bi bi-box-seam"></i>
                    </div>
                    <div>
                        <div class="ri-item-title">${ escapeHtml(item.name) }</div>
                        <div class="ri-item-subtitle">Recommendation outcome</div>
                    </div>
                </div>
            </td>
            <td data-label="Identifier">
                <code class="ri-key">${ escapeHtml(item.key) }</code>
            </td>
            <td data-label="Description">
                <span>${ escapeHtml(item.description || "—") }</span>
            </td>
            <td data-label="Status">
                <span class="ri-status-badge ${item.active ? " ri-status-active" : "ri-status-inactive"}">
                ${ item.active ? "Active" : "Inactive" }
            </span>
        </td>
        <td data-label="Actions">
            <div class="ri-action-group">
                <button type="button"
                    class="ri-action-button"
                    data-ri-action="edit"
                    data-ri-id="${item.id}">
                    <i class="bi bi-pencil"></i> Edit
                </button>
                <button type="button"
                    class="ri-action-button"
                    data-ri-action="toggle"
                    data-ri-id="${item.id}">
                    <i class="bi bi-${item.active ? " pause-circle" : "play-circle"}"></i>
                ${ item.active ? "Deactivate" : "Activate" }
            </button>
        </div>
    </td>
</tr>
`).join("");
        }

        function openModal(item) {
            editingId = item ? item.id : null;

            elements.title.textContent =
                item ? "Edit Recommendation" : "Add Recommendation";

            elements.save.innerHTML = item
                ? '<i class="bi bi-check-lg"></i> Save Changes'
                : '<i class="bi bi-check-lg"></i> Save Recommendation';

            elements.name.value = item ? item.name : "";
            elements.key.value = item ? item.key : "";
            elements.description.value = item ? item.description : "";
            elements.imageUrl.value = item ? item.imageUrl : "";
            elements.activeInput.checked = item ? item.active : true;
            elements.error.textContent = "";

            elements.backdrop.classList.add("is-open");
            elements.backdrop.setAttribute("aria-hidden", "false");
            document.body.classList.add("ri-modal-open");

            requestAnimationFrame(() => elements.name.focus());
        }

        function closeModal() {
            elements.backdrop.classList.remove("is-open");
            elements.backdrop.setAttribute("aria-hidden", "true");
            document.body.classList.remove("ri-modal-open");
            elements.form.reset();
            elements.error.textContent = "";
            editingId = null;
        }

        function saveItem(event) {
            event.preventDefault();
            elements.error.textContent = "";

            const name = elements.name.value.trim();
            const key = elements.key.value.trim();
            const description = elements.description.value.trim();
            const imageUrl = elements.imageUrl.value.trim();

            if (!name || !key) {
                elements.error.textContent =
                    "Please enter both a recommendation name and identifier.";
                return;
            }

            const duplicate = recommendationItems.some(item =>
                item.key.toLowerCase() === key.toLowerCase() &&
                item.id !== editingId
            );

            if (duplicate) {
                elements.error.textContent =
                    "This identifier is already in use. Choose a unique identifier.";
                elements.key.focus();
                return;
            }

            const item = {
                id: editingId ?? Math.max(0, ...recommendationItems.map(x => x.id)) + 1,
                name,
                key,
                description,
                imageUrl,
                active: elements.activeInput.checked
            };

            if (editingId !== null) {
                recommendationItems = recommendationItems.map(existing =>
                    existing.id === editingId ? item : existing
                );
            } else {
                recommendationItems.push(item);
            }

            closeModal();
            render();
        }

        elements.search.addEventListener("input", render);
        elements.filter.addEventListener("change", render);
        elements.form.addEventListener("submit", saveItem);

        $("riAddButton").addEventListener("click", () => openModal(null));
        $("riEmptyAddButton").addEventListener("click", () => openModal(null));
        $("riModalClose").addEventListener("click", closeModal);
        $("riCancelButton").addEventListener("click", closeModal);

        elements.backdrop.addEventListener("click", event => {
            if (event.target === elements.backdrop) {
                closeModal();
            }
        });

        document.addEventListener("keydown", event => {
            if (event.key === "Escape" &&
                elements.backdrop.classList.contains("is-open")) {
                closeModal();
            }
        });

        elements.body.addEventListener("click", event => {
            const button = event.target.closest("[data-ri-action]");
            if (!button) return;

            const id = Number(button.dataset.riId);
            const item = recommendationItems.find(x => x.id === id);
            if (!item) return;

            if (button.dataset.riAction === "edit") {
                openModal(item);
            } else if (button.dataset.riAction === "toggle") {
                item.active = !item.active;
                render();
            }
        });

        window.askPearlsRecommendationItemsRefresh = render;

        render();
    })();

