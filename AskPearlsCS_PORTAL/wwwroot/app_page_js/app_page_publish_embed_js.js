
(function () {
    "use strict";

    const root = document.querySelector(".publish-embed-page");
    if (!root || root.dataset.peInitialized === "true") return;
    root.dataset.peInitialized = "true";

    const $ = id => root.querySelector("#" + id);

    const widgets = {
        pk_demo_sofa_finder: {
            name: "Sofa Finder Widget",
            version: "1.0.0"
        },
        pk_furniture_recommendation: {
            name: "Furniture Recommendation Widget",
            version: "1.0.0"
        },
        pk_demo_recommendation: {
            name: "Demo Recommendation Widget",
            version: "1.0.0"
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
    let toastTimer = null;
    let domains = [
        { id: 1, value: "example.com" },
        { id: 2, value: "store.example.com" }
    ];

    function escapeHtml(value) {
        return String(value ?? "").replace(/[&<>"']/g, ch => ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#39;"
        })[ch]);
    }

    function getWidget() {
        return widgets[$("peWidgetSelect").value] || widgets.pk_demo_sofa_finder;
    }

    function getEmbedCode() {
        const widgetId = $("peWidgetSelect").value;

        // Illustrative placeholder only. Replace the CDN URL with the real
        // production widget loader URL before using this snippet on a live site.
        return '<!-- AskPearls Widget -->\n' +
            '<div id="askpearls-widget" data-widget-id="' + widgetId + '"></div>\n' +
            '<script src="https://cdn.askpearls.com/widget.js" defer><' + '/script>';
    }

    function renderCode() {
        $("peEmbedCode").textContent = getEmbedCode();
    }

    function renderPlatform(platform) {
        activePlatform = platform;
        const config = platforms[platform] || platforms.html;

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

    function renderStatus() {
        const badge = $("peStatusBadge");
        badge.classList.toggle("is-published", isPublished);
        badge.innerHTML = isPublished
            ? '<i class="bi bi-check-circle-fill"></i> Published'
            : '<i class="bi bi-circle-fill"></i> Draft';

        $("peSummaryStatus").textContent = isPublished ? "Published" : "Draft";
        $("peStatusDescription").textContent = isPublished
            ? "Published in this demo session"
            : "Not published yet";

        $("pePublishBtn").innerHTML = isPublished
            ? '<i class="bi bi-check-lg"></i> Published'
            : '<i class="bi bi-rocket-takeoff"></i> Publish Widget';

        $("pePublishBtn").disabled = isPublished;
        $("pePublishBtn").style.opacity = isPublished ? ".75" : "1";
    }

    function renderDomains() {
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

    function normalizeDomain(rawValue) {
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

    function showToast(message) {
        const toast = $("peToast");
        toast.textContent = message;
        toast.classList.add("is-visible");
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => toast.classList.remove("is-visible"), 2800);
    }

    async function copyText(text) {
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
        $("peVersion").textContent = "v" + widget.version;
        renderCode();
        showToast("Selected " + widget.name + ".");
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

    $("pePublishBtn").addEventListener("click", () => {
        if (isPublished) return;

        isPublished = true;
        renderStatus();
        showToast("Demo publish complete. No backend request was made.");
    });

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
    renderStatus();
    renderDomains();
})();


(function initializePublishChecklist () {
    const checkboxes = Array.from(
        document.querySelectorAll(".pe-checklist-checkbox")
    );

    const status = document.getElementById("peChecklistStatus");
    const progressBar = document.getElementById("peChecklistProgressBar");

    if (!checkboxes.length || !status || !progressBar) {
        return;
    }

    function updateChecklist () {
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
    }

    checkboxes.forEach(checkbox => {
        checkbox.addEventListener("change", updateChecklist);
    });

    updateChecklist();
})();

