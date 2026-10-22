
(function () {
    "use strict";

    const root = document.querySelector(".publish-embed-page");
    if (!root || root.dataset.peInitialized === "true") return;
    root.dataset.peInitialized = "true";

    const $ = id => root.querySelector("#" + id);
    const apiBase = (root.dataset.apiBase || "").replace(/\/+$/, "");
    const apiOrigin = (root.dataset.apiOrigin || window.location.origin).replace(/\/+$/, "");
    const widgetScriptUrl = (root.dataset.widgetScriptUrl || (apiOrigin + "/widget.js")).trim();
    const demoOwnerToken = (root.dataset.demoOwnerUserToken || "").trim();

    const widgets = {
        pk_demo_sofa_finder: {
            name: "Sofa Finder Widget",
            version: "—"
        },
        pk_furniture_recommendation: {
            name: "Furniture Recommendation Widget",
            version: "—"
        },
        pk_demo_recommendation: {
            name: "Demo Recommendation Widget",
            version: "—"
        }
    };

    const platforms = {
        html: {
            label: "Custom HTML",
            codeLabel: "HTML embed snippet",
            heading: "Copy your embed code",
            description: "Copy the snippet below and add it to your website.",
            stepTwoHeading: "Paste the code into your website",
            stepTwoDescription: "Place the snippet before the closing body tag, then save your page.",
            tip: "Add the snippet once per website. The widget can then load wherever the configured script is included."
        },
        wordpress: {
            label: "WordPress",
            codeLabel: "WordPress embed snippet",
            heading: "Copy your widget snippet",
            description: "Use the same standard embed snippet on your WordPress website.",
            stepTwoHeading: "Add the snippet to WordPress",
            stepTwoDescription: "Use a trusted custom-code plugin or your theme's appropriate footer code area.",
            tip: "Back up your site before changing theme files. Plugin names and available options vary by WordPress setup."
        },
        shopify: {
            label: "Shopify",
            codeLabel: "Shopify embed snippet",
            heading: "Copy your widget snippet",
            description: "Prepare the snippet for your Shopify storefront.",
            stepTwoHeading: "Add the snippet to your theme",
            stepTwoDescription: "In your Shopify admin, open the theme code editor and locate the appropriate layout or supported custom-code area.",
            tip: "Duplicate your theme before editing it. Check the storefront after installation and theme updates."
        },
        wix: {
            label: "Wix",
            codeLabel: "Wix embed snippet",
            heading: "Copy your widget snippet",
            description: "Prepare the snippet for your Wix website.",
            stepTwoHeading: "Add the code using Wix",
            stepTwoDescription: "Use Wix's supported custom-code or embed feature and select the pages where the widget should appear.",
            tip: "Availability and placement of custom code can depend on your Wix plan and site configuration."
        }
    };

    let activePlatform = "html";
    let isPublished = false;
    let hasDraft = false;
    let isPublishing = false;
    let toastTimer = null;
    let domains = [
        { id: 1, value: "example.com" },
        { id: 2, value: "store.example.com" }
    ];

    function escapeHtml (value) {
        return String(value ?? "").replace(/[&<>"']/g, ch => ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#39;"
        })[ ch ]);
    }

    function getWidget () {
        return widgets[ $("peWidgetSelect").value ] || widgets.pk_demo_sofa_finder;
    }

    function getEmbedCode () {
        const widgetId = $("peWidgetSelect").value;
        return '<!-- AskPearls DecisionOS widget -->\n' +
            '<script src="' + widgetScriptUrl + '" data-widget="' + widgetId +
            '" data-api-url="' + apiOrigin + '"><' + '/script>';
    }

    function renderCode () {
        $("peEmbedCode").textContent = getEmbedCode();
    }

    function renderPlatform (platform) {
        activePlatform = platform;
        const config = platforms[ platform ] || platforms.html;

        root.querySelectorAll(".pe-platform-tab").forEach(button => {
            const active = button.dataset.pePlatform === platform;
            button.classList.toggle("active", active);
            button.setAttribute("aria-selected", active ? "true" : "false");
        });

        $("peSummaryPlatform").textContent = config.label;
        $("peCodeLabel").textContent = config.codeLabel;
        $("peInstructionHeading").textContent = config.heading;
        $("peInstructionDescription").textContent = config.description;
        $("peStepTwoHeading").textContent = config.stepTwoHeading;
        $("peStepTwoDescription").textContent = config.stepTwoDescription;
        $("peInstallTip").textContent = config.tip;

        renderCode();
    }

    function renderStatus () {
        const badge = $("peStatusBadge");
        badge.classList.remove("is-error");
        badge.dataset.status = hasDraft ? "draft" : isPublished ? "published" : "not-published";
        badge.classList.toggle("is-published", isPublished && !hasDraft);
        badge.classList.toggle("is-draft", hasDraft);
        badge.innerHTML = hasDraft
            ? '<i class="bi bi-circle-fill"></i> Draft ready to publish'
            : isPublished
                ? '<i class="bi bi-check-circle-fill"></i> Published'
                : '<i class="bi bi-circle-fill"></i> Not published';

        $("peSummaryStatus").textContent = hasDraft ? "Draft" : isPublished ? "Published" : "Not published";
        $("peStatusDescription").textContent = hasDraft
            ? (isPublished ? "A saved draft is ready; the current published version remains live." : "Saved draft is ready to publish.")
            : isPublished ? "Published version is live." : "Save a design draft before publishing.";

        const button = $("pePublishBtn");
        button.innerHTML = isPublishing
            ? '<i class="bi bi-arrow-repeat"></i> Publishing…'
            : hasDraft
                ? '<i class="bi bi-rocket-takeoff"></i> Publish Widget'
                : isPublished
                    ? '<i class="bi bi-check-lg"></i> Published'
                    : '<i class="bi bi-rocket-takeoff"></i> Publish Widget';
        button.disabled = !hasDraft || isPublishing;
        button.style.opacity = button.disabled ? ".75" : "1";
    }

    function renderDomains () {
        const list = $("peDomainList");
        list.innerHTML = "";

        domains.forEach(domain => {
            const row = document.createElement("div");
            row.className = "pe-domain-row";
            row.innerHTML =
                '<div class="pe-domain-icon"><i class="bi bi-globe2"></i></div>' +
                '<div class="pe-domain-details">' +
                '<strong>' + escapeHtml(domain.value) + '</strong>' +
                '<small><i class="bi bi-check-circle"></i> Allowlist entry (demo)</small>' +
                '</div>' +
                '<button type="button" class="pe-domain-remove" data-domain-id="' +
                domain.id + '" aria-label="Remove ' + escapeHtml(domain.value) +
                '" title="Remove domain"><i class="bi bi-trash3"></i></button>';

            list.appendChild(row);
        });

        $("peDomainCount").textContent = domains.length +
            (domains.length === 1 ? " domain" : " domains");
        $("peDomainListCount").textContent = domains.length;
        $("peDomainEmpty").hidden = domains.length !== 0;
        list.hidden = domains.length === 0;
    }

    function normalizeDomain (rawValue) {
        let value = rawValue.trim().toLowerCase();

        if (!value) return null;

        // Accept a hostname or a full URL, then retain only the hostname.
        try {
            if (/^https?:\/\//i.test(value)) {
                const parsed = new URL(value);
                if (parsed.username || parsed.password) return null;
                value = parsed.hostname;
            }
        } catch (_) {
            return null;
        }

        value = value.replace(/\.$/, "");

        if (value.length > 253 || value.includes("/") ||
            value.includes(":") || value.includes(" ") ||
            value.includes("*") || value.includes("?")) {
            return null;
        }

        const labels = value.split(".");
        if (labels.length < 2) return null;

        const validLabel = /^[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?$/;
        if (!labels.every(label => label.length <= 63 && validLabel.test(label))) {
            return null;
        }

        return value;
    }

    function showToast (message) {
        const toast = $("peToast");
        toast.textContent = message;
        toast.classList.add("is-visible");
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => toast.classList.remove("is-visible"), 2800);
    }

    function requestHeaders (json) {
        const headers = { Accept: "application/json" };
        if (json) headers[ "Content-Type" ] = "application/json";
        if (demoOwnerToken) headers[ "X-Demo-Owner-User-Token" ] = demoOwnerToken;
        return headers;
    }

    async function apiRequest (path, options) {
        const response = await fetch(apiBase + path, {
            ...(options || {}),
            headers: { ...requestHeaders(Boolean(options && options.body)), ...((options && options.headers) || {}) }
        });
        const responseText = await response.text();
        let data = {};
        try { data = responseText ? JSON.parse(responseText) : {}; } catch (_) { data = {}; }
        if (!response.ok) {
            throw new Error(data.message || ("API request failed (HTTP " + response.status + ")."));
        }
        return data;
    }

    async function loadWidgetStatus () {
        const widgetKey = $("peWidgetSelect").value;
        const widget = getWidget();
        const encodedKey = encodeURIComponent(widgetKey);
        const badge = $("peStatusBadge");
        badge.dataset.status = "loading";
        badge.innerHTML = '<i class="bi bi-arrow-repeat"></i> Checking';

        try {
            const results = await Promise.allSettled([
                apiRequest("/widgets/" + encodedKey + "/draft/configuration"),
                apiRequest("/widgets/" + encodedKey + "/version"),
                apiRequest("/widgets")
            ]);
            const draftResponse = results[ 0 ].status === "fulfilled" ? results[ 0 ].value : null;
            const publishedResponse = results[ 1 ].status === "fulfilled" ? results[ 1 ].value : null;
            const inventory = results[ 2 ].status === "fulfilled" ? results[ 2 ].value : null;
            const metadata = Array.isArray(inventory && inventory.widgets)
                ? inventory.widgets.find(item => String(item.publicKey ?? item.PublicKey ?? "") === widgetKey)
                : null;
            if (metadata) {
                widget.draftVersionNumber = String(metadata.draftVersionNumber ?? metadata.DraftVersionNumber ?? "");
                widget.publishedVersionNumber = String(metadata.publishedVersionNumber ?? metadata.PublishedVersionNumber ?? "");
            }
            hasDraft = !!draftResponse && String(draftResponse.status || "").toLowerCase() === "draft";
            isPublished = !!publishedResponse;

            const currentVersion = hasDraft
                ? (draftResponse.versionNumber || widget.draftVersionNumber || widget.versionNumber)
                : publishedResponse && publishedResponse.versionNumber;
            widget.version = currentVersion ? String(currentVersion) : "—";
            const liveVersion = publishedResponse && publishedResponse.versionNumber || widget.publishedVersionNumber;
            $("peVersion").textContent = hasDraft
                ? "Draft v" + (currentVersion || "—") + (isPublished && liveVersion ? " · Live v" + liveVersion : "")
                : liveVersion ? "Live v" + liveVersion : "—";
            renderStatus();

            if (!draftResponse && !publishedResponse) {
                const reason = results[ 0 ].reason || results[ 1 ].reason;
                showToast(reason && reason.message ? reason.message : "Could not load this widget. Check its public key and API settings.");
                badge.dataset.status = "error";
                badge.classList.add("is-error");
            }
        } catch (error) {
            hasDraft = false;
            isPublished = false;
            $("peVersion").textContent = "—";
            renderStatus();
            $("peStatusBadge").dataset.status = "error";
            $("peStatusBadge").classList.add("is-error");
            showToast(error.message || "Could not load widget publishing status.");
        } finally {
            root.dispatchEvent(new CustomEvent("pe:version-ready", { detail: { publicKey: widgetKey } }));
        }
    }

    async function publishWidget () {
        if (!hasDraft || isPublishing) return;
        const widgetKey = $("peWidgetSelect").value;
        if (!window.confirm("Publish this saved draft? It will replace the currently published version.")) return;

        isPublishing = true;
        renderStatus();
        try {
            const result = await apiRequest("/widgets/" + encodeURIComponent(widgetKey) + "/publish", {
                method: "POST"
            });
            showToast("Widget published successfully as version " + (result.versionNumber || "latest") + ".");
            await loadWidgetStatus();
        } catch (error) {
            showToast(error.message || "Could not publish this widget.");
        } finally {
            isPublishing = false;
            renderStatus();
        }
    }

    async function copyText (text) {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return;
        }

        const temporary = document.createElement("textarea");
        temporary.value = text;
        temporary.setAttribute("readonly", "");
        temporary.style.position = "fixed";
        temporary.style.opacity = "0";
        document.body.appendChild(temporary);
        temporary.select();

        const copied = document.execCommand("copy");
        temporary.remove();

        if (!copied) throw new Error("Copy command was not available.");
    }

    root.querySelectorAll(".pe-platform-tab").forEach(button => {
        button.addEventListener("click", () => {
            renderPlatform(button.dataset.pePlatform);
        });
    });

    $("peWidgetSelect").addEventListener("change", () => {
        const widget = getWidget();
        $("peVersion").textContent = widget.version === "—" ? "—" : "v" + widget.version;
        renderCode();
        showToast("Selected " + widget.name + ".");
        loadWidgetStatus();
    });

    $("peCopyCode").addEventListener("click", async () => {
        try {
            await copyText($("peEmbedCode").textContent);
            $("peCopyCode").innerHTML = '<i class="bi bi-check2"></i> Copied';
            showToast("Embed snippet copied.");
            setTimeout(() => {
                if (root.isConnected) {
                    $("peCopyCode").innerHTML = '<i class="bi bi-clipboard"></i> Copy code';
                }
            }, 1800);
        } catch (_) {
            showToast("Copy was blocked by the browser. Select the code and copy it manually.");
        }
    });

    $("pePublishBtn").addEventListener("click", publishWidget);

    $("peDomainForm").addEventListener("submit", event => {
        event.preventDefault();

        const input = $("peDomainInput");
        const error = $("peDomainError");
        const domain = normalizeDomain(input.value);

        error.textContent = "";

        if (!domain) {
            error.textContent = "Enter a valid domain, such as example.com.";
            input.focus();
            return;
        }

        if (domains.some(item => item.value === domain)) {
            error.textContent = "This domain is already in the list.";
            input.focus();
            return;
        }

        domains.push({
            id: Date.now(),
            value: domain
        });

        input.value = "";
        renderDomains();
        showToast("Domain added to the mock allowlist.");
    });

    $("peDomainList").addEventListener("click", event => {
        const button = event.target.closest("[data-domain-id]");
        if (!button) return;

        const id = Number(button.dataset.domainId);
        const domain = domains.find(item => item.id === id);
        if (!domain) return;

        domains = domains.filter(item => item.id !== id);
        renderDomains();
        showToast(domain.value + " removed from the mock allowlist.");
    });

    // The checklist is a front-end readiness aid; it does not gate publishing.
    renderPlatform("html");
    renderDomains();
    renderCode();
    loadWidgetStatus();
})();


