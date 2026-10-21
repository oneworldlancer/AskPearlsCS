(function () {
    "use strict";

    const root = document.querySelector('.page-dashboard[data-ap-page="dashboard"]');
    if (!root || root.dataset.dashboardInitialized === "true") return;
    root.dataset.dashboardInitialized = "true";

    const apiBase = (root.dataset.apiBase || "").replace(/\/+$/, "");
    const ownerToken = (root.dataset.demoOwnerUserToken || "").trim();
    const loading = root.querySelector("#dashboardRecentLoading");
    const error = root.querySelector("#dashboardRecentError");
    const list = root.querySelector("#dashboardRecentList");
    const empty = root.querySelector("#dashboardRecentEmpty");
    const counts = {
        all: root.querySelector("#dashboardWidgetCountAll"),
        published: root.querySelector("#dashboardWidgetCountPublished"),
        drafts: root.querySelector("#dashboardWidgetCountDrafts")
    };

    function escapeHtml (value) {
        return String(value ?? "").replace(/[&<>"']/g, character => ({
            "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
        })[ character ]);
    }

    function read (item, camel, pascal) {
        return item[ camel ] ?? item[ pascal ];
    }

    function render (widgets) {
        counts.all.textContent = String(widgets.length);
        counts.published.textContent = String(widgets.filter(widget => widget.status === "published").length);
        counts.drafts.textContent = String(widgets.filter(widget => widget.status === "draft").length);

        list.innerHTML = widgets.slice(0, 5).map(widget => {
            const statusClass = widget.status === "published" ? "is-published" : "is-draft";
            const publishDisabled = widget.status === "published";
            const detail = widget.description || widget.publicKey;
            const version = widget.versionNumber ? "Version " + escapeHtml(widget.versionNumber) : "";
            return '<article class="dashboard-recent-row">' +
                '<div class="dashboard-recent-main">' +
                '<span class="dashboard-recent-icon" aria-hidden="true"><i class="bi bi-window-stack"></i></span>' +
                '<div class="dashboard-recent-copy"><h3><a href="#" class="dashboard-widget-name" data-page="widget-builder" data-widget-open="' + escapeHtml(widget.publicKey) + '">' + escapeHtml(widget.name) + '</a></h3><p>' + escapeHtml(detail) + '</p></div>' +
                '</div>' +
                '<div class="dashboard-recent-actions">' +
                '<span class="dashboard-recent-status ' + statusClass + '">' + escapeHtml(widget.status) + '</span>' +
                (version ? '<span class="dashboard-recent-version">' + version + '</span>' : '') +
                '<a href="#" class="btn btn-outline-success btn-sm" data-page="design-customize" data-widget-public-key="' + escapeHtml(widget.publicKey) + '">Design</a>' +
                '<button type="button" class="btn btn-success btn-sm dashboard-publish-button" data-page="publish-embed" data-widget-public-key="' + escapeHtml(widget.publicKey) + '"' +
                (publishDisabled ? ' disabled aria-disabled="true" title="This widget is already published"' : '') +
                '>Publish</button>' +
                '</div>' +
                '</article>';
        }).join("");

        loading.hidden = true;
        error.hidden = true;
        list.hidden = widgets.length === 0;
        empty.hidden = widgets.length !== 0;
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
            const response = await fetch(apiBase + "/widgets", { headers });
            const body = await response.json().catch(() => ({}));
            if (!response.ok) throw new Error(body.message || "Your widgets could not be loaded.");

            const records = Array.isArray(body.widgets) ? body.widgets : [];
            const widgets = records.map(item => ({
                publicKey: String(read(item, "publicKey", "PublicKey") || ""),
                name: String(read(item, "name", "Name") || "Untitled widget"),
                description: String(read(item, "description", "Description") || ""),
                status: String(read(item, "status", "Status") || "draft").toLowerCase(),
                versionNumber: String(read(item, "versionNumber", "VersionNumber") || "")
            })).filter(widget => widget.publicKey);

            render(widgets);
        } catch (exception) {
            loading.hidden = true;
            error.hidden = false;
            const message = error.querySelector("p");
            if (message) message.textContent = exception.message || "We couldn't load your widgets.";
        }
    }

    list.addEventListener("click", event => {
        const openLink = event.target.closest("[data-widget-open]");
        if (!openLink) return;
        window.localStorage.setItem("askpearls.activeWidgetPublicKey", openLink.dataset.widgetOpen);
    });

    root.querySelector("#dashboardRecentRetry")?.addEventListener("click", loadWidgets);
    loadWidgets();
})();
