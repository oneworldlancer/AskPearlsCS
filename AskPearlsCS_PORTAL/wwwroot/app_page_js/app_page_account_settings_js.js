
(function () {
    "use strict";

    const root = document.querySelector(".account-settings-page");
    if (!root || root.dataset.asInitialized === "true") return;
    root.dataset.asInitialized = "true";

    const $ = id => root.querySelector("#" + id);
    const settingsUrl = "/Account/Api/settings";
    const notificationPreferencesUrl = "/Account/Api/notifications/preferences";
    const subscriptionEnsureUrl = "/Account/Api/subscription/ensure-free";
    const notificationKeys = [ "nonEssentialNotifications", "emailNotifications", "activityReports", "productUpdates", "securityAlerts" ];

    let preferencesLoaded = false;
    let activeSection = "profile";
    const sectionKeys = {
        profile: [ "firstName", "lastName", "jobTitle", "bio" ],
        organization: [ "companyName", "organizationLogoUrl" ],
        notifications: notificationKeys
    };
    const pickKeys = (obj, keys) => Object.fromEntries(keys.map(k => [ k, obj[ k ] ]));

    const defaults = {
        nonEssentialNotifications: true,
        firstName: "",
        lastName: "",
        email: "",
        jobTitle: "",
        bio: "",
        companyName: "",
        organizationLogoUrl: "",
        emailNotifications: true,
        activityReports: true,
        productUpdates: false,
        securityAlerts: true
    };

    const fieldIds = {
        nonEssentialNotifications: "asNonEssentialNotifications",
        firstName: "asFirstName",
        lastName: "asLastName",
        email: "asEmail",
        jobTitle: "asJobTitle",
        bio: "asBio",
        companyName: "asCompanyName",
        organizationLogoUrl: "asOrgLogoInput",
        emailNotifications: "asEmailNotifications",
        activityReports: "asActivityReports",
        productUpdates: "asProductUpdates",
        securityAlerts: "asSecurityAlerts"
    };

    let savedSettings = { ...defaults };
    let toastTimer = null;
    let settingsLoaded = false;

    // Shared global input-value helper.
    // Uses AppInputValue.clean() when available.
    function cleanValue (value) {
        if (window.AppInputValue &&
            typeof window.AppInputValue.clean === "function") {
            return window.AppInputValue.clean(value);
        }

        if (value === null || value === undefined) return "";

        const text = String(value).trim();
        return text === "0" ? "" : text;
    }

    function currentSettings () {
        const result = {};

        Object.entries(fieldIds).forEach(([ key, id ]) => {
            const element = $(id);
            if (!element) return;

            result[ key ] = element.type === "checkbox"
                ? element.checked
                : element.tagName === "SELECT"
                    ? element.value
                    : cleanValue(element.value);
        });

        return result;
    }

    function setSettings (settings) {
        Object.entries(fieldIds).forEach(([ key, id ]) => {
            const element = $(id);
            if (!element) return;

            if (element.type === "checkbox") {
                element.checked = Boolean(settings[ key ]);
            } else if (element.tagName === "SELECT") {
                const value = settings[ key ];
                element.value = value == null ? "" : String(value);
            } else {
                element.value = cleanValue(settings[ key ]);
            }
        });

        updateProfileDisplay();
        renderOrganizationLogo();
        updateDirtyState();
    }

    function updateProfileDisplay () {
        const first = cleanValue($("asFirstName")?.value);
        const last = cleanValue($("asLastName")?.value);

        const fullName =
            [ first, last ].filter(Boolean).join(" ") || "Your name";

        const initials =
            ((first[ 0 ] || "") + (last[ 0 ] || "")).toUpperCase() || "?";

        const sidebarName = $("asSidebarName");
        const sidebarJobTitle = $("asSidebarJobTitle");
        const jobTitle = cleanValue($("asJobTitle")?.value);
        const avatar = $("asAvatar");
        const largeAvatar = $("asLargeAvatar");

        if (sidebarName) sidebarName.textContent = fullName;
        if (sidebarJobTitle) sidebarJobTitle.textContent = jobTitle || "AskPearls Member";
        if (avatar) avatar.textContent = initials;
        if (largeAvatar) largeAvatar.textContent = initials;
    }

    // ============================================================
    // ORGANIZATION BRANDING
    // Save a public HTTPS URL only. No file upload.
    // ============================================================

    const orgLogoInput = $("asOrgLogoInput");
    const orgLogoImage = $("asOrgLogoImage");
    const orgLogoInitials = $("asOrgLogoInitials");
    const orgMiniLogo = $("asOrgMiniLogo");
    const orgLogoMessage = $("asOrgLogoMessage");
    const orgPreviewName = $("asOrgPreviewName");

    function getOrganizationInitials (name) {
        const words = name.trim().split(/\s+/).filter(Boolean);

        if (!words.length) return "O";

        return words.length === 1
            ? words[ 0 ].slice(0, 2).toUpperCase()
            : (words[ 0 ][ 0 ] +
                words[ words.length - 1 ][ 0 ]).toUpperCase();
    }

    function isValidLogoUrl (value) {
        try {
            const url = new URL(value);

            return url.protocol === "https:" &&
                !url.username &&
                !url.password &&
                value.length <= 500;
        } catch {
            return false;
        }
    }

    function renderOrganizationLogo () {
        if (!orgLogoInput) return;

        const company =
            cleanValue($("asCompanyName")?.value) ||
            "Your organization";

        const initials = getOrganizationInitials(company);

        if (orgPreviewName) {
            orgPreviewName.textContent = company;
        }

        const url = cleanValue(orgLogoInput.value);

        if (orgLogoImage) {
            orgLogoImage.hidden = true;
            orgLogoImage.removeAttribute("src");
        }

        if (orgLogoInitials) {
            orgLogoInitials.hidden = false;
            orgLogoInitials.textContent = initials;
        }

        if (orgMiniLogo) {
            orgMiniLogo.replaceChildren(
                document.createTextNode(initials)
            );
        }

        if (orgLogoMessage) {
            orgLogoMessage.textContent = "";
        }

        if (!url) return;

        if (!isValidLogoUrl(url)) {
            if (orgLogoMessage) {
                orgLogoMessage.textContent =
                    "Enter a valid public HTTPS image URL.";
            }
            return;
        }

        if (orgLogoMessage) {
            orgLogoMessage.textContent = "Loading preview...";
        }

        const img = new Image();

        img.onload = () => {
            if (cleanValue(orgLogoInput.value) !== url) return;

            if (orgLogoImage) {
                orgLogoImage.src = url;
                orgLogoImage.hidden = false;
            }

            if (orgLogoInitials) {
                orgLogoInitials.hidden = true;
            }

            if (orgMiniLogo) {
                const mini = document.createElement("img");
                mini.src = url;
                mini.alt = "";
                orgMiniLogo.replaceChildren(mini);
            }

            if (orgLogoMessage) {
                orgLogoMessage.textContent =
                    "Logo preview ready.";
            }
        };

        img.onerror = () => {
            if (cleanValue(orgLogoInput.value) !== url) return;

            if (orgLogoMessage) {
                orgLogoMessage.textContent =
                    "Image could not be loaded from this URL.";
            }
        };

        img.src = url;
    }

    if (orgLogoInput) {
        orgLogoInput.addEventListener(
            "input",
            renderOrganizationLogo
        );

        orgLogoInput.addEventListener(
            "change",
            renderOrganizationLogo
        );
    }

    $("asOrgLogoRemove")?.addEventListener("click", () => {
        if (!orgLogoInput) return;

        orgLogoInput.value = "";
        renderOrganizationLogo();
        updateDirtyState();
    });

    $("asCompanyName")?.addEventListener(
        "input",
        renderOrganizationLogo
    );

    // ============================================================
    // SAVE STATUS AND TOAST
    // ============================================================

    function updateDirtyState () {
        const current = currentSettings();
        const keys = sectionKeys[ activeSection ] || [];
        const changed = keys.some(k => current[ k ] !== savedSettings[ k ]);

        const status = $("asSaveStatus");
        if (!status) return;

        status.innerHTML = changed
            ? '<i class="bi bi-circle-fill"></i> Unsaved changes'
            : '<i class="bi bi-check-circle-fill"></i> All changes saved';

        status.style.color = changed ? "" : "#0d9278";
    }

    function showToast (message) {
        const toast = $("asToast");
        if (!toast) return;

        toast.textContent = message;
        toast.classList.add("is-visible");

        clearTimeout(toastTimer);

        toastTimer = setTimeout(() => {
            toast.classList.remove("is-visible");
        }, 2800);
    }

    // ============================================================
    // API REQUESTS
    // ============================================================

    function apiHeaders (json) {
        const result = {
            Accept: "application/json"
        };

        if (json) {
            result[ "Content-Type" ] = "application/json";

            const token = document.querySelector(
                'input[name="__RequestVerificationToken"]'
            )?.value;

            if (token) {
                result[ "RequestVerificationToken" ] = token;
            }
        }

        return result;
    }

    async function apiRequest (url, options) {
        const response = await fetch(url, { cache: "no-store", ...options });

        const body = await response.json().catch(() => ({}));

        if (!response.ok || body.success === false) {
            throw new Error(
                body.message ||
                "Account settings request failed."
            );
        }

        return body;
    }

    // ============================================================
    // SUBSCRIPTION SUMMARY
    // ============================================================

    function applySubscription (subscription) {
        if (!subscription) return;

        const plan = subscription.plan || "Free";
        const limit = subscription.widgetLimit;
        const used = Number(subscription.widgetsCreated || 0);

        const remaining = limit == null
            ? null
            : Math.max(
                0,
                Number(
                    subscription.widgetsRemaining ??
                    limit - used
                )
            );

        const percent =
            limit == null || !Number(limit)
                ? 0
                : Math.min(
                    100,
                    Math.round(
                        (used / Number(limit)) * 100
                    )
                );

        if ($("asCurrentPlanName")) {
            $("asCurrentPlanName").textContent =
                plan + " Plan";
        }

        if ($("asWidgetsUsed")) {
            $("asWidgetsUsed").textContent = String(used);
        }

        if ($("asWidgetsLimit")) {
            $("asWidgetsLimit").textContent =
                limit == null ? "No limit" : String(limit);
        }

        if ($("asUsageFill")) {
            $("asUsageFill").style.width = percent + "%";
        }

        if ($("asUsageSummary")) {
            $("asUsageSummary").textContent =
                subscription.isIllustrative
                    ? "No active paid plan is recorded. Showing the Free plan allowance."
                    : percent + "% of your plan allowance used";
        }

        if ($("asWidgetsRemaining")) {
            $("asWidgetsRemaining").textContent =
                remaining == null
                    ? "Unlimited remaining"
                    : remaining + " remaining";
        }

        if ($("asSubscriptionNote")) {
            $("asSubscriptionNote").textContent =
                subscription.isIllustrative
                    ? "No paid subscription is recorded for this account. Plan changes require billing integration."
                    : "Plan and widget usage loaded from your account records. Billing changes are not available here.";
        }
    }

    // ============================================================
    // NOTIFICATIONS
    // ============================================================

    function updateNonEssentialNotifications () {
        const master = $("asNonEssentialNotifications");
        if (!master) return;

        const enabled = master.checked;

        root.querySelectorAll(
            "[data-nonessential='true']"
        ).forEach(row => {
            const input = row.querySelector(
                'input[type="checkbox"]'
            );

            if (!input) return;

            // Security alerts are controlled separately from the optional master switch.
            if (input.id === "asSecurityAlerts") return;
            input.disabled = !enabled;

            row.classList.toggle(
                "as-notification-disabled",
                !enabled
            );

        });

        // Keep this switch interactive when preferences have been loaded.
        // If preferences failed to load, preserve the failure-state protection.
        const security = $("asSecurityAlerts");
        if (security && preferencesLoaded) {
            security.disabled = false;
            security.closest(".as-setting-row")?.classList.remove("as-notification-disabled");
        }

        const status = $("asNotificationMasterStatus");

        if (status) {
            status.textContent = enabled
                ? "Optional notifications are enabled."
                : "Optional notifications are disabled. Security alerts remain independent.";
        }

        updateDirtyState();
    }

    $("asNonEssentialNotifications")?.addEventListener(
        "change",
        () => {
            updateNonEssentialNotifications();

            showToast(
                "Notification preference updated. Save changes to keep it."
            );
        }
    );

    // ============================================================
    // LOAD SETTINGS
    // ============================================================

    async function loadSettings () {
        const saveButton = $("asSave");
        if (saveButton) saveButton.disabled = true;
        if ($("asSaveStatus")) $("asSaveStatus").textContent = "Loading account settings";
        try {
            // Provision an actual Free subscription for authenticated accounts without subscription history.
            // This endpoint never changes existing plans or payment state.
            try {
                await apiRequest(subscriptionEnsureUrl, { method: "POST", credentials: "same-origin", headers: apiHeaders(true) });
            } catch (provisionError) {
                console.warn("Subscription provisioning unavailable; loading existing account data.", provisionError);
            }
            const [ accountResult, prefsResult ] = await Promise.allSettled([
                apiRequest(settingsUrl, { headers: apiHeaders(false) }),
                apiRequest(notificationPreferencesUrl, { headers: apiHeaders(false) })
            ]);
            if (accountResult.status !== "fulfilled") throw accountResult.reason;
            const data = accountResult.value;
            const settings = { ...defaults, ...(data.settings || {}) };
            preferencesLoaded = prefsResult.status === "fulfilled";
            if (preferencesLoaded) {
                Object.assign(settings, pickKeys(prefsResult.value, notificationKeys));
            } else {
                console.error("Notification preferences could not be loaded", prefsResult.reason);
            }
            setSettings(settings);
            updateNonEssentialNotifications();
            savedSettings = currentSettings();
            settingsLoaded = true;
            applySubscription(data.subscription);
            updateDirtyState();
            if ($("asNavBottomMessage")) $("asNavBottomMessage").textContent =
                preferencesLoaded ? "Changes are saved to your account." : "Notification preferences could not be loaded. Retry before saving them.";
            if (!preferencesLoaded) {
                notificationKeys.forEach(k => { const el = $(fieldIds[ k ]); if (el) el.disabled = true; });
                showToast("Notification preferences could not be loaded. Reload this page to retry.");
            }
        } catch (error) {
            settingsLoaded = false;
            if ($("asSaveStatus")) $("asSaveStatus").textContent = "Could not load account settings";
            showToast(error.message || "Could not load your account settings.");
            console.error("Account settings API load failed.", error);
        } finally {
            if (saveButton) saveButton.disabled = !settingsLoaded;
        }
    }

    // ============================================================
    // SECTION NAVIGATION
    // Profile / Organization / Companies / Notifications
    // Security and Preferences removed for MVP.
    // ============================================================

    function showSection (section) {
        activeSection = section;

        // Companies use their own Save company action,
        // not the general step button.
        if ($("asSave")) {
            $("asSave").style.display =
                section === "companies" ? "none" : "";
        }

        if ($("asReset")) {
            $("asReset").style.display =
                section === "companies" ? "none" : "";
        }

        // Update the active navigation item.
        root.querySelectorAll("[data-as-section]")
            .forEach(button => {
                button.classList.toggle(
                    "active",
                    button.dataset.asSection === section
                );
            });

        // Display only the selected content panel.
        root.querySelectorAll("[data-as-panel]")
            .forEach(panel => {
                const active =
                    panel.dataset.asPanel === section;

                panel.hidden = !active;

                panel.classList.toggle("active", active);
            });

        updateDirtyState();
    }

    // ============================================================
    // SCROLL SELECTED SECTION TO TOP
    // ============================================================

    function scrollSelectedSectionToTop (section) {

        const panel = Array.from(
            root.querySelectorAll("[data-as-panel]")
        ).find(item =>
            item.dataset.asPanel === section && !item.hidden
        );

        if (!panel) return;

        // Reset scrollable parent containers.
        // Preserve the left navigation scroll position.
        for (
            let element = panel.parentElement;
            element;
            element = element.parentElement
        ) {
            const style = window.getComputedStyle(element);

            const isScrollable =
                /^(auto|scroll|overlay)$/.test(style.overflowY);

            if (
                isScrollable &&
                element.scrollHeight > element.clientHeight
            ) {
                element.scrollTo({
                    top: 0,
                    behavior: "smooth"
                });
            }
        }

        // Also handle normal document scrolling.
        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    }

    // ============================================================
    // SECTION NAVIGATION CLICK EVENTS
    // ============================================================

    root.querySelectorAll("[data-as-section]")
        .forEach(button => {

            button.addEventListener("click", () => {

                const section = button.dataset.asSection;

                if (!section) return;

                // Change the visible content.
                showSection(section);

                // Wait until the browser applies the panel changes.
                requestAnimationFrame(() => {
                    scrollSelectedSectionToTop(section);
                });

            });

        });


    // ============================================================
    // FIELD CHANGE TRACKING
    // ============================================================

    Object.values(fieldIds).forEach(id => {
        const element = $(id);
        if (!element) return;

        element.addEventListener("input", () => {
            updateProfileDisplay();
            updateDirtyState();
        });

        element.addEventListener("change", () => {
            updateProfileDisplay();
            updateDirtyState();
        });
    });

    // ============================================================
    // SAVE SETTINGS
    // ============================================================

    $("asSave")?.addEventListener("click", async () => {
        if (!settingsLoaded) { showToast("Account settings are still loading."); return; }
        if (activeSection === "companies") {
            showToast("Use the Save company button on the Companies tab.");
            return;
        }
        const keys = sectionKeys[ activeSection ] || [];
        const current = currentSettings();
        if (!keys.some(k => current[ k ] !== savedSettings[ k ])) {
            showToast("No changes to save in this section."); return;
        }
        if (activeSection === "profile") {
            if (!cleanValue(current.firstName) || !cleanValue(current.lastName)) {
                showToast("First and last name are required for your profile.");
                (!cleanValue(current.firstName) ? $("asFirstName") : $("asLastName"))?.focus();
                return;
            }
        }
        if (activeSection === "organization") {
            // An organization can be completed later; save the supplied fields only.
            const logo = cleanValue(current.organizationLogoUrl);
            if (logo && !isValidLogoUrl(logo)) {
                showToast("Please enter a valid HTTPS logo URL.");
                orgLogoInput?.focus(); return;
            }
        }
        const button = $("asSave");
        if (button) button.disabled = true;
        try {
            if (activeSection === "notifications") {
                if (!preferencesLoaded) throw new Error("Notification preferences have not loaded.");
                const payload = { ...pickKeys(current, notificationKeys) };
                const result = await apiRequest(notificationPreferencesUrl, {
                    method: "PUT", headers: apiHeaders(true), body: JSON.stringify(payload)
                });
                // Verify persistence using a fresh read, including Security Alerts = false.
                const verifiedPrefs = await apiRequest(notificationPreferencesUrl + "?verify=" + Date.now(), {
                    method: "GET", headers: apiHeaders(false), cache: "no-store"
                });
                const mismatches = notificationKeys.filter(k => verifiedPrefs[ k ] !== payload[ k ]);
                if (mismatches.length) {
                    console.error("Notification preference persistence mismatch", { payload, verifiedPrefs, mismatches });
                    throw new Error("Notification preferences could not be verified: " + mismatches.join(", "));
                }
                const persisted = { ...payload, ...verifiedPrefs };
                keys.forEach(k => { savedSettings[ k ] = persisted[ k ]; });
                setSettings({ ...currentSettings(), ...pickKeys(persisted, keys) });
                updateNonEssentialNotifications();
            } else {
                const payload = { ...pickKeys(current, keys), saveSection: activeSection };
                const data = await apiRequest(settingsUrl, {
                    method: "PUT", headers: apiHeaders(true), body: JSON.stringify(payload)
                });
                // Never trust a PUT response alone as evidence of database persistence.
                // A separate GET must confirm the values returned by the backend.
                if (!data || data.success !== true) {
                    throw new Error("The API did not confirm that this section was saved.");
                }
                const verified = await apiRequest(settingsUrl + "?verify=" + Date.now(), {
                    method: "GET", headers: apiHeaders(false), cache: "no-store"
                });
                if (!verified || verified.success !== true || !verified.settings) {
                    throw new Error("The API could not verify the saved settings.");
                }
                // SQL uses "0" for empty text. Compare normalized form values,
                // not raw SQL placeholders or incidental line-ending differences.
                const normalized = value => cleanValue(value).replace(/\r\n?/g, "\n");
                const mismatches = keys.filter(k =>
                    normalized(verified.settings[ k ]) !== normalized(current[ k ])
                );
                if (mismatches.length) {
                    console.error("AskPearls persistence mismatch", {
                        section: activeSection,
                        fields: mismatches,
                        submitted: pickKeys(current, keys),
                        readBack: pickKeys(verified.settings, keys),
                        putResponse: data.settings ? pickKeys(data.settings, keys) : null
                    });
                    throw new Error(
                        "Profile save could not be verified for: " +
                        mismatches.join(", ") + ". Check the Console and API response."
                    );
                }
                keys.forEach(k => { savedSettings[ k ] = verified.settings[ k ]; });
                setSettings({ ...currentSettings(), ...pickKeys(verified.settings, keys) });
            }
            updateDirtyState();
            showToast(activeSection === "profile" ? "Profile saved." :
                activeSection === "organization" ? "Organization saved." : "Notifications saved.");
        } catch (error) {
            showToast(error.message || "Could not save this section.");
        } finally {
            if (button) button.disabled = !settingsLoaded;
        }
    });

    // ============================================================
    // RESET SETTINGS
    // ============================================================

    $("asReset")?.addEventListener("click", () => {
        const keys = sectionKeys[ activeSection ] || [];
        setSettings({ ...currentSettings(), ...Object.fromEntries(keys.map(k => [ k, savedSettings[ k ] ])) });
        updateNonEssentialNotifications();

        showToast(
            "Settings reset to the last saved values."
        );
    });

    // ============================================================
    // SUBSCRIPTION PLANS MODAL
    // UI prototype only; no billing changes.
    // ============================================================

    const plansModal = $("asPlansModal");
    const plansConfirmation = $("asPlanConfirmation");

    let selectedPlan = null;

    function resetPlanSelection () {
        selectedPlan = null;

        if (plansConfirmation) {
            plansConfirmation.hidden = true;
        }

        root.querySelectorAll(
            ".as-plan-option"
        ).forEach(card => {
            card.classList.remove(
                "as-plan-selected"
            );
        });

        if ($("asPlanConfirmationTitle")) {
            $("asPlanConfirmationTitle").textContent =
                "Confirm plan selection";
        }

        if ($("asPlanConfirmationText")) {
            $("asPlanConfirmationText").textContent = "";
        }
    }

    function openPlansModal () {
        if (!plansModal) return;

        resetPlanSelection();

        plansModal.hidden = false;
        document.body.style.overflow = "hidden";

        $("asPlansClose")?.focus();
    }

    function closePlansModal () {
        if (!plansModal) return;

        plansModal.hidden = true;
        document.body.style.overflow = "";

        $("asUpgradePlan")?.focus();
    }

    $("asUpgradePlan")?.addEventListener(
        "click",
        openPlansModal
    );

    $("asPlansClose")?.addEventListener(
        "click",
        closePlansModal
    );

    $("asPlansDone")?.addEventListener(
        "click",
        closePlansModal
    );

    $("asPlanCancel")?.addEventListener(
        "click",
        resetPlanSelection
    );

    root.querySelectorAll(
        "[data-select-plan]"
    ).forEach(button => {
        button.addEventListener("click", () => {
            selectedPlan =
                button.dataset.selectPlan;

            root.querySelectorAll(
                ".as-plan-option"
            ).forEach(card => {
                card.classList.toggle(
                    "as-plan-selected",
                    card.dataset.plan === selectedPlan
                );
            });

            if ($("asPlanConfirmationTitle")) {
                $("asPlanConfirmationTitle").textContent =
                    "Confirm " + selectedPlan + " plan";
            }

            if ($("asPlanConfirmationText")) {
                $("asPlanConfirmationText").textContent =
                    selectedPlan === "Free"
                        ? "You are already using the Free plan."
                        : "You selected the " +
                        selectedPlan +
                        " plan. This demo will not charge you or change your subscription.";
            }

            if (plansConfirmation) {
                plansConfirmation.hidden = false;

                plansConfirmation.scrollIntoView({
                    behavior: "smooth",
                    block: "nearest"
                });
            }
        });
    });

    $("asPlanConfirm")?.addEventListener(
        "click",
        () => {
            if (!selectedPlan) {
                showToast(
                    "Please select a plan first."
                );
                return;
            }

            if (selectedPlan === "Free") {
                showToast(
                    "The Free plan is already your current plan."
                );
            } else {
                showToast(
                    selectedPlan +
                    " selected successfully in the demo. No subscription was changed."
                );
            }

            closePlansModal();
        }
    );

    plansModal?.addEventListener(
        "click",
        event => {
            if (event.target === plansModal) {
                closePlansModal();
            }
        }
    );

    document.addEventListener(
        "keydown",
        event => {
            if (
                event.key === "Escape" &&
                plansModal &&
                !plansModal.hidden
            ) {
                closePlansModal();
            }
        }
    );

    // ============================================================
    // INITIALIZATION
    // ============================================================

    showSection("profile");
    renderOrganizationLogo();
    loadSettings();

})();
