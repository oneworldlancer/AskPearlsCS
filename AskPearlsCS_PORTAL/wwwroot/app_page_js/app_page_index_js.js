(function () {
    "use strict";

    function initialize () {
        const page = document.documentElement;
        const themeButton = document.getElementById("theme-toggle") || document.querySelector("[data-theme-toggle]");
        const authCard = document.getElementById("google-auth-card") || document.querySelector("[data-google-auth-url]");
        const signInButton = document.getElementById("google-signin") || document.querySelector("[data-google-signin]");
        const message = document.getElementById("auth-message") || document.querySelector("[data-auth-message]");
        const year = document.getElementById("footer-year") || document.querySelector("[data-year]");
        const modal = document.getElementById("signin-info-modal");
        const dialog = modal && modal.querySelector("[role=dialog]");
        const themeStorageKey = "askpearls-home-theme";
        let returnFocus = null;

        if (year) year.textContent = String(new Date().getFullYear());

        function setTheme (theme, persist) {
            const selected = theme === "dark" ? "dark" : "light";
            page.dataset.theme = selected;
            if (themeButton) {
                const nextTheme = selected === "dark" ? "light" : "dark";
                themeButton.setAttribute("aria-label", `Switch to ${ nextTheme } theme`);
                themeButton.title = `Switch to ${ nextTheme } theme`;
            }
            if (persist) {
                try { window.localStorage.setItem(themeStorageKey, selected); } catch (_) { /* Storage may be disabled. */ }
            }
        }

        let savedTheme = "";
        try { savedTheme = window.localStorage.getItem(themeStorageKey) || ""; } catch (_) { /* Use the page default. */ }
        setTheme(savedTheme || page.dataset.theme || "light", false);
        if (themeButton) themeButton.addEventListener("click", function () {
            setTheme(page.dataset.theme === "dark" ? "light" : "dark", true);
        });

        function showMessage (text) { if (message) message.textContent = text; }
        async function beginGoogleSignIn () {
            showMessage("");
            if (window.AskPearlsGoogleAuth && typeof window.AskPearlsGoogleAuth.start === "function") {
                try { await window.AskPearlsGoogleAuth.start(); }
                catch (error) { showMessage(error && error.message ? error.message : "Google sign-in could not be started. Please try again."); }
                return;
            }
            const configuredUrl = (authCard && authCard.dataset.googleAuthUrl) || window.AskPearlsGoogleAuthUrl || "";
            if (!configuredUrl.trim()) { showMessage("Google sign-in is not connected yet. Check the portal OAuth configuration."); return; }
            let destination;
            try { destination = new URL(configuredUrl, window.location.origin); }
            catch (_) { showMessage("The Google sign-in URL is invalid. Check the portal OAuth configuration."); return; }
            if (destination.protocol !== "https:" || !destination.pathname.endsWith("/api/auth/google/start")) {
                showMessage("The Google sign-in URL must be an HTTPS AskPearls API authentication route.");
                return;
            }
            if (signInButton) {
                signInButton.disabled = true;
                signInButton.setAttribute("aria-busy", "true");
                const label = signInButton.querySelector("span:not(.button-arrow)");
                if (label) label.textContent = "Connecting to Google…";
            }
            window.location.assign(destination.href);
        }
        if (signInButton) signInButton.addEventListener("click", beginGoogleSignIn);

        if (!modal || !dialog) return;
        const tabs = Array.from(modal.querySelectorAll("[data-info-tab]"));
        const panels = Array.from(modal.querySelectorAll("[data-info-panel]"));

        function selectInfo (name, moveFocus) {
            tabs.forEach(function (tab) {
                const active = tab.dataset.infoTab === name;
                tab.setAttribute("aria-selected", String(active));
                tab.tabIndex = active ? 0 : -1;
                if (active && moveFocus) tab.focus();
            });
            panels.forEach(function (panel) { panel.hidden = panel.dataset.infoPanel !== name; });
            const selected = tabs.find(function (tab) { return tab.dataset.infoTab === name; });
            const heading = modal.querySelector("#info-modal-title");
            if (heading && selected) {
                const titles = { help: "Help & support", terms: "Terms of Service", privacy: "Privacy Policy", contact: "Help & support" };
                heading.textContent = titles[ name ] || titles.help;
            }
        }

        function openInfo (name, trigger) {
            returnFocus = trigger || document.activeElement;
            modal.hidden = false;
            document.body.classList.add("info-modal-open");
            selectInfo(name || "help", false);
            const activeTab = tabs.find(function (tab) { return tab.dataset.infoTab === (name || "help"); });
            (modal.querySelector(".info-close") || activeTab || dialog).focus();
        }

        function closeInfo () {
            if (modal.hidden) return;
            modal.hidden = true;
            document.body.classList.remove("info-modal-open");
            if (returnFocus && typeof returnFocus.focus === "function") returnFocus.focus();
        }

        document.querySelectorAll("[data-info-open]").forEach(function (trigger) {
            trigger.addEventListener("click", function (event) {
                event.preventDefault();
                if (!modal.hidden && modal.contains(trigger)) {
                    selectInfo(trigger.dataset.infoOpen, false);
                    const selectedTab = tabs.find(function (tab) { return tab.dataset.infoTab === trigger.dataset.infoOpen; });
                    if (selectedTab) selectedTab.focus();
                    return;
                }
                openInfo(trigger.dataset.infoOpen, trigger);
            });
        });
        modal.querySelectorAll("[data-info-close]").forEach(function (button) { button.addEventListener("click", closeInfo); });
        tabs.forEach(function (tab, index) {
            tab.addEventListener("click", function () { selectInfo(tab.dataset.infoTab, false); });
            tab.addEventListener("keydown", function (event) {
                if (event.key !== "ArrowRight" && event.key !== "ArrowLeft") return;
                event.preventDefault();
                const step = event.key === "ArrowRight" ? 1 : -1;
                const next = tabs[ (index + step + tabs.length) % tabs.length ];
                selectInfo(next.dataset.infoTab, true);
            });
        });
        const supportForm = modal.querySelector("[data-support-form]");
        if (supportForm) supportForm.addEventListener("submit", function (event) {
            event.preventDefault();
            const status = supportForm.querySelector("[data-support-status]");
            if (status) status.textContent = "Support requests are not connected yet. Please contact your AskPearls administrator.";
        });
        dialog.addEventListener("keydown", function (event) {
            if (event.key !== "Tab") return;
            const focusable = Array.from(dialog.querySelectorAll("button:not([disabled]), a[href], [tabindex]:not([tabindex='-1'])"))
                .filter(function (element) { return !element.closest("[hidden]"); });
            if (!focusable.length) return;
            const first = focusable[ 0 ];
            const last = focusable[ focusable.length - 1 ];
            if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
            else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
        });
        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape" && !modal.hidden) closeInfo();
        });
    }

    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", initialize, { once: true });
    else initialize();
})();
