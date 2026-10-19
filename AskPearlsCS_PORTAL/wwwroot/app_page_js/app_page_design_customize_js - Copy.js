
    (function() {
        "use strict";

    const root = document.querySelector(".design-customize-page");
    if (!root || root.dataset.dcInitialized === "true") return;
    root.dataset.dcInitialized = "true";

        const $ = id => root.querySelector("#" + id);

    const defaults = {
        primary: "#0da88b",
    background: "#ffffff",
    text: "#17212b",
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

    let savedSettings = { ...defaults };
    let toastTimer = null;

    function currentSettings() {
            return {
        primary: $("dcPrimaryColor").value,
    background: $("dcBackgroundColor").value,
    text: $("dcTextColor").value,
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

    function validHex(value) {
            return /^#[0-9a-fA-F]{ 6 }$/.test(value);
        }

    function syncColorFromText(textId, colorId) {
            const text = $(textId).value.trim();
    if (validHex(text)) {
        $(colorId).value = text;
    updatePreview();
            }
        }

    function syncColorFromPicker(colorId, textId) {
        $(textId).value = $(colorId).value.toUpperCase();
    updatePreview();
        }

    function setSettings(settings) {
        $("dcPrimaryColor").value = settings.primary;
    $("dcPrimaryHex").value = settings.primary.toUpperCase();
    $("dcBackgroundColor").value = settings.background;
    $("dcBackgroundHex").value = settings.background.toUpperCase();
    $("dcTextColor").value = settings.text;
    $("dcTextHex").value = settings.text.toUpperCase();
    $("dcFontFamily").value = settings.font;
    $("dcFontSize").value = settings.fontSize;
    $("dcRadius").value = settings.radius;
    $("dcShadow").checked = settings.shadow;
    $("dcWidth").value = settings.width;
    $("dcPosition").value = settings.position;
    $("dcButtonShape").value = settings.buttonShape;
    $("dcHeading").value = settings.heading;
    $("dcDescription").value = settings.description;
    $("dcButtonText").value = settings.buttonText;
    $("dcFooterText").value = settings.footer;
    updatePreview();
        }

    function showToast(message) {
            const toast = $("dcToast");
    toast.textContent = message;
    toast.classList.add("is-visible");
    clearTimeout(toastTimer);
            toastTimer = setTimeout(() => toast.classList.remove("is-visible"), 2600);
        }

    function updatePreview() {
            const s = currentSettings();
    const widget = $("dcWidgetPreview");
    const canvas = $("dcPreviewCanvas");
    const position = $("dcPreviewPosition");

    root.style.setProperty("--dc-primary", s.primary);
    root.style.setProperty("--dc-widget-bg", s.background);
    root.style.setProperty("--dc-widget-text", s.text);

    widget.style.backgroundColor = s.background;
    widget.style.color = s.text;
    widget.style.fontFamily = s.font;
    widget.style.fontSize = s.fontSize + "px";
    widget.style.borderRadius = s.radius + "px";
    widget.style.boxShadow = s.shadow
    ? "0 18px 55px rgba(20,35,30,.18)"
    : "none";
    widget.style.width = "min(100%, " + s.width + "px)";

    $("dcPreviewHeading").textContent = s.heading || "Your welcome heading";
    $("dcPreviewHeading").style.color = s.text;
    $("dcPreviewHeading").style.fontSize = Math.max(16, s.fontSize + 6) + "px";

    $("dcPreviewDescription").textContent =
    s.description || "Your welcome message will appear here.";
    $("dcPreviewDescription").style.color = s.text;

    $("dcPreviewButton").innerHTML =
    escapeHtml(s.buttonText || "Get started") +
    ' <i class="bi bi-arrow-right"></i>';

    $("dcPreviewFooter").textContent = s.footer;
    $("dcPreviewFooter").style.color = s.text;

            root.querySelectorAll(".dc-preview-answer").forEach(answer => {
        answer.style.borderRadius = shapeRadius(s.buttonShape);
    answer.style.color = s.text;
            });

    $("dcPreviewButton").style.borderRadius = shapeRadius(s.buttonShape);

    position.dataset.position = s.position;
    $("dcFontSizeValue").textContent = s.fontSize + " px";
    $("dcRadiusValue").textContent = s.radius + " px";
    $("dcWidthValue").textContent = s.width + " px";

    // The preview-mode background is separate from the actual widget colors.
    // Preserve the currently selected preview mode while settings change.
    const activeMode = root.querySelector(".dc-preview-mode.active");
    setPreviewMode(activeMode ? activeMode.dataset.dcMode : "light");
    updateDirtyState();
        }

    function shapeRadius(shape) {
            if (shape === "pill") return "999px";
    if (shape === "square") return "2px";
    return $("dcRadius").value + "px";
        }

    function escapeHtml(value) {
            return String(value ?? "").replace(/[&<>"']/g, character => ({
        "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
            })[character]);
        }

        function setPreviewMode(mode) {
            const canvas = $("dcPreviewCanvas");
        canvas.dataset.mode = mode;
            root.querySelectorAll(".dc-preview-mode").forEach(button => {
            button.classList.toggle("active", button.dataset.dcMode === mode);
            });

        const widget = $("dcWidgetPreview");

        if (mode === "dark") {
            widget.style.backgroundColor = "#15231e";
        widget.style.color = "#e7f0ec";
        $("dcPreviewHeading").style.color = "#e7f0ec";
        $("dcPreviewDescription").style.color = "#e7f0ec";
        $("dcPreviewFooter").style.color = "#e7f0ec";
                root.querySelectorAll(".dc-preview-answer").forEach(answer => {
            answer.style.color = "#e7f0ec";
        answer.style.borderColor = "rgba(231,240,236,.2)";
                });
            } else {
                const s = currentSettings();
        widget.style.backgroundColor = s.background;
        widget.style.color = s.text;
        $("dcPreviewHeading").style.color = s.text;
        $("dcPreviewDescription").style.color = s.text;
        $("dcPreviewFooter").style.color = s.text;
                root.querySelectorAll(".dc-preview-answer").forEach(answer => {
            answer.style.color = s.text;
        answer.style.borderColor = "rgba(100,120,110,.2)";
                });
            }

        if (mode === "mobile") {
            widget.style.width = "min(100%, 320px)";
            } else {
            widget.style.width = "min(100%, " + currentSettings().width + "px)";
            }
        }

        function updateDirtyState() {
            const changed = JSON.stringify(currentSettings()) !== JSON.stringify(savedSettings);
        const indicator = $("dcDraftIndicator");
        indicator.innerHTML = changed
        ? '<i class="bi bi-circle-fill"></i> Unsaved changes'
        : '<i class="bi bi-check-circle-fill"></i> All changes saved';
        indicator.style.color = changed ? "" : "#0d9278";
        }

        [
        ["dcPrimaryColor", "dcPrimaryHex"],
        ["dcBackgroundColor", "dcBackgroundHex"],
        ["dcTextColor", "dcTextHex"]
        ].forEach(([colorId, textId]) => {
            $(colorId).addEventListener("input", () => syncColorFromPicker(colorId, textId));
            $(textId).addEventListener("change", () => syncColorFromText(textId, colorId));
        });

        [
        "dcFontFamily", "dcFontSize", "dcRadius", "dcShadow",
        "dcWidth", "dcPosition", "dcButtonShape", "dcHeading",
        "dcDescription", "dcButtonText", "dcFooterText"
        ].forEach(id => {
            $(id).addEventListener("input", updatePreview);
        $(id).addEventListener("change", updatePreview);
        });

        root.querySelectorAll(".dc-preview-mode").forEach(button => {
            button.addEventListener("click", () => setPreviewMode(button.dataset.dcMode));
        });

        $("dcSave").addEventListener("click", () => {
            savedSettings = currentSettings();
        updateDirtyState();
        showToast("Draft saved in this browser session. Backend saving is not connected yet.");
        });

        $("dcReset").addEventListener("click", () => {
            setSettings(savedSettings);
        showToast("Settings reset to the last saved draft.");
        });

        $("dcWidgetSelect").addEventListener("change", event => {
            const presets = {
            sofa: { heading: "Find your perfect sofa", buttonText: "Get started" },
        furniture: { heading: "Discover your ideal furniture", buttonText: "Explore options" },
        demo: { heading: "Find the right solution for you", buttonText: "Start now" }
            };

        const preset = presets[event.target.value] || presets.sofa;
        $("dcHeading").value = preset.heading;
        $("dcButtonText").value = preset.buttonText;
        updatePreview();
        showToast("Preview preset changed. Save Draft to keep these settings for this session.");
        });

        // Initialize once per AJAX-loaded partial.
        setSettings(defaults);
        savedSettings = currentSettings();
        updatePreview();
    })();
 