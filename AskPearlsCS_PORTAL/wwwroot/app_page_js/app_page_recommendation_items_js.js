(function () {
    "use strict";
    const root = document.querySelector('.ri-page[data-ap-page="recommendation-items"]');
    if (!root || root.dataset.initialized === "true") return;
    root.dataset.initialized = "true";
    const $ = id => root.querySelector("#" + id);
    const apiBase = (root.dataset.apiBase || window.AskPearlsCS_API_URL || (location.origin + "/api")).replace(/\/+$/, "");
    const ownerToken = (root.dataset.demoOwnerUserToken || "").trim();
    const storageKey = "askpearls.activeWidgetPublicKey";
    const select = $("riWidgetSelect"), tbody = $("riTableBody"), modal = $("riModal"), form = $("riForm");
    let widgets = [], items = [], versionNumber = "", selectedKey = localStorage.getItem(storageKey) || "", editingId = null, busy = false;
    const headers = () => ({ Accept: "application/json", "Content-Type": "application/json", ...(ownerToken ? { "X-Demo-Owner-User-Token": ownerToken } : {}) });
    const esc = value => String(value ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[ c ]));
    async function read (response) { const text = await response.text(); let data = {}; try { data = text ? JSON.parse(text) : {}; } catch { throw new Error("The API returned an unreadable response."); } if (!response.ok || data.success === false) throw new Error(data.message || `API request failed (HTTP ${ response.status }).`); return data; }
    function message (text, kind) { const node = $("riMessage"); node.textContent = text || ""; node.hidden = !text; node.className = "ri-message" + (kind ? " is-" + kind : ""); }
    function currentWidget () { return widgets.find(x => String(x.publicKey ?? x.PublicKey) === selectedKey); }
    function formatVersion (widget) { if (!widget) return "—"; const draft = widget.draftVersionNumber ?? widget.DraftVersionNumber; const live = widget.publishedVersionNumber ?? widget.PublishedVersionNumber; const current = widget.versionNumber ?? widget.VersionNumber; return draft ? `Draft v${ draft }${ live ? ` · Live v${ live }` : "" }` : live || current ? `Live v${ live || current }` : "—"; }
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
        $("riUsedCount").textContent = "—"; $("riServedCount").textContent = "—";
        if (!widget) { renderTable(); return; }
        message("Loading recommendation items…");
        const data = await read(await fetch(`${ apiBase }/widgets/${ encodeURIComponent(selectedKey) }/draft/recommendations`, { headers: headers() }));
        versionNumber = data.selectedVersionNumber || data.versionNumber || "";
        items = (data.items || []).map((x, i) => ({ id: String(x.id ?? x.Id ?? `rec-${ i }`), name: x.name ?? x.Name ?? "", key: x.key ?? x.Key ?? "", description: x.description ?? x.Description ?? "", imageUrl: x.imageUrl ?? x.ImageUrl ?? "", url: x.url ?? x.Url ?? "", ctaText: x.ctaText ?? x.CtaText ?? "", active: x.active ?? x.Active ?? true }));
        $("riUsedCount").textContent = String(data.usedInRulesCount ?? 0);
        $("riServedCount").textContent = String(data.recommendationsServedCount ?? 0);
        const live = widget.publishedVersionNumber ?? widget.PublishedVersionNumber;
        $("riWidgetVersion").textContent = widget.draftVersionNumber || widget.DraftVersionNumber ? `Draft v${ versionNumber || widget.draftVersionNumber || widget.DraftVersionNumber }${ live ? ` · Live v${ live }` : "" }` : formatVersion(widget);
        renderTable();
        message(data.usedLiveFallback
            ? `The draft has no saved recommendation items yet. Showing ${ items.length } item${ items.length === 1 ? "" : "s" } from the live version.`
            : items.length
                ? `Loaded ${ items.length } recommendation item${ items.length === 1 ? "" : "s" } from the ${ data.status || "selected" } version.`
                : "No recommendation items are saved for this widget yet. Add an item and save it to the draft.");
    }
    async function loadWidgets () {
        try {
            const data = await read(await fetch(`${ apiBase }/widgets`, { headers: headers() }));
            widgets = data.widgets || [];
            select.innerHTML = widgets.length ? widgets.map(w => `<option value="${ esc(w.publicKey ?? w.PublicKey) }">${ esc(w.name ?? w.Name ?? "Untitled widget") }</option>`).join("") : '<option value="">No widgets available</option>';
            if (!widgets.some(w => String(w.publicKey ?? w.PublicKey) === selectedKey)) selectedKey = String(widgets[ 0 ]?.publicKey ?? widgets[ 0 ]?.PublicKey ?? "");
            select.value = selectedKey; select.disabled = !selectedKey;
            if (selectedKey) localStorage.setItem(storageKey, selectedKey);
            await loadItems();
        } catch (error) { select.innerHTML = '<option value="">Could not load widgets</option>'; message(error.message, "error"); renderTable(); }
    }
    async function saveSnapshot (nextItems) {
        if (!selectedKey) throw new Error("Select a widget first.");
        if (busy) return;
        busy = true; $("riSave").disabled = true;
        try {
            const data = await read(await fetch(`${ apiBase }/widgets/${ encodeURIComponent(selectedKey) }/draft/recommendations`, { method: "PUT", headers: headers(), body: JSON.stringify({ items: nextItems.map(x => ({ name: x.name, key: x.key, description: x.description || "", imageUrl: x.imageUrl || null, url: x.url || null, ctaText: x.ctaText || null, active: !!x.active })) }) }));
            items = nextItems; versionNumber = data.versionNumber || versionNumber;
            $("riTotalCount").textContent = String(items.length); $("riActiveCount").textContent = String(items.filter(x => x.active).length);
            $("riUsedCount").textContent = String(data.usedInRulesCount ?? $("riUsedCount").textContent ?? 0);
            $("riServedCount").textContent = String(data.recommendationsServedCount ?? $("riServedCount").textContent ?? 0);
            renderTable();
            const widget = currentWidget(); if (widget) { widget.draftVersionNumber = versionNumber; widget.hasDraft = true; }
            $("riWidgetVersion").textContent = `Draft v${ versionNumber }${ widget?.publishedVersionNumber ? ` · Live v${ widget.publishedVersionNumber }` : "" }`;
            message(`Draft v${ versionNumber } saved. The live version is unchanged until you publish it.`, "success");
        } finally { busy = false; $("riSave").disabled = false; }
    }
    function openModal (item) {
        editingId = item?.id ?? null; $("riModalTitle").textContent = item ? "Edit Recommendation" : "Add Recommendation";
        $("riName").value = item?.name || ""; $("riKey").value = item?.key || ""; $("riDescription").value = item?.description || "";
        $("riImageUrl").value = item?.imageUrl || ""; $("riUrl").value = item?.url || ""; $("riCtaText").value = item?.ctaText || ""; $("riActive").checked = item ? !!item.active : true;
        $("riFormError").textContent = ""; modal.hidden = false; $("riName").focus();
    }
    function closeModal () { modal.hidden = true; editingId = null; }
    $("riAddButton").addEventListener("click", () => openModal(null)); $("riClose").addEventListener("click", closeModal); $("riCancel").addEventListener("click", closeModal);
    modal.addEventListener("click", e => { if (e.target === modal) closeModal(); });
    form.addEventListener("submit", async e => {
        e.preventDefault(); $("riFormError").textContent = "";
        const item = { id: editingId || `new-${ Date.now() }`, name: $("riName").value.trim(), key: $("riKey").value.trim(), description: $("riDescription").value.trim(), imageUrl: $("riImageUrl").value.trim(), url: $("riUrl").value.trim(), ctaText: $("riCtaText").value.trim(), active: $("riActive").checked };
        const next = editingId ? items.map(x => x.id === editingId ? item : x) : [ ...items, item ];
        try { await saveSnapshot(next); closeModal(); } catch (error) { $("riFormError").textContent = error.message; }
    });
    tbody.addEventListener("click", async e => {
        const edit = e.target.closest("[data-edit]"), toggle = e.target.closest("[data-toggle]");
        if (edit) openModal(items.find(x => x.id === edit.dataset.edit));
        if (toggle) { const id = toggle.dataset.toggle; const next = items.map(x => x.id === id ? { ...x, active: !x.active } : x); try { await saveSnapshot(next); } catch (error) { message(error.message, "error"); } }
    });
    $("riSearch").addEventListener("input", renderTable); $("riStatusFilter").addEventListener("change", renderTable);
    select.addEventListener("change", async () => { selectedKey = select.value; localStorage.setItem(storageKey, selectedKey); try { await loadItems(); } catch (error) { message(error.message, "error"); items = []; renderTable(); } });
    loadWidgets();
})();
