(function () {
    "use strict";

    const root = document.querySelector(".page-widget-builder[data-ap-page='widget-builder']");
    if (!root || root.dataset.wbInitialized === "true") return;
    root.dataset.wbInitialized = "true";

    const $ = id => root.querySelector("#" + id);
    const apiBase = (root.dataset.apiBase || window.AskPearlsCS_API_URL || (window.location.origin + "/api")).replace(/\/+$/, "");
    const ownerToken = (root.dataset.demoOwnerUserToken || "").trim();
    const steps = [ "basics", "appearance", "behavior" ];
    let currentStep = 0;
    let widgets = [];
    let savedSettings = null;
    let toastTimer;
    let creating = window.localStorage.getItem("askpearls.widgetBuilderMode") === "create";

    const defaults = {
        primary: "#16a085", background: "#FFFFFF", text: "#17212B", theme: "custom",
        font: "Inter, Arial, sans-serif", fontSize: 14, radius: 12, shadow: true,
        width: 360, position: "bottom-right", buttonShape: "rounded",
        heading: "Find Your Perfect Sofa", description: "Answer a few simple questions and discover the sofa that suits your needs.",
        buttonText: "Find My Sofa", footer: "Powered by DecisionOS"
    };

    function showMessage (message, kind) {
        const box = $("wbApiMessage");
        box.textContent = message;
        box.className = "wb-api-message" + (kind ? " is-" + kind : "");
        box.hidden = false;
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => { box.hidden = true; }, 5500);
    }

    function headers (json) {
        const result = { Accept: "application/json" };
        if (json) result[ "Content-Type" ] = "application/json";
        if (ownerToken) result[ "X-Demo-Owner-User-Token" ] = ownerToken;
        return result;
    }

    async function readResponse (response) {
        const text = await response.text();
        let data = {};
        if (text) {
            try { data = JSON.parse(text); }
            catch { throw new Error("The API returned an unreadable response."); }
        }
        if (!response.ok) throw new Error(data.message || "API request failed (HTTP " + response.status + ").");
        return data;
    }

    function selectedWidget () {
        return widgets.find(widget => widget.publicKey === $("wbWidgetSelect").value) || null;
    }

    function setDraftState (isDirty) {
        const indicator = $("wbDraftIndicator");
        indicator.innerHTML = isDirty
            ? '<i class="bi bi-circle-fill me-1" style="font-size:7px"></i> Unsaved changes'
            : '<i class="bi bi-check-circle-fill me-1"></i> Saved settings loaded';
        if (creating) {
            const website = $("wbWebsite").value.trim();
            $("wbSaveButton").disabled = !$("wbWidgetName").value.trim() ||
                !$("wbWidgetTitle").value.trim() || !/^https?:\/\//i.test(website);
        } else {
            $("wbSaveButton").disabled = !selectedWidget() || !isDirty;
        }
    }

    function currentSettings () {
        const theme = root.querySelector('input[name="wbTheme"]:checked');
        const corner = $("wbCornerStyle").value;
        return {
            primary: $("wbPrimaryColor").value,
            background: theme && theme.value === "dark" ? "#15231E" : "#FFFFFF",
            text: theme && theme.value === "dark" ? "#E7F0EC" : "#17212B",
            theme: theme ? theme.value : "light",
            font: "Inter, Arial, sans-serif",
            fontSize: 14,
            radius: corner === "soft" ? 24 : corner === "square" ? 0 : 12,
            shadow: true,
            width: 360,
            position: $("wbWidgetPosition").value,
            buttonShape: corner === "square" ? "square" : "rounded",
            heading: $("wbWidgetTitle").value.trim(),
            description: $("wbWidgetDescription").value.trim(),
            buttonText: $("wbButtonText").value.trim(),
            footer: "Powered by DecisionOS"
        };
    }

    function syncDirtyState () {
        if (savedSettings) setDraftState(JSON.stringify(currentSettings()) !== JSON.stringify(savedSettings));
    }

    function updatePreview () {
        const title = $("wbWidgetTitle").value.trim();
        const description = $("wbWidgetDescription").value.trim();
        const buttonText = $("wbButtonText").value.trim();
        const theme = root.querySelector('input[name="wbTheme"]:checked')?.value || "light";
        const corners = $("wbCornerStyle").value;
        const color = $("wbPrimaryColor").value;
        $("wbPreviewTitle").textContent = title || "Your Widget Title";
        $("wbPreviewTitle").hidden = !$("wbShowTitle").checked;
        $("wbPreviewDescription").textContent = description || "Your widget description.";
        $("wbPreviewDescription").hidden = !$("wbShowDescription").checked;
        $("wbPreviewPrimary").textContent = buttonText || "Find My Sofa";
        $("wbPreviewWidget").dataset.theme = theme;
        $("wbPreviewWidget").dataset.corners = corners;
        root.style.setProperty("--wb-primary", color);
        $("wbColorValue").textContent = color;
        syncDirtyState();
    }

    function applySettings (config) {
        const value = { ...defaults, ...(config || {}) };
        const color = /^#[0-9a-f]{6}$/i.test(value.primary) ? value.primary : defaults.primary;
        $("wbPrimaryColor").value = color;
        $("wbWidgetTitle").value = value.heading || defaults.heading;
        $("wbWidgetDescription").value = value.description || defaults.description;
        $("wbButtonText").value = value.buttonText || defaults.buttonText;
        $("wbWidgetPosition").value = [ "bottom-right", "bottom-left", "center" ].includes(value.position) ? value.position : defaults.position;
        $("wbCornerStyle").value = value.buttonShape === "square" ? "square" : Number(value.radius) >= 20 ? "soft" : "rounded";
        const theme = [ "light", "dark" ].includes(value.theme) ? value.theme : "light";
        const themeInput = root.querySelector('input[name="wbTheme"][value="' + theme + '"]');
        if (themeInput) themeInput.checked = true;
        updatePreview();
        savedSettings = currentSettings();
        setDraftState(false);
    }

    async function loadConfiguration () {
        const widget = selectedWidget();
        const version = $("wbWidgetVersion");
        const save = $("wbSaveButton");
        save.disabled = true;
        version.textContent = widget ? (widget.hasDraft ? "Draft v" + (widget.draftVersionNumber || widget.versionNumber) : widget.hasPublishedVersion ? "Live v" + (widget.publishedVersionNumber || widget.versionNumber) : "—") : "—";
        if (!widget) return;

        try {
            const url = apiBase + "/widgets/" + encodeURIComponent(widget.publicKey) + "/draft/configuration";
            const data = await readResponse(await fetch(url, { headers: headers(false) }));
            if (selectedWidget()?.publicKey !== widget.publicKey) return;
            version.textContent = data.versionNumber ? (data.status === "draft" ? "Draft v" : "Live v") + data.versionNumber + (widget.hasPublishedVersion && widget.publishedVersionNumber ? " · Live v" + widget.publishedVersionNumber : "") : version.textContent;
            $("wbWidgetName").value = widget.name || "";
            $("wbWidgetType").value = widget.type || "decision";
            $("wbWebsite").value = widget.websiteAddress || "";
            applySettings(data.configuration || defaults);
            showMessage(data.status === "draft" ? "Saved draft settings loaded for " + widget.name + "." : "Current settings loaded for " + widget.name + ".", "success");
        } catch (error) {
            console.error("Widget Builder could not load the selected widget configuration.", error);
            applySettings(defaults);
            showMessage(error.message || "Could not load widget settings.", "error");
        }
    }

    async function loadWidgets () {
        const select = $("wbWidgetSelect");
        select.disabled = true;
        try {
            const data = await readResponse(await fetch(apiBase + "/widgets", { headers: headers(false) }));
            widgets = (Array.isArray(data.widgets) ? data.widgets : []).map(item => ({
                publicKey: String(item.publicKey ?? item.PublicKey ?? ""),
                name: String(item.name ?? item.Name ?? "Untitled widget"),
                description: String(item.description ?? item.Description ?? ""),
                versionNumber: String(item.versionNumber ?? item.VersionNumber ?? ""),
                draftVersionNumber: String(item.draftVersionNumber ?? item.DraftVersionNumber ?? ""),
                publishedVersionNumber: String(item.publishedVersionNumber ?? item.PublishedVersionNumber ?? ""),
                hasDraft: Boolean(item.hasDraft ?? item.HasDraft ?? String(item.status ?? item.Status ?? "").toLowerCase() === "draft"),
                hasPublishedVersion: Boolean(item.hasPublishedVersion ?? item.HasPublishedVersion ?? String(item.status ?? item.Status ?? "").toLowerCase() === "published"),
                websiteTokenId: String(item.websiteTokenId ?? item.WebsiteTokenId ?? ""),
                websiteAddress: String(item.websiteAddress ?? item.WebsiteAddress ?? ""),
                websiteName: String(item.websiteName ?? item.WebsiteName ?? ""),
                type: String(item.type ?? item.Type ?? "decision")
            })).filter(item => item.publicKey);
            select.replaceChildren();
            if (creating) {
                select.add(new Option("New widget", ""));
                select.disabled = true;
                $("wbPageTitle").textContent = "Create New Widget";
                $("wbPageDescription").textContent = "Set up a new DecisionOS experience for your website.";
                $("wbApiNote").textContent = "Complete the required fields, then create the widget as a draft. You can add questions, recommendations, and scoring after creation.";
                $("wbSaveButton").innerHTML = '<i class="bi bi-plus-lg me-1"></i> Create Widget';
                $("wbWidgetName").value = "";
                $("wbWidgetType").value = "decision";
                $("wbWebsite").value = "";
                $("wbWidgetTitle").value = "";
                $("wbWidgetDescription").value = "";
                $("wbButtonText").value = "Get started";
                $("wbWidgetVersion").textContent = "—";
                updatePreview();
                setDraftState(true);
                return;
            }
            $("wbPageTitle").textContent = "Widget Builder";
            $("wbPageDescription").textContent = "Create and customize your DecisionOS experience.";
            $("wbApiNote").textContent = "Choose an existing widget to edit, or open this page from “Create new widget” to start a new widget.";
            $("wbSaveButton").innerHTML = '<i class="bi bi-save me-1"></i> Save Draft';
            if (!widgets.length) {
                select.add(new Option("No widgets available", ""));
                $("wbWidgetVersion").textContent = "—";
                showMessage("There are no existing widgets. Use Create new widget to start one.", "warning");
                return;
            }
            widgets.forEach(widget => select.add(new Option(widget.name, widget.publicKey)));
            select.disabled = false;
            await loadConfiguration();
        } catch (error) {
            select.replaceChildren(new Option("Could not load widgets", ""));
            showMessage(error.message || "Could not load widgets from the API.", "error");
            console.error("Widget Builder could not load widgets.", error);
        }
    }

    async function saveDraft () {
        if (creating) {
            const websiteAddress = $("wbWebsite").value.trim();
            if (!/^https?:\/\//i.test(websiteAddress)) return showMessage("Enter a website address beginning with http:// or https://.", "warning");
            const button = $("wbSaveButton");
            button.disabled = true;
            try {
                const config = currentSettings();
                const data = await readResponse(await fetch(apiBase + "/widgets", {
                    method: "POST", headers: headers(true),
                    body: JSON.stringify({
                        name: $("wbWidgetName").value.trim(), type: $("wbWidgetType").value,
                        websiteTokenId: "", websiteName: new URL(websiteAddress).hostname,
                        websiteAddress, title: config.heading, description: config.description
                    })
                }));
                window.localStorage.setItem("askpearls.widgetBuilderMode", "edit");
                window.localStorage.setItem("askpearls.activeWidgetPublicKey", data.publicKey);
                creating = false;
                showMessage("Widget created as a draft. You can now add its questions and recommendations.", "success");
                widgets = [];
                await loadWidgets();
                const select = $("wbWidgetSelect");
                select.value = data.publicKey;
                await loadConfiguration();
            } catch (error) {
                button.disabled = false;
                showMessage(error.message || "Could not create this widget.", "error");
            }
            return;
        }
        const widget = selectedWidget();
        if (!widget) return showMessage("Select an existing widget before saving its design draft.", "warning");
        const button = $("wbSaveButton");
        button.disabled = true;
        try {
            const url = apiBase + "/widgets/" + encodeURIComponent(widget.publicKey) + "/draft/configuration";
            const data = await readResponse(await fetch(url, {
                method: "PUT", headers: headers(true), body: JSON.stringify(currentSettings())
            }));
            savedSettings = currentSettings();
            setDraftState(false);
            if (data.versionNumber) $("wbWidgetVersion").textContent = "Draft v" + data.versionNumber + (widget.hasPublishedVersion && widget.publishedVersionNumber ? " · Live v" + widget.publishedVersionNumber : "");
            const selected = selectedWidget();
            if (selected && data.versionNumber) {
                selected.versionNumber = String(data.versionNumber);
                selected.draftVersionNumber = String(data.versionNumber);
                selected.hasDraft = true;
            }
            showMessage("Design draft saved for " + widget.name + (data.versionNumber ? " as version " + data.versionNumber : "") + ".", "success");
        } catch (error) {
            button.disabled = false;
            showMessage(error.message || "Could not save the design draft.", "error");
            console.error("Widget Builder could not save the design draft.", error);
        }
    }

    function showStep (index) {
        currentStep = Math.max(0, Math.min(index, steps.length - 1));
        root.querySelectorAll(".wb-step").forEach(button => button.classList.toggle("active", button.dataset.wbTab === steps[ currentStep ]));
        root.querySelectorAll(".wb-panel").forEach(panel => panel.classList.toggle("active", panel.dataset.wbPanel === steps[ currentStep ]));
        $("wbPreviousButton").disabled = currentStep === 0;
        $("wbNextButton").innerHTML = currentStep === steps.length - 1
            ? 'Finish <i class="bi bi-check2 ms-1"></i>'
            : 'Next <i class="bi bi-arrow-right ms-1"></i>';
    }

    root.querySelectorAll(".wb-step").forEach(button => button.addEventListener("click", () => showStep(steps.indexOf(button.dataset.wbTab))));
    $("wbPreviousButton").addEventListener("click", () => showStep(currentStep - 1));
    $("wbNextButton").addEventListener("click", () => currentStep < steps.length - 1
        ? showStep(currentStep + 1)
        : showMessage("Builder review is ready. Save the supported design settings for the selected widget.", "warning"));
    $("wbWidgetSelect").addEventListener("change", loadConfiguration);
    $("wbSaveButton").addEventListener("click", saveDraft);
    $("wbPreviewButton").addEventListener("click", () => $("wbPreviewStage").scrollIntoView({ behavior: "smooth", block: "center" }));
    root.querySelectorAll("input, textarea, select").forEach(control => {
        control.addEventListener("input", updatePreview);
        control.addEventListener("change", updatePreview);
    });
    [ "wbWidgetName", "wbWidgetType", "wbWebsite" ].forEach(id => {
        $(id).addEventListener("input", updatePreview);
        $(id).addEventListener("change", updatePreview);
    });
    $("wbPreviewPrimary").addEventListener("click", () => showMessage("Visitor question flows are managed in the Questions & Answers workspace.", "warning"));

    showStep(0);
    updatePreview();
    loadWidgets();
})();
