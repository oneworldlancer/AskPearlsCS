
(function () {
    "use strict";

    const root = document.querySelector(
        '.page-dashboard[data-ap-page="dashboard"]'
    );

    if (!root || root.dataset.dashboardInitialized === "true") {
        return;
    }

    root.dataset.dashboardInitialized = "true";

    const widgetsUrl = "/Widget/Api/widgets";

    const STORAGE_KEY = "askpearls.activeWidgetPublicKey";
    const STORAGE_MODE = "askpearls.widgetBuilderMode";

    const el = {
        widgetCount: root.querySelector("#dashboardWidgetCountAll"),
        publishedCount: root.querySelector("#dashboardWidgetCountPublished"),
        draftCount: root.querySelector("#dashboardWidgetCountDrafts"),
        interactionCount: root.querySelector("#dashboardInteractionCount"),
        recentLoading: root.querySelector("#dashboardRecentLoading"),
        recentError: root.querySelector("#dashboardRecentError"),
        recentRetry: root.querySelector("#dashboardRecentRetry"),
        recentList: root.querySelector("#dashboardRecentList"),
        recentEmpty: root.querySelector("#dashboardRecentEmpty")
    };

    let isLoading = false;

    // ==========================================
    // GENERAL HELPERS
    // ==========================================

    function setText (element, value) {
        if (element) {
            element.textContent = String(value);
        }
    }

    function show (element) {
        if (element) element.hidden = false;
    }

    function hide (element) {
        if (element) element.hidden = true;
    }

    function setRecentState (state) {
        hide(el.recentLoading);
        hide(el.recentError);
        hide(el.recentList);
        hide(el.recentEmpty);

        switch (state) {
            case "loading":
                show(el.recentLoading);
                break;

            case "error":
                show(el.recentError);
                break;

            case "empty":
                show(el.recentEmpty);
                break;

            case "list":
                show(el.recentList);
                break;
        }
    }

    function showError (message) {
        const text = el.recentError?.querySelector("p");

        if (text) {
            text.textContent = message ||
                "We couldn't load your widgets.";
        }

        setRecentState("error");
    }

    function normalizeWidgets (result) {
        if (Array.isArray(result)) return result;

        if (!result || typeof result !== "object") {
            throw new Error("Invalid API response.");
        }

        if (result.success === false) {
            throw new Error(
                result.message ||
                result.detail ||
                "Could not load widgets."
            );
        }

        const widgets =
            result.widgets ??
            result.data?.widgets ??
            result.data ??
            result.items;

        if (!Array.isArray(widgets)) {
            throw new Error(
                "The API response does not contain a widgets array."
            );
        }

        return widgets;
    }

    function getWidgetName (widget) {
        return String(
            widget.name ??
            widget.widgetName ??
            widget.title ??
            "Untitled widget"
        );
    }

    function getWidgetKey (widget) {
        return String(
            widget.publicKey ??
            widget.widgetPublicKey ??
            ""
        ).trim();
    }

    function getWidgetStatus (widget) {
        return String(
            widget.status ??
            widget.widgetStatusCode ??
            widget.widgetStatus ??
            "draft"
        ).trim().toLowerCase();
    }

    function getWidgetVersion (widget) {
        const version =
            widget.draftVersionNumber ??
            widget.versionNumber ??
            widget.publishedVersionNumber ??
            widget.widgetPublishedVersion;

        if (version === null ||
            version === undefined ||
            version === "") {
            return "—";
        }

        return String(version);
    }

    // ==========================================
    // DASHBOARD COUNTERS
    // ==========================================

    function updateCounters (widgets) {
        const total = widgets.length;

        const published = widgets.filter(
            widget => getWidgetStatus(widget) === "published"
        ).length;

        const drafts = widgets.filter(
            widget => getWidgetStatus(widget) === "draft"
        ).length;

        setText(el.widgetCount, total);
        setText(el.publishedCount, published);
        setText(el.draftCount, drafts);
    }

    function resetCounters () {
        setText(el.widgetCount, "—");
        setText(el.publishedCount, "—");
        setText(el.draftCount, "—");
    }

    // ==========================================
    // SELECT WIDGET FOR EDIT
    // ==========================================

    function selectWidgetForEdit (publicKey) {
        if (!publicKey) return false;

        try {
            localStorage.setItem(STORAGE_MODE, "edit");
            localStorage.setItem(STORAGE_KEY, publicKey);
            return true;
        } catch (error) {
            console.error(
                "Could not save selected widget:",
                error
            );
            return false;
        }
    }

    // ==========================================
    // CREATE TABLE ELEMENTS SAFELY
    // ==========================================

    function createElement (tag, className, text) {
        const element = document.createElement(tag);

        if (className) element.className = className;

        if (text !== undefined) {
            element.textContent = String(text);
        }

        return element;
    }

    function createActionLink (
        label,
        icon,
        page,
        publicKey,
        className
    ) {
        const link = createElement(
            "a",
            "btn btn-sm " + className
        );

        link.href = "#";
        link.dataset.page = page;
        link.dataset.widgetOpen = publicKey;
        link.title = label;

        const iconElement = createElement(
            "i",
            "bi " + icon + " me-1"
        );

        iconElement.setAttribute("aria-hidden", "true");

        link.append(
            iconElement,
            document.createTextNode(label)
        );

        return link;
    }

    function createTable () {
        const wrapper = createElement(
            "div",
            "dashboard-recent-table-wrapper"
        );

        const table = createElement(
            "table",
            "table dashboard-recent-table align-middle mb-0"
        );

        const thead = document.createElement("thead");
        const headerRow = document.createElement("tr");

        [ "Widget", "Status", "Version", "Actions" ].forEach(
            function (title) {
                const th = createElement("th", "", title);
                th.scope = "col";
                headerRow.appendChild(th);
            }
        );

        thead.appendChild(headerRow);

        const tbody = document.createElement("tbody");

        table.append(thead, tbody);
        wrapper.appendChild(table);

        return { wrapper, tbody };
    }

    // ==========================================
    // RENDER RECENT WIDGETS TABLE
    // ==========================================


    function renderWidgets (widgets) {

        if (!el.recentList) {
            console.warn(
                "AskPearls Dashboard: Recent Widgets container not found."
            );
            return;
        }

        el.recentList.replaceChildren();

        if (!Array.isArray(widgets) || widgets.length === 0) {
            setRecentState("empty");
            return;
        }

        // ==========================================
        // 1. CREATE TABLE
        // ==========================================

        const { wrapper, tbody } = createTable();

        const fragment = document.createDocumentFragment();

        // Display the five most recent widgets.
        widgets.slice(0, 5).forEach(function (widget) {

            const publicKey = getWidgetKey(widget);
            const name = getWidgetName(widget);

            // Parent widget status is the source of truth.
            const status = getWidgetStatus(widget);

            const isDraft = status === "draft";
            const isPublished = status === "published";

            const version = getWidgetVersion(widget);

            const tr = document.createElement("tr");
            tr.className = "dashboard-recent-row";

            // ==========================================
            // 2. WIDGET NAME COLUMN
            // ==========================================

            const widgetCell = document.createElement("td");

            const widgetInfo = createElement(
                "div",
                "dashboard-recent-widget"
            );

            const widgetIcon = createElement(
                "span",
                "dashboard-recent-icon"
            );

            const icon = createElement(
                "i",
                "bi bi-window-stack"
            );

            icon.setAttribute("aria-hidden", "true");

            widgetIcon.appendChild(icon);

            const widgetCopy = createElement(
                "div",
                "dashboard-recent-copy"
            );

            const widgetLink = createElement(
                "a",
                "dashboard-widget-name",
                name
            );

            if (publicKey) {

                widgetLink.href = "#";

                widgetLink.dataset.page = "widget-builder";
                widgetLink.dataset.widgetOpen = publicKey;

                widgetLink.title = "Edit " + name;

            } else {

                widgetLink.classList.add("is-disabled");
                widgetLink.removeAttribute("href");

                widgetLink.setAttribute(
                    "aria-disabled",
                    "true"
                );
            }

            const description = createElement(
                "p",
                "",
                widget.description ??
                widget.widgetDescription ??
                "DecisionOS Widget"
            );

            widgetCopy.append(
                widgetLink,
                description
            );

            widgetInfo.append(
                widgetIcon,
                widgetCopy
            );

            widgetCell.appendChild(widgetInfo);

            // ==========================================
            // 3. STATUS COLUMN
            // ==========================================

            const statusCell = document.createElement("td");

            let statusClass = "is-other";

            if (isDraft) {
                statusClass = "is-draft";
            } else if (isPublished) {
                statusClass = "is-published";
            }

            const statusBadge = createElement(
                "span",
                "dashboard-recent-status " + statusClass,
                status
            );

            statusBadge.dataset.status = status;

            statusCell.appendChild(statusBadge);

            // ==========================================
            // 4. VERSION COLUMN
            // ==========================================

            const versionCell = document.createElement("td");

            versionCell.className =
                "dashboard-recent-version";

            versionCell.textContent =
                version === "—"
                    ? "—"
                    : "v" + version;

            // ==========================================
            // 5. ACTIONS COLUMN
            // ==========================================

            const actionsCell = document.createElement("td");

            const actions = createElement(
                "div",
                "dashboard-recent-actions"
            );

            if (publicKey) {

                // --------------------------------------
                // EDIT BUTTON
                // Always enabled for valid widgets.
                // --------------------------------------

                const editButton = createActionLink(
                    "Edit",
                    "bi-pencil-square",
                    "widget-builder",
                    publicKey,
                    "btn-outline-success"
                );

                editButton.title = "Edit " + name;

                actions.appendChild(editButton);

                // --------------------------------------
                // DESIGN BUTTON
                // Always enabled for valid widgets.
                // --------------------------------------

                const designButton = createActionLink(
                    "Design",
                    "bi-palette",
                    "design-customize",
                    publicKey,
                    "btn-outline-secondary"
                );

                designButton.title =
                    "Customize design for " + name;

                actions.appendChild(designButton);

                // --------------------------------------
                // PUBLISH BUTTON
                // Enabled ONLY for Draft widgets.
                // --------------------------------------

                if (isDraft) {

                    const publishButton = createActionLink(
                        "Publish",
                        "bi-send",
                        "publish-embed",
                        publicKey,
                        "btn-outline-success dashboard-publish-button"
                    );

                    publishButton.title =
                        "Publish " + name;

                    actions.appendChild(publishButton);

                } else {

                    // Published and other non-draft
                    // widgets cannot be published here.

                    const publishButton = createElement(
                        "button",
                        "btn btn-sm btn-outline-secondary dashboard-publish-button"
                    );

                    publishButton.type = "button";
                    publishButton.disabled = true;

                    const publishIcon = createElement(
                        "i",
                        isPublished
                            ? "bi bi-check-circle me-1"
                            : "bi bi-lock me-1"
                    );

                    publishIcon.setAttribute(
                        "aria-hidden",
                        "true"
                    );

                    const publishText = isPublished
                        ? "Published"
                        : "Publish";

                    publishButton.append(
                        publishIcon,
                        document.createTextNode(publishText)
                    );

                    publishButton.title = isPublished
                        ? "This widget is already published."
                        : "Only draft widgets can be published.";

                    publishButton.setAttribute(
                        "aria-label",
                        publishButton.title
                    );

                    actions.appendChild(publishButton);
                }

            } else {

                // Widget without a public key cannot
                // be edited, designed, or published.

                const unavailable = createElement(
                    "span",
                    "small text-muted",
                    "Actions unavailable"
                );

                actions.appendChild(unavailable);
            }

            actionsCell.appendChild(actions);

            // ==========================================
            // 6. APPEND TABLE ROW
            // ==========================================

            tr.append(
                widgetCell,
                statusCell,
                versionCell,
                actionsCell
            );

            fragment.appendChild(tr);
        });

        // ==========================================
        // 7. RENDER TABLE
        // ==========================================

        tbody.appendChild(fragment);

        el.recentList.appendChild(wrapper);

        setRecentState("list");

        console.debug(
            "AskPearls Dashboard: Recent Widgets rendered.",
            {
                displayed: Math.min(widgets.length, 5),
                total: widgets.length
            }
        );
    }


    // ==========================================
    // CLICK HANDLER
    // ==========================================

    // The existing Portal data-page navigation handles
    // page changes. Save the selected widget first.
    // Do not prevent default navigation on success.

    el.recentList?.addEventListener(
        "click",
        function (event) {
            const link = event.target.closest(
                "a[data-widget-open]"
            );

            if (!link || !el.recentList.contains(link)) {
                return;
            }

            const publicKey = link.dataset.widgetOpen;

            if (!selectWidgetForEdit(publicKey)) {
                event.preventDefault();
                event.stopPropagation();
            }
        }
    );

    // ==========================================
    // HTTP RESPONSE HANDLING
    // ==========================================

    async function readResponse (response) {
        if (response.type === "opaqueredirect" ||
            response.redirected) {
            throw new Error(
                "Your session may have expired. Please sign in again."
            );
        }

        if (response.status === 401) {
            throw new Error(
                "Your session has expired. Please sign in again."
            );
        }

        if (response.status === 403) {
            throw new Error(
                "You do not have permission to access these widgets."
            );
        }

        const contentType = (
            response.headers.get("content-type") || ""
        ).toLowerCase();

        if (!contentType.includes("json")) {
            throw new Error(
                "Unexpected server response. Expected JSON."
            );
        }

        let result;

        try {
            result = await response.json();
        } catch {
            throw new Error("Invalid JSON returned by the server.");
        }

        if (!response.ok) {
            throw new Error(
                result?.message ??
                result?.detail ??
                `Could not load widgets (HTTP ${ response.status }).`
            );
        }

        return result;
    }

    // ==========================================
    // LOAD WIDGETS
    // ==========================================

    async function loadWidgets () {
        if (isLoading) return;

        isLoading = true;

        setRecentState("loading");

        if (el.recentRetry) {
            el.recentRetry.disabled = true;
        }

        try {
            const response = await fetch(widgetsUrl, {
                method: "GET",
                credentials: "same-origin",
                cache: "no-store",
                redirect: "manual",
                headers: {
                    "Accept": "application/json"
                }
            });

            const result = await readResponse(response);
            const widgets = normalizeWidgets(result);

            updateCounters(widgets);
            renderWidgets(widgets);

        } catch (error) {
            console.error(
                "AskPearls Dashboard: Loading failed:",
                error
            );

            resetCounters();

            el.recentList?.replaceChildren();

            showError(
                error instanceof Error
                    ? error.message
                    : "Could not load your widgets."
            );

        } finally {
            isLoading = false;

            if (el.recentRetry) {
                el.recentRetry.disabled = false;
            }
        }
    }

    // ==========================================
    // RETRY
    // ==========================================

    el.recentRetry?.addEventListener(
        "click",
        function (event) {
            event.preventDefault();
            loadWidgets();
        }
    );

    // ==========================================
    // CREATE NEW WIDGET
    // ==========================================

    root.querySelectorAll(
        "[data-widget-builder-create]"
    ).forEach(function (button) {
        button.addEventListener("click", function () {
            try {
                localStorage.setItem(
                    STORAGE_MODE,
                    "create"
                );

                localStorage.removeItem(STORAGE_KEY);
            } catch (error) {
                console.error(error);
            }
        });
    });

    // ==========================================
    // INITIALIZATION
    // ==========================================

    resetCounters();

    if (el.interactionCount) {
        setText(el.interactionCount, "—");
    }

    loadWidgets();

})();