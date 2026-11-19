(function () {
    'use strict';
    const root = document.querySelector('.publish-embed-page');
    if (!root || root.dataset.peInitialized === 'true') return;
    root.dataset.peInitialized = 'true';
    const $ = id => root.querySelector('#' + id);
    const checkboxes = Array.from(root.querySelectorAll('.pe-checklist-checkbox'));
    const isActive = () => root.isConnected;
    const endpoint = '/Widget/Api/widgets';
    // Public installation endpoints; never use these for authenticated Portal requests.
    // Replace these development URLs when deploying to production.
    const scriptUrl = root.dataset.peScriptUrl || 'https://cdn.askpearls.com/app.widgets/widget.js';
    const apiOrigin = root.dataset.peApiOrigin || 'https://api.askpearls.com';
    const widgetBuilderUrl = '/Dashboard/WidgetBuilder';
    const state = { widgets: [], selected: null, platform: 'html', publishing: false, loading: false, domains: [], domainBusy: false, quota: null };
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
        if (options.method && options.method.toUpperCase() !== 'GET') { const token = root.querySelector('input[name="__RequestVerificationToken"]')?.value || csrf(); if (token) headers.RequestVerificationToken = token; }
        const response = await fetch(url, { credentials: 'same-origin', ...options, headers });
        if (response.redirected && /login|signin|account\/login/i.test(response.url)) throw new Error('Session expired. Please sign in again.');
        const body = await response.text(); let result;
        try { result = body ? JSON.parse(body) : {}; } catch { result = {}; }
        if (!response.ok) throw new Error(result.message || `Request failed (HTTP ${ response.status }).`);
        if (body && !body.trimStart().startsWith('{') && !body.trimStart().startsWith('[')) throw new Error('Expected JSON from ' + url + '. Check authentication and endpoint routing.');
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

        AskPearlsWidgetVersion.update("peVersion", w);

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
            const payload = data.data ?? data;
            state.widgets = Array.isArray(payload) ? payload : Array.isArray(payload.widgets) ? payload.widgets : Array.isArray(payload.items) ? payload.items : Array.isArray(payload.jsonlist) ? payload.jsonlist : [];
            if (!isActive()) return;
            state.selected = preferredKey ? state.widgets.find(w => keyOf(w) === preferredKey) || null : null;
            renderSelector(); renderStatus(); renderCode(); await loadDomains();
        } catch (e) { console.error('[AskPearls Publish] Widget load failed:', e); if (!isActive()) return; state.widgets = []; state.selected = null; state.domains = []; renderDomains(); renderSelector(); toast(e.message || 'Unable to load widgets.'); }
        finally { if (isActive()) { state.loading = false; renderStatus(); renderCode(); } }
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
        try {
            if (/^https?:\/\//i.test(s)) {
                const url = new URL(s);
                if (url.username || url.password || url.pathname !== '/' || url.search || url.hash || url.port) return null;
                s = url.hostname;
            }
        } catch { return null; }
        s = s.replace(/\.$/, '');
        return s.length <= 253 && s.includes('.') && s.split('.').every(part =>
            /^[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?$/.test(part)) ? s : null;
    }
    function domainUrl () {
        return state.selected ? endpoint + '/' + encodeURIComponent(keyOf(state.selected)) + '/domains' : null;
    }
    async function loadDomains () {
        const url = domainUrl();
        if (!url) { state.domains = []; dnsPanels.clear(); renderDomains(); return; }
        const selectedKey = keyOf(state.selected);
        try {
            const data = await request(url);
            if (keyOf(state.selected) !== selectedKey) return;
            state.domains = Array.isArray(data.domains) ? data.domains : [];
            state.quota = data.quota || null;
        } catch (error) {
            if (keyOf(state.selected) === selectedKey) toast(error.message || 'Failed to load domains.');
        }
        renderDomains();
    }
    async function saveDomain (domain) {
        const url = domainUrl();
        if (!url || state.domainBusy) return;
        state.domainBusy = true;
        try {
            await request(url, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ domain }) });
            await loadDomains();
            toast('Domain saved.');
        } catch (error) { text('peDomainError', error.message || 'Failed to save domain.'); }
        finally { state.domainBusy = false; }
    }
    async function deleteDomain (domain) {
        const url = domainUrl();
        if (!url || state.domainBusy) return;
        state.domainBusy = true;
        try {
            await request(url + '/' + encodeURIComponent(domain), { method: 'DELETE' });
            dnsPanels.delete(domain);
            await loadDomains();
            toast('Domain removed.');
        } catch (error) { toast(error.message || 'Failed to remove domain.'); }
        finally { state.domainBusy = false; }
    }
    const dnsPanels = new Map();
    function dnsPanel (domain, dns, expires) {
        const panel = document.createElement('section');
        panel.className = 'pe-dns-panel';
        panel.setAttribute('aria-label', 'DNS verification for ' + domain);
        const heading = document.createElement('strong'); heading.textContent = 'DNS TXT record for ' + domain;
        panel.append(heading);
        const fields = [ [ 'Type', dns.type || 'TXT' ], [ 'Host / Name', dns.host || '' ], [ 'Value / Content', dns.value || '' ], [ 'Expires (UTC)', expires || 'Not provided' ] ];
        for (const [ label, value ] of fields) {
            const line = document.createElement('div'); line.className = 'pe-dns-field';
            const caption = document.createElement('span'); caption.textContent = label;
            const code = document.createElement('code'); code.textContent = String(value);
            line.append(caption, code);
            if (label === 'Host / Name' || label === 'Value / Content') {
                const copy = document.createElement('button'); copy.type = 'button'; copy.className = 'pe-domain-action'; copy.textContent = 'Copy';
                copy.addEventListener('click', async () => {
                    try { await navigator.clipboard.writeText(String(value)); toast(label + ' copied.'); }
                    catch { toast('Clipboard unavailable. Select the value to copy.'); }
                });
                line.append(copy);
            }
            panel.append(line);
        }
        const help = document.createElement('small');
        help.textContent = 'Add this TXT record at your DNS provider. Some providers append the domain automatically; use _askpearls as the Host in that case. Then click Check DNS.';
        panel.append(help);
        return panel;
    }
    async function domainAction (domain, action, button) {
        const url = domainUrl(); if (!url || state.domainBusy) return;
        if (action === 'verification' && dnsPanels.has(domain)) {
            const info = dnsPanels.get(domain);
            info.open = !info.open;
            const panel = document.querySelector('[data-pe-dns-panel-for="' + CSS.escape(domain) + '"]');
            if (panel) { panel.hidden = !info.open; button?.setAttribute('aria-expanded', String(info.open)); }
            return;
        }
        state.domainBusy = true;
        if (button) { button.disabled = true; button.dataset.originalText = button.textContent; button.textContent = action === 'verify' ? 'Checking…' : 'Loading…'; }
        try {
            const result = await request(url + '/' + encodeURIComponent(domain) + '/' + action, { method: 'POST' });
            if (action === 'verification') {
                if (!result.dns) throw new Error('The server did not return a DNS TXT record.');
                dnsPanels.set(domain, { dns: result.dns, expires: result.expiresAtUtc, open: true });
                renderDomains();
            } else {
                await loadDomains();
                toast(result.message || 'DNS verification checked.');
            }
        } catch (error) { toast(error.message || 'Verification request failed.'); }
        finally { state.domainBusy = false; if (button?.isConnected) { button.disabled = false; button.textContent = button.dataset.originalText; } }
    }
    function renderDomains () {
        const list = $('peDomainList'); if (!list) return;
        list.replaceChildren();
        for (const entry of state.domains) {
            const domain = typeof entry === 'string' ? entry : entry.domain;
            if (!domain) continue;
            const verified = entry.verificationStatus === 'verified';
            const row = document.createElement('div'); row.className = 'pe-domain-row';
            const name = document.createElement('strong'); name.textContent = domain;
            const status = document.createElement('span'); status.textContent = verified ? 'Verified' : 'Pending verification';
            status.className = 'pe-domain-verification-status';
            const controls = document.createElement('div'); controls.className = 'pe-domain-actions';
            if (!verified) {
                for (const [ label, action ] of [ [ 'Get DNS record', 'verification' ], [ 'Check DNS', 'verify' ] ]) {
                    const button = document.createElement('button'); button.type = 'button'; button.textContent = label;
                    button.className = 'pe-domain-action'; button.addEventListener('click', () => domainAction(domain, action, button));
                    if (action === 'verification') button.setAttribute('aria-expanded', String(dnsPanels.get(domain)?.open || false));
                    controls.append(button);
                }
            }
            const remove = document.createElement('button'); remove.type = 'button';
            remove.className = 'pe-domain-remove'; remove.setAttribute('aria-label', 'Remove ' + domain);
            remove.textContent = 'Remove'; remove.addEventListener('click', () => deleteDomain(domain));
            controls.append(remove); row.append(name, status, controls); list.append(row);
            if (verified) dnsPanels.delete(domain);
            const info = dnsPanels.get(domain);
            if (info && !verified) {
                const panel = dnsPanel(domain, info.dns, info.expires);
                panel.dataset.peDnsPanelFor = domain;
                panel.hidden = !info.open;
                list.append(panel);
            }
        }
        const count = state.domains.length;
        text('peDomainCount', count + ' domain' + (count === 1 ? '' : 's'));
        text('peDomainListCount', state.quota ? `${ state.quota.used } / ${ state.quota.limit ?? 'Unlimited' } account domains` : String(count));
        if ($('peDomainEmpty')) $('peDomainEmpty').hidden = count !== 0;
        list.hidden = count === 0;
        const add = root.querySelector('.pe-add-domain');
        if (add) add.disabled = !state.selected || state.domainBusy || (state.quota && state.quota.remaining === 0);
    }
    function updateChecklist () {
        const done = checkboxes.filter(x => x.checked).length;
        text('peChecklistStatus', `${ done } of ${ checkboxes.length } steps completed`);
        const bar = $('peChecklistProgressBar'); if (bar) { bar.style.width = `${ checkboxes.length ? done * 100 / checkboxes.length : 0 }%`; bar.setAttribute('aria-valuenow', String(done)); bar.setAttribute('aria-valuemax', String(checkboxes.length)); }
        if ($('peChecklistComplete')) $('peChecklistComplete').hidden = done !== checkboxes.length;
    }
    root.querySelectorAll('[data-pe-platform]').forEach(btn => btn.addEventListener('click', () => renderPlatform(btn.dataset.pePlatform)));
    $('peWidgetSelect')?.addEventListener('change', e => {
        state.selected = state.widgets.find(w => keyOf(w) === e.target.value) || null;
        dnsPanels.clear(); renderStatus(); renderCode(); loadDomains(); checkboxes.forEach(x => x.checked = false); updateChecklist();
    });
    $('pePublishBtn')?.addEventListener('click', publish);
    $('peCopyCode')?.addEventListener('click', copy);
    $('peNewWidgetBtn')?.addEventListener('click', () => { window.location.href = widgetBuilderUrl; });
    $('peDomainForm')?.addEventListener('submit', async e => {
        e.preventDefault();
        const input = $('peDomainInput');
        const domain = normalizeDomain(input.value);
        text('peDomainError', !state.selected ? 'Select a widget first.' : !domain ? 'Enter a valid hostname, such as example.com.' : state.domains.some(x => (typeof x === "string" ? x : x.domain) === domain) ? 'Domain already configured.' : '');
        if (!state.selected || !domain || state.domains.some(x => (typeof x === "string" ? x : x.domain) === domain)) return;
        await saveDomain(domain);
        if (!state.domainBusy && state.domains.some(x => (typeof x === "string" ? x : x.domain) === domain)) input.value = '';
    });
    checkboxes.forEach(x => x.addEventListener('change', updateChecklist));
    $('peChecklistNext')?.addEventListener('click', () => { $('pePublishBtn')?.scrollIntoView({ behavior: 'smooth', block: 'center' }); $('pePublishBtn')?.focus({ preventScroll: true }); });
    renderPlatform('html'); renderDomains(); updateChecklist(); renderStatus();
    console.info('[AskPearls Publish] Initializing widget list:', endpoint);
    loadWidgets();
})();