(function initializePublishChecklist () {
    const root = document.querySelector(".publish-embed-page");
    const checkboxes = Array.from(
        document.querySelectorAll(".pe-checklist-checkbox")
    );

    const status = document.getElementById("peChecklistStatus");
    const progressBar = document.getElementById("peChecklistProgressBar");
    const widgetSelect = document.getElementById("peWidgetSelect");
    const completionPanel = document.getElementById("peChecklistComplete");
    const nextButton = document.getElementById("peChecklistNext");
    const pageHeader = document.querySelector(".publish-embed-page .pe-page-header");

    if (!checkboxes.length || !status || !progressBar) {
        return;
    }

    const storageKey = () => {
        const widgetKey = widgetSelect?.value || "default";
        const versionText = document.getElementById("peVersion")?.textContent || "";
        const versionMatch = versionText.match(/(?:Draft|Live)\s+v([^\s·]+)/i);
        const versionKey = versionMatch ? versionMatch[ 1 ] : "unknown-version";
        return "askpearls.publish-checklist." + widgetKey + "." + versionKey;
    };

    function restoreChecklist () {
        let saved = null;
        try {
            saved = JSON.parse(window.localStorage.getItem(storageKey()) || "null");
        } catch (_) {
            // Keep the checklist usable when storage is disabled or unavailable.
        }

        checkboxes.forEach(checkbox => {
            checkbox.checked = Boolean(saved && saved[ checkbox.id ]);
        });
        updateChecklist();
    }

    function updateChecklist (persist = true) {
        const completed = checkboxes.filter(
            checkbox => checkbox.checked
        ).length;

        const total = checkboxes.length;
        const percentage = total === 0
            ? 0
            : (completed / total) * 100;

        status.textContent =
            `${ completed } of ${ total } steps completed`;

        progressBar.style.width = `${ percentage }%`;
        progressBar.setAttribute("aria-valuenow", String(completed));
        progressBar.setAttribute("aria-valuemax", String(total));
        if (completionPanel) {
            completionPanel.hidden = completed !== total;
        }

        if (!persist) return;
        try {
            const saved = Object.fromEntries(
                checkboxes.map(checkbox => [ checkbox.id, checkbox.checked ])
            );
            window.localStorage.setItem(storageKey(), JSON.stringify(saved));
        } catch (_) {
            // Progress still updates for this page view if storage is unavailable.
        }
    }

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", updateChecklist);
    });

    widgetSelect?.addEventListener("change", () => {
        checkboxes.forEach(checkbox => { checkbox.checked = false; });
        updateChecklist(false);
    });
    root.addEventListener("pe:version-ready", event => {
        if (event.detail?.publicKey === (widgetSelect?.value || "")) restoreChecklist();
    });
    nextButton?.addEventListener("click", () => {
        pageHeader?.scrollIntoView({ behavior: "smooth", block: "start" });
        window.setTimeout(() => {
            document.getElementById("pePublishBtn")?.focus({ preventScroll: true });
        }, 350);
    });
    checkboxes.forEach(checkbox => { checkbox.checked = false; });
    updateChecklist(false);
})();

