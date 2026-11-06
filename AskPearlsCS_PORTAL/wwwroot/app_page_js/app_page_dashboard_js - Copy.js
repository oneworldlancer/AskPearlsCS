(function () {
    "use strict";

    const root = document.querySelector(
        '.page-dashboard[data-ap-page="dashboard"]'
    );

    if (!root || root.dataset.dashboardInitialized === "true") {
        return;
    }

    root.dataset.dashboardInitialized = "true";

    const widgetList = root.querySelector("[data-dashboard-widgets]");
    const widgetCount = root.querySelector("[data-dashboard-widget-count]");
    const publishedCount = root.querySelector("[data-dashboard-published-count]");
    const draftCount = root.querySelector("[data-dashboard-draft-count]");
    const statusMessage = root.querySelector("[data-dashboard-message]");
    const retryButton = root.querySelector("[data-dashboard-retry]");

    const widgetsUrl = "/Widget/Api/widgets";

    function showMessage (message, isError) {
        if (!statusMessage) return;

        statusMessage.textContent = message || "";
        statusMessage.hidden = !message;
        statusMessage.classList.toggle("is-error", Boolean(isError));
    }

    function setText (element, value) {
        if (element) {
            element.textContent = value;
        }
    }

    function getWidgetName (widget) {
        return widget.name
            || widget.widgetName
            || widget.title
            || "Untitled widget";
    }

    function getWidgetStatus (widget) {
        return (
            widget.status
            || widget.widgetStatus
            || widget.widgetStatusCode
            || "Draft"
        );
    }

    function isPublished (widget) {
        const status = String(getWidgetStatus(widget)).toLowerCase();

        return status === "published"
            || status === "live"
            || status === "active";
    }

    function renderWidgets (widgets) {
        if (!widgetList) return;

        widgetList.replaceChildren();

        if (!widgets.length) {
            const empty = document.createElement("p");
            empty.className = "dashboard-empty";
            empty.textContent = "You haven’t created a widget yet.";
            widgetList.appendChild(empty);
            return;
        }

        widgets.slice(0, 5).forEach(function (widget) {
            const item = document.createElement("article");
            item.className = "dashboard-widget-item";

            const name = document.createElement("h3");
            name.textContent = getWidgetName(widget);

            const status = document.createElement("span");
            status.className = "dashboard-widget-status";
            status.textContent = getWidgetStatus(widget);

            item.append(name, status);
            widgetList.appendChild(item);
        });
    }

    async function readResponse (response) {
        const text = await response.text();

        if (!text) return null;

        try {
            return JSON.parse(text);
        } catch {
            return { message: text };
        }
    }

    async function loadWidgets () {
        showMessage("", false);

        if (retryButton) {
            retryButton.disabled = true;
        }

        if (widgetList) {
            widgetList.setAttribute("aria-busy", "true");
        }

        try {
            const response = await fetch(widgetsUrl, {
                method: "GET",
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json"
                }
            });

            const result = await readResponse(response);

            if (!response.ok) {
                const message =
                    result?.detail
                    || result?.message
                    || `Could not load widgets (HTTP ${ response.status }).`;

                throw new Error(message);
            }

            // Accept either a plain JSON array or a wrapped API response.
            const widgets = Array.isArray(result)
                ? result
                : Array.isArray(result?.widgets)
                    ? result.widgets
                    : Array.isArray(result?.data)
                        ? result.data
                        : Array.isArray(result?.items)
                            ? result.items
                            : null;

            if (!widgets) {
                throw new Error("The Portal returned an unexpected widget response.");
            }

            const published = widgets.filter(isPublished).length;
            const drafts = widgets.length - published;

            setText(widgetCount, String(widgets.length));
            setText(publishedCount, String(published));
            setText(draftCount, String(drafts));

            renderWidgets(widgets);
        } catch (error) {
            console.error("Dashboard widget loading failed:", error);

            setText(widgetCount, "—");
            setText(publishedCount, "—");
            setText(draftCount, "—");

            if (widgetList) {
                widgetList.replaceChildren();
            }

            showMessage(
                error instanceof Error
                    ? error.message
                    : "Could not load your widgets. Please try again.",
                true
            );
        } finally {
            if (retryButton) {
                retryButton.disabled = false;
            }

            if (widgetList) {
                widgetList.removeAttribute("aria-busy");
            }
        }
    }

    if (retryButton) {
        retryButton.addEventListener("click", loadWidgets);
    }

    // Keep the existing dashboard action behavior.
    root.querySelectorAll("[data-dashboard-create-widget]").forEach(function (button) {
        button.addEventListener("click", function () {
            try {
                localStorage.setItem("askpearls.widgetBuilderMode", "create");
                localStorage.removeItem("askpearls.activeWidgetPublicKey");
            } catch (error) {
                console.warn("Could not save the Widget Builder mode:", error);
            }
        });
    });

    loadWidgets();
})();