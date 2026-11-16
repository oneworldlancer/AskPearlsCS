(function () {
    'use strict';
    const root = document.querySelector('.publish-embed-page');
    if (!root || root.dataset.peInitialized === 'true') return;
    root.dataset.peInitialized = 'true';
    const $ = id => root.querySelector('#' + id);
    const endpoint = '/Widget/Api/widgets';
    // Public installation endpoints; never use these for authenticated Portal requests.
    // Replace these development URLs when deploying to production.
    const scriptUrl = 'https://localhost:7087/app.widgets/widget.js';
    const apiOrigin = 'https://localhost:7139';
    const widgetBuilderUrl = '/Dashboard/WidgetBuilder';
    const state = { widgets: [], selected: null, platform: 'html', publishing: false, loading: false, domains: [] };
    const platformInfo = {
        html: [ 'Custom HTML', 'HTML embed snippet', 'Copy your embed code', 'Copy the snippet below and add it to your website.', 'Paste the code into your website', 'Place the snippet before the closing body tag, then save your page.', 'Add the snippet once per website.' ],
        wordpress: [ 'WordPress', 'WordPress embed snippet', 'Copy your widget snippet', 'Use the standard embed snippet on WordPress.', 'Add the snippet to WordPress', 'Use a trusted custom-code plugin or your theme footer.', 'Back up your site before changing theme files.' ],
        shopify: [ 'Shopify', 'Shopify embed snippet', 'Copy your widget snippet', 'Prepare the snippet for your Shopify storefront.', 'Add the snippet to your theme', 'Use the theme code editor or supported custom-code area.', 'Duplicate your theme before editing it.' ],
        wix: [ 'Wix', 'Wix embed snippet', 'Copy your widget snippet', 'Prepare the snippet for your Wix website.', 'Add the code using Wix', 'Use Wix supported custom-code or embed features.', 'Available features depend on your Wix plan.' ]
    };
    const integerVersion = value => { const n = Number.parseInt(String(value ?? '0'), 10); return Number.isFinite(n) && n > 0 ? n : 0; };
    const text = (id, value) => { if ($(id)) $(id).textContent = value; };
    const get = (obj, ...names) => names.map(n => obj?.[ n ]).find(v => v !== undefined && v !== null);
    const keyOf = w => String(get(w, 'publicKey', 'PublicKey', 'widgetPublicKey', 'WidgetPublicKey') || '');
    const nameOf = w => String(get(w, 'name', 'Name', 'widgetName', 'WidgetName') || 'Untitled widget');
    const draftOf = w => integerVersion(get(w, 'draftVersionNumber', 'DraftVersionNumber', 'widgetDraftVersion', 'WidgetDraftVersion'));
    const liveOf = w => integerVersion(get(w, 'publishedVersionNumber', 'PublishedVersionNumber', 'widgetPublishedVersion', 'WidgetPublishedVersion'));
    const hasDraft = w => !!w && (get(w, 'hasDraft', 'HasDraft') === true || draftOf(w) > 0);
    const hasLive = w => !!w && liveOf(w) > 0;
    const csrf = () => document.querySelector('input[name="__RequestVerificationToken"]')?.value || document.querySelector('meta[name="request-verification-token"]')?.content || document.cookie.split('; ').find(x => x.startsWith('XSRF-TOKEN='))?.split('=').slice(1).join('=') || '';
    let toastTimeout;
    function toast (message) { const el = $('peToast'); if (!el) return; el.textContent = message; el.classList.add('is-visible'); clearTimeout(toastTimeout); toastTimeout = setTimeout(() => el.classList.remove('is-visible'), 3200); }
    async function request (url, options = {}) {
        const headers = { Accept: 'application/json', ...(options.headers || {}) };
        if (options.method && options.method.toUpperCase() !== 'GET') { const token = csrf(); if (token) headers.RequestVerificationToken = token; }
        const response = await fetch(url, { credentials: 'same-origin', ...options, headers });
        const body = await response.text(); let result;
        try { result = body ? JSON.parse(body) : {}; } catch { result = {}; }
        if (!response.ok) throw new Error(result.message || `Request failed (HTTP ${ response.status }).`);
        return result;
    }
    function renderStatus () {
        const w = state.selected, draft = hasDraft(w), live = hasLive(w);
        const badge = $('peStatusBadge'), button = $('pePublishBtn');
        if (badge) {
            badge.classList.toggle('is-published', live && !draft);
            badge.classList.toggle('is-draft', draft);
            badge.dataset.status = !w ? 'not-selected' : draft ? 'draft' : live ? 'published' : 'not-published';
            badge.innerHTML = !w ? '<i class="bi bi-circle-fill"></i> Select a widget' : draft ? '<i class="bi bi-circle-fill"></i> Draft ready to publish' : live ? '<i class="bi bi-check-circle-fill"></i> Published' : '<i class="bi bi-circle-fill"></i> Not published';
        }
        text('peVersion', w ? `Draft v${ draftOf(w) } · Live v${ liveOf(w) }` : '—');
        text('peSummaryStatus', !w ? 'No widget selected' : draft ? 'Draft ready' : live ? 'Published' : 'Not published');
        text('peStatusDescription', !w ? 'Select a widget to view its publishing status.' : draft ? (live ? 'Saved draft is ready; the current published version remains live.' : 'Saved draft is ready to publish.') : live ? 'Published version is live.' : 'Save a design draft before publishing.');
        if (button) { button.disabled = !draft || state.publishing || state.loading; button.innerHTML = state.publishing ? '<i class="bi bi-arrow-repeat"></i> Publishing…' : '<i class="bi bi-rocket-takeoff"></i> Publish Widget'; }
        if ($('peCopyCode')) $('peCopyCode').disabled = !w || !live;
    }
    function embedCode () {
        if (!state.selected || !hasLive(state.selected)) return '<!-- Select a published widget to generate an installation snippet. -->';
        const key = keyOf(state.selected);
        return '<!-- AskPearls DecisionOS widget -->\n' + '<script src="' + scriptUrl + '" data-widget="' + key + '" data-api-url="' + apiOrigin + '"><' + '/script>';
    }
    function renderCode () { text('peEmbedCode', embedCode()); }
    function renderPlatform (platform) {
        state.platform = platformInfo[ platform ] ? platform : 'html';
        const p = platformInfo[ state.platform ];
        root.querySelectorAll('[data-pe-platform]').forEach(btn => { const active = btn.dataset.pePlatform === state.platform; btn.classList.toggle('active', active); btn.setAttribute('aria-selected', String(active)); });
        [ 'peSummaryPlatform', 'peCodeLabel', 'peInstructionHeading', 'peInstructionDescription', 'peStepTwoHeading', 'peStepTwoDescription', 'peInstallTip' ].forEach((id, i) => text(id, p[ i ]));
        renderCode();
    }
    function renderSelector () {
        const select = $('peWidgetSelect'); if (!select) return;
        select.replaceChildren(new Option('-- Select a widget --', ''));
        for (const w of state.widgets) { const key = keyOf(w); if (key) select.add(new Option(nameOf(w), key)); }
        select.value = state.selected ? keyOf(state.selected) : '';
    }
    async function loadWidgets (preferredKey = '') {
        state.loading = true; renderStatus();
        try {
            const data = await request(endpoint);
            if (data.success === false) throw new Error(data.message || 'Unable to load widgets.');
            state.widgets = Array.isArray(data.widgets) ? data.widgets : Array.isArray(data.data?.widgets) ? data.data.widgets : [];
            state.selected = preferredKey ? state.widgets.find(w => keyOf(w) === preferredKey) || null : null;
            renderSelector(); renderStatus(); renderCode();
        } catch (e) { state.widgets = []; state.selected = null; renderSelector(); toast(e.message || 'Unable to load widgets.'); }
        finally { state.loading = false; renderStatus(); renderCode(); }
    }
    async function publish () {
        const selected = state.selected;
        if (!selected || !hasDraft(selected) || state.publishing) return;
        const key = keyOf(selected);
        if (!window.confirm(`Publish the saved draft for "${ nameOf(selected) }"? This will replace its live version.`)) return;
        state.publishing = true; renderStatus();
        try {
            const result = await request(`${ endpoint }/${ encodeURIComponent(key) }/publish`, { method: 'POST' });
            if (result.success === false) throw new Error(result.message || 'Publish failed.');
            toast('Widget published successfully.');
            await loadWidgets(key);
        } catch (e) { toast(e.message || 'Could not publish widget.'); }
        finally { state.publishing = false; renderStatus(); }
    }
    async function copy () {
        if (!state.selected || !hasLive(state.selected)) return;
        try { await navigator.clipboard.writeText(embedCode()); toast('Embed snippet copied.'); }
        catch { toast('Clipboard access failed. Select the snippet and copy it manually.'); }
    }
    function normalizeDomain (value) {
        let s = String(value || '').trim().toLowerCase();
        if (!s) return null;
        try { if (/^https?:\/\//.test(s)) { const u = new URL(s); if (u.username || u.password) return null; s = u.hostname; } } catch { return null; }
        s = s.replace(/\.$/, '');
        return s.length <= 253 && s.split('.').length >= 2 && s.split('.').every(x => /^[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?$/.test(x)) ? s : null;
    }
    function renderDomains () {
        const list = $('peDomainList'); if (!list) return;
        list.replaceChildren();
        for (const domain of state.domains) {
            const row = document.createElement('div'); row.className = 'pe-domain-row';
            const name = document.createElement('strong'); name.textContent = domain;
            const remove = document.createElement('button'); remove.type = 'button'; remove.className = 'pe-domain-remove'; remove.setAttribute('aria-label', 'Remove ' + domain); remove.innerHTML = '<i class="bi bi-trash3"></i>';
            remove.addEventListener('click', () => { state.domains = state.domains.filter(x => x !== domain); renderDomains(); toast('Domain removed from demo allowlist.'); });
            row.append(name, remove); list.append(row);
        }
        text('peDomainCount', `${ state.domains.length } domain${ state.domains.length === 1 ? '' : 's' }`);
        text('peDomainListCount', String(state.domains.length));
        if ($('peDomainEmpty')) $('peDomainEmpty').hidden = state.domains.length !== 0;
        list.hidden = state.domains.length === 0;
    }
    const checkboxes = [ ...root.querySelectorAll('.pe-checklist-checkbox') ];
    function updateChecklist () {
        const done = checkboxes.filter(x => x.checked).length;
        text('peChecklistStatus', `${ done } of ${ checkboxes.length } steps completed`);
        const bar = $('peChecklistProgressBar'); if (bar) { bar.style.width = `${ checkboxes.length ? done * 100 / checkboxes.length : 0 }%`; bar.setAttribute('aria-valuenow', String(done)); bar.setAttribute('aria-valuemax', String(checkboxes.length)); }
        if ($('peChecklistComplete')) $('peChecklistComplete').hidden = done !== checkboxes.length;
    }
    root.querySelectorAll('[data-pe-platform]').forEach(btn => btn.addEventListener('click', () => renderPlatform(btn.dataset.pePlatform)));
    $('peWidgetSelect')?.addEventListener('change', e => {
        state.selected = state.widgets.find(w => keyOf(w) === e.target.value) || null;
        renderStatus(); renderCode(); checkboxes.forEach(x => x.checked = false); updateChecklist();
    });
    $('pePublishBtn')?.addEventListener('click', publish);
    $('peCopyCode')?.addEventListener('click', copy);
    $('peNewWidgetBtn')?.addEventListener('click', () => { window.location.href = widgetBuilderUrl; });
    $('peDomainForm')?.addEventListener('submit', e => {
        e.preventDefault(); const input = $('peDomainInput'); const domain = normalizeDomain(input.value);
        text('peDomainError', !domain ? 'Enter a valid domain, such as example.com.' : state.domains.includes(domain) ? 'This domain is already listed.' : '');
        if (!domain || state.domains.includes(domain)) return;
        state.domains.push(domain); input.value = ''; renderDomains(); toast('Domain added to demo allowlist (not enforced).');
    });
    checkboxes.forEach(x => x.addEventListener('change', updateChecklist));
    $('peChecklistNext')?.addEventListener('click', () => { $('pePublishBtn')?.scrollIntoView({ behavior: 'smooth', block: 'center' }); $('pePublishBtn')?.focus({ preventScroll: true }); });
    renderPlatform('html'); renderDomains(); updateChecklist(); renderStatus(); loadWidgets();
})();
