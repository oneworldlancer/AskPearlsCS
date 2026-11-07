(function () {
    "use strict";
    const root = document.querySelector('.ri-page[data-ap-page="recommendation-items"]');
    if (!root || root.dataset.initialized === "true") return;
    root.dataset.initialized = "true";
    const $ = id => root.querySelector("#" + id);
    const apiBase = "/Widget/Api";

    const storageKey = "askpearls.activeWidgetPublicKey";
    const select = $("riWidgetSelect"), tbody = $("riTableBody"), modal = $("riModal"), form = $("riForm");
    let widgets = [], items = [], versionNumber = "", selectedKey = "", editingId = null, busy = false;
    let loadSequence = 0;
    const antiforgeryToken = () => document.querySelector('input[name="__RequestVerificationToken"]')?.value
        || document.querySelector('meta[name="request-verification-token"]')?.content
        || document.querySelector('meta[name="csrf-token"]')?.content || "";
    const headers = (write = false) => ({ Accept: "application/json", "Content-Type": "application/json", ...(write ? { "RequestVerificationToken": antiforgeryToken() } : {}) });
    const esc = value => String(value ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[ c ]));
    async function read (response) { const text = await response.text(); let data = {}; try { data = text ? JSON.parse(text) : {}; } catch { throw new Error("The API returned an unreadable response."); } if (!response.ok || data.success === false) throw new Error(data.message || `API request failed (HTTP ${ response.status }).`); return data; }
    function message (text, kind) { const node = $("riMessage"); node.textContent = text || ""; node.hidden = !text; node.className = "ri-message" + (kind ? " is-" + kind : ""); }
    function currentWidget () { return widgets.find(x => String(x.publicKey ?? x.PublicKey) === selectedKey); }
    function formatVersion (widget) {
        if (!widget) return "—"; const draft = widget.draftVersionNumber ?? widget.DraftVersionNumber; const live = widget.publishedVersionNumber ?? widget.PublishedVersionNumber; const current = widget.versionNumber ?? widget.VersionNumber;
        const status = String(widget.status ?? widget.Status ?? widget.widgetStatusCode ?? "").toLowerCase();
        const valid = x => x != null && /^[1-9]\d*$/.test(String(x));
        const liveText = valid(live) ? `Live v${ live }` : "";
        if (valid(draft)) return `Draft v${ draft }${ liveText ? ` · ${ liveText }` : "" }`;
        if (valid(live)) return liveText;
        if (valid(current)) return `${ status === "published" || status === "live" ? "Live" : "Draft" } v${ current }`;
        return "—";
    }
    function renderTable () {
        const query = $("riSearch").value.trim().toLowerCase(), filter = $("riStatusFilter").value;
        const shown = items.filter(x => (!query || [ x.name, x.key, x.description ].some(v => String(v || "").toLowerCase().includes(query))) && (filter === "all" || (filter === "active" ? x.active : !x.active)));
        $("riTotalCount").textContent = String(items.length); $("riActiveCount").textContent = String(items.filter(x => x.active).length);
        if (!shown.length) { tbody.innerHTML = `<tr><td colspan="5" class="ri-empty">${ items.length ? "No recommendations match your search." : "No recommendations yet. Add the first recommendation item." }</td></tr>`; return; }
        tbody.innerHTML = shown.map(x => `<tr><td><span class="ri-item-name">${ esc(x.name) }</span><span class="ri-item-sub">Recommendation outcome</span></td><td><code class="ri-key">${ esc(x.key) }</code></td><td>${ esc(x.description || "—") }</td><td><span class="ri-badge${ x.active ? "" : " inactive" }">${ x.active ? "● Active" : "Inactive" }</span></td><td><div class="ri-row-actions"><button class="ri-button" type="button" data-edit="${ esc(x.id) }">Edit</button><button class="ri-button" type="button" data-toggle="${ esc(x.id) }">${ x.active ? "Deactivate" : "Activate" }</button></div></td></tr>`).join("");
    }
    async function loadItems () {
        const widget = currentWidget();
        items = []; versionNumber = ""; $("riWidgetVersion").textContent = formatVersion(widget);
        $("riAddButton").disabled = !widget;
        const sequence = ++loadSequence;
        $("riUsedCount").textContent = "—"; $("riServedCount").textContent = "—";
        if (!widget) { renderTable(); return; }
        message("Loading recommendation items…");
        const data = await read(await fetch(`${ apiBase }/widgets/${ encodeURIComponent(selectedKey) }/draft/recommendations`, { headers: headers() }));
        if (sequence !== loadSequence) return;
        versionNumber = data.selectedVersionNumber || data.versionNumber || "";
        // The immutable identifier is the stable UI identity. The server
        // returns one canonical item per key even for legacy duplicate rows.
        items = (data.items || []).map((x, i) => ({ id: String(x.key ?? x.Key ?? x.id ?? x.Id ?? `rec-${ i }`), name: x.name ?? x.Name ?? "", key: x.key ?? x.Key ?? "", description: x.description ?? x.Description ?? "", imageUrl: x.imageUrl ?? x.ImageUrl ?? "", url: x.url ?? x.Url ?? "", ctaText: x.ctaText ?? x.CtaText ?? "", active: x.active ?? x.Active ?? true }));
        $("riUsedCount").textContent = String(data.usedInRulesCount ?? 0);
        $("riServedCount").textContent = String(data.recommendationsServedCount ?? 0);
        const live = widget.publishedVersionNumber ?? widget.PublishedVersionNumber;
        $("riWidgetVersion").textContent = formatVersion(widget);
        renderTable();
        message(data.usedLiveFallback
            ? `The draft has no saved recommendation items yet. Showing ${ items.length } item${ items.length === 1 ? "" : "s" } from the live version.`
            : items.length
                ? `Loaded ${ items.length } recommendation item${ items.length === 1 ? "" : "s" } from the ${ data.status || "selected" } version.`
                : "No recommendation items are saved for this widget yet. Add an item and save it to the draft.");
    }
    async function loadWidgets () {
        select.disabled = true;
        select.innerHTML = '<option value="">Loading widgets…</option>';
        try {
            const data = await read(await fetch(`${ apiBase }/widgets`, { headers: headers(), credentials: "same-origin" }));
            widgets = data.widgets ?? data.items ?? [];
            select.innerHTML = '<option value="">-- Select a widget --</option>' +
                widgets.map(w => `<option value="${ esc(w.publicKey ?? w.PublicKey) }">${ esc(w.name ?? w.Name ?? "Untitled widget") }</option>`).join("");
            selectedKey = "";
            select.value = "";
            select.disabled = !widgets.length;
            $("riWidgetVersion").textContent = "—";
            $("riAddButton").disabled = true;
            renderTable();
            message(widgets.length ? "Select a widget to manage its recommendation items." : "No widgets available. Create a widget first.");
        } catch (error) {
            selectedKey = ""; widgets = []; items = [];
            select.innerHTML = '<option value="">Could not load widgets</option>';
            select.disabled = true;
            $("riWidgetVersion").textContent = "—";
            $("riAddButton").disabled = true;
            message(error.message + (error.message.includes("401") ? " Sign in again, then reload this page." : ""), "error");
            renderTable();
        }
    }
    async function saveSnapshot (nextItems) {
        if (!selectedKey) throw new Error("Select a widget first.");
        if (busy) return;
        busy = true; $("riSave").disabled = true;
        try {
            const data = await read(await fetch(`${ apiBase }/widgets/${ encodeURIComponent(selectedKey) }/draft/recommendations`, { method: "PUT", headers: headers(true), credentials: "same-origin", body: JSON.stringify({ items: nextItems.map(x => ({ name: x.name, key: x.key, description: x.description || "", imageUrl: x.imageUrl || null, url: x.url || null, ctaText: x.ctaText || null, active: !!x.active })) }) }));
            versionNumber = data.versionNumber || versionNumber;
            // Re-read canonical IDs and values returned by the DAL after a successful save.
            await loadItems();
            $("riTotalCount").textContent = String(items.length); $("riActiveCount").textContent = String(items.filter(x => x.active).length);
            $("riUsedCount").textContent = String(data.usedInRulesCount ?? $("riUsedCount").textContent ?? 0);
            $("riServedCount").textContent = String(data.recommendationsServedCount ?? $("riServedCount").textContent ?? 0);
            renderTable();
            const widget = currentWidget(); if (widget) { widget.draftVersionNumber = versionNumber; widget.hasDraft = true; }
            $("riWidgetVersion").textContent = formatVersion(widget);
            message(`Draft v${ versionNumber } saved. The live version is unchanged until you publish it.`, "success");
        } finally { busy = false; $("riSave").disabled = false; }
    }
    function openModal (item) {
        editingId = item?.id ?? null; $("riModalTitle").textContent = item ? "Edit Recommendation" : "Add Recommendation";
        $("riName").value = item?.name || ""; $("riKey").value = item?.key || ""; $("riDescription").value = item?.description || "";
        $("riKey").readOnly = !!item;
        $("riKey").title = item ? "The identifier cannot be changed after creation." : "";
        $("riSave").textContent = item ? "Update Recommendation" : "Save Recommendation";
        $("riImageUrl").value = item?.imageUrl || ""; $("riUrl").value = item?.url || ""; $("riCtaText").value = item?.ctaText || ""; $("riActive").checked = item ? !!item.active : true;
        $("riFormError").textContent = ""; modal.hidden = false; $("riName").focus();
    }
    function closeModal () { modal.hidden = true; editingId = null; $("riKey").readOnly = false; }
    $("riAddButton").addEventListener("click", () => { if (!selectedKey) { message("Select a widget first.", "error"); return; } openModal(null); }); $("riClose").addEventListener("click", closeModal); $("riCancel").addEventListener("click", closeModal);
    modal.addEventListener("click", e => { if (e.target === modal) closeModal(); });
    form.addEventListener("submit", async e => {
        e.preventDefault(); $("riFormError").textContent = "";
        const existing = editingId === null ? null : items.find(x => x.id === editingId);
        if (editingId !== null && !existing) { $("riFormError").textContent = "The selected recommendation no longer exists. Reload and try again."; return; }
        const item = {
            id: existing?.id || `new-${ Date.now() }`, name: $("riName").value.trim(),
            key: existing ? existing.key : $("riKey").value.trim(),
            description: $("riDescription").value.trim(), imageUrl: $("riImageUrl").value.trim(),
            url: $("riUrl").value.trim(), ctaText: $("riCtaText").value.trim(), active: $("riActive").checked
        };
        if (items.some(x => x.id !== item.id && x.key.toLowerCase() === item.key.toLowerCase())) {
            $("riFormError").textContent = "This identifier already exists in the selected widget."; return;
        }
        const next = existing ? items.map(x => x.id === existing.id ? item : x) : [ ...items, item ];
        try { await saveSnapshot(next); closeModal(); } catch (error) { $("riFormError").textContent = error.message; }
    });
    tbody.addEventListener("click", async e => {
        const edit = e.target.closest("[data-edit]"), toggle = e.target.closest("[data-toggle]");
        if (edit) openModal(items.find(x => x.id === edit.dataset.edit));
        if (toggle) { const id = toggle.dataset.toggle; const next = items.map(x => x.id === id ? { ...x, active: !x.active } : x); try { await saveSnapshot(next); } catch (error) { message(error.message, "error"); } }
    });
    $("riSearch").addEventListener("input", renderTable); $("riStatusFilter").addEventListener("change", renderTable);
    select.addEventListener("change", async () => {
        selectedKey = select.value;
        if (selectedKey) localStorage.setItem(storageKey, selectedKey);
        else localStorage.removeItem(storageKey);
        try { await loadItems(); } catch (error) { message(error.message, "error"); items = []; renderTable(); }
    });
    root.querySelector("#riCreateWidget")?.addEventListener("click", () => {
        localStorage.setItem("askpearls.widgetBuilderMode", "create");
        localStorage.removeItem(storageKey);
    });
    loadWidgets();
})();
