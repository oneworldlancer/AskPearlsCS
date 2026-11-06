
(function () {
    "use strict";

    // ==========================================
    // 1. DASHBOARD INITIALIZATION
    // ==========================================

    const root = document.querySelector(
        '.page-dashboard[data-ap-page="dashboard"]'
    );

    if (!root) {
        console.warn(
            "AskPearls Dashboard: Root element not found."
        );
        return;
    }

    if (root.dataset.dashboardInitialized === "true") {
        return;
    }

    root.dataset.dashboardInitialized = "true";

    // ==========================================
    // 2. RAZOR ELEMENT REFERENCES
    // ==========================================

    const widgetCount = root.querySelector(
        "#dashboardWidgetCountAll"
    );

    const publishedCount = root.querySelector(
        "#dashboardWidgetCountPublished"
    );

    const draftCount = root.querySelector(
        "#dashboardWidgetCountDrafts"
    );

    const interactionCount = root.querySelector(
        "#dashboardInteractionCount"
    );

    const recentLoading = root.querySelector(
        "#dashboardRecentLoading"
    );

    const recentError = root.querySelector(
        "#dashboardRecentError"
    );

    const recentRetry = root.querySelector(
        "#dashboardRecentRetry"
    );

    const recentList = root.querySelector(
        "#dashboardRecentList"
    );

    const recentEmpty = root.querySelector(
        "#dashboardRecentEmpty"
    );

    // Portal controller endpoint.
    // Portal forwards requests to AskPearls API.

    const widgetsUrl = "/Widget/Api/widgets";

    let isLoading = false;

    // ==========================================
    // 3. GENERAL HELPERS
    // ==========================================

    function setText (element, value) {
        if (!element) return;

        element.textContent = String(value);
    }

    function showElement (element) {
        if (element) {
            element.hidden = false;
        }
    }

    function hideElement (element) {
        if (element) {
            element.hidden = true;
        }
    }

    function setRecentState (state) {

        hideElement(recentLoading);
        hideElement(recentError);
        hideElement(recentList);
        hideElement(recentEmpty);

        switch (state) {

            case "loading":
                showElement(recentLoading);
                break;

            case "error":
                showElement(recentError);
                break;

            case "empty":
                showElement(recentEmpty);
                break;

            case "list":
                showElement(recentList);
                break;

            default:
                console.warn(
                    "AskPearls Dashboard: Unknown state:",
                    state
                );
                break;
        }
    }

    function showError (message) {

        if (recentError) {

            const errorText = recentError.querySelector("p");

            if (errorText) {
                errorText.textContent =
                    message || "We couldn't load your widgets.";
            }
        }

        setRecentState("error");
    }

    // ==========================================
    // 4. API RESPONSE NORMALIZATION
    // ==========================================

    function normalizeWidgets (result) {

        // Support a plain JSON array.
        if (Array.isArray(result)) {
            return result;
        }

        if (!result || typeof result !== "object") {
            throw new Error(
                "The API returned an invalid response."
            );
        }

        // Handle API business errors.
        if (result.success === false) {

            throw new Error(
                result.message ||
                result.detail ||
                "The API could not load widgets."
            );
        }

        // Current AskPearls API format:
        // { success: true, widgets: [] }

        if (Array.isArray(result.widgets)) {
            return result.widgets;
        }

        // Support alternative wrapped formats.

        if (Array.isArray(result.data)) {
            return result.data;
        }

        if (Array.isArray(result.items)) {
            return result.items;
        }

        if (Array.isArray(result.data?.widgets)) {
            return result.data.widgets;
        }

        throw new Error(
            "The API response does not contain a widgets array."
        );
    }

    // ==========================================
    // 5. WIDGET DATA HELPERS
    // ==========================================

    function getWidgetName (widget) {

        if (!widget || typeof widget !== "object") {
            return "Untitled widget";
        }

        return widget.name ||
            widget.widgetName ||
            widget.title ||
            "Untitled widget";
    }

    function getWidgetStatus (widget) {

        if (!widget || typeof widget !== "object") {
            return "draft";
        }

        const status =
            widget.status ??
            widget.widgetStatus ??
            widget.widgetStatusCode ??
            "draft";

        return String(status).trim().toLowerCase();
    }

    function hasPublishedVersion (widget) {

        if (!widget || typeof widget !== "object") {
            return false;
        }

        if (
            typeof widget.hasPublishedVersion === "boolean"
        ) {
            return widget.hasPublishedVersion;
        }

        if (
            typeof widget.isPublished === "boolean"
        ) {
            return widget.isPublished;
        }

        return [
            "published",
            "live",
            "active"
        ].includes(getWidgetStatus(widget));
    }

    function hasDraftVersion (widget) {

        if (!widget || typeof widget !== "object") {
            return false;
        }

        if (typeof widget.hasDraft === "boolean") {
            return widget.hasDraft;
        }

        if (
            typeof widget.hasDraftVersion === "boolean"
        ) {
            return widget.hasDraftVersion;
        }

        return getWidgetStatus(widget) === "draft";
    }

    // ==========================================
    // 6. DASHBOARD COUNTERS
    // ==========================================

    function updateCounters (widgets) {

        const total = widgets.length;

        const published = widgets.filter(
            hasPublishedVersion
        ).length;

        const drafts = widgets.filter(
            hasDraftVersion
        ).length;

        setText(widgetCount, total);

        setText(publishedCount, published);

        setText(draftCount, drafts);

        console.debug(
            "AskPearls Dashboard: Counters updated",
            {
                total: total,
                published: published,
                drafts: drafts
            }
        );
    }

    function resetCounters (value = "—") {

        setText(widgetCount, value);

        setText(publishedCount, value);

        setText(draftCount, value);
    }

    // ==========================================
    // 7. RENDER RECENT WIDGETS
    // ==========================================

    function renderWidgets (widgets) {

        if (!recentList) {
            console.warn(
                "AskPearls Dashboard: Recent list not found."
            );
            return;
        }

        recentList.replaceChildren();

        if (widgets.length === 0) {

            setRecentState("empty");

            return;
        }

        const fragment = document.createDocumentFragment();

        // Display up to five recent widgets.
        // Assumes API returns newest widgets first.

        widgets.slice(0, 5).forEach(function (widget) {

            const item = document.createElement("article");

            item.className = "dashboard-widget-item";

            const name = document.createElement("h3");

            name.className = "dashboard-widget-name";

            name.textContent = getWidgetName(widget);

            const status = document.createElement("span");

            status.className = "dashboard-widget-status";

            const currentStatus = getWidgetStatus(widget);

            status.textContent = currentStatus;

            status.dataset.status = currentStatus;

            item.append(name, status);

            fragment.appendChild(item);
        });

        recentList.appendChild(fragment);

        setRecentState("list");
    }

    // ==========================================
    // 8. HTTP RESPONSE HANDLING
    // ==========================================

    async function readResponse (response) {

        if (
            response.type === "opaqueredirect" ||
            (response.status >= 300 &&
                response.status < 400)
        ) {

            throw new Error(
                "The API redirected the request. " +
                "Please sign in again."
            );
        }

        if (response.redirected) {

            throw new Error(
                "The server redirected the API request. " +
                "Your session may have expired."
            );
        }

        if (response.status === 401) {

            throw new Error(
                "Your session has expired. Please sign in again."
            );
        }

        if (response.status === 403) {

            throw new Error(
                "Access denied. You do not have permission " +
                "to access these widgets."
            );
        }

        const contentType = (
            response.headers.get("content-type") || ""
        ).toLowerCase();

        if (!contentType.includes("json")) {

            throw new Error(
                `Unexpected server response (HTTP ${ response.status }). ` +
                "Expected JSON."
            );
        }

        let result;

        try {

            result = await response.json();

        } catch {

            throw new Error(
                "The server returned invalid JSON."
            );
        }

        if (!response.ok) {

            throw new Error(
                result?.detail ||
                result?.message ||
                result?.title ||
                `Could not load widgets (HTTP ${ response.status }).`
            );
        }

        return result;
    }

    // ==========================================
    // 9. LOAD DASHBOARD WIDGETS
    // ==========================================

    async function loadWidgets () {

        if (isLoading) {
            return;
        }

        isLoading = true;

        setRecentState("loading");

        if (recentRetry) {
            recentRetry.disabled = true;
        }

        if (recentList) {
            recentList.setAttribute("aria-busy", "true");
        }

        try {

            console.info(
                "AskPearls Dashboard: Loading widgets..."
            );

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

            // Update overview cards.
            updateCounters(widgets);

            // Render recent widgets or empty state.
            renderWidgets(widgets);

            console.info(
                "AskPearls Dashboard: Widgets loaded successfully.",
                {
                    count: widgets.length
                }
            );

        } catch (error) {

            console.error(
                "AskPearls Dashboard: Widget loading failed:",
                error
            );

            resetCounters("—");

            if (recentList) {
                recentList.replaceChildren();
            }

            showError(
                error instanceof Error
                    ? error.message
                    : "Could not load your widgets."
            );

        } finally {

            isLoading = false;

            if (recentRetry) {
                recentRetry.disabled = false;
            }

            if (recentList) {
                recentList.removeAttribute("aria-busy");
            }
        }
    }

    // ==========================================
    // 10. RETRY BUTTON
    // ==========================================

    if (recentRetry) {

        recentRetry.addEventListener(
            "click",
            function (event) {

                event.preventDefault();

                loadWidgets();
            }
        );
    }

    // ==========================================
    // 11. CREATE WIDGET NAVIGATION
    // ==========================================

    // Match existing Razor attribute:
    // data-widget-builder-create

    root.querySelectorAll(
        "[data-widget-builder-create]"
    ).forEach(function (button) {

        button.addEventListener("click", function () {

            try {

                localStorage.setItem(
                    "askpearls.widgetBuilderMode",
                    "create"
                );

                localStorage.removeItem(
                    "askpearls.activeWidgetPublicKey"
                );

            } catch (error) {

                console.warn(
                    "AskPearls Dashboard: Could not save " +
                    "Widget Builder mode:",
                    error
                );
            }

            // Do not prevent default navigation.
            // Existing data-page navigation handles it.
        });
    });

    // ==========================================
    // 12. VALIDATE RAZOR ELEMENTS
    // ==========================================

    function validateElements () {

        const elements = {

            widgetCount,
            publishedCount,
            draftCount,
            recentLoading,
            recentError,
            recentRetry,
            recentList,
            recentEmpty
        };

        Object.entries(elements).forEach(
            function ([ name, element ]) {

                if (!element) {

                    console.warn(
                        `AskPearls Dashboard: Missing Razor element: ${ name }`
                    );
                }
            }
        );
    }

    // ==========================================
    // 13. INITIALIZE DASHBOARD
    // ==========================================

    validateElements();

    // Initial counter values.
    resetCounters("—");

    // Visitor interactions require a separate
    // analytics endpoint. Do not invent a count.
    // Keep the Razor placeholder until implemented.

    if (interactionCount) {
        setText(interactionCount, "—");
    }

    // Load widgets from Portal controller.
    loadWidgets();

})();
