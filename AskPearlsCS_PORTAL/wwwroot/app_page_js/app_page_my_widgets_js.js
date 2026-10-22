(function () {
    "use strict";

    const root = document.querySelector('.page-widgets[data-ap-page="my-widgets"]');
    if (!root || root.dataset.widgetsInitialized === "true") return;
    root.dataset.widgetsInitialized = "true";

    const apiBase = (root.dataset.apiBase || "").replace(/\/+$/, "");
    const ownerToken = (root.dataset.demoOwnerUserToken || "").trim();
    const loading = root.querySelector("#widgetLoadingState");
    const error = root.querySelector("#widgetErrorState");
    const list = root.querySelector("#widgetInventoryList");
    const empty = root.querySelector("#widgetEmptyState");
    const select = root.querySelector("#activeWidgetSelect");
    const activeStatus = root.querySelector("#activeWidgetStatus");
    const activeVersion = root.querySelector("#activeWidgetVersion");
    const search = root.querySelector("#widgetSearch");
    const statusFilter = root.querySelector("#widgetStatusFilter");
    const counts = {
        all: root.querySelector("#widgetCountAll"),
        published: root.querySelector("#widgetCountPublished"),
        drafts: root.querySelector("#widgetCountDrafts")
    };
    let widgets = [];
    let selectedKey = "";

    function escapeHtml (value) {
        return String(value ?? "").replace(/[&<>"']/g, character => ({
            "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
        })[ character ]);
    }

    function value (item, camel, pascal) {
        return item[ camel ] ?? item[ pascal ];
    }

    function formatDate (raw) {
        if (!raw) return '<span class="widget-table-muted">—</span>';
        const date = new Date(raw);
        if (Number.isNaN(date.getTime())) return '<span class="widget-table-muted">—</span>';
        const absolute = new Intl.DateTimeFormat(undefined, { month: "short", day: "numeric", year: "numeric" }).format(date);
        const elapsed = Math.max(0, Date.now() - date.getTime());
        const minutes = Math.floor(elapsed / 60000);
        let relative = "just now";
        if (minutes >= 60) {
            const hours = Math.floor(minutes / 60);
            relative = hours < 24 ? `${ hours } ${ hours === 1 ? "hour" : "hours" } ago` : `${ Math.floor(hours / 24) } ${ Math.floor(hours / 24) === 1 ? "day" : "days" } ago`;
        } else if (minutes > 1) relative = `${ minutes } minutes ago`;
        return `<span>${ escapeHtml(absolute) }<small>${ escapeHtml(relative) }</small></span>`;
    }

    function statusLabel (status) {
        return status === "published" ? "Published" : status === "draft" ? "Draft" : "Unpublished";
    }

    function render () {
        if (counts.all) counts.all.textContent = String(widgets.length);
        if (counts.published) counts.published.textContent = String(widgets.filter(widget => widget.hasPublishedVersion).length);
        if (counts.drafts) counts.drafts.textContent = String(widgets.filter(widget => widget.hasDraft).length);

        const current = widgets.find(widget => widget.publicKey === selectedKey);
        if (select) {
            select.innerHTML = widgets.length
                ? widgets.map(widget => `<option value="${ escapeHtml(widget.publicKey) }">${ escapeHtml(widget.name) }</option>`).join("")
                : '<option value="">No widgets yet</option>';
            select.disabled = widgets.length === 0;
            select.value = current ? selectedKey : (widgets[ 0 ]?.publicKey || "");
            selectedKey = select.value;
        }
        const active = widgets.find(widget => widget.publicKey === selectedKey);
        if (activeVersion) activeVersion.textContent = active ? (active.hasDraft ? `Draft v${ active.draftVersionNumber || active.versionNumber }${ active.hasPublishedVersion ? " · Live v" + active.publishedVersionNumber : "" }` : "Live v" + (active.publishedVersionNumber || active.versionNumber)) : "—";
        if (activeStatus) {
            activeStatus.hidden = !active;
            activeStatus.classList.toggle("is-draft", active?.status === "draft");
            activeStatus.classList.toggle("is-unpublished", !!active && active.status !== "draft" && active.status !== "published");
            activeStatus.textContent = active ? statusLabel(active.status) : "";
        }

        const query = (search?.value || "").trim().toLowerCase();
        const filter = (statusFilter?.value || "all").toLowerCase();
        const filtered = widgets.filter(widget => {
            const matchesQuery = !query || [ widget.name, widget.description, widget.publicKey ]
                .some(part => String(part || "").toLowerCase().includes(query));
            return matchesQuery && (filter === "all" || (filter === "draft" ? widget.hasDraft : widget.hasPublishedVersion));
        });
        const tableHeading = '<div class="widget-table-heading" aria-hidden="true"><span>Name</span><span>Status</span><span>Actions</span></div>';
        list.innerHTML = tableHeading + filtered.map(widget => {
            const isSelected = widget.publicKey === selectedKey;
            const publishDisabled = !widget.hasDraft;
            const description = widget.description || "Decision experience";
            const versionText = widget.hasDraft ? `Draft v${ escapeHtml(widget.draftVersionNumber || widget.versionNumber) }${ widget.hasPublishedVersion ? ` · Live v${ escapeHtml(widget.publishedVersionNumber) }` : "" }` : widget.hasPublishedVersion ? `Live v${ escapeHtml(widget.publishedVersionNumber || widget.versionNumber) }` : "—";
            return `<div class="widget-table-row${ isSelected ? " is-selected" : "" }" data-widget-row="${ escapeHtml(widget.publicKey) }">
                <div class="widget-table-name"><span class="widget-row-icon" aria-hidden="true"><i class="bi bi-window-stack"></i></span><a class="widget-row-copy widget-workspace-link" href="#" data-page="widget-builder" data-widget-open="${ escapeHtml(widget.publicKey) }"><strong>${ escapeHtml(widget.name) }</strong><small>${ escapeHtml(description) }</small></a></div>
                <div class="widget-table-cell" data-label="Status"><div class="widget-status-version">${ widget.hasDraft ? '<span class="widget-status-badge is-draft">Draft</span>' : '' }${ widget.hasPublishedVersion ? '<span class="widget-status-badge is-published">Published</span>' : '' }${ !widget.hasDraft && !widget.hasPublishedVersion ? '<span class="widget-status-badge is-unpublished">Unpublished</span>' : '' }<small class="widget-table-version">${ versionText }</small></div></div>
                <div class="widget-table-actions" data-label="Actions">
                    <a class="btn btn-outline-success btn-sm${ publishDisabled ? " disabled" : "" }" href="#" ${ publishDisabled ? 'aria-disabled="true" tabindex="-1" title="There is no saved draft to publish"' : `data-page="publish-embed" data-widget-public-key="${ escapeHtml(widget.publicKey) }"` }><i class="bi bi-send me-1" aria-hidden="true"></i>Publish</a>
                    <div class="dropdown"><button class="btn btn-icon btn-sm" type="button" data-bs-toggle="dropdown" aria-expanded="false" aria-label="More actions for ${ escapeHtml(widget.name) }"><i class="bi bi-three-dots" aria-hidden="true"></i></button><ul class="dropdown-menu dropdown-menu-end"><li><button class="dropdown-item" type="button" data-copy-widget-key="${ escapeHtml(widget.publicKey) }">Copy widget key</button></li></ul></div>
                </div>
            </div>`;
        }).join("");

        const hasWidgets = filtered.length > 0;
        list.hidden = !hasWidgets;
        empty.hidden = hasWidgets;
        if (!hasWidgets && widgets.length > 0) {
            const title = empty.querySelector("h3");
            const message = empty.querySelector("#widgetEmptyMessage");
            if (title) title.textContent = "No matching widgets";
            if (message) message.textContent = "No widgets match your search or selected status. Update the filters and try again.";
            const createLink = empty.querySelector(".ap-widget-builder-link");
            if (createLink) createLink.hidden = true;
        } else {
            const title = empty.querySelector("h3");
            const message = empty.querySelector("#widgetEmptyMessage");
            if (title) title.textContent = "No widgets to display yet";
            if (message) message.textContent = "Create your first widget to start building a guided decision experience for your visitors.";
            const createLink = empty.querySelector(".ap-widget-builder-link");
            if (createLink) createLink.hidden = false;
        }
    }

    async function loadWidgets () {
        loading.hidden = false;
        error.hidden = true;
        list.hidden = true;
        empty.hidden = true;
        try {
            if (!apiBase) throw new Error("The widgets API address is not configured.");
            const headers = { Accept: "application/json" };
            if (ownerToken) headers[ "X-Demo-Owner-User-Token" ] = ownerToken;
            const response = await fetch(`${ apiBase }/widgets`, { headers });
            const body = await response.json().catch(() => ({}));
            if (!response.ok) throw new Error(body.message || "Widget inventory could not be loaded.");
            const records = Array.isArray(body.widgets) ? body.widgets : [];
            widgets = records.map(item => ({
                publicKey: String(value(item, "publicKey", "PublicKey") || ""),
                name: String(value(item, "name", "Name") || "Untitled widget"),
                description: String(value(item, "description", "Description") || ""),
                status: String(value(item, "status", "Status") || "draft").toLowerCase(),
                versionNumber: String(value(item, "versionNumber", "VersionNumber") || ""),
                draftVersionNumber: String(value(item, "draftVersionNumber", "DraftVersionNumber") || ""),
                publishedVersionNumber: String(value(item, "publishedVersionNumber", "PublishedVersionNumber") || ""),
                hasDraft: Boolean(value(item, "hasDraft", "HasDraft") ?? String(value(item, "status", "Status") || "").toLowerCase() === "draft"),
                hasPublishedVersion: Boolean(value(item, "hasPublishedVersion", "HasPublishedVersion") ?? String(value(item, "status", "Status") || "").toLowerCase() === "published"),
                updatedAt: value(item, "updatedAt", "UpdatedAtUtc") || value(item, "updatedAtUtc", "UpdatedAtUtc"),
                questionCount: value(item, "questionCount", "QuestionCount"),
                recommendationCount: value(item, "recommendationCount", "RecommendationCount")
            })).filter(widget => widget.publicKey);
            const savedKey = window.localStorage.getItem("askpearls.activeWidgetPublicKey");
            selectedKey = widgets.some(widget => widget.publicKey === savedKey) ? savedKey : (widgets[ 0 ]?.publicKey || "");
            loading.hidden = true;
            render();
        } catch (exception) {
            loading.hidden = true;
            error.hidden = false;
            const message = error.querySelector("span");
            if (message) message.textContent = exception.message || "We couldn't load your widgets.";
        }
    }

    select?.addEventListener("change", () => {
        selectedKey = select.value;
        if (selectedKey) window.localStorage.setItem("askpearls.activeWidgetPublicKey", selectedKey);
        render();
    });
    search?.addEventListener("input", render);
    statusFilter?.addEventListener("change", render);
    list?.addEventListener("click", event => {
        const createLink = event.target.closest(".ap-widget-builder-link");
        if (createLink) {
            window.localStorage.setItem("askpearls.widgetBuilderMode", "create");
            return;
        }
        const openLink = event.target.closest("[data-widget-open]");
        if (openLink) {
            window.localStorage.setItem("askpearls.widgetBuilderMode", "edit");
            selectedKey = openLink.dataset.widgetOpen;
            window.localStorage.setItem("askpearls.activeWidgetPublicKey", selectedKey);
            return;
        }
        const row = event.target.closest("[data-widget-row]");
        if (row && !event.target.closest("a,button")) {
            window.localStorage.setItem("askpearls.widgetBuilderMode", "edit");
            selectedKey = row.dataset.widgetRow;
            window.localStorage.setItem("askpearls.activeWidgetPublicKey", selectedKey);
            render();
        }
        const copyButton = event.target.closest("[data-copy-widget-key]");
        if (copyButton) navigator.clipboard?.writeText(copyButton.dataset.copyWidgetKey);
    });
    root.querySelector("#widgetRetry")?.addEventListener("click", loadWidgets);
    root.addEventListener("click", event => {
        if (event.target.closest(".ap-widget-builder-link")) {
            window.localStorage.setItem("askpearls.widgetBuilderMode", "create");
        }
    });
    loadWidgets();
})();
