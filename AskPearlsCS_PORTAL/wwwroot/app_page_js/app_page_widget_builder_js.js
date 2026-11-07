
(function () {
    "use strict";

    const root = document.querySelector(
        ".page-widget-builder[data-ap-page='widget-builder']"
    );

    if (!root || root.dataset.wbInitialized === "true") return;

    root.dataset.wbInitialized = "true";

    const $ = id => root.querySelector("#" + id);

    const el = {
        message: $("wbApiMessage"),
        apiNote: $("wbApiNote"),
        indicator: $("wbDraftIndicator"),
        save: $("wbSaveButton"),
        saveBottom: $("wbSaveBottomButton"),
        previewButton: $("wbPreviewButton"),

        pageTitle: $("wbPageTitle"),
        pageDescription: $("wbPageDescription"),

        select: $("wbWidgetSelect"),
        createButton: $("wbCreateWidgetButton"),
        version: $("wbWidgetVersion"),

        name: $("wbWidgetName"),
        type: $("wbWidgetType"),
        website: $("wbWebsite"),
        title: $("wbWidgetTitle"),
        description: $("wbWidgetDescription"),

        primary: $("wbPrimaryColor"),
        colorValue: $("wbColorValue"),
        buttonText: $("wbButtonText"),
        corners: $("wbCornerStyle"),
        position: $("wbWidgetPosition"),

        showTitle: $("wbShowTitle"),
        showDescription: $("wbShowDescription"),
        showIntroduction: $("wbShowIntroduction"),

        previous: $("wbPreviousButton"),
        next: $("wbNextButton"),

        previewStage: $("wbPreviewStage"),
        previewWidget: $("wbPreviewWidget"),
        previewTitle: $("wbPreviewTitle"),
        previewDescription: $("wbPreviewDescription"),
        previewPrimary: $("wbPreviewPrimary"),
        previewAssistantIcon: $("wbPreviewAssistantIcon"),
        previewHeaderTitle: $("wbPreviewHeaderTitle"),
        previewCompanyName: $("wbPreviewCompanyName"),
        previewCompanyLogo: $("wbPreviewCompanyLogo"),
        previewFooter: $("wbPreviewFooter")
    };

    const API_ROOT = "/Widget/Api";

    const STORAGE_KEY = "askpearls.activeWidgetPublicKey";
    const STORAGE_MODE = "askpearls.widgetBuilderMode";
    // Explicit navigation intent. A normal sidebar visit must not reuse a previously edited widget.
    const route = new URLSearchParams(window.location.search);
    const routeMode = (route.get("mode") || route.get("action") || "").toLowerCase();
    const routeKey = route.get("publicKey") || route.get("widgetPublicKey") || route.get("widget") || "";

    const steps = [ "basics", "appearance", "behavior" ];

    const state = {
        widgets: [],
        key: "",
        creating: false,
        busy: false,
        dirty: false,
        step: 0,
        loadedDesign: null
    };

    const tabs = Array.from(
        root.querySelectorAll("[data-wb-tab]")
    );

    const panels = Array.from(
        root.querySelectorAll("[data-wb-panel]")
    );

    function value (element, fallback = "") {
        if (!element) return fallback;

        if (element.type === "checkbox") {
            return element.checked;
        }

        return typeof element.value === "string"
            ? element.value.trim()
            : fallback;
    }

    function setValue (element, newValue) {
        if (!element || newValue === undefined ||
            newValue === null) return;

        if (element.type === "checkbox") {
            element.checked = Boolean(newValue);
        } else {
            element.value = String(newValue);
        }
    }

    function field (object, camel, pascal) {
        if (!object) return undefined;

        return object[ camel ] !== undefined
            ? object[ camel ]
            : object[ pascal ];
    }

    function storageGet (key) {
        try {
            return localStorage.getItem(key) || "";
        } catch {
            return "";
        }
    }

    function storageSet (key, val) {
        try {
            if (val) localStorage.setItem(key, val);
            else localStorage.removeItem(key);
        } catch {
            // Storage is optional.
        }
    }

    function message (text, error = false) {
        if (!el.message) return;

        el.message.textContent = text || "";
        el.message.hidden = !text;

        el.message.classList.toggle("is-error", error);
        el.message.classList.toggle(
            "is-success",
            Boolean(text) && !error
        );
    }

    function setIndicator (text, saved = false) {
        if (!el.indicator) return;

        el.indicator.textContent = text;
        el.indicator.classList.toggle("is-saved", saved);
    }

    function updateSaveButton () {
        [ el.save, el.saveBottom ].filter(Boolean).forEach(button => {
            button.disabled = state.busy;
            button.setAttribute("aria-busy", String(state.busy));
            const icon = button.querySelector("i");
            button.replaceChildren();
            if (icon) button.appendChild(icon);
            button.append(document.createTextNode(state.busy ? " Saving..." : " Save Draft"));
        });
    }

    function setBusy (busy) {
        state.busy = Boolean(busy);

        updateSaveButton();

        if (el.select) {
            el.select.disabled =
                state.busy ||
                state.creating ||
                state.widgets.length === 0;
        }
    }

    function antiForgeryToken () {
        return root.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value || document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value || "";
    }

    function apiError (data, fallback) {
        if (!data) return fallback;

        if (typeof data === "string") {
            return data || fallback;
        }

        if (data.errors &&
            typeof data.errors === "object") {
            const errors = Object.values(data.errors)
                .flat()
                .filter(Boolean);

            if (errors.length) return errors.join(" ");
        }

        return data.message ||
            data.detail ||
            data.title ||
            data.error ||
            fallback;
    }

    async function portalRequest (path, options = {}) {
        const method = (
            options.method || "GET"
        ).toUpperCase();

        const headers = new Headers({
            "Accept": "application/json",
            "X-Requested-With": "XMLHttpRequest"
        });

        if (![ "GET", "HEAD" ].includes(method)) {
            const token = antiForgeryToken();

            if (!token) {
                throw new Error(
                    "Anti-forgery token is missing. " +
                    "Refresh the Portal page."
                );
            }

            headers.set(
                "RequestVerificationToken",
                token
            );
        }

        if (options.body !== undefined) {
            headers.set(
                "Content-Type",
                "application/json"
            );
        }

        const response = await fetch(
            API_ROOT + "/" + path.replace(/^\/+/, ""),
            {
                method,
                credentials: "same-origin",
                cache: "no-store",
                redirect: "manual",
                headers,
                body: options.body === undefined
                    ? undefined
                    : JSON.stringify(options.body)
            }
        );

        if (response.type === "opaqueredirect" ||
            (response.status >= 300 &&
                response.status < 400)) {
            throw new Error(
                "Your session may have expired. " +
                "Please sign in again."
            );
        }

        if (response.status === 401) {
            throw new Error(
                "Your login session has expired."
            );
        }

        if (response.status === 403) {
            throw new Error(
                "Access denied to this widget."
            );
        }

        const contentType = (
            response.headers.get("content-type") || ""
        ).toLowerCase();

        const raw = await response.text();

        let data = null;

        if (raw) {
            if (!contentType.includes("json")) {
                throw new Error(
                    "Expected JSON, received " +
                    (contentType || "unknown content type")
                );
            }

            try {
                data = JSON.parse(raw);
            } catch {
                throw new Error(
                    "The API returned invalid JSON."
                );
            }
        }

        if (!response.ok || data?.success === false) {
            throw new Error(
                apiError(
                    data,
                    "API request failed: HTTP " +
                    response.status
                )
            );
        }

        return data;
    }

    function normalizeWidgets (result) {
        if (Array.isArray(result)) return result;

        const list =
            result?.widgets ??
            result?.data?.widgets ??
            result?.data ??
            result?.items;

        if (Array.isArray(list)) return list;

        throw new Error(
            "The API returned an invalid widget list."
        );
    }

    function publicKey (widget) {
        return String(
            field(widget, "publicKey", "PublicKey") ||
            field(
                widget,
                "widgetPublicKey",
                "WidgetPublicKey"
            ) ||
            ""
        );
    }

    function widgetName (widget) {
        return String(
            field(widget, "name", "Name") ||
            field(widget, "widgetName", "WidgetName") ||
            "Untitled Widget"
        );
    }

    function renderWidgetOptions () {
        if (!el.select) return;

        el.select.replaceChildren();

        if (state.creating) {
            el.select.add(new Option("NEW Widget", ""));
            el.select.value = "";
            el.select.disabled = true;
            return;
        }

        el.select.add(new Option("-- Select a widget --", ""));

        state.widgets.forEach(widget => {
            const key = publicKey(widget);

            if (!key) return;

            el.select.add(
                new Option(widgetName(widget), key)
            );
        });

        const exists = state.widgets.some(
            widget => publicKey(widget) === state.key
        );

        el.select.value = exists ? state.key : "";
        el.select.disabled = state.busy;
    }

    function theme () {
        return root.querySelector(
            'input[name="wbTheme"]:checked'
        )?.value || "light";
    }

    function setTheme (newTheme) {
        const selected = newTheme === "dark"
            ? "dark"
            : "light";

        const radio = root.querySelector(
            'input[name="wbTheme"][value="' +
            selected + '"]'
        );

        if (radio) radio.checked = true;
    }

    function currentSettings () {
        const dark = theme() === "dark";

        return {
            ...(state.loadedDesign || {}),
            primary: value(el.primary, "#16a085"),
            background: state.loadedDesign?.background ?? (dark ? "#10242b" : "#ffffff"),
            text: state.loadedDesign?.text ?? (dark ? "#edf5f7" : "#16352b"),
            font: "Inter, Arial, sans-serif",
            fontSize: 16,
            radius: 16,
            shadow: true,
            width: 420,
            position: value(
                el.position,
                "bottom-right"
            ),
            buttonShape: value(el.corners, "rounded") === "soft"
                ? "rounded"
                : value(el.corners, "rounded"),
            heading: value(el.title),
            description: value(el.description),
            buttonText: value(
                el.buttonText,
                "Get Started"
            ),
            footer: state.loadedDesign?.footer || "Powered by AskPearls",
            theme: state.loadedDesign?.theme === "custom" ? "custom" : theme(),

            showTitle: value(el.showTitle, true),
            showDescription: value(
                el.showDescription,
                true
            ),
            showIntroduction: value(
                el.showIntroduction,
                true
            )
        };
    }

    function updatePreview () {
        const settings = currentSettings();

        if (el.previewTitle) {
            el.previewTitle.textContent =
                settings.heading || "Your widget title";

            el.previewTitle.hidden =
                !settings.showTitle;
        }

        if (el.previewDescription) {
            el.previewDescription.textContent =
                settings.description ||
                "Your description";

            el.previewDescription.hidden =
                !settings.showDescription;
        }

        if (el.previewPrimary) {
            el.previewPrimary.textContent =
                settings.buttonText || "Get Started";

            el.previewPrimary.style.backgroundColor =
                settings.primary;

            el.previewPrimary.style.borderColor =
                settings.primary;
        }

        if (el.colorValue) {
            el.colorValue.textContent =
                settings.primary;
        }

        if (el.previewHeaderTitle) el.previewHeaderTitle.textContent = settings.headerTitle || 'AskPearls Assistant';
        if (el.previewCompanyName) el.previewCompanyName.textContent =
            settings.brandName && settings.brandName !== settings.headerTitle ? settings.brandName : '';
        if (el.previewAssistantIcon) {
            const icon = document.createElement('i');
            icon.className = 'bi bi-' + ({ building: 'building', chat: 'chat-dots', robot: 'robot' }[ settings.brandIcon ] || 'stars');
            el.previewAssistantIcon.replaceChildren(icon);
        }
        if (el.previewCompanyLogo) {
            const holder = el.previewCompanyLogo;
            holder.replaceChildren();
            if (settings.logoUrl && /^https:\/\//i.test(settings.logoUrl)) {
                const img = document.createElement('img');
                img.src = settings.logoUrl; img.alt = (settings.brandName || 'Company') + ' logo';
                img.referrerPolicy = 'no-referrer';
                img.onerror = () => { holder.replaceChildren(); holder.hidden = true; };
                holder.append(img); holder.hidden = false;
            } else holder.hidden = true;
        }
        if (el.previewFooter) {
            const footer = settings.footer || 'Powered by AskPearls';
            el.previewFooter.textContent = settings.hideAskPearlsBranding ? footer :
                (footer.toLowerCase().includes('askpearls') ? footer : footer + ' · AskPearls');
        }

        if (el.previewWidget) {
            el.previewWidget.style.setProperty("--wb-primary", settings.primary);
            el.previewWidget.style.backgroundColor = settings.background || '#ffffff';
            el.previewWidget.style.color = settings.text || '#17212B';

            el.previewWidget.dataset.theme =
                settings.theme;

            el.previewWidget.dataset.corners =
                settings.buttonShape;

            el.previewWidget.dataset.position =
                settings.position;
        }
    }

    function markDirty () {
        state.dirty = true;
        setIndicator("Unsaved changes", false);
        updatePreview();
    }

    function markSaved () {
        state.dirty = false;
        setIndicator("Draft saved", true);
    }

    function setStep (index) {
        state.step = Math.max(
            0,
            Math.min(index, steps.length - 1)
        );

        const activeStep = steps[ state.step ];

        tabs.forEach(button => {
            const active =
                button.dataset.wbTab === activeStep;

            button.classList.toggle(
                "active",
                active
            );

            button.setAttribute(
                "aria-current",
                active ? "step" : "false"
            );
        });

        panels.forEach(panel => {
            const active =
                panel.dataset.wbPanel === activeStep;

            panel.classList.toggle(
                "active",
                active
            );

            panel.hidden = !active;
        });

        if (el.previous) {
            el.previous.disabled =
                state.step === 0;
        }

        if (el.next) {
            el.next.hidden =
                state.step === steps.length - 1;
        }
    }

    function resetFields () {
        setValue(el.name, "");
        setValue(el.type, "decision");
        setValue(el.website, "");
        setValue(el.title, "");
        setValue(el.description, "");

        setValue(
            el.buttonText,
            "Get Started"
        );

        setValue(el.primary, "#16a085");
        setValue(el.corners, "rounded");
        setValue(el.position, "bottom-right");

        setValue(el.showTitle, true);
        setValue(el.showDescription, true);
        setValue(el.showIntroduction, true);

        setTheme("light");
        updatePreview();
    }

    function selectionMode () {
        state.creating = false;
        state.key = "";
        state.loadedDesign = null;
        resetFields();
        renderWidgetOptions();
        if (el.pageTitle) el.pageTitle.textContent = "Widget Builder";
        if (el.pageDescription) el.pageDescription.textContent = "Create and customize your DecisionOS experience.";
        if (el.version) el.version.textContent = "—";
        if (el.apiNote) el.apiNote.textContent = "Choose an existing widget to edit, or click + to create a new widget.";
        setStep(0);
        state.dirty = false;
        setIndicator("Select a widget", false);
        message("");
        renderWidgetOptions();
    }

    function createMode () {
        storageSet(STORAGE_MODE, "");

        state.creating = true;
        state.key = "";

        storageSet(STORAGE_KEY, "");

        state.loadedDesign = null;
        resetFields();
        renderWidgetOptions();

        if (el.pageTitle) {
            el.pageTitle.textContent =
                "Create a new widget";
        }

        if (el.pageDescription) {
            el.pageDescription.textContent =
                "Set up your widget details, " +
                "appearance, and behavior.";
        }

        if (el.version) {
            el.version.textContent = "New";
        }

        if (el.apiNote) {
            el.apiNote.textContent =
                "Create a widget by entering its " +
                "details and clicking Save Draft.";
        }

        setStep(0);
        markDirty();

        message(
            "Enter your widget details to create " +
            "a new widget."
        );
    }

    function applySettings (result) {
        if (!result) return;

        const source =
            result.configuration ??
            result.design ??
            result.data ??
            result;

        if (!source) return;
        state.loadedDesign = { ...source };

        const mappings = [
            [ "primary", "primary", "Primary" ],
            [ "title", "heading", "Heading" ],
            [ "description", "description", "Description" ],
            [ "buttonText", "buttonText", "ButtonText" ],
            [ "position", "position", "Position" ],
            [ "corners", "buttonShape", "ButtonShape" ],
            [ "showTitle", "showTitle", "ShowTitle" ],
            [
                "showDescription",
                "showDescription",
                "ShowDescription"
            ],
            [
                "showIntroduction",
                "showIntroduction",
                "ShowIntroduction"
            ]
        ];

        mappings.forEach(([ elementName, camel, pascal ]) => {
            const setting = field(
                source,
                camel,
                pascal
            );

            if (setting !== undefined &&
                setting !== null) {
                setValue(el[ elementName ], setting);
            }
        });

        const savedTheme = field(
            source,
            "theme",
            "Theme"
        );

        if (savedTheme) setTheme(savedTheme);

        updatePreview();
    }

    function applyWidget (widget) {
        if (!widget) return;

        const key = publicKey(widget);

        if (!key) {
            throw new Error(
                "Widget public key is missing."
            );
        }

        state.key = key;
        state.creating = false;

        storageSet(STORAGE_KEY, key);

        setValue(el.name, widgetName(widget));

        const widgetType = field(
            widget,
            "type",
            "Type"
        );

        if (widgetType) {
            setValue(el.type, widgetType);
        }

        const website =
            field(
                widget,
                "websiteAddress",
                "WebsiteAddress"
            ) ||
            field(
                widget,
                "targetWebsite",
                "TargetWebsite"
            );

        if (website) {
            setValue(el.website, website);
        }

        const title = field(
            widget,
            "title",
            "Title"
        );

        if (title !== undefined) {
            setValue(el.title, title);
        }

        const description = field(
            widget,
            "description",
            "Description"
        );

        if (description !== undefined) {
            setValue(el.description, description);
        }

        if (el.pageTitle) {
            el.pageTitle.textContent =
                "Widget Builder";
        }

        if (el.version) {
            el.version.textContent = "Draft";
        }

        renderWidgetOptions();
        updatePreview();
    }

    async function loadConfiguration (key) {
        const result = await portalRequest(
            "widgets/" +
            encodeURIComponent(key) +
            "/draft/configuration"
        );

        applySettings(result);

        const version = field(
            result,
            "versionNumber",
            "VersionNumber"
        );

        if (version !== undefined &&
            version !== null &&
            el.version) {
            el.version.textContent =
                "Draft v" + version;
        }

        markSaved();
    }

    async function loadWidgets () {
        const result = await portalRequest("widgets");

        state.widgets = normalizeWidgets(result);

        renderWidgetOptions();
    }

    async function selectWidget (key) {
        if (state.busy) return;
        if (!key) { selectionMode(); return; }

        const widget = state.widgets.find(
            item => publicKey(item) === key
        );

        if (!widget) {
            message(
                "Selected widget was not found.",
                true
            );
            return;
        }

        setBusy(true);
        message("Loading widget...");

        // Every widget selection starts the wizard at Basics (Step 1).
        // Reset prior widget settings before applying the selected widget.
        state.loadedDesign = null;
        setStep(0);
        resetFields();

        try {
            applyWidget(widget);
            await loadConfiguration(key);

            message("");
        } catch (error) {
            message(
                error.message ||
                "Could not load widget configuration.",
                true
            );
        } finally {
            setBusy(false);
            renderWidgetOptions();
        }
    }

    function normalizedWebsite () {
        const input = value(el.website);

        if (!input) return "";

        return /^https?:\/\//i.test(input)
            ? input
            : "https://" + input;
    }

    function validate () {
        const name = value(el.name);
        const type = value(el.type, "decision");
        const title = value(el.title);
        const description = value(el.description);

        if (!name) {
            return "Enter an internal widget name.";
        }

        if (name.length > 100) {
            return "Widget name must be 100 characters or fewer.";
        }

        if (type !== "decision") {
            return "Unsupported widget type.";
        }

        const website = normalizedWebsite();

        if (!website) {
            return "Enter the target website.";
        }

        try {
            const url = new URL(website);

            if (![ "http:", "https:" ].includes(
                url.protocol
            )) {
                throw new Error();
            }

            if (!url.hostname.includes(".") &&
                url.hostname !== "localhost") {
                throw new Error();
            }

            if (website.length > 250) {
                return "Website URL must be 250 characters or fewer.";
            }
        } catch {
            return "Enter a valid website URL.";
        }

        if (!title) {
            return "Enter a visitor-facing widget title.";
        }

        if (title.length > 120) {
            return "Widget title must be 120 characters or fewer.";
        }

        if (description.length > 500) {
            return "Description must be 500 characters or fewer.";
        }

        return "";
    }

    async function resolveWebsite () {
        const address = normalizedWebsite();

        const result = await portalRequest(
            "widgets/websites"
        );

        const websites =
            result?.websites ??
            result?.data?.websites ??
            result?.data ??
            [];

        if (!Array.isArray(websites)) {
            throw new Error(
                "The websites API returned an invalid list."
            );
        }

        function normalizeAddress (input) {
            try {
                const url = new URL(input);

                return (
                    url.origin +
                    url.pathname.replace(/\/+$/, "")
                ).toLowerCase();
            } catch {
                return "";
            }
        }

        const target = normalizeAddress(address);

        const existing = websites.find(site => {
            const siteAddress =
                field(site, "address", "Address") ||
                field(
                    site,
                    "websiteAddress",
                    "WebsiteAddress"
                ) ||
                "";

            return normalizeAddress(siteAddress) === target;
        });

        const token = existing
            ? String(
                field(
                    existing,
                    "websiteTokenId",
                    "WebsiteTokenId"
                ) || ""
            )
            : "";

        return {
            websiteTokenId: token,
            websiteName: existing
                ? (
                    field(existing, "name", "Name") ||
                    new URL(address).hostname
                )
                : new URL(address).hostname,
            websiteAddress: address
        };
    }

    async function createPayload () {
        const website = await resolveWebsite();

        return {
            name: value(el.name),
            type: value(el.type, "decision"),
            websiteTokenId: website.websiteTokenId,
            websiteName: website.websiteName,
            websiteAddress: website.websiteAddress,
            title: value(el.title),
            description: value(el.description)
        };
    }

    async function saveWidgetBasics (key) {
        const website = await resolveWebsite();
        return portalRequest("widgets/" + encodeURIComponent(key) + "/basics", {
            method: "PUT",
            body: {
                name: value(el.name),
                title: value(el.title),
                description: value(el.description),
                websiteTokenId: website.websiteTokenId,
                websiteName: website.websiteName,
                websiteAddress: website.websiteAddress
            }
        });
    }

    async function saveDesign (key) {
        return portalRequest(
            "widgets/" +
            encodeURIComponent(key) +
            "/draft/configuration",
            {
                method: "PUT",
                body: currentSettings()
            }
        );
    }

    async function saveDraft () {
        if (state.busy) return;

        const validationError = validate();

        if (validationError) {
            message(validationError, true);
            return;
        }

        setBusy(true);
        message("Saving widget draft...");

        let createdKey = "";

        try {
            if (state.creating) {
                const payload = await createPayload();

                const result = await portalRequest(
                    "widgets",
                    {
                        method: "POST",
                        body: payload
                    }
                );

                const created =
                    result?.widget ??
                    result?.data?.widget ??
                    result?.data ??
                    result;

                createdKey = publicKey(created);

                if (!createdKey) {
                    throw new Error(
                        "Widget creation returned no public key. " +
                        "Check the API response before retrying."
                    );
                }

                state.key = createdKey;
                state.creating = false;

                storageSet(STORAGE_KEY, createdKey);
                storageSet(STORAGE_MODE, "");
            }

            if (!state.key) {
                throw new Error(
                    "No widget is selected for saving."
                );
            }

            // Save Basics first, including new or existing Target Website.
            const basicsResult = await saveWidgetBasics(state.key);
            const savedWebsiteTokenId = field(basicsResult, "websiteTokenId", "WebsiteTokenId");
            console.debug("Saved Target Website token:", savedWebsiteTokenId);
            // Save design and receive the authoritative new draft version.
            const saveResult = await saveDesign(state.key);

            // The draft has been saved at this point.
            markSaved();

            const savedVersion = field(saveResult, "versionNumber", "VersionNumber") ??
                field(saveResult?.data, "versionNumber", "VersionNumber");
            if (el.version && savedVersion !== undefined && savedVersion !== null) {
                el.version.textContent = "Draft v" + savedVersion;
            }

            // A list refresh failure must not be treated
            // as a failure to save the draft.
            try {
                await loadWidgets();
            } catch (refreshError) {
                console.warn(
                    "Widget saved, but list refresh failed:",
                    refreshError
                );
            }

            renderWidgetOptions();

            message(
                createdKey
                    ? "Widget created and draft saved successfully."
                    : "Widget draft saved successfully."
            );

        } catch (error) {
            console.error(
                "AskPearls Widget Builder save error:",
                error
            );

            state.dirty = true;

            if (createdKey) {
                setIndicator(
                    "Widget created; settings not saved",
                    false
                );

                message(
                    "Widget was created, but saving its " +
                    "configuration failed. Click Save Draft " +
                    "again to retry without creating a duplicate. " +
                    (error.message || ""),
                    true
                );
            } else {
                setIndicator(
                    "Unsaved changes",
                    false
                );

                message(
                    error.message ||
                    "Could not save the widget.",
                    true
                );
            }
        } finally {
            setBusy(false);
            renderWidgetOptions();
        }
    }

    function bindEvents () {
        tabs.forEach(button => {
            button.addEventListener("click", () => {
                const index = steps.indexOf(
                    button.dataset.wbTab
                );

                if (index >= 0) setStep(index);
            });
        });

        el.previous?.addEventListener(
            "click",
            () => setStep(state.step - 1)
        );

        el.next?.addEventListener(
            "click",
            () => setStep(state.step + 1)
        );

        el.save?.addEventListener(
            "click",
            saveDraft
        );

        el.saveBottom?.addEventListener(
            "click",
            saveDraft
        );

        el.select?.addEventListener(
            "change",
            () => selectWidget(el.select.value)
        );

        el.createButton?.addEventListener("click", () => {
            if (state.busy) return;
            createMode();
        });

        const inputs = [
            el.name,
            el.type,
            el.website,
            el.title,
            el.description,
            el.primary,
            el.buttonText,
            el.corners,
            el.position,
            el.showTitle,
            el.showDescription,
            el.showIntroduction
        ];

        inputs.filter(Boolean).forEach(input => {
            input.addEventListener(
                "input",
                markDirty
            );

            input.addEventListener(
                "change",
                markDirty
            );
        });

        root.querySelectorAll(
            'input[name="wbTheme"]'
        ).forEach(radio => {
            radio.addEventListener(
                "change",
                markDirty
            );
        });

        el.previewButton?.addEventListener(
            "click",
            () => {
                el.previewStage?.scrollIntoView({
                    behavior: "smooth",
                    block: "nearest"
                });
            }
        );
    }

    async function initialize () {
        bindEvents();
        setStep(0);
        updatePreview();
        updateSaveButton();

        try {
            // Query-string navigation is preferred. Legacy Dashboard sessionStorage
            // intent is consumed once; sidebar navigation cannot auto-select.
            const legacyMode = storageGet(STORAGE_MODE);
            const legacyKey = storageGet(STORAGE_KEY);
            storageSet(STORAGE_MODE, "");
            const explicitCreate = routeMode === "create" || route.get("new") === "1" ||
                (!routeMode && !routeKey && legacyMode === "create");
            const explicitEdit = routeMode === "edit" || Boolean(routeKey) ||
                (!routeMode && legacyMode === "edit");

            if (explicitCreate) {
                createMode();
                return;
            }

            await loadWidgets();
            if (explicitEdit) {
                const requestedKey = routeKey || legacyKey;
                if (!requestedKey) {
                    selectionMode();
                    message("No widget was specified for editing.", true);
                    return;
                }
                const match = state.widgets.find(w => publicKey(w) === requestedKey);
                if (!match) {
                    selectionMode();
                    message("The requested widget was not found or is unavailable.", true);
                    return;
                }
                await selectWidget(requestedKey);
                return;
            }

            // Direct/sidebar navigation: ALWAYS require manual selection.
            selectionMode();
        } catch (error) {
            message(error.message || "Could not initialize Widget Builder.", true);
            console.error("AskPearls Widget Builder initialization error:", error);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            initialize,
            { once: true }
        );
    } else {
        initialize();
    }

})();
