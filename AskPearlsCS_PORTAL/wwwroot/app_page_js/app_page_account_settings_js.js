(function () {
    "use strict";

    const root = document.querySelector(".account-settings-page");
    if (!root || root.dataset.asInitialized === "true") return;
    root.dataset.asInitialized = "true";

    const $ = id => root.querySelector("#" + id);

    const defaults = {
nonEssentialNotifications: true,
        firstName: "John",
        lastName: "Doe",
        email: "john.doe@example.com",
        jobTitle: "Product Admin",
        bio: "Building better customer experiences with AskPearls.",
        companyName: "AskPearls Limited",
        website: "https://askpearls.com",
        industry: "technology",
        companySize: "2-10",
        country: "EG",
        twoFactor: false,
        emailNotifications: true,
        activityReports: true,
        productUpdates: false,
        securityAlerts: true,
        language: "en",
        timezone: "Africa/Cairo",
        dateFormat: "dd/mm/yyyy",
        weekStart: "monday",
        compactMode: false
    };

    const fieldIds = {
nonEssentialNotifications: "asNonEssentialNotifications",
        firstName: "asFirstName",
        lastName: "asLastName",
        email: "asEmail",
        jobTitle: "asJobTitle",
        bio: "asBio",
        companyName: "asCompanyName",
        website: "asWebsite",
        industry: "asIndustry",
        companySize: "asCompanySize",
        country: "asCountry",
        twoFactor: "asTwoFactor",
        emailNotifications: "asEmailNotifications",
        activityReports: "asActivityReports",
        productUpdates: "asProductUpdates",
        securityAlerts: "asSecurityAlerts",
        language: "asLanguage",
        timezone: "asTimezone",
        dateFormat: "asDateFormat",
        weekStart: "asWeekStart",
        compactMode: "asCompactMode"
    };

    let savedSettings = { ...defaults };
    let toastTimer = null;

    function currentSettings() {
        const result = {};
        Object.entries(fieldIds).forEach(([key, id]) => {
            const element = $(id);
            result[key] = element.type === "checkbox" ? element.checked : element.value;
        });
        return result;
    }

    function setSettings(settings) {
        Object.entries(fieldIds).forEach(([key, id]) => {
            const element = $(id);
            if (element.type === "checkbox") {
                element.checked = Boolean(settings[key]);
            } else {
                element.value = settings[key];
            }
        });
        updateProfileDisplay();
        updateDirtyState();
    }

    function updateProfileDisplay() {
        const first = $("asFirstName").value.trim();
        const last = $("asLastName").value.trim();
        const fullName = [first, last].filter(Boolean).join(" ") || "Your name";
        const initials = ((first[0] || "") + (last[0] || "")).toUpperCase() || "?";

        $("asSidebarName").textContent = fullName;
        $("asAvatar").textContent = initials;
        $("asLargeAvatar").textContent = initials;
    }



    
    // Organization branding — local preview only
    const orgLogoInput = $("asOrgLogoInput");
    const orgLogoImage = $("asOrgLogoImage");
    const orgLogoInitials = $("asOrgLogoInitials");
    const orgLogoPreview = $("asOrgLogoPreview");
    const orgMiniLogo = $("asOrgMiniLogo");
    const orgLogoMessage = $("asOrgLogoMessage");
    const orgPreviewName = $("asOrgPreviewName");

    let orgLogoObjectUrl = null;

    function getOrganizationInitials(name) {
        const words = name.trim().split(/\s+/).filter(Boolean);

        if (!words.length) return "O";

        return words.length === 1
            ? words[0].slice(0, 2).toUpperCase()
            : (words[0][0] + words[words.length - 1][0]).toUpperCase();
    }

    function updateOrganizationPreview() {
        const companyName = $("asCompanyName").value.trim()
            || "Your organization";

        const initials = getOrganizationInitials(companyName);

        orgPreviewName.textContent = companyName;

        if (!orgLogoObjectUrl) {
            orgLogoInitials.textContent = initials;
            orgMiniLogo.textContent = initials;
        }
    }

    function removeOrganizationLogo() {
        if (orgLogoObjectUrl) {
            URL.revokeObjectURL(orgLogoObjectUrl);
            orgLogoObjectUrl = null;
        }

        orgLogoImage.removeAttribute("src");
        orgLogoImage.hidden = true;
        orgLogoInitials.hidden = false;

        orgMiniLogo.replaceChildren(
            document.createTextNode(getOrganizationInitials(
                $("asCompanyName").value.trim() || "Organization"
            ))
        );

        orgLogoInput.value = "";
        orgLogoMessage.textContent = "Logo preview removed.";
    }

    orgLogoInput.addEventListener("change", () => {
        const file = orgLogoInput.files && orgLogoInput.files[0];

        orgLogoMessage.textContent = "";

        if (!file) return;

        const allowedTypes = [
            "image/png",
            "image/jpeg",
            "image/webp",
            "image/svg+xml"
        ];

        if (!allowedTypes.includes(file.type)) {
            orgLogoInput.value = "";
            orgLogoMessage.textContent =
                "Choose a PNG, JPG, WebP, or SVG image.";
            return;
        }

        if (file.size > 2 * 1024 * 1024) {
            orgLogoInput.value = "";
            orgLogoMessage.textContent =
                "The logo must be no larger than 2 MB.";
            return;
        }

        const nextUrl = URL.createObjectURL(file);
        const testImage = new Image();

        testImage.onload = () => {
            if (orgLogoObjectUrl) {
                URL.revokeObjectURL(orgLogoObjectUrl);
            }

            orgLogoObjectUrl = nextUrl;

            orgLogoImage.src = nextUrl;
            orgLogoImage.hidden = false;
            orgLogoInitials.hidden = true;

            const miniImage = document.createElement("img");
            miniImage.src = nextUrl;
            miniImage.alt = "";

            orgMiniLogo.replaceChildren(miniImage);

            orgLogoMessage.textContent =
                "Logo preview updated. Changes are not uploaded or saved.";
        };

        testImage.onerror = () => {
            URL.revokeObjectURL(nextUrl);
            orgLogoInput.value = "";
            orgLogoMessage.textContent =
                "This image could not be previewed. Choose another file.";
        };

        testImage.src = nextUrl;
    });

    $("asOrgLogoRemove").addEventListener("click", removeOrganizationLogo);

    $("asCompanyName").addEventListener("input", updateOrganizationPreview);

    updateOrganizationPreview();




    function updateDirtyState() {
        const changed = JSON.stringify(currentSettings()) !== JSON.stringify(savedSettings);
        const status = $("asSaveStatus");
        status.innerHTML = changed
            ? '<i class="bi bi-circle-fill"></i> Unsaved changes'
            : '<i class="bi bi-check-circle-fill"></i> All changes saved';
        status.style.color = changed ? "" : "#0d9278";
    }

    function showToast(message) {
        const toast = $("asToast");
        toast.textContent = message;
        toast.classList.add("is-visible");
        clearTimeout(toastTimer);
        toastTimer = setTimeout(() => toast.classList.remove("is-visible"), 2800);
    }

    function updateNonEssentialNotifications() {
    const master = $("asNonEssentialNotifications");
    const enabled = master.checked;

    root.querySelectorAll("[data-nonessential='true']").forEach(row => {
        const input = row.querySelector('input[type="checkbox"]');

        if (!input) return;

        input.disabled = !enabled;
        row.classList.toggle("as-notification-disabled", !enabled);

        if (!enabled) {
            input.checked = false;
        }
    });

    $("asNotificationMasterStatus").textContent = enabled
        ? "Optional notifications are enabled."
        : "Optional notifications are disabled.";

    updateDirtyState();
}

$("asNonEssentialNotifications").addEventListener("change", () => {
    updateNonEssentialNotifications();

    showToast(
        $("asNonEssentialNotifications").checked
            ? "Optional notifications enabled in this demo."
            : "Optional notifications disabled in this demo."
    );
});

// Apply the correct initial state.
updateNonEssentialNotifications();
     
    // Security interactions — frontend demo only
    const twoFactor = $("asTwoFactor");
    const twoFactorModal = $("asTwoFactorModal");
    const sessionsModal = $("asSessionsModal");
    const signOutModal = $("asSignOutModal");

    let twoFactorPendingValue = false;
    let lastSecurityTrigger = null;
    let lastSessionTrigger = null;

    function openSecurityModal(modal, trigger) {
        lastSecurityTrigger = trigger || document.activeElement;
        modal.hidden = false;
        document.body.style.overflow = "hidden";

        const firstButton = modal.querySelector(
            ".as-modal-close, button"
        );

        if (firstButton) firstButton.focus();
    }

    function closeSecurityModal(modal, trigger) {
        modal.hidden = true;

        if (sessionsModal.hidden &&
            twoFactorModal.hidden &&
            signOutModal.hidden) {
            document.body.style.overflow = "";
        }

        const target = trigger || lastSecurityTrigger;
        if (target && typeof target.focus === "function") {
            target.focus();
        }
    }

    function updateSecuritySummary() {
        const enabled = twoFactor.checked;

        $("asTwoFactorDescription").textContent = enabled
            ? "Enabled in this demo only; real sign-in security is unchanged."
            : "Currently disabled in this demo.";

        $("asSecuritySummary").textContent = enabled
            ? "Two-factor authentication is marked as enabled in this demo."
            : "Two-factor authentication is currently marked as disabled.";

        $("asSecurityStatus").classList.toggle("is-enabled", enabled);
        $("asSecurityStatus").innerHTML = enabled
            ? '<i class="bi bi-shield-check"></i> Demo enabled'
            : '<i class="bi bi-info-circle"></i> Review settings';
    }

    // Two-factor confirmation
    twoFactor.addEventListener("change", () => {
        twoFactorPendingValue = twoFactor.checked;

        // Wait for confirmation before updating the displayed setting.
        twoFactor.checked = !twoFactorPendingValue;

        $("asTwoFactorActionTitle").textContent = twoFactorPendingValue
            ? "Enable two-factor authentication?"
            : "Disable two-factor authentication?";

        $("asTwoFactorActionText").textContent = twoFactorPendingValue
            ? "The demo will mark two-factor authentication as enabled."
            : "The demo will mark two-factor authentication as disabled.";

        $("asTwoFactorConfirm").textContent = twoFactorPendingValue
            ? "Enable in demo"
            : "Disable in demo";

        openSecurityModal(twoFactorModal, twoFactor);
    });

    function cancelTwoFactor() {
        closeSecurityModal(twoFactorModal, twoFactor);
    }

    $("asTwoFactorClose").addEventListener("click", cancelTwoFactor);
    $("asTwoFactorCancel").addEventListener("click", cancelTwoFactor);

    $("asTwoFactorConfirm").addEventListener("click", () => {
        twoFactor.checked = twoFactorPendingValue;
        updateSecuritySummary();
        closeSecurityModal(twoFactorModal, twoFactor);
        showToast(
            twoFactor.checked
                ? "2FA enabled in the demo only."
                : "2FA disabled in the demo only."
        );
    });

    // Active sessions
    function getVisibleDemoSessions() {
        return Array.from(
            root.querySelectorAll(".as-session-item")
        ).filter(item => !item.classList.contains("is-revoked"));
    }

    function updateSessionCount() {
        const count = getVisibleDemoSessions().length;
        const label = count + (count === 1 ? " demo session" : " demo sessions");

        $("asSessionCount").textContent = label;
        $("asSessionsSummaryCount").textContent = label;
    }

    function showSessionFeedback(message) {
        const feedback = $("asSessionFeedback");
        feedback.textContent = message;
        feedback.hidden = false;
    }

    $("asViewSessions").addEventListener("click", () => {
        lastSessionTrigger = $("asViewSessions");
        $("asSessionFeedback").hidden = true;
        openSecurityModal(sessionsModal, $("asViewSessions"));
        updateSessionCount();
    });

    function closeSessionsModal() {
        closeSecurityModal(sessionsModal, lastSessionTrigger);
    }

    $("asSessionsClose").addEventListener("click", closeSessionsModal);
    $("asSessionsDone").addEventListener("click", closeSessionsModal);

    // Revoke one mock session
    root.querySelectorAll("[data-revoke-session]").forEach(button => {
        button.addEventListener("click", () => {
            const sessionId = button.dataset.revokeSession;
            const item = root.querySelector(
                '[data-session-id="' + sessionId + '"]'
            );

            if (!item || item.classList.contains("is-revoked")) return;

            item.classList.add("is-revoked");
            button.disabled = true;
            button.textContent = "Revoked";

            showSessionFeedback(
                "Demo session marked as revoked. No real device was signed out."
            );

            updateSessionCount();
            showToast("Demo session revoked. No real session was affected.");
        });
    });

    // Sign out other mock sessions
    $("asSignOutOthers").addEventListener("click", () => {
        openSecurityModal(signOutModal, $("asSignOutOthers"));
    });

    function closeSignOutModal() {
        closeSecurityModal(signOutModal, $("asSignOutOthers"));
    }

    $("asSignOutClose").addEventListener("click", closeSignOutModal);
    $("asSignOutCancel").addEventListener("click", closeSignOutModal);

    $("asSignOutConfirm").addEventListener("click", () => {
        root.querySelectorAll(".as-session-item").forEach(item => {
            if (item.dataset.sessionId === "session-current") return;

            item.classList.add("is-revoked");

            const revokeButton = item.querySelector("[data-revoke-session]");
            if (revokeButton) {
                revokeButton.disabled = true;
                revokeButton.textContent = "Signed out";
            }
        });

        updateSessionCount();
        closeSignOutModal();

        showSessionFeedback(
            "Other demo sessions have been marked as signed out. " +
            "Your current session remains active."
        );

        showToast("Other demo sessions signed out. No real sessions were affected.");
    });

    // Close security dialogs by clicking outside
    [twoFactorModal, sessionsModal, signOutModal].forEach(modal => {
        modal.addEventListener("click", event => {
            if (event.target !== modal) return;

            if (modal === twoFactorModal) cancelTwoFactor();
            else if (modal === sessionsModal) closeSessionsModal();
            else closeSignOutModal();
        });
    });

    // Escape closes the topmost security modal
    document.addEventListener("keydown", event => {
        if (event.key !== "Escape") return;

        if (!signOutModal.hidden) closeSignOutModal();
        else if (!sessionsModal.hidden) closeSessionsModal();
        else if (!twoFactorModal.hidden) cancelTwoFactor();
    });

    updateSecuritySummary();
    updateSessionCount();



    function showSection(section) {
        root.querySelectorAll("[data-as-section]").forEach(button => {
            button.classList.toggle("active", button.dataset.asSection === section);
        });

        root.querySelectorAll("[data-as-panel]").forEach(panel => {
            const active = panel.dataset.asPanel === section;
            panel.hidden = !active;
            panel.classList.toggle("active", active);
        });
    }

    root.querySelectorAll("[data-as-section]").forEach(button => {
        button.addEventListener("click", () => showSection(button.dataset.asSection));
    });

    Object.values(fieldIds).forEach(id => {
        const element = $(id);
        element.addEventListener("input", () => {
            updateProfileDisplay();
            updateDirtyState();
        });
        element.addEventListener("change", () => {
            updateProfileDisplay();
            updateDirtyState();
        });
    });

    $("asSave").addEventListener("click", () => {
        const firstName = $("asFirstName").value.trim();
        const lastName = $("asLastName").value.trim();
        const email = $("asEmail").value.trim();
        const company = $("asCompanyName").value.trim();

        if (!firstName || !lastName || !email || !company) {
            showToast("Please complete all required fields.");
            const firstInvalid = !firstName ? $("asFirstName")
                : !lastName ? $("asLastName")
                : !email ? $("asEmail")
                : $("asCompanyName");
            firstInvalid.focus();
            return;
        }

        if (!$("asEmail").checkValidity()) {
            showToast("Please enter a valid email address.");
            $("asEmail").focus();
            return;
        }

        if ($("asWebsite").value.trim() && !$("asWebsite").checkValidity()) {
            showToast("Please enter a valid website URL.");
            $("asWebsite").focus();
            return;
        }

        savedSettings = currentSettings();
        updateDirtyState();
        showToast("Changes saved in this page session only. Backend saving is not connected.");
    });

    $("asReset").addEventListener("click", () => {
        setSettings(savedSettings);
        showToast("Settings reset to the last saved values.");
    });

    // $("asChangePassword").addEventListener("click", () => {
    //     showToast("Password changes will be enabled when authentication integration is implemented.");
    // });

    
        const passwordModal = $("asPasswordModal");
        const passwordForm = $("asPasswordForm");
        const passwordIds = [
            "asCurrentPassword",
            "asNewPassword",
            "asConfirmPassword"
        ];

        function clearPasswordErrors() {
            root.querySelectorAll(".as-password-error").forEach(el => {
                el.textContent = "";
            });

            $("asPasswordFormMessage").textContent = "";
        }

        function resetPasswordForm() {
            passwordForm.reset();
            clearPasswordErrors();
            updatePasswordStrength();
            root.querySelectorAll("[data-password-toggle]").forEach(button => {
                const input = $(button.dataset.passwordToggle);
                input.type = "password";
                button.innerHTML = '<i class="bi bi-eye"></i>';
                button.setAttribute("aria-label", "Show password");
            });
        }

        function openPasswordModal() {
            resetPasswordForm();
            passwordModal.hidden = false;
            document.body.style.overflow = "hidden";
            $("asCurrentPassword").focus();
        }

        function closePasswordModal() {
            passwordModal.hidden = true;
            document.body.style.overflow = "";
            $("asChangePassword").focus();
        }

        function updatePasswordStrength() {
            const password = $("asNewPassword").value;
            const checks = {
                length: password.length >= 8,
                upper: /[A-Z]/.test(password),
                lower: /[a-z]/.test(password),
                number: /\d/.test(password)
            };

            Object.entries(checks).forEach(([rule, passed]) => {
                const item = root.querySelector('[data-rule="' + rule + '"]');
                item.classList.toggle("met", passed);
            });

            const score = Object.values(checks).filter(Boolean).length;
            const bar = $("asPasswordStrengthBar");
            const label = $("asPasswordStrengthText");

            bar.style.width = (score * 25) + "%";
            bar.style.background = score <= 1 ? "#d94d58"
                : score <= 2 ? "#e0a23c"
                    : score === 3 ? "#4c91d9" : "#0da88b";

            label.textContent = !password ? "Enter a new password"
                : score <= 1 ? "Weak"
                    : score === 2 ? "Fair"
                        : score === 3 ? "Good" : "Meets demo requirements";
        }

        $("asChangePassword").addEventListener("click", openPasswordModal);
        $("asPasswordClose").addEventListener("click", closePasswordModal);
        $("asPasswordCancel").addEventListener("click", closePasswordModal);
        $("asPasswordReset").addEventListener("click", resetPasswordForm);

        $("asNewPassword").addEventListener("input", () => {
            updatePasswordStrength();
            $("asPasswordFormMessage").textContent = "";
        });

        root.querySelectorAll("[data-password-toggle]").forEach(button => {
            button.addEventListener("click", () => {
                const input = $(button.dataset.passwordToggle);
                const showing = input.type === "password";

                input.type = showing ? "text" : "password";
                button.innerHTML = showing
                    ? '<i class="bi bi-eye-slash"></i>'
                    : '<i class="bi bi-eye"></i>';

                button.setAttribute(
                    "aria-label",
                    showing ? "Hide password" : "Show password"
                );
            });
        });

        passwordModal.addEventListener("click", event => {
            if (event.target === passwordModal) closePasswordModal();
        });

        document.addEventListener("keydown", event => {
            if (event.key === "Escape" && !passwordModal.hidden) {
                closePasswordModal();
            }
        });

        passwordForm.addEventListener("submit", event => {
            event.preventDefault();
            clearPasswordErrors();

            const current = $("asCurrentPassword").value;
            const password = $("asNewPassword").value;
            const confirm = $("asConfirmPassword").value;
            let firstInvalid = null;

            function error(id, message) {
                const target = root.querySelector('[data-error-for="' + id + '"]');
                target.textContent = message;
                if (!firstInvalid) firstInvalid = $(id);
            }

            if (!current) {
                error("asCurrentPassword", "Enter a current password for this demo.");
            }

            if (password.length < 8 ||
                !/[A-Z]/.test(password) ||
                !/[a-z]/.test(password) ||
                !/\d/.test(password)) {
                error("asNewPassword",
                    "Use at least 8 characters with uppercase, lowercase, and a number.");
            }

            if (!confirm) {
                error("asConfirmPassword", "Confirm your new password.");
            } else if (password !== confirm) {
                error("asConfirmPassword", "The passwords do not match.");
            }

            if (firstInvalid) {
                firstInvalid.focus();
                return;
            }

            $("asPasswordFormMessage").style.color = "#0d9278";
            $("asPasswordFormMessage").textContent =
                "Demo validation passed. No password was changed or stored.";

            showToast("Demo validation passed. No real password changes were made.");
        });


    // $("asUpgradePlan").addEventListener("click", () => {
    //     showToast("Plan selection will be available when subscription management is implemented.");
    // });

    
        // Subscription plans modal — UI prototype only
        const plansModal = $("asPlansModal");
        const plansConfirmation = $("asPlanConfirmation");
        let selectedPlan = null;

        function resetPlanSelection() {
            selectedPlan = null;
            plansConfirmation.hidden = true;

            root.querySelectorAll(".as-plan-option").forEach(card => {
                card.classList.remove("as-plan-selected");
            });

            $("asPlanConfirmationTitle").textContent =
                "Confirm plan selection";
            $("asPlanConfirmationText").textContent = "";
        }

        function openPlansModal() {
            resetPlanSelection();
            plansModal.hidden = false;
            document.body.style.overflow = "hidden";
            $("asPlansClose").focus();
        }

        function closePlansModal() {
            plansModal.hidden = true;
            document.body.style.overflow = "";
            $("asUpgradePlan").focus();
        }

        $("asUpgradePlan").addEventListener("click", openPlansModal);
        $("asPlansClose").addEventListener("click", closePlansModal);
        $("asPlansDone").addEventListener("click", closePlansModal);

        $("asPlanCancel").addEventListener("click", resetPlanSelection);

        root.querySelectorAll("[data-select-plan]").forEach(button => {
            button.addEventListener("click", () => {
                selectedPlan = button.dataset.selectPlan;

                root.querySelectorAll(".as-plan-option").forEach(card => {
                    card.classList.toggle(
                        "as-plan-selected",
                        card.dataset.plan === selectedPlan
                    );
                });

                $("asPlanConfirmationTitle").textContent =
                    "Confirm " + selectedPlan + " plan";

                $("asPlanConfirmationText").textContent =
                    selectedPlan === "Free"
                        ? "You are already using the Free plan."
                        : "You selected the " + selectedPlan +
                        " plan. This demo will not charge you or change your subscription.";

                plansConfirmation.hidden = false;

                plansConfirmation.scrollIntoView({
                    behavior: "smooth",
                    block: "nearest"
                });
            });
        });

        $("asPlanConfirm").addEventListener("click", () => {
            if (!selectedPlan) {
                showToast("Please select a plan first.");
                return;
            }

            if (selectedPlan === "Free") {
                showToast("The Free plan is already your current plan.");
            } else {
                showToast(
                    selectedPlan +
                    " selected successfully in the demo. No subscription was changed."
                );
            }

            closePlansModal();
        });

        plansModal.addEventListener("click", event => {
            if (event.target === plansModal) {
                closePlansModal();
            }
        });

        document.addEventListener("keydown", event => {
            if (event.key === "Escape" && !plansModal.hidden) {
                closePlansModal();
            }
        });


    setSettings(defaults);
    savedSettings = currentSettings();
    updateDirtyState();
    showSection("profile");
})();

