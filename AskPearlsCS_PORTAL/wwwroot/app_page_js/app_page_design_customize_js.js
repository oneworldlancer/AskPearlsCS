(function () {
    "use strict";

    const root = document.querySelector(".design-customize-page");
    if (!root || root.dataset.dcInitialized === "true") return;
    root.dataset.dcInitialized = "true";

    const $ = id => root.querySelector("#" + id);
    const defaults = {
        primary: "#0DA88B",
        background: "#FFFFFF",
        text: "#17212B",
        theme: "custom",
        font: "Inter, Arial, sans-serif",
        fontSize: 14,
        radius: 12,
        shadow: true,
        width: 360,
        position: "bottom-right",
        buttonShape: "rounded",
        heading: "Find your perfect sofa",
        description: "Answer a few quick questions and discover the sofa that fits your home.",
        buttonText: "Get started",
        footer: "Powered by AskPearls"
    };

    const apiBase = (root.dataset.apiBase || window.AskPearlsCS_API_URL || (window.location.origin + "/api")).replace(/\/+$/, "");
    const demoOwnerToken = (root.dataset.demoOwnerUserToken || "").trim();
    const defaultPublicKey = (root.dataset.widgetPublicKey || "").trim();
    let savedSettings = { ...defaults };
    let toastTimer = null;

    function currentPublicKey () {
        const select = $("dcWidgetSelect");
        const option = select && select.options[ select.selectedIndex ];
        return (option && option.dataset.publicKey) ||
            (select && select.value === "sofa" ? defaultPublicKey : "") ||
            (select && /^pk_[a-zA-Z0-9_-]+$/.test(select.value) ? select.value : "");
    }

    function currentSettings () {
        return {
            primary: $("dcPrimaryColor").value,
            background: $("dcBackgroundColor").value,
            text: $("dcTextColor").value,
            theme: $("dcTheme").value,
            font: $("dcFontFamily").value,
            fontSize: Number($("dcFontSize").value),
            radius: Number($("dcRadius").value),
            shadow: $("dcShadow").checked,
            width: Number($("dcWidth").value),
            position: $("dcPosition").value,
            buttonShape: $("dcButtonShape").value,
            heading: $("dcHeading").value,
            description: $("dcDescription").value,
            buttonText: $("dcButtonText").value,
            footer: $("dcFooterText").value
        };
    }

    function validHex (value) {
        return /^#[0-9a-fA-F]{6}$/.test(value);
    }

    function syncBackgroundToTheme (theme) {
        const background = theme === "dark"
            ? "#15231E"
            : theme === "light" ? "#FFFFFF" : null;
        if (!background) return;
        $("dcBackgroundColor").value = background;
        $("dcBackgroundHex").value = background;
    }

    function showToast (message) {
        const toast = $("dcToast");
        toast.textContent = message;
        toast.classList.add("is-visible");
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => toast.classList.remove("is-visible"), 3500);
    }

    function syncColorFromText (textId, colorId) {
        const value = $(textId).value.trim();
        if (!validHex(value)) {
            showToast("Enter a valid 6-digit hex color, for example #0DA88B.");
            return;
        }
        $(colorId).value = value;
        if (colorId !== "dcPrimaryColor") $("dcTheme").value = "custom";
        updatePreview();
    }

    function syncColorFromPicker (colorId, textId) {
        $(textId).value = $(colorId).value.toUpperCase();
        if (colorId !== "dcPrimaryColor") $("dcTheme").value = "custom";
        updatePreview();
    }

    function applySettings (settings) {
        const s = { ...defaults, ...(settings || {}) };
        $("dcPrimaryColor").value = validHex(s.primary) ? s.primary : defaults.primary;
        $("dcPrimaryHex").value = $("dcPrimaryColor").value.toUpperCase();
        const theme = [ "custom", "light", "dark" ].includes(s.theme) ? s.theme : defaults.theme;
        const background = theme === "dark" ? "#15231E"
            : theme === "light" ? "#FFFFFF"
                : (validHex(s.background) ? s.background : defaults.background);
        $("dcBackgroundColor").value = background;
        $("dcBackgroundHex").value = $("dcBackgroundColor").value.toUpperCase();
        $("dcTextColor").value = validHex(s.text) ? s.text : defaults.text;
        $("dcTextHex").value = $("dcTextColor").value.toUpperCase();
        $("dcTheme").value = theme;
        $("dcFontFamily").value = s.font;
        $("dcFontSize").value = s.fontSize;
        $("dcRadius").value = s.radius;
        $("dcShadow").checked = Boolean(s.shadow);
        $("dcWidth").value = s.width;
        $("dcPosition").value = s.position;
        $("dcButtonShape").value = s.buttonShape;
        $("dcHeading").value = s.heading;
        $("dcDescription").value = s.description;
        $("dcButtonText").value = s.buttonText;
        $("dcFooterText").value = s.footer;
        updatePreview();
    }

    function setPreviewMode (mode) {
        const canvas = $("dcPreviewCanvas");
        canvas.dataset.mode = mode;
        root.querySelectorAll(".dc-preview-mode").forEach(button => {
            button.classList.toggle("active", button.dataset.dcMode === mode);
        });

        const s = currentSettings();
        const previewTheme = mode === "dark" ? "dark" : s.theme;
        const savedText = /^#[0-9a-f]{6}$/i.test(s.text || "") ? s.text : "#17212B";
        const previewText = previewTheme === "dark" && savedText.toUpperCase() === "#17212B"
            ? "#E7F0EC"
            : savedText;
        const colors = previewTheme === "dark"
            ? { background: "#15231E", text: previewText, border: "rgba(231,240,236,.2)" }
            : previewTheme === "light"
                ? { background: "#FFFFFF", text: previewText, border: "rgba(100,120,110,.2)" }
                : { background: s.background, text: previewText, border: "rgba(100,120,110,.2)" };
        const widget = $("dcWidgetPreview");
        widget.style.backgroundColor = colors.background;
        widget.style.color = colors.text;
        $("dcPreviewHeading").style.color = colors.text;
        $("dcPreviewDescription").style.color = colors.text;
        $("dcPreviewFooter").style.color = colors.text;
        root.querySelectorAll(".dc-preview-answer").forEach(answer => {
            answer.style.color = colors.text;
            answer.style.borderColor = colors.border;
        });

        widget.style.width = mode === "mobile"
            ? "min(100%, 320px)"
            : "min(100%, " + currentSettings().width + "px)";
    }

    function shapeRadius (shape) {
        if (shape === "pill") return "999px";
        if (shape === "square") return "2px";
        return $("dcRadius").value + "px";
    }

    function escapeHtml (value) {
        return String(value ?? "").replace(/[&<>"']/g, character => ({
            "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
        })[ character ]);
    }

    function updateDirtyState () {
        const changed = JSON.stringify(currentSettings()) !== JSON.stringify(savedSettings);
        const indicator = $("dcDraftIndicator");
        indicator.innerHTML = changed
            ? '<i class="bi bi-circle-fill"></i> Unsaved changes'
            : '<i class="bi bi-check-circle-fill"></i> Saved settings loaded';
        indicator.style.color = changed ? "" : "#0d9278";
    }

    function updatePreview () {
        const s = currentSettings();
        const widget = $("dcWidgetPreview");
        root.style.setProperty("--dc-primary", s.primary);
        root.style.setProperty("--dc-widget-bg", s.background);
        root.style.setProperty("--dc-widget-text", s.text);
        widget.style.fontFamily = s.font;
        widget.style.fontSize = s.fontSize + "px";
        widget.style.borderRadius = s.radius + "px";
        widget.style.boxShadow = s.shadow ? "0 18px 55px rgba(20,35,30,.18)" : "none";
        widget.style.width = "min(100%, " + s.width + "px)";

        $("dcPreviewHeading").textContent = s.heading || "Your welcome heading";
        $("dcPreviewHeading").style.fontSize = Math.max(16, s.fontSize + 6) + "px";
        $("dcPreviewDescription").textContent = s.description || "Your welcome message will appear here.";
        $("dcPreviewButton").innerHTML = escapeHtml(s.buttonText || "Get started") + ' <i class="bi bi-arrow-right"></i>';
        $("dcPreviewFooter").textContent = s.footer;

        root.querySelectorAll(".dc-preview-answer").forEach(answer => {
            answer.style.borderRadius = shapeRadius(s.buttonShape);
        });
        $("dcPreviewButton").style.borderRadius = shapeRadius(s.buttonShape);
        root.querySelector("#dcPreviewPosition").dataset.position = s.position;
        $("dcFontSizeValue").textContent = s.fontSize + " px";
        $("dcRadiusValue").textContent = s.radius + " px";
        $("dcWidthValue").textContent = s.width + " px";

        const activeMode = root.querySelector(".dc-preview-mode.active");
        setPreviewMode(activeMode ? activeMode.dataset.dcMode : "light");
        updateDirtyState();
    }

    function requestHeaders (json) {
        const headers = { "Accept": "application/json" };
        if (json) headers[ "Content-Type" ] = "application/json";
        if (demoOwnerToken) headers[ "X-Demo-Owner-User-Token" ] = demoOwnerToken;
        return headers;
    }

    async function readResponse (response) {
        const text = await response.text();
        let data = null;
        if (text) {
            try { data = JSON.parse(text); }
            catch { throw new Error("The API returned an unreadable response."); }
        }
        if (!response.ok) {
            throw new Error((data && data.message) || ("API request failed (HTTP " + response.status + ")."));
        }
        return data || {};
    }

    async function loadSettings (showMessage) {
        const publicKey = currentPublicKey();
        if (!publicKey) {
            applySettings(defaults);
            savedSettings = currentSettings();
            showToast("This widget has no API public key configured yet.");
            return;
        }

        try {
            const response = await fetch(apiBase + "/widgets/" + encodeURIComponent(publicKey) + "/draft/configuration", {
                method: "GET",
                headers: requestHeaders(false)
            });
            const data = await readResponse(response);
            const configuration = data.configuration || defaults;
            applySettings(configuration);
            savedSettings = currentSettings();
            if (showMessage) showToast(data.status === "draft" ? "Draft settings loaded." : "Published settings loaded.");
        } catch (error) {
            console.error("Could not load widget design settings.", error);
            showToast(error.message || "Could not load widget settings.");
        }
    }

    async function saveDraft () {
        const publicKey = currentPublicKey();
        if (!publicKey) {
            showToast("This widget has no API public key configured yet.");
            return;
        }

        const button = $("dcSave");
        button.disabled = true;
        try {
            const response = await fetch(apiBase + "/widgets/" + encodeURIComponent(publicKey) + "/draft/configuration", {
                method: "PUT",
                headers: requestHeaders(true),
                body: JSON.stringify(currentSettings())
            });
            const data = await readResponse(response);
            savedSettings = currentSettings();
            updateDirtyState();
            showToast("Draft saved" + (data.versionNumber ? " as version " + data.versionNumber : "") + ". Publish it when ready.");
        } catch (error) {
            console.error("Could not save widget design draft.", error);
            showToast(error.message || "Could not save the draft.");
        } finally {
            button.disabled = false;
        }
    }

    [ [ "dcPrimaryColor", "dcPrimaryHex" ], [ "dcBackgroundColor", "dcBackgroundHex" ], [ "dcTextColor", "dcTextHex" ] ]
        .forEach(([ colorId, textId ]) => {
            $(colorId).addEventListener("input", () => syncColorFromPicker(colorId, textId));
            $(textId).addEventListener("change", () => syncColorFromText(textId, colorId));
        });

    $("dcTheme").addEventListener("change", () => {
        syncBackgroundToTheme($("dcTheme").value);
        updatePreview();
    });

    [ "dcFontFamily", "dcFontSize", "dcRadius", "dcShadow", "dcWidth", "dcPosition", "dcButtonShape", "dcHeading", "dcDescription", "dcButtonText", "dcFooterText" ]
        .forEach(id => {
            $(id).addEventListener("input", updatePreview);
            $(id).addEventListener("change", updatePreview);
        });

    root.querySelectorAll(".dc-preview-mode").forEach(button => {
        button.addEventListener("click", () => setPreviewMode(button.dataset.dcMode));
    });

    $("dcSave").addEventListener("click", saveDraft);
    $("dcReset").addEventListener("click", () => {
        applySettings(savedSettings);
        showToast("Restored the settings last loaded or saved from the server.");
    });
    $("dcWidgetSelect").addEventListener("change", () => loadSettings(true));

    applySettings(defaults);
    savedSettings = currentSettings();
    loadSettings(false);
})();
