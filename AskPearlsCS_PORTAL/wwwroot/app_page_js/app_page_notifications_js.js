(function () {
    "use strict";
    if (window.AskPearlsNotifications) return;
    const base = "/Account/Api/notifications";
    const $ = id => document.getElementById(id);
    const esc = s => String(s ?? "").replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#39;" }[ c ]));
    const date = s => { if (!s) return ""; const d = new Date(s); return Number.isNaN(d.getTime()) ? "" : d.toLocaleString(); };
    const icon = t => ({ security: "bi-shield-check", widget: "bi-grid", activity: "bi-graph-up", product: "bi-stars", billing: "bi-credit-card", account: "bi-person" })[ String(t || "").toLowerCase() ] || "bi-bell";
    const safeUrl = s => { if (!s || s === "0") return ""; try { const u = new URL(s, location.origin); return u.origin === location.origin && [ "http:", "https:" ].includes(u.protocol) ? u.pathname + u.search + u.hash : ""; } catch { return ""; } };
    function token () { return document.querySelector('.ap-notifications-page input[name="__RequestVerificationToken"]')?.value || document.querySelector('input[name="__RequestVerificationToken"]')?.value || ""; }
    async function request (path, method = "GET") {
        const headers = { Accept: "application/json" }; if (method !== "GET") { const t = token(); if (!t) throw Error("Security token unavailable. Refresh the page."); headers.RequestVerificationToken = t; }
        const r = await fetch(base + path, { method, headers, credentials: "same-origin" }); if (r.status === 204) return null; const b = await r.json().catch(() => ({})); if (!r.ok) throw Error(b.message || b.title || "Notification request failed (" + r.status + ")."); return b;
    }
    let filter = "all", page = 0, cache = [], count = 0, total = 0, loading = false, epoch = 0;
    const size = 20;
    function showError (msg) { const e = $("apNotifError"); if (e) { e.hidden = !msg; e.textContent = msg || ""; } }
    function listRow (n) {
        const id = Number(n.id);
        if (!Number.isSafeInteger(id) || id <= 0) return "";
        return `<article class="ap-n-item ${ n.isRead ? "" : "unread" }"><div class="ap-n-icon"><i class="bi ${ icon(n.type) }"></i></div><div class="ap-n-body"><div class="ap-n-title">${ esc(n.title || "Notification") }</div><div class="ap-n-message">${ esc(n.message || "") }</div><div class="ap-n-meta">${ esc(n.type || "General") } · ${ esc(date(n.createdAtUtc)) }</div><div class="ap-n-actions"><button type="button" class="ap-n-action" data-n-view="${ id }">View / Read message</button>${ !n.isRead ? `<button type="button" class="ap-n-action" data-n-read="${ id }">Mark as read</button>` : "" }</div></div></article>`;
    }
    async function viewNotification (id) {
        const n = cache.find(x => Number(x.id) === Number(id));
        if (!n) { showError("Notification not found. Refresh the list."); return; }
        const el = $("apNotificationReader");
        if (!el || !window.bootstrap?.Modal) { showError("Message viewer is unavailable. Ensure Bootstrap JavaScript is loaded."); return; }
        $("apNotificationReaderTitle").dataset.notificationTitle = n.title || "Notification";
        $("apNotificationReaderTitle").textContent = n.title || "Notification";
        $("apNotificationReaderMeta").textContent = `${ n.type || "General" } · ${ date(n.createdAtUtc) }`;
        $("apNotificationReaderMessage").textContent = n.message || n.text || "No additional message content.";
        const subject = $("apNotificationSupportSubject");
        if (subject) subject.value = "Regarding: " + (n.title || "Notification");
        showReaderView("read");
        const related = $("apNotificationReaderAction");
        const url = safeUrl(n.actionUrl);
        related.hidden = !url;
        if (url) related.href = url;
        else related.removeAttribute("href");
        bootstrap.Modal.getOrCreateInstance(el).show();
        if (!n.isRead) {
            try {
                await request(`/${ Number(n.id) }/read`, "PUT");
                n.isRead = true;
                await Promise.all([ loadPage(), refreshBell() ]);
            } catch (e) { showError(e.message); }
        }
    }

    function showReaderView (view) {
        const read = $("apNotificationReadPanel"), support = $("apNotificationSupportPanel");
        if (!read || !support) return;
        const contact = view === "support";
        read.hidden = contact;
        support.hidden = !contact;
        $("apNotificationReaderTitle").textContent = contact ? "Contact AskPearls Support" : ($("apNotificationReaderTitle").dataset.notificationTitle || "Notification");
        $("apNotificationReadFooter").hidden = contact;
        $("apNotificationSupportFooter").hidden = !contact;
        if (contact) $("apNotificationSupportSubject")?.focus();
    }
    function initReader () {
        const modal = $("apNotificationReader");
        if (!modal || modal.dataset.readerInitialized) return;
        modal.dataset.readerInitialized = "1";
        modal.addEventListener("click", e => {
            if (e.target.closest("[data-n-contact]")) { e.preventDefault(); showReaderView("support"); }
            if (e.target.closest("[data-n-back]")) { e.preventDefault(); showReaderView("read"); }
        });
        modal.querySelector("#apNotificationSupportForm")?.addEventListener("submit", e => {
            e.preventDefault();
            const status = $("apNotificationSupportStatus");
            if (status) status.textContent = "Email delivery is not configured yet. Your message has not been sent.";
        });
        modal.addEventListener("hidden.bs.modal", () => {
            const status = $("apNotificationSupportStatus");
            if (status) status.textContent = "";
        });
    }

    async function loadPage () {
        const root = document.querySelector('.ap-notifications-page'); if (!root) return; const seq = ++epoch; loading = true; showError(""); $("apNotifList").innerHTML = '<div class="ap-n-empty">Loading notifications…</div>'; try {
            // Existing API uses skip/take, with no server-side read filter. Fetch in batches to correctly filter the visible history.
            let all = [], offset = 0, known = Infinity; const batch = 100;
            while (offset < known) { const d = await request(`?skip=${ offset }&take=${ batch }`); if (seq !== epoch) return; const items = Array.isArray(d.items) ? d.items : []; known = Math.max(0, Number(d.totalCount) || 0); all.push(...items); offset += items.length; if (!items.length || items.length < batch) break; }
            if (seq !== epoch) return; cache = all; count = Number((await request("/unread-count")).count) || 0; const shown = all.filter(x => filter === "all" || (filter === "read" ? x.isRead : !x.isRead)); total = shown.length; if (page * size >= total && page > 0) page = Math.max(0, Math.ceil(total / size) - 1); const slice = shown.slice(page * size, (page + 1) * size);
            $("apNotifList").innerHTML = slice.length ? slice.map(listRow).join("") : '<div class="ap-n-empty"><i class="bi bi-bell-slash"></i><p>No notifications in this view.</p></div>';
            $("apNotifSummary").textContent = `${ count } unread · ${ total } ${ filter === "all" ? "total" : "matching" }`;
            $("apNotifPageLabel").textContent = `Page ${ page + 1 } of ${ Math.max(1, Math.ceil(total / size)) }`;
            $("apNotifPrev").disabled = page === 0; $("apNotifNext").disabled = (page + 1) * size >= total;
            $("apNotifMarkAll").disabled = count === 0;
        } catch (e) { if (seq === epoch) { showError(e.message); $("apNotifList").innerHTML = '<div class="ap-n-empty">Could not load notifications.</div>'; } } finally { if (seq === epoch) loading = false; }
    }
    async function markRead (id) { await request(`/${ id }/read`, "PUT"); await Promise.all([ loadPage(), refreshBell() ]); }
    async function markAll () { await request("/read-all", "PUT"); await Promise.all([ loadPage(), refreshBell() ]); }
    function initPage () {
        const root = document.querySelector('.ap-notifications-page'); if (!root || root.dataset.notificationsInitialized) return; root.dataset.notificationsInitialized = "1"; filter = "all"; page = 0;
        root.querySelectorAll('[data-notification-filter]').forEach(b => b.addEventListener('click', () => { filter = b.dataset.notificationFilter; page = 0; root.querySelectorAll('[data-notification-filter]').forEach(x => { const active = x === b; x.classList.toggle('active', active); x.setAttribute('aria-pressed', String(active)); }); loadPage(); }));
        $("apNotifPrev")?.addEventListener("click", () => { if (page > 0) { page--; loadPage(); } }); $("apNotifNext")?.addEventListener("click", () => { page++; loadPage(); }); $("apNotifMarkAll")?.addEventListener("click", async () => { try { await markAll(); } catch (e) { showError(e.message); } });
        $("apNotifList")?.addEventListener("click", async e => { const b = e.target.closest('[data-n-read]'); if (!b) return; b.disabled = true; try { await markRead(b.dataset.nRead); } catch (err) { showError(err.message); b.disabled = false; } });
        $("apNotifList")?.addEventListener("click", e => { const b = e.target.closest('[data-n-view]'); if (b) { e.preventDefault(); viewNotification(b.dataset.nView); } });
        $("apNotificationReaderAction")?.addEventListener("click", () => { const el = $("apNotificationReader"); if (el && window.bootstrap?.Modal) bootstrap.Modal.getInstance(el)?.hide(); });
        initReader();
        loadPage();
    }
    async function refreshBell () { const bell = document.querySelector('.notification-dropdown'); if (!bell) return; try { const data = await request('/unread-count'); const n = Math.max(0, Number(data.count) || 0); const badge = bell.querySelector('.header-badge'); if (badge) { badge.hidden = n === 0; badge.textContent = n > 99 ? '99+' : String(n); } const status = bell.querySelector('.notification-header span'); if (status) status.textContent = `${ n } unread`; const mobile = document.querySelector('.mobile-header-menu [data-ap-notifications-count]'); if (mobile) { mobile.hidden = n === 0; mobile.textContent = n > 99 ? '99+' : String(n); } } catch (e) { console.warn('Notification badge unavailable', e); } }
    async function refreshDropdown () { const bell = document.querySelector('.notification-dropdown'); if (!bell) return; const list = bell.querySelector('.notification-list'); if (!list) return; list.innerHTML = '<div class="ap-n-empty">Loading…</div>'; try { const d = await request('?skip=0&take=6'); const items = Array.isArray(d.items) ? d.items : []; list.innerHTML = items.length ? items.map(n => `<button type="button" class="ap-notification-menu-item ${ n.isRead ? '' : 'unread' }" data-bell-read="${ Number(n.id) }"><strong>${ esc(n.title || 'Notification') }</strong><small>${ esc(n.message || '') }</small><time>${ esc(date(n.createdAtUtc)) }</time></button>`).join('') : '<div class="ap-n-empty">No notifications yet.</div>'; } catch (e) { list.innerHTML = '<div class="ap-n-empty">Notifications unavailable.</div>'; } }
    function openSection () { if (typeof window.loadPortalSection === 'function') window.loadPortalSection('notifications'); else { const link = document.querySelector('#askPearlsSidebar [data-page="notifications"]'); if (link) link.click(); else location.href = '/Dashboard'; } }
    function initBell () { const bell = document.querySelector('.notification-dropdown'); if (!bell || bell.dataset.notificationsInitialized) return; bell.dataset.notificationsInitialized = '1'; const btn = bell.querySelector('button[data-bs-toggle="dropdown"]'); btn?.addEventListener('click', () => { refreshDropdown(); refreshBell(); }); bell.querySelector('.notification-list')?.addEventListener('click', async e => { const b = e.target.closest('[data-bell-read]'); if (!b) return; try { await request(`/${ b.dataset.bellRead }/read`, 'PUT'); await refreshBell(); openSection(); } catch (err) { console.error(err); } }); bell.querySelector('[data-notification-action="mark-all-read"]')?.addEventListener('click', async e => { e.preventDefault(); try { await request('/read-all', 'PUT'); await refreshBell(); await refreshDropdown(); if (document.querySelector('.ap-notifications-page')) loadPage(); } catch (err) { console.error(err); } }); bell.querySelector('.notification-footer a')?.addEventListener('click', e => { e.preventDefault(); openSection(); }); refreshBell(); setInterval(() => { if (!document.hidden) refreshBell(); }, 60000); }
    function boot () { initBell(); initPage(); const target = $('apMainContent'); if (target) { new MutationObserver(() => initPage()).observe(target, { childList: true, subtree: false }); } document.querySelectorAll('.mobile-header-menu a[data-ap-notifications-link]').forEach(a => a.addEventListener('click', e => { e.preventDefault(); openSection(); })); }
    window.AskPearlsNotifications = { refreshBell, refreshDropdown, loadPage }; if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', boot); else boot();
})();
