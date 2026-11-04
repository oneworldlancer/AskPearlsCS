(function () {
    "use strict";

    const root = document.querySelector(
        ".page-widget-builder[data-ap-page='widget-builder']"
    );

    if (!root || root.dataset.wbInitialized === "true") return;
    root.dataset.wbInitialized = "true";

    const $ = (id) => root.querySelector("#" + id);

    const elements = {
        message: $("wbApiMessage"),
        draftIndicator: $("wbDraftIndicator"),
        saveButton: $("wbSaveButton"),
        widgetName: $("wbWidgetName"),
        widgetType: $("wbWidgetType"),
        website: $("wbWebsite"),
        title: $("wbWidgetTitle"),
        description: $("wbWidgetDescription"),
        buttonText: $("wbButtonText"),
        position: $("wbWidgetPosition"),
        cornerStyle: $("wbCornerStyle"),
        primaryColor: $("wbPrimaryColor"),
        colorValue: $("wbColorValue"),
        previewTitle: $("wbPreviewTitle"),
        previewDescription: $("wbPreviewDescription"),
        previewPrimary: $("wbPreviewPrimary"),
        previewWidget: $("wbPreviewWidget"),
        widgetSelect: $("wbWidgetSelect"),
        widgetVersion: $("wbWidgetVersion"),
        apiNote: $("wbApiNote"),
        pageTitle: $("wbPageTitle"),
        pageDescription: $("wbPageDescription"),
        previousButton: $("wbPreviousButton"),
        nextButton: $("wbNextButton"),
        previewButton: $("wbPreviewButton"),
        previewStage: $("wbPreviewStage")
    };

    const storage = {
        mode: "askpearls.widgetBuilderMode",
        activeWidget: "askpearls.activeWidgetPublicKey"
    };

    const steps = [ "basics", "appearance", "behavior" ];
    let currentStep = 0;
    let widgets = [];
    let activeWidgetPublicKey = "";
    let creating = false;
    let toastTimer = null;

    function getAntiForgeryToken () {
        const tokenInput = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        );
        return tokenInput ? tokenInput.value : "";
    }

    function showMessage (message, isError) {
        if (!elements.message) return;

        elements.message.textContent = message || "";
        elements.message.hidden = !message;
        elements.message.classList.toggle("is-error", Boolean(isError));
        elements.message.classList.toggle("is-success", Boolean(message) && !isError);
    }

    function setDraftIndicator (message, isSaved) {
        if (elements.draftIndicator) {
            elements.draftIndicator.textContent = message;
            elements.draftIndicator.classList.toggle("is-saved", Boolean(isSaved));
        }

        if (elements.saveButton) {
            elements.saveButton.disabled = false;
        }
    }

    function setSaving (isSaving) {
        if (!elements.saveButton) return;

        elements.saveButton.disabled = isSaving;
        elements.saveButton.setAttribute("aria-busy", String(isSaving));

        const label = elements.saveButton.querySelector("[data-save-label]");
        if (label) {
            label.textContent = isSaving ? "Saving…" : "Save Draft";
        }
    }

    function getValue (element, fallback) {
        return element && typeof element.value === "string"
            ? element.value.trim()
            : (fallback || "");
    }

    function getApiError (data, fallback) {
        if (!data) return fallback;

        if (typeof data === "string" && data.trim()) {
            return data;
        }

        return data.detail ||
            data.message ||
            data.title ||
            data.error ||
            fallback;
    }

    async function portalRequest (path, options) {
        const requestOptions = options || {};
        const headers = new Headers(requestOptions.headers || {});

        headers.set("Accept", "application/json");
        headers.set("X-Requested-With", "XMLHttpRequest");

        const csrfToken = getAntiForgeryToken();
        if (csrfToken) {
            headers.set("RequestVerificationToken", csrfToken);
        }

        if (requestOptions.body !== undefined) {
            headers.set("Content-Type", "application/json");
        }

        const response = await fetch("/Widget/Api/" + path.replace(/^\/+/, ""), {
            method: requestOptions.method || "GET",
            credentials: "same-origin",
            headers: headers,
            body: requestOptions.body === undefined
                ? undefined
                : JSON.stringify(requestOptions.body)
        });

        const text = await response.text();
        let data = null;

        if (text) {
            try {
                data = JSON.parse(text);
            } catch (_) {
                data = text;
            }
        }

        if (!response.ok) {
            throw new Error(
                getApiError(data, "Request failed (" + response.status + ").")
            );
        }

        return data;
    }

    function setCreateMode () {
        creating = true;
        activeWidgetPublicKey = "";

        try {
            localStorage.setItem(storage.mode, "create");
            localStorage.removeItem(storage.activeWidget);
        } catch (_) {
            // Browser storage can be unavailable; this page still works.
        }

        if (elements.pageTitle) {
            elements.pageTitle.textContent = "Create a new widget";
        }

        if (elements.pageDescription) {
            elements.pageDescription.textContent =
                "Set up your widget details, appearance, and behavior.";
        }

        if (elements.widgetSelect) {
            elements.widgetSelect.innerHTML =
                '<option value="">New widget</option>';
            elements.widgetSelect.value = "";
        }

        if (elements.widgetName) elements.widgetName.value = "";
        if (elements.widgetType) elements.widgetType.value = "decision";
        if (elements.website) elements.website.value = "";
        if (elements.title) elements.title.value = "";
        if (elements.description) elements.description.value = "";
        if (elements.buttonText) elements.buttonText.value = "";

        setDraftIndicator("Unsaved draft", false);
        updatePreview();
    }

    function isCreateModeRequested () {
        try {
            return localStorage.getItem(storage.mode) === "create";
        } catch (_) {
            return false;
        }
    }

    function getStoredActiveWidgetKey () {
        try {
            return localStorage.getItem(storage.activeWidget) || "";
        } catch (_) {
            return "";
        }
    }

    function currentSettings () {
        const color = getValue(elements.primaryColor, "#16a96b");

        return {
            primary: color,
            background: "#ffffff",
            text: "#16352b",
            font: "system",
            fontSize: 16,
            radius: 16,
            shadow: true,
            width: 420,
            position: getValue(elements.position, "right"),
            buttonShape: getValue(elements.cornerStyle, "rounded"),
            heading: getValue(elements.title, ""),
            description: getValue(elements.description, ""),
            buttonText: getValue(elements.buttonText, ""),
            footer: "",
            theme: "light"
        };
    }

    function updatePreview () {
        const title = getValue(elements.title, "Your widget title");
        const description = getValue(elements.description, "Your description");
        const buttonText = getValue(elements.buttonText, "Get started");
        const color = getValue(elements.primaryColor, "#16a96b");

        if (elements.previewTitle) {
            elements.previewTitle.textContent = title || "Your widget title";
        }

        if (elements.previewDescription) {
            elements.previewDescription.textContent =
                description || "Your description";
        }

        if (elements.previewPrimary) {
            elements.previewPrimary.textContent = buttonText || "Get started";
            elements.previewPrimary.style.backgroundColor = color;
        }

        if (elements.colorValue) {
            elements.colorValue.textContent = color;
        }

        if (elements.previewWidget) {
            elements.previewWidget.style.setProperty("--wb-primary", color);
        }
    }

    function markUnsaved () {
        setDraftIndicator("Unsaved draft", false);
        updatePreview();
    }

    function setStep (index) {
        currentStep = Math.max(0, Math.min(index, steps.length - 1));

        root.querySelectorAll("[data-wb-step]").forEach(function (button, i) {
            const selected = i === currentStep;
            button.classList.toggle("is-active", selected);
            button.setAttribute("aria-current", selected ? "step" : "false");
        });

        root.querySelectorAll("[data-wb-panel]").forEach(function (panel) {
            panel.hidden = panel.dataset.wbPanel !== steps[ currentStep ];
        });

        if (elements.previousButton) {
            elements.previousButton.disabled = currentStep === 0;
        }

        if (elements.nextButton) {
            elements.nextButton.hidden = currentStep === steps.length - 1;
        }
    }

    function readField (object, lowerName, upperName) {
        if (!object) return undefined;
        return object[ lowerName ] !== undefined
            ? object[ lowerName ]
            : object[ upperName ];
    }

    function applySettings (configuration) {
        if (!configuration) return;

        const fields = [
            [ "primaryColor", "primary", "Primary" ],
            [ "title", "heading", "Heading" ],
            [ "description", "description", "Description" ],
            [ "buttonText", "buttonText", "ButtonText" ],
            [ "position", "position", "Position" ],
            [ "cornerStyle", "buttonShape", "ButtonShape" ]
        ];

        fields.forEach(function (field) {
            const element = elements[ field[ 0 ] ];
            const value = readField(configuration, field[ 1 ], field[ 2 ]);
            if (element && value !== undefined && value !== null) {
                element.value = value;
            }
        });

        updatePreview();
    }

    function widgetPublicKey (widget) {
        return readField(widget, "publicKey", "PublicKey") ||
            readField(widget, "widgetPublicKey", "WidgetPublicKey") ||
            "";
    }

    function widgetName (widget) {
        return readField(widget, "name", "Name") ||
            readField(widget, "widgetName", "WidgetName") ||
            "Widget";
    }

    async function loadWidgets () {
        if (creating) return;

        try {
            const result = await portalRequest("widgets");
            const list = Array.isArray(result)
                ? result
                : (result && (result.widgets || result.data)) || [];

            widgets = Array.isArray(list) ? list : [];

            if (elements.widgetSelect) {
                elements.widgetSelect.innerHTML = "";

                if (widgets.length === 0) {
                    elements.widgetSelect.add(
                        new Option("No widgets yet — create a new widget", "")
                    );
                    activeWidgetPublicKey = "";
                    setDraftIndicator("No saved widgets", false);
                    return;
                }

                widgets.forEach(function (widget) {
                    const key = widgetPublicKey(widget);
                    if (!key) return;

                    elements.widgetSelect.add(
                        new Option(widgetName(widget), key)
                    );
                });
            }

            const storedKey = getStoredActiveWidgetKey();
            const selectedWidget = widgets.find(function (widget) {
                return widgetPublicKey(widget) === storedKey;
            }) || widgets[ 0 ];

            if (selectedWidget) {
                activeWidgetPublicKey = widgetPublicKey(selectedWidget);

                if (elements.widgetSelect) {
                    elements.widgetSelect.value = activeWidgetPublicKey;
                }

                await loadConfiguration(activeWidgetPublicKey);
            }
        } catch (error) {
            widgets = [];
            if (elements.widgetSelect) {
                elements.widgetSelect.innerHTML =
                    '<option value="">Could not load widgets</option>';
            }
            showMessage(error.message || "Could not load your widgets.", true);
        }
    }

    async function loadConfiguration (publicKey) {
        if (!publicKey) return;

        try {
            const result = await portalRequest(
                "widgets/" + encodeURIComponent(publicKey) +
                "/draft/configuration"
            );

            const configuration =
                result.configuration ||
                result.design ||
                result.data ||
                result;

            applySettings(configuration);

            const version = readField(result, "versionNumber", "VersionNumber");
            if (elements.widgetVersion && version !== undefined) {
                elements.widgetVersion.textContent = "Draft v" + version;
            }

            setDraftIndicator("Saved settings loaded", true);
            showMessage("", false);
        } catch (error) {
            showMessage(
                error.message || "Could not load this widget's saved settings.",
                true
            );
        }
    }

    function validateForm () {
        const name = getValue(elements.widgetName);
        const type = getValue(elements.widgetType, "decision");
        const website = getValue(elements.website);
        const title = getValue(elements.title);
        const description = getValue(elements.description);

        if (!name) return "Enter an internal widget name.";
        if (name.length > 100) return "Widget name must be 100 characters or fewer.";
        if (type.toLowerCase() !== "decision") {
            return "Widget type must be Decision.";
        }
        if (!website) return "Enter the website address for this widget.";

        let websiteUrl;
        try {
            websiteUrl = new URL(
                /^https?:\/\//i.test(website) ? website : "https://" + website
            );
        } catch (_) {
            return "Enter a valid website address.";
        }

        if (![ "http:", "https:" ].includes(websiteUrl.protocol)) {
            return "Website address must use HTTP or HTTPS.";
        }

        if (!title) return "Enter the title visitors will see.";
        if (title.length > 120) return "Widget title must be 120 characters or fewer.";
        if (description.length > 500) {
            return "Description must be 500 characters or fewer.";
        }

        return "";
    }

    function buildCreateRequest () {
        const websiteInput = getValue(elements.website);
        const websiteAddress = /^https?:\/\//i.test(websiteInput)
            ? websiteInput
            : "https://" + websiteInput;

        let hostname = websiteAddress;
        try {
            hostname = new URL(websiteAddress).hostname;
        } catch (_) {
            // Validation reports an invalid address before this is called.
        }

        return {
            name: getValue(elements.widgetName),
            type: getValue(elements.widgetType, "decision"),
            websiteTokenId: "",
            websiteName: hostname,
            websiteAddress: websiteAddress,
            title: getValue(elements.title),
            description: getValue(elements.description)
        };
    }

    async function saveDesign (publicKey) {
        return portalRequest(
            "widgets/" + encodeURIComponent(publicKey) +
            "/draft/configuration",
            {
                method: "PUT",
                body: currentSettings()
            }
        );
    }

    async function saveDraft () {
        const validationMessage = validateForm();

        if (validationMessage) {
            showMessage(validationMessage, true);
            return;
        }

        setSaving(true);
        showMessage("", false);

        try {
            if (creating) {
                const created = await portalRequest("widgets", {
                    method: "POST",
                    body: buildCreateRequest()
                });

                const createdKey = widgetPublicKey(created);
                if (!createdKey) {
                    throw new Error(
                        "The API created no widget key in its response. Check the Portal proxy response."
                    );
                }

                activeWidgetPublicKey = createdKey;
                creating = false;

                try {
                    localStorage.removeItem(storage.mode);
                    localStorage.setItem(storage.activeWidget, createdKey);
                } catch (_) {
                    // The server has created the widget; storage is optional.
                }

                await saveDesign(createdKey);
                await loadWidgets();

                if (elements.widgetSelect) {
                    elements.widgetSelect.value = createdKey;
                }

                setDraftIndicator("Draft saved", true);
                showMessage("Widget created and draft saved.", false);
                return;
            }

            if (!activeWidgetPublicKey) {
                throw new Error(
                    "Select a widget to save, or open Widget Builder using Create New Widget."
                );
            }

            await saveDesign(activeWidgetPublicKey);
            setDraftIndicator("Draft saved", true);
            showMessage("Draft saved.", false);
        } catch (error) {
            showMessage(error.message || "Could not save the widget draft.", true);
            setDraftIndicator("Unsaved draft", false);
        } finally {
            setSaving(false);
        }
    }

    function bindEvents () {
        root.querySelectorAll("[data-wb-step]").forEach(function (button, index) {
            button.addEventListener("click", function () {
                setStep(index);
            });
        });

        if (elements.previousButton) {
            elements.previousButton.addEventListener("click", function () {
                setStep(currentStep - 1);
            });
        }

        if (elements.nextButton) {
            elements.nextButton.addEventListener("click", function () {
                setStep(currentStep + 1);
            });
        }

        if (elements.saveButton) {
            elements.saveButton.addEventListener("click", saveDraft);
        }

        if (elements.widgetSelect) {
            elements.widgetSelect.addEventListener("change", async function () {
                const selectedKey = elements.widgetSelect.value;
                if (!selectedKey) return;

                creating = false;
                activeWidgetPublicKey = selectedKey;

                try {
                    localStorage.removeItem(storage.mode);
                    localStorage.setItem(storage.activeWidget, selectedKey);
                } catch (_) {
                    // Storage is optional.
                }

                await loadConfiguration(selectedKey);
            });
        }

        [
            elements.widgetName,
            elements.widgetType,
            elements.website,
            elements.title,
            elements.description,
            elements.buttonText,
            elements.position,
            elements.cornerStyle,
            elements.primaryColor
        ].forEach(function (element) {
            if (element) {
                element.addEventListener("input", markUnsaved);
                element.addEventListener("change", markUnsaved);
            }
        });

        if (elements.previewButton) {
            elements.previewButton.addEventListener("click", function () {
                if (elements.previewStage) {
                    elements.previewStage.scrollIntoView({
                        behavior: "smooth",
                        block: "nearest"
                    });
                }
            });
        }

        root.querySelectorAll("[data-wb-preview-close]").forEach(function (button) {
            button.addEventListener("click", function () {
                if (elements.previewStage) {
                    elements.previewStage.hidden = true;
                }
            });
        });
    }

    async function initialize () {
        bindEvents();
        setStep(0);
        updatePreview();

        if (isCreateModeRequested()) {
            setCreateMode();
            return;
        }

        await loadWidgets();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize, { once: true });
    } else {
        initialize();
    }
})();